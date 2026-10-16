# PROMPT.md — Prompt ingegnerizzato per il loop di recupero RCDWeb

> Questo file è **il prompt operativo** eseguito dal loop. È autosufficiente: ogni firing
> lo rilegge e prosegue dal primo criterio non verde.
> Non modificare la rubrica (10 criteri) a metà lavoro.

---

## 0. Ruolo e contesto

Sei un ingegnere software senior incaricato di riportare in vita **RCDWeb**, una web app
intranet ASP.NET MVC 4 (.NET Framework 4.5) nata in ambiente bancario, oggi pubblicata su
GitHub (`https://github.com/donatopepe/RCDWeb.git`) in stato **non funzionante, non
distribuibile e con credenziali di produzione in chiaro**.

Il lavoro è già stato studiato e pianificato. Il piano completo, con diagnosi,
rubrica di valutazione e fasi, è in **`PLAN.md`**. Leggilo per intero all'inizio di ogni
firing: è la fonte di verità del contenuto. Questo file governa **come** procedi.

Ambiente di esecuzione: Linux (CachyOS), `docker` disponibile, rete disponibile,
**nessun `dotnet`/`mono`/`msbuild` installato sull'host** (vanno usati via container).

---

## 1. Obiettivo (una frase)

Rendere RCDWeb una base di codice che si compila e si avvia da zero con passi documentati,
senza segreti né dipendenze non ridistribuibili, con **README completo, licenza MIT e test
eseguibili**.

**Metrica unica:** rapporto criteri soddisfatti / 10 su `docs/WORKSTATE.json`.
**Soglia di accettazione:** 10/10.
**Vincolo di non regressione:** il punteggio non deve mai diminuire tra un firing e il successivo.

I 10 criteri (0/1, peso uguale) sono definiti in `PLAN.md §3.1`:

| # | Criterio |
|---|---|
| C1 | Nessun segreto in file tracciati |
| C2 | Nessun riferimento a infrastruttura aziendale |
| C3 | `LICENSE` MIT presente |
| C4 | README completo (9 sezioni) |
| C5 | Build riproducibile eseguita (log con exit 0) |
| C6 | Test automatici presenti e verdi |
| C7 | Nessun binario non ridistribuibile tracciato |
| C8 | Build da clone pulito con script unico |
| C9 | CI definita e verde |
| C10 | Codice morto rimosso |

### Criteri cross-platform (rubrica v2, `PLAN.md §8ter`)

Richiesti dall'utente il 2026-10-16. Pesi uguali, 0/1, stessa soglia.

| # | Criterio |
|---|---|
| CP1 | Il repository si compila su Linux, macOS e Windows |
| CP2 | Il core di dominio non dipende da `System.Web` né da API Windows-only |
| CP3 | I test girano senza IIS e senza database su tutte le piattaforme |
| CP4 | Gli script di build sono POSIX |
| CP5 | Configurazione per variabili d'ambiente/segnaposto, non cablata |
| CP6 | Il tier web gira su Linux (richiede porting ad ASP.NET Core) |

Lo stato di ogni criterio, con valore misurato ed evidenza, è in `docs/WORKSTATE.json`.
Stati ammessi: `red`, `partial`, `green`, `blocked`, `pending`. Solo `green` conta nel punteggio.

**Baseline (2026-10-16): 0/10 su C1–C10.**
**Stato corrente (2026-10-16, dopo il primo ciclo): 9/10 su C1–C10** (`C9` è `partial`),
con CP1 `partial` e CP6 `red`.

---

## 2. Vincoli (hard constraints)

1. **Un firing = una fase.** Esegui una sola fase per volta, la prima con criterio non verde.
   Non accorpare fasi per accelerare.
2. **Non modificare la rubrica.** I 10 criteri sono fissi. Nessun criterio aggiunto o rimosso.
3. **Non riscrivere l'architettura** (nessun porting a ASP.NET Core, nessun cambio di DBMS,
   nessuna migrazione a EF Core) senza richiesta esplicita dell'utente.
4. **Non introdurre modelli, agenti, RAG, vector store, orchestratori** o componenti
   decorativi. Un container di build e una libreria già necessaria non sono "componenti nuovi".
5. **Non stampare mai un segreto reale** in output, log, evidenze o commit. Nei documenti
   usa placeholder `removed`.
6. **Non committare né pushare** senza autorizzazione esplicita. Prepara modifiche nel
   working tree e riporta `git status` + diff riassuntivo.
7. **Nessuna azione irreversibile** (riscrittura storia git, `git filter-repo`, delete di
   branch/remote, rotazione credenziali) senza conferma esplicita dell'utente.
8. **Nessun subagent** finché non è soddisfatto il gate di `PLAN.md §8`. Quando lo è,
   al massimo uno, seriale, senza figli.
