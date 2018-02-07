using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Westwind.Globalization.Sample.Properties;
using Westwind.Utilities;
using Westwind.Web;

namespace Westwind.Globalization.Sample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {                       
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Specify where config information comes from (config file is default - can be separate Json/Xml)
            //DbResourceConfiguration.ConfigurationMode = ConfigurationModes.ConfigFile;
            
            // *** override which connection string is used for the provider configuration values
            // *** Note: The appropriate providers and Westwind.Globalization.DataProvider package
            //           have to be installed for all but SQL Server. 
            //           On Nuget.org: Search for Westwind.Globalization.
            // this is the default and doesn't need to be explicitly set
            //DbResourceConfiguration.Current.ConnectionString = "SqlServerLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceSqlServerDataManager);
            // DbResourceConfiguration.Current.DataProvider = DbResourceProviderTypes.SqlServer;

            //DbResourceConfiguration.Current.ConnectionString = "MySqlLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceMySqlDataManager);

            //DbResourceConfiguration.Current.ConnectionString = "SqLiteLocalizations";
            //DbResourceConfiguration.Current.ConnectionString = "Data Source=.\\App_Data\\SqLiteLocalizations.db";
            //DbResourceConfiguration.Current.DataProvider = DbResourceProviderTypes.SqLite;
            //DbResourceConfiguration.Current.ResourceTableName = "TestData";

            // for all other providers explicitly override the DbResourceDataManagerType
            //DbResourceConfiguration.Current.ConnectionString = "SqlServerCeLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceSqlServerCeDataManager);            

            // force ResourceMode explicitly. Default is AspNetResourceProvider
            //GeneratedResourceSettings.ResourceAccessMode = ResourceAccessMode.DbResourceManager;
            //GeneratedResourceSettings.ResourceAccessMode = ResourceAccessMode.Resx;

            // *** Remove or Add custom resource converters
            // *** By default the MarkdownResourceConverter is provided
            //DbResourceConfiguration.Current.ResourceSetValueConverters.Clear();
            //DbResourceConfiguration.Current.ResourceSetValueConverters.Add(new MarkdownResourceSetValueConverter());
        }

        protected void Application_BeginRequest()
        {
            WebUtils.SetUserLocale(currencySymbol: "$",allowedLocales: "en,de,fr");
            Trace.WriteLine("App_BeginRequest - Culture: " + Thread.CurrentThread.CurrentCulture.IetfLanguageTag);
        }
  
    }

}
