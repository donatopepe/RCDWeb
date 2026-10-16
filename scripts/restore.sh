#!/usr/bin/env bash
#
# restore.sh — ripristino delle dipendenze
#
# 1. Pacchetti NuGet dei progetti cross-platform (.NET SDK)
# 2. Pacchetti NuGet dei progetti legacy (via nuget.exe/Mono, se disponibile)
#
# Uso:
#   ./scripts/restore.sh
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

echo "==> RCDWeb restore"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "ERRORE: 'dotnet' non trovato. Installare .NET SDK 6+ oppure usare Dockerfile.build." >&2
  exit 1
fi

echo
echo "==> [1/2] Ripristino cross-platform"
dotnet restore RCDWeb.CrossPlatform.sln --nologo

echo
echo "==> [2/2] Ripristino legacy (opzionale)"

if command -v nuget >/dev/null 2>&1; then
  if nuget restore RCDWeb.sln -NonInteractive; then
    echo "    Ripristino legacy completato."
  else
    echo "    ATTENZIONE: ripristino legacy fallito (pacchetti non disponibili o sorgenti non accessibili)."
  fi
else
  echo "    nuget non disponibile: ripristino legacy saltato."
fi

echo
echo "==> Restore terminato."
