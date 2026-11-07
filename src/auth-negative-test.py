#!/usr/bin/env python3
"""Every endpoint must refuse a bad token: run against a local or staging stack (services in Development, so Swagger is on).

For each route in each service's Swagger document, and for the chemvault routes MLogix uses, it sends:
no token, a non-JWT string, an unsigned (alg none) JWT, a JWT with the right tenant and audience signed by an
attacker's RSA key, and one HS256-signed with the audience as the secret (algorithm confusion).
Each request goes to the service itself and through the gateway. Every response must be 401 with no-store.

The host publishes no service port (docker-compose.override.yml exposes the gateway only, and behind Caddy not even
that), so run it inside the Docker network, where services answer by container name on port 80:

    docker run --rm --network daikon-be-net -v "$PWD:/src:ro" python:3.12-alpine sh -c \
      "apk add -q openssl && python /src/auth-negative-test.py --in-network --env /src/.env"

Against a staging stack that publishes service ports: --host <host> [--gateway <url>] [--chemvault <url>].
Needs python3 and openssl. Exits non-zero if any request is not refused.
"""
import argparse, base64, hashlib, hmac, json, os, subprocess, sys, tempfile, time, urllib.error, urllib.request

SERVICES = {  # service -> host port when a stack publishes service ports (the local one no longer does)
    "gene": 8001, "target": 8002, "screen": 8003, "hit-assessment": 8004, "project": 8005, "horizon": 8006,
    "mlogix": 8007, "questionnaire": 8008, "comment": 8009, "userstore": 8011, "event-history": 8012,
    "docustore": 8014, "event-management": 8015, "user-preferences": 8016, "aggregators": 8017,
}
CONTAINERS = {"hit-assessment": "hitassessment-api"}  # --in-network: container names are <service>-api except this one
IN_NETWORK = {"gateway": "http://simplegw-api", "chemvault": "http://daikon_chem_vault_api:10001"}
CHEMVAULT_ROUTES = [  # the routes MLogix calls (chemvault serves no Swagger in this mode)
    ("GET", "/molecules/by-id/{id}"), ("GET", "/molecules/by-name?name=x"), ("GET", "/molecules/by-name-exact?names=x"),
    ("GET", "/molecules/similarity?smiles=C"), ("GET", "/molecules/substructure?smiles=C"), ("POST", "/molecules/"),
    ("POST", "/molecules/batch"), ("POST", "/molecules/by-ids"), ("POST", "/molecules/by-smiles-list/"),
    ("POST", "/molecules/get-molecules-by-name-or-synonym-exact"), ("POST", "/molecules/batch-create-parents"),
    ("PUT", "/molecules/{id}"), ("DELETE", "/molecules/{id}"), ("GET", "/admet/{id}"), ("POST", "/admet/by-ids"),
    ("POST", "/admet/predict"), ("POST", "/admet/backfill"), ("GET", "/admet/backfill/status"), ("POST", "/molcal/cluster?cutoff=0.7"),
]
ANONYMOUS = {"/health", "/health/ready"}
FAKE_ID = "00000000-0000-0000-0000-000000000001"


def b64url(data: bytes) -> str:
    return base64.urlsafe_b64encode(data).rstrip(b"=").decode()


def read_env(path):
    values = {}
    if os.path.exists(path):
        for line in open(path):
            line = line.strip()
            if line and not line.startswith("#") and "=" in line:
                key, value = line.split("=", 1)
                values[key.strip()] = value.strip().strip('"').strip("'")
    return values


def bad_tokens(tenant, audience):
    now = int(time.time())
    claims = {"iss": f"https://login.microsoftonline.com/{tenant}/v2.0", "aud": audience, "tid": tenant,
              "oid": "11111111-2222-3333-4444-555555555555", "sub": "forged", "scp": "access_as_user",
              "name": "Forged User", "nbf": now - 60, "iat": now - 60, "exp": now + 3600}
    payload = b64url(json.dumps(claims).encode())

    def jwt(header, sign):
        signing_input = f"{b64url(json.dumps(header).encode())}.{payload}"
        return f"{signing_input}.{sign(signing_input.encode())}"

    with tempfile.TemporaryDirectory() as tmp:
        key = os.path.join(tmp, "attacker.pem")
        subprocess.run(["openssl", "genrsa", "-out", key, "2048"], check=True, capture_output=True)
        rs256 = jwt({"alg": "RS256", "typ": "JWT", "kid": "attacker"},
                    lambda data: b64url(subprocess.run(["openssl", "dgst", "-sha256", "-sign", key],
                                                       input=data, check=True, capture_output=True).stdout))
    return {
        "no token": None,
        "not a JWT": "Bearer not-a-jwt",
        "alg none": "Bearer " + jwt({"alg": "none", "typ": "JWT"}, lambda data: ""),
        "attacker RS256": "Bearer " + rs256,
        "HS256 confusion": "Bearer " + jwt({"alg": "HS256", "typ": "JWT"},
                                           lambda data: b64url(hmac.new(audience.encode(), data, hashlib.sha256).digest())),
    }


