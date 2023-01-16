
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

namespace RCDWeb.Areas.RCD.Models
{
    public class Comune
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Matricola Addetto")]
        public string Matricola { get; set; }
        [ScaffoldColumn(false)]
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "UOCode in gestione")]
        public string UOCodeNext { get; set; }
        [ScaffoldColumn(false)]
        [Display(Name = "Data ultima modifica")]
        [HiddenInput(DisplayValue = true)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? UltimaModifica { get; set; } = DateTime.Now;

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
        public bool Obbligatorio { get; set; }
        public Stati StatoSuccessivoObbligatorio { get; set; }
        [Display(Name = "Messaggio di Errore")]
        public string ErrorMessage { get; set; } = "Attenzione manca l'allegato ";
        public bool IsActive { get; set; } = true;
    }
    public class Allegati : Comune
    {

        public TipoAllegato tipoAllegato { get; set; }

        [ScaffoldColumn(false)]
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
        public int? NumCartone { get; set; }
    }

    public class Richieste : Comune
    {
        public Richieste() 
        {
            Stati = new HashSet<LogStati>();
            Allegati = new HashSet<Allegati>();
            DocumentiRichiesti = new HashSet<DocumentiRichiesti>();
        }


        [Display(Name = "Filiale")]
        public string Filiale { get; set; }
        [Display(Name = "Luogo")]
        public string Luogo { get; set; }

        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Istituto")]
        public int Istituto { get; set; }
        [Display(Name = "UOCode")]
        public string UOCode { get; set; }

        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Codice NDG")]
        public long NDG { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Persona Fisica")]
        public bool isPerson { get; set; }
        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Natura Giuridica")]
        public string NaturaGiuridica { get; set; }
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
        [Display(Name = "Richiedente")]
        public string Richiedente { get; set; }


        [Display(Name = "Filiale di consegna")]
        public string FilialeConsegna { get; set; }
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
        [Display(Name = "Classificazione, cessione rilevata")]
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
        [Display(Name = "Data avviso")]
        public DateTime? DataAvviso { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data II avviso")]
        public DateTime? DataIIAvviso { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data consegna")]
        public DateTime? DataConsegna { get; set; }

        [HiddenInput(DisplayValue = true)]
        [DataType(DataType.Date)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data chiusura")]
        public DateTime? DataChiusa { get; set; }

        [Display(Name = "Data Stop Banca")]
        public DateTime? DataStopBanca { get; set; }
        //[NotMapped]
        [Display(Name = "Giorni Attesa Banca")]
        public int? GiorniAttesaBanca
        {
            get {
                if (DataRichiesta != null)
                {
                    DateTime datafine = DateTime.Now;
                    if (DataStopBanca != null) datafine = (DateTime)DataStopBanca;
                    return (datafine - (DateTime)DataRichiesta).Days;
                }
                return null;
            }
            set { }
        }
        //[NotMapped]
        [Display(Name = "Giorni Attesa Cliente")]
        public int? GiorniAttesaCliente
        {
            get
            {
                if (DataStopBanca != null)
                {
                    DateTime datafine = DateTime.Now;
                    if (DataChiusa != null) datafine = (DateTime)DataChiusa;
                    return (datafine - (DateTime)DataRichiesta).Days;
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
        [Display(Name = "Canale consegna richiesta")]
        public virtual Canale CanaleConsegna { get; set; }
        [Display(Name = "Modalità di pagamento")]
        public virtual TipiPagamento ModalitaPagamento { get; set; }

        public virtual ICollection<DocumentiRichiesti> DocumentiRichiesti { get; set; }
        public virtual ICollection<Allegati> Allegati { get; set; }
        public virtual ICollection<LogStati> Stati { get; set; }
    }

    public class LogStati : Comune
    {

        public Stati Stato { get; set; }
        [Display(Name = "Motivazione")]
        public string Motivazione { get; set; }
        [Display(Name = "Note")]
        public string Note { get; set; }

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
        [Display(Name = "Stato di chiusura richiesta")]
        public bool Chiuso { get; set; }
        public bool IsActive { get; set; } = true;
    }


    public class Avvisi
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Stato Inizo")]
        public Stati StatoInzio { get; set; }
        [Display(Name = "Stato Fine")]
        public Stati StatoFine { get; set; }
        [Display(Name = "Canale Comunicazione")]
        public Canale CanaleComunicazione { get; set; }
        [Display(Name = "Messaggio")]
        public int Giorni { get; set; }
        [Display(Name = "Messaggio")]
        public string Messaggio { get; set; }
        public bool IsActive { get; set; } = true;
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
        [Display(Name = "Notifica")]
        public bool Notifica { get; set; }
        [Display(Name = "Notifica Responsabile")]
        public bool NotificaResp { get; set; }
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
