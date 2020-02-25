using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Westwind.Globalization.AspnetCore
{
    public class DbResHtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private DbResourceConfiguration _config;


        private IWebHostEnvironment _host;

        public DbResHtmlLocalizerFactory(DbResourceConfiguration config, IWebHostEnvironment env)
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


        /// <summary>
        /// Creates a localizer from a given type's name
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <returns></returns>
        public IHtmlLocalizer Create(Type resourceSource)
        {
            string baseName = resourceSource.FullName;
            return Create(baseName, _host.ApplicationName);
        }


    }
}
