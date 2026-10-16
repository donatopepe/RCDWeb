# Fase 10 — Rimozione dei riferimenti all'azienda di origine

Data: 2026-10-16T12:30:00Z
Variabile modificata: **eliminazione di ogni riferimento all'azienda di origine** (identità,
marchi, loghi, nomi di sistemi interni, dati di persone fisiche) da codice, configurazione,
documenti Office, asset e storia Git.
Criteri target: consolidamento di C1, C2, C7 + nuovi criteri C11 (branding) e C12 (metadati documenti)

Richiesta dell'utente: *"non ci devono essere riferimenti ad azienda"*.

## Baseline

Riferimenti rilevati nel working tree:

| Categoria | Occorrenze | Dove |
|---|---|---|
| Ragione sociale nel testo dei documenti | 6 file | piè di pagina dei template `.docx` |
| Ragione sociale nei metadati dei documenti | 6 file | `docProps/core.xml`, `docProps/app.xml` |
| Nomi di persone fisiche nei metadati | 4 | `dc:creator`, `cp:lastModifiedBy`, `vt:lpwstr` |
| Percorsi locali con nome utente | 4 | `descr="Immagine"` nelle immagini |
| Indirizzo sede legale, telefono, fax, partita IVA, REA, ABI, capitale sociale | 6 file | piè di pagina `.docx` |
| Logo dell'istituto | 2 asset + 5 immagini incorporate | `Content/img`, `word/media/` |
| Acronimo aziendale in identificatori di codice | 42 | `DescrizioneUObyIstituto`, `RedirectIstituto` |
| Nomi di sistemi interni | 4 | nome del portale interno, endpoint SSO proprietario, nome del portale host, un asset con nome di sistema |
| Codice applicazione interno | 2 | `CodApp=PTT` |
| Riferimenti nei documenti di progetto | 7 | `PLAN.md`, `CHANGELOG.md`, evidenze |
| **Asset immagine di marchio non aggiornato nel progetto** | 1 | `RCDWeb.csproj` referenziava ancora il vecchio nome file (difetto di build) |

## Modifica

### 1. Identificatori di codice

| Prima | Dopo | Nota |
|---|---|---|
| `DescrizioneUObyIstituto` (40 occorrenze, 13 file) | `DescrizioneUObyIstituto` | proprietà della libreria chiusa `AreaCommonWeb`: lo shim deve esporre il nuovo nome |
| `RedirectIstituto` (2 occorrenze) | `RedirectIstituto` | chiave di configurazione della libreria chiusa |

Il sostituzione ha richiesto anche la ricomposizione di un frammento di stringa spezzato fra due
`WriteLiteral` in `DatiRichiedente.generated.cs` (RazorGenerator divide il letterale a metà).

### 2. Configurazione

| File | Prima | Dopo |
|---|---|---|
| `Source/Web.config` | `CodApp=PTT` | `CodApp=APP` |
| `Source/Web.config` | URL portale interno con endpoint SSO proprietario | `https://sso.example.com/sso/login.aspx` |
| `RCDJob/…/App.config` | `NomePortale=Portal` | `NomePortale=Portal` |

### 3. Asset e progetto

