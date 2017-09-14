using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Westwind.Globalization
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
        public static IServiceCollection  AddWestwindGlobalization(this IServiceCollection services, Action<DbResourceConfiguration> setOptionsAction)
        {           
            DbResourceConfiguration.Current.Initialize();
            setOptionsAction.Invoke(DbResourceConfiguration.Current);            
            return services;
        }
    }

}