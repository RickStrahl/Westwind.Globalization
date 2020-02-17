using System;
using Microsoft.Extensions.Localization;
#if NETCOREAPP3_1
using HostEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;
#else
using HostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace Westwind.Globalization.AspnetCore
{
    public class DbResStringLocalizerFactory : IStringLocalizerFactory
    {
        private DbResourceConfiguration _config;
        private HostEnvironment _host;

        public DbResStringLocalizerFactory(DbResourceConfiguration config, HostEnvironment host)
        {
            _config = config;
            _host = host;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            // strip off application base(location) if it's provided
            if (baseName != null && !string.IsNullOrEmpty(location) && baseName.StartsWith(location))
                baseName = baseName.Substring(location.Length + 1);

            return new DbResStringLocalizer(_config) { ResourceSet = baseName };
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            string baseName = resourceSource.FullName;
            return Create(baseName, _host.ApplicationName);
        }
    }
}
