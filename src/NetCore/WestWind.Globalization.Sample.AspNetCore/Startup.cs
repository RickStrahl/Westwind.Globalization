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
using Westwind.Globalization.AspnetCore.StringLocalizer;
using Westwind.Globalization.AspNetCore;

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


            services.Configure<DbResourceConfiguration>(Configuration.GetSection("DbResourceConfiguration"));

            var provider = services.BuildServiceProvider();            
            
            // Optionally enable IStringLocalizer to use DbRes objects instead of default ResourceManager
            //services.AddSingleton(typeof(IStringLocalizerFactory), typeof(DbResStringLocalizerFactory));
            //services.AddSingleton(typeof(IHtmlLocalizerFactory), typeof(DbResHtmlLocalizerFactory));

            // Required for Westwind.Globalization to work!
            services.AddWestwindGlobalization(opt =>
            {
                // the defaults are loaded from:
                // 1. DbResourceConfiguration.json if exists
                // 2. AspNetCore Configuration Manager (IConfiguration)
                //    (appsettings.json, environment, user secrets - overrides entire object if set)
                // 3. Settings overridden in AddWestwindGlobalization

                // you can override settings here - possibly with standard config settings

                // Resource Mode - Resx or DbResourceManager                
                opt.ResourceAccessMode = ResourceAccessMode.DbResourceManager;  // ResourceAccessMode.Resx

                opt.ConnectionString = "server=.;database=localizations;integrated security=true;";
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

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
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
        }
    }
}
