using System.Web.Mvc;

namespace RCDWeb.Areas.RCD
{
    public class RCDAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RCD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            //context.MapRoute(
            //    "RCD_default2",
            //    "RCD/{controller}/{action}#{id}",
            //    new { controller = "Richieste", action = "Index", id = UrlParameter.Optional },
            //       namespaces: new[] { "RCDWeb.Areas.RCD.Controllers" }
            //);

            context.MapRoute(
                "RCD_default",
                "RCD/{controller}/{action}/{id}",
                new { controller = "Richiesta", action = "GestioneRichieste", id = UrlParameter.Optional },
                namespaces: new[] { "RCDWeb.Areas.RCD.Controllers" }
            );

        }
    }
}
