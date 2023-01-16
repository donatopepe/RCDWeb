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

namespace RCDWeb.Areas.RCD.Controllers
{
    public partial class RichiestaController : Controller
    {
        private ResourcesRepository resourcesRepository;

        private RCDContext db = new RCDContext();
        private RCDRepository repository;
        private CommonBusiness commonBusiness = new CommonBusiness();
        private int MioIstituto;
        private string MioUocode;
        private string MiaMatricola;
        private string MiaFiliale;

        public RichiestaController()
        {
            var pippo = db.Database.CreateIfNotExists();
            if (pippo)
            {
                new InitialCreate().On(db);
            }
            this.resourcesRepository = new ResourcesRepository(new ResourcesContext(), new UserProfileManager().GetCurrent().Utenza.CodApp);
            repository = new RCDRepository(db);

            UserProfile myProfile = new UserProfileManager().GetCurrent();
            var myUocode = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");
            string flagUffRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "flagUffRuolo"); //////non credo serva
            string filtroIst = UserProfileManager.GetFiltroSicurezza(myProfile, "IST");
            int? istID = !String.IsNullOrEmpty(filtroIst) ? Convert.ToInt32(filtroIst) : Convert.ToInt32(myProfile.UserInfo.CodIstituto);
            var matricola = myProfile.UserInfo.Matricola;

            

            MiaMatricola = "131132";        ///////////////////////simulato
            MioUocode = "2424"; ////////////////////////simulato
            MioIstituto = 46; ////////////////////////simuulato

            MiaFiliale = "Bari";


        }

        public RichiestaController(ResourcesRepository resourcesRepository)
        {
            var pippo = db.Database.CreateIfNotExists();
            if (pippo)
            {
                new InitialCreate().On(db);
            }
            this.resourcesRepository = resourcesRepository;
            repository = new RCDRepository(db);
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
            //IQueryable<DimUO> filiali = commonBusiness.getUoByText(prefix).Where(a => a.FlagUfficioFiliale == "F" || a.FlagUfficioFiliale == "O");
            var filiali = new List<DimUO> {
                new DimUO { DescrizioneUObyIstituto="Bari", UOCode="435678"},
                new DimUO { DescrizioneUObyIstituto="Taranto", UOCode="55533"},
                new DimUO { DescrizioneUObyIstituto="Foggia", UOCode="3534553"},
                new DimUO { DescrizioneUObyIstituto="Brindisi", UOCode="54333"},
                new DimUO { DescrizioneUObyIstituto="Roma", UOCode="6666666"},
                new DimUO { DescrizioneUObyIstituto="Milano", UOCode="4444444"},
            };
            return Json(filiali, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            //FiltriRichiesteModel filtri = new FiltriRichiesteModel();

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
            ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome");

            //string filtroIst = UserProfileManager.GetFiltroSicurezza(myProfile, "IST");
            //int? istID = !String.IsNullOrEmpty(filtroIst)
            //  ? Convert.ToInt32(filtroIst)
            //  : Convert.ToInt32(myProfile.UserInfo.CodIstituto);


            //IQueryable listAree;
            //if (UserProfileManager.ContainsSottoFunzione(myProfile.RuoliSicurezza, "S01"))
            //{
            //    istID = Convert.ToInt32(istID);
            //    ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome", istID);
            //}
            //else
            //{
            //    istID = Convert.ToInt32(istID);
            //    ViewData["SelIstituti"] = new SelectList(selectListIst, "codice", "nome", istID);

            //}

            ViewData["istituto"] = MioIstituto; //istID;
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



            //ViewBag.MioUocode = MioUocode;
            //ViewBag.MiaMatricola = MiaMatricola;
            //filtri.filtroUocode = MioUocode;

            //return View(new AreaCommonWeb.Models.CampiRicerca());
            return View("Index");
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
            filtri.myUocode = MioUocode;            //////simulato
            filtri.flagUffFil = FlagUff;
            filtri.campoGenerico = propBlocPost;


            if (filtri.istituto == null) filtri.istituto = MioIstituto;
            ViewData["istituto"] = filtri.istituto; // da valutare
            ViewData["uocode"] = filtri.myUocode; // da valutare

            return PartialView("IndexJqueryTable", repository.GetRichieste(filtri));

        }
        public void GetCliente(Richieste richieste, int istituto, long ndg, string codfiscale = "")
        {
            richieste.isPerson = true;
            richieste.Istituto = 46;
            richieste.NDG = 3456789;
            richieste.DataNascita = DateTime.Now;
            richieste.LuogoNascita = "Taranto";
            richieste.Indirizzo = "Pippo";
            richieste.IntestazioneNDG = "Paperino";
            if (richieste.isPerson)
            {
                /////// sono cliente
                richieste.CodiceFiscale = "PPEDNT77B24L049L";
            }
            else
            {
                //////// sonbo società
                richieste.PIva = "345678909876543";
                richieste.SedeLegale = "Pluto";
            }
        }

    }
}
