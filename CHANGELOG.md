# Changelog

Tutte le modifiche rilevanti sono documentate in questo file.

Il formato segue [Keep a Changelog](https://keepachangelog.com/it/1.1.0/);
il progetto aderisce a [Semantic Versioning](https://semver.org/lang/it/).

## [Unreleased]

### Added

- **Cross-platform**: `Directory.Build.props`, `RCDWeb.CrossPlatform.sln`, `Dockerfile.build`
  (`.NET SDK 8` + Mono/MSBuild), script POSIX `scripts/{restore,build,test}.sh`,
  workflow GitHub Actions con matrice `ubuntu-latest` / `macos-latest` / `windows-latest`.
- **`src/RCDWeb.Domain`**: libreria di dominio indipendente dalla piattaforma
  (`TargetFrameworks=netstandard2.0;net45`), senza dipendenze da `System.Web`:
  `DateUtil`, `StringExtensions`, `MapObj` e l'astrazione `IErrorReporter`
  (sostituisce `Elmah.ErrorSignal`).
- **`tests/RCDWeb.Domain.Tests`**: 41 test xUnit su `net8.0`, eseguibili su Linux, macOS e
  Windows senza IIS e senza database. Includono le guardie di regressione dei criteri
  C1, C2, C3, C4, C7 e C10, verificate anche in negativo.
- `docs/evidence/`: log di build e servizio di evidenze per ogni fase.
- `README.md`: sezione 3.3 "Build cross-platform" con tabella di cosa gira e cosa no su
  ciascun sistema operativo.
- `PLAN.md §8ter` — Track B cross-platform con rubrica v2 (CP1–CP6).

### Changed

- La verifica dei criteri di sicurezza passa da comandi `grep` manuali a test automatici
  eseguiti in CI su tre sistemi operativi.
- `DateUtil`, `StringExtensions` e il mapping oggetti sono portati senza dipendenze da ELMAH
  né da `System.Web`; gli errori sono segnalati tramite `IErrorReporter`.
- `README.md §9.5` e `docs/WORKSTATE.json` riportano lo stato reale della rubrica (9/10),
  e `C9` è marcato `partial` finché la prima run GitHub non è verde.

---

## [Unreleased] — ciclo 1 (sicurezza e documentazione)

### Security

- Rimosse **tutte** le credenziali in chiaro dai file tracciati: stringhe di connessione SQL
  (`Source/connection.config`, `RCDJob/ConsoleRCDJob/ConsoleRCDJob/App.config`,
  `RCDWeb.Job/bak/connection.config`), password dei wrapper di web service e del server SMTP
  (`Source/Web.config`), credenziali nelle connection string EF commentate e nei metadati di
  progetto (`Source/RCDWeb.csproj`).
- Rimossi tutti i riferimenti a infrastruttura interna: hostname, indirizzi IP privati, domini
  aziendali, share UNC, URL SSO e Help Desk, endpoint TFS.
- Rimossi account nominali dalla sezione `<location path="elmah.axd">`; l'accesso è ora
  regolato dal ruolo `Admin`.
- ELMAH: `elmah.mvc.requiresAuthentication` da `false` a `true`, `allowedRoles` da `*` a `Admin`,
  `allowedUsers` da `*` a vuoto, `<security allowRemoteAccess>` da `1` a `0`.
- `customErrors` da `Off` a `RemoteOnly` con redirect su `~/Error`.
- Rimossi dati personali dai dati di seed (`InitialCreate`), dai metadati di assembly e dal footer.
- Neutralizzati marchi e loghi aziendali (`logo_Acme.*` → `logo_placeholder.*`).

### Added

- `LICENSE` (MIT).
- `README.md`: guida completa (panoramica, stack, requisiti, installazione, configurazione,
  uso, test, troubleshooting, licenza e sicurezza).
- `PLAN.md`: piano di recupero con diagnosi, rubrica di valutazione a 10 criteri e 8 fasi.
- `PROMPT.md`: prompt operativo per l'esecuzione automatica delle fasi.
- `SECURITY.md`, `CONTRIBUTING.md`, `CHANGELOG.md`, `THIRD-PARTY.md`, `lib/README.md`.
- `docs/WORKSTATE.json`: stato di avanzamento machine-readable.
- `docs/evidence/`: evidenze riproducibili per ogni fase.
- Nuove chiavi di configurazione esternalizzate: `SsoPortalUrl`, `HelpDeskUrl`, `DocFilesPath`.

### Changed

- URL SSO e link Help Desk esternalizzati in `appSettings` anziché hardcoded nel codice e nelle viste.

### Removed

- Bindings di controllo del codice sorgente TFS dai file di progetto.
- **Binari non ridistribuibili dal versionamento**: `AreaCommonWeb.dll`, `WebServiceModel.dll`,
  `Kendo.Mvc.dll` (+ assembly di risorse), `Telerik.Web.Mvc.dll`, `itextsharp.dll` (AGPL),
  `Microsoft.Office.Interop.Word.dll`, `Microsoft.Web.Services3.dll`, `lib.zip`.
  Restano usabili in locale (`.gitignore`) e sono documentati in `lib/README.md` e
  `THIRD-PARTY.md`.
- **Codice morto**: `old/` (48 file), `BuildProcessTemplates/` (3 template TFS),
  `RCDWeb.Job/bak/`, `_RCDContext.cs`, `_TabelleDB.cs`, `_RCDRepository.cs`,
  `_StampaDocumenti.cs`, `StampaDocumenti_old.cs`, `Details_old.cshtml`,
  `IndexJqueryTable - Old.cshtml`.
- Readme del template Visual Studio `Source/readme.txt` (contenuto assorbito nel README).

---

## Cronologia precedente

Il repository non aveva un changelog strutturato. La cronologia è nei commit Git.
