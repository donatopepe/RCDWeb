using System;
using System.Collections.Generic;
using AreaCommonWeb.DAL;
using DAL.AreaCommonWeb;
using WebServiceModel.WSAcmeProfiles;
using WebServiceModel.WSSicurezza;

namespace RCDWeb.Business
{
  public class AccountBusiness
  {
    private ResourcesRepository resourcesRepository;

    public AccountBusiness()
    {

      this.resourcesRepository = new ResourcesRepository(new ResourcesContext());

    }

    public AccountBusiness(ResourcesRepository resourcesRepository)
    {
      this.resourcesRepository = resourcesRepository;
    }



    public Dictionary<string, string> GetMappingRuoloMansione(string istituto, string codMansione, string flagUffFil, string codApp)
    {
      var mappingRuoloMansione = resourcesRepository.GetMappingRuoloMansione(istituto, codMansione, flagUffFil, codApp);

      return mappingRuoloMansione;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="utente"></param>
    /// <param name="ruolo"></param>
    /// <returns></returns>
    public List<FiltroSicurezza> GetFiltriSicForUtnOrg(UserInfo utente, string ruolo, string flagUff)
    {
      List<FiltroSicurezza> filtriTotali = new List<FiltroSicurezza>();

      FiltroSicurezza filtriSicurezzaIstituto = new FiltroSicurezza();
      FiltroSicurezza filtriSicurezzaFiliale = new FiltroSicurezza();
      FiltroSicurezza filtriSicurezzaArea = new FiltroSicurezza();
      FiltroSicurezza filtriSicurezzaSegmento = new FiltroSicurezza();


      filtriSicurezzaIstituto.CodFiltro = "IST";
      filtriSicurezzaIstituto.ValoreFiltro = Convert.ToInt32(utente.CodIstituto).ToString();


      filtriSicurezzaFiliale.CodFiltro = "FIL";
      filtriSicurezzaFiliale.ValoreFiltro = resourcesRepository.GetCodFilialiByUo(utente.CodUnit).ToString();


      filtriSicurezzaArea.CodFiltro = "ARE";
      if (String.IsNullOrEmpty(filtriSicurezzaFiliale.ValoreFiltro) && flagUff == "A")
      {
          filtriSicurezzaArea.ValoreFiltro = resourcesRepository.GetCodAreaByUoArea(utente.CodUnit).ToString();
      }

      filtriSicurezzaSegmento.CodFiltro = "SEG";
      if (!String.IsNullOrEmpty(filtriSicurezzaArea.ValoreFiltro))
      {
          filtriSicurezzaSegmento.ValoreFiltro = "R";
          if (utente.DescUnit.Contains("Corporate")
            ||
                  utente.DescUnit.Contains("Imprese")
            )
          {
              filtriSicurezzaSegmento.ValoreFiltro = "C";

          }
      }
      filtriTotali.Add(filtriSicurezzaIstituto);
      if (filtriSicurezzaFiliale.ValoreFiltro != null)
        filtriTotali.Add(filtriSicurezzaFiliale);

      if (filtriSicurezzaArea.ValoreFiltro != null)
        filtriTotali.Add(filtriSicurezzaArea);

      if (filtriSicurezzaSegmento.ValoreFiltro != null)
        filtriTotali.Add(filtriSicurezzaSegmento);


      return filtriTotali;
    }

    public string GetFlagUff(string ouCode)
    {
      var flagUff = resourcesRepository.GetFlagUff(ouCode);
      return flagUff;
    }

    public string GetFlagUffArea(string ouCode)
    {
      var flagUff = resourcesRepository.GetFlagUffArea(ouCode);
      return flagUff;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="codFil"></param>
    /// <returns></returns>
    public string GetUOFiliale(int codFil)
    {
      var uoFiliale = resourcesRepository.GetUOFiliale(codFil);

      return uoFiliale;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="codUnit"></param>
    /// <returns></returns>
    public string GetDescUnit(string codUnit)
    {

      var descUnit = resourcesRepository.GetDescUnit(codUnit);

      return descUnit;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="codUnit"></param>
    /// <returns></returns>
    public string GetDescUnitArea(string codUnit)
    {

      var descUnit = resourcesRepository.GetDescUnitArea(codUnit);

      return descUnit;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numArea"></param>
    /// <param name="segArea"></param>
    /// <returns></returns>
    public string GetCodUoCodeArea(int istid,int numArea, string segArea)
    {

      var UoCodeArea = resourcesRepository.GetCodUoCodeArea(istid,numArea, segArea);

      return UoCodeArea;
    }

    /// <summary>
    /// responsabile designato  in mancanza del responsabile diretto
    /// </summary>
    /// <param name="numArea"></param>
    /// <param name="segArea"></param>
    /// <returns></returns>
    public string GetUoCodeResponsabileDesignato(string uocode)
    {

      var UoCode = resourcesRepository.GetUoCodeResponsabileDesignato(uocode);

      return UoCode;
    }

  }
}