using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Westwind.Utilities;

namespace Westwind.Globalization.AspNetCore
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
            // initialize the static instance - but you can override with 
            // a new customized instance if desired
            DbResourceConfiguration.Current.Initialize();
            var config = DbResourceConfiguration.Current;
            setOptionsAction?.Invoke(config);

            services.AddSingleton<DbResourceConfiguration>(config);

            return services;
        }
    }
}