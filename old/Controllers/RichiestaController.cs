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
using AreaCommonWeb.DAL.Profiles;
using AreaCommonWeb.Models;
using AreaCommonWeb.ModelView.Security;
using RCDWeb.Areas.RCD.Repository;
using AreaCommonWeb.ModelView;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using WebServiceModel.WSAnagrafe;
using WebServiceModel.DAL;

namespace RCDWeb.Areas.RCD.Controllers
{
    public partial class RichiestaController : Controller
    {
        private ResourcesRepository resourcesRepository;

        private RCDContext db = new RCDContext();
        private RCDRepository repository;
        private CommonBusiness commonBusiness = new CommonBusiness();

        private ProfilesRepository profilesRepository = new ProfilesRepository();
        private ResourcesRepository resoucesRepository = new ResourcesRepository();

        private int MioIstituto;
        private string MioUocode;
        private string MiaMatricola;
        private string MiaFiliale;

        public RichiestaController()
        {
            this.resourcesRepository = new ResourcesRepository(new ResourcesContext(), new UserProfileManager().GetCurrent().Utenza.CodApp);
            repository = new RCDRepository(db);

            UserProfile myProfile = new UserProfileManager().GetCurrent();
            var myUocode = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");
            string flagUffRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "flagUffRuolo"); //////non credo serva
            string filtroIst = UserProfileManager.GetFiltroSicurezza(myProfile, "IST");
            int? istID = !String.IsNullOrEmpty(filtroIst) ? Convert.ToInt32(filtroIst) : Convert.ToInt32(myProfile.UserInfo.CodIstituto);
            var matricola = myProfile.UserInfo.Matricola;


            if (istID != null) MioIstituto = (int)istID;
            MiaMatricola = matricola;
            MioUocode = myUocode;
            var myFlaf = myProfile.UserInfo.FlagUfficioFiliale;
            if ((myFlaf == "F") || (myFlaf == "O"))
            {
                IQueryable<DimUO> filiali = resourcesRepository.GetDimUoByUocode(MioUocode);
                MiaFiliale = filiali.FirstOrDefault().DescrizioneUObyIstituto;
            }

        }

        public RichiestaController(ResourcesRepository resourcesRepository)
        {
            this.resourcesRepository = resourcesRepository;
            repository = new RCDRepository(db);

            UserProfile myProfile = new UserProfileManager().GetCurrent();
            var myUocode = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");
            string flagUffRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "flagUffRuolo"); //////non credo serva
            string filtroIst = UserProfileManager.GetFiltroSicurezza(myProfile, "IST");
            int? istID = !String.IsNullOrEmpty(filtroIst) ? Convert.ToInt32(filtroIst) : Convert.ToInt32(myProfile.UserInfo.CodIstituto);
            var matricola = myProfile.UserInfo.Matricola;

