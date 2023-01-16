using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RCDWeb.Areas.RCD.Models
{
    //[NotMapped]
    public class GestioneViewModel : Richieste
    {
        public Guid Id_Documento { get; set; }
        [Display(Name = "Rapporto")]
        public string Rapporto { get; set; }
        //[Required(ErrorMessage = "Richiesto")]
        [Display(Name = "Tipo")]
        public TipiDocumenti Documento { get; set; }
        //[Required(ErrorMessage = "Richiesto")]
        [Display(Name = "Descrizione")]
        public string DescrizioneDocumento { get; set; }
        //[DataType(DataType.Currency)]
        [Display(Name = "Costo")]
        public decimal? Costo { get; set; }
        [Display(Name = "Copie")]
        public int NumCopie { get; set; }
        [Display(Name = "In Originale")]
        public bool DocOrig { get; set; }
        [Display(Name = "Numero cartone")]
        public string NumCartone { get; set; }


        [Display(Name = "Tipo")]
        public TipoAllegato tipoAllegato { get; set; }
        [Display(Name = "Descrizione")]
        public string DescrizioneFile { get; set; }


        [HiddenInput(DisplayValue = true)]
        [Display(Name = "Stato Successivo")]
        public string StatoSuccessivo { get; set; }

    }
}