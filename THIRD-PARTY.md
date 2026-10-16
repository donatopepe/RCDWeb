# Dipendenze di terze parti

Questo documento elenca le dipendenze di RCDWeb, la relativa licenza e se possono essere
ridistribuite con questo repository (licenza MIT).

> **Regola:** nessun binario con licenza incompatibile con la MIT è versionato in questo
> repository. Le dipendenze elencate come *non incluse* vanno approvvigionate autonomamente
> dall'utente, che è responsabile del rispetto della licenza.

## 1. Incluse nel repository

### 1.1 Pacchetti NuGet (ripristinati da `packages.config`, non versionati)

| Pacchetto | Versione | Licenza | Note |
|---|---|---|---|
| EntityFramework | 5.0.0 | Apache-2.0 | |
| Microsoft.AspNet.Mvc (+ Razor, WebPages, WebApi) | 4.0.x | Apache-2.0 | |
| Microsoft.AspNet.Web.Optimization | 1.0.0 | Apache-2.0 | |
| Microsoft.Web.Infrastructure | 1.0.0 | Microsoft EULA (redistribuibile) | |
| WebGrease | 1.1.0 | Apache-2.0 | |
| Antlr3.Runtime | 3.1.3.5 | BSD-3-Clause | |
| Newtonsoft.Json | 4.5.11 | MIT | |
| NPOI / NPOI.OOXML / NPOI.OpenXml4Net / NPOI.OpenXmlFormats | 1.2.4 | Apache-2.0 | |
| DocumentFormat.OpenXml | 2.0.5022 | MIT | |
| RazorGenerator.Mvc | 2.0.0 | Apache-2.0 | |
| MvcSiteMapProvider (+ Web) | 3.3.4 | Apache-2.0 (v3) | verificare la versione |
| Elmah / Elmah.Mvc | 1.2.2 / 2.0.2 | Apache-2.0 | |
| DotNetOpenAuth.Core / .OAuth / .OpenId / .AspNet | 4.1.x / 4.3.x | MIT | |
| ActionMailer.Net / .Mvc | 0.8.2 | MIT | |
| DataAnnotationsExtensions (+ ClientValidation) | 1.1.0.0 | BSD-2-Clause | |
| Quartz | 2.1.2 | Apache-2.0 | |
| RazorEngine | 3.7.2 | Apache-2.0 | |
| Glimpse | 1.8.2 | Apache-2.0 | solo build Debug |
| WebActivatorEx | 2.x | Apache-2.0 | fornito in `lib/lib.zip` |
| System.Web.WebPages | 2.x | Apache-2.0 | fornito in `lib/lib.zip` |
| Common.Logging | 2.1.2 | Apache-2.0 | |
| ICSharpCode.SharpZipLib | 0.86.0 | MIT | |
| Kendo.Mvc.UI.Fluent / Kendo.Mvc | 2015.2.902 | richiede il pacchetto *Kendo.Mvc* (vedi §2) | il pacchetto NuGet è incluso solo se si dispone di licenza |

### 1.2 Asset front-end (inclusi in `Source/Scripts`, `Source/Content`)

| Asset | Licenza |
|---|---|
| jQuery, jQuery UI, jQuery Validation, jQuery Unobtrusive Ajax | MIT |
| Modernizr | MIT |
| Kendo UI Web / Telerik Extensions (CSS e JS) | **Commerciale Telerik** — vedi §2 |
| Bootstrap (dove presente) | MIT |

## 2. NON incluse — richiedono approvvigionamento autonomo

| Componente | Natura | Licenza | Impatto se assente |
|---|---|---|---|
| **`AreaCommonWeb.dll`** | Libreria interna *closed source*: autenticazione, profili utente, `UserProfileManager`, `ResourcesRepository`, `DimUO`, sicurezza, `CommonBusiness` | Proprietaria | **Blocco totale**: l'applicazione non compila e il login non funziona |
| **`WebServiceModel.dll`** | Proxy dei servizi interni `WSAnagrafe` | Proprietaria | Ricerca anagrafica (NDG, CF, telefono) non disponibile |
| **`Kendo.Mvc.dll` 2015.2.902** | Telerik Kendo UI for MVC | **Commerciale Telerik** – non ridistribuibile | Le viste Razor che usano gli helper Kendo non compilano |
| **`Telerik.Web.Mvc.dll`** | Telerik Extensions for ASP.NET MVC (legacy) | **Commerciale Telerik** | Componenti Telerik legacy non disponibili |
| **`itextsharp.dll` 5.4.2** | Generazione PDF | **AGPL-3.0** oppure licenza commerciale iText | Generazione PDF non disponibile |
| **`Microsoft.Office.Interop.Word.dll`** | Interoperabilità Word | Microsoft EULA — richiede Office installato | Generazione DOCX non disponibile |
| **`Microsoft.Web.Services3.dll`** | WSE 3.0 (deprecato, 2006) | Microsoft | Chiamate ai servizi WS-Security non disponibili |
| Modulistica reale (`.docx` aziendali, loghi, marchi) | Documenti e marchi aziendali | Proprietaria | I template in `Doc_Files/` sono segnaposto |

Le istruzioni di approvvigionamento sono in `lib/README.md`.

## 3. Incompatibilità con la licenza MIT

I seguenti componenti **non possono** convivere con una distribuzione MIT senza una licenza
separata dell'utente:

- **Kendo UI / Telerik** — licenza commerciale per sviluppatore/servizio; la redistribuzione
  del binario è vietata.
- **iTextSharp 5.4.2** — AGPL-3.0: obbligherebbe a distribuire il codice sorgente completo
  dell'applicazione sotto AGPL, oppure ad acquistare una licenza commerciale iText.

Per questo motivo i binari corrispondenti sono esclusi dal versionamento. Vedi la Fase 2 di
`PLAN.md` per la strategia di sostituzione.

## 4. Segnalazioni

Se si ritiene che un file incluso violi la propria licenza, aprire una issue indicando
file, licenza e titolare dei diritti: il file verrà rimosso tempestivamente.
