using LightInject;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;

namespace XServices.EmailTemplateWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new ServiceContainer();

            container.RegisterApiControllers();
            container.EnableWebApi(GlobalConfiguration.Configuration);
            container.RegisterAssembly(typeof(XServices.EmailTemplateWebApi.Controllers.EmailTemplateController).Assembly);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //   RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Force JSON responses on all requests
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}