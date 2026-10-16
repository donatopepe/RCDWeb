# Fase 02 — Dependencies

Data: 2026-10-16T10:10:00Z
Variabile modificata: **rimozione dal versionamento dei binari non ridistribuibili**.
Criteri target: C7

## Baseline

```bash
git ls-files lib/ | wc -l                                        # => 60
git ls-files lib/ | grep -cE 'Kendo|Telerik|AreaCommonWeb|WebServiceModel|itextsharp|Interop.Word|Services3|lib.zip'
                                                                 # => 58
du -sh lib/                                                      # => 8.1 MB
```

Contenuto: `AreaCommonWeb.dll` (closed source), `WebServiceModel.dll`, `Kendo.Mvc.dll` +
17 assembly di risorse localizzate, `Telerik.Web.Mvc.dll`, `itextsharp.dll` (AGPL),
`Microsoft.Office.Interop.Word.dll`, `Microsoft.Web.Services3.dll`, `lib.zip`.

## Modifica

1. `git rm -r --cached lib/` → i 58 binari non ridistribuibili **non sono più versionati**.
2. `lib/README.md` mantenuto tracciato: documenta ogni dipendenza, la licenza, dove ottenerla
   e il sostituto open source proposto.
3. `.gitignore` esteso:

```gitignore
lib/*
!lib/README.md
Source/connection.local.config
**/Web.*.secret.config
**/*.local.config
.env
```

I file restano presenti in locale (dove servono alla build legacy su Windows) ma non possono
più essere committati per errore.

4. `THIRD-PARTY.md` creato: distingue dipendenze **incluse** (licenze compatibili) da quelle
   **non incluse**, e dichiara esplicitamente l'incompatibilità **MIT ↔ Kendo UI (commerciale)**
   e **MIT ↔ iTextSharp 5.4.2 (AGPL)**.
5. Rimosso `` e le altre chiavi `Scc*` residue da
   `Source/RCDWeb.csproj` e `RCDWeb.Job/RCDWeb.Job.csproj`.

## Verifica

```bash
git ls-files lib/
# => lib/README.md

git ls-files lib/ | grep -E 'Kendo|Telerik|AreaCommonWeb|WebServiceModel|itextsharp|Interop.Word|Services3|lib\.zip'
# => nessun output

git check-ignore -v lib/AreaCommonWeb.dll lib/KENDOUIMVC/2015.2.902/Kendo.Mvc.dll
# => .gitignore:369:lib/*   ...

git check-ignore -v lib/README.md || echo "OK: lib/README.md tracciato"
# => OK: lib/README.md tracciato
```

Guardia automatica: `tests/RCDWeb.Domain.Tests/SecurityRegressionTests.C7_NoNonRedistributableBinariesTracked`
esegue `git ls-files` e fallisce se uno dei binari ricompare nell'indice.

Esito: **PASS** — C7 verde.

## Debito residuo e piano di sostituzione

Rimuovere i binari dal repository **non** rende il progetto compilabile su Windows senza di
essi. La sostituzione è tracciata in `PLAN.md §4 FASE 2` e in `lib/README.md`:

| Dipendenza | Sostituto | Licenza | Stato |
|---|---|---|---|
| `itextsharp` (PDF) | QuestPDF / PdfSharpCore | MIT | da fare |
| `Microsoft.Office.Interop.Word` | `DocumentFormat.OpenXml` (già fra i pacchetti NuGet) | MIT | da fare |
| `Kendo.Mvc` (griglie, editor) | griglie HTML + DataTables, oppure conservare con licenza Telerik dell'utente | MIT | da fare |
| `Telerik.Web.Mvc` | rimozione (componenti legacy) | — | da fare |
| `Microsoft.Web.Services3` | `HttpClient` / `System.ServiceModel` | MIT | da fare |
| `AreaCommonWeb.dll` | shim `stubs/AreaCommonWeb` + provider utenti configurabile | MIT | **impatto maggiore**: 40+ call site |

## Criteri aggiornati

| Criterio | Prima | Dopo |
|---|---|---|
| C7 | 🔴 red (58 file) | 🟢 green (0 file) |
| C1, C2, C3, C4 | 🟢 | 🟢 (nessuna regressione) |

## Prossima fase

FASE 6 — Cleanup (C10).
