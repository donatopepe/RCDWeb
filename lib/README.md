# `lib/` — dipendenze binarie

Questa cartella contiene riferimenti a dipendenze che **non** vengono ripristinate da NuGet.
Leggere prima [`../THIRD-PARTY.md`](../THIRD-PARTY.md) per licenze e compatibilità.

## Stato

I binari non ridistribuibili sono stati **rimossi dal versionamento** (Fase 2 di `PLAN.md`).
Questa cartella contiene solo la documentazione necessaria a ripristinarli localmente.

### Dipendenze storiche e relativo stato

| File atteso | Stato nel repo | Dove ottenerlo | Licenza |
|---|---|---|---|
| `AreaCommonWeb.dll` | ❌ non incluso | Fornito dall'organizzazione; **closed source**, nessuna sorgente pubblica | Proprietaria |
| `WebServiceModel.dll` | ❌ non incluso | Generato dal WSDL dei servizi interni `WSAnagrafe` | Proprietaria |
| `Kendo.Mvc.dll` (2015.2.902) | ❌ non incluso | Pacchetto NuGet `Kendo.Mvc` con licenza Telerik attiva | Commerciale Telerik |
| `KENDOUIMVC/…/Kendo.Mvc.resources.dll` | ❌ non incluso | Come sopra | Commerciale Telerik |
| `Telerik.Web.Mvc.dll` | ❌ non incluso | Distribuzione Telerik Extensions for ASP.NET MVC | Commerciale Telerik |
| `itextsharp.dll` (5.4.2) | ❌ non incluso | iTextSharp 5.4.2 (storico) | AGPL-3.0 o commerciale iText |
| `Microsoft.Office.Interop.Word.dll` (15.0.0.0) | ❌ non incluso | Da un'installazione di Microsoft Office, o tramite NuGet `Microsoft.Office.Interop.Word` | Microsoft EULA |
| `Microsoft.Web.Services3.dll` (3.0.0.0) | ❌ non incluso | WSE 3.0 (Microsoft, 2006, deprecato) | Microsoft |
| `lib.zip` (`System.Web.WebPages.dll`, `WebActivatorEx.dll`) | ❌ non incluso | NuGet: `Microsoft.AspNet.WebPages`, `WebActivatorEx` | Apache-2.0 |

## Ripristino locale

1. Ottenere ciascun binario secondo la licenza applicabile.
2. Collocarlo in questa cartella con **il nome file esatto** indicato sopra.
3. Ripristinare l'elemento `<Reference>` / `<HintPath>` corrispondente nel file `.csproj` se è
   stato rimosso (vedi la cronologia Git per i riferimenti originali).
4. Non eseguire `git add` su questi file: un test di regressione rifiuta la loro presenza
   nel versionamento.

## Alternativa consigliata

Invece di reinstallare le dipendenze proprietarie, contribuire alla loro sostituzione
(Fase 2 di `PLAN.md`):

| Dipendenza | Sostituto proposto | Licenza |
|---|---|---|
| `AreaCommonWeb.dll` | shim open source `stubs/AreaCommonWeb` che reimplementa le API usate, con provider utenti configurabile | MIT |
| `ithetxtsharp` (PDF) | QuestPDF o PdfSharpCore | MIT |
| `Microsoft.Office.Interop.Word` | DocumentFormat.OpenXml (già presente) | MIT |
| `Kendo.Mvc` (griglie) | griglie HTML + jQuery DataTables, oppure rimozione degli helper Kendo | MIT |
| `Telerik.Web.Mvc` | rimozione (componenti legacy già sostituiti) | — |
| `Microsoft.Web.Services3` | `HttpClient` / `System.ServiceModel` | MIT |

## Verifica

```bash
git ls-files lib/ | grep -E 'Kendo|Telerik|AreaCommonWeb|WebServiceModel|itextsharp|Interop.Word|Services3|lib\.zip'
# deve restituire output vuoto
```
