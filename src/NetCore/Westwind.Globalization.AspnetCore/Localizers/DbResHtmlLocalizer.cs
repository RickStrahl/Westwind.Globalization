using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Westwind.Globalization;

namespace Westwind.Globalization.AspnetCore
{
    public class DbResHtmlLocalizer : IHtmlLocalizer
    {
        DbResInstance DbRes { get; }

        public string ResourceSet { get; set; } = "Resources";

        private IHtmlLocalizerFactory HtmlLocalizerFactory { get; }

        public DbResHtmlLocalizer(DbResInstance dbRes, IHtmlLocalizerFactory htmlLocalizerFactory)
        {
            DbRes = dbRes ?? throw new System.ArgumentNullException(nameof(dbRes));
            HtmlLocalizerFactory = htmlLocalizerFactory ?? throw new System.ArgumentNullException(nameof(htmlLocalizerFactory));
        }

        public LocalizedString GetString(string name)
        {
            var val = DbRes.T(name, ResourceSet);
            return new LocalizedString(name, val);
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            var format = DbRes.T(name, ResourceSet);
            var value = string.Format(format, arguments);
            return new LocalizedString(name, value, resourceNotFound: format == null);
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

        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return HtmlLocalizerFactory.Create(ResourceSet, null);
        }

        LocalizedHtmlString IHtmlLocalizer.this[string name]
        {
            get { return new LocalizedHtmlString(name, GetString(name)); }
        }

        LocalizedHtmlString IHtmlLocalizer.this[string name, params object[] arguments]
        {
            get { return new LocalizedHtmlString(name, GetString(name, arguments)); }
        }
    }
}