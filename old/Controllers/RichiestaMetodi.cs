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

namespace RCDWeb.Areas.RCD.Controllers
{
    public partial class RichiestaController : Controller
    {

        public void PopolaViewGestione(Richieste richieste)
        {
            var lista = new List<SelectListItem>();
            foreach (var item in repository.GetCanaliRicezione())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Descrizione;
                pippo.Value = item.Id.ToString();
                if ((richieste != null) && (richieste.CanaleRicezione != null))
                    pippo.Selected = richieste.CanaleRicezione.Id == item.Id;
                lista.Add(pippo);
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

            var lista4 = new List<SelectListItem>();
            var uffici = repository.GetUffici(richieste.Istituto, richieste.UOCodeNext, richieste.UOCode);
            foreach (var item in uffici)
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Nome;
                pippo.Value = item.Id.ToString();
                if ((richieste != null) && (!String.IsNullOrWhiteSpace(richieste.UOCode_Inoltro)))
                    pippo.Selected = richieste.UOCode_Inoltro == item.UOCODE;
                lista4.Add(pippo);
            }
            ViewBag.DropDownUffici = lista4;

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


            bool? legittimato = null;
            if ((richieste != null) && (richieste.Legittimato != null))
                legittimato = richieste.Legittimato;
            var lista8 = new List<SelectListItem> {
                new SelectListItem { Value= "" , Text="-- Selezionare ---", Selected = legittimato==null },
                new SelectListItem { Value= ((bool)true).ToString() , Text="Legittimato", Selected = legittimato==true },
                new SelectListItem { Value= ((bool)false).ToString() , Text="Non Legittimato", Selected = legittimato==false },
            };
            ViewBag.DropDownLegit = lista8;