            var MiaFiliale = myProfile.UserInfo.SedeLavoro;
            if (istID != null) MioIstituto = (int)istID;
            MiaMatricola = matricola;
            MioUocode = myUocode;
            var myFlaf = myProfile.UserInfo.FlagUfficioFiliale;
            if ((myFlaf == "F") || (myFlaf == "O"))
            {
                IQueryable<DimUO> filiali = resourcesRepository.GetDimUoByUocode(MioUocode);
                MiaFiliale = filiali.FirstOrDefault().DescrizioneUObyIstituto;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public JsonResult GetFiliali(string prefix)
        {
            IQueryable<DimUO> filiali = commonBusiness.getUoByText(prefix).Where(a => a.FlagUfficioFiliale == "F" || a.FlagUfficioFiliale == "O");
            return Json(filiali, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            UserProfile myProfile = new UserProfileManager().GetCurrent();
            ViewData.Add("istituto", myProfile.UserInfo.CodIstituto);
            ViewData.Add("myUocode", myProfile.UserInfo.CodUnit);


            var myUocode = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");

            string flagUffRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "flagUffRuolo");

            //ISTITUTO
            var selectListIst = new[] {
                                new Istituto { codice = 0 , nome = ""},
                                new Istituto { codice = 46, nome = "46"},
                                new Istituto { codice = 47, nome = "47"}
                            };


            //ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome");


            //AREA
            string filtroSeg = UserProfileManager.GetFiltroSicurezza(myProfile, "SEG");
            string segArea = !String.IsNullOrEmpty(filtroSeg)
                                     ? filtroSeg
                                     : UserProfileManager.GetFiltroSicurezza(myProfile, "mercato");
            //segArea = "";
            IQueryable listAree;

            //FILIALI
            IQueryable<Vista_Filiali> listFil = commonBusiness.GetFilialiHub(selectListIst.First().codice, "");

            //Gestori
            string filtroIst = UserProfileManager.GetFiltroSicurezza(myProfile, "IST");
            int? istituto = !String.IsNullOrEmpty(filtroIst) ?
                                   Convert.ToInt32(filtroIst) :
                                   Convert.ToInt32(myProfile.UserInfo.CodIstituto);

            ViewData["DataDa"] = System.DateTime.Today.AddMonths(-1).ToString().Substring(0, 10);
            ViewData["DataA"] = System.DateTime.Today.ToString().Substring(0, 10);

            int? istID = !String.IsNullOrEmpty(filtroIst) ?
                                     Convert.ToInt32(filtroIst) :
                                     Convert.ToInt32(myProfile.UserInfo.CodIstituto);

            FiltriHomeModel filtri = new FiltriHomeModel();

            //Check Mercati
            //List<CheckMercatiModel> checkMercati = richiestaBusiness.GetCheckBoxMercati(segArea, myProfile.RuoliSicurezza, "");
            //ViewData["checkBoxMercati"] = checkMercati;

            if (UserProfileManager.ContainsSottoFunzione(myProfile.RuoliSicurezza, "S01"))
            {
                istID = null;
                listAree = commonBusiness.GetAree("", null);
                ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome", 0);
                ViewData["SelArea"] = new SelectList(listAree, "UOArea", "CodArea", 0);
                ViewData["SelFilialiHub"] = new SelectList(new[] { "" }, "", "");
                ViewData["SelFilialiSpoke"] = new SelectList(new[] { "" }, "", "");
                ViewData["selFiltriHome"] = "false";
            }
            else
            {
                istID = Convert.ToInt32(istID);
                listAree = commonBusiness.GetAree("", istID);
                filtri = commonBusiness.GetFiltriHome(istID, myUocode, flagUffRuolo, segArea);

                ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome", istID);
                ViewData["SelArea"] = new SelectList(listAree, "UOArea", "CodArea", filtri.area);
                if (filtri.filialiHub.Any() && filtri.filialiHub.First().FlagUfficioCapofila == "T")
                {
                    ViewData["SelFilialiHub"] = new SelectList(filtri.filialiHub.Select(x => new { UOFiliale = x.UOCapofila, Filiale = x.DescCapofila.Split('-')[1] }).Distinct(), "UOFiliale", "Filiale"); //(IEnumerable<String>)filtri.filialiHub.Select(f => f.DescFiliale).ToList();

                }
                else
                {
                    ViewData["SelFilialiHub"] = new SelectList(filtri.filialiHub, "UOFiliale", "Filiale"); //(IEnumerable<String>)filtri.filialiHub.Select(f => f.DescFiliale).ToList();

                }
                ViewData["SelFilialiSpoke"] = new SelectList(filtri.filialiSpoke, "UOFiliale", "Filiale"); //(IEnumerable<String>)filtri.filialiSpoke.Select(f => f.DescFiliale).ToList();
                ViewData["selFiltriHome"] = "true";
            }



            ViewData["istituto"] = istID;
            ViewData["area"] = filtri.area;
            ViewData["flagUff"] = flagUffRuolo;
            ViewData["uocode"] = myUocode;
            ViewData["haveSottoFunzS01"] = UserProfileManager.ContainsSottoFunzione(myProfile.RuoliSicurezza, "S01");

            var lista = new List<SelectListItem>();
            foreach (var item in repository.GetStati())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Descrizione;
                pippo.Value = item.Id.ToString();
                lista.Add(pippo);
            }
            ViewData["StatoRichiestaF"] = lista;

            return View("Index");
        }

        public ActionResult ApriRichiestaVuota()
        {

          UserProfile myProfile = new UserProfileManager().GetCurrent();
          ViewData.Add("istituto", myProfile.UserInfo.CodIstituto);
          ViewData.Add("myUocode", myProfile.UserInfo.CodUnit);

          var myUocode = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");

          string flagUffRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "flagUffRuolo");

          //ISTITUTO
          var selectListIst = new[]
          {
            //new Istituto {codice = 0, nome = ""},
            new Istituto {codice = 46, nome = "46"},
            new Istituto {codice = 47, nome = "47"}
          };
          string filtroIst = UserProfileManager.GetFiltroSicurezza(myProfile, "IST");
          int? istID = !String.IsNullOrEmpty(filtroIst)
            ? Convert.ToInt32(filtroIst)
            : Convert.ToInt32(myProfile.UserInfo.CodIstituto);

          FiltriHomeModel filtri = new FiltriHomeModel();
          IQueryable listAree;
          if (UserProfileManager.ContainsSottoFunzione(myProfile.RuoliSicurezza, "S01"))
          {
            istID = Convert.ToInt32(istID);
            ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome", istID);
          }
          else
          {
            istID = Convert.ToInt32(istID);
            ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome", istID);

          }

          ViewData["istituto"] = istID;
          ViewData["haveSottoFunzS01"] = UserProfileManager.ContainsSottoFunzione(myProfile.RuoliSicurezza, "S01");


            var lista = new List<SelectListItem>();
            foreach (var item in repository.GetStati())
            {
                var pippo = new SelectListItem();
                pippo.Text = item.Descrizione;
                pippo.Value = item.Id.ToString();
                lista.Add(pippo);
            }
            ViewData["StatoRichiestaF"] = lista;



