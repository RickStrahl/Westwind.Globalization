using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Westwind.Globalization.AspnetCore
{

    /// <summary>
    /// An IStringLocalizer that loads resources through 
    /// Westwind.Globalizations DbResourceManager from
    /// a database or optionally from Resx resources (depending
    /// on the configured cref="Westwind.Globalization.ResourceAccessMode"
    /// </summary>
    public class DbResStringLocalizer : IStringLocalizer
    {        
        DbResInstance DbRes { get;   }

        DbResourceConfiguration Config { get;  }

        /// <summary>
        /// Default ResourceSetName if no Template type is provided
        /// </summary>
        public string ResourceSet { get; set; }

        public DbResStringLocalizer(DbResourceConfiguration config)
        {
            DbRes = new DbResInstance(config);
            Config = config;
            
            // default
            ResourceSet = Config.StringLocalizerResourcePath + ".CommonResources";
        }


        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var resources = DbRes.GetResourceSet(ResourceSet);
            if (resources != null)
            {
                foreach (DictionaryEntry resource in resources)
                {
                    var key = resource.Key as string;
                    var value = resource.Value as string;

                    var localizedString = new LocalizedString(key, value);
                    yield return localizedString;

                }
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return new DbResStringLocalizer(Config);            
        }

        LocalizedString IStringLocalizer.this[string name]
        {
            get
            {
                var val = DbRes.T(name,ResourceSet);
                return new LocalizedString(name, val);
            }
        }

        LocalizedString IStringLocalizer.this[string name, params object[] arguments]
        {
            get
            {
                var format = DbRes.T(name, ResourceSet);
                var value = string.Format(format, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);                
            }
        }
    }


    
}