def swagger_routes(host, port):
    routes = set()
    for version in ("v1", "v2"):
        try:
            with urllib.request.urlopen(f"http://{host}:{port}/swagger/{version}/swagger.json", timeout=10) as r:
                doc = json.load(r)
        except Exception:
            continue
        for path, operations in doc.get("paths", {}).items():
            for method in operations:
                if method.upper() in ("GET", "POST", "PUT", "PATCH", "DELETE") and path not in ANONYMOUS:
                    routes.add((method.upper(), path))
    return sorted(routes)


def fill(path):
    out, depth = "", 0
    for ch in path:  # replace every {param} with a fake id
        if ch == "{":
            depth += 1
            if depth == 1:
                out += FAKE_ID
        elif ch == "}":
            depth -= 1
        elif depth == 0:
            out += ch
    return out


def call(method, url, authorization):
    body = b"{}" if method in ("POST", "PUT", "PATCH", "DELETE") else None
    request = urllib.request.Request(url, data=body, method=method)
    if body is not None:
        request.add_header("Content-Type", "application/json")
    if authorization:
        request.add_header("Authorization", authorization)
    try:
        with urllib.request.urlopen(request, timeout=15) as r:
            return r.status, r.headers
    except urllib.error.HTTPError as e:
        return e.code, e.headers
    except Exception as e:
        return f"error: {e.__class__.__name__}", {}


def main():
    here = os.path.dirname(os.path.abspath(__file__))
    parser = argparse.ArgumentParser()
    parser.add_argument("--host", default="localhost")
    parser.add_argument("--gateway", default=None, help="default http://localhost:8010, or the gateway container with --in-network")
    parser.add_argument("--chemvault", default=None, help="default http://localhost:10001, or the chemvault container with --in-network")
    parser.add_argument("--env", default=os.path.join(here, ".env"))
    parser.add_argument("--in-network", action="store_true",
                        help="running inside the daikon-be-net Docker network: services by container name on port 80")
    args = parser.parse_args()
    args.gateway = args.gateway or (IN_NETWORK["gateway"] if args.in_network else "http://localhost:8010")
    args.chemvault = args.chemvault or (IN_NETWORK["chemvault"] if args.in_network else "http://localhost:10001")

    env = read_env(args.env)
    tenant = env.get("EntraID_TenantId", "00000000-0000-0000-0000-000000000000")
    audience = env.get("EntraID_Audience", "api://daikon")
    tokens = bad_tokens(tenant, audience)

    targets = []  # (label, method, url)
    for service, host_port in SERVICES.items():
        host, port = (CONTAINERS.get(service, f"{service}-api"), 80) if args.in_network else (args.host, host_port)
        routes = swagger_routes(host, port)
        if not routes:
            print(f"!! {service}: no Swagger routes found at {host}:{port} (service down, or not in Development)")
            targets.append((service, "GET", f"http://{host}:{port}/swagger-missing"))
            continue
        for method, path in routes:
            targets.append((service, method, f"http://{host}:{port}{fill(path)}"))
            gateway_path = fill(path)[4:] if path.startswith("/api/") else fill(path)
            targets.append((f"gateway/{service}", method, f"{args.gateway.rstrip('/')}{gateway_path}"))
    for method, path in CHEMVAULT_ROUTES:
        targets.append(("chemvault", method, f"{args.chemvault.rstrip('/')}{fill(path)}"))

    failures, counts = [], {}
    for label, method, url in targets:
        for name, authorization in tokens.items():
            status, headers = call(method, url, authorization)
            counts[label] = counts.get(label, 0) + 1
            cache = headers.get("Cache-Control", "") if hasattr(headers, "get") else ""
            ok = status == 401 and (label == "chemvault" or "no-store" in cache)
            if not ok:
                failures.append(f"{label:24} {method:6} {url}  [{name}] -> {status} cache-control={cache!r}")

    for label in sorted(counts):
        print(f"{label:24} {counts[label]:5} requests")
    print(f"\n{sum(counts.values())} requests with {len(tokens)} kinds of bad token: {len(failures)} not refused")
    for line in failures[:60]:
        print("FAIL", line)
    sys.exit(1 if failures else 0)


if __name__ == "__main__":
    main()