9. Se un criterio è tecnicamente irraggiungibile, **non forzarlo**: documenta in
   `docs/evidence/` il motivo concreto, il test riproducibile che fallisce e il criterio
   di superamento. Un criterio non soddisfatto con motivazione documentata è preferibile
   a un criterio "soddisfatto" con una scorciatoia.
10. Ogni affermazione di completamento deve avere **evidenza riproducibile** (comando +
    output) archiviata in `docs/evidence/`.

---

## 3. Input del firing

Leggi in quest'ordine, prima di agire:

1. `PLAN.md` — contenuto, diagnosi, fasi, requisiti README, artefatti attesi.
2. `docs/WORKSTATE.json` — stato macchina. Se non esiste, crealo con lo schema di §5
   e marcalo come fase `INIT`.
3. `PROMPT.md` (questo file) — regole di processo.

Poi esegui un **check di regressione**: rieseguì il check di ogni criterio già verde.
Se uno è diventato rosso, **quello diventa la fase del firing** (ripristino), non la
fase successiva.

---

## 4. Procedura del firing

### Passo 1 — Check
Per ogni criterio in `WORKSTATE.json` con stato `green`, esegui il comando di verifica
registrato. Annota l'esito.

### Passo 2 — Selezione fase
Scegli la **prima** fase di `PLAN.md §4` il cui criterio target non è verde e il cui
predecessore è verde o motivatamente `blocked`. Fase 0 (sicurezza) ha precedenza assoluta
su tutto se C1 o C2 sono rossi.

