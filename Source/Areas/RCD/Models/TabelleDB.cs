
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

using System.Globalization;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web;
using RCDWeb.Areas.RCD.Uility;
using Elmah;
using DAL.AreaCommonWeb;
using AreaCommonWeb.DAL;

namespace RCDWeb.Areas.RCD.Models
{
    public class ParametriApp
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Val { get; set; }
        public string Descrizione { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Comune
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Matricola Addetto ultima modifica")]
        public string Matricola { get; set; }
        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "UOCode in gestione")]
        public string UOCodeNext { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "Ultima Modifica")]
        [HiddenInput(DisplayValue = true)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? UltimaModifica { get; set; } = DateTime.Now;
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Modificato da:")]
        public string Addetto
        {
            get {
                var addetto = Matricola;
                try
                {
                    if (!String.IsNullOrWhiteSpace(Matricola))
                    {
                        addetto = new ResourcesRepository().GetNomeUtente(Matricola);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
                return addetto;
            }
            set { }
        }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Gestito da:")]
        public string Struttura
        {
            get
            {
                var uocode = UOCodeNext;
                try
                {
                    if (!String.IsNullOrWhiteSpace(uocode))
                    {
                        var infoUocode = new ResourcesRepository().GetDimUoByUocode(uocode).FirstOrDefault();
                        if (infoUocode != null)
                        {
                            uocode = infoUocode.DescrizioneUObyIstituto;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
                return uocode;
            }
            set { }
        }


    }

    public class Uffici
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        [Display(Name = "Istituto")]
        public int Istituto { get; set; }
        public string UOCODE { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class TipoAllegato
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome allegato")]
        public string Nome { get; set; }
        [Display(Name = "Descrizione allegato")]
        public string Descrizione { get; set; }

        public bool IsActive { get; set; } = true;


    }
    public class CondizioniAllegati
    {
        [Key]
        public int Id { get; set; }
        public virtual TipoAllegato TipoAllegato { get; set; }
        [Display(Name = "Messaggio di Errore")]
        public string ErrorMessage { get; set; } = "Attenzione manca l'allegato ";

        public bool IsActive { get; set; } = true;

        public bool Obbligatorio { get; set; } = true;
        public virtual Stati StatoIn { get; set; }
        public virtual Stati StatoOut { get; set; }
        [Display(Name = "Canale ricezione richiesta")]
        public virtual Canale CanaleRicezione { get; set; }
        [Display(Name = "Canale consegna richiesta")]
        public virtual Canale CanaleConsegna { get; set; }

    }

    public class Allegati : Comune
    {

        public TipoAllegato tipoAllegato { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Nome File")]
        public string FileName { get; set; }

        [Display(Name = "Descrizione")]
        public string DescrizioneFile { get; set; }

        [ScaffoldColumn(false)]
        public byte[] FileContent { get; set; }
        [HiddenInput(DisplayValue = false)]
        [ScaffoldColumn(false)]
        public string ContentType { get; set; }

        [HiddenInput(DisplayValue = false)]
        [ScaffoldColumn(false)]
        public long Lenght { get; set; }

        public string FormattaFile
        {
            get
            {                
                string[] testo = this.FileName.Split('\\');
                return testo[testo.Length-1];
                
            }
        }
    }

    public class TipiDocumenti
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome documento")]
        public string Nome { get; set; }
        [Display(Name = "Descrizione Documento richiesto")]
        public string Descrizione { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class TipiRichiedente
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class DocumentiRichiesti : Comune
    {

        [Display(Name = "Rapporto")]
        public string Rapporto { get; set; }
        //[Required(ErrorMessage = "Richiesto")]
        [Display(Name = "Tipo Documento")]
        public TipiDocumenti Documento { get; set; }
        //[Required(ErrorMessage = "Richiesto")]
        [Display(Name = "Descrizione Documento")]
        public string DescrizioneDocumento { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Costo")]
        public decimal? Costo { get; set; }
        [Display(Name = "Copie")]
        public int NumCopie { get; set; }
        [Display(Name = "In Originale")]
        public bool DocOrig { get; set; }
        [Display(Name = "Numero cartone")]
        public string NumCartone { get; set; }
    }
    public class Cointestatari
        {
        [Key]
        public Guid Id { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "NDG")]
        public long NDG { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Persona Fisica")]
        public bool isPerson { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Natura Giuridica")]
        public string NaturaGiuridica { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Intestazione NDG")]
        public string IntestazioneNDG { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Codice Fiscale")]
        public string CodiceFiscale { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "P.Iva")]
        public string PIva { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Luogo di nascita")]
        public string LuogoNascita { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Sede Legale")]
        public string SedeLegale { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Data di nascita")]
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataNascita { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Indirizzo")]
        public string Indirizzo { get; set; }

        [Display(Name = "Iscrizione C.C.I.A.A.")]
        public string IscrizioneCCIAA { get; set; }
        [Display(Name = "Albo Professionale")]
        public string AlboProfessionale { get; set; }

    }
    public class Richieste : Comune
    {
        public Richieste() 
        {
            Stati = new HashSet<LogStati>();
            Allegati = new HashSet<Allegati>();
            DocumentiRichiesti = new HashSet<DocumentiRichiesti>();
            Cointestatari = new HashSet<Cointestatari>();
        }

        [Display(Name = "N.")]
        public long Protocollo { get; set; }

        [Display(Name = "Filiale di riferimento")]
        public string Filiale { get; set; }
        [Display(Name = "Luogo")]
        public string Luogo { get; set; }

        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Istituto")]
        public int Istituto { get; set; }
        [Display(Name = "UOCode")]
        public string UOCode { get; set; }
        [Display(Name = "Inserito da:")]
        public string DescrizioneUOCode
        {
            get
            {
                var uocode = UOCode;
                try
                {
                    if (!String.IsNullOrWhiteSpace(uocode))
                    {
                        var infoUocode = new ResourcesRepository().GetDimUoByUocode(uocode).FirstOrDefault();
                        if (infoUocode != null)
                        {
                            uocode = infoUocode.DescrizioneUObyIstituto;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
                return uocode;
            }
            set { }
        }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "NDG")]
        public long NDG { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Persona Fisica")]
        public bool isPerson { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Natura Giuridica")]
        public string NaturaGiuridica { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Intestazione NDG")]
        public string IntestazioneNDG { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Codice Fiscale")]
        public string CodiceFiscale { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "P.Iva")]
        public string PIva { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Luogo di nascita")]
        public string LuogoNascita { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Sede Legale")]
        public string SedeLegale { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Data di nascita")]
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataNascita { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Indirizzo")]
        public string Indirizzo { get; set; }

        [Display(Name = "Iscrizione C.C.I.A.A.")]
        public string IscrizioneCCIAA { get; set; }
        [Display(Name = "Albo Professionale")]
        public string AlboProfessionale { get; set; }

        [Display(Name = "Cognome Nome/Ragione Sociale")]
        public string Richiedente { get; set; }
        [Display(Name = "Indirizzo Completo")]
        public string IndirizzoRichiedente { get; set; }
        [Display(Name = "Codice Fiscale/P.Iva")]
        public string CodiceFiscaleRichiedente { get; set; }
        [Display(Name = "Luogo di nascita/SedeLegale-N.Iscriz. C.C.I.A.A./AlboProfessionale")]
        public string LuogoNascitaRichiedente { get; set; }


        [Display(Name = "Filiale di consegna")]
        public string FilialeUOcode { get; set; }
        //[Display(Name = "Filiale di consegna")]
        //public string DescrizioneFilialeUOcode
        //{
        //    get
        //    {
        //        var FilialeUOcode = FilialeUOcode;
        //        try
        //        {
        //            if (!String.IsNullOrWhiteSpace(FilialeUOcode))
        //            {
        //                IQueryable<DimUO> filiali = new ResourcesRepository().GetDimUoByUocode(FilialeUOcode);
        //                if (filiali != null)
        //                {
        //                    FilialeUOcode = filiali.FirstOrDefault().DescrizioneUObyIstituto;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine(ex.Message);
        //            ErrorSignal.FromCurrentContext().Raise(ex);
        //        }
        //        return FilialeUOcode;
        //    }
        //    set { }
        //}

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Indirizzo Email")]
        public string email { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Indirizzo Pec")]
        public string pec { get; set; }



        [Display(Name = "Numero conto su cui effettuare addebito")]
        public string NumeroConto { get; set; }

        [Display(Name = "Tipologia Richiedente")]
        public TipiRichiedente TipoRichiedente { get; set; }
        [Display(Name = "Cessione Rilevata")]
        public bool? Cessione { get; set; }
        [Display(Name = "Legittimato")]
        public bool? Legittimato { get; set; }

        [Display(Name = "Motivazione")]
        public string Motivazione { get; set; }

        [Display(Name = "UOCode Inoltro")]
        public string UOCode_Inoltro { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        //[Required(ErrorMessage = "Richiesto")]
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data richiesta")]
        public DateTime? DataRichiesta { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data inserimento")]
        public DateTime? DataInserimento { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data avviso giacenza Doc.")]
        public DateTime? DataAvviso { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data II avviso giacenza Doc.")]
        public DateTime? DataIIAvviso { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data consegna Doc.")]
        public DateTime? DataConsegna { get; set; }

        [HiddenInput(DisplayValue = true)]
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data chiusura")]
        public DateTime? DataChiusa { get; set; }

        [Display(Name = "Data Stop Banca")]
        public DateTime? DataStopBanca { get; set; }


        [Display(Name = "Avvisi inviati")]
        public string Avvisi { get; set; }

        //[NotMapped]
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "GG Banca")]
        public int? GiorniAttesaBanca
        {
            get {
                if (DataRichiesta != null)
                {
                    DateTime datafine = DateTime.Now;
                    if (DataStopBanca != null) datafine = (DateTime)DataStopBanca;
                    return (datafine.Date - ((DateTime)DataRichiesta).Date).Days;
                }
                return null;
            }
            set { }
        }
        //[NotMapped]
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "GG Cliente")]
        public int? GiorniAttesaCliente
        {
            get
            {
                if (DataStopBanca != null)
                {
                    DateTime datafine = DateTime.Now;
                    if (DataChiusa != null) datafine = (DateTime)DataChiusa;
                    return (datafine.Date - ((DateTime)DataStopBanca).Date).Days;
                }
                return null;
            }
            set { }
        }

        [Display(Name = "Spese addebitabili")]
        public bool SpesaAddebitabile { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataPagamento { get; set; }

        public decimal? ImportoTotale { get; set; }
        //[NotMapped]
        [HiddenInput(DisplayValue = true)]
        [DataType(DataType.Currency)]
        [Display(Name = "Totale")]
        public decimal? TotaleCosto
        {
            get
            {
                decimal _Totale = 0;
                foreach (DocumentiRichiesti item in DocumentiRichiesti)
                {
                    if ((item != null)&&(item.Costo!=null))
                        _Totale = _Totale + (decimal)item.Costo*item.NumCopie;

                }
                ImportoTotale = _Totale;
                return _Totale;
            }
            set {
                ImportoTotale = TotaleCosto;
            }

        }


        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Stato attuale")]
        public virtual Stati Stato { get; set; }
        [Display(Name = "Canale ricezione richiesta")]
        public virtual Canale CanaleRicezione { get; set; }
        [Display(Name = "Canale consegna documentazione")]
        public virtual Canale CanaleConsegna { get; set; }
        [Display(Name = "Modalità di pagamento")]
        public virtual TipiPagamento ModalitaPagamento { get; set; }

        public virtual ICollection<DocumentiRichiesti> DocumentiRichiesti { get; set; }
        [Display(Name = "Bypass obbligatorietà allegati")]
        public bool bypassallegati { get; set; }
        [Display(Name = "Motivazione Bypass")]
        public string MotivazioneBypass { get; set; }
        [Display(Name = "File")]
        public virtual ICollection<Allegati> Allegati { get; set; }
        public virtual ICollection<LogStati> Stati { get; set; }
        public virtual ICollection<Cointestatari> Cointestatari { get; set; }
    }

    public class LogStati : Comune
    {

        public Stati Stato { get; set; }
        [Display(Name = "Motivazione")]
        public string Motivazione { get; set; }
        [Display(Name = "Note")]
        public string Note { get; set; }

    }
    public class LogEmail: EmailTipo
    {
        [Key]
        public Guid Id { get; set; }
        public Richieste Richiesta { get; set; }
        public DateTime Data { get; set; }

    }

    //public class Autorizzazione
    //{
    //    [Key]
    //    public long Id { get; set; }
    //    [Display(Name = "Ucode")]
    //    public string Ucode { get; set; }
    //    [Display(Name = "Funzione")]
    //    public string Funz { get; set; }
    //    [Display(Name = "ReadOnly")]
    //    public bool ReadOnly { get; set; }
    //    [Display(Name = "Descrizione")]
    //    public string Descrizione { get; set; }
    //}
    public class Stati 
    {
        public Stati()
        {
        //    WhiteList = new HashSet<Autorizzazione>();
        }
        //public ICollection<Autorizzazione> WhiteList { get; set; }

        [Key]
        public int Id { get; set; }
        
        [Display(Name = "Nome breve stato")]
        public string Stato { get; set; }

        [Display(Name = "Descrizione stato")]
        public string Descrizione { get; set; }
        
        [Display(Name = "Stato Data Banca")]
        public bool DataStopBanca { get; set; } = false;

        [Display(Name = "Chiedi Motivazione")]
        public bool ChiediMotivazione { get; set; } = false;

        [Display(Name = "Stato di chiusura richiesta")]
        public bool Chiuso { get; set; }
        
        public bool IsActive { get; set; } = true;

        public bool ConteggioBanca { get; set; }
        public bool ConteggioCliente { get; set; }

        [Display(Name = "Notifica")]
        public bool NotificaUOCodeNext { get; set; }
        [Display(Name = "Notifica Creatore")]
        public bool NotificaUOCode { get; set; }
        [Display(Name = "Notifica Responsabile")]
        public bool NotificaResp { get; set; }

        public EmailTesti EmailTesti { get; set; }
    }


    public class Avvisi
    {
        [Key]
        public int Id { get; set; }
        //[Display(Name = "Stato Inizo")]
        //public Stati StatoInzio { get; set; }
        //[Display(Name = "Stato Fine")]
        //public Stati StatoFine { get; set; }
        [Display(Name = "Canale Comunicazione")]
        public Canale CanaleComunicazione { get; set; }
        [Display(Name = "Messaggio")]
        public int Giorni { get; set; }
        [Display(Name = "Messaggio")]
        public string Messaggio { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsBanca { get; set; }

        [Display(Name = "Notifica Responsabile")]
        public bool NotificaResp { get; set; } = true;

        public EmailTesti EmailTesti { get; set; }
    }


    public class Canale
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome canale")]
        public string Nome { get; set; }
        [Display(Name = "Descrizione canale")]
        public string Descrizione { get; set; }
        [Display(Name = "Abilitato alla ricezione")]
        public bool Ricezione { get; set; }
        [Display(Name = "Abilitato alla comunicazione")]
        public bool Comunicazione { get; set; }
        [Display(Name = "Abilitato alla consegna")]
        public bool Consegna { get; set; }
        public bool IsActive { get; set; } = true;
    }


    public class AbilitaElementiStato
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Attributo")]
        public string Attributo { get; set; }
        [Display(Name = "NomeElemento")]
        public string NomeElemento { get; set; }
        [Display(Name = "NomeProprietà")]
        public string NomeProprieta { get; set; }
        [Display(Name = "ValoreProprietà")]
        public string ValoreProprieta { get; set; }
        public Stati Stato { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class TipiPagamento
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome addebito")]
        public string Nome { get; set; }
        [Display(Name = "Descrizione addebito")]
        public string Descrizione { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ParametriRichiestaConsegna
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Richiesta")]
        public Canale Richiesta { get; set; }
        [Display(Name = "Consegna")]
        public Canale Consegna { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Elementi
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "ID Elemento")]
        public string IdElemento { get; set; }
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Display(Name = "Tipo")]
        public string Tipo { get; set; }
        [Display(Name = "PlaceHolder")]
        public string PlaceHolder { get; set; }

    }

    public class Condizioni
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Elemento")]
        public Elementi Elemento { get; set; }
        [Display(Name = "Messaggio di Errore")]
        public string ErrorMessage { get; set; }
        [Display(Name = "Min")]
        public int? Min { get; set; }
        [Display(Name = "Valore")]
        public int? Valore { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class EmailTesti
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Display(Name = "Oggetto")]
        public string Oggetto { get; set; }
        [Display(Name = "Corpo email")]
        public string Corpo { get; set; }
        [Display(Name = "Corpo in Html")]
        public bool Html { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class CondizioneStato
    {
        public CondizioneStato()
        {
            Richiesti = new HashSet<Condizioni>();
           
        }

        [Key]
        public int Id { get; set; }
        [Display(Name = "operationType")]
        public string operationType { get; set; } = "Conferma";
        [Display(Name = "Stato IN")]
        public Stati StatoIn { get; set; }
        [Display(Name = "Stato OUT")]
        public Stati StatoOut { get; set; }


        [Display(Name = "Solo UO Competente")]
        public bool? OnlyUOCompetente { get; set; }
        [Display(Name = "Discrimino Stesso UO")]
        public bool? IsSameUO { get; set; }
        [Display(Name = "Richiesta")]
        public Canale Richiesta { get; set; }
        [Display(Name = "Consegna")]
        public Canale Consegna { get; set; }

        [Display(Name = "Tipologia Richiedente")]
        public TipiRichiedente TipoRichiedente { get; set; }

        public ICollection<Condizioni> Richiesti { get; set; }

        public bool IsActive { get; set; } = true;
    }
    public class ErroriGenerici
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Display(Name = "Messaggio di Errore")]
        public string ErrorMessage { get; set; } = "Richiesto";
        public bool IsActive { get; set; } = true;
    }
}
