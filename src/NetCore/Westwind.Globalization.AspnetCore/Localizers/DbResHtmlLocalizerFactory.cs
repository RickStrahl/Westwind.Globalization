using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Westwind.Globalization.AspnetCore
{
    public class DbResHtmlLocalizerFactory : IHtmlLocalizerFactory
    {
        private IHostingEnvironment _host;
        private DbResourceConfiguration _config;

        public DbResHtmlLocalizerFactory(DbResourceConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _host = env;
        }

        public IHtmlLocalizer Create(string baseName, string location)
        {
            if (_config.ResourceAccessMode == ResourceAccessMode.Resx)
                baseName = location + "." + baseName;

            return new DbResHtmlLocalizer(_config) { ResourceSet = baseName };
        }


        public IHtmlLocalizer Create(Type resourceSource)
        {
            string appNameSpace = _host.ApplicationName;
            string baseName = resourceSource.FullName;

            // strip off project prefix - we just use the relative path
            if (baseName.StartsWith(appNameSpace))
                baseName = baseName.Substring(appNameSpace.Length + 1);

            return Create(baseName, appNameSpace);
        }


    }
}
