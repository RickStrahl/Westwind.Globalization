using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;

namespace Westwind.Globalization.AspnetCore
{
    public class DbResStringLocalizerFactory : IStringLocalizerFactory
    {
        private DbResourceConfiguration _config;
        private IHostingEnvironment _host;

        public DbResStringLocalizerFactory(DbResourceConfiguration config, IHostingEnvironment host)            
        {
            _config = config;
            _host = host;
        }
        

        public IStringLocalizer Create(string baseName, string location)
        {
            // strip off application base (location) if it's provided
            if (baseName.StartsWith(_host.ApplicationName))
                baseName = baseName.Substring(_host.ApplicationName.Length + 1);

            return new DbResStringLocalizer(_config) { ResourceSet = baseName };
        }

        public IStringLocalizer Create(Type resourceSource)
        {                        
            string baseName = resourceSource.FullName;

            // strip off project prefix - we just use the relative path
            if (baseName.StartsWith(_host.ApplicationName))
                    baseName = baseName.Substring(_host.ApplicationName.Length + 1);
                  
            return Create(baseName, _host.ApplicationName);
        }        
    }
}
