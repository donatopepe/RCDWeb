using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RCDWeb
{
    // Nota: per istruzioni su come abilitare la modalità classica di IIS6 o IIS7, 
    // visitare il sito Web all'indirizzo http://go.microsoft.com/?LinkId=9394801

    public class JsonModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            // If the request is not of content type "application/json"
            // then use the default model binder.
            if (!IsJSONRequest(controllerContext))
            {
                return base.BindModel(controllerContext, bindingContext);
            }

            // Get the JSON data posted
            var request = controllerContext.HttpContext.Request;
            var jsonStringData =
                new System.IO.StreamReader(request.InputStream).ReadToEnd();

            return new System.Web.Script.Serialization.JavaScriptSerializer()
                .Deserialize(jsonStringData, bindingContext.ModelMetadata.ModelType);
        }

        private bool IsJSONRequest(ControllerContext controllerContext)
        {
            var contentType = controllerContext.HttpContext.Request.ContentType;
            return contentType.Contains("application/json");
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Nome route
                "{controller}/{action}/{id}", // URL con parametri
                new { controller = "Account", action = "LogOnWithUtenzaTest", id = UrlParameter.Optional }, // Valori predefiniti parametri
                namespaces: new[] { "RCDWeb.Controllers" }
            );

        }

        /// <summary>
        /// Handle application error on a global level.
        /// 
        /// </summary>
        //protected void Application_Error()
        //{
        //    var exception = Server.GetLastError();
        //    var httpException = exception as HttpException;
        //    Response.Clear();
        //    Server.ClearError();
        //    var routeData = new RouteData();
        //    routeData.Values["controller"] = "Errors";
        //    routeData.Values["action"] = "General";

        //    routeData.Values["exception"] = exception;
        //    Response.StatusCode = 500;

        //    if (httpException != null)
        //    {
        //        Response.StatusCode = httpException.GetHttpCode();

        //        switch (Response.StatusCode)
        //        {
        //            case 403:
        //                routeData.Values["action"] = "Http403";
        //                break;

        //            case 404:
        //                routeData.Values["action"] = "Http404";
        //                break;

        //            default:
        //                routeData.Values["action"] = "Index";
        //                break;

        //        }
        //    }

        //    IController errorsController = new AreaCommonWeb.Areas.Common.Controllers.ErrorController();
        //    var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
        //    //Log.Write(exception, "", "");
        //    errorsController.Execute(rc);
        //}


        protected void Application_Start()
        {
            // Set the model binder
            //ModelBinders.Binders.DefaultBinder = new JsonModelBinder();


            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

    }
}