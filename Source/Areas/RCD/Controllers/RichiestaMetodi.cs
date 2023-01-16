using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RCDWeb.Areas.RCD.DAL;
using RCDWeb.Areas.RCD.Models;
using RCDWeb.Areas.RCD.Uility;
using AreaCommonWeb.Business;
using DAL.AreaCommonWeb;
using AreaCommonWeb.DAL;
using AreaCommonWeb.ModelView.Security;
using RCDWeb.Areas.RCD.Repository;
using AreaCommonWeb.ModelView;
using WebServiceModel.WSAnagrafe;
using WebServiceModel.DAL;
using AreaCommonWeb.Models;
using System.ComponentModel.DataAnnotations;
using AreaCommonWeb.DAL.Profiles;
using Elmah;

namespace RCDWeb.Areas.RCD.Controllers
{
    [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
    public partial class RichiestaController : Controller
    {
        private ResourcesRepository resourcesRepository = new ResourcesRepository();
        private RCDRepository repository = new RCDRepository();
        private CommonBusiness commonBusiness = new CommonBusiness();
        private ProfilesRepository profilesRepository = new ProfilesRepository();


        private int MioIstituto;
        private string codRuolo;
        private string MioUocode;
        private string MiaMatricola;
        //private string MiaFiliale;
        //private string MioComune;
        private bool isFiliale;


        public void PopolaViewGestione(Richieste richieste)
        {
            ViewBag.giorniscadenzabanca = repository.giorniscadenzabanca;
            ViewBag.giorniscadenzacliente = repository.giorniscadenzacliente;


            var ric_isFiliale = true;
            try
            {
                var info_uocode = resourcesRepository.GetDimUoByUocode(richieste.UOCode).FirstOrDefault();
                if (info_uocode != null)
                {
                    var myFlaf = info_uocode.FlagUfficioFiliale;
                    ric_isFiliale = ((myFlaf == "F") || (myFlaf == "O"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            var lista = new List<SelectListItem>();
            foreach (var item in repository.GetCanaliRicezione())
            {
                if (!ric_isFiliale && item.Nome.EqualsNormalized("Filiale"))
                {
                    // non sono una filiale quindi ricezione in filiale è impossibile
                }
                else
                {
                    var pippo = new SelectListItem();
                    pippo.Text = item.Descrizione;
                    pippo.Value = item.Id.ToString();
                    if ((richieste != null) && (richieste.CanaleRicezione != null))
                        pippo.Selected = richieste.CanaleRicezione.Id == item.Id;
                    lista.Add(pippo);
                }
            }
            ViewBag.DropDownRicezione = lista;

            var lista2 = new List<SelectListItem>();
            foreach (var item in repository.GetCanaliConsegna())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Descrizione;
                pippo.Value = item.Id.ToString();
                if ((richieste != null) && (richieste.CanaleConsegna != null))
                    pippo.Selected = richieste.CanaleConsegna.Id == item.Id;
                lista2.Add(pippo);
            }
            ViewBag.DropDownConsegna = lista2;

            var lista3 = new List<SelectListItem>();
            foreach (var item in repository.GetTipiPagamento())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Descrizione;
                pippo.Value = item.Id.ToString();
                if ((richieste != null) && (richieste.ModalitaPagamento != null))
                    pippo.Selected = richieste.ModalitaPagamento.Id == item.Id;
                lista3.Add(pippo);
            }
            ViewBag.DropDownPagamento = lista3;

            var statoattuale = richieste.Stato;
            var listaElementi = repository.GetAbilitaElementiStato(statoattuale.Id).ToList();
            var placehoders = repository.GetPlaceHolder().ToList();
            foreach (var item in placehoders)
            {
                listaElementi.Add(new AbilitaElementiStato
                {
                    Attributo = "id",
                    NomeElemento = item.IdElemento,
                    NomeProprieta = "placeholder",
                    ValoreProprieta = item.PlaceHolder
                }); ;
            }
            ViewBag.ElementiStato = listaElementi;

            var ParametriRichiestaConsegna = repository.GetParametriRichiestaConsegna().ToList();
            ViewBag.ParametriRichiestaConsegna = ParametriRichiestaConsegna;
            ViewBag.CanaleFilialeID = repository.GetCanale("Filiale").Id;
            ViewBag.CanalePecID = repository.GetCanale("Pec").Id;
            ViewBag.CanaleEmailID = repository.GetCanale("Email").Id;
            ViewBag.CanaleRaccomandataID = repository.GetCanale("Raccomandata").Id;

            //ViewBag.Annulla = repository.GetStato("Annullata").Stato;
            //ViewBag.Respingixmod = repository.GetStato("Respinta per modifica").Stato;
            //ViewBag.Archivia = repository.GetStato("Archiviata").Stato;
            //ViewBag.Respinta = repository.GetStato("Respinta").Stato;

            //var StatiMotivazione = new string[] {
            //    repository.GetStato("Annullata").Stato,
            //    repository.GetStato("Respinta per modifica").Stato,
            //    repository.GetStato("Archiviata").Stato,
            //    repository.GetStato("Respinta").Stato,
            //};
            var StatiMotivazione = repository.GetStati().Where(a => a.ChiediMotivazione).Select(b => b.Stato).ToArray();
            ViewBag.StatiMotivazione = StatiMotivazione;

            var lista5 = new List<SelectListItem>();
            foreach (var item in repository.GetTipiDocumenti())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Descrizione;
                pippo.Value = (item.Id).ToString();
                lista5.Add(pippo);
            }
            ViewBag.DropDownTipoDocumenti = lista5;

            var lista6 = new List<SelectListItem>();
            foreach (var item in repository.GetTipoAllegato())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Nome;
                pippo.Value = (item.Id).ToString();
                lista6.Add(pippo);
            }
            ViewBag.DropDownTipoAllegato = lista6;

            var lista7 = new List<SelectListItem>();
            foreach (var item in repository.GetTipiRichiedente())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Descrizione;
                pippo.Value = (item.Id).ToString();
                if ((richieste != null) && (richieste.TipoRichiedente != null))
                    pippo.Selected = richieste.TipoRichiedente.Id == item.Id;
                lista7.Add(pippo);
            }
            ViewBag.DropDownTipoRichiedente = lista7;
            ViewBag.CuratoreId = repository.GetTipoRichiedente("Curatore").Id;
            ViewBag.TitolareId = repository.GetTipoRichiedente("Titolare").Id;

            bool? legittimato = null;
            if ((richieste != null) && (richieste.Legittimato != null))
                legittimato = richieste.Legittimato;
            var lista8 = new List<SelectListItem> {
                new SelectListItem { Value= "" , Text="-- Selezionare ---", Selected = legittimato==null },
                new SelectListItem { Value= ((bool)true).ToString() , Text="Si", Selected = legittimato==true },
                new SelectListItem { Value= ((bool)false).ToString() , Text="No", Selected = legittimato==false },
            };
            ViewBag.DropDownLegit = lista8;

            bool? cessione = null;
            if ((richieste != null) && (richieste.Cessione != null))
                cessione = richieste.Cessione;
            var lista9 = new List<SelectListItem> {
                new SelectListItem { Value= "" , Text="-- Selezionare ---", Selected = cessione==null },
                new SelectListItem { Value= ((bool)true).ToString() , Text="Si", Selected = cessione==true },
                new SelectListItem { Value= ((bool)false).ToString() , Text="No", Selected = cessione==false },
            };
            ViewBag.DropDownCess = lista9;


            //ViewData["FilialeCuocode"] = "";
            ////ViewData["FilialeC"] = "";
            //ViewData["FilialeDesc"] = "";
            //if (!String.IsNullOrWhiteSpace(richieste.Filiale))
            //{
            ViewData["FilialeCuocode"] = richieste.FilialeUOcode;
            //ViewData["FilialeC"] = MioUocode + " - " + MiaFiliale;
            ViewData["FilialeDesc"] = richieste.Filiale;
            //}
            //if (!String.IsNullOrWhiteSpace(richieste.Filiale))
            //{
            //    try
            //    {
            //        IQueryable<DimUO> filiali = resourcesRepository.GetDimUoByUocode(richieste.FilialeConsegna);
            //        var filiale = filiali.FirstOrDefault().DescrizioneUObyIstituto;
            //        ViewData["FilialeCuocode"] = richieste.FilialeConsegna;
            //        ViewData["FilialeC"] = richieste.FilialeConsegna + " - " + filiale;
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine(ex.Message);
            //        ErrorSignal.FromCurrentContext().Raise(ex);
            //    }
            //}

            var lista4 = new List<SelectListItem>();
            var uffici = repository.GetUffici(MioIstituto, MioUocode);
            bool uocodeIsUfficio = false;
            foreach (var item in uffici)
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Nome;
                pippo.Value = item.UOCODE;
                if ((richieste != null) && (!String.IsNullOrWhiteSpace(richieste.UOCode_Inoltro)))
                {
                    if (richieste.UOCode_Inoltro == item.UOCODE)
                    {
                        pippo.Selected = true;
                        uocodeIsUfficio = true;
                    }
                }
                lista4.Add(pippo);
            }
            ViewBag.DropDownUffici = lista4;
            if (uocodeIsUfficio)
            {
                ViewData["UOCode_InoltroC"] = "";
            }
            else
            {
                ViewData["UOCode_InoltroC"] = richieste.UOCode_Inoltro;
                if (!String.IsNullOrWhiteSpace(richieste.UOCode_Inoltro))
                {
                    try
                    {
                        string filiale = "";
                        var result = resourcesRepository.GetDimUoByUocode(richieste.UOCode_Inoltro).FirstOrDefault();
                        if (result != null)
                        {
                            filiale = result.DescrizioneUObyIstituto;
                        }

                        ViewData["UOCode_InoltroC"] = richieste.UOCode_Inoltro + " - " + filiale;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }

            }

            //fix
            //se richiesta motivazione e' compilata, dev'essere azzerata e visualizzata accanto con una label
            ViewData["Motivazione"] = "";
            if (!String.IsNullOrWhiteSpace(richieste.Motivazione))
            {
                ViewData["Motivazione"] = richieste.Motivazione;
                //richieste.Motivazione = "";
            }
            ViewBag.MatricolaNome = "Operatore";
            if (!String.IsNullOrWhiteSpace(richieste.Matricola))
            {
                try
                {
                    ViewBag.MatricolaNome = resourcesRepository.GetNomeUtente(richieste.Matricola);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
        }



        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult GetIndexTable()
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        FiltriRichiesteModel filtri = new FiltriRichiesteModel();
        //        ViewBag.MioUocode = MioUocode.UOCODE;
        //        ViewBag.MiaMatricola = MiaMatricola;
        //        filtri.filtroUocode = MioUocode.UOCODE;
        //        return Json(new { success = true, data = repository.GetRichieste(filtri).ToList() }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        // Altrimenti visualizziamo nuovamente tutto il form
        //        return View();
        //    }
        //}

        private bool UfficioAutorizzato(Richieste richieste, string MioUocode)
        {
            if (MioUocode == richieste.UOCodeNext) return true;
            return false;
        }

        //private bool AddettoAutorizzato(Richieste richieste, string MiaMatricola)
        //{
        //    if (MiaMatricola == richieste.Matricola) return true;
        //    return false;
        //}

        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult Gestione(Guid? id, int? istituto, long? ndg, string codfiscale)
        {
            Richieste richieste;
            if (id == null)
            {

                if (istituto == null || ndg == null) return RedirectToAction("GestioneRichieste");

                richieste = new Richieste();
                richieste.DataRichiesta = DateTime.Now;
                richieste.Stato = repository.GetStato("Inizio");

                richieste.UOCode = MioUocode;
                richieste.UOCodeNext = MioUocode;
                richieste.Matricola = MiaMatricola;

                GetCliente(richieste, (int)istituto, (long)ndg, codfiscale);

            }
            else
            {
                richieste = repository.GetRichiesta((Guid)id);
                //if (!UfficioAutorizzato(richieste, MioUocode)) return RedirectToAction("GestioneRichieste");
                if (richieste.Stato.Id == repository.GetStato("Assegnata").Id)
                {
                    richieste.Matricola = MiaMatricola;
                    richieste.Stato = repository.GetStato("In Lavorazione");
                    repository.Save(richieste);
                }
                if (richieste.Stato.Id == repository.GetStato("Respinta per modifica").Id)
                {
                    richieste.Matricola = MiaMatricola;
                    richieste.Stato = repository.GetStato("In Bozza");
                    repository.Save(richieste);
                }
            }


            PopolaViewGestione(richieste);

            var richiestaView = MapObj.ObjectTo<GestioneViewModel>(richieste, null);

            //pulisco il contenuto file per non appesantire pagina grafica
            if (richiestaView.Allegati.Count > 0)
                for (int i = 0; i < richiestaView.Allegati.Count; i++)
                {
                    richiestaView.Allegati.ElementAt(i).FileContent = null;
                }

            if (richiestaView.NumCopie < 1) richiestaView.NumCopie = 1;
            return View(richiestaView);
        }

        private void CopyfromView(GestioneViewModel richieste, Richieste richiestedb)
        {
            richiestedb.Cointestatari = richieste.Cointestatari.ToList();



            if (richiestedb.Stato == null)
                richiestedb.Stato = repository.GetStato(1);

            richieste.UltimaModifica = DateTime.Now;
            if (richieste.DataRichiesta == null) richieste.DataRichiesta = DateTime.Now;

            if ((richieste.CanaleConsegna != null) && (richieste.CanaleConsegna.Id > 0))
            {
                richiestedb.CanaleConsegna = repository.GetCanale(richieste.CanaleConsegna.Id);
            }
            else richiestedb.CanaleConsegna = null;
            if ((richieste.CanaleRicezione != null) && (richieste.CanaleRicezione.Id > 0))
            {
                richiestedb.CanaleRicezione = repository.GetCanale(richieste.CanaleRicezione.Id);
            }
            else richiestedb.CanaleRicezione = null;
            if ((richieste.TipoRichiedente != null) && (richieste.TipoRichiedente.Id > 0))
            {
                richiestedb.TipoRichiedente = repository.GetTipoRichiedente(richieste.TipoRichiedente.Id);
            }
            else
                richiestedb.TipoRichiedente = null;
            if ((richieste.ModalitaPagamento != null) && (richieste.ModalitaPagamento.Id > 0))
            {
                richiestedb.ModalitaPagamento = repository.GetTipoPagamento(richieste.ModalitaPagamento.Id);
            }
            else richiestedb.ModalitaPagamento = null;

            if ((richieste.CanaleConsegna != null) &&
               (richieste.CanaleConsegna.Id == repository.GetCanale("Pec").Id))
            {
                richiestedb.pec = richieste.pec;
            }
            else
            {
                richiestedb.pec = "";
            }
            if ((richieste.CanaleConsegna != null) &&
                (richieste.CanaleConsegna.Id == repository.GetCanale("Email").Id))
            {
                richiestedb.email = richieste.email;
            }
            else
            {
                richiestedb.email = "";
            }

            //if ((richieste.CanaleConsegna != null) &&
            //    (richieste.CanaleConsegna.Id == repository.GetCanale("Filiale").Id))
            //{
            //    richiestedb.FilialeConsegna = richieste.FilialeConsegna;
            //}
            //else
            //{
            //    richiestedb.FilialeConsegna = "";
            //}
            richiestedb.FilialeUOcode = richieste.FilialeUOcode;

            if (String.IsNullOrWhiteSpace(richieste.UOCode))
            {
                richieste.UOCode = MioUocode;
            }
            if (String.IsNullOrWhiteSpace(richieste.UOCodeNext))
            {
                richieste.UOCodeNext = MioUocode;
            }
            richiestedb.UOCode = richieste.UOCode;
            richiestedb.UOCodeNext = richieste.UOCodeNext;

            richiestedb.Matricola = MiaMatricola;
            richiestedb.AlboProfessionale = richieste.AlboProfessionale;
            richiestedb.Cessione = richieste.Cessione;
            richiestedb.CodiceFiscale = richieste.CodiceFiscale;
            richiestedb.DataAvviso = richieste.DataAvviso;
            richiestedb.DataConsegna = richieste.DataConsegna;
            richiestedb.DataIIAvviso = richieste.DataIIAvviso;
            richiestedb.DataNascita = richieste.DataNascita;
            richiestedb.DataPagamento = richieste.DataPagamento;
            richiestedb.DataRichiesta = richieste.DataRichiesta;

            richiestedb.Indirizzo = richieste.Indirizzo;
            richiestedb.IntestazioneNDG = richieste.IntestazioneNDG;
            richiestedb.IscrizioneCCIAA = richieste.IscrizioneCCIAA;
            richiestedb.Istituto = richieste.Istituto;
            richiestedb.Legittimato = richieste.Legittimato;
            richiestedb.Luogo = richieste.Luogo;
            richiestedb.LuogoNascita = richieste.LuogoNascita;
            richiestedb.Motivazione = richieste.Motivazione;
            richiestedb.isPerson = richieste.isPerson;
            richiestedb.NDG = richieste.NDG;
            richiestedb.Note = richieste.Note;
            richiestedb.NumeroConto = richieste.NumeroConto;

            richiestedb.PIva = richieste.PIva;
            richiestedb.Richiedente = richieste.Richiedente;
            richiestedb.IndirizzoRichiedente = richieste.IndirizzoRichiedente;
            richiestedb.LuogoNascitaRichiedente = richieste.LuogoNascitaRichiedente;
            richiestedb.CodiceFiscaleRichiedente = richieste.CodiceFiscaleRichiedente;

            richiestedb.SedeLegale = richieste.SedeLegale;
            richiestedb.SpesaAddebitabile = richieste.SpesaAddebitabile;
            richiestedb.Filiale = richieste.Filiale;

            richiestedb.UOCode_Inoltro = richieste.UOCode_Inoltro;
            richiestedb.NaturaGiuridica = richieste.NaturaGiuridica;

            richiestedb.bypassallegati = richieste.bypassallegati;
            richiestedb.MotivazioneBypass = richieste.MotivazioneBypass;

        }
        private void Errore(string id, string nome = "")
        {
            if (String.IsNullOrWhiteSpace(nome)) nome = id;
            var err = repository.GetErroreGenerico(nome);
            if (err != null)
            {
                var message = err.ErrorMessage;
                var errorenonpresente = true;
                foreach (var item in ViewData.ModelState)
                {
                    //if (item.Key == id)
                    foreach (ModelError error in item.Value.Errors)
                    {
                        if (error.ErrorMessage.EqualsNormalized(message))
                        {
                            errorenonpresente = false;
                        }
                    }
                }
                if (errorenonpresente) ModelState.AddModelError(id, message);
            }
        }

        private void ErroreSpecifico(string id, string err = "")
        {
            if (err != null)
            {
                var message = err;
                var errorenonpresente = true;
                foreach (var item in ViewData.ModelState)
                {
                    //// pezza a colori per non visualizzare errore cessione in caso di presenza errore Legittimato
                    /// programmata modifica selettore unico con casi: Non Legittimato, Cessione Rilevata e Cesssione non Rilevata
                    /////
                    if ((id.EqualsNormalized("Cessione")) && (item.Key.Equals("Legittimato")))
                    {
                        errorenonpresente = false;
                    }



                    //if (item.Key == id)
                    foreach (ModelError error in item.Value.Errors)
                    {
                        if (error.ErrorMessage.EqualsNormalized(message))
                        {
                            errorenonpresente = false;
                        }
                    }
                }
                if (errorenonpresente) ModelState.AddModelError(id, message);
            }
        }

        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        //[ValidateAntiForgeryToken]
        public ActionResult Gestione(GestioneViewModel richieste, string operationType, Guid? ModDocumento, Guid? DelDocumento, HttpPostedFileBase FileForm, Guid? DelAllegato)  //[Bind(Include = "Id,NDG,isPerson,IntestazioneNDG,Nome,IntestazioneNDG,CodiceFiscale,PIva,LuogoNascita,SedeLegale,IscrizioneCCIAA,DataNascita,AlboProfessionale,Indirizzo,Richiedente,CanaleRicezione,CanaleConsegna,email,pec,ModalitaPagamento,NumeroConto,AddebitoCassa,AddebitoIBAN,Titolare,TipoRichiedente,Cessione,Legittimato,InoltroUfficio,Motivazione,Note,DataPagamento,DataRichiesta,DataRichiesta,CanaleComunicazione,DataAvviso,DataIIAvviso,DataConsegna,DataChiusa,Filiale,Luogo,Matricola,UltimaModifica")]
        {
            if (!UfficioAutorizzato(richieste, MioUocode)) return RedirectToAction("GestioneRichieste");
            //System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(operationType));
            ModelState.Clear();
            var richiestedb = repository.GetRichiestaOrCreate(richieste.Id);
            CopyfromView(richieste, richiestedb);

            //se operatore salva e non sta aggiungendo documenti o allegato lo stato passa da inizio a inserita
            if ((richiestedb.Id != Guid.Empty) && (!operationType.EqualsNormalized("AddFile")) && (!operationType.EqualsNormalized("AddDocumento"))
                && (richiestedb.Stato.Id == repository.GetStato("Inizio").Id))
            {
                richiestedb.Stato = repository.GetStato("In Bozza");
            }
            var errorecostooblbligatorio = false;
            int canalericezione = 0;
            int canaleconsegna = 0;
            var IsSameUO = (richieste.UOCode == richieste.UOCodeNext);
            int tiporichiedente = 0;
            if ((richiestedb.TipoRichiedente != null) && (richiestedb.TipoRichiedente.Id > 0))
            {
                tiporichiedente = richiestedb.TipoRichiedente.Id;
            }
            if (richieste.CanaleRicezione != null)
            { canalericezione = richieste.CanaleRicezione.Id; }
            if (richieste.CanaleConsegna != null)
            { canaleconsegna = richieste.CanaleConsegna.Id; }
            bool? OnlyUOCompetente = null;
            if (isFiliale && (richiestedb.TipoRichiedente != null) && (richiestedb.TipoRichiedente.Nome.ContainsNormalized("curatore")))
            {
                OnlyUOCompetente = true;
            }
            if (isFiliale && (richiestedb.TipoRichiedente != null) && (richiestedb.TipoRichiedente.Nome.ContainsNormalized("titolare"))
                && richiestedb.Legittimato != null && richiestedb.Legittimato.Value
                && richiestedb.Cessione != null && richiestedb.Cessione.Value
                )
            {
                OnlyUOCompetente = true;
            }

            #region ControlloValidazione
            var stato_operationtype = "";
            CondizioneStato condizionestatotrovata = null;
            if ((!String.IsNullOrWhiteSpace(operationType)) && (operationType.ContainsNormalized("Conferma")))
            {
                stato_operationtype = operationType.Replace("Conferma ", "").Replace("Ok", "");
            }
            Stati StatoSuccessivo = null;
            var StatoSuccessivoDescrizone = "";
            if (!string.IsNullOrWhiteSpace(operationType) && (operationType.ContainsNormalized("Conferma")))
            {
                var statipossibili = repository.GetStatiPossibili(stato_operationtype, richieste.Stato.Id, canalericezione, canaleconsegna, IsSameUO, tiporichiedente, OnlyUOCompetente).ToList();
                foreach (var item in statipossibili)
                {
                    StatoSuccessivo = item.StatoOut;
                    foreach (var rich in item.Richiesti.Where(a => a.IsActive))
                    {
                        if (rich.Valore != null)
                        {
                            var val = (int)rich.Valore;
                            var propertyInfo = typeof(Richieste).GetProperty(rich.Elemento.IdElemento);
                            if (propertyInfo != null)
                            {
                                var prop = propertyInfo.PropertyType;
                                var valore = propertyInfo.GetValue(richieste);
                                if (rich.Elemento.Tipo.ContainsNormalized("DateTime"))
                                {
                                    if ((valore != null) && ((DateTime)valore).ToShortDateString().Length != val)
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }
                                else
                                if (rich.Elemento.Tipo.EqualsNormalized("Canale"))
                                {
                                    if (rich.Elemento.Nome.EqualsNormalized("CanaleRicezione"))
                                    {
                                        if ((richieste.CanaleRicezione == null) || (richieste.CanaleRicezione.Id != val))
                                        {
                                            ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                            StatoSuccessivo = null;
                                        }
                                    }
                                    else
                                    if (rich.Elemento.Nome.EqualsNormalized("CanaleConsegna"))
                                    {
                                        if ((richieste.CanaleConsegna == null) || (richieste.CanaleConsegna.Id != val))
                                        {
                                            ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                            StatoSuccessivo = null;
                                        }
                                    }
                                }
                                else
                                if (rich.Elemento.Tipo.EqualsNormalized("TipiRichiedente"))
                                {
                                    if ((valore == null) || (((TipiRichiedente)valore).Id != val))
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }
                                else
                                if (rich.Elemento.Tipo.EqualsNormalized("Boolean"))
                                {
                                    if ((valore == null) || ((bool)valore != Convert.ToBoolean(val)))
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }
                                else
                                if (rich.Elemento.Tipo.EqualsNormalized("String"))
                                {
                                    if (String.IsNullOrWhiteSpace((String)valore) == Convert.ToBoolean(val))
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }

                            }
                        }
                        if (rich.Min != null)
                        {
                            var min = (int)rich.Min;
                            if (rich.Elemento.Tipo.EqualsNormalized("Canale"))
                            {
                                if (rich.Elemento.Nome.EqualsNormalized("CanaleRicezione"))
                                {
                                    if ((richieste.CanaleRicezione == null) || (richieste.CanaleRicezione.Id < min))
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }
                                else
                                if (rich.Elemento.Nome.EqualsNormalized("CanaleConsegna"))
                                {
                                    if ((richieste.CanaleConsegna == null) || (richieste.CanaleConsegna.Id < min))
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }
                            }
                            else
                            if (rich.Elemento.Tipo.EqualsNormalized("TipiRichiedente"))
                            {
                                if (rich.Elemento.Nome == "TipoRichiedente")
                                {
                                    if ((richieste.TipoRichiedente == null) || (richieste.TipoRichiedente.Id < min))
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }
                            }
                            else
                            {
                                var propertyInfo = typeof(Richieste).GetProperty(rich.Elemento.IdElemento);
                                if (propertyInfo != null)
                                {
                                    var prop = propertyInfo.PropertyType;
                                    var tipo = prop.Name;
                                    var valore = propertyInfo.GetValue(richieste);
                                    if (valore != null)
                                    {
                                        if (prop.FullName.ContainsNormalized("DateTime"))
                                        {
                                            tipo = "DateTime";
                                        }

                                        if (tipo.EqualsNormalized("DateTime"))
                                        {
                                            if (((DateTime)valore).Year < min)
                                            {
                                                ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                                StatoSuccessivo = null;
                                            }
                                        }
                                        else
                                        if (tipo.EqualsNormalized("String"))
                                        {
                                            if (String.IsNullOrWhiteSpace((String)valore) || (((String)valore).Length < min))
                                            {
                                                ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                                StatoSuccessivo = null;
                                            }
                                        }
                                        else
                                        if (tipo.EqualsNormalized("Int64"))
                                        {
                                            if ((long)valore < min)
                                            {
                                                ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                                StatoSuccessivo = null;
                                            }
                                        }
                                        else
                                        if (tipo.EqualsNormalized("Int32"))
                                        {
                                            if ((int)valore < min)
                                            {
                                                ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                                StatoSuccessivo = null;
                                            }
                                        }
                                        else
                                        if (tipo.EqualsNormalized("Boolean"))
                                        {
                                            if (valore == null)// || ((bool)valore != Convert.ToBoolean(valore)))
                                            {
                                                ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                                StatoSuccessivo = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ErroreSpecifico(rich.Elemento.IdElemento, rich.ErrorMessage);// + " per stato " + item.StatoOut.Descrizione);
                                        StatoSuccessivo = null;
                                    }
                                }
                            }

                        }

                    }

                    //fix
                    if (!String.IsNullOrWhiteSpace(operationType) &&
                        operationType.ContainsNormalized("archivia") &&
                        richieste.DataAvviso != null &&
                        ((DateTime)richieste.DataAvviso.Value).AddDays(repository.giorniscadenzacliente) >= DateTime.Now)
                    {
                        Errore("DataAvvisoArchivia");
                        StatoSuccessivo = null;
                    }


                    if (StatoSuccessivo != null)
                    {
                        ModelState.Clear();
                        condizionestatotrovata = item;
                        StatoSuccessivoDescrizone = StatoSuccessivo.Descrizione;
                        break;
                    }
                }

                if (statipossibili.Count == 0)
                {
                    if (IsSameUO && stato_operationtype.ContainsNormalized("Respinta per Modifica"))
                    {
                        Errore("Respingimodifica");
                    }
                    else
                    if (OnlyUOCompetente != null && OnlyUOCompetente.Value && stato_operationtype.ContainsNormalized("lavorazione"))
                    {
                        Errore("Solouocompetente");
                    }
                    else
                    {
                        Errore("Nessunostatopossibile");
                    }
                }




                //////// inzio sezione controllo errori non da database solo da codice

                if ((richiestedb.CanaleRicezione != null) &&
                (richiestedb.CanaleRicezione.Id == repository.GetCanale("Filiale").Id) &&
                 richiestedb.ModalitaPagamento == null)
                {
                    Errore("ModalitaPagamento");
                }
                ////fix
                //var idallegato = repository.GetTipoAllegato().Where(s => s.IsActive && s.Nome.ContainsNormalized("Sottoscrizione")).FirstOrDefault();
                //var idcanale = repository.GetCanale("Filiale");
                //if (idallegato != null && idcanale != null &&
                //    richiestedb.CanaleRicezione != null && richiestedb.CanaleRicezione.Id == idcanale.Id && //cliente in filiale                                                     
                //    richiestedb.Allegati.Where(a => a.tipoAllegato.Id == idallegato.Id).Count() == 0)
                //{
                //    ErroreSpecifico("Allegato", idallegato.ErrorMessage);
                //}

                if ((richiestedb.CanaleConsegna != null) &&
                    (richiestedb.CanaleConsegna.Id == repository.GetCanale("Pec").Id) &&
                    !(String.IsNullOrWhiteSpace(richieste.pec)) &&
                    !(new EmailAddressAttribute().IsValid(richieste.pec)))
                {
                    Errore("pec", "pecnonvalida");
                }
                if ((richiestedb.CanaleConsegna != null) &&
                    (richiestedb.CanaleConsegna.Id == repository.GetCanale("Email").Id) &&
                    !(String.IsNullOrWhiteSpace(richieste.email)) &&
                    !(new EmailAddressAttribute().IsValid(richieste.email)))
                {
                    Errore("email", "emailnonvalida");
                }
                if ((richiestedb.CanaleConsegna != null) && (richiestedb.CanaleConsegna.Id == repository.GetCanale("Filiale").Id) &&
                    ((String.IsNullOrWhiteSpace(richiestedb.FilialeUOcode)) || (richiestedb.FilialeUOcode.Trim().Length < 5)))
                {
                    Errore("FilialeConsegna");
                }
                if ((richiestedb.CanaleConsegna != null) && (richiestedb.CanaleConsegna.Id == repository.GetCanale("Pec").Id) &&
                    ((String.IsNullOrWhiteSpace(richiestedb.pec)) || (richiestedb.pec.Trim().Length < 7)))
                {
                    Errore("pec");
                }
                if ((richiestedb.CanaleConsegna != null) && (richiestedb.CanaleConsegna.Id == repository.GetCanale("Email").Id) &&
                     ((String.IsNullOrWhiteSpace(richiestedb.email)) || (richiestedb.email.Trim().Length < 7)))
                {
                    Errore("email");
                }

                //if ((richiestedb.TipoRichiedente != null) &&
                //    (richiestedb.TipoRichiedente.Nome == "Curatore") &&
                //    string.IsNullOrEmpty(richiestedb.Richiedente))
                //{
                //    Errore("Richiedente");
                //}
                if (!string.IsNullOrEmpty(richiestedb.Richiedente)
                    &&
                     ((String.IsNullOrWhiteSpace(richiestedb.IndirizzoRichiedente)) || (richiestedb.IndirizzoRichiedente.Trim().Length < 7))
                    )
                {
                    Errore("IndirizzoRichiedente");
                }


                if ((richiestedb.ModalitaPagamento != null) && (richiestedb.ModalitaPagamento.Id == repository.GetTipoPagamento("Conto").Id) &&
                    ((String.IsNullOrWhiteSpace(richiestedb.NumeroConto)) || (richiestedb.NumeroConto.Trim().Length < 7)))
                {
                    Errore("NumeroConto");
                }
                if ((richiestedb.DocumentiRichiesti == null) || (richiestedb.DocumentiRichiesti.Count() == 0))
                {
                    Errore("Documenti");
                }

                if (richieste.SpesaAddebitabile && richiestedb.DocumentiRichiesti != null)
                {
                    foreach (var doc in richiestedb.DocumentiRichiesti)
                    {
                        if ((richiestedb.CanaleRicezione != null) &&
                            (richiestedb.CanaleRicezione.Id == repository.GetCanale("Filiale").Id) &&
                            (richiestedb.SpesaAddebitabile) &&
                            (doc.Costo == null || doc.Costo <= 0))
                        {
                            Errore("Costo");
                            errorecostooblbligatorio = true;
                        }
                    }
                }

                if (((richiestedb.DataRichiesta != null) && (((DateTime)richiestedb.DataRichiesta).Date > DateTime.Now.Date)))
                {
                    Errore("DataRichiesta");
                }
                if (((richiestedb.DataAvviso != null) && (((DateTime)richiestedb.DataAvviso).Date > DateTime.Now.Date)))
                {
                    Errore("DataAvviso");
                }
                if (((richiestedb.DataIIAvviso != null) && (((DateTime)richiestedb.DataIIAvviso).Date > DateTime.Now.Date)))
                {
                    Errore("DataIIAvviso");
                }
                if (((richiestedb.DataConsegna != null) && (((DateTime)richiestedb.DataConsegna).Date > DateTime.Now.Date)))
                {
                    Errore("DataConsegna");
                }

                if ((richiestedb.DataAvviso != null) && ((richiestedb.DataRichiesta == null) || (richiestedb.DataAvviso < richiestedb.DataRichiesta)))
                {
                    Errore("DataAvviso", "DataAvvisoRispettoDataRichiesta");
                }
                if ((richiestedb.DataIIAvviso != null) && ((richiestedb.DataAvviso == null) || (richiestedb.DataIIAvviso < richiestedb.DataAvviso)))
                {
                    Errore("DataIIAvviso", "DataIIAvvisoRispettoDataAvviso");
                }
                if (richiestedb.Stato.Id != repository.GetStato("In Lavorazione").Id)
                {
                    if ((richiestedb.DataConsegna != null) && (richiestedb.DataAvviso != null) && (richiestedb.DataConsegna < richiestedb.DataAvviso))
                    {
                        Errore("DataConsegna", "DataConsegnaRispettoDataAvviso");
                    }
                }
                else
                {
                    if ((richiestedb.DataConsegna != null) && ((richiestedb.DataRichiesta == null) || (richiestedb.DataConsegna < richiestedb.DataRichiesta)))
                    {
                        Errore("DataConsegna", "DataConsegnaRispettoDataRichiesta");
                    }
                }

            }
            #endregion

            #region Allegati e Documentazione
            if ((richiestedb.Id != Guid.Empty) && (operationType.EqualsNormalized("AddFile")))
            {
                if (richieste.tipoAllegato.Id <= 0)
                {
                    Errore("tipoAllegato");
                }

                if ((richieste.tipoAllegato.Id == repository.GetTipoAllegato().Where(a => a.Nome.EqualsNormalized("Altro")).FirstOrDefault().Id) && ((String.IsNullOrWhiteSpace(richieste.DescrizioneFile)) || (richieste.DescrizioneFile.Trim().Length < 7)))
                {
                    Errore("DescrizioneFile");
                }

                if (FileForm == null)
                {
                    Errore("FileForm");
                }

                if (ModelState.IsValid)
                {
                    var fileForm = new FromFileHttp().FileForm(FileForm);
                    if ((fileForm != null) && (String.IsNullOrWhiteSpace(fileForm.err)))
                    {
                        repository.AddFile(richiestedb, richieste, fileForm);
                        richieste.DescrizioneFile = "";
                        richieste.tipoAllegato = null;
                    }
                    else
                    {
                        if (fileForm != null)
                        {
                            ErroreSpecifico("FileForm", fileForm.err);
                        }
                        else
                        {
                            ErroreSpecifico("FileForm", "Errore file");
                        }
                    }
                }
            }
            bool riproponidatidocumenti = false;
            if ((richiestedb.Id != Guid.Empty) && (operationType.EqualsNormalized("AddDocumento")))
            {
                DocumentiRichiesti documentodb = MapObj.ObjectTo<DocumentiRichiesti>(richieste, null);
                if (documentodb.Documento.Id <= 0)
                {
                    Errore("Documento");
                }

                //if ((richieste.Documento.Id == db.TipiDocumenti.Where(a => a.Nome == "Altro").FirstOrDefault().Id) && ((String.IsNullOrWhiteSpace(richieste.DescrizioneDocumento)) || (richieste.DescrizioneDocumento.Trim().Length < 7)))
                if (((String.IsNullOrWhiteSpace(richieste.DescrizioneDocumento)) || (richieste.DescrizioneDocumento.Trim().Length < 7)))
                {
                    Errore("DescrizioneDocumento");
                }

                if (documentodb.NumCopie <= 0)
                {
                    Errore("NumCopie");
                }

                if ((richiestedb.CanaleRicezione != null) &&
                    (richiestedb.CanaleRicezione.Id == repository.GetCanale("Filiale").Id) &&
                    (richiestedb.SpesaAddebitabile) &&
                    (richieste.Costo == null || richieste.Costo <= 0))
                {
                    Errore("Costo");
                }
                if (ModelState.IsValid)
                {
                    if (richieste.Id_Documento == Guid.Empty)
                    {
                        repository.AddDocumento(richiestedb, documentodb);
                    }
                    else
                    {
                        repository.UpdateDocumento(richieste, documentodb);
                    }
                    riproponidatidocumenti = false;
                }
                else
                {
                    riproponidatidocumenti = true;
                }
            }
            if ((DelDocumento != null) && (DelDocumento != Guid.Empty))
            {
                repository.DeleteDocumento((Guid)DelDocumento);
            }
            if ((DelAllegato != null) && (DelAllegato != Guid.Empty))
            {
                repository.DeleteAllegato((Guid)DelAllegato);
            }
            if (!richieste.bypassallegati)
            {
                int StatoSuccessivo_Id = 0;
                if (StatoSuccessivo != null)
                {
                    StatoSuccessivo_Id = StatoSuccessivo.Id;
                    foreach (var allegato in repository.GetCondizioniAllegato(richiestedb.Stato.Id, StatoSuccessivo_Id, canalericezione, canaleconsegna))
                    {
                        if (richiestedb.Allegati.Where(a => a.tipoAllegato.Id == allegato.TipoAllegato.Id).Count() <= 0)
                        {
                            ErroreSpecifico("Allegato", allegato.ErrorMessage);// + " per " + item.StatoOut.Descrizione);
                        }

                    }
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(richiestedb.MotivazioneBypass) || (richiestedb.MotivazioneBypass.Trim().Length < 7))
                {
                    Errore("MotivazioneBypass");
                }
            }
            if ((operationType != null) && (operationType.ContainsNormalized("annulla")))
            {
                ModelState.Clear();
            }
            #endregion

            #region GestioneConfermaCambioStato
            if ((operationType != null) && (operationType.ContainsNormalized("Ok")))
            {
                if ((ModelState.IsValid) && (StatoSuccessivo != null))
                {
                    if (StatoSuccessivo.ChiediMotivazione)
                    {
                        if (((String.IsNullOrWhiteSpace(richieste.Motivazione)) || (richieste.Motivazione.Trim().Length < 7)))
                        {
                            Errore("Motivazione");
                        }
                    }
                }
                if ((ModelState.IsValid) && (StatoSuccessivo != null))
                {
                    richiestedb.Stato = repository.GetStato(StatoSuccessivo.Id);
                    if ((richiestedb.DataInserimento == null) && (richiestedb.Stato.Id != 1))
                    {
                        richiestedb.DataInserimento = DateTime.Now;
                    }

                    if (richiestedb.Stato.Chiuso)
                    {
                        richiestedb.DataChiusa = DateTime.Now;
                    }

                    //if (!String.IsNullOrWhiteSpace(operationType) && (operationType.ContainsNormalized("Respinta per modifica")))
                    //{
                    //    richieste.UOCode_Inoltro = richiestedb.UOCode;
                    //    StatoSuccessivo = repository.GetStato("Respinta per modifica");
                    //    StatoSuccessivoDescrizone = StatoSuccessivo.Descrizione;
                    //}

                    if (!String.IsNullOrWhiteSpace(richieste.UOCode_Inoltro))
                    {
                        richiestedb.UOCodeNext = richieste.UOCode_Inoltro;
                        richieste.UOCode_Inoltro = "";
                        richiestedb.UOCode_Inoltro = "";
                    }
                    if (richiestedb.Stato.Stato.EqualsNormalized("Respinta per modifica"))
                    {
                        richiestedb.UOCodeNext = richiestedb.UOCode;
                    }
                    if ((richiestedb.CanaleConsegna != null) &&
                        (richiestedb.CanaleConsegna.Id == repository.GetCanale("Filiale").Id) &&
                        (!String.IsNullOrWhiteSpace(richiestedb.FilialeUOcode)) &&
                        (richiestedb.FilialeUOcode.Trim().Length >= 5) &&
                        (richiestedb.Stato.Stato.EqualsNormalized("Documenti in Giacenza"))
                        )
                    {
                        richiestedb.UOCodeNext = richiestedb.FilialeUOcode;
                    }
                    //if (richiestedb.Stato.Stato == "Documenti in Giacenza")
                    //{
                    //    richiestedb.UOCodeNext = richiestedb.FilialeConsegna;
                    //}

                    //var condizionecambioattuale = repository.GetStatiPossibili(operationType, richieste.Stato.Id).Where(a => a.StatoOut.Id == StatoSuccessivo.Id).FirstOrDefault();
                    if ((condizionestatotrovata != null) && (richiestedb.Stato.EmailTesti != null) && ((richiestedb.Stato.NotificaUOCodeNext) || (richiestedb.Stato.NotificaUOCode)))
                    {

                        var emailtesto = richiestedb.Stato.EmailTesti;
                        var uocodeList = new List<string>();
                        uocodeList.Add(richiestedb.UOCodeNext);
                        if ((richiestedb.UOCode != richiestedb.UOCodeNext) && (richiestedb.Stato.NotificaUOCode)
                        )
                        {
                            uocodeList.Add(richiestedb.UOCode);
                        }
                        repository.Save(richiestedb);
                        repository.SendEmail(richiestedb, uocodeList, emailtesto, richiestedb.Stato.NotificaResp);
                    }

                    operationType = "StatoCambiato";
                }
                else
                {
                    operationType = "StatoNonCambiato";
                }
            }

            #endregion

            PopolaViewGestione(richiestedb);

            repository.Save(richiestedb);
            //var richiestaView = new GestioneViewModel();
            var richiestaView = MapObj.ObjectTo<GestioneViewModel>(richiestedb, richieste);
            if (ModelState.IsValid)
            {
                richiestaView.StatoSuccessivo = StatoSuccessivoDescrizone;
            }
            else
            {
                richiestaView.StatoSuccessivo = "";
            }


            #region GestioneModificaDocumento
            if ((ModDocumento != null) && (ModDocumento != Guid.Empty))
            {
                DocumentiRichiesti documentiRichiesti = repository.GetDocumento((Guid)ModDocumento);
                if (documentiRichiesti != null)
                {
                    richiestaView.Id_Documento = documentiRichiesti.Id;
                    richiestaView.Rapporto = documentiRichiesti.Rapporto;
                    richiestaView.Documento = documentiRichiesti.Documento;
                    richiestaView.DescrizioneDocumento = documentiRichiesti.DescrizioneDocumento;
                    richiestaView.Costo = documentiRichiesti.Costo;
                    richiestaView.NumCopie = documentiRichiesti.NumCopie;
                    richiestaView.DocOrig = documentiRichiesti.DocOrig;
                    richiestaView.NumCartone = documentiRichiesti.NumCartone;
                    var lista5 = new List<SelectListItem>();
                    foreach (var item in repository.GetTipiDocumenti())
                    {
                        var pippo = new SelectListItem();
                        pippo.Text = item.Descrizione;
                        pippo.Value = (item.Id).ToString();
                        pippo.Selected = item.Id == documentiRichiesti.Documento.Id;
                        lista5.Add(pippo);
                    }
                    ViewBag.TipoDocumenti = lista5;
                }
            }
            else
            {
                if (!riproponidatidocumenti)
                {
                    richiestaView.Id_Documento = Guid.Empty;
                    richiestaView.Rapporto = "";
                    richiestaView.Documento = new TipiDocumenti();
                    richiestaView.DescrizioneDocumento = "";
                    richiestaView.Costo = null;
                    richiestaView.NumCopie = 1;
                    richiestaView.DocOrig = false;
                    richiestaView.NumCartone = null;
                }
            }
            if (!errorecostooblbligatorio)
            {
                var errorecosto = false;
                foreach (var item in richiestaView.DocumentiRichiesti)
                {
                    if ((richiestaView.CanaleConsegna != null) &&
                        //(richiestaView.CanaleConsegna.Id == repository.GetCanale("Filiale").Id) && 
                        richiestaView.SpesaAddebitabile && (item.Costo == null || item.Costo <= 0))
                    {
                        errorecosto = true;
                    }
                }
                if (errorecosto) Errore("errorecosto");
            }

            if (richiestaView.NumCopie < 1) richiestaView.NumCopie = 1;
            #endregion



            //pulisco il contenuto file per non appesantire pagina grafica
            if (richiestaView.Allegati.Count > 0)
                for (int i = 0; i < richiestaView.Allegati.Count; i++)
                {
                    richiestaView.Allegati.ElementAt(i).FileContent = null;
                }

            var result = Json(new
            {
                data = richiestaView,
                errors = !ModelState.IsValid,
                errorList = new ModelStateErrors().GetModelStateErrors(ModelState),
                operationType = operationType
            }, JsonRequestBehavior.AllowGet);
            return result;
        }

        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult LogStati(Guid? id)
        {
            if (id != null)
            {
                if (Request.IsAjaxRequest())
                {
                    return PartialView("LogStati", repository.GetLogStati((Guid)id));
                }
            }
            return RedirectToAction("GestioneRichieste");
        }

        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult Abilitaelementi(int? statoattuale)
        {

            if (statoattuale != null)
            {
                var listaElementi = repository.GetAbilitaElementiStato((int)statoattuale).ToList();
                var placehoders = repository.GetPlaceHolder().ToList();
                foreach (var item in placehoders)
                {
                    listaElementi.Add(new AbilitaElementiStato
                    {
                        Attributo = "id",
                        NomeElemento = item.IdElemento,
                        NomeProprieta = "placeholder",
                        ValoreProprieta = item.PlaceHolder
                    });
                    ;
                }

                if (Request.IsAjaxRequest())
                {
                    return Json(listaElementi);
                }
            }

            return null;
        }



        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult BottoniCambioStato(int? statoattuale, string uocode)
        {
            bool? IsSameUO = null;
            if (!String.IsNullOrWhiteSpace(uocode))
            {
                IsSameUO = uocode == MioUocode;
            }
            if (statoattuale != null)
            {
                var listaElementi = repository.GetStatiPossibili((int)statoattuale, IsSameUO).Select(a => a.StatoOut).OrderBy(c => c.Stato).Distinct().ToList();


                if (Request.IsAjaxRequest())
                {
                    return PartialView(listaElementi);
                }
            }

            return null;
        }

        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult TabellaAllegati(Guid? id)
        {
            var richiestaView = new GestioneViewModel();
            if (id != null)
            {
                var richiestedb = repository.GetRichiesta((Guid)id);

                richiestaView = MapObj.ObjectTo<GestioneViewModel>(richiestedb, richiestaView);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("Allegati", richiestaView);
                }
            }

            return View(richiestaView);

        }

        [HttpPost]
        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult TabellaDocumenti(Guid? id)
        {
            var richiestaView = new GestioneViewModel();
            if (id != null)
            {
                var richiestedb = repository.GetRichiesta((Guid)id);
                richiestaView = MapObj.ObjectTo<GestioneViewModel>(richiestedb, richiestaView);
                ViewBag.CanaleFilialeID = repository.GetCanale("Filiale").Id;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("Documentazione", richiestaView);
                }
            }
            return View(richiestaView);

        }

        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult Download(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var fileDescription = repository.GetFile((Guid)id);

            if (fileDescription == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return File(fileDescription.FileContent, fileDescription.ContentType, fileDescription.FileName);
        }

        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult DownloadDoc(string operationType, Guid? id)
        {
            string content_type = "application / vnd.openxmlformats - officedocument.wordprocessingm";
            byte[] files = null;


            if (id == null || operationType == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var richiestedb = repository.GetRichiesta((Guid)id);

            //stampa documenti
            StampaDocumenti stadoc;
            if ((!String.IsNullOrWhiteSpace(operationType)) && operationType == "StampaRichiesta")
            {
                stadoc = new StampaDocumenti(richiestedb);
                files = stadoc.Compila_Richiesta();
            }
            if ((!String.IsNullOrWhiteSpace(operationType)) && operationType == "StampaConsegna")
            {
                stadoc = new StampaDocumenti(richiestedb);
                files = stadoc.Compila_Consegna();
            }
            if ((!String.IsNullOrWhiteSpace(operationType)) && operationType == "StampaAddspese")
            {
                stadoc = new StampaDocumenti(richiestedb);
                files = stadoc.Compila_Addspese();
            }
            if ((!String.IsNullOrWhiteSpace(operationType)) && operationType == "StampaModGiacenza")
            {
                stadoc = new StampaDocumenti(richiestedb);
                if (richiestedb.CanaleRicezione != null && (richiestedb.CanaleRicezione.Nome == "Pec" || richiestedb.CanaleRicezione.Nome == "Raccomandata"))
                    files = stadoc.Compila_ModDocumentiGiacenza();

                if (richiestedb.CanaleRicezione != null && richiestedb.CanaleConsegna != null &&
                    ((richiestedb.CanaleRicezione.Nome == "Filiale" && richiestedb.CanaleConsegna.Nome == "Filiale") ||
                    (richiestedb.CanaleRicezione.Nome == "Email" && richiestedb.CanaleConsegna.Nome == "Filiale"))
                    )
                    files = stadoc.Compila_ModDocumentiGiacenza2();
            }

            if ((!String.IsNullOrWhiteSpace(operationType)) && operationType == "ModInvioDocumentazione")
            {
                stadoc = new StampaDocumenti(richiestedb);
                //FileStreamResult files = stadoc.Compila_ModInvioDocumentazione();
                files = stadoc.Compila_ModInvioDocumentazione();

            }
            if (files == null) return RedirectToAction("GestioneRichieste");
            return File(files, content_type, operationType + ".docx");
        }
        // GET: Richieste/Details/5
        [OutputCache(Duration = 0, NoStore = true, VaryByParam = "*")]
        public ActionResult Details(Guid? id)
        {
            if ((id == null) || (id == Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Richieste richieste = repository.GetRichiesta((Guid)id);
            if (richieste == null)
            {
                return HttpNotFound();
            }

            //if ((MioUocode != richieste.UOCodeNext) && (MioUocode != richieste.UOCode))
            //{
            //    RedirectToAction("GestioneRichieste");
            //}

            //ViewBag.MatricolaNome = "Operatore";
            //try
            //{
            //    ViewBag.DescrizioneFilialeConsegna = richieste.FilialeConsegna;
            //    if (!String.IsNullOrWhiteSpace(richieste.FilialeConsegna))
            //    {
            //        IQueryable<DimUO> filiali = resourcesRepository.GetDimUoByUocode(richieste.FilialeConsegna);
            //        if (filiali != null)
            //        {
            //            ViewBag.DescrizioneFilialeConsegna = filiali.FirstOrDefault().DescrizioneUObyIstituto;
            //        }
            //    }
            //    ViewBag.DescrizioneUOCode = richieste.UOCode;
            //    if (!String.IsNullOrWhiteSpace(richieste.UOCode))
            //    {
            //        IQueryable<DimUO> filiali = resourcesRepository.GetDimUoByUocode(richieste.UOCode);
            //        if (filiali != null)
            //        {
            //            ViewBag.DescrizioneUOCode = filiali.FirstOrDefault().DescrizioneUObyIstituto;
            //        }
            //    }
            //    ViewBag.DescrizioneUOCodeNext = richieste.UOCodeNext;
            //    if (!String.IsNullOrWhiteSpace(richieste.UOCodeNext))
            //    {
            //        IQueryable<DimUO> filiali = resourcesRepository.GetDimUoByUocode(richieste.UOCodeNext);
            //        if (filiali != null)
            //        {
            //            ViewBag.DescrizioneUOCodeNext = filiali.FirstOrDefault().DescrizioneUObyIstituto;
            //        }
            //    }

            //    if (!String.IsNullOrWhiteSpace(richieste.Matricola))
            //    {
            //        ViewBag.MatricolaNome = resourcesRepository.GetNomeUtente(richieste.Matricola);
            //    }

            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine(ex.Message);
            //    ErrorSignal.FromCurrentContext().Raise(ex);
            //}
            //ViewData["istituto"] = MioIstituto;
            ViewData["uocode"] = MioUocode;
            ViewBag.CanaleFilialeID = repository.GetCanale("Filiale").Id;
            return View(richieste);
        }

        //// GET: Richieste/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    //if (id == null)
        //    //{
        //    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    //}
        //    //Richieste richieste = repository.GetRichiesta((Guid)id);
        //    //if (richieste == null)
        //    //{
        //    //    return HttpNotFound();
        //    //}
        //    //return View(richieste);
        //    if (id.HasValue)
        //    {
        //        Richieste richieste = repository.GetRichiesta((Guid)id);
        //        if ((!UfficioAutorizzato(richieste, MioUocode)) || (richieste.Stato.Stato != "Inizio")) return RedirectToAction("Index");
        //        repository.DeleteRichiesta((Guid)id);
        //    }

        //    return RedirectToAction("Index");
        //}

        //// POST: Richieste/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid? id)
        //{
        //    if (id.HasValue)
        //    {
        //        Richieste richieste = repository.GetRichiesta((Guid)id);
        //        if ((!UfficioAutorizzato(richieste, MioUocode)) || (richieste.Stato.Stato != "Inizio")) return RedirectToAction("Index");
        //        repository.DeleteRichiesta((Guid)id);
        //    }

        //    return RedirectToAction("Index");
        //}

    }
}