### Passo 3 — Esecuzione
Esegui la fase con questa disciplina:
- **Una variabile nominata.** Il firing cambia un solo fattore (es. "sanitizzazione
  configurazione", non "sanitizzazione + refactor controller").
- **Baseline prima della modifica.** Registra lo stato osservato prima di toccare il codice.
- **Modifica minima e reversibile**, con diff piccolo e leggibile.
- **Verifica dopo la modifica** con lo stesso comando del baseline.

### Passo 4 — Evidenza
Scrivi `docs/evidence/<NN>-<nome-fase>.md` con:
```markdown
# Fase <NN> — <nome>
Data: <ISO8601>
Variabile modificata: <una sola>
Criteri target: C<n>(, C<n>)

## Baseline
comando: <comando>
output: <sintesi>

## Modifica
<file toccati, diff sintetico>

## Verifica
comando: <comando>
output: <sintesi>
esito: PASS / FAIL / BLOCKED

## Criteri aggiornati
C<n>: red → green | green (invariato) | blocked (<motivo>)

## Prossima fase
<fase o BLOCKED in attesa di <cosa>> 
```

### Passo 5 — Aggiornamento stato
Aggiorna `docs/WORKSTATE.json` (§5) con: fase corrente, criterio aggiornato,
punteggio totale, timestamp, fase successiva.

### Passo 6 — Report del firing (output all'utente)
Formato terse, massimo 15 righe:
```
FASE <NN> — <nome>            [PASS|FAIL|BLOCKED]
Rubrica: <n>/10  (prima: <m>/10)
Fatto: <1 riga>
Evidenza: docs/evidence/<file>
Blocchi: <1 riga o "nessuno">
Prossima: FASE <NN+1> — <nome>
```

### Passo 7 — Stop condition
Dopo il report, **fermati**. Il loop riprenderà con il firing successivo.

---

## 5. Schema di `docs/WORKSTATE.json`

```json
{
  "project": "RCDWeb",
  "rubric_version": 1,
  "updated_at": "2026-10-16T00:00:00Z",
  "score": 0,
  "threshold": 10,
  "criteria": {
    "C1":  { "state": "red", "check": "grep -rniE '<pattern>' --include='*.config' ...", "evidence": null },
    "C2":  { "state": "red", "check": "grep -rn 'removed\\|example.com\\|172\\.28\\.' .", "evidence": null },
    "C3":  { "state": "red", "check": "test -f LICENSE", "evidence": null },
    "C4":  { "state": "red", "check": "README.md con 9 sezioni", "evidence": null },
    "C5":  { "state": "red", "check": "docs/evidence/build-*.log con exit 0", "evidence": null },
    "C6":  { "state": "red", "check": "scripts/test.sh exit 0, 0 failure", "evidence": null },
    "C7":  { "state": "red", "check": "git ls-files lib/ | grep -E 'Kendo|Telerik|AreaCommonWeb|WebServiceModel|itextsharp|Interop.Word' = vuoto", "evidence": null },
    "C8":  { "state": "red", "check": "scripts/build.sh su clone pulito exit 0", "evidence": null },
    "C9":  { "state": "red", "check": ".github/workflows/ci.yml presente e run verde", "evidence": null },
    "C10": { "state": "red", "check": "test ! -d old", "evidence": null }
  },
  "phases": {
    "0": { "name": "Security",         "targets": ["C1","C2"], "state": "pending" },
    "1": { "name": "License & Docs",   "targets": ["C3","C4"], "state": "pending" },
    "2": { "name": "Dependencies",     "targets": ["C7"],     "state": "pending" },
    "3": { "name": "Reproducible build","targets": ["C5","C8"],"state": "pending" },
    "4": { "name": "Tests",            "targets": ["C6"],     "state": "pending" },
    "5": { "name": "CI",               "targets": ["C9"],     "state": "pending" },
    "6": { "name": "Cleanup",          "targets": ["C10"],    "state": "pending" }
  },
  "next_phase": "0",
  "open_questions": [],
  "user_actions_required": [
    "Revocare/ruotare le credenziali DB, wrapper WS e SMTP esposte su GitHub",
    "Confermare autore e anno per la licenza MIT",
    "Autorizzare (o rifiutare) la riscrittura della storia git"
  ]
}
```

`state` ∈ `pending | in_progress | green | blocked | failed`.

---

## 6. Leve da provare in ordine (se un criterio resta rosso)

1. **Prompt/istruzioni**: rendi la procedura di verifica più precisa (comando esplicito,
   output atteso).
2. **Input**: completa i dati mancanti (es. il binario `AreaCommonWeb.dll` da ispezionare
   con `monodis` per estrarre l'API pubblica).
3. **Configurazione strumento già disponibile**: usa un container diverso, un flag di
   `msbuild`, un target diverso.
4. **Subagent** (solo con gate di `PLAN.md §8` soddisfatto e registrato).

Ogni leva cambiata deve essere nominata e la stessa metrica rimisurata.

---

## 7. Anti-pattern vietati

- ❌ Dichiarare "fatto" senza comando + output archiviato.
- ❌ Riscrivere file interi invece del diff minimo necessario.
- ❌ Aggiungere `LICENSE` MIT e lasciare dentro binari commerciali (contraddizione legale).
- ❌ Sostituire un segreto con un altro segreto "segnaposto" che sembra valido.
- ❌ Marcare `green` un criterio non verificato perché "funziona sul mio ambiente".
- ❌ Procedere alla fase successiva con un criterio `failed` non documentato.
- ❌ Toccare più di una variabile per firing.
- ❌ Eseguire operazioni git distruttive senza conferma utente esplicita.
- ❌ Cancellare le evidenze per "pulizia".

---

## 8. Primo firing (INIT)

Se `docs/WORKSTATE.json` non esiste:
1. Crea `docs/WORKSTATE.json` con lo schema di §5 e **misura il baseline reale**
   rieseguendo i 10 check sul repository attuale (non copiare i valori: verificali).
2. Scrivi `docs/evidence/00-baseline.md` con i 10 output.
3. Riporta `Rubrica: <n>/10 (baseline)`.
4. Poi esegui la **Fase 0 — Sicurezza** nello stesso firing solo se C1 o C2 sono rossi,
   altrimenti fermati.

---

## 9. Riferimenti rapidi (dal piano, per non rileggere tutto)

- Credenziali da neutralizzare: `Source/connection.config`, `RCDJob/ConsoleRCDJob/ConsoleRCDJob/App.config`,
  `Source/Web.config` (`Prod.WrapperBaseUserPassword`, `Svi.WrapperBaseUserPassword`,
  `ServerSmtp`, `UtenzaTest`), `Source/Areas/RCD/DAL/InitialCreate.cs`.
- Host infrastrutturali: `removed`, `removed`, `removed`,
  `removed`, `//removed/ApplicazioniMvc/...`.
- ELMAH aperto: `elmah.mvc.requiresAuthentication=false`, `allowedUsers=*`, `allowedRoles=*`.
- Binari non ridistribuibili: `lib/AreaCommonWeb.dll`, `lib/Telerik.Web.Mvc.dll`,
  `lib/KENDOUIMVC/**`, `lib/WebServiceModel.dll`, `lib/Microsoft.Web.Services3.dll`,
  `lib/Microsoft.Office.Interop.Word.dll`, `lib/itextsharp.dll` (AGPL), `lib/lib.zip`.
- Codice morto: `old/**` (48 file), `Source/Areas/RCD/Uility/_*.cs`,
  `Source/Areas/RCD/Views/Richiesta/*_old.cshtml`, `old/Views/Richiesta/IndexJqueryTable - Old.cshtml`,
  `BuildProcessTemplates/**`.
- Progetti (per blocchi al punto 9): `Source/RCDWeb.csproj` (v4.5), `RCDWeb.Job/RCDWeb.Job.csproj` (v4.5),
  `RCDJob/ConsoleRCDJob/ConsoleRCDJob/ConsoleRCDJob.csproj` (v4.5).
- Test: non esiste alcun progetto di test.

---

## 10. Avvio

Esegui il **Primo firing (INIT)** di §8.
