#!/bin/sh
# Configuration and secret hygiene: fails when credentials or environment-specific configuration would land in git
# or inside an image. Everything environment-specific (tenant, client id, origins, connection strings, service URLs)
# belongs in the deployment's environment variables, never in tracked files or image layers.
#
#   src/check-config-hygiene.sh repo             # tracked files; CI test stage and .githooks/pre-commit run this
#   src/check-config-hygiene.sh image <image>    # a built image's /app; CI runs this before pushing
#
# Allowed: ${VAR} templates and empty values in compose/YAML, and the placeholder ids the tests use.
# POSIX sh on purpose: CI's build jobs run it inside the Alpine docker image, which has no bash.
set -u
cd "$(dirname "$0")/.."

fail=0
say() { printf 'config-hygiene: %s\n' "$*" >&2; fail=1; }

placeholder_guids='(00000000-0000-0000-0000-000000000000|11111111-1111-1111-1111-111111111111|22222222-2222-2222-2222-222222222222|33333333-3333-3333-3333-333333333333)'
secret_keys='(ConnectionString|Password|Secret|ClientSecret|ApiKey|Token|PrivateKey)'

check_appsettings() {  # appsettings*.json: defaults only, no credentials, no real identity-provider configuration
  local f=$1
  grep -qE "\"[A-Za-z]*${secret_keys}[A-Za-z]*\"[[:space:]]*:[[:space:]]*\"[^\"]+\"" "$f" && say "$f: credential-like key with a value"
  grep -oE '"[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"' "$f" | grep -qvE "$placeholder_guids" && say "$f: real-looking tenant or client id"
  grep -E '[a-z0-9-]+\.onmicrosoft\.com' "$f" | grep -qv 'example\.onmicrosoft\.com' && say "$f: Entra domain"
  grep -qE '(mongodb|bolt|neo4j|amqp|postgres|redis)(\+srv)?://[^"/]*@' "$f" && say "$f: connection string with credentials"
  return 0
}

check_yaml() {  # compose, Helm values, CI: credentials must be ${templated} or empty
  local f=$1
  grep -nE '(PASSWORD|SECRET|TOKEN|_AUTH|CONNECTIONSTRING|ConnectionString|Password|Secret)[A-Za-z_]*[[:space:]]*[:=][[:space:]]*[^$[:space:]"]' "$f" \
    | grep -vE '\$\{|\$[A-Z_]' | grep -qv '^$' && say "$f: literal credential; use \${VAR}"
  return 0
}

repo() {
  local never
  never=$(git ls-files | grep -iE '(^|/)\.env($|\.)|\.(pfx|pem|key|p12|jks)$|(^|/)secrets[^/]*\.json$|^internal-docs/|^CLAUDE\.md$' | grep -vE '\.env\.example$' || true)
  [ -n "$never" ] && say "tracked but must never be: $(printf '%s ' $never)"
  for f in $(git ls-files -- '*appsettings*.json'); do check_appsettings "$f"; done
  for f in $(git ls-files -- '*.yml' '*.yaml'); do check_yaml "$f"; done
  for f in $(git ls-files -- '*launchSettings.json'); do
    grep -qE "\"[A-Za-z_:]*${secret_keys}[A-Za-z_:]*\"[[:space:]]*:[[:space:]]*\"[^\"]+\"" "$f" && say "$f: credential in launch settings"
  done
}

image() {
  local ref=$1 dir cid
  dir=$(mktemp -d)
  cid=$(docker create "$ref") || { say "cannot create a container from $ref"; return; }
  docker cp "$cid:/app" "$dir/app" >/dev/null
  docker rm "$cid" >/dev/null
  local never
  never=$(cd "$dir" && find app \( -name '.env*' -o -name '*.pfx' -o -name '*.pem' -o -name '*.key' -o -name '*.p12' -o -name 'secrets*.json' -o -name 'launchSettings.json' -o -name '*.user' \) || true)
  [ -n "$never" ] && say "$ref: must not be in the image: $(printf '%s ' $never)"
  cd "$dir"
  for f in $(find app -name 'appsettings*.json'); do check_appsettings "$f"; done
  cd / && rm -rf "$dir"
}

case "${1:-}" in
  repo) repo ;;
  image) [ -n "${2:-}" ] || { echo "usage: $0 image <image>" >&2; exit 2; }; image "$2" ;;
  *) echo "usage: $0 repo | image <image>" >&2; exit 2 ;;
esac

if [ "$fail" -ne 0 ]; then
  echo "config-hygiene: FAILED" >&2
  exit 1
fi
echo "config-hygiene: ok (${1})"
