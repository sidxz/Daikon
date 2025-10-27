#!/usr/bin/env bash
# Fancy sequential docker compose builder (no confirm)
set -euo pipefail

# --- Config ---
DEFAULT_FILES="-f docker-compose.yml -f docker-compose.override.yml"

# Colors/icons (fallback if no TTY colors)
if [[ -t 1 ]] && tput colors &>/dev/null; then
  BOLD="$(tput bold)"; DIM="$(tput dim)"; OFF="$(tput sgr0)"
  RED="$(tput setaf 1)"; GREEN="$(tput setaf 2)"; YELLOW="$(tput setaf 3)"
  BLUE="$(tput setaf 4)"; MAGENTA="$(tput setaf 5)"; CYAN="$(tput setaf 6)"
else
  BOLD=""; DIM=""; OFF=""; RED=""; GREEN=""; YELLOW=""; BLUE=""; MAGENTA=""; CYAN=""
fi
CHECK="✅"; CROSS="❌"; DOT="•"; BOX_TL="┌"; BOX_TR="┐"; BOX_BL="└"; BOX_BR="┘"; BOX_H="─"; ARROW="➜"

# --- Arg parsing ---
FILES="$DEFAULT_FILES"
BUILD_ARGS=()
PASS_THRU=0

while (( "$#" )); do
  case "$1" in
    -h|--help)
      echo "Usage: $0 [--] [docker compose build args...]"
      echo "Examples:"
      echo "  $0"
      echo "  $0 -- --pull --no-cache"
      echo "  $0 -- -f docker-compose.yml -f docker-compose.prod.yml"
      exit 0
      ;;
    --)
      PASS_THRU=1
      shift
      BUILD_ARGS=("$@")
      FILES="" # trust user-provided -f flags in BUILD_ARGS
      break
      ;;
    *)
      BUILD_ARGS+=("$1")
      shift
      ;;
  esac
done

# --- Resolve services (avoid arrays/mapfile) ---
if [[ -n "$FILES" ]]; then
  SERVICES_RAW="$(eval "docker compose $FILES config --services")" || {
    echo "${RED}${CROSS} Failed to list services.${OFF}"; exit 1; }
  COMPOSE_DESC="$FILES"
else
  # User supplies -f flags via BUILD_ARGS
  SERVICES_RAW="$(docker compose "${BUILD_ARGS[@]}" config --services)" || {
    echo "${RED}${CROSS} Failed to list services (from provided flags).${OFF}"; exit 1; }
  COMPOSE_DESC="(from provided -f flags)"
fi

# Normalize service list to lines
SERVICES="$(printf '%s\n' "$SERVICES_RAW" | sed '/^[[:space:]]*$/d')"
COUNT="$(printf '%s\n' "$SERVICES" | wc -l | tr -d '[:space:]')"
if [[ "$COUNT" -eq 0 ]]; then
  echo "${RED}${CROSS} No services found.${OFF}"
  exit 1
fi

# Compute max width for pretty columns
MAXLEN=0
while IFS= read -r s; do
  [[ ${#s} -gt $MAXLEN ]] && MAXLEN=${#s}
done <<< "$SERVICES"

box() {
  local title="$1"; shift || true
  local pad="$1"; [[ -z "${pad:-}" ]] && pad=$(( ${#title} + 6 ))
  printf "%s" "${CYAN}${BOX_TL}"
  printf "%*s" "$pad" "" | tr ' ' "${BOX_H}"
  printf "%s\n" "${BOX_TR}${OFF}"
  printf "${CYAN}│${OFF} ${BOLD}%s${OFF}\n" "$title"
  printf "%s" "${CYAN}${BOX_BL}"
  printf "%*s" "$pad" "" | tr ' ' "${BOX_H}"
  printf "%s\n" "${CYAN}${BOX_BR}${OFF}"
}

# --- Show detected services (numbered) ---
box "Docker Compose services detected ($COUNT)  ${DIM}${COMPOSE_DESC}${OFF}" $(( MAXLEN + 30 ))
i=0
while IFS= read -r s; do
  i=$(( i + 1 ))
  printf "  %s %2d/%-2d. ${BOLD}%-*s${OFF} %s\n" "$DOT" "$i" "$COUNT" "$MAXLEN" "$s" "${DIM}(queued)${OFF}"
done <<< "$SERVICES"
echo
echo "${DIM}${ARROW} Build args:${OFF} ${YELLOW}${BUILD_ARGS[*]:-(none)}${OFF}"
echo
box "Starting sequential build ($COUNT services)" $(( MAXLEN + 28 ))

# --- Build loop (sequential) ---
i=0
while IFS= read -r svc; do
  i=$(( i + 1 ))
  echo
  printf "%s %s %s %s (%d/%d)\n" "${BLUE}${ARROW}${OFF}" "${BOLD}Building${OFF}" "${MAGENTA}${svc}${OFF}" "${DIM}${COMPOSE_DESC}${OFF}" "$i" "$COUNT"
  echo "${DIM}docker compose ${FILES:-${BUILD_ARGS[*]}} build ${BUILD_ARGS[*]:-} ${svc}${OFF}"

  # Don't exit the whole script if a single build fails; report nicely
  set +e
  if [[ -n "$FILES" ]]; then
    eval docker compose $FILES build "${BUILD_ARGS[@]}" "$svc"
  else
    docker compose build "${BUILD_ARGS[@]}" "$svc"
  fi
  rc=$?
  set -e

  if [[ $rc -ne 0 ]]; then
    echo "${RED}${CROSS} Build failed for: ${svc}${OFF}"
    exit $rc
  fi
  echo "${GREEN}${CHECK} Built:${OFF} ${BOLD}${svc}${OFF}"
done <<< "$SERVICES"

echo
box "All builds completed successfully ${CHECK}" $(( MAXLEN + 34 ))
