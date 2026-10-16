#!/usr/bin/env bash
#
# test.sh — esegue la suite di test cross-platform di RCDWeb
#
# I test girano su Linux, macOS e Windows con .NET SDK 6+.
# Includono i test di regressione di sicurezza (criteri C1, C2, C7, C10, C3, C4
# della rubrica in PLAN.md), che leggono il repository e richiedono `git`.
#
# Uso:
#   ./scripts/test.sh                       # tutti i test
#   ./scripts/test.sh --filter C1           # solo i test di un criterio
#   ./scripts/test.sh "" Release            # configurazione Release
#
set -euo pipefail

FILTER="${1:-}"
CONFIGURATION="${2:-Debug}"
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

cd "$ROOT"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "ERRORE: 'dotnet' non trovato. Installare .NET SDK 6+ oppure usare Dockerfile.build." >&2
  exit 1
fi

if ! command -v git >/dev/null 2>&1; then
  echo "AVVISO: 'git' non trovato. I test di regressione C7 falliranno." >&2
fi

echo "==> RCDWeb test — configurazione: $CONFIGURATION"

ARGS=(
  test RCDWeb.CrossPlatform.sln
  --configuration "$CONFIGURATION"
  --nologo
  --verbosity quiet
)

if [[ -n "$FILTER" ]]; then
  ARGS+=(--filter "$FILTER")
fi

dotnet "${ARGS[@]}"

echo
echo "==> Test terminati."
