# Track B — Cross-platform

Data: 2026-10-16T11:00:00Z
Variabile modificata: **architettura di build, dominio e test resa indipendente dalla piattaforma**.
Criteri target: CP1–CP6 (rubrica v2, `PLAN.md §8ter`)

Richiesta dell'utente: *"rendi cross-platform"*.

## Baseline e vincolo tecnico dichiarato

Prima di questa fase:

- host senza `dotnet`, `mono`, `msbuild`, `nuget`;
- nessuna pipeline di build eseguibile;
- nessun test;
- dominio e utility accoppiati a `System.Web` e a ELMAH (`ErrorSignal.FromCurrentContext()`);
- asset e binari Windows-only (`Office.Interop.Word`, WSE 3.0, Telerik/Kendo).

**Vincolo tecnico non aggirabile:** ASP.NET MVC 4 dipende da `System.Web`, disponibile solo su
.NET Framework/Windows. Mono offre un supporto parziale e deprecato. Quindi:

| Ambito | Cross-platform senza porting? |
|---|---|
| Build dei progetti moderni (`src/`, `tests/`) | **sì** |
| Esecuzione dei test | **sì** |
| Script di build | **sì** |
| Compilazione dei progetti legacy `.NET Framework 4.5` | **parziale** (Mono su Linux/macOS) |
| **Esecuzione del sito web** | **no** — richiede porting ad ASP.NET Core |

Questo limite è dichiarato in `README.md §3.3`, in `PLAN.md §8ter` e in `docs/WORKSTATE.json`.

## Modifica

### 1. Dominio estratto in una libreria indipendente dalla piattaforma

`src/RCDWeb.Domain` con `<TargetFrameworks>netstandard2.0;net45</TargetFrameworks>`:

| Classe | Dipendenza rimossa |
|---|---|
| `Utility/DateUtil.cs` | `Elmah.ErrorSignal.FromCurrentContext().Raise` → `IErrorReporter` |
| `Utility/StringExtensions.cs` | `using System.Web` (inutilizzato) |
| `Utility/MapObj.cs` | `Elmah` + `Newtonsoft.Json` (inutilizzato) |
| `Abstractions/IErrorReporter.cs` | nuovo: `IErrorReporter`, `NullErrorReporter`, `ErrorReporter` |

Il doppio target `net45` è compilabile **anche su Linux** grazie a
`Microsoft.NETFramework.ReferenceAssemblies` (in `Directory.Build.props`), quindi il progetto
potrà essere referenziato dal tier web legacy senza cambiare il target framework.

### 2. Test indipendenti da IIS e dal database

`tests/RCDWeb.Domain.Tests` (`net8.0`, xUnit): 41 test, 0 dipendenze da IIS/SQL Server/git
(tranne la guardia C7, che usa `git ls-files` e suona un errore esplicito se git manca).

### 3. Pipeline di build e CI

`scripts/restore.sh`, `scripts/build.sh`, `scripts/test.sh` (POSIX), `Dockerfile.build`
(`.NET SDK 8` + `mono-complete` + `msbuild` + `nuget` + `git`, utente non-root, locale UTF-8),
`.github/workflows/ci.yml` con matrice `ubuntu-latest` / `macos-latest` / `windows-latest`.

## Verifica

### Compilazione dei due target su Linux

```
RCDWeb.Domain -> /src/src/RCDWeb.Domain/bin/Release/netstandard2.0/RCDWeb.Domain.dll
RCDWeb.Domain -> /src/src/RCDWeb.Domain/bin/Release/net45/RCDWeb.Domain.dll
Build succeeded. 0 Warning(s) 0 Error(s)
```

Il target `net45` compilato su Linux è la prova che la libreria resta consumabile dal progetto
web legacy.

### Esecuzione dei test su Linux

```
Passed! - Failed: 0, Passed: 41, Skipped: 0, Total: 41
```

Ripetuto 3 volte su checkout pulito: 41/41 ogni volta.

### Struttura verificata

```bash
$ docker run --rm -v "$PWD:/src" -w /src mcr.microsoft.com/dotnet/sdk:8.0 ./scripts/build.sh Release
==> [1/2] Restore + build della soluzione cross-platform
==> [2/2] Build legacy (.NET Framework 4.5)
    msbuild non disponibile: build legacy saltata.
```

Il messaggio è intenzionale: senza `Dockerfile.build` (che include Mono) la build legacy è
saltata **senza** far fallire la pipeline cross-platform.

Esito: **PASS** per CP2, CP3, CP4, CP5. CP1 verificato solo su Linux in container (matrice
GitHub/macOS/Windows definita ma non eseguita). CP6 **non soddisfatto**, per limite tecnico.

## Criteri aggiornati (rubrica v2)

| Criterio | Stato |
|---|---|
| CP1 — build su Linux, macOS, Windows | 🟡 matrice definita; verificata su Linux |
| CP2 — dominio senza dipendenze di piattaforma | 🟢 |
| CP3 — test senza IIS e database | 🟢 |
| CP4 — script POSIX | 🟢 |
| CP5 — configurazione per variabili/segnaposto | 🟢 |
| CP6 — tier web su Linux | 🔴 richiede porting ad ASP.NET Core |

## Cosa serve per CP6 (non eseguito in questo ciclo)

Porting del tier web, per incrementi verificabili:

1. **Host ASP.NET Core** (`net8.0`) accanto al progetto web legacy, che riusa `RCDWeb.Domain`.
2. **EF Core** con la stessa mappa di `RCDContext` (28 entità) verso SQL Server o PostgreSQL.
3. **Autenticazione**: sostituire `UserProfileManager`/`AreaCommonWeb` con un provider utenti
   configurabile (Windows/Negotiate, OIDC o tabella locale).
4. **Viste**: convertire le 56 `.cshtml` e rimuovere gli helper Telerik/Kendo.
5. **Documenti**: `DocumentFormat.OpenXml` al posto di `Office.Interop.Word`;
   QuestPDF/PdfSharpCore al posto di `itextsharp`.
6. **Job**: `ConsoleRCDJob` → `Microsoft.Extensions.Hosting` (`BackgroundService`).

Ogni incremento è misurabile con la stessa rubrica: compilazione verde sui 3 OS e test verdi.

## Prossima fase

FASE 7 — Storia git e pubblicazione (richiede la fornitura delle credenziali di push).
