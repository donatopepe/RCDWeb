# Fase 00 — Baseline rubric (INIT)

Data: 2026-10-16T07:40:00Z
Variabile modificata: nessuna (misurazione)
Criteri target: C1–C10

## Comandi eseguiti

```bash
cd /home/public/RCDWeb
grep -rniE "password|pwdXXX|pwdXXX|User ID=|Uid=" --include=*.config --include=*.cs --include=*.csproj --include=*.bat . | grep -v '\.generated\.cs' | wc -l
grep -rn "localhost\|example\.it\|172\.28\.\|svc-rcd\|removed[0-9]" . | grep -v '^./.git/' | wc -l
test -f LICENSE; test -f README.md
ls docs/evidence/build-*.log
ls tests
git ls-files lib/ | grep -cE "Kendo|Telerik|AreaCommonWeb|WebServiceModel|itextsharp|Interop.Word|Services3|lib.zip"
test -f scripts/build.sh
ls .github/workflows/
test -d old
```

## Output

| Criterio | Check | Valore misurato | Stato |
|---|---|---|---|
| C1 | occorrenze pattern segreti | **35** | 🔴 red |
| C2 | occorrenze infrastruttura aziendale | **49** | 🔴 red |
| C3 | `LICENSE` | assente | 🔴 red |
| C4 | `README.md` | assente | 🔴 red |
| C5 | log di build | 0 file | 🔴 red |
| C6 | progetti di test | 0 | 🔴 red |
| C7 | binari non ridistribuibili tracciati in `lib/` | **58** | 🔴 red |
| C8 | `scripts/build.sh` | assente | 🔴 red |
| C9 | workflow CI | 0 | 🔴 red |
| C10 | `old/` presente | 48 file | 🔴 red |

## Esito

**Score baseline = 0/10.** Divario dalla soglia: **-10**.

Toolchain host: `dotnet`, `mono`, `msbuild`, `nuget` **assenti**. `docker` presente e
funzionante. Rete verso `api.nuget.org` OK (HTTP 200). → La build dovrà passare da container.

Repository pubblico (`origin` = `https://github.com/donatopepe/RCDWeb.git`, branch `master`,
HEAD `f02045f`) contenente credenziali di produzione in chiaro: **incidente di sicurezza
già avvenuto** — la sanitizzazione del working tree non annulla l'esposizione.
Vedi `user_actions_required` in `docs/WORKSTATE.json`.

## Prossima fase

FASE 0 — Security (C1, C2).
