using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;

namespace Westwind.Globalization.AspnetCore
{
    public class DbResStringLocalizerFactory : IStringLocalizerFactory
    {
        private DbResourceConfiguration _config;
        private IHostingEnvironment _host;
        private readonly IResourceReaderFactory _resourceReaderFactory;

        public DbResStringLocalizerFactory(DbResourceConfiguration config, IHostingEnvironment host, IResourceReaderFactory resourceReaderFactory)
        {
            _config = config;
            _host = host;
            _resourceReaderFactory = resourceReaderFactory ?? throw new ArgumentNullException(nameof(resourceReaderFactory));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            // strip off application base(location) if it's provided
            if (baseName != null && baseName.StartsWith(location))
                baseName = baseName.Substring(location.Length + 1);

            var dbResInstance = new DbResInstance(_resourceReaderFactory, _config);
            return new DbResStringLocalizer(_config, dbResInstance, this) { ResourceSet = baseName };
        }

        public IStringLocalizer Create(Type resourceSource)
        {            
            string baseName = resourceSource.FullName;                             
            return Create(baseName, _host.ApplicationName);
        }
    }
}
