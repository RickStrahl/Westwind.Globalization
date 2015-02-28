using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            GeneratedResourceSettings.ResourceAccessMode = ResourceAccessMode.DbResourceManager;

            DbResourceConfiguration.ConfigurationMode = ConfigurationModes.ConfigFile;


            // *** override which connection string is used for the provider configuration values
            // *** Note: The appropriate providers and Westwind.Globalization.DataProvider package
            //           have to be installed for all but SQL Server. 
            //           On Nuget.org: Search for Westwind.Globalization.
            // this is the default and doesn't need to be explicitly set
            //DbResourceConfiguration.Current.ConnectionString = "SqlServerLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceSqlServerDataManager);

            //DbResourceConfiguration.Current.ConnectionString = "MySqlLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceMySqlDataManager);

            //DbResourceConfiguration.Current.ConnectionString = "SqLiteLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceSqLiteDataManager);

            // for all other providers explicitly override the DbResourceDataManagerType
            //DbResourceConfiguration.Current.ConnectionString = "SqlServerCeLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceSqlServerCeDataManager);
        }

        protected void Application_BeginRequest()
        {
            WebUtils.SetUserLocale(currencySymbol: "$");
            Trace.WriteLine("App_BeginRequest - Culture: " + Thread.CurrentThread.CurrentCulture.IetfLanguageTag);
        }
  
    }

}
