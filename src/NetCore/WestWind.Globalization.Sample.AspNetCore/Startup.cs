using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Westwind.Globalization;
using Westwind.Globalization.AspnetCore;
    

namespace WestWind.Globalization.Sample.AspNetCore
{
    public class Startup
    {
        IConfiguration Configuration;        

        public Startup(IConfiguration config)
        {            
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {                        
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Properties";
            });

            // Optionally enable IStringLocalizer to use DbRes objects instead of default ResourceManager
            services.AddSingleton<IStringLocalizerFactory, DbResStringLocalizerFactory>();
            services.AddSingleton<IHtmlLocalizerFactory, DbResHtmlLocalizerFactory>();            

            // Required for Westwind.Globalization to work!
            //services.AddWestwindGlobalization();
            services.AddWestwindGlobalization(opt =>
            {
                // the defaults are loaded in this order with later providers overwriting earlier values:
                // 0. Default DbResourceConfiguration values
                // 1. DbResourceConfiguration.json if exists
                // 2. AspNetCore Configuration Manager (IConfiguration/appsettings etc.)
                //    (appsettings.json, environment, user secrets - overrides entire object if set)
                // 3. Settings can be overridden in AddWestwindGlobalization(opt) here
                
                // Resource Mode - Resx or DbResourceManager                
                opt.ResourceAccessMode = ResourceAccessMode.DbResourceManager;  // ResourceAccessMode.Resx

                // *** override provider configuration
                // *** use ConnectionString + DataProvider (or DbResourceManagerType)

                // Sql Server
                // opt.ConnectionString = "server=.;database=localizations;integrated security=true;";
                // opt.ConnectionString = "server=.;database=localizations;uid=localizations;pwd=local;";
                // opt.DataProvider = DbResourceProviderTypes.SqlServer;

                // SqLite
                //opt.ConnectionString = "Data Source=./Data/SqLiteLocalizations.db";
                // // opt.DbResourceDataManagerType = typeof(DbResourceSqLiteDataManager);    // use this with custom providers            
                //opt.DataProvider = DbResourceProviderTypes.SqLite;

                // MySql
                //opt.ConnectionString = "server=localhost;uid=testuser;pwd=super10seekrit;database=Localizations;charset=utf8";
                //opt.DataProvider = DbResourceProviderTypes.MySql;

                opt.ResourceTableName = "localizations";
                opt.AddMissingResources = false;
                opt.ResxBaseFolder = "~/Properties/";

                // Set up security for Localization Administration form
                opt.ConfigureAuthorizeLocalizationAdministration(actionContext =>
                {
                    // return true or false whether this request is authorized
                    return true;   //actionContext.HttpContext.User.Identity.IsAuthenticated;
                });
            });

            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            // this *has to go here*  after view localization have been initialized
            // so that Pages can localize - note required even if you're not using
            // the DbResource manager. Fix in post ASP.NET Core 2.0
            services.AddTransient<IViewLocalizer, DbResViewLocalizer>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration, IOptions<DbResourceConfiguration> localizationConfig)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("en"),
                new CultureInfo("de-DE"),
                new CultureInfo("de"),
                new CultureInfo("fr")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures                 
            });



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            // print some environment information
            Console.WriteLine("\r\nPlatform: " + System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            Console.WriteLine("DbResourceMode: " + DbResourceConfiguration.Current.ResourceAccessMode);
            Console.WriteLine("Connection: " + DbResourceConfiguration.Current.ConnectionString);
            Console.WriteLine("Connection: " + DbResourceConfiguration.Current.ResourceTableName);
        }
    }
}
