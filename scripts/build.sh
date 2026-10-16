#!/usr/bin/env bash
#
# build.sh — build cross-platform di RCDWeb
#
# Compila i progetti .NET moderni (src/, tests/) con qualsiasi .NET SDK 6+.
# Se `msbuild` (Mono) è disponibile, tenta anche la build dei progetti legacy
# .NET Framework 4.5; i fallimenti dei progetti legacy non sono bloccanti.
#
# Uso:
#   ./scripts/build.sh              # build Debug
#   ./scripts/build.sh Release      # build Release
#
set -euo pipefail

CONFIGURATION="${1:-Debug}"
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

cd "$ROOT"

echo "==> RCDWeb build — configurazione: $CONFIGURATION"
echo "==> Radice: $ROOT"

# ---------------------------------------------------------------------------
# 1) Progetti cross-platform (.NET SDK)
# ---------------------------------------------------------------------------
if ! command -v dotnet >/dev/null 2>&1; then
  echo "ERRORE: 'dotnet' non trovato. Installare .NET SDK 6+ oppure usare Dockerfile.build." >&2
  exit 1
fi

echo
echo "==> [1/2] Restore + build della soluzione cross-platform"
dotnet build RCDWeb.CrossPlatform.sln \
  --configuration "$CONFIGURATION" \
  --nologo \
  -p:ContinuousIntegrationBuild=true

# ---------------------------------------------------------------------------
# 2) Progetti legacy (.NET Framework 4.5) — opzionale
# ---------------------------------------------------------------------------
echo
echo "==> [2/2] Build legacy (.NET Framework 4.5)"

if ! command -v msbuild >/dev/null 2>&1; then
  echo "    msbuild non disponibile: build legacy saltata."
  echo "    Su Linux/macOS usare: docker run --rm -v \"\$PWD:/src\" rcdweb-build ./scripts/build.sh"
  echo "    Su Windows usare:     msbuild RCDWeb.sln /p:Configuration=$CONFIGURATION"
elif ! command -v nuget >/dev/null 2>&1; then
  echo "    nuget non disponibile: build legacy saltata."
else
  echo "    Ripristino pacchetti NuGet dei progetti legacy..."
  if nuget restore RCDWeb.sln -NonInteractive >/dev/null 2>&1; then
    echo "    Ripristino completato."

    if msbuild RCDWeb.sln \
         -p:Configuration="$CONFIGURATION" \
         -p:Platform="Any CPU" \
         -v:minimal \
         -nologo; then
      echo "    Build legacy completata."
    else
      echo "    ATTENZIONE: la build legacy è fallita."
      echo "    Causa tipica: dipendenze binarie non ridistribuibili assenti in lib/"
      echo "    (vedi lib/README.md). La build cross-platform è comunque valida."
    fi
  else
    echo "    ATTENZIONE: ripristino NuGet dei progetti legacy fallito. Build legacy saltata."
  fi
fi

echo
echo "==> Build terminata."
