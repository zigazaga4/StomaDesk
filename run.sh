#!/usr/bin/env bash
# Builds StomaDesk and starts it on Linux: with Mono if installed, otherwise with Wine.
# Any arguments are passed to the app, for example:  ./run.sh --selftest
set -euo pipefail
cd "$(dirname "$0")"

dotnet build src/StomaDesk/StomaDesk.csproj -c Release -nologo -v quiet
exe=src/StomaDesk/bin/Release/net45/StomaDesk.exe

if command -v mono >/dev/null 2>&1; then
    exec mono "$exe" "$@"
elif command -v wine >/dev/null 2>&1; then
    exec wine "$exe" "$@"
else
    echo "Lipsește Mono. Instalați-l cu:  sudo apt install mono-complete" >&2
    exit 1
fi
