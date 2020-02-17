using System;
using Microsoft.AspNetCore.Mvc.Localization;
#if NETCOREAPP3_1
using HostEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
#else
using HostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace Westwind.Globalization.AspnetCore
{
    public class DbResHtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private HostEnvironment _host;
        private DbResourceConfiguration _config;

        public DbResHtmlLocalizerFactory(DbResourceConfiguration config, HostEnvironment env)
        {
            _config = config;
            _host = env;
        }

        /// <summary>
        /// Handler View Localizer access which provides full resource name
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public IHtmlLocalizer Create(string baseName, string location)
        {
            // strip off application base (location) if it's provided
            if (baseName != null && !string.IsNullOrEmpty(location) && baseName.StartsWith(location))
                baseName = baseName.Substring(location.Length + 1);

            return new DbResHtmlLocalizer(_config) { ResourceSet = baseName };
        }

        public IHtmlLocalizer Create(Type resourceSource)
        {
            string baseName = resourceSource.FullName;
            return Create(baseName, _host.ApplicationName);
        }


    }
}
