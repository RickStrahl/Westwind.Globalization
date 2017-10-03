using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Westwind.Utilities;

namespace Westwind.Globalization.AspnetCore.StringLocalizer
{
    public class DbResStringLocalizerFactory : IStringLocalizerFactory
    {
        private DbResourceConfiguration Config;

        public DbResStringLocalizerFactory(DbResourceConfiguration config)            
        {
            Config = config;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new DbResStringLocalizer(Config) { ResourceSet = baseName };
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var appAssembly = Assembly.GetEntryAssembly();
            var appAssemblyName = appAssembly.GetName();

            string appNameSpace = appAssembly.GetName().Name;
            string baseName = resourceSource.FullName;

            if (baseName.StartsWith(appNameSpace))
                baseName = baseName.Substring(appNameSpace.Length + 1);

            return Create(baseName, null);
        }        
    }
}
