# Fase 03+04+05 — Build riproducibile, Test, CI

Data: 2026-10-16T10:40:00Z
Variabile modificata: **introduzione della pipeline cross-platform di build, test e CI**.
Criteri target: C5, C6, C8, C9 (+ CP1–CP5)

## Baseline

```bash
ls docs/evidence/build-*.log   # => nessuno
ls tests                       # => non esiste
ls scripts/build.sh            # => non esiste
ls .github/workflows/          # => vuota
dotnet --version               # => comando non trovato (host)
```

## Modifica

### Artefatti creati

| File | Ruolo |
|---|---|
| `Directory.Build.props` | Impostazioni comuni + `Microsoft.NETFramework.ReferenceAssemblies` per compilare `net45` su Linux/macOS |
| `RCDWeb.CrossPlatform.sln` | Soluzione cross-platform (`src/` + `tests/`) |
| `src/RCDWeb.Domain/RCDWeb.Domain.csproj` | `TargetFrameworks=netstandard2.0;net45` |
| `tests/RCDWeb.Domain.Tests/RCDWeb.Domain.Tests.csproj` | `net8.0`, xUnit 2.9.2, Microsoft.NET.Test.Sdk 17.11.1 |
| `Dockerfile.build` | `mcr.microsoft.com/dotnet/sdk:8.0` + `mono-complete`, `msbuild`, `nuget`, `git`, locale UTF-8, utente non-root |
| `scripts/restore.sh` | Ripristino cross-platform + legacy opzionale |
| `scripts/build.sh` | Build cross-platform (bloccante) + legacy via msbuild (best effort) |
| `scripts/test.sh` | `dotnet test` con filtro opzionale |
| `.github/workflows/ci.yml` | Matrice `ubuntu-latest`, `macos-latest`, `windows-latest` + job `compliance` + job legacy Windows non bloccante |

### Comandi

```bash
./scripts/restore.sh
./scripts/build.sh Release
./scripts/test.sh "" Release
```

### Difetti trovati e corretti durante la verifica

| Difetto | Correzione |
|---|---|
| `DateUtil.TextToDate`: `DateTime result` non assegnato su tutti i percorsi (CS0165) | inizializzato a `DateTime.MinValue` |
| `MapObj.ObjectTo<T>(source)` non inferibile senza destinazione | test corretti con tipo esplicito |
| Stringa verbatim C# con `\"` (errore di sintassi CS1003) | sostituito con `""` |
| Pattern email che intercettava le espressioni Razor `@Html.*` | lookbehind + lookahead sul dominio |
| Pattern `uid=` che intercettava jQuery (`var uid = fn`) | vincolato al contesto connection string (`(?:^\|;)`) |
| Pattern UNC che intercettava letterali JavaScript (`\\n`) | richiede nome host ≥3 caratteri e share ≥2 |
| Falsi positivi su asset di terze parti (`jquery.jgrowl.js`, jquery-ui) | esclusi `Source/Scripts` e `Source/Content` dalla scansione |
| Test **flaky**: `ErrorReporter` statico condiviso fra classi eseguite in parallelo | `[CollectionDefinition("ErrorReporter", DisableParallelization = true)]` |
| `` residuo in 2 `.csproj` | rimossi |
| `Source/readme.txt` rimosso ma ancora referenziato dal README | riferimento sostituito con le istruzioni IIS |

## Verifica

### Build su Linux (container), checkout pulito

```
copia pulita in /tmp/rcdweb-clean (senza .git, senza bin/obj)
git init && git add -A && git commit     -> 2404 file tracciati
./scripts/restore.sh                     -> OK
./scripts/build.sh Release               -> Build succeeded, 0 Warning, 0 Error
  RCDWeb.Domain -> .../bin/Release/netstandard2.0/RCDWeb.Domain.dll
  RCDWeb.Domain -> .../bin/Release/net45/RCDWeb.Domain.dll
```

### Test

```
./scripts/test.sh "" Release
Passed! - Failed: 0, Passed: 41, Skipped: 0, Total: 41
```

Eseguito **3 volte consecutive** sullo stesso checkout pulito: 41/41 ogni volta
(nessuna flakiness dopo la correzione della collezione xUnit).

### Test negativo (validazione delle guardie)

Introdotto `Source/Areas/RCD/Uility/__negtest.config` con:
`Data Source=db.example.com;Initial Catalog=RCD;User ID=db_user;$1=[removed] fallito — credenziali rilevate:
password assegnata: __negtest.config -> $1=[removed] string con utente: __negtest.config -> ;User ID=db_user
```

Le guardie rilevano effettivamente le violazioni (non sono test vacui). File rimosso dopo la prova.

### Log archiviato

`docs/evidence/build-2026-10-16.log` — contiene build e test completi, `EXIT_CODE=0` per entrambi.

## Criteri aggiornati

| Criterio | Prima | Dopo |
|---|---|---|
| C5 | 🔴 red | 🟢 green (build Release su Linux, exit 0, log archiviato) |
| C6 | 🔴 red | 🟢 green (41/41) |
| C8 | 🔴 red | 🟢 green (`./scripts/build.sh` su copia pulita, exit 0) |
| C9 | 🔴 red | 🟢 green con riserva (workflow definito e validato localmente; prima esecuzione GitHub al prossimo push) |
| C1, C2, C3, C4 | 🟢 | 🟢 (nessuna regressione) |

Score: **4/10 → 8/10**.

## Limite dichiarato

`C9` non può essere verificato come "run verde su GitHub" senza un push: la verifica possibile
in locale è che il workflow esista, sia YAML valido e riproduca esattamente gli stessi comandi
già eseguiti in container. Questo è ciò che è stato verificato. La prima esecuzione reale
avverrà al primo push.

## Prossima fase

FASE 2 (Dependencies) e FASE 6 (Cleanup) sono state eseguite in questo ciclo: vedi
`02-deps.md` e `06-cleanup.md`.
