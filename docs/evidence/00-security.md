# Fase 00 — Security

Data: 2026-10-16T08:05:00Z
Variabile modificata: **neutralizzazione di segreti e riferimenti infrastrutturali** nei file di
configurazione, nel seed DAL, nelle viste (incluse le `.generated.cs`), nei metadati di assembly
e nei file di progetto.
Criteri target: C1, C2

## Baseline

```bash
grep -rniE "$1=[removed]"' ;<]|<PATTERN_SEGRETI_REDATTI>" \
  --include=*.config --include=*.cs --include=*.cshtml --include=*.csproj --include=*.js --include=*.bat . \
  | grep -v CHANGE_ME_DB_PASSWORD | wc -l      # => 35
grep -rniE "localhost|appserver1|appserver2|example|172\.28\.|svc-rcd|rcd_user|<DOMINIO_AZIENDALE>|<EMAIL_PERSONALE>" \
  --include=*.config --include=*.cs --include=*.cshtml --include=*.csproj --include=*.js --include=*.bat --include=*.xml --include=*.txt . \
  | wc -l                                      # => 49
```

## Modifica

**Credenziali → placeholder** (nessun valore reale residuo)
- `Source/connection.config`: `localhost`, `rcd_user/rcd_user` → `localhost`, `rcd_user`,
  `CHANGE_ME_DB_PASSWORD`.
- `RCDJob/ConsoleRCDJob/ConsoleRCDJob/App.config`: idem + connection string Quartz.
- `RCDWeb.Job/bak/connection.config` (non compilato): `sa/CHANGE_ME_DB_PASSWORD` → placeholder.
- `Source/Web.config`: `Prod.WrapperBaseUserPassword`, `Svi.WrapperBaseUserPassword`,
  `ServerSmtp`, `UtenzaTest` svuotati/neutralizzati; connection string EF commentate con
  `rcd_user/CHANGE_ME_DB_PASSWORD` sanificate; rimosso `<allow users="<DOMINIO>\utente, <DOMINIO2>\utente2" />`
  sostituito con `<allow roles="Admin" />`.
- `Source/Areas/RCD/DAL/InitialCreate.cs` + `old/DAL/InitialCreate.cs`: email reali e
  `dev-null@example.com` → `noreply@example.com` / `dev-null@example.com`; `SmtpLocal` → `SmtpLocal`.

**Host e URL interni → segnaposto/esternalizzati**
- `localhost` → `localhost`; share UNC `Doc_Files/RCD/`
  → `Doc_Files/RCD/`.
- `smtp.example.com` → `smtp.example.com`; `portal.example.com` → `portal.example.com`.
- `sso.example.com` SSO → `sso.example.com`, e **esternalizzato** in 4 file C#:
  `ConfigurationManager.AppSettings["SsoPortalUrl"]` (nuova chiave in `Web.config`).
- Help Desk link in `_Layout.cshtml` / `_Layout_NoMenu.cshtml` → `AppSettings["HelpDeskUrl"]`;
  nelle rispettive `.generated.cs` → `https://helpdesk.example.com/`.
- Nuove chiavi in `Web.config`: `SsoPortalUrl`, `HelpDeskUrl`, `DocFilesPath`.
- Commenti `<!-- appserver1 -->` / `<!-- appserver2 -->` rimossi.
- Bindings TFS (`SccProjectName`, `SccAuxPath`, `SccProvider`) rimossi da `ConsoleRCDJob.csproj`.

**Hardening ELMAH / errori**
- `elmah.mvc.requiresAuthentication` `false` → `true`
- `elmah.mvc.allowedRoles` `*` → `Admin`; `elmah.mvc.allowedUsers` `*` → `` (vuoto)
- `<security allowRemoteAccess="1" />` → `0`
- `customErrors mode="Off"` → `RemoteOnly` con redirect `~/Error`
- `<add key="ServerSmtp" value="smtp.example.com" />` → `smtp.example.com`

**Identità aziendale e dati personali**
- `Source/Properties/AssemblyInfo.cs`: `AssemblyCompany`/`AssemblyCopyright`
  "RCDWeb contributors" → "RCDWeb contributors".
- Footer con nome commerciale dell'istituto → `RCDWeb © 2014-2026` (cshtml + generated).
- Asset `Content/img/logo_placeholder.gif|jpg` → `logo_placeholder.gif|jpg`, riferimenti aggiornati.
- `StampaDocumenti.cs`: rimosso path personale commentato `H:\Il mio Drive\...`.
- `Source/Scripts/TracciatoGarante.js`: URL interno commentato neutralizzato.

**Documenti di pianificazione** (`PLAN.md`, `PROMPT.md`): password e dati personali sostituiti con
`removed` / placeholder. Restano gli **hostname come pattern di detection** delle check C1/C2
(necessari al loop); non sono credenziali e sono già esposti nei commit pubblici.

## Verifica

```bash
grep -rniE "$1=[removed]"' ;<]|<PATTERN_SEGRETI_REDATTI>" \
  --include=*.config --include=*.cs --include=*.cshtml --include=*.csproj --include=*.js --include=*.bat . \
  | grep -v CHANGE_ME_DB_PASSWORD | wc -l      # => 0
grep -rniE "localhost|appserver1|appserver2|example|172\.28\.|svc-rcd|rcd_user|<DOMINIO_AZIENDALE>|<EMAIL_PERSONALE>" \
  --include=*.config --include=*.cs --include=*.cshtml --include=*.csproj --include=*.js --include=*.bat --include=*.xml --include=*.txt . \
  | wc -l                                      # => 0
python3 -c "import xml.dom.minidom as m; m.parse('Source/Web.config'); m.parse('Source/connection.config'); m.parse('RCDJob/ConsoleRCDJob/ConsoleRCDJob/App.config'); m.parse('RCDJob/ConsoleRCDJob/ConsoleRCDJob/ConsoleRCDJob.csproj'); print('XML OK')"
```

Esito: **PASS** — C1 `35 → 0`, C2 `49 → 0`. Tutti i file XML toccati restano ben formati.

## Criteri aggiornati

| Criterio | Prima | Dopo |
|---|---|---|
| C1 | 🔴 red (35) | 🟢 green (0) |
| C2 | 🔴 red (49) | 🟢 green (0) |
| C3–C10 | 🔴 red | 🔴 red (invariati) |

Score: **0/10 → 2/10**.

## Note e debito residuo

- I segreti restano nella **storia git pubblica** (2 commit). La sanitizzazione del working tree
  non annulla l'esposizione: serve rotazione credenziali + eventuale riscrittura storia
  (azione irreversibile, richiede conferma utente — vedi `user_actions_required`).
- `DescrizioneUObyIstituto`, `GetConfigValue("RedirectIstituto", ...)`: identificatori di API della libreria
  chiusa `AreaCommonWeb.dll`. Rinominarli romperebbe la compilazione → lasciati, documentati.
- `Source/Web.config` contiene ancora connection string EF commentate storiche: sanificate ma da
  rivalutare in Fase 2.

## Prossima fase

FASE 1 — License & Docs (C3, C4).
