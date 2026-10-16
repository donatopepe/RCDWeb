using Newtonsoft.Json;
using RCDWeb.Areas.RCD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace RCDWeb.Areas.RCD.DAL
{
    public class InitialCreate
    {
        public void On(RCDContext context)
        {
            var ParametriApp = new List<ParametriApp>
            {
                new ParametriApp{ Key="giorniscadenzabanca", Descrizione="Giorni scadenza Banca da Richiesta", Val="90"},
                new ParametriApp{ Key="giorniscadenzacliente", Descrizione="Giorni scadenza Cliente da Documenti in Giacenza", Val="90"},
                new ParametriApp{ Key="docfilesfullpath", Descrizione="Url contenete documentazione", Val=@"Doc_Files/RCD/"},
            };
            ParametriApp.ForEach(s => context.ParametriApp.Add(s));
            context.SaveChanges();

            context.EmailSettings.Add(new Uility.EmailSettings { Name = "SmtpLocal", isActive = true, SenderName = "noreply@example.com", SmtpServer = "smtpin.example.com", SmtpUsername = "noreply@example.com", isSimulated = true, SimulatedEmail = "dev-null@example.com" });
            context.SaveChanges();



            var ErroriGenerici = new List<ErroriGenerici>
            {
                new ErroriGenerici{ Nome="tipoAllegato",ErrorMessage="Richiesto tipo allegato"},
                new ErroriGenerici{ Nome="DescrizioneFile",ErrorMessage="Richiesta descrizione del file almeno 7 caratteri"},
                new ErroriGenerici{ Nome="FileForm",ErrorMessage="Richiesta scelta file"},
                new ErroriGenerici{ Nome="Documento",ErrorMessage="Richiesta tipo documento"},
                new ErroriGenerici{ Nome="DescrizioneDocumento",ErrorMessage="Richiesta descrizione documento, almeno 7 caratteri"},
                new ErroriGenerici{ Nome="NumCopie",ErrorMessage="Richiesto numero copie >0 documentazione"},
                new ErroriGenerici{ Nome="Costo",ErrorMessage="Richiesto costo documentazione"},
                new ErroriGenerici{ Nome="pec",ErrorMessage="Richiesta Pec, almeno 7 caratteri."},
                new ErroriGenerici{ Nome="pecnonvalida",ErrorMessage="Pec non valida."},
                new ErroriGenerici{ Nome="email",ErrorMessage="Richiesta Email, almeno 7 caratteri."},
                new ErroriGenerici{ Nome="emailnonvalida",ErrorMessage="Email non valida."},
                new ErroriGenerici{ Nome="FilialeUOcode",ErrorMessage="Errore UO Code Filiale di consegna, almeno 5 caratteri."},
                new ErroriGenerici{ Nome="NumeroConto",ErrorMessage="Richiesto numero conto, almeno 7 caratteri."},
                new ErroriGenerici{ Nome="Documenti",ErrorMessage="Attenzione non sono presenti documenti richiesti."},
                new ErroriGenerici{ Nome="DataRichiesta",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataAvviso",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataIIAvviso",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataConsegna",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataAvvisoRispettoDataRichiesta",ErrorMessage="Attenzione Data Avviso non può essere inferiore a Data Richiesta."},
                new ErroriGenerici{ Nome="DataIIAvvisoRispettoDataAvviso",ErrorMessage="Attenzione Data Avviso II non può essere inferiore a Data Avviso."},
                new ErroriGenerici{ Nome="DataConsegnaRispettoDataAvviso",ErrorMessage="Attenzione Data Consegna non può essere inferiore a Data Avviso."},
                new ErroriGenerici{ Nome="DataConsegnaRispettoDataRichiesta",ErrorMessage="Attenzione Data Consegna non può essere inferiore a Data Richiesta."},
                new ErroriGenerici{ Nome="ModalitaPagamento",ErrorMessage="Richiesta modalità di pagamento"},
                new ErroriGenerici{ Nome="Richiedente",ErrorMessage="Richiesto richiedente, almeno 7 caratteri."},
                new ErroriGenerici{ Nome="IndirizzoRichiedente",ErrorMessage="Richiesto indirizzo richiedente, almeno 7 caratteri."},
                new ErroriGenerici{ Nome="DataAvvisoArchivia",ErrorMessage="Data archiviazione non ancora possibile"},
                new ErroriGenerici{ Nome="Motivazione",ErrorMessage="Motivazione richiesta, minimo 7 caratteri"},
                new ErroriGenerici{ Nome="Respingimodifica",ErrorMessage="Respingi per modifica possibile solo per diversa Unità Organizzativa"},
                new ErroriGenerici{ Nome="Nessunostatopossibile",ErrorMessage="Nessun cambio stato possibile"},
                new ErroriGenerici{ Nome="Solouocompetente",ErrorMessage="Possibile solo assegnare a UO Competente"},
                new ErroriGenerici{ Nome="MotivazioneBypass",ErrorMessage="Richiesta Motivazione Bypass Allegati, minimo 7 caratteri"},
                new ErroriGenerici{ Nome="errorecosto",ErrorMessage="Attenzione controllare costo documenti! (Messaggio non bloccante)"},
            };
            ErroriGenerici.ForEach(s => context.ErroriGenerici.Add(s));
            context.SaveChanges();

            var uffici = new List<Uffici>
            {
                new Uffici{ Nome="Uff. Affari Legali e Consulenza Legale Rete (856765)",UOCODE="856765",Istituto=47},
                new Uffici{ Nome="Uff. Recupero Crediti (856779)",UOCODE="856779",Istituto=47},
                new Uffici{ Nome="Uff. Crediti Esternalizzati (856833)",UOCODE="856833",Istituto=47},
                new Uffici{ Nome="Uff. Gestione Interna UTP (856834)",UOCODE="856834",Istituto=47},
            };
            uffici.ForEach(s => context.Uffici.Add(s));
            context.SaveChanges();

            var emailcorpo = @"<html>
	<link href='{css}' rel='stylesheet' type='text/css'>
		
		<table border=0 cellspacing=1 cellpadding=0 align=center width='700'><tr><td><tbody>
		<table border=1 cellspacing=0 cellpadding=10 width='100%' bgcolor=white>
		<tr>
		<td class='desc_cell_c'>
	      		<TABLE BORDER=0 WIDTH='600' CELLSPACING=5 CELLPADDING=0 align=center> 
				<tr>
				<td align='right' colspan='2'><img src='http://www.example.com/example/img/logo_placeholder.gif' width=106 height=57><br><br></td>
				</tr>
								<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'>Numero richiesta:</td>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'><b>{protocollo}</b> </td>
				</tr>
				
				<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'>Richiesta stato attuale:</td>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'><b>{descrizioneStato}</b> </td>
				</tr>


				<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'>Canale richiesta: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{Canalerichiesta}</b></td>
				</tr>	

				<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'>Tipologia richiedente: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{Tipologiarichiedente}</b></td>
				</tr>	

									<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Data Richiesta: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{dataRichiesta}</b></td>
				</tr>		
        									<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Data Scadenza Banca: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{dataScadenzaBanca}</b></td>
				</tr>			
                									<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Data Scadenza Cliente: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{dataScadenzaCliente}</b></td>
				</tr>		
				<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> NDG: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{ndg}</b></td>
				</tr>
                <tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Intestazione NDG: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{IntestazioneNDG}</b></td>
				</tr>
                <tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Operatore: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{operatore}</b></td>
				</tr>
        				<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Filiale: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{filiale}</b></td>
				</tr>
				<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Note: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{note}</b></td>
				</tr>			
				<tr>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'> Motivazione: </td>
				<td style='background-color:#F4F4F4;font-size:11px;font-family:verdana;'><b>{motivazione}</b></td>
				</tr>	
				<tr><td style='font-size:10px;font-family:verdana;' colspan=2>
                <a title='Link alla Richiesta' href = '{link}' target = '_blank' >Richiesta</a>
                </td></tr>
				<tr><td style='font-size:10px;font-family:verdana;' colspan=2>L'occasione &egrave; gradita per porgere distinti saluti.<br><br></td></tr>
				<tr><td style='font-size:10px;font-family:verdana;' colspan=2><i>Per favore, non rispondere a questa mail.</i></td></tr>
				
				</table>
		</td></tr></table>

	</tbody></td></tr></table>
	</html>";

            var stati = new List<Stati>();
            var inizio = new Stati { Stato = "Inizio", Descrizione = "Inserimento", Chiuso = false, IsActive = false, ConteggioBanca = false, ConteggioCliente = false };
            var inserita = new Stati
            {
                Stato = "In Bozza",
                Descrizione = "Inserisci",
                Chiuso = false,
                ConteggioBanca = true,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "In Bozza",
                    Oggetto = "RCD Cambio Stato a In Bozza",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }
            };
            var assegnata = new Stati
            {
                Stato = "Assegnata",
                Descrizione = "Assegna",
                Chiuso = false,
                ConteggioBanca = true,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "Assegnata",
                    Oggetto = "RCD Cambio Stato a Assegnata",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                },
                NotificaUOCodeNext = true,
                NotificaResp = false,
                NotificaUOCode = false
            };
            var inlavorazione = new Stati
            {
                Stato = "In Lavorazione",
                Descrizione = "In lavorazione",
                ConteggioBanca = true,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "In Lavorazione",
                    Oggetto = "RCD Cambio Stato a In Lavorazione",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }
            };
            var respinta = new Stati
            {
                ChiediMotivazione = true,
                Stato = "Respinta",
                Descrizione = "Respingi",
                DataStopBanca = true,
                Chiuso = true,
                ConteggioBanca = false,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "Respinta",
                    Oggetto = "RCD Cambio Stato a Respinta",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }
            };
            var annullata = new Stati
            {
                ChiediMotivazione = true,
                Stato = "Annullata",
                Descrizione = "Annulla",
                DataStopBanca = true,
                Chiuso = true,
                ConteggioBanca = false,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "Annullata",
                    Oggetto = "RCD Cambio Stato a Annullata",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }
            };
            var respintaxmodifica = new Stati
            {
                ChiediMotivazione = true,
                Stato = "Respinta per modifica",
                Descrizione = "Respingi per modifica",
                Chiuso = false,
                ConteggioBanca = true,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "Respinta per modifica",
                    Oggetto = "RCD Cambio Stato a Respinta per modifica",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                },
                NotificaUOCodeNext = true,
                NotificaResp = false,
                NotificaUOCode = false
            };
            var documentiingiacenza = new Stati
            {
                Stato = "Documenti in Giacenza",
                Descrizione = "Documenti in giacenza",
                DataStopBanca = true,
                Chiuso = false,
                ConteggioBanca = false,
                ConteggioCliente = true,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "Documenti in Giacenza",
                    Oggetto = "RCD Cambio Stato a Documenti in Giacenza",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                },
                NotificaUOCodeNext = true,
                NotificaResp = false,
                NotificaUOCode = true
            };
            var evasa = new Stati
            {
                Stato = "Evasa",
                Descrizione = "Evadi",
                DataStopBanca = true,
                Chiuso = true,
                ConteggioBanca = false,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "Evasa",
                    Oggetto = "RCD Cambio Stato a Evasa",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                },
                NotificaUOCodeNext = true,
                NotificaResp = false,
                NotificaUOCode = true
            };
            var archiviata = new Stati
            {
                ChiediMotivazione = true,
                Stato = "Archiviata",
                Descrizione = "Archivia",
                DataStopBanca = true,
                Chiuso = true,
                ConteggioBanca = false,
                ConteggioCliente = false,
                EmailTesti =
                new EmailTesti
                {
                    Nome = "Archiviata",
                    Oggetto = "RCD Cambio Stato a Archiviata",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                },
                NotificaUOCodeNext = true,
                NotificaResp = false,
                NotificaUOCode = true
            };
            stati.Add(inizio);
            stati.Add(inserita);
            stati.Add(assegnata);
            stati.Add(inlavorazione);
            stati.Add(respinta);
            stati.Add(annullata);
            stati.Add(respintaxmodifica);
            stati.Add(documentiingiacenza);
            stati.Add(evasa);
            stati.Add(archiviata);
            stati.ForEach(s => context.Stati.Add(s));
            context.SaveChanges();



            //var emailcambiostato = new EmailTesti
            //{
            //    Nome = "avvisi",
            //    Oggetto = "RCD Avviso",
            //    Corpo = emailcorpo,
            //    Html = true,
            //    IsActive = true,
            //};
            //context.EmailTesti.Add(emailcambiostato);
            //stati.ForEach(s => context.EmailTesti.Add(new EmailTesti
            //{
            //    Nome = s.Stato,
            //    Oggetto = "RCD Cambio Stato a " + s.Stato,
            //    Corpo = emailcorpo,
            //    Html = true,
            //    IsActive = true,
            //}));
            //context.SaveChanges();



            var stato = inizio;
            var abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="class~",NomeElemento="TipoRichiedente2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="bottonidocumenti",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "NDG", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                ////new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "isPerson", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "hidden" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "IntestazioneNDG", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "true" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "CodiceFiscale", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "true" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "LuogoNascita", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "true" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "DataNascita", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "true" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "PIva", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "true" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "SedeLegale", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "true" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Indirizzo", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "true" },

                //new AbilitaElementiStato { Attributo = "id*", NomeElemento = "IscrizioneCCIAA", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "false" },              ///unici modificabili
                //new AbilitaElementiStato { Attributo = "id*", NomeElemento = "AlboProfessionale", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "false" },            ///unici modificabili

                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Richiedente", Stato = stato, NomeProprieta = "readonly", ValoreProprieta = "false" },
                //new AbilitaElementiStato {Attributo="id",NomeElemento="IndirizzoRichiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="false"},


                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "ModalitaPagamento_Id", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "NumeroConto", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "email", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "pec", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Titolare", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "TipoRichiedente_Id", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Legittimato", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Cessione", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "InoltroUfficio", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Motivazione", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Note", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "DataRichiesta", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },


                new AbilitaElementiStato { Attributo = "class*", NomeElemento = "DataPagamento", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
                new AbilitaElementiStato { Attributo = "class*", NomeElemento = "DataAvviso", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
                new AbilitaElementiStato { Attributo = "class*", NomeElemento = "DataIIAvviso", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
                new AbilitaElementiStato { Attributo = "class*", NomeElemento = "DataConsegna", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
                new AbilitaElementiStato { Attributo = "class*", NomeElemento = "DataChiusa", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },

                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Filiale", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "Luogo", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "false" },

                 new AbilitaElementiStato { Attributo = "class~", NomeElemento = "legittimato2", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
                new AbilitaElementiStato { Attributo = "class~", NomeElemento = "cessione2", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
                new AbilitaElementiStato { Attributo = "class~", NomeElemento = "DataRichiesta2", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
 
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnCambioStato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "btnArchivia", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato { Attributo = "id", NomeElemento = "btnRespingixmod", Stato = stato, NomeProprieta = "hidden", ValoreProprieta = "true" },

                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaAddspese",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaModGiacenza",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnModInvioDocumentazione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},


            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO INSERITA
            stato = inserita;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="class~",NomeElemento="bottonidocumenti",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},

                new AbilitaElementiStato{Attributo="class~",NomeElemento="canalericezione2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="TipoRichiedente2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id*",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id*",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="false"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                ////new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},

                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="legittimato2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="cessione2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaAddspese",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnStampaModGiacenza",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnModInvioDocumentazione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO INLAVORAZIONE
            stato = inlavorazione;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                //new AbilitaElementiStato{Attributo="class~",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id*",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id*",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IndirizzoRichiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="TipoRichiedente",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="canalericezione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="UOCode_Inoltro",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="class~",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataAvviso2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataConsegna2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataIIAvviso2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},


            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();


            //CAMBIO STATO DOC IN GIACENZA
            stato = documentiingiacenza;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {

                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id*",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id*",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IndirizzoRichiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="TipoRichiedente",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="canalericezione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleConsegna_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
               // new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="FilialeUOcode",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id",NomeElemento="Rapporto",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Documento_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="disabled"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneDocumento",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Costo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCopie",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DocOrig",Stato=stato,NomeProprieta="disabled", ValoreProprieta="disabled"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCartone",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AddDocumento",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnModificaDoc",Stato=stato,NomeProprieta="disabled", ValoreProprieta="disabled"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCancellaDoc",Stato=stato,NomeProprieta="disabled", ValoreProprieta="disabled"},


                new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},


                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataAvviso2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataConsegna2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataIIAvviso2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="class~",NomeElemento="inoltronew",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},


            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO ASSEGNATA
            stato = assegnata;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                //new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="class~",NomeElemento="canalericezione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id*",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id*",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IndirizzoRichiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},


                //new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},

                new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="TipoRichiedente",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},

                  new AbilitaElementiStato{Attributo="class*",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

               //new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
               // new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            var canale = new List<Canale>();
            var CanaleFiliale = new Canale { Nome = "Filiale", Descrizione = "Cliente in Filiale", Ricezione = true, Comunicazione = false, Consegna = true };
            var CanalePec = new Canale { Nome = "Pec", Descrizione = "PEC", Ricezione = true, Comunicazione = true, Consegna = true };
            var CanaleEmail = new Canale { Nome = "Email", Descrizione = "Email", Ricezione = true, Comunicazione = true, Consegna = true };
            var CanaleRaccomandata = new Canale { Nome = "Raccomandata", Descrizione = "Raccomandata A/R", Ricezione = true, Comunicazione = true, Consegna = true };
            canale.Add(CanaleFiliale);
            canale.Add(CanalePec);
            canale.Add(CanaleEmail);
            canale.Add(CanaleRaccomandata);
            canale.ForEach(s => context.Canale.Add(s));
            context.SaveChanges();

            var richiestadoc = new TipoAllegato { Nome = "Richiesta documenti" };
            var modulorichiesta = new TipoAllegato { Nome = "Modulo di richiesta" };
            var addebito = new TipoAllegato { Nome = "Disposizione di addebito spese" };
            var diniego = new TipoAllegato { Nome = "Comunicazione di diniego" };
            var ricevavvisogiacenza = new TipoAllegato { Nome = "Ricevuta di avviso giacenza documentazione" };
            var avvisogiacenza = new TipoAllegato { Nome = "Avviso di giacenza documentazione" };
            var consegnaPecRacc = new TipoAllegato { Nome = "Ricevuta di consegna PEC/Raccomandata" };
            var consegnaFiliale = new TipoAllegato { Nome = "Ricevuta di consegna documentazione in filiale" };
            var invioconsegna = new TipoAllegato { Nome = "Comunicazione di invio documentazione" };
            var invioconsegnaavvenuto = new TipoAllegato { Nome = "Comunicazione di avvenuto invio documentazione" };
            var altro = new TipoAllegato { Nome = "Altro" };

            //var tipoAllegato = new List<TipoAllegato>
            //{
            //    new TipoAllegato { Nome = "Modulo Richiesta", Obbligatorio = true, ErrorMessage = "Attenzione manca Sottoscrizione richiesta", StatoSuccessivoObbligatorio = evasa },
            //    new TipoAllegato{Nome="Disposizione di addebito spese", Obbligatorio=false , StatoSuccessivoObbligatorio=null},
            //    new TipoAllegato{Nome="Comunicazione di diniego", Obbligatorio=false , StatoSuccessivoObbligatorio=null},
            //    new TipoAllegato{Nome="Avviso di giacenza documentazione", Obbligatorio=true, ErrorMessage="Attenzione manca Ricevuta invio comunicazione" , StatoSuccessivoObbligatorio=evasa},
            //    new TipoAllegato{Nome="Ricevuta consegna documentazione", Obbligatorio=true, ErrorMessage="Attenzione manca Ricevuta consegna documentazione" , StatoSuccessivoObbligatorio=evasa},
            //    new TipoAllegato{Nome="Altro", Obbligatorio=false , StatoSuccessivoObbligatorio=null},
            //};
            //tipoAllegato.ForEach(s => context.TipoAllegato.Add(s));
            context.TipoAllegato.Add(richiestadoc);
            context.TipoAllegato.Add(modulorichiesta);
            context.TipoAllegato.Add(addebito);
            context.TipoAllegato.Add(diniego);
            context.TipoAllegato.Add(avvisogiacenza);
            context.TipoAllegato.Add(consegnaPecRacc);
            context.TipoAllegato.Add(consegnaFiliale);
            context.TipoAllegato.Add(invioconsegna);
            context.TipoAllegato.Add(invioconsegnaavvenuto);
            context.TipoAllegato.Add(altro);
            context.SaveChanges();

            var condizioniallegati = new List<CondizioniAllegati>
            { 
                ////////stato evasa
                ////////////// consegna filiale
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanaleFiliale, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanaleFiliale, TipoAllegato= consegnaFiliale,  ErrorMessage = "Attenzione manca Ricevuta di avvenuta consegna documentazione in Filiale"},

                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleFiliale, TipoAllegato= richiestadoc,  ErrorMessage = "Attenzione manca Richiesta documenti"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleFiliale, TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleFiliale, TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleFiliale, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleFiliale, TipoAllegato= consegnaFiliale,  ErrorMessage = "Attenzione manca Ricevuta di avvenuta consegna documentazione in Filiale"},

                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanaleFiliale, TipoAllegato= richiestadoc,  ErrorMessage = "Attenzione manca Richiesta documenti"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanaleFiliale, TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanaleFiliale, TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanaleFiliale, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanaleFiliale, TipoAllegato= consegnaFiliale,  ErrorMessage = "Attenzione manca Ricevuta di avvenuta consegna documentazione in Filiale"},

                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleEmail, CanaleConsegna=CanaleFiliale, TipoAllegato= richiestadoc,  ErrorMessage = "Attenzione manca Richiesta documenti"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleEmail, CanaleConsegna=CanaleFiliale, TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleEmail, CanaleConsegna=CanaleFiliale, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleEmail, CanaleConsegna=CanaleFiliale, TipoAllegato= consegnaFiliale,  ErrorMessage = "Attenzione manca Ricevuta di avvenuta consegna documentazione in Filiale"},

                ///////////////// consegna raccomandata
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleRaccomandata, TipoAllegato= richiestadoc,  ErrorMessage = "Attenzione manca Richiesta documenti"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleRaccomandata, TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleRaccomandata, TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleRaccomandata, TipoAllegato= invioconsegna,  ErrorMessage = "Attenzione manca Comunicazione di invio documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanaleRaccomandata, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna RACCOMANDATA"},

                //////////////// consegna pec
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanalePec, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanalePec, TipoAllegato= invioconsegna,  ErrorMessage = "Attenzione manca Comunicazione di invio documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanalePec, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna PEC"},

                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanalePec, TipoAllegato= richiestadoc,  ErrorMessage = "Attenzione manca Richiesta documenti"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanalePec, TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanalePec, TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanalePec, TipoAllegato= invioconsegna,  ErrorMessage = "Attenzione manca Comunicazione di invio documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleRaccomandata, CanaleConsegna=CanalePec, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna PEC"},

                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanalePec, TipoAllegato= richiestadoc,  ErrorMessage = "Attenzione manca Richiesta documenti"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanalePec, TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanalePec, TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanalePec, TipoAllegato= invioconsegna,  ErrorMessage = "Attenzione manca Comunicazione di invio documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanalePec, CanaleConsegna=CanalePec, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna PEC"},
                //////////// consegna email
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanaleEmail, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanaleEmail, TipoAllegato= invioconsegna,  ErrorMessage = "Attenzione manca Comunicazione di invio documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanaleEmail, TipoAllegato= invioconsegnaavvenuto,  ErrorMessage = "Attenzione manca Comunicazione di avvenuto invio documentazione"},
                new CondizioniAllegati{ StatoOut=evasa, CanaleRicezione=CanaleFiliale, CanaleConsegna=CanaleEmail, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna RACCOMANDATA"},

                ///////// stato archiviata
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleFiliale, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleFiliale,  TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleFiliale,  TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},

                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleRaccomandata, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleRaccomandata,  TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleRaccomandata,  TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},

                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanalePec, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanalePec,  TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanalePec,  TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},

                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleEmail, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleEmail,  TipoAllegato= avvisogiacenza,  ErrorMessage = "Attenzione manca Avviso di giacenza documentazione"},
                new CondizioniAllegati{ StatoOut=archiviata, CanaleRicezione=CanaleEmail,  TipoAllegato= ricevavvisogiacenza,  ErrorMessage = "Attenzione manca Ricevuta di avviso giacenza documentazione"},

                ///////// stato respinta
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleFiliale, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleFiliale,  TipoAllegato= diniego,  ErrorMessage = "Attenzione manca Comunicazione di diniego"},
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleFiliale, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna Raccomandata/PEC"},

                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleRaccomandata, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleRaccomandata,  TipoAllegato= diniego,  ErrorMessage = "Attenzione manca Comunicazione di diniego"},
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleRaccomandata, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna Raccomandata/PEC"},

                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanalePec, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanalePec,  TipoAllegato= diniego,  ErrorMessage = "Attenzione manca Comunicazione di diniego"},
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanalePec, TipoAllegato= consegnaPecRacc,  ErrorMessage = "Attenzione manca Ricevuta di consegna PEC"},

                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleEmail, TipoAllegato= modulorichiesta,  ErrorMessage = "Attenzione manca Modulo di richiesta"},
                new CondizioniAllegati{ StatoOut=respinta, CanaleRicezione=CanaleEmail,  TipoAllegato= diniego,  ErrorMessage = "Attenzione manca Comunicazione di diniego"},


            };
            condizioniallegati.ForEach(s => context.CondizioniAllegati.Add(s));
            context.SaveChanges();





            var tipiDocumenti = new List<TipiDocumenti>
            {

                new TipiDocumenti{Nome="assegni", Descrizione= "Copia assegni"},
                new TipiDocumenti{Nome="contrattuale", Descrizione= "Documentazione contrattuale"},
                new TipiDocumenti{Nome="contabile", Descrizione= "Documentazione contabile"},
                new TipiDocumenti{Nome="trasparenza", Descrizione= "Documentazione di trasparenza"},
                new TipiDocumenti{Nome="estrattoconto", Descrizione= "Duplicato estratto conto"},
                new TipiDocumenti{Nome="altro", Descrizione= "Altro"},
            };
            tipiDocumenti.ForEach(s => context.TipiDocumenti.Add(s));
            context.SaveChanges();


            var tipiPagamento = new List<TipiPagamento>
            {
                new TipiPagamento{Nome="Conto", Descrizione= "Addebito su conto"},
                new TipiPagamento{Nome="Cassa", Descrizione= "Pagamento in cassa"},
               // new TipiPagamento{Nome="Iban", Descrizione= "Pagamento su Iban"},
            };
            tipiPagamento.ForEach(s => context.TipiPagamento.Add(s));
            context.SaveChanges();


            var parametriRichiestaConsegna = new List<ParametriRichiestaConsegna>
            {
                new ParametriRichiestaConsegna{Richiesta=CanaleFiliale, Consegna= CanaleFiliale},
                new ParametriRichiestaConsegna{Richiesta=CanaleFiliale, Consegna= CanalePec},
                new ParametriRichiestaConsegna{Richiesta=CanaleFiliale, Consegna= CanaleEmail},
                //new ParametriRichiestaConsegna{Richiesta=CanaleFiliale, Consegna= CanaleRaccomandata},

                new ParametriRichiestaConsegna{Richiesta=CanalePec, Consegna= CanaleFiliale},
                new ParametriRichiestaConsegna{Richiesta=CanalePec, Consegna= CanalePec},
                new ParametriRichiestaConsegna{Richiesta=CanalePec, Consegna= CanaleRaccomandata},

                new ParametriRichiestaConsegna{Richiesta=CanaleEmail, Consegna= CanaleFiliale},

                new ParametriRichiestaConsegna{Richiesta=CanaleRaccomandata, Consegna= CanaleFiliale},
                new ParametriRichiestaConsegna{Richiesta=CanaleRaccomandata, Consegna= CanalePec},
                new ParametriRichiestaConsegna{Richiesta=CanaleRaccomandata, Consegna= CanaleRaccomandata},
            };
            parametriRichiestaConsegna.ForEach(s => context.ParametriRichiestaConsegna.Add(s));
            context.SaveChanges();


            var elementi = new List<Elementi>();
            foreach (PropertyInfo propertyInfo in typeof(Richieste).GetProperties())
            {
                var prop = propertyInfo.PropertyType;
                var tipo = prop.Name;
                if (prop.FullName.Contains("DateTime"))
                {
                    tipo = "DateTime";
                }
                if (prop.FullName.Contains("Boolean"))
                {
                    tipo = "Boolean";
                }
                string placehoder = "";
                if (tipo == "DateTime")
                {
                    placehoder = "dd/MM/yyyy";
                }
                if (tipo == "String")
                {
                    placehoder = propertyInfo.Name;
                }
                if (placehoder == "Richiedente") placehoder = "";
                if (placehoder == "IndirizzoRichiedente") placehoder = "";
                if (placehoder == "Note") placehoder = "";
                if (placehoder == "Luogo") placehoder = "";
                if (placehoder == "Filiale") placehoder = "";
                if (placehoder == "MotivazioneBypass") placehoder = "";
                if (placehoder == "CodiceFiscaleRichiedente") placehoder = "";
                if (placehoder == "LuogoNascitaRichiedente") placehoder = "";

                elementi.Add(new Elementi()
                {
                    IdElemento = propertyInfo.Name,
                    Nome = propertyInfo.Name,
                    Tipo = tipo,
                    PlaceHolder = placehoder,
                    //
                });
            }
            elementi.ForEach(s => context.Elementi.Add(s));
            context.SaveChanges();

            var TipiRichiedente = new List<TipiRichiedente>();

            var Curatore = new TipiRichiedente { Nome = "Curatore", Descrizione = "Curatore Fallimentare (solo per istanze di competenza UO centrale)" };
            var Titolare = new TipiRichiedente { Nome = "Titolare", Descrizione = "Titolare, erede o altro soggetto" };
            TipiRichiedente.Add(Curatore);
            TipiRichiedente.Add(Titolare);
            TipiRichiedente.ForEach(s => context.TipiRichiedente.Add(s));
            context.SaveChanges();


            var eleNDG = context.Elementi.Where(a => a.IdElemento == "NDG").FirstOrDefault();
            var eleMotivazione = context.Elementi.Where(a => a.IdElemento == "Motivazione").FirstOrDefault();
            var eleLegittimato = context.Elementi.Where(a => a.IdElemento == "Legittimato").FirstOrDefault();      ////////////modificato
            var eleDataRichiesta = context.Elementi.Where(a => a.IdElemento == "DataRichiesta").FirstOrDefault();
            var eleDataAvviso = context.Elementi.Where(a => a.IdElemento == "DataAvviso").FirstOrDefault();
            var eleDataIIAvviso = context.Elementi.Where(a => a.IdElemento == "DataIIAvviso").FirstOrDefault();
            var eleDataConsegna = context.Elementi.Where(a => a.IdElemento == "DataConsegna").FirstOrDefault();
            var eleCessione = context.Elementi.Where(a => a.IdElemento == "Cessione").FirstOrDefault();
            var eleTipoRichiedente = context.Elementi.Where(a => a.IdElemento == "TipoRichiedente").FirstOrDefault();
            var eleCanalerichiesta = context.Elementi.Where(a => a.IdElemento == "CanaleRicezione").FirstOrDefault();//aggiunto
            var eleCanaleconsegna = context.Elementi.Where(a => a.IdElemento == "CanaleConsegna").FirstOrDefault();//aggiunto
            var eleUOCodeInoltro = context.Elementi.Where(a => a.IdElemento == "UOCode_Inoltro").FirstOrDefault();//aggiunto
            var eleFiliale = context.Elementi.Where(a => a.IdElemento == "Filiale").FirstOrDefault();//aggiunto
            var eleLuogo = context.Elementi.Where(a => a.IdElemento == "Luogo").FirstOrDefault();//aggiunto
            var eleRichiedente = context.Elementi.Where(a => a.IdElemento == "Richiedente").FirstOrDefault();//aggiunto
            var eleIndirizzoRichiedente = context.Elementi.Where(a => a.IdElemento == "IndirizzoRichiedente").FirstOrDefault();//aggiunto
            var condStato = new List<CondizioneStato>();

            ////////// da inzio a inserita
            //var inzio_inserita = new CondizioneStato { StatoIn = inizio, StatoOut = inserita, Notifica = true, NotificaResp = false };
            ////////////condizioni discriminanti cambio stato

            ///////////campi richiesti
            //inzio_inserita.Richiesti.Add(new Richiesti { Elemento = eleNDG, ErrorMessage = "Richiesto", Min = 1 });
            //inzio_inserita.Richiesti.Add(new Richiesti { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });

            ////////// da inzio a annullata
            //var inzio_annullata = new CondizioneStato { operationType = "Annulla", StatoIn = inizio, StatoOut = annullata, Notifica = false, NotificaResp = false };////////////modificato
            /////inzio_annullata.Richiesti.Add(new Richiesti { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 7 });////////////modificato


            ////////// da inizio a in lavorazione o assegnata saltando inserita
            //var inizio_inlavorazione = new CondizioneStato { StatoIn = inizio, StatoOut = inlavorazione, Notifica = false, NotificaResp = false };
            //inizio_inlavorazione.Condizioni.Add(new Condizione { Elemento = eleLegittimato, Valore = 1 });
            //inizio_inlavorazione.Condizioni.Add(new Condizione { Elemento = eleTipoRichiedente, Valore = Titolare.Id });
            //inizio_inlavorazione.Condizioni.Add(new Condizione { Elemento = eleCessione, Valore = 0 });

            //inizio_inlavorazione.Richiesti.Add(new Richiesti { Elemento = eleNDG, ErrorMessage = "Richiesto", Min = 1 });
            //inizio_inlavorazione.Richiesti.Add(new Richiesti { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });
            ////inizio_inlavorazione.Richiesti.Add(new Richiesti { Elemento = eleLegittimato, ErrorMessage = "Richiesto", Min = 1 });

            //var inizio_assegnata = new CondizioneStato { StatoIn = inizio, StatoOut = assegnata, Notifica = true, NotificaResp = false };
            //inizio_assegnata.Condizioni.Add(new Condizione { Elemento = eleTipoRichiedente, Valore = Curatore.Id });

            //inizio_assegnata.Richiesti.Add(new Richiesti { Elemento = eleNDG, ErrorMessage = "Richiesto", Min = 1 });
            //inizio_assegnata.Richiesti.Add(new Richiesti { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });

            //var inizio_assegnata2 = new CondizioneStato { StatoIn = inizio, StatoOut = assegnata, Notifica = true, NotificaResp = false };
            //inizio_assegnata2.Condizioni.Add(new Condizione { Elemento = eleTipoRichiedente, Valore = Titolare.Id });
            //inizio_assegnata2.Condizioni.Add(new Condizione { Elemento = eleLegittimato, Valore = 1 });
            //inizio_assegnata2.Condizioni.Add(new Condizione { Elemento = eleCessione, Valore = 1 });


            //inizio_assegnata2.Richiesti.Add(new Richiesti { Elemento = eleNDG, ErrorMessage = "Richiesto", Min = 1 });
            //inizio_assegnata2.Richiesti.Add(new Richiesti { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });
            ////////// da inizio a in lavorazione o assegnata saltando inserita fine



            //////// da inserita a annullata
            var inserita_annullata = new CondizioneStato { operationType = "Annulla", StatoIn = inserita, StatoOut = annullata };////////////modificato
            /////////campi richiesti
            //inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Motivazione richiesta minimo 7 caratteri", Min = 7 });////////////modificato
            //seguenti commentati 05/04/22
            //inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto canale richiesta", Min = 1 });////////////modificato
            //inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesta Data Richiesta", Min = 2021 });////////////modificato
            //inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            //inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });


            ////////// da inserita a respinta
            var inserita_respinta = new CondizioneStato { TipoRichiedente = Titolare, StatoIn = inserita, StatoOut = respinta };
            ////////////condizioni discriminanti cambio stato
            inserita_respinta.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, Valore = 0, ErrorMessage = "Selezione Non legittimato richiesto(se richiedente è Titolare)" });
            ///////////campi richiesti
            //inserita_respinta.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Motivazione richiesta minimo 7 caratteri", Min = 7 });
            inserita_respinta.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inserita_respinta.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });


            //////// da inserita a in lavorazione comune sia che si filale o meno caso legittimato titolare
            var inserita_inlavorazione = new CondizioneStato { OnlyUOCompetente = false, TipoRichiedente = Titolare, StatoIn = inserita, StatoOut = inlavorazione };
            //////////condizioni discriminanti cambio stato
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta", Min = 1 });////////////modificato
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto Data richiesta", Min = 2021 });////////////modificato
            //inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            //inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Titolare.Id, ErrorMessage = "Richiesta selezione Tipo Richiedente" });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Min = 1, ErrorMessage = "Richiesta selezione Tipo Richiedente" });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, Valore = 1, ErrorMessage = "Nel caso di Titolare è richiesto che sia Legittimato" });
            //inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleCessione, Valore = 0, ErrorMessage = "Richiesta selezione cessione non rilevata" });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleCessione, Min = 1, ErrorMessage = "Se il richiedente è Titolare la selezione se Cessione rilevata è richiesta" });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });


            ////////// da inserita a in lavorazione caso se non sono filiale ed è curatore
            //var inserita_inlavorazione2 = new CondizioneStato { IsFiliale = false, StatoIn = inserita, StatoOut = inlavorazione, Notifica = false, NotificaResp = false };

            //////// da inserita a in lavorazione caso se  curatore
            var inserita_inlavorazione2 = new CondizioneStato { OnlyUOCompetente = false, TipoRichiedente = Curatore, StatoIn = inserita, StatoOut = inlavorazione };
            //////////condizioni discriminanti cambio stato
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta", Min = 1 });////////////modificato
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto Data richiesta", Min = 2021 });////////////modificato
            //inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            //inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Curatore.Id, ErrorMessage = "Se il richiedente è un Curatore selezionarlo in Tipo Richiedente" });
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Min = 1, ErrorMessage = "Richiesta selezione Tipo Richiedente" });
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleRichiedente, ErrorMessage = "Se il richiedente è un Curatore compilare il campo Richiedente, minimo 7 caratteri", Min = 7 });
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleIndirizzoRichiedente, ErrorMessage = "Se il richiedente è un Curatore compilare il campo Indirizzo Richiedente, minimo 7 caratteri", Min = 7 });
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inserita_inlavorazione2.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });


            ////////// da inserita a in lavorazione caso se non sono filiale ed è curatore
            //var inserita_inlavorazione3 = new CondizioneStato { IsFiliale = false, StatoIn = inserita, StatoOut = inlavorazione, Notifica = false, NotificaResp = false };
            ////////////condizioni discriminanti cambio stato
            //inserita_inlavorazione3.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente (Cessione rilevata)" });
            //inserita_inlavorazione3.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Titolare.Id, ErrorMessage = "Richiesto selezione Titolare  (Cessione rilevata)" });
            //inserita_inlavorazione3.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, Valore = 1, ErrorMessage = "Richiesta selezione Legittimato (Cessione rilevata)" });
            //inserita_inlavorazione3.Richiesti.Add(new Condizioni { Elemento = eleCessione, Valore = 1, ErrorMessage = "Richiesta selezione Cessione rilevata" });
            //inserita_inlavorazione3.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta (Cessione rilevata)", Min = 1 });////////////modificato
            //inserita_inlavorazione3.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto Data richiesta (Cessione rilevata)", Min = 2021 });////////////modificato



            //////// da inserita a assegnata caso legittimato titolare
            var inserita_assegnata2 = new CondizioneStato { TipoRichiedente = Titolare, StatoIn = inserita, StatoOut = assegnata };
            //////////condizioni discriminanti cambio stato
            //inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta", Min = 1 });////////////modificato
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto Data richiesta", Min = 2021 });////////////modificato
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, ErrorMessage = "E' necessario selezionare il Destinatario di inoltro", Min = 1 });////////////modificato

            //inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Titolare.Id, ErrorMessage = "Richiesta selezione Tipo Richiedente" });
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Min = 1, ErrorMessage = "Richiesta selezione Tipo Richiedente" });

            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, Valore = 1, ErrorMessage = "Nel caso di Titolare è richiesto che sia Legittimato" });

            //inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleCessione, Valore = 1, ErrorMessage = "Richiesta selezione Cessione rilevata" });
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleCessione, Min = 1, ErrorMessage = "Se il richiedente è Titolare la selezione se Cessione rilevata è richiesta" });


            //////// da inserita a assegnata caso curatore
            var inserita_assegnata = new CondizioneStato { TipoRichiedente = Curatore, StatoIn = inserita, StatoOut = assegnata };
            //////////condizioni discriminanti cambio stato
            //inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta", Min = 1 });////////////modificato
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto Data richiesta", Min = 2021 });////////////modificato
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, ErrorMessage = "E' necessario selezionare il Destinatario di inoltro", Min = 1 });////////////modificato
            //inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Curatore.Id, ErrorMessage = "Se il richiedente è un Curatore selezionarlo in Tipo Richiedente" });
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Min = 1, ErrorMessage = "Richiesta selezione Tipo Richiedente" });
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleRichiedente, ErrorMessage = "Se il richiedente è un Curatore compilare il campo Richiedente, minimo 7 caratteri", Min = 7 });
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleIndirizzoRichiedente, ErrorMessage = "Se il richiedente è un Curatore compilare il campo Indirizzo Richiedente, minimo 7 caratteri", Min = 7 });





            //////// da inserita a inlavorazione fine







            //////// da assegnata a inlavorazione
            var assegnata_inlavorazione = new CondizioneStato { StatoIn = assegnata, StatoOut = inlavorazione };

            //////// da assegnata a assegnata
            var assegnata_assegnata = new CondizioneStato { StatoIn = assegnata, StatoOut = assegnata };

            //////// da assegnata a respintaxmodifica
            //var assegnata_respintaxmodifica = new CondizioneStato { operationType = "Respingixmod", StatoIn = assegnata, StatoOut = respintaxmodifica, Notifica = true, NotificaResp = false };////////////modificato
            //assegnata_respintaxmodifica.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 7 });
            ////// da in_lavorazione (uo) a respintaxmodifica
            var lavorazione_respintaxmodifica = new CondizioneStato { operationType = "Respingixmod", StatoIn = inlavorazione, StatoOut = respintaxmodifica, IsSameUO = false };////////////modificato
            //lavorazione_respintaxmodifica.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Motivazione richiesta minimo 7 caratteri", Min = 7 });
            lavorazione_respintaxmodifica.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato


            var inlavorazione_documentiingiacenza = new CondizioneStato { StatoIn = inlavorazione, StatoOut = documentiingiacenza };
            inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            //inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleDataAvviso, ErrorMessage = "Data di avviso comunicazione richiesta", Min = 2021 });
            inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato
            inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, Valore = 0, ErrorMessage = "Data di consegna non può essere presente" });
            inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });

            var inlavorazione_assegnata = new CondizioneStato { StatoIn = inlavorazione, StatoOut = assegnata };
            inlavorazione_assegnata.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            //inlavorazione_assegnata.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inlavorazione_assegnata.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, ErrorMessage = "E' richiesto il Destinatario di inoltro", Min = 1 });////////////modificato

            var documentiingiacenza_evasa = new CondizioneStato { StatoIn = documentiingiacenza, StatoOut = evasa };
            documentiingiacenza_evasa.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            //documentiingiacenza_evasa.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            documentiingiacenza_evasa.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Data di consegna documentazione richiesta", Min = 2021 });
            documentiingiacenza_evasa.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            documentiingiacenza_evasa.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });

            var inlavorazione_evasa1 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa };
            //inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Possibile evadere sono nel caso in cui il canale di ricezione è filiale oppure canale di consegna è Pec, Email o Raccomandata", Valore = CanaleFiliale.Id });
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto Canale consegna", Min = 1 });////////////modificato
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Data di consegna richiesta", Min = 2021 });
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });

            var inlavorazione_evasa2 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa };
            //inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta", Min = 1 });////////////modificato
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Possibile evadere sono nel caso in cui il canale di ricezione è filiale oppure canale di consegna è Pec, Email o Raccomandata", Valore = CanalePec.Id });
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Data di consegna richiesta", Min = 2021 });
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });

            var inlavorazione_evasa3 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa };
            //inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta", Min = 1 });////////////modificato
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Possibile evadere sono nel caso in cui il canale di ricezione è filiale oppure canale di consegna è Pec, Email o Raccomandata", Valore = CanaleEmail.Id });
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Data di consegna richiesta", Min = 2021 });
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });

            var inlavorazione_evasa4 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa };
            //inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, Valore = 0, ErrorMessage = "Motivazione deve essere assente" });
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto Canale richiesta", Min = 1 });////////////modificato
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Possibile evadere sono nel caso in cui il canale di ricezione è filiale oppure canale di consegna è Pec, Email o Raccomandata", Valore = CanaleRaccomandata.Id });
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Data di consegna richiesta", Min = 2021 });
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, Valore = 0, ErrorMessage = "Il Destinatario di inoltro deve essere presente solo in caso di assegnazione" });////////////modificato
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });


            var documentiingiacenza_archiviata = new CondizioneStato { operationType = "Archivia", StatoIn = documentiingiacenza, StatoOut = archiviata };////////////modificato
            //documentiingiacenza_archiviata.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Motivazione richiesta minimo 7 caratteri", Min = 7 });
            documentiingiacenza_archiviata.Richiesti.Add(new Condizioni { Elemento = eleDataAvviso, ErrorMessage = "Data di avviso comunicazione richiesto", Min = 2021 });
            documentiingiacenza_archiviata.Richiesti.Add(new Condizioni { Elemento = eleFiliale, ErrorMessage = "Filiale richiesta minimo 7 caratteri", Min = 7 });
            documentiingiacenza_archiviata.Richiesti.Add(new Condizioni { Elemento = eleLuogo, ErrorMessage = "Luogo richiesto minimo 3 caratteri", Min = 3 });


            //condStato.Add(inzio_inserita);
            //condStato.Add(inizio_inlavorazione);
            //condStato.Add(inizio_assegnata);
            //condStato.Add(inizio_assegnata2);

            condStato.Add(inserita_annullata);
            condStato.Add(inserita_respinta);
            condStato.Add(inserita_inlavorazione);
            condStato.Add(inserita_inlavorazione2);
            // condStato.Add(inserita_inlavorazione3);
            condStato.Add(inserita_assegnata);
            condStato.Add(inserita_assegnata2);
            condStato.Add(assegnata_inlavorazione);
            condStato.Add(assegnata_assegnata);
            condStato.Add(lavorazione_respintaxmodifica);
            condStato.Add(inlavorazione_documentiingiacenza);
            condStato.Add(inlavorazione_assegnata);
            condStato.Add(inlavorazione_evasa1);
            condStato.Add(inlavorazione_evasa2);
            condStato.Add(inlavorazione_evasa3);
            condStato.Add(inlavorazione_evasa4);
            condStato.Add(documentiingiacenza_evasa);
            condStato.Add(documentiingiacenza_archiviata);
            condStato.ForEach(s => context.CondizioneStato.Add(s));
            context.SaveChanges();

            var avvisi = new List<Avvisi> {
                new Avvisi{CanaleComunicazione=CanaleEmail,Giorni=40,Messaggio="Attenzione passati 40 giorni dalla comunicazione per email dall'avviso di documenti in giacenza.", IsBanca = false, EmailTesti =
                new EmailTesti
                {
                    Nome = "Avviso",
                    Oggetto = "RCD Avviso",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }},
                new Avvisi{CanaleComunicazione=CanaleEmail,Giorni=45,Messaggio="Scaduti i 45 giorni dalla comunicazione per email dall'avviso di documenti in giacenza.", IsBanca = false , EmailTesti =
                new EmailTesti
                {
                    Nome = "Avviso",
                    Oggetto = "RCD Avviso",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }},
                new Avvisi{CanaleComunicazione=null,Giorni=90,Messaggio="Scaduti i 90 giorni da prima comunicazione al cliente" , IsBanca = false, EmailTesti =
                new EmailTesti
                {
                    Nome = "Avviso",
                    Oggetto = "RCD Avviso",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }},

                new Avvisi{CanaleComunicazione=null,Giorni=60,Messaggio="Attenzione passati 60 giorni da ricezione richiesta" , IsBanca = true, EmailTesti =
                new EmailTesti
                {
                    Nome = "Avviso",
                    Oggetto = "RCD Avviso",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }},
                new Avvisi{CanaleComunicazione=null,Giorni=85,Messaggio="Attenzione passati 85 giorni da ricezione richiesta" , IsBanca = true, EmailTesti =
                new EmailTesti
                {
                    Nome = "Avviso",
                    Oggetto = "RCD Avviso",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }},
                new Avvisi{CanaleComunicazione=null,Giorni=90,Messaggio="Scaduti i 90 giorni da ricezione richiesta" , IsBanca = true, EmailTesti =
                new EmailTesti
                {
                    Nome = "Avviso",
                    Oggetto = "RCD Avviso",
                    Corpo = emailcorpo,
                    Html = true,
                    IsActive = true,
                }},
            };

            avvisi.ForEach(s => context.Avvisi.Add(s));
            context.SaveChanges();


        }


    }
}

