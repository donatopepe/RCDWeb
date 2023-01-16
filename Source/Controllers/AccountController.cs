using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using AreaCommonWeb.Consts;
using Kendo.Mvc.UI;
using AreaCommonWeb.Business;
using AreaCommonWeb.Controllers;
using AreaCommonWeb.Models;
using AreaCommonWeb.ModelView;
using Kendo.Mvc.Extensions;
using AreaCommonWeb.ModelView.Security;
using RCDWeb.Areas.RCD;
using RCDWeb.Business;
using WebServiceModel.WSSicurezza;
using DAL.AreaCommonWeb;
using AreaCommonWeb.DAL;

namespace RCDWeb.Controllers
{
  public class AccountController : BaseController
  {
    //private string CodApp = "GRA";
    //
    // GET: /Account/LogOn
    
    private ResourcesRepository resourcesRepository;
    private AccountBusiness accountBusiness = new AccountBusiness();
    private CommonBusiness commonBusiness = new CommonBusiness();

    public AccountController()
    {
     
      this.resourcesRepository = new ResourcesRepository(new ResourcesContext());

    }

    public AccountController( ResourcesRepository resourcesRepository)
    {
     
      this.resourcesRepository = resourcesRepository;

    }

    public ActionResult SelectedLogOn(string IdUtenza)
    {
      try
      {
        string username = User.Identity.Name;
        username = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["UtenzaTest"].ToString()) ? ConfigurationManager.AppSettings["UtenzaTest"].ToString() : username;

        WebServiceModel.WSSicurezza.Service wsSicurezza = new WebServiceModel.WSSicurezza.Service();
        GenericHelpers.SetWebServiceURL(wsSicurezza);

        var Utenza = wsSicurezza.GetUtenzaById(Int32.Parse(IdUtenza));
        CreaCookieUtenza(Utenza);

        WebServiceModel.WSAcmeProfiles.Service wsAcmeProfiles = new WebServiceModel.WSAcmeProfiles.Service();
        GenericHelpers.SetWebServiceURL(wsAcmeProfiles);

        WebServiceModel.WSAcmeProfiles.UserInfo Utente = wsAcmeProfiles.GetUserInfo(username);
        CreaCookieUtente(Utente);

        var funzRuolo = wsSicurezza.GetFunzSottoFunz(Utenza.CodRuolo, this.CodApp).ToList();
        CreaCookieRuoliSicurezza(funzRuolo);

        var filtriSic = wsSicurezza.GetFiltri(Utenza.IdUtenza).ToList();
        CreaCookieFiltriSicurezza(filtriSic);

        return RedirectToAction("Index", "Home", new { area = "RCD" });

      }
      catch (Exception ex)
      {
        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        string localpath = HostingEnvironment.MapPath("~/");
        string dirTemplate = ConfigHelpers.GetConfigValue("docFilesAPP", Session[SessionKeys.CurrentCodApp] != null ? Session[SessionKeys.CurrentCodApp].ToString() : "");
        string path = localpath + dirTemplate;
        string exeption = "exeption:" + ex.Message;
        if (ex.InnerException != null)
        {
          exeption += " Inner: " + ex.InnerException.Message;
        }
        System.IO.File.WriteAllText(path + "exeption.txt", exeption);
        return RedirectToAction("VisualizzaErrore", "Home", new { area="Common",ex_msg = ex.Message });
      }

    }



