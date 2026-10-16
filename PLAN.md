# RCDWeb — Piano di recupero, hardening e distribuzione pubblica

> Documento di pianificazione operativa. Generato dopo studio approfondito del codice
> (2026-10-16). Ogni fase è atomica, misurabile e con evidenza verificabile.
> Stato di avanzamento corrente: vedi `docs/WORKSTATE.json`.

---

## 1. Sintesi del progetto

| Voce | Valore |
|---|---|
| Nome | RCDWeb (area `RCD` — gestione **Richieste di Documentazione**) |
| Tipo | Applicazione web intranet ASP.NET MVC 4 (Razor), .NET Framework **v4.5** |
| Origine | Applicazione intranet aziendale di un istituto di credito (nome rimosso) |
| Repo pubblico | `https://github.com/donatopepe/RCDWeb.git` (branch `master`) |
| Ultimo commit | `f02045f` "Aggiungere i file di progetto." |
| Commit totali | 2 |
| Soluzione | `RCDWeb.sln` → 3 progetti: `Source/RCDWeb.csproj`, `RCDWeb.Job/RCDWeb.Job.csproj`, `RCDJob/ConsoleRCDJob/ConsoleRCDJob/ConsoleRCDJob.csproj` |
| Dimensione repo | ~96 MB (di cui ~2498 file tracciati, ~1105 JS, 418 PNG, 191 CSS) |
| Test esistenti | **Nessuno** |
| CI/CD | **Nessuna** |
| LICENSE | **Assente** |
| README | **Assente** (solo `Source/readme.txt` generico di Visual Studio) |

### 1.1 Dominio funzionale

RCDWeb gestisce il ciclo di vita di una **richiesta di documentazione bancaria**:
apertura richiesta → assegnazione filiale/ufficio → richiesta documenti al cliente →
giacenza documenti → consegna/notifica → cambio stato → stampa modulistica PDF/DOCX →
log stati, email, allegati.

Entità di dominio (28 `DbSet` in `Source/Areas/RCD/DAL/RCDContext.cs`):

```
Richieste, Stati, LogStati, Condizioni, CondizioneStato, Canale, TipoAllegato, Allegati,
DocumentiRichiesti, TipiDocumenti, Uffici, TipiPagamento, TipiRichiedente, Cointestatari,
AbilitaElementiStato, Elementi, Avvisi, EmailSettings, EmailTesti, LogEmail, ErroriGenerici,
ParametriApp, ParametriRichiestaConsegna, Allegati, ... (+ entità AreaCommonWeb)
```

### 1.2 Architettura

```
RCDWeb.sln
├── Source/RCDWeb.csproj                 → web app MVC4 (Area "RCD")
│   ├── Global.asax.cs                   → Application_Start: aree, filtri, route, RazorViewEngine
│   ├── App_Start/                       → AuthConfig, BundleConfig, RazorGeneratorMvcStart,
│   │                                      RegisterClientValidationExtensions
│   ├── Controllers/AccountController.cs → login, gestione utenze
│   ├── Business/AccountBusiness.cs
│   ├── Areas/RCD/
│   │   ├── Controllers/  RichiestaController.cs (1258 righe), RichiestaMetodi.cs (1540, partial),
│   │   │                 RichiestaControllerSimulato.cs (265)
│   │   ├── DAL/          RCDContext.cs (DbContext), InitialCreate.cs (seed dati)
│   │   ├── Models/       TabelleDB.cs (787 righe, entità EF), GestioneViewModel.cs, EmailSettings.cs
│   │   ├── Repository/   RCDRepository.cs (741 righe)
│   │   ├── Uility/       StampaDocumenti.cs (1156, iTextSharp+Word interop), EmailSender.cs,
│   │   │                 DateUtil.cs, FromFileHttp.cs, MapObj.cs, ModelStateErrors.cs, Extensions.cs
│   │   ├── Views/        56 .cshtml + .generated.cs (RazorGenerator, viste precompilate)
│   │   └── Doc_Files/    6 template .docx per la modulistica
│   ├── Content/, Scripts/  Kendo UI 2015.2.902 + Telerik 2012.1.214 + jQuery
│   └── Web.config, connection.config, Mvc.sitemap, packages.config
├── RCDWeb.Job/RCDWeb.Job.csproj         → scheduler Quartz.NET (CheckRichiesteJob, KeepAlive)
├── RCDJob/ConsoleRCDJob/                → host console del job
├── old/                                 → snapshot pre-refactoring (codice morto, vedi §3)
├── lib/                                 → binari di terze parti + AreaCommonWeb.dll proprietaria
└── BuildProcessTemplates/               → template build TFS (eredità, inutili)
```

