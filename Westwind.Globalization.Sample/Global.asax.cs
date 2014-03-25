using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Westwind.Utilities;

namespace Westwind.Globalization.Sample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            // Automatically set the user's locale to what the browser returns
            // and set the currency symbol
            WebUtils.SetUserLocale(currencySymbol: "$", setUiCulture: true,allowedLocales: "en,de");
            Console.WriteLine(Thread.CurrentThread.CurrentUICulture.IetfLanguageTag);
        }
    }
}
