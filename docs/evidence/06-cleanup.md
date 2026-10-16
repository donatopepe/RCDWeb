# Fase 06 — Cleanup

Data: 2026-10-16T10:20:00Z
Variabile modificata: **rimozione del codice morto**.
Criteri target: C10

## Baseline

```bash
find . -type d -name old              # => ./old (48 file)
ls BuildProcessTemplates              # => 3 template TFS .xaml
find RCDWeb.Job -type d -name bak     # => RCDWeb.Job/bak
find Source RCDWeb.Job -name "_*.cs" ! -name "*.generated.cs"
find Source -name "*_old.cshtml" -o -name "* - Old.cshtml"
```

File dead-code individuati (nessuno referenziato da alcun `.csproj`, verificato con
`grep -rl <nome> --include=*.csproj`):

| File / cartella | Natura |
|---|---|
| `old/` (48 file) | Snapshot pre-refactoring ad Area MVC: 16 `.cs`, 25 `.cshtml`, 7 `.generated.cs` |
| `BuildProcessTemplates/` (3 file) | Template di build TFS non più usati |
| `RCDWeb.Job/bak/` (2 file) | Copie di backup della configurazione |
| `RCDWeb.Job/DAL/_RCDContext.cs` | Duplicato non compilato di `RCDContext.cs` |
| `RCDWeb.Job/Models/_TabelleDB.cs` | Duplicato non compilato di `TabelleDB.cs` |
| `RCDWeb.Job/Repository/_RCDRepository.cs` | Duplicato non compilato di `RCDRepository.cs` |
| `Source/Areas/RCD/Uility/_StampaDocumenti.cs` | Duplicato non compilato di `StampaDocumenti.cs` |
| `Source/Areas/RCD/Uility/StampaDocumenti_old.cs` | Versione precedente (649 righe) |
| `Source/Areas/RCD/Views/Richiesta/Details_old.cshtml` (+`.generated.cs`) | Vista obsoleta |
| `Source/Areas/RCD/Views/Richiesta/IndexJqueryTable - Old.cshtml` | Vista obsoleta (nome con spazio) |

## Modifica

```bash
git rm -r -f old BuildProcessTemplates RCDWeb.Job/bak
git rm -f Source/Areas/RCD/Uility/_StampaDocumenti.cs \
          Source/Areas/RCD/Uility/StampaDocumenti_old.cs \
          Source/Areas/RCD/Views/Richiesta/Details_old.cshtml \
          "Source/Areas/RCD/Views/Richiesta/IndexJqueryTable - Old.cshtml" \
          RCDWeb.Job/DAL/_RCDContext.cs \
          RCDWeb.Job/Models/_TabelleDB.cs \
          RCDWeb.Job/Repository/_RCDRepository.cs
```

Nessun file `.csproj` è stato modificato: nessuno di questi file era incluso nella compilazione,
quindi la rimozione non altera la build legacy.

**Non** rimossi (richiedono verifica funzionale, Fase successiva):

- i file `.generated.cs` di RazorGenerator, che sono **compilati** e legittimi;
- `Source/Views/Shared/_Layout_10.cshtml` e `_Layout_10 old.cshtml` (layout alternativi:
  verificare l'uso prima di rimuoverli);
- la duplicazione deliberata fra `Source/Areas/RCD/Uility/*.cs` e `src/RCDWeb.Domain/Utility/*.cs`
  (vedi `PLAN.md §B.3`: convergenza prevista dopo il porting del tier web).

## Verifica

```bash
test ! -d old                        && echo ok
test ! -d BuildProcessTemplates      && echo ok
test ! -d RCDWeb.Job/bak             && echo ok
find Source RCDWeb.Job -name "_*.cs" ! -name "*.generated.cs"     # => nessun output
find Source RCDWeb.Job -name "*_old.cshtml" -o -name "* - Old.cshtml"  # => nessun output
git ls-files | wc -l                 # => 2371  (da 2498)
```

Guardia automatica: `SecurityRegressionTests.C10_NoDeadCodeDirectories` — esclude
esplicitamente i `*.generated.cs`, che hanno prefisso `_` ma sono file compilati.

Esito: **PASS** — C10 verde.

## Criteri aggiornati

| Criterio | Prima | Dopo |
|---|---|---|
| C10 | 🔴 red (48+ file) | 🟢 green (0) |
| C1, C2, C3, C4, C7 | 🟢 | 🟢 (nessuna regressione) |

## Prossima fase

Track B — Cross-platform (`docs/evidence/08-crossplatform.md`).
