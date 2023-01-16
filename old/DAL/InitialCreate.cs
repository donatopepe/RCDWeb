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

            var ErroriGenerici = new List<ErroriGenerici>
            {
                new ErroriGenerici{ Nome="tipoAllegato",ErrorMessage="Richiesto"},
                new ErroriGenerici{ Nome="DescrizioneFile",ErrorMessage="Richiesto"},
                new ErroriGenerici{ Nome="FileForm"},
                new ErroriGenerici{ Nome="Documento"},
                new ErroriGenerici{ Nome="DescrizioneDocumento"},
                new ErroriGenerici{ Nome="NumCopie"},
                new ErroriGenerici{ Nome="Costo"},
                new ErroriGenerici{ Nome="pec"},
                new ErroriGenerici{ Nome="pecnonvalida",ErrorMessage="Pec non valida."},
                new ErroriGenerici{ Nome="email"},
                new ErroriGenerici{ Nome="emailnonvalida",ErrorMessage="email non valida."},
                new ErroriGenerici{ Nome="FilialeConsegna"},
                new ErroriGenerici{ Nome="NumeroConto"},
                new ErroriGenerici{ Nome="Documenti",ErrorMessage="Attenzione non sono presenti documenti richiesti."},
                new ErroriGenerici{ Nome="DataRichiesta",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataAvviso",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataIIAvviso",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataConsegna",ErrorMessage="Attenzione Data non può essere superiore alla data di oggi."},
                new ErroriGenerici{ Nome="DataAvvisoRispettoDataRichiesta",ErrorMessage="Attenzione Data Avviso non può essere inferiore a Data Richiesta."},
                new ErroriGenerici{ Nome="DataIIAvvisoRispettoDataAvviso",ErrorMessage="Attenzione Data Avviso II non può essere inferiore a Data Avviso."},
                new ErroriGenerici{ Nome="DataConsegnaRispettoDataAvviso",ErrorMessage="Attenzione Data Consegna non può essere inferiore a Data Avviso."},
                new ErroriGenerici{ Nome="DataConsegnaRispettoDataRichiesta",ErrorMessage="Attenzione Data Consegna non può essere inferiore a Data Richiesta."},
                new ErroriGenerici{ Nome="ModalitaPagamento",ErrorMessage="Richiesto"},
                new ErroriGenerici{ Nome="Richiedente",ErrorMessage="Richiesto"},
            };
            ErroriGenerici.ForEach(s => context.ErroriGenerici.Add(s));
            context.SaveChanges();

            var uffici = new List<Uffici>
            {
                new Uffici{ Nome="Uffico1",UOCODE="2424",Istituto=46},
                new Uffici{ Nome="Uffico2",UOCODE="12324",Istituto=46},
                new Uffici{ Nome="Uffico3",UOCODE="24242",Istituto=46},
                new Uffici{ Nome="Uffico4",UOCODE="23424",Istituto=46},
                new Uffici{ Nome="Uffico5",UOCODE="2432",Istituto=47},
                new Uffici{ Nome="Uffico6",UOCODE="36464",Istituto=47},
                new Uffici{ Nome="Uffico7",UOCODE="47547",Istituto=47},
                new Uffici{ Nome="Uffico8",UOCODE="476457",Istituto=47},
            };
            uffici.ForEach(s => context.Uffici.Add(s));
            context.SaveChanges();

            var stati = new List<Stati>();
            //var inizio = new Stati { Stato = "Inizio", Descrizione = "Inserimento", Chiuso = false };
            var inserita = new Stati { Stato = "Inserita", Descrizione = "Stato inserita", Chiuso = false };
            var assegnata = new Stati { Stato = "Assegnata", Descrizione = "Assegnata a UO", Chiuso = false };
            var inlavorazione = new Stati { Stato = "In Lavorazione", Descrizione = "Stato in lavorazione" };
            var respinta = new Stati { Stato = "Respinta", Descrizione = "Stato respinta", DataStopBanca = true, Chiuso = true };
            var annullata = new Stati { Stato = "annullata", Descrizione = "Stato annullata", DataStopBanca = true, Chiuso = true };
            var respintaxmodifica = new Stati { Stato = "Respinta per modifica", Descrizione = "Stato respinta per modifica", Chiuso = false };
            var documentiingiacenza = new Stati { Stato = "Documenti in Giacenza", Descrizione = "Stato documenti in giacenza", DataStopBanca = true, Chiuso = false };
            var evasa = new Stati { Stato = "Evasa", Descrizione = "Stato Evasa", DataStopBanca = true, Chiuso = true };
            var archiviata = new Stati { Stato = "Archiviata", Descrizione = "Stato archiviata", DataStopBanca = true, Chiuso = true };
            //stati.Add(inizio);
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

            //var stato = inizio;
            //var abilitaElementiStato = new List<AbilitaElementiStato>
            //{
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="isPerson",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="CodiceFiscale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="LuogoNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="DataNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="SedeLegale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Indirizzo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="class*",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="class*",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="class*",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="class*",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="class*",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            //};
            //abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            //context.SaveChanges();

            //CAMBIO STATO INSERITA
            var stato = inserita;
            var abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="isPerson",Stato=stato,NomeProprieta="hidden", ValoreProprieta="hidden"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CodiceFiscale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="LuogoNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataNascita",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="SedeLegale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Indirizzo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="legittimato2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="cessione2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            //    new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO INLAVORAZIONE
            stato = inlavorazione;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Istituto",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="class~",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="class~",NomeElemento="isPerson",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="IntestazioneNDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="PIva",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="SedeLegale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="Indirizzo",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="UOCode_Inoltro",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="class~",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},


            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();


            //CAMBIO STATO DOC IN GIACENZA
            stato = documentiingiacenza;
            abilitaElementiStato = new List<AbilitaElementiStato>
            {
                new AbilitaElementiStato{Attributo="id",NomeElemento="NDG",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Istituto",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="IscrizioneCCIAA",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="AlboProfessionale",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Richiedente",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleRicezione_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                //new AbilitaElementiStato{Attributo="id",NomeElemento="CanaleConsegna_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
               // new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="FilialeConsegna",Stato=stato,NomeProprieta="readonly", ValoreProprieta="true"},

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
                new AbilitaElementiStato{Attributo="id",NomeElemento="UOCode_Inoltro",Stato=stato,NomeProprieta="disabled", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataIIAvviso2",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},


            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();

            //CAMBIO STATO ASSEGNATA
            stato = assegnata;
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
                new AbilitaElementiStato{Attributo="id",NomeElemento="ModalitaPagamento_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="NumeroConto",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="email",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="pec",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Titolare",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="TipoRichiedente_Id",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Legittimato",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Cessione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="InoltroUfficio",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Motivazione",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Note",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},

                  new AbilitaElementiStato{Attributo="class*",NomeElemento="DataPagamento",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataIIAvviso",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataConsegna",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class*",NomeElemento="DataChiusa",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
               new AbilitaElementiStato{Attributo="id",NomeElemento="Filiale",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="Luogo",Stato=stato,NomeProprieta="hidden", ValoreProprieta="false"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnArchivia",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
            //    new AbilitaElementiStato{Attributo="id",NomeElemento="btnRespingixmod",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="id",NomeElemento="btnAnnulla",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},
                new AbilitaElementiStato{Attributo="class~",NomeElemento="DataRichiesta",Stato=stato,NomeProprieta="hidden", ValoreProprieta="true"},

            };
            abilitaElementiStato.ForEach(s => context.AbilitaElementiStato.Add(s));
            context.SaveChanges();


            var tipoAllegato = new List<TipoAllegato>
            {
                new TipoAllegato{Nome="Altro", Obbligatorio=false , StatoSuccessivoObbligatorio=null},
                new TipoAllegato{Nome="Disposizione di addebito spese", Obbligatorio=false , StatoSuccessivoObbligatorio=null},
                new TipoAllegato{Nome="Comunicazione di diniego", Obbligatorio=false , StatoSuccessivoObbligatorio=null},
                new TipoAllegato{Nome="Sottoscrizione richiesta", Obbligatorio=true , StatoSuccessivoObbligatorio=evasa},
                new TipoAllegato{Nome="Ricevuta invio comunicazione", Obbligatorio=true , StatoSuccessivoObbligatorio=documentiingiacenza},
                new TipoAllegato{Nome="Ricevuta consegna documentazione", Obbligatorio=true , StatoSuccessivoObbligatorio=evasa},
            };
            tipoAllegato.ForEach(s => context.TipoAllegato.Add(s));
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

            var tipiDocumenti = new List<TipiDocumenti>
            {
                new TipiDocumenti{Nome="altro", Descrizione= "Altro"},
                new TipiDocumenti{Nome="assegni", Descrizione= "Richiesta copia assegni"},
                new TipiDocumenti{Nome="contrattuale", Descrizione= "Documentazione contrattuale"},
                new TipiDocumenti{Nome="contabile", Descrizione= "Documentazione contabile"},
                new TipiDocumenti{Nome="trasparenza", Descrizione= "Documentazione di trasparenza"},
                new TipiDocumenti{Nome="estrattoconto", Descrizione= "Duplicato estratto conto"},
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
                new ParametriRichiestaConsegna{Richiesta=CanaleFiliale, Consegna= CanaleRaccomandata},

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

            var Curatore = new TipiRichiedente { Nome = "Curatore", Descrizione = "Curatore" };
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

            var condStato = new List<CondizioneStato>();

            ////////// da inzio a inserita
            //var inzio_inserita = new CondizioneStato { StatoIn = inizio, StatoOut = inserita, Notifica = true, NotificaResp = false };
            ////////////condizioni discriminanti cambio stato

            ///////////campi richiesti
            //inzio_inserita.Richiesti.Add(new Richiesti { Elemento = eleNDG, ErrorMessage = "Richiesto", Min = 1 });
            //inzio_inserita.Richiesti.Add(new Richiesti { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });

            ////////// da inzio a annullata
            //var inzio_annullata = new CondizioneStato { operationType = "Annulla", StatoIn = inizio, StatoOut = annullata, Notifica = false, NotificaResp = false };////////////modificato
            /////inzio_annullata.Richiesti.Add(new Richiesti { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 8 });////////////modificato


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
            var inserita_annullata = new CondizioneStato { operationType = "Annulla", StatoIn = inserita, StatoOut = annullata, Notifica = false, NotificaResp = false };////////////modificato
                                                                                                                                                                         //////////condizioni discriminanti cambio stato

            /////////campi richiesti
            inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 8 });////////////modificato
            inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto", Min = 1 });////////////modificato
            inserita_annullata.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });////////////modificato

            //////// da inserita a respinta
            var inserita_respinta = new CondizioneStato { StatoIn = inserita, StatoOut = respinta, Notifica = false, NotificaResp = false };
            //////////condizioni discriminanti cambio stato
            inserita_respinta.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, Valore = 0, ErrorMessage = "Richiesto" });
            /////////campi richiesti
            inserita_respinta.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 8 });

            //////// da inserita a inlavorazione
            var inserita_inlavorazione = new CondizioneStato { StatoIn = inserita, StatoOut = inlavorazione, Notifica = false, NotificaResp = false };
            //////////condizioni discriminanti cambio stato
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, Valore = 1, ErrorMessage = "Richiesto" });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Titolare.Id, ErrorMessage = "Richiesto" });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleCessione, Valore = 0, ErrorMessage = "Richiesto" });

            //inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, ErrorMessage = "Richiesto", Min = 1 });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, ErrorMessage = "Richiesto", Min = 1 });
            //           inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleCessione, ErrorMessage = "Richiesto", Min = 1 });
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto", Min = 1 });////////////modificato
            inserita_inlavorazione.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });////////////modificato


            //////// da inserita a assegnata
            var inserita_assegnata = new CondizioneStato { StatoIn = inserita, StatoOut = assegnata, Notifica = true, NotificaResp = false };
            //////////condizioni discriminanti cambio stato
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Curatore.Id, ErrorMessage = "Richiesto" });
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto", Min = 1 });////////////modificato
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });////////////modificato
            inserita_assegnata.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, ErrorMessage = "Richiesto (Cessione Rilevata)", Min = 1 });////////////modificato

            //////// da inserita a assegnata
            var inserita_assegnata2 = new CondizioneStato { StatoIn = inserita, StatoOut = assegnata, Notifica = true, NotificaResp = false };
            //////////condizioni discriminanti cambio stato
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleTipoRichiedente, Valore = Titolare.Id, ErrorMessage = "Richiesto" });
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleLegittimato, Valore = 1, ErrorMessage = "Richiesto (Titolare/Cessione Rilevata)" });
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleCessione, Valore = 1, ErrorMessage = "Richiesto" });
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto", Min = 1 });////////////modificato
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleDataRichiesta, ErrorMessage = "Richiesto", Min = 2021 });////////////modificato
            inserita_assegnata2.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, ErrorMessage = "Richiesto (Curatore)", Min = 1 });////////////modificato


            //////// da inserita a inlavorazione fine







            //////// da assegnata a inlavorazione
            var assegnata_inlavorazione = new CondizioneStato { StatoIn = assegnata, StatoOut = inlavorazione, Notifica = false, NotificaResp = false };

            //////// da assegnata a assegnata
            var assegnata_assegnata = new CondizioneStato { StatoIn = assegnata, StatoOut = assegnata, Notifica = true, NotificaResp = false };

            //////// da assegnata a respintaxmodifica
            //var assegnata_respintaxmodifica = new CondizioneStato { operationType = "Respingixmod", StatoIn = assegnata, StatoOut = respintaxmodifica, Notifica = true, NotificaResp = false };////////////modificato
            //assegnata_respintaxmodifica.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 8 });
            ////// da in_lavorazione (uo) a respintaxmodifica
            var lavorazione_respintaxmodifica = new CondizioneStato { operationType = "Respingixmod", StatoIn = inlavorazione, StatoOut = inserita, Notifica = true, NotificaResp = false };////////////modificato
            lavorazione_respintaxmodifica.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 8 });

            var inlavorazione_documentiingiacenza = new CondizioneStato { StatoIn = inlavorazione, StatoOut = documentiingiacenza, Notifica = true, NotificaResp = false };
            inlavorazione_documentiingiacenza.Richiesti.Add(new Condizioni { Elemento = eleDataAvviso, ErrorMessage = "Richiesto", Min = 2021 });

            var inlavorazione_assegnata = new CondizioneStato { StatoIn = inlavorazione, StatoOut = assegnata, Notifica = true, NotificaResp = false };
            inlavorazione_assegnata.Richiesti.Add(new Condizioni { Elemento = eleUOCodeInoltro, ErrorMessage = "Richiesto", Min = 1 });////////////modificato

            var documentiingiacenza_evasa = new CondizioneStato { StatoIn = documentiingiacenza, StatoOut = evasa, Notifica = true, NotificaResp = false };
            documentiingiacenza_evasa.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Richiesto", Min = 2021 });

            var inlavorazione_evasa1 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa, Notifica = true, NotificaResp = false };
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleCanalerichiesta, ErrorMessage = "Richiesto", Valore = CanaleFiliale.Id });
            inlavorazione_evasa1.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Richiesto", Min = 2021 });

            var inlavorazione_evasa2 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa, Notifica = true, NotificaResp = false };
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto", Valore = CanalePec.Id});
            inlavorazione_evasa2.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Richiesto", Min = 2021 });

            var inlavorazione_evasa3 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa, Notifica = true, NotificaResp = false };
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto", Valore = CanaleEmail.Id });
            inlavorazione_evasa3.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Richiesto", Min = 2021 });

            var inlavorazione_evasa4 = new CondizioneStato { StatoIn = inlavorazione, StatoOut = evasa, Notifica = true, NotificaResp = false };
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleCanaleconsegna, ErrorMessage = "Richiesto", Valore = CanaleRaccomandata.Id });
            inlavorazione_evasa4.Richiesti.Add(new Condizioni { Elemento = eleDataConsegna, ErrorMessage = "Richiesto", Min = 2021 });






            var documentiingiacenza_archiviata = new CondizioneStato { operationType = "Archivia", StatoIn = documentiingiacenza, StatoOut = archiviata, Notifica = true, NotificaResp = false };////////////modificato
            documentiingiacenza_archiviata.Richiesti.Add(new Condizioni { Elemento = eleMotivazione, ErrorMessage = "Richiesto", Min = 8 });
            documentiingiacenza_archiviata.Richiesti.Add(new Condizioni { Elemento = eleDataAvviso, ErrorMessage = "Richiesto", Min = 2021 });

            //condStato.Add(inzio_inserita);
            //condStato.Add(inizio_inlavorazione);
            //condStato.Add(inizio_assegnata);
            //condStato.Add(inizio_assegnata2);

            condStato.Add(inserita_annullata);
            condStato.Add(inserita_respinta);
            condStato.Add(inserita_inlavorazione);
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
                new Avvisi{StatoInzio=documentiingiacenza, StatoFine=null, CanaleComunicazione=CanaleEmail,Giorni=45,Messaggio="Scaduti i 45 giorni dalla comunicazione per email dall'avviso di documenti in giacenza." },
                new Avvisi{StatoInzio=documentiingiacenza, StatoFine=null, CanaleComunicazione=CanaleEmail,Giorni=90,Messaggio="Scaduti i 90 giorni dalla comunicazione per email dall'avviso di documenti in giacenza." },
                new Avvisi{StatoInzio=inserita, StatoFine=documentiingiacenza, CanaleComunicazione=null,Giorni=60,Messaggio="Scaduti i 60 giorni da ricezione richiesta" },
                new Avvisi{StatoInzio=inserita, StatoFine=documentiingiacenza, CanaleComunicazione=null,Giorni=60,Messaggio="Scaduti i 90 giorni da ricezione richiesta" },
            };
            avvisi.ForEach(s => context.Avvisi.Add(s));
            context.SaveChanges();


            var emailcambiostato = new EmailTesti
            {
                Nome = "cambiostato",
                Oggetto = "RCD Cambio Stato",
                Corpo = @"<html>
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
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'>Richiesta stato attuale:</td>
				<td style='background-color:#dce5ea;font-size:11px;font-family:verdana;'><b>{descrizioneStato}</b> </td>
				</tr>

									<tr>
				<td style='background-color:#dce5ea;font-size:10px;font-family:verdana;'> Data Richiesta: </td>
				<td style='background-color:#F4F4F4;font-size:10px;font-family:verdana;'><b>{dataRichiesta}</b></td>
				</tr>						
				<tr>
				<td style='background-color:#dce5ea;font-size:10px;font-family:verdana;'> NDG: </td>
				<td style='background-color:#F4F4F4;font-size:10px;font-family:verdana;'><b>{ndg}</b></td>
				</tr>
                <tr>
				<td style='background-color:#dce5ea;font-size:10px;font-family:verdana;'> UO Code iniziale: </td>
				<td style='background-color:#F4F4F4;font-size:10px;font-family:verdana;'><b>{UOCode}</b></td>
				</tr>
                <tr>
				<td style='background-color:#dce5ea;font-size:10px;font-family:verdana;'> UO Code attuale: </td>
				<td style='background-color:#F4F4F4;font-size:10px;font-family:verdana;'><b>{UOCodeNext}</b></td>
				</tr>
			
				<tr>
				<td style='background-color:#dce5ea;font-size:10px;font-family:verdana;'> Note: </td>
				<td style='background-color:#F4F4F4;font-size:10px;font-family:verdana;'><b>{note}</b></td>
				</tr>
				<tr>
				<td style='background-color:#dce5ea;font-size:10px;font-family:verdana;'> Motivazione: </td>
				<td style='background-color:#F4F4F4;font-size:10px;font-family:verdana;'><b>{motivazione}</b></td>
				</tr>				

				<tr><td style='font-size:10px;font-family:verdana;' colspan=2>{link}</td></tr>
				
				<tr><td style='font-size:10px;font-family:verdana;' colspan=2>L'occasione &egrave; gradita per porgere distinti saluti.<br><br></td></tr>

				
				<tr><td style='font-size:10px;font-family:verdana;' colspan=2><i>Per favore, non rispondere a questa mail.</i></td></tr>
				
				</table>
		</td></tr></table>

	</tbody></td></tr></table>
	</html>",
                Html = true,
                IsActive = true,
            };
            context.EmailTesti.Add(emailcambiostato);
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
