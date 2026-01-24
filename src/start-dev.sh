#!/usr/bin/env bash
set -e

# Load environment variables from .env.local
if [[ -f ".env.local" ]]; then
  echo "Loading environment variables from .env.local..."
  env_args=()
  while IFS='=' read -r key value; do
    # skip empty or comment lines
    [[ -z "$key" || "$key" =~ ^# ]] && continue
    # strip quotes
    value="${value%\"}"
    value="${value#\"}"
    env_args+=("$key=$value")
  done < .env.local
else
  echo "No .env.local file found!"
fi

# Run the .NET project with hot reload
echo "Starting .NET project with dotnet watch..."
#env "${env_args[@]}" dotnet watch run