- `Content/img/logo_placeholder.gif` e `.jpg` (erano il logo dell'istituto): **sostituiti** con
  un segnaposto neutro generato (riquadro chiaro con la scritta `RCDWeb`).
- `Source/RCDWeb.csproj` referenziava ancora `logo_placeholder.jpg`: corretto in `logo_placeholder.jpg`
  (**difetto di build preesistente**, il file era già stato rinominato).
- `Content/img/clock_icons.png` (nome di un sistema interno) → `clock_icons.png`, con
  aggiornamento del riferimento nel progetto.

### 4. Documenti Office (7 file, non 6)

La guardia automatica ha individuato un **settimo** documento sfuggito all'inventario manuale:
`Source/Areas/RCD/Modulo_Richiesta_Documentazione_REV_new.docx`.

Per ciascun documento sono stati riscritti:

| Parte | Intervento |
|---|---|
| `word/document.xml`, `word/header*.xml`, `word/footer*.xml` | ragione sociale, sede legale, telefono, fax, sito, email, partita IVA, codice fiscale, REA, ABI, capitale sociale, riferimenti al gruppo bancario |
| `docProps/core.xml`, `docProps/app.xml` | `dc:creator`, `cp:lastModifiedBy`, `Company`, `Manager` → `RCDWeb` |
| `docProps/custom.xml` | tutti i valori `vt:lpwstr` svuotati (contenevano nomi di persone) |
| `word/media/image1.jpeg` | sostituita con un segnaposto neutro |
| attributi `descr` delle immagini | percorsi locali con nome utente rimossi |

I 7 file restano archivi ZIP validi (`ZipFile.testzip()` superato) e i segnaposto
`FILIALE_PROG`, `DATA_PROG`, ecc. usati dall'applicazione sono **intatti**.

### 5. Guardie automatiche rese hash-based

Poiché il repository non deve contenere i valori vietati, i test non possono elencarli in chiaro.
Sono state introdotte due guardie che confrontano **SHA-256**:

| Guardia | Ambito | Tecnica |
|---|---|---|
| `C1b_NoKnownCompromisedTokensAnywhere` | tutti i file di testo | tokenizzazione, normalizzazione, suddivisione su punteggiatura e confronto hash |
| `C1c_NoBankIdentityInOfficeDocumentsOrImages` | documenti Office e immagini | **hash a finestra scorrevole** sulle parti XML (rileva il marcatore anche immerso in una frase) + confronto hash dei file immagine |

L'elenco dei pattern letterali in `InfrastructurePatterns` è stato rimosso: nessun dominio o
ragione sociale compare più nel repository.

### 6. Post-build

`Source/postbuild.bat` conteneva percorsi di una macchina di sviluppo e il nome di una soluzione
interna. Riscritto come copia condizionata su `HOST_LIB_PATH`; l'evento `PostBuildEvent` è stato
reso non bloccante.

### 7. Documenti di progetto

`PLAN.md`, `CHANGELOG.md`, `docs/evidence/00-security.md`, `docs/evidence/01-docs.md`,
`README.md`: i riferimenti all'azienda sono stati sostituiti da segnaposto
(`<DOMINIO_AZIENDALE>`, `<EMAIL_PERSONALE>`) o da descrizioni di categoria.

## Verifica

### Guardie in positivo

```
Passed! - Failed: 0, Passed: 43, Skipped: 0, Total: 43
```

### Guardie in negativo

| Prova | Esito |
|---|---|
| file `.cs` contenente `www.<dominio aziendale>` e `DescrizioneUObyIstituto` | `C1b` **fallisce** come previsto |
| `.docx` con `ISTITUTO DI CREDITO` reinserito in `word/document.xml` | `C1c` **fallisce** come previsto |
| ripristino dei file | suite di nuovo 43/43 |

Le guardie non sono test vacui.

### Inventario finale

```bash
# identificatori con l'acronimo aziendale
grep -rn "ACME" --include=*.cs --include=*.cshtml .      # => nessun risultato

# riferimenti all'azienda nei documenti Office
unzip -p <docx> word/*.xml docProps/*.xml | grep -i "removed_BANK"   # => nessun risultato

# letterali concatenati dei file .generated.cs
# (estrae e concatena i WriteLiteral, poi cerca i marcatori)            # => nessun risultato
```

## Limiti dichiarati

1. **Identificatori della dipendenza chiusa.** `DescrizioneUObyIstituto` e `RedirectIstituto`
   sono usati dal codice di RCDWeb ma la libreria binaria originale espone i vecchi nomi:
   senza lo shim `stubs/AreaCommonWeb` (Track B, punto 1) il progetto legacy non compila.
   Il requisito è documentato in `lib/README.md`.
2. **Storia Git.** I commit precedenti contengono ancora i riferimenti: la riscrittura è
   tracciata in `docs/evidence/07-git-history.md`.
3. **Codici numerici generici.** `REA`, `ABI`, partita IVA e capitale sociale sono stati sostituiti
   con zeri: non sono recuperabili, servono solo a mantenere la struttura del documento.

## Criteri aggiornati

| Criterio | Stato |
|---|---|
| C1 | 🟢 green — nessun segreto, guardia hash-based |
| C2 | 🟢 green — nessun riferimento a infrastruttura o azienda in chiaro |
| C11 (nuovo) — nessun riferimento all'azienda di origine | 🟢 green |
| C12 (nuovo) — nessun dato identificativo nei documenti Office | 🟢 green |

## Prossima fase

Riscrittura della storia Git per rimuovere i riferimenti anche dai commit precedenti,
inclusi i blob binari `.docx` e le immagini di marchio.