### 1.3 Stack e dipendenze

**Framework/piattaforma:** .NET Framework 4.5, ASP.NET MVC 4, Razor, IIS/Windows Authentication,
EF 5.0 (`packages.config`) / EF 4.4 assembly ref, SQL Server.

**Dipendenze NuGet dichiarate** (`Source/packages.config`, **cartella `packages/` assente nel repo**):
EntityFramework 5.0, Microsoft.AspNet.Mvc, RazorGenerator.Mvc 2.0, MvcSiteMapProvider 3.3.4,
Elmah, Elmah.Mvc, DotNetOpenAuth.*, ActionMailer.Net, DataAnnotationsExtensions, NPOI,
Newtonsoft.Json, Quartz, RazorEngine, Glimpse, WebGrease, Antlr3.Runtime, WebActivatorEx.

**Binari in `lib/` (non NuGet, non ridistribuibili pubblicamente):**

| File | Natura | Redistribuibile? |
|---|---|---|
| `AreaCommonWeb.dll` (817 KB) | **Libreria interna aziendale, closed source** (auth, profili, `ResourcesRepository`, `UserProfileManager`) | ❌ NO — blocker assoluto |
| `Telerik.Web.Mvc.dll` (958 KB) | Commerciale Telerik | ❌ NO |
| `KENDOUIMVC/2015.2.902/Kendo.Mvc.dll` | Commerciale Telerik (licenza a pagamento) | ❌ NO |
| `WebServiceModel.dll` (965 KB) | Proxy servizi interni (`WebServiceModel.WSAnagrafe`) | ❌ NO |
| `Microsoft.Web.Services3.dll` (WSE 3.0) | Microsoft, deprecato (2006) | ⚠️ legacy |
| `Microsoft.Office.Interop.Word.dll` | Richiede Word installato sul server | ⚠️ non portabile |
| `itextsharp.dll` 5.4.2 | AGPL/Commerciale | ⚠️ AGPL → incompatibile con MIT |
| `lib/lib.zip` | Contiene `System.Web.WebPages.dll`, `WebActivatorEx.dll` | ✅ sì |

### 1.4 Il "vecchio progetto funzionante"

La cartella **`old/`** è lo snapshot del codice **prima** del refactoring ad Area MVC.
Non è compilata da nessun `.csproj` (non è referenziata in `RCDWeb.sln`) → è codice morto:
16 file `.cs`, 25 `.cshtml`, 7 `.generated.cs`, un `IndexJqueryTable - Old.cshtml` con spazio nel nome.
Contiene le stesse classi di `Source/Areas/RCD/**` con nomi identici.

Stessa cosa per i file `_Prefisso.cs` in `Source/Areas/RCD/Uility/`
(`_StampaDocumenti.cs`, `_StampaDocumenti_old.cs`) e per `Details_old.cshtml`.

---

## 2. Diagnosi — perché oggi NON è funzionante né distribuibile

### 2.1 Blocker di sicurezza (CRITICO — repo pubblico)