            return View("InitSearch");
        }

        [HttpPost]
        public PartialViewResult _GridElencoRichieste(string intestazione,
          string DataRichiestaDA,
          string DataRichiestaA,
          int? StatoRichiestaF,
          string ndg,
          string istitutoR,
          string FlagUff,
          string uocode,
          string propBlocPost

        )
        {
          UserProfile myProfile = new UserProfileManager().GetCurrent();

          var myUocode = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");

          FiltriRichiesteModel filtri = new FiltriRichiesteModel();
          filtri.istituto = istitutoR != "" ? Convert.ToInt32(istitutoR) : (int?)null;
          filtri.idStatoOrdine = StatoRichiestaF;
          filtri.intestazione = intestazione;
          filtri.ndg = ndg != "" ? Convert.ToInt64(ndg) : (long?)null;
          filtri.dataOrdineA = DataRichiestaA;
          filtri.dataOrdineDA = DataRichiestaDA;
          filtri.ruoliSicurezza = myProfile.RuoliSicurezza;
          filtri.filtroUocode = uocode;
          filtri.myUocode = myUocode;
          filtri.flagUffFil = FlagUff;
          filtri.campoGenerico = propBlocPost;

          if (filtri.istituto == null) filtri.istituto = MioIstituto; 
          ViewData["istituto"] = filtri.istituto; // da valutare
          ViewData["uocode"] = myUocode; // da valutare

          return PartialView("IndexJqueryTable", repository.GetRichieste(filtri));

        }

        public ActionResult InitSearch()
        {
            return View(new AreaCommonWeb.Models.CampiRicerca());
        }

        [HttpPost]
        public PartialViewResult _GridElencoNdg(string cognome, string nome, string istitutoR, string ndg, string codiceFiscale)
        {
            UserProfile myProfile = new UserProfileManager().GetCurrent();
            FiltriRichiesteModel filtri = new FiltriRichiesteModel();
            filtri.istituto = istitutoR != "" ? Convert.ToInt32(istitutoR) : (int?)null;
            filtri.intestazione = cognome + "_" + nome;
            filtri.ndg = ndg != "" ? Convert.ToInt64(ndg) : (long?)null;
            filtri.campoGenerico = codiceFiscale;
            ViewData["istitutoSelect"] = filtri.istituto;
            return PartialView("_GridElencoNdg", filtri);
        }

        public string getTelefonoCliente(int istituto, Int64 ndg)
        {
            Cliente clie;
            Societa soc;
            string telefono = "";

            string cellulare = "";

            WebServiceModel.WSAnagrafe.Rapporto rapporto = null; //ricerca per rapporto non implementata
            var resp = GenericHelpers.GetAnagraficaFromCache(istituto, ndg, rapporto);

            var clienteBase = resp.cliente;

            if (clienteBase.naturaGiuridica.naturaNominativo == "F" ||
                (
                  clienteBase.naturaGiuridica.naturaNominativo == "S"
                  && (clienteBase.naturaGiuridica.codiceGiuridico == "MM" ||
                      clienteBase.naturaGiuridica.codiceGiuridico == "FF")
                )
            )
            {
                clie = (Cliente)resp.cliente;
                //Tel
                var TELEFONO_UFF = clie.datiAggiuntivi.SingleOrDefault(a => a.tipo == 2065);
                var TELEFONO_ABITAZIONE = clie.datiAggiuntivi.SingleOrDefault(a => a.tipo == 3051);
                if (clie.telefono != "")
                {
                    telefono = clie.telefono;
                }
                else if (TELEFONO_UFF != null && TELEFONO_UFF.valore != "")
                {
                    telefono = TELEFONO_UFF.valore;
                }
                else if (TELEFONO_ABITAZIONE != null && TELEFONO_ABITAZIONE.valore != "")
                {
                    telefono = TELEFONO_ABITAZIONE.valore;
                }

                //cell
                var CELLULARE = clie.datiAggiuntivi.SingleOrDefault(a => a.tipo == 2050);
                var CELLULARE_UFF = clie.datiAggiuntivi.SingleOrDefault(a => a.tipo == 2063);
                if (CELLULARE != null && CELLULARE.valore != "")
                {
                    cellulare = CELLULARE.valore;
                }
                else if (CELLULARE_UFF != null && CELLULARE_UFF.valore != "")
                {
                    cellulare = CELLULARE_UFF.valore;
                }

            }
            else
            {
                soc = (Societa)resp.cliente;
                //Tel
                var TELEFONO_UFF = soc.datiAggiuntivi.SingleOrDefault(a => a.tipo == 2065);
                var TELEFONO_ABITAZIONE = soc.datiAggiuntivi.SingleOrDefault(a => a.tipo == 3051);
                if (soc.telefono != "")
                {
                    telefono = soc.telefono;
                }
                else if (TELEFONO_UFF != null && TELEFONO_UFF.valore != "")
                {
                    telefono = TELEFONO_UFF.valore;
                }
                else if (TELEFONO_ABITAZIONE != null && TELEFONO_ABITAZIONE.valore != "")
                {
                    telefono = TELEFONO_ABITAZIONE.valore;
                }

                //cell
                var CELLULARE = soc.datiAggiuntivi.SingleOrDefault(a => a.tipo == 2050);
                var CELLULARE_UFF = soc.datiAggiuntivi.SingleOrDefault(a => a.tipo == 2063);
                if (CELLULARE != null && CELLULARE.valore != "")
                {
                    cellulare = CELLULARE.valore;
                }
                else if (CELLULARE_UFF != null && CELLULARE_UFF.valore != "")
                {
                    cellulare = CELLULARE_UFF.valore;
                }
            }

            return String.IsNullOrEmpty(telefono) ? cellulare : telefono;
        }

        public JsonResult _GridElencoNdgBinding([DataSourceRequest] DataSourceRequest request,
          FiltriRichiesteModel filtri
        )
        {

            Cliente clie;
            Societa soc;
            List<GridElencoNDGModel> elencoNdg = new List<GridElencoNDGModel>();

            Int64 ndg;
            string intestazione;
            string natura_giuridica;
            string indirizzo;
            int? numRappDiretti;
            int? numRappIndiretti;
            DateTime? dataNascita;
            string codiceFiscale;
            string comune = "";
            string telefono = "";

            string cellulare = "";
            int codGestore = 0;
            try
            {


                if (filtri.ndg != null)
                {
                    //web service GetAnagrafica


                    WebServiceModel.WSAnagrafe.Rapporto rapporto = null; //ricerca per rapporto non implementata
                    var resp = GenericHelpers.GetAnagraficaFromCache(filtri.istituto.Value, (long)filtri.ndg, rapporto);

                    var clienteBase = resp.cliente;

                    if (clienteBase.naturaGiuridica.naturaNominativo == "F" ||
                       (
                       clienteBase.naturaGiuridica.naturaNominativo == "S"
                       && (clienteBase.naturaGiuridica.codiceGiuridico == "MM" || clienteBase.naturaGiuridica.codiceGiuridico == "FF")
                       )
                     )
                    {
                        clie = (Cliente)resp.cliente;
                        codGestore = clie.matricolaGestore;
                        ndg = clie.ndg;
                        intestazione = clie.cognome + " " + clie.nome;
                        natura_giuridica = clie.naturaGiuridica.naturaNominativo + clie.naturaGiuridica.codiceGiuridico;
                        if ((clie.indirizzoResidenza.localita ?? "") != "")
                        {
                            comune = clie.indirizzoResidenza.localita + " - " + clie.indirizzoResidenza.comune;
                        }
                        else
                        {
                            comune = clie.indirizzoResidenza.comune;
                        }
                        indirizzo = clie.indirizzoResidenza.tipoVia + " " + clie.indirizzoResidenza.nomeVia + " " +
                                    clie.indirizzoResidenza.numeroCivico + " - " + comune + " (" +
                                    clie.indirizzoResidenza.provincia + ")";
                        numRappDiretti = clie.numRapportiDiretti;
                        numRappIndiretti = clie.numRapportiIndiretti;
                        dataNascita = clie.dataNascita;
                        codiceFiscale = clie.codiceFiscale;



                    }
                    else
                    {
                        soc = (Societa)resp.cliente;
                        codGestore = soc.matricolaGestore;
                        ndg = soc.ndg;
                        intestazione = "";
                        foreach (var cointestatario in soc.cointestatari.ToList())
                        {
                            intestazione += " " + cointestatario.Replace('+', ' ');
                        }
                        natura_giuridica = soc.naturaGiuridica.naturaNominativo + soc.naturaGiuridica.codiceGiuridico;

                        if ((soc.sedeLegale.localita ?? "") != "")
                        {
                            comune = soc.sedeLegale.localita + " - " + soc.sedeLegale.comune;
                        }
                        else
                        {
                            comune = soc.sedeLegale.comune;
                        }
                        indirizzo = soc.sedeLegale.tipoVia + " " + soc.sedeLegale.nomeVia + ", " +
                                        soc.sedeLegale.numeroCivico + " - " + comune + " (" +
                                        soc.sedeLegale.provincia + ")";
                        //}
                        numRappDiretti = soc.numRapportiDiretti;
                        numRappIndiretti = soc.numRapportiIndiretti;
                        dataNascita = null;
                        codiceFiscale = soc.partitaIva;

                    }

                    elencoNdg.Add(new GridElencoNDGModel(ndg, intestazione, natura_giuridica, indirizzo,
                                                                     numRappDiretti,
                                                                     numRappIndiretti, dataNascita, codiceFiscale, codGestore.ToString()));

                }
                else if (filtri.intestazione.Split('_')[0] != "" || filtri.campoGenerico != "")
                {
                    RicercaNDGResponse resp;
                    //web service GetNDG
                    if (filtri.intestazione.Split('_')[0] != "")
                    {
                        resp = (new WSWrapperAnagrafe()).GetNDG(filtri.istituto.Value, filtri.intestazione.Split('_')[1], filtri.intestazione.Split('_')[0]);
                        codiceFiscale = "";
                    }
                    else
                    {
                        resp = (new WSWrapperAnagrafe()).GetNDGbyCF(filtri.istituto.Value, filtri.campoGenerico);
                        codiceFiscale = filtri.campoGenerico;
                    }

                    if (resp.listaClienti != null)
                    {


                        foreach (var sintesiAnag in resp.listaClienti.ToList())
                        {


                            if (sintesiAnag.anagrafe.naturaGiuridica.naturaNominativo == "F" ||
                        (
                        sintesiAnag.anagrafe.naturaGiuridica.naturaNominativo == "S"
                        && (sintesiAnag.anagrafe.naturaGiuridica.codiceGiuridico == "MM" || sintesiAnag.anagrafe.naturaGiuridica.codiceGiuridico == "FF")
                        )
                      )
                            {
                                clie = (Cliente)sintesiAnag.anagrafe;
                                ndg = clie.ndg;
                                codGestore = clie.matricolaGestore;
                                intestazione = "";
                                foreach (var intestatario in sintesiAnag.intestatari.ToList())
                                {
                                    intestazione += " " + intestatario;
                                }
                                natura_giuridica = clie.naturaGiuridica.naturaNominativo + clie.naturaGiuridica.codiceGiuridico;
                                if ((clie.indirizzoResidenza.localita ?? "") != "")
                                {
                                    comune = clie.indirizzoResidenza.localita + " - " + clie.indirizzoResidenza.comune;
                                }
                                else
                                {
                                    comune = clie.indirizzoResidenza.comune;
                                }
                                indirizzo = clie.indirizzoResidenza.tipoVia + " " + clie.indirizzoResidenza.nomeVia + ", " +
                                            clie.indirizzoResidenza.numeroCivico + " - " + comune + " (" +
                                            clie.indirizzoResidenza.provincia + ")";
                                numRappDiretti = clie.numRapportiDiretti;
                                numRappIndiretti = clie.numRapportiIndiretti;
                                dataNascita = clie.dataNascita;
                                //codiceFiscale = clie.codiceFiscale;

                            }
                            else
                            {
                                soc = (Societa)sintesiAnag.anagrafe;
                                ndg = soc.ndg;
                                codGestore = soc.matricolaGestore;
                                intestazione = "";
                                foreach (var intestatario in sintesiAnag.intestatari.ToList())
                                {
                                    intestazione += " " + intestatario.Replace('+', ' ');
                                }
                                natura_giuridica = soc.naturaGiuridica.naturaNominativo + soc.naturaGiuridica.codiceGiuridico;

                                if ((soc.sedeLegale.localita ?? "") != "")
                                {
                                    comune = soc.sedeLegale.localita + " - " + soc.sedeLegale.comune;
                                }
                                else
                                {
                                    comune = soc.sedeLegale.comune;
                                }
                                indirizzo = soc.sedeLegale.tipoVia + " " + soc.sedeLegale.nomeVia + ", " +
                                                soc.sedeLegale.numeroCivico + " - " + comune + " (" +
                                                soc.sedeLegale.provincia + ")";
                                //}
                                numRappDiretti = soc.numRapportiDiretti;
                                numRappIndiretti = soc.numRapportiIndiretti;
                                dataNascita = null;
                                //codiceFiscale = soc.partitaIva;


                            }


                            if (numRappDiretti != 0)
                            {
                                elencoNdg.Add(new GridElencoNDGModel(ndg, intestazione, natura_giuridica, indirizzo,
                                                                                     numRappDiretti,
                                                                                     numRappIndiretti, dataNascita, codiceFiscale, codGestore.ToString()));

                            }


                        }
                    }
                }



                return Json(elencoNdg.ToDataSourceResult(request));


            }
            catch (Exception ex)
            {
                string msg = "";
                if (ex.Message.Contains("NON TROVATI"))
                {
                    msg = "Ndg non trovato o inesistente.Riprovare";
                }
                else
                {
                    msg = "Errore WebService sulla ricerca effettuata.";
                }

                int i = elencoNdg.Count;
                if (i > 0)
                {
                    elencoNdg.RemoveRange(0, i);
                }

                //inserisco una riga con valori di default per visualizzare l'errore nella Grid col metodo OnDataBound()
                elencoNdg.Add(new GridElencoNDGModel(-1, msg, "", "", null, null, null, "", ""));
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(elencoNdg.ToDataSourceResult(request));
            }
        }

        [HttpPost]
        public ActionResult _getSceltaNdg(int istituto, Int64 ndg, string codfiscale, int? protocollo, string fromMail)
        {
            UserProfile myProfile = new UserProfileManager().GetCurrent();
            var matricolaOperatore = myProfile.UserInfo.Matricola;
            var myUocode = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");
            FiltriRichiesteModel filtri = new FiltriRichiesteModel();
            filtri.istituto = istituto;
            filtri.ndg = ndg;

            return null;

            //ViewData["IsModificabile"] = "N";
            //ViewData["IsModificabileAllegati"] = "N";
            //ViewData["IsModificabileFilEr"] = "N";
            //ViewData["IsModificabileAzioniRetroc"] = "N";
            //Proposte cnz;
            //if (protocollo.HasValue)
            //{
            //    cnz = richiestaBusiness.getProposta(protocollo.Value);
            //}
            //else
            //{
            //    cnz = richiestaBusiness.checkCNZNdg(istituto, ndg, codfiscale);
            //}


            //if (cnz != null)
            //{
            //    if (cnz.ID_Stato == 1 && (cnz.Matricola_Operatore_Filiale == matricolaOperatore || cnz.UOCode_Filiale_Operante == myUocode))
            //    {
            //        ViewData["IsModificabileAllegati"] = "S";
            //    }

            //    if (cnz.ID_Stato == 1
            //        || (cnz.ID_Stato == 2 && myProfile.Utenza.CodRuolo == "VRF")
            //        )
            //    {
            //        ViewData["IsModificabile"] = "S";

            //    }

            //    if (cnz.ID_Stato == 3)
            //    {

            //        Decimal valoreAzione = Convert.ToDecimal(ConfigHelpers.GetConfigValue("valoreAzione", System.Web.HttpContext.Current.Session[SessionKeys.CurrentCodApp] != null ? System.Web.HttpContext.Current.Session[SessionKeys.CurrentCodApp].ToString() : ""));
            //        Decimal valoreCap = Convert.ToDecimal(ConfigHelpers.GetConfigValue("valoreCAP", System.Web.HttpContext.Current.Session[SessionKeys.CurrentCodApp] != null ? System.Web.HttpContext.Current.Session[SessionKeys.CurrentCodApp].ToString() : ""));

            //        if (cnz.N_Azioni_Totali * valoreAzione > valoreCap)
            //        {
            //            cnz.Valore_Azioni_Conciliazione = valoreCap;
            //            cnz.N_Azioni_Conciliazione = Convert.ToInt32(valoreCap / valoreAzione);
            //        }
            //        else
            //        {
            //            cnz.Valore_Azioni_Conciliazione = cnz.N_Azioni_Totali * valoreAzione;
            //            cnz.N_Azioni_Conciliazione = cnz.N_Azioni_Totali;
            //        }
            //    }
            //}
            //else
            //{
            //    ViewData["IsModificabile"] = "S";
            //    ViewData["IsModificabileAllegati"] = "S";
            //    AnagraficaModel anagraficaNdg = GetAnagraficaCliente(istituto, ndg, codfiscale);
            //    Proposte proposta = new Proposte();

            //    proposta.ID_Stato = 1;
            //    proposta.Istituto = (short?)anagraficaNdg.istituto;
            //    proposta.NDG = anagraficaNdg.ndg;
            //    proposta.Data_inserimento = System.DateTime.Now;
            //    proposta.Intestazione = anagraficaNdg.intestazione;
            //    proposta.DataNascita = anagraficaNdg.dataNascita;
            //    proposta.LuogoNascita = !String.IsNullOrEmpty(anagraficaNdg.LuogoNascita) ? anagraficaNdg.LuogoNascita.Split('(')[0].Trim() : "";
            //    proposta.ProvinciaNascita = !String.IsNullOrEmpty(anagraficaNdg.LuogoNascita) ?
            //                                      anagraficaNdg.LuogoNascita.Split('(').Length > 2 ?
            //                                      anagraficaNdg.LuogoNascita.Split('(')[2].Trim().Replace(")", "")
            //                                      : anagraficaNdg.LuogoNascita.Split('(')[1].Trim().Replace(")", "")
            //                                : "";
            //    proposta.CittaResidenza = anagraficaNdg.ComuneResidenza.Split('_').Length > 1 ? anagraficaNdg.ComuneResidenza.Split('_')[1] : anagraficaNdg.ComuneResidenza.Split('_')[0]; ;
            //    proposta.IndirizzoResidenza = anagraficaNdg.indirizzoResidenza;
            //    string tel = getTelefonoCliente(anagraficaNdg.istituto, anagraficaNdg.ndg);
            //    proposta.Telefono = tel.Length > 12 ? tel.Substring(0, 12) : tel;
            //    proposta.CodiceFiscale = anagraficaNdg.codiceFiscale;
            //    proposta.CAPResidenza = anagraficaNdg.CAPResidenza;
            //    proposta.Classificazione_Giuridica = anagraficaNdg.natura_giuridica;
            //    proposta.Descrizione_Classificazione_Giuridica = richiestaBusiness.GetDescNatGiuridica(anagraficaNdg.natura_giuridica);
            //    proposta.ProvinciaResidenza = anagraficaNdg.ProvResidenza;
            //    proposta.Matricola_Operatore_Filiale = matricolaOperatore;
            //    proposta.UOCode_Filiale_Operante = myUocode;
            //    proposta.UOCode_Filiale_Erogante = myProfile.Utenza.CodRuolo == "SCS" || myProfile.Utenza.CodRuolo == "GSP" || myProfile.Utenza.CodRuolo == "GSC" ? "" : myUocode;
            //    int? codFiliale = null;
            //    if (myProfile.Utenza.CodRuolo != "SCS" && myProfile.Utenza.CodRuolo != "GSP" && myProfile.Utenza.CodRuolo != "GSC")
            //    {
            //        int? codfil = resoucesRepository.GetCodFilialiByUo(myUocode);
            //        if (codfil.HasValue)
            //        {
            //            codFiliale = codfil.Value;
            //        }
            //    }
            //    proposta.Codice_Filiale_Erogante = codFiliale;
            //    string luogo = "";
            //    if (myProfile.Utenza.CodRuolo == "SCS")
            //    {
            //        luogo = "UFF.CENTRALE";
            //    }
            //    else if (myProfile.Utenza.CodRuolo == "GSP")
            //    {
            //        luogo = "AREA PRIVATE";
            //    }
            //    else if (myProfile.Utenza.CodRuolo == "GSC")
            //    {
            //        luogo = "AREA CORPORATE";
            //    }
            //    else
            //    {
            //        luogo = richiestaBusiness.GetComuneFilialeRapporto(istituto, codFiliale.Value);
            //    }


            //    proposta.LuogoRichiesta = luogo;
            //    proposta.Protocollo_Anno = System.DateTime.Now.Year;
            //    proposta.Codice_Riscontro = Guid.NewGuid();
            //    richiestaBusiness.insertProposta(proposta);

            //    Proposte CNZnew = richiestaBusiness.checkCNZNdg(istituto, ndg, anagraficaNdg.codiceFiscale);
            //    cnz = CNZnew;

            //    Log_Azioni log = new Log_Azioni();
            //    log.Protocollo = cnz.Protocollo;
            //    log.Data_Azione = System.DateTime.Now;
            //    log.Matricola_Operatore = matricolaOperatore;
            //    log.ID_Stato = 1;
            //    log.Motivazione = "";
            //    log.Azione = "Inserita";
            //    richiestaBusiness.InsertLogAzioni(log);

            //}

            //if (cnz.ID_Stato == 1 && (myProfile.Utenza.CodRuolo == "SCS" || myProfile.Utenza.CodRuolo == "GSP" || myProfile.Utenza.CodRuolo == "GSC"))
            //{
            //    ViewData["IsModificabileFilEr"] = "S";
            //}
            //if (cnz.ID_Stato == 3 && myProfile.Utenza.CodRuolo == "DLB")
            //{
            //    ViewData["IsModificabileAzioniRetroc"] = "S";
            //}
            //ViewData["radioButton"] = richiestaBusiness.GetOpzioni();
            //ViewData["modelli"] = richiestaBusiness.GetModelli();
            //ViewData["tastiAzione"] = richiestaBusiness.GetTastiAzione(myProfile.Utenza.CodRuolo, cnz.ID_Stato.Value);
            //ViewData["masterFile"] = perimetro;

            //if (fromMail == "S")
            //{
            //    return View("_SceltaNdgFromMail", cnz);
            //}
            //else
            //{
            //    return View("_SceltaNdg", cnz);
            //}




        }

        public JsonResult _GetUnitaOrganizzativa(string text)
        {
            IQueryable<DimUO> listaUnita = commonBusiness.getUoByText(text).Where(a => a.FlagUfficioFiliale == "F" || a.FlagUfficioFiliale == "O");

            return Json(listaUnita);
        }

        public AnagraficaModel GetAnagraficaCliente(int istituto, Int64 ndg, string codfiscale)
        {
            Cliente clie;
            Societa soc;

            string intestazione = "";
            string natura_giuridica = "";
            string indirizzoResidenza = "";
            DateTime? dataNascita = null;
            string codiceFiscale = "";
            string LuogoNascita = "";
            string ComuneResidenza = "";
            string CAPResidenza = "";
            string ProvResidenza = "";


            try
            {

                if (!String.IsNullOrEmpty(codfiscale))
                {
                    WebServiceModel.WSAnagrafe.Rapporto rapporto = null; //ricerca per rapporto non implementata

                    var resp = GenericHelpers.GetAnagraficaFromCache(istituto, ndg, rapporto);
                    // var resp = (new WSWrapperAnagrafe()).GetAnagrafica(istituto, ndg, rapporto);

                    var clienteBase = resp.cliente;

                    if (clienteBase.naturaGiuridica.naturaNominativo == "F" ||
                        (
                          clienteBase.naturaGiuridica.naturaNominativo == "S"
                          && (clienteBase.naturaGiuridica.codiceGiuridico == "MM" ||
                              clienteBase.naturaGiuridica.codiceGiuridico == "FF")
                        )
                    )
                    {
                        clie = (Cliente)resp.cliente;
                        intestazione = clie.cognome + " " + clie.nome;
                        natura_giuridica = clie.naturaGiuridica.naturaNominativo + clie.naturaGiuridica.codiceGiuridico;
                        indirizzoResidenza = clie.indirizzoResidenza.tipoVia + " " + clie.indirizzoResidenza.nomeVia + ", " +
                                             clie.indirizzoResidenza.numeroCivico;

                        dataNascita = clie.dataNascita;
                        codiceFiscale = clie.codiceFiscale;
                        LuogoNascita = clie.luogoDiNascita.comune + " (" + clie.luogoDiNascita.provincia + ")";
                        ComuneResidenza = clie.indirizzoResidenza.localita + "_" + clie.indirizzoResidenza.comune;
                        var capRes = "00000" + clie.indirizzoResidenza.cap;
                        CAPResidenza = capRes.Substring(capRes.Length - 5);
                        ProvResidenza = clie.indirizzoResidenza.provincia;
                    }


                    else
                    {
                        soc = (Societa)resp.cliente;
                        intestazione = "";
                        foreach (var cointestatario in soc.cointestatari.ToList())
                        {
                            intestazione += " " + cointestatario;
                        }

                        natura_giuridica = soc.naturaGiuridica.naturaNominativo + soc.naturaGiuridica.codiceGiuridico;

                        indirizzoResidenza = soc.sedeLegale.tipoVia + " " + soc.sedeLegale.nomeVia + ", " +
                                             soc.sedeLegale.numeroCivico;
                        ComuneResidenza = soc.sedeLegale.localita + " " + soc.sedeLegale.comune;
                        var cap = "00000" + soc.sedeLegale.cap;
                        CAPResidenza = cap.Substring(cap.Length - 5);
                        ProvResidenza = soc.sedeLegale.provincia;

                        dataNascita = null;
                        codiceFiscale = soc.partitaIva;
                        LuogoNascita = "";



                    }

                }
                else
                {

                    RicercaNDGResponse resp;

                    resp = (new WSWrapperAnagrafe()).GetNDGbyCF(istituto, codfiscale);


                    if (resp.listaClienti != null)
                    {


                        foreach (var sintesiAnag in resp.listaClienti.ToList())
                        {


                            if (sintesiAnag.anagrafe.naturaGiuridica.naturaNominativo == "F" ||
                        (
                        sintesiAnag.anagrafe.naturaGiuridica.naturaNominativo == "S"
                        && (sintesiAnag.anagrafe.naturaGiuridica.codiceGiuridico == "MM" || sintesiAnag.anagrafe.naturaGiuridica.codiceGiuridico == "FF")
                        )
                      )
                            {
                                clie = (Cliente)sintesiAnag.anagrafe;
                                intestazione = clie.cognome + " " + clie.nome;
                                natura_giuridica = clie.naturaGiuridica.naturaNominativo + clie.naturaGiuridica.codiceGiuridico;
                                indirizzoResidenza = clie.indirizzoResidenza.tipoVia + " " + clie.indirizzoResidenza.nomeVia + ", " +
                                                     clie.indirizzoResidenza.numeroCivico;

                                dataNascita = clie.dataNascita;
                                codiceFiscale = clie.codiceFiscale;
                                LuogoNascita = clie.luogoDiNascita.comune + " (" + clie.luogoDiNascita.provincia + ")";
                                ComuneResidenza = clie.indirizzoResidenza.localita + "_" + clie.indirizzoResidenza.comune;
                                var capRes = "00000" + clie.indirizzoResidenza.cap;
                                CAPResidenza = capRes.Substring(capRes.Length - 5);
                                ProvResidenza = clie.indirizzoResidenza.provincia;

                            }
                            else
                            {
                                soc = (Societa)sintesiAnag.anagrafe;
                                intestazione = "";
                                foreach (var cointestatario in soc.cointestatari.ToList())
                                {
                                    intestazione += " " + cointestatario;
                                }

                                natura_giuridica = soc.naturaGiuridica.naturaNominativo + soc.naturaGiuridica.codiceGiuridico;

                                indirizzoResidenza = soc.sedeLegale.tipoVia + " " + soc.sedeLegale.nomeVia + ", " +
                                                     soc.sedeLegale.numeroCivico;
                                ComuneResidenza = soc.sedeLegale.localita + " " + soc.sedeLegale.comune;
                                var cap = "00000" + soc.sedeLegale.cap;
                                CAPResidenza = cap.Substring(cap.Length - 5);
                                ProvResidenza = soc.sedeLegale.provincia;

                                dataNascita = null;
                                codiceFiscale = soc.partitaIva;
                                LuogoNascita = "";

                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                //inserisco una riga con valori di default per visualizzare l'errore nella Grid col metodo OnDataBound()
                return new AnagraficaModel();
            }

            if (!String.IsNullOrEmpty(codfiscale))
            {
                string codiceFiscaleFormatted = "0000000000000000" + codiceFiscale;
                string cf = codiceFiscaleFormatted.Substring(codiceFiscaleFormatted.Length - 16);
                codiceFiscale = cf;
            }

            return new AnagraficaModel(istituto, ndg, intestazione, natura_giuridica, indirizzoResidenza, 0,
              0, dataNascita, codiceFiscale, true, LuogoNascita, ComuneResidenza,
              CAPResidenza, ProvResidenza, 0, "", "", "", "");
        }

        [HttpPost]
        public string DescFilialeByUocode(string uocode)
        {

            string uoForm = "000000" + uocode;
            string uoSubstring = uoForm.Substring(uoForm.Length - 6);

            string desc = "";
            var result = resoucesRepository.GetDimUoByUocode(uoSubstring).FirstOrDefault();
            if (result != null)
            {
                desc = result.DescrizioneUObyIstituto;
            }

            return desc;
        }

        [HttpPost]
        public string getCfNdg(int istituto, Int64 ndg)
        {
            Cliente clie;
            Societa soc;

            string codiceFiscale;

            WebServiceModel.WSAnagrafe.Rapporto rapporto = null; //ricerca per rapporto non implementata

            var resp = GenericHelpers.GetAnagraficaFromCache(istituto, ndg, rapporto);
            // var resp = (new WSWrapperAnagrafe()).GetAnagrafica(istituto, ndg, rapporto);

            var clienteBase = resp.cliente;

            if (clienteBase.naturaGiuridica.naturaNominativo == "F" ||
                (
                  clienteBase.naturaGiuridica.naturaNominativo == "S"
                  && (clienteBase.naturaGiuridica.codiceGiuridico == "MM" ||
                      clienteBase.naturaGiuridica.codiceGiuridico == "FF")
                )
            )
            {
                clie = (Cliente)resp.cliente;
                codiceFiscale = clie.codiceFiscale;
            }
            else
            {
                soc = (Societa)resp.cliente;
                codiceFiscale = soc.partitaIva;
            }

            return codiceFiscale;
        }

        public void DeleteAsyncFolder(String path)
        {
            try
            {
                if (System.IO.Directory.Exists(path))
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() => { System.IO.Directory.Delete(path, true); });
                }

            }
            catch (Exception e)
            {

            }

        }

        public void GetCliente(Richieste richieste, int istituto, long ndg, string codfiscale = "")
        {
            var cliente = GetAnagraficaCliente(istituto, ndg, codfiscale);

            if (!String.IsNullOrEmpty(codfiscale))
            {
                string codiceFiscaleFormatted = "0000000000000000" + cliente.codiceFiscale;
                string cf = codiceFiscaleFormatted.Substring(codiceFiscaleFormatted.Length - 16);
                cliente.codiceFiscale = cf;
            }
            richieste.Istituto = istituto;
            richieste.NDG = cliente.ndg;
            richieste.IntestazioneNDG = cliente.intestazione;
            richieste.DataNascita = cliente.dataNascita;
            richieste.LuogoNascita = cliente.LuogoNascita;
            richieste.NaturaGiuridica = cliente.natura_giuridica;
            ///////////////
            /////////////
            //////////// mi puoi controllare se con la seguente condizione copro le possibilità
            /////////////

            if (cliente.natura_giuridica.Substring(0, 1) == "F" ||
            (
              cliente.natura_giuridica.Substring(0, 1) == "S"
              && (cliente.natura_giuridica.Substring(1, 2) == "MM" ||
                  cliente.natura_giuridica.Substring(1, 2) == "FF")
            ))
            {
                richieste.isPerson = true;   ////persona fisica
            }
            else
            {
                richieste.isPerson = false;    ////società
            }
            ////////////  fine controllo



            if (richieste.isPerson)
            {
                /////// sono cliente
                richieste.CodiceFiscale = cliente.codiceFiscale;
                richieste.Indirizzo = cliente.indirizzoResidenza + " " + cliente.ComuneResidenza + " " + cliente.CAPResidenza + " " + cliente.ProvResidenza;

            }
            else
            {
                //////// sonbo società
                richieste.PIva = cliente.codiceFiscale;
                richieste.SedeLegale = cliente.indirizzoResidenza + " " + cliente.ComuneResidenza + " " + cliente.CAPResidenza + " " + cliente.ProvResidenza;
            }
        }

    }
}
