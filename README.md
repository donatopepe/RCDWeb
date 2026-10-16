<div align="center">

# RCDWeb

**Gestione delle Richieste di Documentazione bancaria — applicazione web ASP.NET MVC 4**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.5-purple.svg)](https://dotnet.microsoft.com/)
[![ASP.NET MVC](https://img.shields.io/badge/ASP.NET%20MVC-4.0-blue.svg)](https://learn.microsoft.com/aspnet/mvc/)
[![Build](https://github.com/donatopepe/RCDWeb/actions/workflows/ci.yml/badge.svg)](../../actions/workflows/ci.yml)

*Applicazione legacy recuperata e riportata a uno stato distribuibile. Vedi [Stato del progetto](#stato-del-progetto).*

</div>

---

## Indice

1. [Panoramica e funzionalità](#1-panoramica-e-funzionalità)
2. [Stack tecnologico](#2-stack-tecnologico)
3. [Requisiti](#3-requisiti)
4. [Installazione passo-passo](#4-installazione-passo-passo)
5. [Configurazione](#5-configurazione)
6. [Uso e flusso applicativo](#6-uso-e-flusso-applicativo)
7. [Test](#7-test)
8. [Troubleshooting](#8-troubleshooting)
9. [Licenza, crediti e sicurezza](#9-licenza-crediti-e-sicurezza)

---

## 1. Panoramica e funzionalità

**RCDWeb** gestisce il ciclo di vita completo di una *richiesta di documentazione* presentata da
un cliente a una banca: dalla richiesta iniziale alla consegna della documentazione, con gestione
di filiali, stati, allegati, avvisi, spese e modulistica.

### Flusso di dominio

```
┌──────────────┐   ┌─────────────┐   ┌────────────────────┐   ┌───────────────┐
│ Apertura     │──▶│ Assegnazione│──▶│ Documenti richiesti│──▶│ Giacenza /    │
│ richiesta    │   │ filiale/UO  │   │ al cliente         │   │ consegna      │
└──────────────┘   └─────────────┘   └────────────────────┘   └───────────────┘
        │                  │                    │                     │
        ▼                  ▼                    ▼                     ▼
   Canale, Tipo      Uffici / Filiali     TipiDocumenti /        Stati, LogStati,
   richiedente       (DimUO)              DocumentiRichiesti     avvisi, spese
                                                      │
                                                      ▼
                                        Stampa modulistica (DOCX/PDF),
                                        notifiche email, allegati
```

### Funzionalità principali

| Area | Descrizione |
|---|---|
| **Ricerca e selezione posizione** | Ricerca per NDG (numero di gestione), recupero intestatario, telefono, codice fiscale tramite servizi anagrafici |
| **Nuova richiesta** | Apertura richiesta vuota con anagrafica cliente, filiale, canale di ricezione, tipo richiedente, cointestatari |
| **Gestione richieste** | Griglia Kendo filtrabile per stato, filiale, data; inoltro tra uffici; cambio stato con vincoli per stato |
| **Documenti** | Elenco documenti richiesti, costi, numero copie, numero cartone, documentazione in giacenza |
| **Allegati** | Upload di allegati tipizzati con descrizione obbligatoria (min. 7 caratteri) |
| **Avvisi** | Avvisi di documentazione in giacenza con generazione modulistica |
| **Spese** | Disposizione di addebito spese (tipi di pagamento, spesa addebitabile) |
| **Stampe** | Generazione di 6 template DOCX e conversione PDF |
| **Email** | Invio notifiche con template testuali e log degli invii |
| **Job schedulato** | Controllo periodico delle richieste scadute (Quartz.NET) con notifica via URL firmato |
| **Sicurezza** | Autenticazione Windows/IIS, profili utente, filtri di sicurezza per UO/istituto, log errori ELMAH |
| **Amministrazione** | Gestione utenze di test, cookie di sicurezza (ruoli, filtri, utenza) |

### Entità di dominio (28 `DbSet`)

`Richieste`, `Stati`, `LogStati`, `Condizioni`, `CondizioneStato`, `Canale`, `TipoAllegato`,
`Allegati`, `DocumentiRichiesti`, `TipiDocumenti`, `Uffici`, `TipiPagamento`, `TipiRichiedente`,
`Cointestatari`, `AbilitaElementiStato`, `Elementi`, `Avvisi`, `EmailSettings`, `EmailTesti`,
`LogEmail`, `ErroriGenerici`, `ParametriApp`, `ParametriRichiestaConsegna`, `CondizioniAllegati`.

---

## 2. Stack tecnologico

| Componente | Versione |
|---|---|
| Runtime applicazione web | .NET Framework **4.5** |
| Runtime dominio e test | **.NET Standard 2.0** / **.NET 8** |
| Web framework | ASP.NET **MVC 4** (Razor) |
| ORM | Entity Framework **5.0** (Code First) |
| Database | Microsoft SQL Server (2012+) |
| UI | Kendo UI for MVC 2015.2.902, Telerik Extensions 2012.1.214, jQuery |
| PDF / Office | iTextSharp 5.4.2, DocumentFormat.OpenXml, Microsoft Office Interop Word |
| Autenticazione | Windows Authentication (IIS) / moduli |
| Error logging | ELMAH + Elmah.Mvc |
| Validazione client | DataAnnotationsExtensions, jQuery Unobtrusive |
| Navigazione | MvcSiteMapProvider 3.3.4 |
| Scheduler | Quartz.NET |
| Test | **xUnit** (progetto `tests/RCDWeb.Domain.Tests`, `net8.0`) |
| Build/CI | **cross-platform**: .NET SDK 8 + MSBuild/Mono, matrice GitHub Actions Linux/macOS/Windows |

### Struttura del repository

```
RCDWeb/
├── RCDWeb.sln                    Soluzione legacy (Windows / .NET Framework 4.5)
├── RCDWeb.CrossPlatform.sln      Soluzione cross-platform (Linux, macOS, Windows)
├── Directory.Build.props         Impostazioni comuni ai progetti moderni
├── Dockerfile.build              Immagine di build (.NET SDK 8 + Mono)
├── src/RCDWeb.Domain/            Libreria di dominio cross-platform (netstandard2.0;net45)
│   ├── Abstractions/             IErrorReporter (sostituisce Elmah.ErrorSignal)
│   └── Utility/                  DateUtil, StringExtensions, MapObj
├── tests/RCDWeb.Domain.Tests/    Test xUnit + guardie di regressione di sicurezza
├── Source/                       Web application legacy (Area "RCD")
│   ├── App_Start/                Bundle, auth, RazorGenerator, validazione client
│   ├── Areas/RCD/
│   │   ├── Controllers/          RichiestaController + RichiestaMetodi (partial)
│   │   ├── DAL/                  RCDContext (DbContext), InitialCreate (seed)
│   │   ├── Models/               Entità EF e view model
│   │   ├── Repository/           Accesso dati applicativo
│   │   ├── Uility/               Stampa documenti, email, utility
│   │   ├── Views/                Viste Razor (+ .generated.cs precompilate)
│   │   └── Doc_Files/            Template .docx della modulistica
│   ├── Controllers/              AccountController
│   ├── Views/                    Layout e viste account
│   ├── Content/ Scripts/         Asset statici
│   ├── Web.config                Configurazione applicazione
│   └── connection.config         Stringhe di connessione (segnaposto)
├── RCDWeb.Job/                   Libreria job schedulati (Quartz)
├── RCDJob/ConsoleRCDJob/         Host console del job
├── lib/                          Dipendenze binarie non-NuGet (vedi lib/README.md)
├── docs/                         Piano di recupero, stato, evidenze
├── scripts/                      Script di build / test / restore (POSIX)
└── .github/workflows/ci.yml      CI su Linux, macOS e Windows
```

---

## 3. Requisiti

### 3.1 Requisiti minimi (esecuzione dell'applicazione)

| Requisito | Dettaglio |
|---|---|
| Sistema operativo | Windows Server 2016+ / Windows 10+ (l'applicazione web richiede IIS) |
| Web server | IIS 7+ con **Autenticazione Windows** abilitata e **Autenticazione anonima** disabilitata sull'area protetta |
| Runtime | .NET Framework 4.5 o superiore (4.7.2+ consigliato) |
| Database | SQL Server 2012+ raggiungibile; l'utente applicativo necessita di `db_owner` sul database in fase di primo avvio |
| Build tools | Visual Studio 2019/2022 con workload *ASP.NET e sviluppo web*, oppure MSBuild 16+ |
| NuGet | `nuget.exe` 5+ o Visual Studio Package Manager |

### 3.2 Requisiti per componenti opzionali

| Componente | Requisito | Se assente |
|---|---|---|
| Stampa modulistica DOCX | Microsoft Word installato sul server (Office Interop) **oppure** il percorso OpenXML | La generazione DOCX/PDF non è disponibile; il resto dell'applicazione funziona |
| Servizi anagrafici (`WSAnagrafe`) | Endpoint SOAP raggiungibile | Ricerca NDG non disponibile; si usa il controller simulato |
| Notifiche email | Server SMTP raggiungibile | Invio email disabilitato (log attivo) |
| Directory utenti (`AreaCommonWeb`) | Fornita dall'organizzazione (binario non incluso) | Vedi §9.3 — l'autenticazione reale non è disponibile, usare la modalità demo |

### 3.3 Build cross-platform

Il progetto è **cross-platform a livello di build, test e CI**: `src/` e `tests/` girano su
Linux, macOS e Windows con .NET SDK 6+. Il container incluso fornisce anche Mono/MSBuild per
tentare la build dei progetti legacy .NET Framework.

```bash
# Linux / macOS
./scripts/restore.sh
./scripts/build.sh Release
./scripts/test.sh "" Release
```

```bat
:: Windows
scripts\restore.sh      :: Git Bash / WSL
msbuild RCDWeb.sln /p:Configuration=Release /p:Platform="Any CPU"
```

Con Docker (qualsiasi sistema operativo):

```bash
docker build -f Dockerfile.build -t rcdweb-build .
docker run --rm -v "$PWD:/src" rcdweb-build ./scripts/build.sh Release
docker run --rm -v "$PWD:/src" rcdweb-build ./scripts/test.sh "" Release
```

#### Cosa è cross-platform e cosa no

| Ambito | Linux | macOS | Windows | Note |
|---|---|---|---|---|
| `src/RCDWeb.Domain` (dominio) | ✅ | ✅ | ✅ | `netstandard2.0` + `net45`, nessuna dipendenza da `System.Web` |
| `tests/RCDWeb.Domain.Tests` | ✅ | ✅ | ✅ | 41 test, nessun IIS e nessun database richiesto |
| Script `scripts/*.sh` | ✅ | ✅ | ✅ | Git Bash / WSL su Windows |
| Build dei progetti legacy (`.NET Framework 4.5`) | ⚠️ | ⚠️ | ✅ | Su Linux/macOS via Mono, richiede i binari di `lib/` |
| **Esecuzione del sito web (ASP.NET MVC 4)** | ❌ | ❌ | ✅ | Richiede `System.Web` e IIS: **richiede un porting ad ASP.NET Core** |
| Stampa DOCX via Office Interop | ❌ | ❌ | ✅ | Richiede Microsoft Word installato |

> **Nota onesta:** il *dominio* e la pipeline di sviluppo sono cross-platform; il *sito web*
> non lo è. Rendere eseguibile il tier web su Linux/macOS richiede il porting ad ASP.NET Core
> (viste Razor, routing, EF Core, rimozione Telerik/Kendo, astrazione autenticazione).
> Vedi Track B in [`PLAN.md`](./PLAN.md).

---

## 4. Installazione passo-passo

### Passo 1 — Ottenere il codice

```bash
git clone https://github.com/donatopepe/RCDWeb.git
cd RCDWeb
```

### Passo 2 — Ripristinare i pacchetti NuGet

```bash
nuget restore RCDWeb.sln
```

su Linux/macOS:

```bash
./scripts/restore.sh
```

> La cartella `packages/` **non** è versionata: viene ricreata da `packages.config`.

### Passo 3 — Predisporre le dipendenze binarie non-NuGet

Alcune dipendenze non sono distribuibili pubblicamente e **non sono incluse** nel repository.
Consultare `lib/README.md` per l'elenco e le istruzioni.

### Passo 4 — Creare il database

L'applicazione usa Entity Framework Code First: al primo avvio `RCDRepository` esegue
`Database.CreateIfNotExists()` e, se il database è stato creato, popola i dati di riferimento
tramite `InitialCreate.On(context)` (parametri applicativi, stati, tipi documento, testi email,
messaggi di errore).

**Non sono presenti migrazioni EF**: lo schema viene generato dal modello Code First.

```sql
-- Eseguire come amministratore
CREATE DATABASE RCD;
CREATE LOGIN rcd_user WITH PASSWORD = '<password>';
USE RCD;
CREATE USER rcd_user FOR LOGIN rcd_user;
ALTER ROLE db_owner ADD MEMBER rcd_user;   -- necessario per il primo avvio (CREATE TABLE)
```

Per il job Quartz è richiesto anche un database degli scheduler (tabelle `QRTZ_*`), oppure la
modalità `RAMJobStore` (predefinita in assenza di configurazione persistente).

### Passo 5 — Configurare l'applicazione

Vedi [§5 Configurazione](#5-configurazione). **Nessuna credenziale è versionata**: vanno fornite
in locale.

### Passo 6 — Compilare

Windows:

```bat
msbuild RCDWeb.sln /p:Configuration=Release /p:Platform="Any CPU"
```

Linux (container):

```bash
./scripts/build.sh
```

### Passo 7 — Pubblicare su IIS

1. Pubblicare il progetto `Source/RCDWeb.csproj` su una cartella (es. `C:\inetpub\RCDWeb`).
2. Creare un sito/applicazione IIS puntata a quella cartella.
3. Pool di applicazioni: **.NET CLR v4.0**, pipeline **Integrated**.
4. Autenticazione: **Windows Authentication = Abilitata**, **Anonymous = Disabilitata**.
   In ambienti IIS 6 seguire la procedura equivalente (Scheda *Directory Security* →
   *Authentication and access control*).
5. Assegnare i permessi di scrittura all'identità del pool sulla cartella dei documenti
   generati e sul percorso di upload allegati.
6. Assicurarsi che `connection.config` sia presente accanto a `Web.config` e sia compilato con
   i valori d'ambiente.

### Passo 8 — Eseguire il job (opzionale)

```bat
RCDJob\ConsoleRCDJob\ConsoleRCDJob\bin\Release\ConsoleRCDJob.exe
```

Schedulare l'eseguibile con Task Scheduler o come servizio Windows. Il job richiede le proprie
stringhe di connessione in `RCDJob\ConsoleRCDJob\ConsoleRCDJob\App.config`.

### Passo 9 — Verifica

Aprire `http://<host>/RCD/Richiesta/GestioneRichieste`. La route predefinita dell'applicazione è
`{controller}/{action}/{id}` con default `Account/LogOnWithUtenzaTest`.

---

## 5. Configurazione

> ⚠️ **Regola di sicurezza del progetto:** nessuna credenziale, host di produzione o dato
> personale deve essere committato. I valori sotto sono segnaposto; fornire i reali tramite
> variabili d'ambiente o un file di override **non versionato**.

### 5.1 File di configurazione

| File | Scopo |
|---|---|
| `Source/Web.config` | Configurazione applicazione web, `appSettings`, sezione ELMAH |
| `Source/connection.config` | Stringhe di connessione dell'applicazione web (referenziato da `Web.config` con `configSource`) |
| `RCDJob/ConsoleRCDJob/ConsoleRCDJob/App.config` | Configurazione dell'host console del job |
| `RCDWeb.Job/App.config` | Configurazione della libreria job |

### 5.2 Stringhe di connessione

| Nome | Utilizzo | Note |
|---|---|---|
| `RCDContext` | Database applicativo RCD | Accesso principale EF Code First |
| `ApplicationServices` | Servizi applicativi / membership | Usata dal framework |
| `elmah-sql` | Log errori ELMAH | Se non configurata, ELMAH usa il log in memoria |
| `RichiestaContext`, `ResourcesContext`, `MenuContext`, `RicercaContext`, `GradimentoContext` | Contesti EF storici (commentati) | Ereditati da applicazioni correlate; **non** richiesti da RCDWeb |

Formato atteso in `connection.config`:

```xml
<connectionStrings>
  <add name="RCDContext"
       connectionString="Data Source=removed;Initial Catalog=RCD;User ID=removed;$1=[removed]"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Alternativa consigliata:** usare `Integrated Security=SSPI` con l'identità del pool IIS, così
nessuna password compare su disco.

### 5.3 `appSettings` rilevanti

| Chiave | Default nel repo | Obbligatoria | Descrizione |
|---|---|---|---|
| `ClientValidationEnabled` | `true` | no | Validazione client MVC |
| `UnobtrusiveJavaScriptEnabled` | `true` | no | JavaScript non invasivo |
| `ServerSmtp` | `smtp.example.com` | sì per email | Host del server SMTP |
| `CodApp` | `PTT` | sì | Codice applicazione usato dal layer utenti |
| `SsoPortalUrl` | `https://sso.example.com/...` | sì | Base URL SSO usata per costruire i link nelle email |
| `HelpDeskUrl` | `https://helpdesk.example.com/` | no | Link Help Desk nel footer |
| `DocFilesPath` | `~/Areas/RCD/Doc_Files/` | sì | Percorso dei template di modulistica |
| `UtenzaTest` | *(vuoto)* | no | Matricola per la modalità di profilazione di test (vuoto = disabilitata) |
| `Prod.WrapperBaseUserName` / `Prod.WrapperBaseUserPassword` | *(vuoto)* | sì per i servizi | Credenziali wrapper servizi — ambiente produzione |
| `Svi.WrapperBaseUserName` / `Svi.WrapperBaseUserPassword` | *(vuoto)* | sì per i servizi | Credenziali wrapper servizi — ambiente sviluppo |
| `elmah.mvc.requiresAuthentication` | `true` | sì | ELMAH richiede autenticazione |
| `elmah.mvc.allowedRoles` | `Admin` | sì | Ruolo autorizzato a vedere gli errori |
| `elmah.mvc.allowedUsers` | *(vuoto)* | no | Utenti autorizzati (oltre al ruolo) |
| `elmah.mvc.route` | `elmah` | no | Route del pannello ELMAH |
| `SessionWarning` | `18` | no | Minuti prima dell'avviso di scadenza sessione |
| `Glimpse:DisableAsyncSupport` | `true` | no | Compatibilità Glimpse |

### 5.4 Parametri applicativi nel database

Sono salvati nella tabella `ParametriApp` e modificabili da amministratore:

| Chiave | Descrizione | Default nel seed |
|---|---|---|
| `giorniscadenzabanca` | Giorni di scadenza verso la banca | 90 |
| `giorniscadenzacliente` | Giorni di scadenza verso il cliente | 90 |
| `docfilesfullpath` | Percorso della documentazione | `Doc_Files/RCD/` |

### 5.5 Configurazione ELMAH

```xml
<elmah>
  <security allowRemoteAccess="0" />
  <errorLog type="Elmah.SqlErrorLog, Elmah" connectionStringName="elmah-sql" />
</elmah>
```

`allowRemoteAccess="0"` impedisce l'accesso remoto al log. Lo script di creazione delle tabelle
ELMAH è `SQLServer.sql` nella distribuzione di ELMAH.

### 5.6 Variabili d'ambiente (build Linux/Docker)

| Variabile | Scopo |
|---|---|
| `NUGET_PACKAGES` | Cache dei pacchetti |
| `MSBUILD_ARGS` | Argomenti aggiuntivi di MSBuild |
| `RCD_CONNECTION_STRING` | Override della stringa `RCDContext` in fase di test |

---

## 6. Uso e flusso applicativo

### 6.1 Route

| Route | URL | Descrizione |
|---|---|---|
| Default | `{controller}/{action}/{id}` | Area esterna → `Account/LogOnWithUtenzaTest` |
| Area RCD | `RCD/{controller}/{action}/{id}` | Default → `Richiesta/GestioneRichieste` |
| ELMAH | `elmah` | Log errori (richiede ruolo `Admin`) |

### 6.2 Azioni principali

**`AccountController`** (autenticazione)

| Azione | Descrizione |
|---|---|
| `LogOn` | Accesso tramite identità Windows |
| `LogOnWithUtenzaTest` | Accesso con matricola di test (pagina predefinita) |
| `SelectedLogOn` | Selezione di una delle utenze disponibili |
| `LogOff` | Chiusura sessione |
| `_GridUtenze` | Griglia utenze |
| `CreaCookieUtente`, `CreaCookieRuoli…`, `CreaCookieFiltri…` | Creazione cookie di contesto sicurezza |

**`RichiestaController`** (area RCD)

| Azione | Descrizione |
|---|---|
| `GestioneRichieste` | Elenco richieste con filtri |
| `_GridElencoRichieste`, `_GridElencoNdg`, `_GridElencoNdgBinding` | Griglie Kendo (server-side) |
| `ApriRichiestaVuota` | Nuova richiesta |
| `checkNdg`, `GetCliente`, `getCfNdg`, `getTelefonoCliente` | Ricerca e recupero anagrafica |
| `GetFiliali`, `GetPossibiliInoltri`, `_GetUnitaOrganizzativa`, `DescFilialeByUocode` | UO e inoltri |
| `InitSearch`, `_getSceltaNdg` | Inizializzazione ricerca |
| `DeleteAsyncFolder` | Pulizia cartella temporanea |

**`RichiestaMetodi`** (partial, transizioni di stato)

`Abilitaelementi`, `BottoniCambioStato`, `Delete`, e le operazioni di gestione di allegati,
documenti, avvisi, spese e stampe.

**`RichiestaControllerSimulato`** — controller che espone gli stessi punti di ingresso con dati
di UO fittizi, utilizzabile quando i servizi anagrafici non sono disponibili.

### 6.3 Ruoli e sicurezza

- `UserProfileManager.GetFiltroSicurezza(profile, chiave)` restituisce i filtri per
  `codUnitRuolo`, `flagUffRuolo`, `IST` (istituto).
- Il costruttore di `RichiestaController` risolve il profilo corrente e memorizza
  `MioIstituto`, `MiaMatricola`, `MioUocode`.
- Un flag utente `FlagUfficioFiliale` con valore `F` (filiale) o `O` (ufficio) determina
  il contesto organizzativo.
- Ruolo `Admin` → accesso al pannello ELMAH.

### 6.4 Job schedulato

`RCDWeb.Job` contiene:

| Classe | Ruolo |
|---|---|
| `CheckRichiesteJob` | Individua le richieste scadute e notifica |
| `JobManager` | Configura lo scheduler Quartz |
| `KeepAlive` | Mantiene attiva l'applicazione IIS |
| `EmailSender` | Invio notifiche dal job |

Il job costruisce un URL firmato partendo da `Host` + `NomePortale` + `SsoPortalUrl` e invoca
`/RCD/Richiesta/GestioneRichieste/`.

### 6.5 Modulistica (`Source/Areas/RCD/Doc_Files/`)

| Template | Utilizzo |
|---|---|
| `Modulo_Richiesta_Documentazione.docx` | Richiesta documentazione |
| `Modulo_Invio_Documentazione.docx` | Invio documentazione |
| `Avviso_Documenti_in_Giacenza_1.docx` / `_2.docx` | Primo e secondo avviso |
| `Lettera_Disposizione_Addebito_Spese.docx` | Addebito spese |
| `Ricevuta_consegna_copia_documentazione.docx` | Ricevuta di consegna |

I segnaposto nel testo (es. `FILIALE_PROG`) vengono sostituiti a runtime.

---

## 7. Test

### 7.1 Esecuzione

```bash
./scripts/restore.sh
./scripts/test.sh "" Release
```

La suite gira su **Linux, macOS e Windows** con .NET SDK 6+, senza IIS e senza database.

Docker:

```bash
docker run --rm -v "$PWD:/src" rcdweb-build ./scripts/test.sh "" Release
```

Windows (PowerShell/CMD):

```bat
dotnet test RCDWeb.CrossPlatform.sln --configuration Release
```

### 7.2 Cosa è coperto

`tests/RCDWeb.Domain.Tests` (41 test) copre la logica testabile **senza IIS e senza database**:

- **`DateUtil`** — conversione date, formati accettati, date fuori intervallo, segnalazione errori
- **`StringExtensions`** — confronto e ricerca su stringhe normalizzate
- **`MapObj`** — mapping per riflessione, conversione di tipo, valori non convertibili
- **Guardie di regressione di sicurezza**:
  - `C1` nessuna credenziale nei file di codice e configurazione
  - `C2` nessun riferimento a infrastruttura interna o dato personale
  - `C3` licenza MIT presente
  - `C4` README con tutte le sezioni richieste
  - `C7` nessun binario non ridistribuibile **versionato** (verifica su `git ls-files`)
  - `C10` nessun codice morto

Le guardie sono state verificate anche **in negativo**: introducendo un file di configurazione
con credenziali finte, i test `C1` falliscono come previsto.

### 7.3 Cosa **non** è coperto

- Rendering delle viste Razor e interazione browser (nessun test end-to-end)
- Integrazione reale con SQL Server, IIS, SMTP e servizi anagrafici
- Generazione effettiva dei documenti (richiede Word/OpenXML e template reali)
- Job Quartz in esecuzione schedulata
- Il tier web legacy non è testato: richiede .NET Framework e IIS

### 7.4 Risultato di riferimento

Evidenza in [`docs/evidence/build-2026-10-16.log`](./docs/evidence/build-2026-10-16.log):
build `Release` e `41/41` test verdi su checkout pulito, in container Linux.

---

## 8. Troubleshooting

| Sintomo | Causa probabile | Soluzione |
|---|---|---|
| `Could not load file or assembly 'AreaCommonWeb'` | Dipendenza aziendale non inclusa | Vedi `lib/README.md` e §9.3 |
| `Could not load file or assembly 'Kendo.Mvc'` | Pacchetto Kendo UI non installato | Installare Kendo UI for MVC 2015.2.902 (licenza richiesta) |
| `The type initializer for 'System.Data.Entity...' threw an exception` | EntityFramework non ripristinato | Eseguire `nuget restore` e verificare la versione 5.0 |
| `CREATE DATABASE permission denied` | Utente DB senza permessi | Concedere `db_owner` o creare lo schema in anticipo |
| `Login failed for user 'removed'` | Stringa di connessione errata | Verificare `connection.config` (§5.2) |
| `La pagina elmah reindirizza al login` | Comportamento corretto | L'accesso richiede il ruolo `Admin` |
| `Could not load file or assembly 'Microsoft.Office.Interop.Word'` | Word non installato | Installare Word oppure disabilitare la stampa DOCX |
| Errore di compilazione su `.generated.cs` | Disallineamento fra `.cshtml` e `.generated.cs` | Rigenerare con RazorGenerator o rimuovere il `.generated.cs` obsoleto |
| Immagini/CSS non caricati | Asset statici mancanti in pubblicazione | Includere `Content/` e `Scripts/` nella pubblicazione |
| `InvalidOperationException: No owin.Environment item` | Avvio fuori da IIS | L'applicazione richiede IIS o IIS Express |
| Timeout chiamata servizi anagrafici | Endpoint non raggiungibile | Usare `RichiestaControllerSimulato` in sviluppo |
| La build fallisce su Linux | Dipendenze Windows-only | Usare `Dockerfile.build`; le dipendenze Office restano Windows-only |

---

## 9. Licenza, crediti e sicurezza

### 9.1 Licenza

Distribuito con licenza **MIT**. Vedi [`LICENSE`](./LICENSE).

Copyright (c) 2014-2026 RCDWeb contributors.

### 9.2 Crediti e origine

Il progetto nasce come applicazione intranet aziendale, successivamente ripulita da credenziali,
riferimenti infrastrutturali e dipendenze non ridistribuibili per poter essere pubblicata come
progetto open source. Il piano di recupero, la rubrica di valutazione e le evidenze sono in
[`PLAN.md`](./PLAN.md) e [`docs/`](./docs/).

### 9.3 Dipendenze non incluse

Alcuni componenti **non sono distribuiti** con questo repository perché proprietari, commerciali
o soggetti a licenze incompatibili con la MIT. Vedi [`THIRD-PARTY.md`](./THIRD-PARTY.md) e
`lib/README.md` per l'elenco completo, la licenza di ciascuno e le istruzioni di approvvigionamento.

Senza questi componenti l'applicazione **non è compilabile integralmente** e le funzionalità
dipendenti restano indisponibili.

### 9.4 Sicurezza

- Il repository **non contiene** credenziali, host interni o dati personali.
- Prima di distribuire una propria istanza, consultare [`SECURITY.md`](./SECURITY.md).
- Segnalazioni di vulnerabilità: non aprire issue pubbliche (vedi `SECURITY.md`).
- Le stringhe di connessione vanno fornite a runtime; preferire `Integrated Security`.

### 9.5 Stato del progetto

Il recupero è in corso ed è tracciato in `docs/WORKSTATE.json` con una rubrica fissa di
10 criteri. Avanzamento al 2026-10-16: **9/10**.

| # | Criterio | Stato |
|---|---|---|
| C1 | Nessun segreto nei file tracciati | 🟢 |
| C2 | Nessun riferimento a infrastruttura aziendale | 🟢 |
| C3 | Licenza MIT presente | 🟢 |
| C4 | README completo | 🟢 |
| C5 | Build riproducibile verificata | 🟢 |
| C6 | Test automatici verdi | 🟢 41/41 |
| C7 | Nessun binario non ridistribuibile tracciato | 🟢 |
| C8 | Build da clone pulito con script unico | 🟢 |
| C9 | CI definita e verde | 🟡 definita e validata localmente; prima esecuzione GitHub al prossimo push |
| C10 | Codice morto rimosso | 🟢 |

#### Criteri cross-platform (rubrica v2)

| # | Criterio | Stato |
|---|---|---|
| CP1 | Build su Linux, macOS e Windows | 🟡 matrice CI definita; verificata su Linux in container |
| CP2 | Core di dominio senza dipendenze di piattaforma | 🟢 `netstandard2.0` + `net45` |
| CP3 | Test eseguibili senza IIS e senza database | 🟢 |
| CP4 | Script POSIX | 🟢 |
| CP5 | Configurazione per variabili d'ambiente/segnaposto | 🟢 |
| CP6 | Il tier web gira su Linux | 🔴 richiede porting ad ASP.NET Core (Track B, PLAN.md) |

### 9.6 Contribuire

Vedi [`CONTRIBUTING.md`](./CONTRIBUTING.md). Modifiche e cronologia in [`CHANGELOG.md`](./CHANGELOG.md).

<div align="center">
<sub>RCDWeb — MIT License</sub>
</div>