| # | Problema | File | Impatto |
|---|---|---|---|
| S1 | Credenziali SQL Server in chiaro | `Source/connection.config`, `RCDJob/.../App.config` (`User ID=removed;$1=[removed] `sa/removed`) | Account DB esposto |
| S2 | Password wrapper web service in chiaro | `Source/Web.config` appSettings `Prod.WrapperBaseUser$1=[removed] `Svi.WrapperBaseUser$1=[removed] | Credenziali di produzione esposte |
| S3 | ELMAH aperto al mondo | `elmah.mvc.requiresAuthentication=false`, `allowedUsers=*`, `allowedRoles=*` | Log di errore con stack/connection string pubblici |
| S4 | `customErrors mode="Off"` in produzione | `Web.config` | Stack trace esposti |
| S5 | SMTP e host interni hardcoded | `ServerSmtp=removed`, `removed`, `removed`, `removed`, `\\removed\ApplicazioniMvc\...` | Fingerprinting infrastruttura |
| S6 | Utente di test hardcoded | `UtenzaTest=removedXXXX` | Dato personale |
| S7 | Email reale in seed | `InitialCreate.cs`: `developer@example.com`, `noreply@example.com` | Dati personali in repo pubblico |
| S8 | Path assoluti con `//serveur/share` | `InitialCreate.cs`, `PostAjaxForm`, upload | Non portabile |

> ⚠️ Queste credenziali sono **già pubbliche** su GitHub. La sanitizzazione del repo
> non le rende sicure: vanno **revocate/ruotate** a valle. Va registrato come azione utente.

### 2.2 Blocker di compilazione

| # | Problema | Evidenza |
|---|---|---|
| B1 | `packages/` assente | 0 riferimenti risolvibili per ~25 pacchetti NuGet |
| B2 | `AreaCommonWeb.dll` closed source | 40+ call site in `AccountController`, `RichiestaController`, `TabelleDB`, `RichiestaControllerSimulato` |
| B3 | `Microsoft.Office.Interop.Word` richiede Word su Windows | `StampaDocumenti.cs` |
| B4 | WSE 3.0 (`Microsoft.Web.Services3`) deprecato, non supportato su .NET moderno | `Web.config` section handler + policy cache |
| B5 | Duplicazione `TabelleDB.cs` / `_TabelleDB.cs` tra web e job | 787 vs 746 righe, divergenti |
| B6 | `RazorGenerator` + file `.generated.cs` committati: doppia compilazione delle viste | 56 `.cshtml` + 56 `.generated.cs` |
| B7 | Nessun `bin/`/`obj/`, nessun artefatto di build: nessuna prova che compili oggi | — |
| B8 | `BuildProcessTemplates/*.xaml` (TFS) non più usati | 3 file |

### 2.3 Blocker legali per distribuzione pubblica

| # | Problema |
|---|---|
| L1 | **MIT + Telerik Kendo UI commerciale = violazione licenza.** Kendo UI richiede licenza commerciale; non può essere ridistribuito con licenza MIT. |
| L2 | **MIT + iTextSharp 5.4.2 = violazione AGPL** (o licenza commerciale). |
| L3 | `AreaCommonWeb.dll` e `WebServiceModel.dll`: proprietà intellettuale aziendale, non ridistribuibili. |
| L4 | Marchi, loghi, ragione sociale, indirizzi e dati di persone fisiche in immagini e documenti `.docx`. |
| L5 | Dati personali (email, matricole) nei sorgenti e nel DB di seed. |

### 2.4 Debito tecnico e qualità

- Nessun test automatico, nessuna CI.
- Nessun `README.md`, nessuna `LICENSE`, nessun `CONTRIBUTING`, nessun `CHANGELOG`.
- Codice morto: `old/` (48 file), `_*.cs` (3 file), `*_old.*` (4 file), `* - Old.cshtml`.
- `catch` vuoti e `// TODO: handle exception` in tutti e 3 gli `EmailSender.cs`.
- `RichiestaController` con logica di dominio nel costruttore (chiamate a `UserProfileManager`)
  → non testabile, non istanziabile fuori da IIS.
- `AutoDetectChangesEnabled = false` + `ValidateOnSaveEnabled = false` + `LazyLoadingEnabled = false`
  → comportamenti EF non standard, da documentare.
- Stringhe/commenti in italiano misti a inglese, nessuna localizzazione strutturata.
- `Source/readme.txt` è il readme di template VS, inutile.

---

## 3. Obiettivo del recupero

> **Rendere RCDWeb una base di codice che (a) si compila e si avvia da zero con passi
> documentati su una macchina pulita, (b) non contiene segreti né dipendenze non
> ridistribuibili, (c) ha README completo, licenza MIT e test eseguibili.**

### 3.1 Metriche e soglie (rubrica fissa, pesi uguali 0/1)

Metrica primaria: **rapporto criteri soddisfatti / 10** misurato su `docs/WORKSTATE.json`.
Soglia di accettazione finale: **10/10**. Soglia di avanzamento minimo per fase: **nessuna regressione**
(il punteggio non deve mai diminuire).

| # | Criterio (0/1) | Verifica oggettiva |
|---|---|---|
| C1 | Nessun segreto in file tracciati | `grep` su pattern password/key/host interni = 0 match |
| C2 | Nessun riferimento a infrastruttura aziendale | 0 occorrenze di `removed`, `example.com`, `172.28.`, `removed`, `<SVC_USER>` |
| C3 | `LICENSE` MIT presente alla root | file esiste, header SPDX valido |
| C4 | README completo | tutte le 9 sezioni richieste presenti (vedi §5) |
| C5 | Build riproducibile eseguita | log di build con exit code 0 archiviato in `docs/evidence/` |
| C6 | Test automatici presenti e verdi | `dotnet test`/`nunit-console` output con 0 failure |
| C7 | Nessun binario non ridistribuibile tracciato | `lib/` ridotto a sole dipendenze lecite o a placeholder documentati |
| C8 | Build da clone pulito con script unico | `./scripts/build.sh` esegue su checkout fresco |
| C9 | CI definita e verde | workflow GitHub Actions presente e passato |
| C10 | Codice morto rimosso | `old/`, `_*.cs`, `*_old.*`, `BuildProcessTemplates/` assenti |

### 3.2 Baseline misurato (2026-10-16)

**Baseline = 0/10.** Ogni criterio fallisce. Divario dalla soglia: **-10**.

---

## 4. Fasi di lavoro (una variabile nominata per fase)

Ogni fase: identifica UN cambiamento, registra evidenza in `docs/evidence/<fase>.md`,
aggiorna `docs/WORKSTATE.json`, non deve rompere i criteri già verdi.

### FASE 0 — Sicurezza (bloccante, priorità massima) → C1, C2
1. Esternalizzare **tutte** le stringhe di connessione: `connection.config` →
   `connection.config.example` + lettura da variabile d'ambiente/`Web.config` non tracciato.
2. Rimuovere `Prod.WrapperBaseUserPassword`, `Svi.WrapperBaseUserPassword`, `ServerSmtp`,
   `UtenzaTest` hardcoded → placeholder + documentazione.
3. `elmah.mvc.requiresAuthentication=true`, `allowedUsers=` amministratore, `allowedRoles=Admin`,
   `customErrors mode="RemoteOnly"`.
4. Neutralizzare/rimuovere `InitialCreate.cs` seed con email e path reali → dati finti
   (`example.com`, path relativi).
5. Sostituire `removed`, `removed`, `*.example.com`, `//server/share` con costanti
   configurabili.
6. `git filter-repo`/BFG per riscrivere la storia ed eliminare i segreti dal passato
   **prima** di rendere pubblico il repo (azione distruttiva: richiede conferma utente).
7. Produrre `SECURITY.md` con elenco credenziali da **ruotare** (azione utente).

**Evidenza:** `docs/evidence/00-security.md` con output dei comandi `grep` (0 match).

### FASE 1 — Licenza e documentazione → C3, C4
1. `LICENSE` MIT (anno 2026, autore da confermare con l'utente).
2. `README.md` completo, 9 sezioni (§5).
3. `NOTICE.md` / `THIRD-PARTY.md` con tutte le dipendenze e relative licenze.
4. `CONTRIBUTING.md`, `CHANGELOG.md`, `CODE_OF_CONDUCT.md` (opzionali ma richiesti per
   un repo "distribuibile pubblicamente").
5. Rimuovere/sostituire `Source/readme.txt`.

**Evidenza:** `docs/evidence/01-docs.md`.

### FASE 2 — Dipendenze ridistribuibili → C7
Decisione architetturale (vedi §6): rimuovere i binari non licenziabili dal repo.
1. Eliminare `lib/Telerik.Web.Mvc.dll`, `lib/KENDOUIMVC/**`, `lib/AreaCommonWeb.dll`,
   `lib/WebServiceModel.dll`, `lib/Microsoft.Office.Interop.Word.dll`, `lib/itextsharp.dll`,
   `lib/lib.zip` dal tracking (restano in `.gitignore` + `lib/README.md` con istruzioni).
2. Creare `stubs/AreaCommonWeb/` — progetto sorgente **shim** che reimplementa le sole API usate
   (`UserProfileManager`, `ResourcesRepository`, `UserProfile`, `DimUO`, `UserInfo`,
   `ResourcesContext`, `Consts`) leggendo da un provider configurabile (in-memory per demo).
3. Sostituire i call site (40+) con l'astrazione `IUserDirectory`.
4. Migrare la stampa documenti da `Office.Interop.Word` a **OpenXML SDK** (già in `lib`
   come `DocumentFormat.OpenXml`) e da `iTextSharp` a **PdfSharpCore/QuestPDF** (licenza MIT/Apache).
5. Alternativa pragmatica se il tempo è insufficiente: mantenere i binari **fuori** dal repo
   + `<Reference>` condizionale + `lib/README.md` con download manuale e nota licenza.

**Evidenza:** `docs/evidence/02-deps.md`.

### FASE 3 — Build riproducibile → C5, C8
1. Introdurre `Directory.Build.props` e convertire a **SDK-style** oppure mantenere legacy
   e usare `msbuild` da container Mono (scelta da validare con test binario).
2. `scripts/build.sh` + `scripts/restore.sh` + `Dockerfile.build` (immagine `mono:6.12`
   con nuget) per compilare su Linux.
3. Test binario di fattibilità: `msbuild RCDWeb.sln /t:Build /p:Configuration=Release`
   dentro container. Criterio di superamento: exit 0 e `RCDWeb.dll` prodotto.
4. Se il target `net45` non è compilabile con le dipendenze residue: applicare il
   **fallback documentato** — conversione a SDK-style multi-target o esclusione dei
   progetti non recuperabili con motivazione scritta in `docs/evidence/03-build.md`.
5. Archiviare il log di build in `docs/evidence/`.

**Evidenza:** `docs/evidence/03-build.md` + `docs/evidence/build-<data>.log`.

### FASE 4 — Test → C6
1. Nuovo progetto `tests/RCDWeb.Tests` (NUnit/xUnit, net48 o net8 se multi-target).
2. Test unitari sui punti testabili **senza** IIS/DB:
   `DateUtil`, `MapObj`, `ModelStateErrors`, `Extensions`, logica pura di `StampaDocumenti`,
   mapping entità `TabelleDB`, `InitialCreate` (seed su provider in-memory).
3. Test di smoke: la soluzione carica i tipi pubblici senza errori di binding.
4. Test di configurazione: nessun segreto nei file di config (test-guard che fallisce
   se qualcuno reintroduce una password) → **test di regressione per C1/C2**.
5. `scripts/test.sh`.

**Evidenza:** `docs/evidence/04-tests.md` con output della test run.

### FASE 5 — CI → C9
1. `.github/workflows/ci.yml`: restore → build → test su `ubuntu-latest` (container mono)
   + `windows-latest` (msbuild nativo).
2. Badge nel README.
3. `.github/workflows/codeql.yml` per analisi sicurezza (rafforza C1/C2).

**Evidenza:** `docs/evidence/05-ci.md` con link alla run verde.

### FASE 6 — Pulizia e rifinitura → C10
1. Rimuovere `old/`, `_*.cs`, `*_old.*`, `* - Old.cshtml`, `BuildProcessTemplates/`.
2. Sostituire i `.generated.cs` committati con generazione a build time (o documentare
   la scelta).
3. Unificare `TabelleDB`/`_TabelleDB` e `RCDRepository`/`_RCDRepository` in un progetto
   condiviso `RCDWeb.Domain` referenziato da web e job.
4. `git gc`/ripulire la storia se richiesto.
5. Verifica finale 10/10 e report.

**Evidenza:** `docs/evidence/06-cleanup.md` + `docs/FINAL-REPORT.md`.

---

## 5. Contenuto richiesto del README (C4 — 9 sezioni)

1. **Titolo + badge** (build, license, .NET) + una frase di descrizione
2. **Panoramica / funzionalità** — cosa fa RCDWeb, flusso della richiesta di documentazione
3. **Stack tecnologico** — versioni esatte
4. **Requisiti** — Windows Server/IIS o Docker, SQL Server, .NET Framework 4.5 / Mono,
   Word (se ancora richiesto), variabili d'ambiente
5. **Installazione passo-passo** — clone, restore, configurazione, creazione DB, seed, build, run
6. **Configurazione** — tabella di *tutte* le chiavi `appSettings`/connection string con
   default e obbligatorietà (nessun valore reale)
7. **Uso / flusso applicativo** — login, ruoli, schermate principali, job Quartz
8. **Test** — come eseguire, cosa è coperto, cosa non lo è
9. **Troubleshooting + licenza + crediti + note di sicurezza**

---

## 6. Decisione architetturale aperta (da validare in Fase 3)

| Opzione | Pro | Contro | Costo |
|---|---|---|---|
| **A. Recupero legacy** (mono/Docker, deps esterne documentate, stub AreaCommonWeb) | mantiene il codice, rapido | resta su .NET Framework 4.5 EOL | basso |
| **B. Porting a .NET 8 + ASP.NET Core MVC** | moderno, CI nativa, testabile | riscrittura di viste/route/EF, altissimo | molto alto |
| **C. Archivio documentato** (README + LICENSE + sanitizzazione, build non garantita) | onesto, rapido, sicuro | non soddisfa C5/C6/C8 | minimo |

**Decisione (2026-10-16, approvata dall'utente): OPZIONE A.** Recupero legacy con build via
container Mono/Docker. Il porting a .NET 8 (opzione B) **non** è autorizzato. Se un criterio
resta irraggiungibile, si applica il fallback documentato dell'opzione C per quel solo criterio,
registrando in `docs/evidence/03-build.md` motivazione e test riproducibile.

---

## 7. Rischi

| Rischio | Probabilità | Mitigazione |
|---|---|---|
| Mono non compila WSE3/RazorGenerator | Alta | Fase 2 rimuove WSE3; fallback documentato |
| `AreaCommonWeb.dll` impossibile da shimmare (API non ispezionabili al 100%) | Media | `monodis`/ILSpy sul binario per estrarre la superficie API pubblica |
| Riscrittura storia git non gradita | Media | richiede conferma utente; alternativa: ruotare i segreti e documentare |
| iTextSharp AGPL | Alta | sostituzione o esclusione motivata |
| Scope creep (56 viste, 3000+ righe controller) | Alta | rubrica fissa a 10 criteri; nessun criterio aggiunto a metà |

---

## 8. Gate per un eventuale subagent

Ammesso **al più un** subagent, seriale, solo se dopo le fasi 0–3 resta un criterio binario
fallito e riproducibile. Se attivato, registrare prima:

1. Criterio fallito + evidenza del fallimento + criterio di superamento.
2. Compito delimitato in una frase (es. "estrarre la superficie API pubblica di
   `AreaCommonWeb.dll` e produrre `docs/area-common-web-api.md`").
3. Stessa metrica e soglia (rubrica /10).
4. Beneficio atteso: +N criteri.
5. Stop: fermarsi appena il criterio è verde; nessun secondo subagent, nessun retry, nessuna catena.

---

## 8bis. FASE 7 — Storia git e pubblicazione (autorizzata 2026-10-16)

1. Backup completo del repository (`git bundle` / copia della directory `.git`) prima di ogni
   operazione distruttiva.
2. `git filter-repo --replace-text` con un file di sostituzione per riscrivere password, host
   interni, nomi utente e dati personali in **tutti** i commit.
3. Verifica `git log -p | grep` sui pattern di C1/C2 = 0 match sull'intera storia.
4. `git push --force-with-lease origin master` (richiede credenziali GitHub fornite dall'utente).
5. Avvisare che i fork/clone esistenti mantengono i segreti: la rotazione delle credenziali
   resta **obbligatoria** e indipendente dalla riscrittura.
6. Se le credenziali di push non sono disponibili, consegnare lo script `scripts/rewrite-history.sh`
   e la procedura, e marcare la fase `blocked` senza forzare.

**Evidenza:** `docs/evidence/07-git-history.md`.

---

## 8ter. TRACK B — Cross-platform (richiesto dall'utente il 2026-10-16)

### B.0 Realtà tecnica da dichiarare

Il tier web **non può** essere reso cross-platform senza un porting: ASP.NET MVC 4 dipende da
`System.Web`, che esiste solo su .NET Framework/Windows (Mono offre un supporto parziale e
deprecato). Pertanto il cross-platform è organizzato in due obiettivi distinti:

| Obiettivo | Raggiungibile | Stato |
|---|---|---|
| **B1 — Build, test e CI su Linux/macOS/Windows** | subito | ✅ fatto |
| **B2 — Core di dominio senza dipendenze di piattaforma** | subito | ✅ fatto |
| **B3 — Esecuzione del tier web su Linux/macOS** | richiede ASP.NET Core | 🔴 non fatto |

B3 è un porting completo (viste Razor, routing, EF Core, rimozione Telerik/Kendo/WebForms,
abstrazione autenticazione). È tracciato ma **non** eseguito in questo ciclo.

### B.1 Criteri cross-platform (rubrica v2, in aggiunta a C1–C10)

Pesi uguali, 0/1, stessa soglia finale (tutti soddisfatti).

| # | Criterio | Verifica oggettiva |
|---|---|---|
| CP1 | Il repository si compila su Linux, macOS e Windows | matrice CI a 3 OS verde |
| CP2 | Il core di dominio non dipende da `System.Web` né da API Windows-only | `src/RCDWeb.Domain` compila su `netstandard2.0` e `net45` |
| CP3 | I test girano su tutte le piattaforme senza IIS e senza database | `dotnet test` verde sui 3 OS |
| CP4 | Gli script di build sono POSIX e non richiedono PowerShell | `scripts/*.sh` eseguibili su Linux e macOS |
| CP5 | La configurazione avviene per variabili d'ambiente/segnaposto, non per valori cablati | nessuna credenziale versionata (C1) e override documentato |
| CP6 | Il tier web gira su Linux | **non soddisfatto**: richiede porting ad ASP.NET Core (B3) |

### B.2 Struttura introdotta

```
Directory.Build.props                     impostazioni comuni + reference assemblies .NET Framework
RCDWeb.CrossPlatform.sln                  soluzione buildabile su ogni piattaforma
src/RCDWeb.Domain/                        libreria di dominio, TargetFrameworks netstandard2.0;net45
  Abstractions/IErrorReporter.cs          sostituisce Elmah.ErrorSignal (nessuna dipendenza da System.Web)
  Utility/DateUtil.cs                     porting di DateUtil
  Utility/StringExtensions.cs             porting di Extensions
  Utility/MapObj.cs                       porting di MapObj
tests/RCDWeb.Domain.Tests/                xUnit, net8.0
  DateUtilTests.cs, StringExtensionsTests.cs, MapObjTests.cs
  SecurityRegressionTests.cs              guardie automatiche per C1, C2, C7, C10, C3, C4
Dockerfile.build                          .NET SDK 8 + Mono/msbuild
scripts/{restore,build,test}.sh           POSIX
.github/workflows/ci.yml                  matrice ubuntu/macos/windows + job di conformità
```

### B.3 Duplicazione temporanea e regola di convergenza

Le classi portate in `src/RCDWeb.Domain` **duplicano** quelle in
`Source/Areas/RCD/Uility` (che restano per il tier web legacy).
Regola registrata: **`src/RCDWeb.Domain` è la versione canonica**. Le copie legacy vanno
eliminate non appena il tier web è portato o non appena il progetto legacy referenzia la
libreria (possibile solo dopo l'aggiornamento del target a .NET Framework 4.6.1+).

---

## 9. Artefatti finali attesi

```
RCDWeb/
├── LICENSE                     (MIT)
├── README.md                   (guida completa, 9 sezioni)
├── PLAN.md                     (questo file)
├── PROMPT.md                   (prompt ingegnerizzato)
├── SECURITY.md
├── CONTRIBUTING.md
├── CHANGELOG.md
├── THIRD-PARTY.md
├── Directory.Build.props
├── Dockerfile.build
├── RCDWeb.CrossPlatform.sln
├── src/RCDWeb.Domain/          (libreria di dominio cross-platform)
├── tests/RCDWeb.Domain.Tests/  (xUnit + guardie di sicurezza)
├── docs/
│   ├── WORKSTATE.json          (stato macchina del loop)
│   ├── evidence/*.md           (evidenze per fase)
│   └── FINAL-REPORT.md
├── scripts/{build,restore,test}.sh
└── .github/workflows/ci.yml
```
