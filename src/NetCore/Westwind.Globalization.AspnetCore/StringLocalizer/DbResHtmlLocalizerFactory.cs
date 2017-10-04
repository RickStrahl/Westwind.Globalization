using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Westwind.Utilities;

namespace Westwind.Globalization.AspnetCore.StringLocalizer
{
    public class DbResHtmlLocalizerFactory : IHtmlLocalizerFactory
    {

        private DbResourceConfiguration Config;

        public DbResHtmlLocalizerFactory(DbResourceConfiguration config)
        {
            Config = config;
        }

        public IHtmlLocalizer Create(string baseName, string location)
        {
            return new DbResHtmlLocalizer(Config) { ResourceSet = baseName };
        }

        public IHtmlLocalizer Create(Type resourceSource)
        {
            var appAssembly = Assembly.GetEntryAssembly();
            string appNameSpace = appAssembly.GetName().Name;
            string baseName = resourceSource.FullName;

            if (baseName.StartsWith(appNameSpace))
                baseName = baseName.Substring(appNameSpace.Length + 1);

            return Create(baseName, null);
        }

      
    }
}