/*
            //CAMBIO STATO RESPINTA
            stato = respinta;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="isPerson",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CodiceFiscale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="LuogoNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SedeLegale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Indirizzo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //AGGIUNTA
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleConsegna_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="UOCodeNext",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SpesaAddebitabile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Rapporto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Documento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneDocumento",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Costo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCopie",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DocOrig",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCartone",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AddDocumento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="tipoAllegato_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneFile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="FileForm",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnAddAllegato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCambioStato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCancellaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnModificaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO ANNULLATA
            stato = annullata;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="isPerson",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CodiceFiscale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="LuogoNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SedeLegale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Indirizzo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //AGGIUNTA
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleConsegna_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="UOCodeNext",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SpesaAddebitabile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Rapporto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Documento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneDocumento",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Costo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCopie",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DocOrig",Stato=stato,NomeProprieta="hidden", ValoreProprieta="hidden"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCartone",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AddDocumento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="tipoAllegato_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneFile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="FileForm",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnAddAllegato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCambioStato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCancellaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnModificaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO EVASA
            stato = evasa;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="isPerson",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CodiceFiscale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="LuogoNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SedeLegale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Indirizzo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //AGGIUNTA
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleConsegna_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="UOCodeNext",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SpesaAddebitabile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Rapporto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Documento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneDocumento",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Costo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCopie",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DocOrig",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCartone",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AddDocumento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="tipoAllegato_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneFile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="FileForm",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnAddAllegato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCambioStato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCancellaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnModificaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO ARCHIVIATA
            stato = archiviata;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="isPerson",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CodiceFiscale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="LuogoNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SedeLegale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Indirizzo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                //AGGIUNTA
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleConsegna_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="UOCodeNext",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SpesaAddebitabile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Rapporto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Documento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneDocumento",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Costo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCopie",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DocOrig",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumCartone",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AddDocumento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="tipoAllegato_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DescrizioneFile",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="FileForm",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnAddAllegato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCambioStato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnCancellaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnModificaDoc",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();
            */
