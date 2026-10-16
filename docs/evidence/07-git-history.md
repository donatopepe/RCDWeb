# Fase 07 — Storia git e pubblicazione

Data: 2026-10-16T11:20:00Z
Variabile modificata: **riscrittura della storia git per eliminare segreti e dati personali da
tutti i commit** (autorizzata dall'utente il 2026-10-16).
Criteri target: consolidamento di C1 e C2 sull'intera storia

> **Nota sulla redazione.** Questo documento descrive la riscrittura senza riportare i valori
> rimossi: elencarli qui li avrebbe reintrodotti nel repository. Al loro posto sono usate
> categorie e segnaposto. La corrispondenza esatta fra categorie e valori è conservata solo
> nel file di sostituzione, tenuto **fuori** dal repository
> (`/home/public/HermesVault/Backups/rcdweb/`, da cancellare dopo la rotazione delle credenziali).

## Baseline

Il repository è **pubblico** su `https://github.com/donatopepe/RCDWeb.git`, branch `master`,
2 commit, entrambi contenenti credenziali di produzione in chiaro.

```bash
git log --oneline --all
19ae708 Aggiungere i file con estensione gitignore e gitattributes.
f02045f Aggiungere i file di progetto.
```

Occorrenze rilevate su **tutta** la storia con `git grep … $(git rev-list --all)`:

| Categoria | Occorrenze | Note |
|---|---|---|
| Credenziali di database (password) | 7 | utente applicativo, `sa` e account amministrativo |
| Credenziali di web service (wrapper) | 2 | ambiente produzione e sviluppo |
| Credenziali di posta | 1 | account SMTP |
| Host interni e nomi di server applicativi | 12 | server DB, 2 server applicativi |
| Indirizzi IP privati | 5 | host SMTP e portale |
| Domini aziendali e URL interni | 12 | SSO, posta, help desk, proxy TFS |
| Account di dominio e dati personali | 5 | account nominativi, matricole, email personali |
| Marchi e nomi societari | 8 | intestazione assembly, footer, nomi di asset |
| **Totale** | **52** | |

La sanitizzazione del working tree (Fase 0) **non** eliminava questi valori dai commit passati:
chiunque avesse clonato il repository li conservava.

## Backup preventivo

Operazione distruttiva: backup creato **prima** di qualsiasi modifica.

```bash
mkdir -p /home/public/HermesVault/Backups/rcdweb
git bundle create .../rcdweb-pre-rewrite-20261016-104219.bundle --all   # 20 MB
tar czf .../rcdweb-git-dir-20261016-104219.tar.gz .git                  # 20 MB
```

> Questi backup contengono la storia **con** i segreti. Non vanno pubblicati e vanno cancellati
> dopo la rotazione delle credenziali.

## Modifica

### 1. Consolidamento del lavoro nel repository

Le 186 modifiche del ciclo precedente sono state committate **prima** della riscrittura:
`git-filter-repo` richiede un albero di lavoro pulito e i nuovi contenuti devono essere
anch'essi sanificati.

### 2. `git filter-repo --replace-text`

Strumento: `git-filter-repo` (script Python ufficiale, scaricato in `/tmp/git-filter-repo`;
`pip install` non disponibile per PEP 668). Il file di sostituzione contiene 70 regole:

| Categoria | Tipo di regola |
|---|---|
| Credenziali DB | regex su `user id=` e `$1=[removed] (case-insensitive) → `rcd_user` / `CHANGE_ME_DB_PASSWORD` |
| Credenziali servizi | i due valori reali → stringa vuota; l'utente di servizio → `svc-rcd` |
| Host e IP | sostituzione letterale dei nomi di server → `localhost` / `appserver1` / `appserver2`; regex `172\.28\.\d+\.\d+` → `10.0.0.1` |
| Domini e URL | domini aziendali → `example.com`; URL SSO/help desk/proxy TFS → segnaposto |
| Account e dati personali | `DOMINIO\utente` → `DOMAIN\user`; email personali → `dev-null@example.com`; matricole → `removed0000` |
| Marchi e nomi | ragione sociale → `RCDWeb contributors`; nomi di asset e percorsi con marchio → `logo_placeholder` / `example` |
| Controllo di versione | regex `<(SccProjectName\|SccLocalPath\|SccAuxPath\|SccProvider)>…</…>` → vuoto |
| Divulgazione parziale | prefissi dei valori reali citati nella documentazione → `pwdXXX` / `PasswordXXX` |

Comando (eseguito due volte, la seconda con le regole aggiuntive):

```bash
git filter-repo --replace-text /tmp/rcdweb-replacements.txt --force --replace-refs delete-no-add
```

### 3. Pulizia degli oggetti orfani

```bash
git reflog expire --expire=now --all
git gc --prune=now
```

### 4. Ripristino della guardia automatica danneggiata dalla riscrittura

`--replace-text` ha agito anche sul file dei test di sicurezza: la regola sul dominio
aziendale ha trasformato il pattern di rilevamento in `example\.it`, rendendo la guardia C2
incapace di rilevare il dominio reale.

Correzione: pattern ripristinato e committato **dopo** la riscrittura. Il valore è un
*pattern di rilevamento*, non un segreto, ed è già escluso dalla scansione tramite
`ExcludedFiles`.

### 5. Correzione di una reintroduzione accidentale (incidente)

La **prima versione di questo stesso documento** elencava, nella tabella di baseline, i valori
realmente rimossi. Essendo stata scritta *dopo* la riscrittura, è stata committata e pushatta
con i segreti in chiaro: un incidente auto-inflitto.

Rilevato dalla verifica `git clone` + `git grep` sul repository remoto (106 occorrenze
riscontrate dopo un push che doveva risultare pulito).

Correzione:

1. il documento è stato riscritto con categorie e segnaposto al posto dei valori;
2. `git commit --amend` ha sostituito il commit interessato, che era la punta del branch,
   facendo sparire il blob dalla storia raggiungibile;
3. `git gc --prune=now` e nuovo `--force-with-lease`;
4. è stata aggiunta una guardia automatica **hash-based** che rende impossibile ripetere
   l'errore: `SecurityRegressionTests.C1b_NoKnownCompromisedTokensAnywhere` confronta lo
   SHA-256 di ogni token presente in **tutti** i file (compresi `.md` e `.json`) con gli
   SHA-256 dei valori compromessi storici. I valori non sono scritti nel repository, quindi
   la guardia protegge senza divulgare.

Limite dichiarato: GitHub può conservare per un certo periodo i blob resi non raggiungibili.
Questo rafforza — non sostituisce — l'obbligo di **ruotare** le credenziali.

## Verifica

### Nuova storia

```
<tip>  Ripristina la guardia C2 danneggiata dalla riscrittura della storia
6d4a671 Recupero RCDWeb: sicurezza, licenza MIT, documentazione, test, cross-platform
b63b5c6 Aggiungere i file di progetto.
19ae708 Aggiungere i file con estensione gitignore e gitattributes.
```

### Segreti su TUTTA la storia, sul repository remoto clonato

```bash
git clone https://github.com/donatopepe/RCDWeb.git /tmp/rcdweb-verify
cd /tmp/rcdweb-verify
git grep -I -F <pattern> $(git rev-list --all) --
```

| Categoria | Prima | Dopo |
|---|---|---|
| Password di database | 7 | **0** |
| Credenziali di web service | 2 | **0** |
| Credenziali di posta | 1 | **0** |
| Host interni e server applicativi | 12 | **0** |
| Indirizzi IP privati | 5 | **0** |
| Domini aziendali | 12 | **0** |
| Account di dominio e dati personali | 5 | **0** |
| Marchi e nomi societari | 8 | **0** |
| **Totale** | **52** | **0** |

```bash
git fsck --unreachable --no-progress
# => nessun oggetto orfano
```

### Residui intenzionali (pattern di rilevamento, non segreti)

Alcuni nomi di dominio aziendale, il prefisso di una classe di IP privati e i nomi delle
chiavi TFS compaiono **solo** in:

- `tests/RCDWeb.Domain.Tests/SecurityRegressionTests.cs` — espressioni regolari delle guardie C2;
- `PLAN.md`, `docs/evidence/*.md` — documentazione dei criteri e delle check.

Sono necessari perché le guardie continuino a funzionare e perché le evidenze restino leggibili.
Non sono credenziali né host reali.

### Suite di test dopo la riscrittura

```
Build succeeded.
Passed! - Failed: 0, Passed: 41, Skipped: 0, Total: 41
```

## Push sul repository pubblico

```bash
git fetch origin
git push --force-with-lease origin master
```

`--force-with-lease` (non `--force`) impedisce di sovrascrivere il remoto se qualcun altro
vi ha pushato nel frattempo. Esito in `docs/evidence/07-push.txt`.

## Secondo e terzo passaggio — riscrittura dei contenuti binari

La prima riscrittura (`--replace-text`) agisce solo sui blob testuali: ragione sociale,
loghi e metadati contenuti nei **documenti Office** e nelle **immagini** restavano nella storia.

È stato quindi aggiunto `git filter-repo --blob-callback <file>` con un callback che:

1. riconosce i blob ZIP contenenti `word/document.xml` e riscrive le parti XML
   (testo, intestazioni, piè di pagina, `docProps`) e le immagini incorporate;
2. riconosce per SHA-256 le immagini di marchio e le sostituisce con il segnaposto neutro;
3. applica le stesse sostituzioni testuali a tutti i blob testuali.

Errore commesso e corretto: il primo tentativo passava al callback un **file di script
completo**; `git filter-repo` si aspetta invece il **corpo** della funzione
`callback(blob, _do_not_use_this_var=None)`. Il callback veniva quindi eseguito senza
effetto e la verifica iniziale (che leggeva gli archivi da uno stream) non lo rilevava.
Rilevato rieseguendo il controllo scrivendo i blob su file temporanei.

Verifica finale su clone pulito del repository remoto:

```bash
git clone https://github.com/donatopepe/RCDWeb.git /tmp/rcdweb-final
cd /tmp/rcdweb-final
git grep -I -F <pattern> $(git rev-list --all) --     # => 0 per tutti i pattern
```

| Categoria | Prima | Dopo |
|---|---|---|
| Segreti (password, host, account, dati personali) | 52 | **0** |
| Riferimenti all'azienda (ragione sociale, marchi, loghi, sistemi interni, acronimi) | 60+ | **0** |
| Riferimenti nei documenti Office della storia | 7 file × 3 commit | **0** |
| Immagini di marchio nella storia | 3 blob | **0** |
| Oggetti non raggiungibili | — | **0** |

## Limiti — da leggere

1. **La riscrittura non richiama i cloni e i fork esistenti.** Chi ha clonato il repository
   prima di questa data possiede ancora i segreti.
2. GitHub può conservare gli oggetti non più raggiungibili per un certo periodo, raggiungibili
   tramite SHA diretto.
3. Per questi due motivi i valori rimossi restano **compromessi**: la **rotazione delle
   credenziali è obbligatoria** e indipendente da questa operazione.

## Criteri aggiornati

| Criterio | Nota |
|---|---|
| C1 | green — 0 segreti anche sull'intera storia, più guardia hash-based sui token compromessi |
| C2 | green — 0 riferimenti a infrastruttura anche sull'intera storia, salvo i pattern di rilevamento |
| C9 | il push è stato eseguito; la prima run di GitHub Actions è in corso |

## Prossima fase

Attesa dell'esito della prima run di CI su GitHub (C9, CP1) e decisione dell'utente su CP6
(porting ad ASP.NET Core).
