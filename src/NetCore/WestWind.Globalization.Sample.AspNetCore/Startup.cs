using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Westwind.Globalization;

namespace WestWind.Globalization.Sample.AspNetCore
{
    public class Startup
    {
    
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Properties";
            });

            services.AddWestwindGlobalization(opt =>
            {                
                // the default settings comme from DbResourceConfiguration.json if exists

                // you can override the settings here - possibly with standard config settings

                // Resource Mode - Resx or DbResourceManager                
                opt.ResourceAccessMode = ResourceAccessMode.DbResourceManager;  // ResourceAccessMode.Resx

                opt.ConnectionString = "server=dev.west-wind.com;database=localizations;uid=localizations;pwd=local";
                opt.ResourceTableName = "localizations";
                opt.AddMissingResources = false;
                opt.ResxBaseFolder = "~/Properties/";
             
                
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