    public JsonResult _GridUtenze([DataSourceRequest] DataSourceRequest request, string codApp, string matricola)
    {
      try
      {
        WebServiceModel.WSSicurezza.Service wsSicurezza = new WebServiceModel.WSSicurezza.Service();
        GenericHelpers.SetWebServiceURL(wsSicurezza);

        var listUtenze = wsSicurezza.GetUtenze(codApp, matricola);
        return Json(listUtenze.ToDataSourceResult(request));
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

   
    public ActionResult LogOnWithUtenzaTest()
    {
      return View("LogOnWithUtenzaTest");
    }

    public ActionResult setUtenzaTest(string UtenzaTest)
    {
      ConfigurationManager.AppSettings["UtenzaTest"] = UtenzaTest;
      return RedirectToAction("LogOn");
    }

    [HttpPost]
    public ActionResult LogOn(string istituto, string matricola, string check)
    {
      string localpath = HostingEnvironment.MapPath("~/");
      string dirTemplate = "Doc_Files/";
      string path = localpath + dirTemplate;
      try
      {
        string username = "";
        //username = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["UtenzaTest"].ToString()) ? ConfigurationManager.AppSettings["UtenzaTest"].ToString() : User.Identity.Name;

        // implementazione SSO
        string strDaCifrare = string.Format("Matricola={0}&Istituto={1}", matricola, istituto);
        string encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"].ToString();
        string strCifrata = GenericHelpers.GetHashString(strDaCifrare + encryptionKey);
        //Controllo i token
        if (strCifrata != check)
        {
          return RedirectToAction("VisualizzaErrore", "Base", new { area = "Common", ex_msg = "Utente non autorizzato!" });
        }

        if (istituto == "047")
        {
          username = "BB" + matricola;
        }
        else
        {
          username = "KO" + matricola;
        }
        WebServiceModel.WSAcmeProfiles.UserInfo Utente = new WebServiceModel.WSAcmeProfiles.UserInfo();
        WebServiceModel.WSAcmeProfiles.Service wsAcmeProfiles = new WebServiceModel.WSAcmeProfiles.Service();
        GenericHelpers.SetWebServiceURL(wsAcmeProfiles);
        Utente = wsAcmeProfiles.GetUserInfo(username);

        var matricolaUtente = Utente.Matricola;
        if (matricolaUtente != "")
        {
          CreaCookieUtente(Utente);
          WebServiceModel.WSSicurezza.Service wsSicurezza = new WebServiceModel.WSSicurezza.Service();
          GenericHelpers.SetWebServiceURL(wsSicurezza);
          var listUtenze = wsSicurezza.GetUtenze(this.CodApp, matricolaUtente).ToList();


          if (listUtenze.Count == 1)
          {
            CreaCookieUtenza(listUtenze.FirstOrDefault());
            var funzRuolo = wsSicurezza.GetFunzSottoFunz(listUtenze.FirstOrDefault().CodRuolo, this.CodApp).ToList();
            CreaCookieRuoliSicurezza(funzRuolo);

            var filtriSic = wsSicurezza.GetFiltri(listUtenze.FirstOrDefault().IdUtenza).ToList();
            CreaCookieFiltriSicurezza(filtriSic);

            return RedirectToAction("Index", "Home", new { area = "RCD" });
          }
          else if (listUtenze.Count > 1)
          {
            ViewBag.codApp = this.CodApp;
            ViewBag.matricola = matricolaUtente;

            return View(listUtenze);
          }
          else if (listUtenze.Count == 0 && Utente.Mansione != null)
          {
            Dictionary<string, string> mappingRuoloMansione = accountBusiness.GetMappingRuoloMansione(Utente.CodIstituto, Utente.CodMansione,
                                                        Utente.FlagUfficioFiliale, this.CodApp);


            if (mappingRuoloMansione.Count != 0)
            {
              var nomeRuolo = mappingRuoloMansione.Single().Value;
              var codRuolo = mappingRuoloMansione.Single().Key;
              var flagResp = Utente.FlagResp;

              if (codRuolo == "ARR")
              {
                if (Utente.DescUnit.Contains("Corporate")
                  ||
                  Utente.DescUnit.Contains("Imprese")
                  )
                {
                  codRuolo = "ARC";
                  nomeRuolo = nomeRuolo.Replace(nomeRuolo.Split('-')[0], codRuolo);
                }
              }
              //string mapping1 = "Mansione: " + Utente.Mansione + " nomeRuolo: " + nomeRuolo +
              //                  " codRuolo: " + codRuolo;
              //System.IO.File.WriteAllText(path + "logmapping.txt", mapping1);

              WebServiceModel.WSSicurezza.Utenza ut = new WebServiceModel.WSSicurezza.Utenza();
              ut.IdUtenza = -1;
              ut.NomeRuolo = nomeRuolo;
              ut.CodRuolo = codRuolo;
              ut.FlagResp = flagResp;

              CreaCookieUtenza(ut);

              var funzRuolo = wsSicurezza.GetFunzSottoFunz(codRuolo, this.CodApp).ToList();
              CreaCookieRuoliSicurezza(funzRuolo);

              List<FiltroSicurezza> filtriSic = accountBusiness.GetFiltriSicForUtnOrg(Utente, codRuolo, Utente.FlagUfficioFiliale);

              //var filtriSic = WSSicurezza.GetFiltri(listUtenze.FirstOrDefault().IdUtenza).ToList();
              CreaCookieFiltriSicurezza(filtriSic);

              return RedirectToAction("Index", "Home", new { area = "RCD" });
            }
            else
            {
              return RedirectToAction("VisualizzaErrore", "Home", new { area = "Common", ex_msg = "Utente non autorizzato!" });
            }
          }
          else
          {
            return RedirectToAction("VisualizzaErrore", "Home", new { area = "Common", ex_msg = "Non si dispone di un account di accesso!" });
          }


        }
        else
        {
          return RedirectToAction("VisualizzaErrore", "Home", new { area = "Common", ex_msg = "Utente non abilitato!" });
        }

      }
      catch (Exception ex)
      {

        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        return RedirectToAction("VisualizzaErrore", "Home", new { area = "Common", ex_msg = ex.Message });
      }


    }


    public ActionResult LogOn()
    {
      string localpath = HostingEnvironment.MapPath("~/");
      string dirTemplate = ConfigHelpers.GetConfigValue("docFilesAPP", Session[SessionKeys.CurrentCodApp] != null ? Session[SessionKeys.CurrentCodApp].ToString() : "");
      string path = localpath + dirTemplate;
      try
      {
        string username = "";
        username = User.Identity.Name;
        WebServiceModel.WSAcmeProfiles.UserInfo Utente = new WebServiceModel.WSAcmeProfiles.UserInfo();
        username = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["UtenzaTest"].ToString()) ? ConfigurationManager.AppSettings["UtenzaTest"].ToString() : username;
        WebServiceModel.WSAcmeProfiles.Service wsAcmeProfiles = new WebServiceModel.WSAcmeProfiles.Service();
        GenericHelpers.SetWebServiceURL(wsAcmeProfiles);
        Utente = wsAcmeProfiles.GetUserInfo(username);

        var matricola = Utente.Matricola;
        if (matricola != "")
        {
          CreaCookieUtente(Utente);
          WebServiceModel.WSSicurezza.Service wsSicurezza = new WebServiceModel.WSSicurezza.Service();
          GenericHelpers.SetWebServiceURL(wsSicurezza);
          var listUtenze = wsSicurezza.GetUtenze(this.CodApp, matricola).ToList();
     

          if (listUtenze.Count == 1)
          {
            CreaCookieUtenza(listUtenze.FirstOrDefault());
            var funzRuolo = wsSicurezza.GetFunzSottoFunz(listUtenze.FirstOrDefault().CodRuolo, this.CodApp).ToList();
            CreaCookieRuoliSicurezza(funzRuolo);

            var filtriSic = wsSicurezza.GetFiltri(listUtenze.FirstOrDefault().IdUtenza).ToList();
            CreaCookieFiltriSicurezza(filtriSic);

            return RedirectToAction("Index", "Home", new { area = "RCD" });
          }
          else if (listUtenze.Count > 1)
          {
            ViewBag.codApp = this.CodApp;
            ViewBag.matricola = matricola;

            return View(listUtenze);
          }
          else if (listUtenze.Count == 0 && Utente.Mansione != null)
          {
            Dictionary<string, string> mappingRuoloMansione = accountBusiness.GetMappingRuoloMansione(Utente.CodIstituto, Utente.CodMansione,
                                                        Utente.FlagUfficioFiliale, this.CodApp);
            

            if (mappingRuoloMansione.Count != 0)
            {
              var nomeRuolo = mappingRuoloMansione.Single().Value;
              var codRuolo = mappingRuoloMansione.Single().Key;
              var flagResp = Utente.FlagResp;

              if (codRuolo == "ARR")
              {
                  if (Utente.DescUnit.Contains("Corporate")
                    ||
                    Utente.DescUnit.Contains("Imprese")
                    )
                  {
                      codRuolo = "ARC";
                      nomeRuolo = nomeRuolo.Replace(nomeRuolo.Split('-')[0], codRuolo);
                  }
              }
              //string mapping1 = "Mansione: " + Utente.Mansione + " nomeRuolo: " + nomeRuolo +
              //                  " codRuolo: " + codRuolo;
              //System.IO.File.WriteAllText(path + "logmapping.txt", mapping1);

              WebServiceModel.WSSicurezza.Utenza ut = new WebServiceModel.WSSicurezza.Utenza();
              ut.IdUtenza = -1;
              ut.NomeRuolo = nomeRuolo;
              ut.CodRuolo = codRuolo;
              ut.FlagResp = flagResp;

              CreaCookieUtenza(ut);

              var funzRuolo = wsSicurezza.GetFunzSottoFunz(codRuolo, this.CodApp).ToList();
              CreaCookieRuoliSicurezza(funzRuolo);

              List<FiltroSicurezza> filtriSic = accountBusiness.GetFiltriSicForUtnOrg(Utente, codRuolo, Utente.FlagUfficioFiliale);

              //var filtriSic = WSSicurezza.GetFiltri(listUtenze.FirstOrDefault().IdUtenza).ToList();
              CreaCookieFiltriSicurezza(filtriSic);

              return RedirectToAction("Index", "Home", new { area = "RCD" });
            }
            else
            {
              return RedirectToAction("VisualizzaErrore", "Home", new {area="Common" , ex_msg = "Utente non autorizzato!" });
            }
          }
          else
          {
            return RedirectToAction("VisualizzaErrore", "Home", new {area="Common", ex_msg = "Non si dispone di un account di accesso!" });
          }


        }
        else
        {
          return RedirectToAction("VisualizzaErrore", "Home", new {area="Common", ex_msg = "Utente non abilitato!" });
        }

      }
      catch (Exception ex)
      {

        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        return RedirectToAction("VisualizzaErrore", "Home", new { area="Common", ex_msg = ex.Message });
      }


    }




    public void CreaCookieUtenza(WebServiceModel.WSSicurezza.Utenza ut)
    {
      UserProfile myProfile = new UserProfileManager().GetCurrent();
      myProfile.Utenza.IdUtenza = ut.IdUtenza;
      myProfile.Utenza.CodRuolo = ut.CodRuolo;
      myProfile.Utenza.NomeRuolo = ut.NomeRuolo;
      myProfile.Utenza.FlagResp = ut.FlagResp == "Y" ? true : false;
    }

    public void CreaCookieRuoliSicurezza(List<FunzSottoFunz> list_funz)
    {
      UserProfile myProfile = new UserProfileManager().GetCurrent();
      foreach (var funzioneRuolo in list_funz)
      {
        if (!myProfile.RuoliSicurezza.ContainsKey(funzioneRuolo.funz))
        {
          myProfile.RuoliSicurezza.Add(funzioneRuolo.funz, new List<string>());
        }
        myProfile.RuoliSicurezza[funzioneRuolo.funz].Add(funzioneRuolo.sottoFunz);
      }
    }

    public void CreaCookieFiltriSicurezza(List<FiltroSicurezza> list_filtri)
    {
      UserProfile myProfile = new UserProfileManager().GetCurrent();
      foreach (var filtroSicurezza in list_filtri)
      {
        if (!myProfile.FiltriSicurezza.ContainsKey(filtroSicurezza.CodFiltro))
        myProfile.FiltriSicurezza.Add(filtroSicurezza.CodFiltro, filtroSicurezza.ValoreFiltro);        
      }
      ProfileAugmentation.Doit();
    }

    public void CreaCookieUtente(WebServiceModel.WSAcmeProfiles.UserInfo utente)
    {

      UserProfile myProfile = new UserProfileManager().GetCurrent();
      //GenericHelpers.RemoveCookie(cookieName, this.ControllerContext.HttpContext);
      myProfile.UserInfo.Nome = utente.Nome;
      myProfile.UserInfo.Matricola = utente.Matricola;
      myProfile.UserInfo.CodMansione = utente.CodMansione;
      myProfile.UserInfo.Mansione = utente.Mansione;
      myProfile.UserInfo.LastName = utente.LastName;
      myProfile.UserInfo.FirstName = utente.FirstName;
      myProfile.UserInfo.MatrNumber = utente.MatrNumber;
      myProfile.UserInfo.DataCessazione = utente.DataCessazione;
      myProfile.UserInfo.CodIstituto = utente.CodIstituto;
      myProfile.UserInfo.Email = utente.Email;
      myProfile.UserInfo.FlagIncaricoPrincipale = utente.FlagIncaricoPrincipale;
      myProfile.UserInfo.FlagResp = utente.FlagResp;
      myProfile.UserInfo.NDG = utente.NDG;
      myProfile.UserInfo.CodUnit = utente.CodUnit;
      myProfile.UserInfo.DescUnit = utente.DescUnit;
      myProfile.UserInfo.EmailUnit = utente.EmailUnit;
      myProfile.UserInfo.FlagUfficioFiliale = utente.FlagUfficioFiliale;
      myProfile.UserInfo.CodParentUnit = utente.CodParentUnit;
      myProfile.UserInfo.DescParentUnit = utente.DescParentUnit;
      myProfile.UserInfo.TelefonoFisso = utente.TelefonoFisso;
      myProfile.UserInfo.TelefonoMobile = utente.TelefonoMobile;
      myProfile.UserInfo.TelefonoFissoPersonale = utente.TelefonoFissoPersonale;
      myProfile.UserInfo.TelefonoMobilePersonale = utente.TelefonoMobilePersonale;
      myProfile.UserInfo.AbbreviazioneCellulare = utente.AbbreviazioneCellulare;
      myProfile.UserInfo.Fax = utente.Fax;
      myProfile.UserInfo.codGestore = utente.codGestore;
    }

    //
    // GET: /Account/LogOff

    public ActionResult LogOff()
    {
      UserProfileManager.RemoveCurrent();
      return View();
    }

  }
}