            bool? cessione = null;
            if ((richieste != null) && (richieste.Cessione != null))
                cessione = richieste.Cessione;
            var lista9 = new List<SelectListItem> {
                new SelectListItem { Value= "" , Text="-- Selezionare ---", Selected = cessione==null },
                new SelectListItem { Value= ((bool)true).ToString() , Text="Cessione rilevata", Selected = cessione==true },
                new SelectListItem { Value= ((bool)false).ToString() , Text="Cessione non  rilevata", Selected = cessione==false },
            };
            ViewBag.DropDownCess = lista9;


            
            if (!String.IsNullOrWhiteSpace(MiaFiliale))
            {
                richieste.FilialeConsegna = MioUocode;
                ViewData["FilialeC"] = MioUocode+ " - "+ MiaFiliale;
                ViewData["FilialeDesc"] = MiaFiliale;
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(richieste.FilialeConsegna))
                {
                    IQueryable<DimUO> filiali = resourcesRepository.GetDimUoByUocode(MioUocode);
                    var filiale = filiali.FirstOrDefault().DescrizioneUObyIstituto;
                    ViewData["FilialeC"] = richieste.FilialeConsegna + " - " + filiale;
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

        public ActionResult Gestione(Guid? id, int istituto = 0, long ndg = 0)
        {
            Richieste richieste;
            if (id == null)
            {
                richieste = new Richieste();
                richieste.DataRichiesta = DateTime.Now;
                richieste.Stato = repository.GetStato(1);

                richieste.UOCode = MioUocode;
                richieste.UOCodeNext = MioUocode;
                richieste.Matricola = MiaMatricola;
                richieste.Istituto = MioIstituto;

                GetCliente(richieste, istituto, ndg);

            }
            else
            {
                richieste = repository.GetRichiesta((Guid)id);
                if (!UfficioAutorizzato(richieste, MioUocode)) return RedirectToAction("Index");
                if (richieste.Stato.Id == repository.GetStato("Assegnata").Id)
                {
                    richieste.Stato = repository.GetStato("In Lavorazione");
                    repository.Save(richieste);
                }
            }


            PopolaViewGestione(richieste);

            var richiestaView = new MapObj().ObjectToStatic<GestioneViewModel>(richieste, null);
            if (richiestaView.NumCopie<1) richiestaView.NumCopie = 1;
            return View(richiestaView);
        }

        private void CopyfromView(GestioneViewModel richieste, Richieste richiestedb)
        {
            if (richiestedb.Stato == null)
                richiestedb.Stato = repository.GetStato(1);

            richieste.UltimaModifica = DateTime.Now;
            if (richieste.DataRichiesta == null) richieste.DataRichiesta = DateTime.Now;

            if ((richieste.CanaleConsegna != null) && (richieste.CanaleConsegna.Id > 0))
            {
                richiestedb.CanaleConsegna = repository.GetCanale(richieste.CanaleConsegna.Id);
            }
            if ((richieste.CanaleRicezione != null) && (richieste.CanaleRicezione.Id > 0))
            {
                richiestedb.CanaleRicezione = repository.GetCanale(richieste.CanaleRicezione.Id);
            }
            if ((richieste.TipoRichiedente != null) && (richieste.TipoRichiedente.Id > 0))
            {
                richiestedb.TipoRichiedente = repository.GetTipoRichiedente(richieste.TipoRichiedente.Id);
            }
            if ((richieste.ModalitaPagamento != null) && (richieste.ModalitaPagamento.Id > 0))
            {
                richiestedb.ModalitaPagamento = repository.GetTipoPagamento(richieste.ModalitaPagamento.Id);
            }

            if ((richieste.CanaleConsegna != null) &&
               (richieste.CanaleConsegna.Id == repository.GetCanale("Pec").Id) )
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
            if ((richieste.CanaleConsegna != null) &&
                (richieste.CanaleConsegna.Id == repository.GetCanale("Filiale").Id))
            {
                richiestedb.FilialeConsegna = richieste.FilialeConsegna;
            }
            else
            {
                richiestedb.FilialeConsegna = "";
            }

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
            richiestedb.SedeLegale = richieste.SedeLegale;
            richiestedb.SpesaAddebitabile = richieste.SpesaAddebitabile;
            richiestedb.Filiale = richieste.Filiale;
            
            richiestedb.UOCode_Inoltro = richieste.UOCode_Inoltro;
            richiestedb.NaturaGiuridica = richieste.NaturaGiuridica;

        }
        private void Errore(string id, string nome="")
        {
            if (String.IsNullOrWhiteSpace(nome)) nome = id;
            var err = repository.GetErroreGenerico(nome);
            if (err != null)
            {
                var message = err.ErrorMessage;
                ModelState.AddModelError(id, message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Gestione(GestioneViewModel richieste, string operationType, Guid? ModDocumento, Guid? DelDocumento, HttpPostedFileBase FileForm, Guid? DelAllegato)  //[Bind(Include = "Id,NDG,isPerson,IntestazioneNDG,Nome,IntestazioneNDG,CodiceFiscale,PIva,LuogoNascita,SedeLegale,IscrizioneCCIAA,DataNascita,AlboProfessionale,Indirizzo,Richiedente,CanaleRicezione,CanaleConsegna,email,pec,ModalitaPagamento,NumeroConto,AddebitoCassa,AddebitoIBAN,Titolare,TipoRichiedente,Cessione,Legittimato,InoltroUfficio,Motivazione,Note,DataPagamento,DataRichiesta,DataRichiesta,CanaleComunicazione,DataAvviso,DataIIAvviso,DataConsegna,DataChiusa,Filiale,Luogo,Matricola,UltimaModifica")]
        {
            if (!UfficioAutorizzato(richieste, MioUocode)) return RedirectToAction("Index");
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(operationType));
            ModelState.Clear();

            var richiestedb = repository.GetRichiestaOrCreate(richieste.Id);

            CopyfromView(richieste, richiestedb);
            Stati StatoSuccessivo = null;
            var StatoSuccessivoDescrizone = "";
            bool statoverificato = false;
            if (!string.IsNullOrWhiteSpace(operationType))
            {
                var statipossibili = repository.GetStatiPossibili(operationType, richieste.Stato.Id).ToList();
                CondizioneStato condizionestatotrovata = null;
                foreach (var item in statipossibili)
                {
                    statoverificato = true;

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
                                if (rich.Elemento.Tipo == "Canale")
                                {
                                    if (rich.Elemento.Nome == "CanaleRicezione")
                                    {
                                        if ((richieste.CanaleRicezione == null) || (richieste.CanaleRicezione.Id != val))
                                        {
                                            statoverificato = false;
                                            ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                        }
                                    }
                                    else
                                    if (rich.Elemento.Nome == "CanaleConsegna")
                                    {
                                        if ((richieste.CanaleConsegna == null) || (richieste.CanaleConsegna.Id != val))
                                        {
                                            statoverificato = false;
                                            ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                        }
                                    }
                                }
                                else
                                if (rich.Elemento.Tipo == "TipiRichiedente")
                                {
                                    if ((valore == null) || (((TipiRichiedente)valore).Id != val))
                                    {
                                        statoverificato = false;
                                        ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                    }
                                }
                                else
                                if (rich.Elemento.Tipo == "Boolean")
                                {
                                    if ((valore == null) || ((bool)valore != Convert.ToBoolean(val)))
                                    {
                                        statoverificato = false;
                                        ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                    }
                                }
                            }
                        }
                        if (rich.Min != null)
                        {
                            var min = (int)rich.Min;
                            if (rich.Elemento.Tipo == "Canale")
                            {
                                if (rich.Elemento.Nome == "CanaleRicezione")
                                {
                                    if ((richieste.CanaleRicezione == null) || (richieste.CanaleRicezione.Id < min))
                                    {
                                        statoverificato = false;
                                        ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                    }
                                }
                                else
                                if (rich.Elemento.Nome == "CanaleConsegna")
                                {
                                    if ((richieste.CanaleConsegna == null) || (richieste.CanaleConsegna.Id < min))
                                    {
                                        statoverificato = false;
                                        ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                    }
                                }
                            }
                            else
                            if (rich.Elemento.Tipo == "TipiRichiedente")
                            {
                                if (rich.Elemento.Nome == "TipoRichiedente")
                                {
                                    if ((richieste.TipoRichiedente == null) || (richieste.TipoRichiedente.Id < min))
                                    {
                                        statoverificato = false;
                                        ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);
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
                                        if (prop.FullName.Contains("DateTime"))
                                        {
                                            tipo = "DateTime";
                                        }
                                        if (tipo == "DateTime")
                                        {
                                            if (((DateTime)valore).Year < min)
                                            {
                                                statoverificato = false;
                                                ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                            }
                                        }
                                        if (tipo == "String")
                                        {
                                            if (String.IsNullOrWhiteSpace((String)valore) || (((String)valore).Length < min))
                                            {
                                                statoverificato = false;
                                                ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                            }
                                        }
                                        if (tipo == "Int64")
                                        {
                                            if ((long)valore < min)
                                            {
                                                statoverificato = false;
                                                ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                            }
                                        }
                                        if (tipo == "Int32")
                                        {
                                            if ((int)valore < min)
                                            {
                                                statoverificato = false;
                                                ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        statoverificato = false;
                                        ModelState.AddModelError(rich.Elemento.IdElemento, rich.ErrorMessage + " per stato " + item.StatoOut.Descrizione);
                                    }
                                }
                            }

                        }

                    }
                    foreach (var allegato in repository.GetTipoAllegato()
                        .Where(a => (a.Obbligatorio) && (a.StatoSuccessivoObbligatorio.Id == item.StatoOut.Id)))
                    {
                        if (richiestedb.Allegati.Where(a => a.tipoAllegato.Id == allegato.Id).Count() <= 0)
                        {
                            statoverificato = false;
                            ModelState.AddModelError("Allegato", allegato.ErrorMessage + " " + allegato.Nome + " per " + item.StatoOut.Descrizione);
                        }

                    }
                    if (statoverificato)
                    {
                        StatoSuccessivo = item.StatoOut;
                        condizionestatotrovata = item;
                        StatoSuccessivoDescrizone = StatoSuccessivo.Descrizione;
                        ModelState.Clear();
                        break;
                    }
                }
            }

            if ((richiestedb.Id != Guid.Empty) && (operationType == "AddFile"))
            {
                if (richieste.tipoAllegato.Id <= 0)
                {
                    //ModelState.AddModelError("tipoAllegato", "Richiesto");
                    Errore("tipoAllegato");
                }

                if ((richieste.tipoAllegato.Id == db.TipoAllegato.Where(a => a.Nome == "Altro").FirstOrDefault().Id) && ((String.IsNullOrWhiteSpace(richieste.DescrizioneFile)) || (richieste.DescrizioneFile.Trim().Length < 7)))
                {
                    //ModelState.AddModelError("DescrizioneFile", "Richiesti almeno 7 caratteri.");
                    Errore("DescrizioneFile");
                }

                if (FileForm == null)
                {
                    //ModelState.AddModelError("FileForm", "Richiesto");
                    Errore("FileForm");
                }

                if (ModelState.IsValid)
                {
                    var fileForm = new FromFileHttp().FileForm(FileForm);
                    if ((fileForm != null) && (String.IsNullOrWhiteSpace(fileForm.err)))
                    {
                        repository.AddFile(richiestedb, richieste, fileForm);
                    }
                    else
                    {
                        if (fileForm != null)
                        { 
                            ModelState.AddModelError("FileForm", fileForm.err); 
                        }
                        else 
                        { 
                            ModelState.AddModelError("FileForm", "Errore file"); 
                        }
                    }
                }
            }
            if ((richiestedb.Id != Guid.Empty) && (operationType == "AddDocumento"))
            {
                DocumentiRichiesti documentodb = new MapObj().ObjectToStatic<DocumentiRichiesti>(richieste, null);
                if (documentodb.Documento.Id <= 0)
                {
                    //ModelState.AddModelError("Documento", "Richiesto");
                    Errore("Documento");
                }

                //if ((richieste.Documento.Id == db.TipiDocumenti.Where(a => a.Nome == "Altro").FirstOrDefault().Id) && ((String.IsNullOrWhiteSpace(richieste.DescrizioneDocumento)) || (richieste.DescrizioneDocumento.Trim().Length < 7)))
                if (((String.IsNullOrWhiteSpace(richieste.DescrizioneDocumento)) || (richieste.DescrizioneDocumento.Trim().Length < 7)))
                {
                    //ModelState.AddModelError("DescrizioneDocumento", "Richiesti almeno 7 caratteri.");
                    Errore("DescrizioneDocumento");
                }

                if (documentodb.NumCopie <= 0)
                {
                    //ModelState.AddModelError("NumCopie", "Numero deve essere >0");
                    Errore("NumCopie");
                }

                if ((richiestedb.CanaleConsegna != null) &&
                    (richiestedb.CanaleConsegna.Id == repository.GetCanale("Filiale").Id) &&
                    (richieste.Costo <= 0))
                {
                    //ModelState.AddModelError("Costo", "Costo deve essere > 0");
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
            if ((operationType != null) && (operationType.Replace("Ok", "") == "Respingixmod"))
            {
                richieste.UOCode_Inoltro = richiestedb.UOCode;
                StatoSuccessivo = repository.GetStato("Inserita");
                StatoSuccessivoDescrizone = StatoSuccessivo.Descrizione;
            }



            if ((richiestedb.CanaleRicezione != null) &&
                (richiestedb.CanaleRicezione.Id == repository.GetCanale("Filiale").Id) && 
                 richiestedb.ModalitaPagamento== null ) 
            {                
                Errore("ModalitaPagamento");
            }

            if ((richiestedb.CanaleConsegna != null) &&
                (richiestedb.CanaleConsegna.Id == repository.GetCanale("Pec").Id) &&
                ((String.IsNullOrWhiteSpace(richieste.pec)) || !(new EmailAddressAttribute().IsValid(richieste.pec))))
            {
                //ModelState.AddModelError("pec", "Pec non valida");
                Errore("pec", "pecnonvalida");
            }
            if ((richiestedb.CanaleConsegna != null) &&
                (richiestedb.CanaleConsegna.Id == repository.GetCanale("Email").Id) &&
                ((String.IsNullOrWhiteSpace(richieste.email)) || !(new EmailAddressAttribute().IsValid(richieste.email))))
            {
                //ModelState.AddModelError("email", "Email non valida");
                Errore("email", "emailnonvalida");
            }
            if ((richiestedb.CanaleConsegna != null) && (richiestedb.CanaleConsegna.Id == repository.GetCanale("Filiale").Id) &&
                ((String.IsNullOrWhiteSpace(richiestedb.FilialeConsegna)) || (richiestedb.FilialeConsegna.Trim().Length < 5)))
            {
                //ModelState.AddModelError("FilialeConsegna", "Richiesti almeno 7 caratteri.");
                Errore("FilialeConsegna");
            }
            if ((richiestedb.CanaleConsegna != null) && (richiestedb.CanaleConsegna.Id == repository.GetCanale("Pec").Id) &&
                ((String.IsNullOrWhiteSpace(richiestedb.pec)) || (richiestedb.pec.Trim().Length < 7)))
            {
                // ModelState.AddModelError("pec", "Richiesti almeno 7 caratteri.");
                Errore("pec");
            }
            if ((richiestedb.CanaleConsegna != null) && (richiestedb.CanaleConsegna.Id == repository.GetCanale("Email").Id) &&
                 ((String.IsNullOrWhiteSpace(richiestedb.email)) || (richiestedb.email.Trim().Length < 7)))
            {
                //ModelState.AddModelError("email", "Richiesti almeno 7 caratteri.");
                Errore("email");
            }

            if ((richiestedb.TipoRichiedente != null) &&
                (richiestedb.TipoRichiedente.Nome == "Curatore") &&
                string.IsNullOrEmpty( richiestedb.Richiedente))
            {
                Errore("Richiedente");
            }

            if ((richiestedb.ModalitaPagamento != null) && (richiestedb.ModalitaPagamento.Id == repository.GetTipoPagamento("Conto").Id) &&
                ((String.IsNullOrWhiteSpace(richiestedb.NumeroConto)) || (richiestedb.NumeroConto.Trim().Length < 7)))
            {
                //ModelState.AddModelError("NumeroConto", "Richiesti almeno 7 caratteri.");
                Errore("NumeroConto");
            }
            if ((richiestedb.DocumentiRichiesti == null) || (richiestedb.DocumentiRichiesti.Count() == 0))
            {
                //ModelState.AddModelError("Documenti", "Attenzione non sono presenti documenti richiesti.");
                Errore("Documenti");

            }

            if (((richiestedb.DataRichiesta != null) && (((DateTime)richiestedb.DataRichiesta).Date > DateTime.Now.Date)))
            {
                //ModelState.AddModelError("DataRichiesta", "Attenzione Data non può essere superiore alla data di oggi.");
                Errore("DataRichiesta");
            }
            if (((richiestedb.DataAvviso != null) && (((DateTime)richiestedb.DataAvviso).Date > DateTime.Now.Date)))
            {
                //ModelState.AddModelError("DataAvviso", "Attenzione Data non può essere superiore alla data di oggi.");
                Errore("DataAvviso");
            }
            if (((richiestedb.DataIIAvviso != null) && (((DateTime)richiestedb.DataIIAvviso).Date > DateTime.Now.Date)))
            {
                //ModelState.AddModelError("DataIIAvviso", "Attenzione Data non può essere superiore alla data di oggi.");
                Errore("DataIIAvviso");
            }
            if (((richiestedb.DataConsegna != null) && (((DateTime)richiestedb.DataConsegna).Date > DateTime.Now.Date)))
            {
                //ModelState.AddModelError("DataConsegna", "Attenzione Data non può essere superiore alla data di oggi.");
                Errore("DataConsegna");
            }

            if ((richiestedb.DataAvviso != null) && ((richiestedb.DataRichiesta == null) || (richiestedb.DataAvviso < richiestedb.DataRichiesta)))
            {
                //ModelState.AddModelError("DataAvviso", "Attenzione Data Avviso non può essere inferiore a Data Richiesta.");
                Errore("DataAvviso", "DataAvvisoRispettoDataRichiesta");
            }
            if ((richiestedb.DataIIAvviso != null) && ((richiestedb.DataAvviso == null) || (richiestedb.DataIIAvviso < richiestedb.DataAvviso)))
            {
                //ModelState.AddModelError("DataIIAvviso", "Attenzione Data Avviso II non può essere inferiore a Data Avviso.");
                Errore("DataIIAvviso", "DataIIAvvisoRispettoDataAvviso");
            }
            if (richiestedb.Stato.Id != repository.GetStato("In Lavorazione").Id)
            {
                if ((richiestedb.DataConsegna != null) && ((richiestedb.DataAvviso == null) || (richiestedb.DataConsegna < richiestedb.DataAvviso)))
                {
                    //ModelState.AddModelError("DataConsegna", "Attenzione Data Consegna non può essere inferiore a Data Avviso.");
                    Errore("DataConsegna", "DataConsegnaRispettoDataAvviso");
                }
            }
            else
            {
                if ((richiestedb.DataConsegna != null) && ((richiestedb.DataRichiesta == null) || (richiestedb.DataConsegna < richiestedb.DataRichiesta)))
                {
                    //ModelState.AddModelError("DataConsegna", "Attenzione Data Consegna non può essere inferiore a Data Richiesta.");
                    Errore("DataConsegna", "DataConsegnaRispettoDataRichiesta");
                }
            }

            if ((operationType != null) && (operationType.Contains("Ok")))
            {
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

                    if (!String.IsNullOrWhiteSpace(richieste.UOCode_Inoltro))
                    {
                        richiestedb.UOCodeNext = richieste.UOCode_Inoltro;
                        richieste.UOCode_Inoltro = "";
                        richiestedb.UOCode_Inoltro = "";
                    }

                    if ((richiestedb.CanaleConsegna != null) && (richiestedb.CanaleConsegna.Id == repository.GetCanale("Filiale").Id) &&
                        (!String.IsNullOrWhiteSpace(richiestedb.FilialeConsegna)) && (richiestedb.FilialeConsegna.Trim().Length >= 5) &&
                        (richiestedb.Stato.Id == repository.GetStato("Documenti in Giacenza").Id)
                        )
                    {
                        richiestedb.UOCodeNext = richiestedb.FilialeConsegna;
                    }


                    var condizionecambioattuale = repository.GetStatiPossibili(operationType, richieste.Stato.Id).Where(a => a.StatoOut.Id == StatoSuccessivo.Id).FirstOrDefault();
                    if ((condizionecambioattuale != null) && ((condizionecambioattuale.Notifica) || (condizionecambioattuale.NotificaResp)))
                    {
                        var emailset = repository.EmailSettings();
                        var emailtesto = repository.EmailTesto("cambiostato");
                        if ((emailset != null) && (emailtesto != null))
                        {
                            var oggetto = emailtesto.Oggetto;
                            var Corpo = emailtesto.Corpo;

                            Corpo = Corpo.Replace("{descrizioneStato}", richiestedb.Stato.Descrizione);
                            Corpo = Corpo.Replace("{dataRichiesta}", ((DateTime)richiestedb.DataRichiesta).ToString("dd/MM/yyyy"));
                            Corpo = Corpo.Replace("{ndg}", richiestedb.NDG.ToString());
                            Corpo = Corpo.Replace("{UOCode}", richiestedb.UOCode);
                            Corpo = Corpo.Replace("{UOCodeNext}", richiestedb.UOCodeNext);
                            Corpo = Corpo.Replace("{note}", richiestedb.Note);
                            Corpo = Corpo.Replace("{motivazione}", richiestedb.Motivazione);
                            //var fullUrl = this.Url.Action("Gestione", "Richiesta", new { id = richiestedb.Id }, this.Request.Url.Scheme);
                            var fullUrl = this.Url.Action("Index", "Richiesta", new { }, this.Request.Url.Scheme);
                            Corpo = Corpo.Replace("{link}", fullUrl);

                            var sendmail = new EmailSender(emailset);
                            string destinatari = emailset.SmtpUsername;
                            sendmail.SendEmailAsync(destinatari, oggetto, Corpo, emailtesto.Html);
                        }
                    }



                    operationType = "StatoCambiato";
                }
                else
                {
                    operationType = "StatoNonCambiato";
                }
            }

            PopolaViewGestione(richiestedb);

            repository.Save(richiestedb);

            //var richiestaView = new GestioneViewModel();

            var richiestaView = new MapObj().ObjectToStatic<GestioneViewModel>(richiestedb, richieste);
            richiestaView.StatoSuccessivo = StatoSuccessivoDescrizone;

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
                    foreach (var item in db.TipiDocumenti.Where(s => s.IsActive == true))
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
                richiestaView.Id_Documento = Guid.Empty;
                richiestaView.Rapporto = "";
                richiestaView.Documento = new TipiDocumenti();
                richiestaView.DescrizioneDocumento = "";
                richiestaView.Costo = 0;
                richiestaView.NumCopie = 1;
                richiestaView.DocOrig = false;
                richiestaView.NumCartone = null;
            }

            if (richiestaView.NumCopie < 1) richiestaView.NumCopie = 1;
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
        public ActionResult LogStati(Guid? id)
        {
            if (id != null)
            {
                if (Request.IsAjaxRequest())
                {
                    return PartialView("LogStati", repository.GetLogStati((Guid)id));
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult TabellaAllegati(Guid? id)
        {
            var richiestaView = new GestioneViewModel();
            if (id != null)
            {
                var richiestedb = repository.GetRichiesta((Guid)id);

                richiestaView = new MapObj().ObjectToStatic<GestioneViewModel>(richiestedb, richiestaView);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("Allegati", richiestaView);
                }
            }

            return View(richiestaView);

        }

        [HttpPost]
        public ActionResult TabellaDocumenti(Guid? id)
        {
            var richiestaView = new GestioneViewModel();
            if (id != null)
            {
                var richiestedb = repository.GetRichiesta((Guid)id);
                richiestaView = new MapObj().ObjectToStatic<GestioneViewModel>(richiestedb, richiestaView);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("Documentazione", richiestaView);
                }
            }
            return View(richiestaView);

        }

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

        // GET: Richieste/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Richieste richieste = repository.GetRichiesta((Guid)id);
            if (richieste == null)
            {
                return HttpNotFound();
            }
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
