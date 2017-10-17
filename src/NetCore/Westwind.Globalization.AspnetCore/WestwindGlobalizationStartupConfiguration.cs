using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;



namespace Westwind.Globalization.AspnetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWestwindGlobalization(this IApplicationBuilder app)
        {
            return app;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWestwindGlobalization(this IServiceCollection services,
            Action<DbResourceConfiguration> setOptionsAction = null)
        {
            // initialize the static instance from DbResourceConfiguration.json if it exists 
            // But you can override with a new customized instance if desired
            DbResourceConfiguration.Current.Initialize();
            var config = DbResourceConfiguration.Current;

            // we allow configuration via AppSettings so make sure that's loaded
            services.AddOptions();

            var provider = services.BuildServiceProvider();
            var serviceConfiguration = provider.GetService<IConfiguration>();

            var section = serviceConfiguration.GetSection("DbResourceConfiguration");
            // read settings from DbResourceConfiguration in Appsettings.json
            services.Configure<DbResourceConfiguration>(section);

            // HAVE TO rebuild or else the added config isn't available
            provider = services.BuildServiceProvider();
            var configData = provider.GetRequiredService<IOptions<DbResourceConfiguration>>();
            if (configData != null && configData.Value != null && !configData.Value.ConnectionString.StartsWith("***"))
            {
                config = configData.Value;
                DbResourceConfiguration.Current = config;
            }
            setOptionsAction?.Invoke(config);

            // register with DI
            services.AddSingleton(config);

            return services;
        }
    }

    public static class DbResourceConfigurationExtensions
    {
        public static void ConfigureAuthorizeLocalizationAdministration(
            this DbResourceConfiguration config,
            Func<ActionContext, bool> onAuthorizeLocalizationAdministration)
        {
            config.OnAuthorizeLocalizationAdministration = onAuthorizeLocalizationAdministration;
        }
    }
}