#!/usr/bin/env bash
# Post-deploy check of DAIKON's auth and header rules, run from OUTSIDE the backend network.
#
#   ./smoke-test.sh <gateway-url> <frontend-origin> [backend-host]
#   ./smoke-test.sh https://daikon-api.example.org https://daikon.example.org daikon-host.example.org
#
# backend-host (optional): the machine running the containers; every service port on it must be unreachable.
# Exits non-zero if any check fails.
set -uo pipefail

GW="${1:?usage: $0 <gateway-url> <frontend-origin> [backend-host]}"
FE="${2:?usage: $0 <gateway-url> <frontend-origin> [backend-host]}"
HOST="${3:-}"
GW="${GW%/}"
failures=0

check() { # check <description> <condition-exit-code>
  if [ "$2" -eq 0 ]; then echo "PASS  $1"; else echo "FAIL  $1"; failures=$((failures + 1)); fi
}
has() { grep -qi "^$1" <<<"$headers"; }

# 1. No token: 401 with WWW-Authenticate, no-store and the security headers, and no Server header
headers="$(curl -s -D - -o /dev/null --max-time 15 "$GW/v2/gene" | tr -d '\r')"
check "no token -> 401"                        "$(grep -q '^HTTP/[0-9.]* 401' <<<"$headers"; echo $?)"
check "401 has WWW-Authenticate: Bearer"       "$(has 'www-authenticate: Bearer'; echo $?)"
check "Cache-Control: no-store"                "$(has 'cache-control: no-store'; echo $?)"
check "Content-Security-Policy"                "$(has "content-security-policy: default-src 'none'; frame-ancestors 'none'"; echo $?)"
check "X-Frame-Options: DENY"                  "$(has 'x-frame-options: DENY'; echo $?)"
check "X-Content-Type-Options: nosniff"        "$(has 'x-content-type-options: nosniff'; echo $?)"
check "Strict-Transport-Security"              "$(has 'strict-transport-security: max-age='; echo $?)"
check "no Server header"                       "$(! has 'server:'; echo $?)"

# 2. CORS: the frontend origin is allowed, any other origin is not
headers="$(curl -s -D - -o /dev/null --max-time 15 -X OPTIONS "$GW/v2/gene" \
  -H "Origin: $FE" -H "Access-Control-Request-Method: GET" -H "Access-Control-Request-Headers: authorization" | tr -d '\r')"
check "preflight from $FE allowed"             "$(has "access-control-allow-origin: $FE"; echo $?)"
headers="$(curl -s -D - -o /dev/null --max-time 15 -X OPTIONS "$GW/v2/gene" \
  -H "Origin: https://not-daikon.example" -H "Access-Control-Request-Method: GET" | tr -d '\r')"
check "preflight from another origin refused"  "$(! has 'access-control-allow-origin'; echo $?)"

# 3. The service health page needs a signed-in user
status="$(curl -s -o /dev/null -w '%{http_code}' --max-time 15 "$GW/health/services")"
check "/health/services without a token -> 401" "$([ "$status" = 401 ]; echo $?)"

# 4. Service ports are not reachable from outside (only the gateway is public)
if [ -n "$HOST" ]; then
  for port in 8001 8002 8003 8004 8005 8006 8007 8008 8009 8011 8012 8014 8015 8016 8017 10001 10002; do
    curl -s -o /dev/null --max-time 5 "http://$HOST:$port/" 2>/dev/null
    reached=$?
    check "port $port on $HOST unreachable" "$([ "$reached" -ne 0 ]; echo $?)"
  done
fi

echo
[ "$failures" -eq 0 ] && echo "All checks passed." || echo "$failures check(s) failed."
exit "$failures"
