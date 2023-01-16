using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using AreaCommonWeb.Business;
using AreaCommonWeb.Consts;
using AreaCommonWeb.ModelView;
using AreaCommonWeb.ModelView.Security;
using DAL.AreaCommonWeb;
using AreaCommonWeb.DAL;
using RCDWeb.Business;


namespace RCDWeb.Areas.RCD
{
  public static class ProfileAugmentation
  {
    public static void Doit()
    {
      // filtri sicurezza

      AccountBusiness accountBusiness = new AccountBusiness();
       CommonBusiness commonBusiness = new CommonBusiness();
     
;      ResourcesRepository resourcesRepository = new ResourcesRepository(new ResourcesContext());
      
      //RichiesteRepository richiesteRepository = new RichiesteRepository(new RichiestaContext());
      UserProfile myProfile = new UserProfileManager().GetCurrent();
      // all'interno del portale alcuni ruoli non devono avere filtro di sicurezza basato sull'istituto
      if (ConfigurationManager.AppSettings["PortalMode"] == "1"
        && (myProfile.Utenza.CodRuolo == "ADM" || myProfile.Utenza.CodRuolo == "HDM" || myProfile.Utenza.CodRuolo == "GRP"))
      {
        UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "IST", null);
        UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "FIL", null);
        UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "ARE", null);
        UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "SEG", null);
      }
      string sfiltroIst = "";
      myProfile.FiltriSicurezza.TryGetValue("IST", out sfiltroIst);
      if (sfiltroIst == null)
      {
        sfiltroIst = string.Empty;
      }

      string sfiltroFil = "";
      myProfile.FiltriSicurezza.TryGetValue("FIL", out sfiltroFil);
      if (sfiltroFil == null)
      {
        sfiltroFil = string.Empty;
      }

      string sfiltroArea = "";
      myProfile.FiltriSicurezza.TryGetValue("ARE", out sfiltroArea);
      if (sfiltroArea == null)
      {
        sfiltroArea = string.Empty;
      }

      string sfiltroSeg = "";
      myProfile.FiltriSicurezza.TryGetValue("SEG", out sfiltroSeg);
      if (sfiltroSeg == null)
      {
        sfiltroSeg = string.Empty;
      }

      string uocode = myProfile.UserInfo.CodUnit;
      string FlagUfficioFiliale = myProfile.UserInfo.FlagUfficioFiliale;
      string FlagResp = myProfile.UserInfo.FlagResp;
      string codRuolo = myProfile.Utenza.CodRuolo;
      string FlagRespUtenza = myProfile.Utenza.FlagResp ? "Y" : "N";
      string descUnit = myProfile.UserInfo.DescUnit;
      var matricola_utente = myProfile.UserInfo.Matricola;

      var codUnitRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "codUnitRuolo");
      var flagUffRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "flagUffRuolo");
      var respRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "respRuolo");
      var descUnitRuolo = UserProfileManager.GetFiltroSicurezza(myProfile, "descUnitRuolo");

      if (!string.IsNullOrEmpty(sfiltroFil))
      {
        //calcolo il codUnit e flag uffi dell'utente FILIALE loggato(non da organigramma)
        codUnitRuolo = accountBusiness.GetUOFiliale(Convert.ToInt32(sfiltroFil));
        flagUffRuolo = accountBusiness.GetFlagUff(codUnitRuolo);
        descUnitRuolo = accountBusiness.GetDescUnit(codUnitRuolo);
      }
      else if (!string.IsNullOrEmpty(sfiltroArea))
      {
        //calcolo il codUnit e flag uffi dell'utente AREA loggato(non da organigramma)
        codUnitRuolo = accountBusiness.GetCodUoCodeArea(Convert.ToInt32(sfiltroIst),Convert.ToInt32(sfiltroArea), sfiltroSeg);
        flagUffRuolo = accountBusiness.GetFlagUffArea(codUnitRuolo);
        descUnitRuolo = accountBusiness.GetDescUnitArea(codUnitRuolo);
      }
      else if (!string.IsNullOrEmpty(codRuolo) && codRuolo.Equals("RSC"))
      {
        //calcolo il codUnit e flag uffi del Supporto Commerciale loggato(non da organigramma)
        //recupero i dati dalla tabella ciclo approvatori
        //var approvatore = paybackRepository.GetApprovatoreByCodRuolo(codRuolo);
        //codUnitRuolo = approvatore.Codice;
        //flagUffRuolo = approvatore.FlagUff;
        //descUnitRuolo = resourcesRepository.GetUOCodeDesc(codUnitRuolo);
      }
      else
      {
        //Verifico che il parent sia di tipo area(PRIVATE)
        var uoParent = resourcesRepository.GetUOCodeParent(uocode,sfiltroIst);
        string descUnitArea = accountBusiness.GetDescUnitArea(uoParent);
        string flagUffParent = accountBusiness.GetFlagUffArea(uoParent);

        if (flagUffParent == "A" && (FlagUfficioFiliale != "F" && FlagUfficioFiliale != "H") && (myProfile.Utenza.CodRuolo == "ARE" || myProfile.Utenza.CodRuolo == "GSP"))
        {
          //sostituisco il codUnit e flag uffi dell'utente loggato(da organigramma) come profilo AREA
          codUnitRuolo = uoParent;
          flagUffRuolo = flagUffParent;
          descUnitRuolo = descUnitArea;
          UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "ARE", resourcesRepository.GetCodAreaByUoArea(codUnitRuolo).ToString());
        }
        //else
        //{
        //  //calcolo il codUnit e flag uffi dell'utente loggato(da organigramma)
        //  codUnitRuolo = uocode;
        //  flagUffRuolo = FlagUfficioFiliale;
        //  descUnitRuolo = descUnit;
        //}

      }

      respRuolo = codRuolo != "" ? FlagRespUtenza : FlagResp;

      UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "codUnitRuolo", codUnitRuolo);
      UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "flagUffRuolo", flagUffRuolo);
      UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "respRuolo", respRuolo);
      UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "descUnitRuolo", descUnitRuolo);
      
      // Informazioni relative al mercato,.
      // il campo 'visibilita' viene utilizzato nei filtri per la comparsa o meno dei checkbox in view 
      // il campo 'flagConfrontoGestoreNDG' viene utilizzato per determinare se è necessario confrontare
      // il codice del gestore piuttosto che la filiale nella selezionedei rapporti linkabili dell'NDG
      //LoginGestoreUtenteModel flussoGestore = richiesteRepository.GetFlussoGestoreByMatr(matricola_utente, sfiltroIst != "" ? Convert.ToInt32(sfiltroIst) : 0);
      string mercato = "";
      if(!String.IsNullOrEmpty(myProfile.UserInfo.codGestore))
      {
         mercato =commonBusiness.getMercatoByCodGestore(Convert.ToInt32(myProfile.UserInfo.codGestore)).Substring(0, 1);
      }
      else if (flagUffRuolo=="A")
      {
        // ricerco il codice dell'area per escludere che sia un'area privati, in tal caso il mercato diverta Corporate (C)
        int? codArea = commonBusiness.GetCodAreaByUoArea(codUnitRuolo);
        if(codArea==null)
        {
          mercato = "C";
        }
      }
      UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "mercato", mercato==""?"R":mercato);
      UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "codGestore", myProfile.UserInfo.FlagResp!="Y"?myProfile.UserInfo.codGestore:"");
      //UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "mercato", flussoGestore != null ? flussoGestore.mercato : "");
      //UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "codGestore", flussoGestore != null ? flussoGestore.codGestore.ToString() : "");
      //UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "sottomercato", flussoGestore != null ? flussoGestore.sottomercato : "");
      //UserProfileManager.SetValueDictionary(myProfile.FiltriSicurezza, "flagConfrontoGestoreNDG", flussoGestore != null ? flussoGestore.flagConfrontoGestoreNDG.ToString() : "");

    }

  }
}