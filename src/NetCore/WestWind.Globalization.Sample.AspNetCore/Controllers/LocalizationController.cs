using System.Globalization;
using System.Threading;
//using AppResources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace WestWind.Globalization.Sample.AspNetCore.Controllers
{
    [Route("api/Localization")]
    public class LocalizationController : Controller
    {
        private readonly IStringLocalizer _localizer;
        private readonly IHtmlLocalizer _htmlLocalizer;

        public LocalizationController(IStringLocalizer<LocalizationController> localizer,
                IHtmlLocalizer<LocalizationController> htmlLocalizer)
        {
            _localizer = localizer;
            _htmlLocalizer = htmlLocalizer;
        }

        [HttpGet]
        [Route("Localizer")]
        public string Localizer(string languageId)
        {
            return _localizer["HelloWorld"];
        }

        [HttpGet]
        [Route("HtmlLocalizer")]
        public LocalizedHtmlString HtmlLocalizer(string languageId)
        {
            return _htmlLocalizer["HelloWorld"];
        } 


        [HttpGet]
        [Route("StronglyTypedResource")]
        public string StronglyTypedResource(string languageId)
        {
            return AppResources.Resources.HelloWorld;
        }

        [HttpGet]
        [Route("GetAllLocalizerStrings")]
        public object GetAllLocalizerStrings()
        {
            return _localizer.GetAllStrings();
        }


        [HttpGet]
        [Route("DbRes")]
        public string DbRes(string id)
        {
            //return Westwind.Globalization.DbRes.T("HelloWorld","Resources","de-DE");
            return Westwind.Globalization.DbRes.T("HelloWorld", "Controllers.LocalizationController");
        }

        /// <summary>
        /// This gets Resx Resources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("StrongDbResResources")]
        public string StrongDbResResources()
        {
            return AppResources.Resources.HelloWorld;
            //return WestWind.Globalization.Sample.AspNetCore.Properties.Resources.HelloWorld;
        }


        /// <summary>
        /// This gets generated strongly typed resources using DbRes or Resx depending
        /// on configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("StrongDesignerResources")]
        public string StrongDesignerResources()
        {
            return AppResources.Resources.HelloWorld;
        }

        [HttpGet]
        [Route("Test")]
        public ActionResult Test()
        {
            return View("Test","Testing");
        }

        /// <summary>
        /// Sets the culture and UI culture to a specific culture. Allows overriding of currency
        /// and optionally disallows setting the UI culture.
        /// 
        /// You can also limit the locales that are allowed in order to minimize
        /// resource access for locales that aren't implemented at all.
        /// </summary>
        /// <param name="culture">
        /// 2 or 5 letter ietf string code for the Culture to set. 
        /// Examples: en-US or en</param>
        /// <param name="uiCulture">ietf string code for UiCulture to set</param>
        /// <param name="currencySymbol">Override the currency symbol on the culture</param>
        /// <param name="setUiCulture">
        /// if uiCulture is not set but setUiCulture is true 
        /// it's set to the same as main culture
        /// </param>
        /// <param name="allowedLocales">
        /// Names of 2 or 5 letter ietf locale codes you want to allow
        /// separated by commas. If two letter codes are used any
        /// specific version (ie. en-US, en-GB for en) are accepted.
        /// Any other locales revert to the machine's default locale.
        /// Useful reducing overhead in looking up resource sets that
        /// don't exist and using unsupported culture settings .
        /// Example: de,fr,it,en-US
        /// </param>
        public void SetUserLocale(string culture = null,
            string uiCulture = null,
            string currencySymbol = null,
            bool setUiCulture = true,
            string allowedLocales = null,
            HttpContext httpContext = null)
        {
            // Use browser detection in ASP.NET
            if (string.IsNullOrEmpty(culture) && httpContext != null)
            {
                HttpRequest Request = httpContext.Request;

                var langs = Request.Headers.GetCommaSeparatedValues("accept-language");


                // if no user lang leave existing but make writable
                if (langs == null || langs.Length == 0)
                {
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
                    if (setUiCulture)
                        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture.Clone() as CultureInfo;
                    return;
                }
                culture = langs[0];
            }
            else
                culture = culture?.ToLower();

            if (!string.IsNullOrEmpty(uiCulture))
                setUiCulture = true;

            if (!string.IsNullOrEmpty(culture) && !string.IsNullOrEmpty(allowedLocales))
            {
                allowedLocales = "," + allowedLocales.ToLower() + ",";
                if (!allowedLocales.Contains("," + culture + ","))
                {
                    int i = culture.IndexOf('-');
                    if (i > 0)
                    {
                        culture = culture.Substring(0, i);
                        if (!allowedLocales.Contains("," + culture + ","))
                        {
                            // Always create writable CultureInfo
                            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
                            if (setUiCulture)
                                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture.Clone() as CultureInfo;

                            return;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(culture))
                culture = CultureInfo.InstalledUICulture.IetfLanguageTag;

            if (string.IsNullOrEmpty(uiCulture))
                uiCulture = culture;

            try
            {
                CultureInfo Culture = new CultureInfo(culture);

                if (currencySymbol != null && currencySymbol != "")
                    Culture.NumberFormat.CurrencySymbol = currencySymbol;

                Thread.CurrentThread.CurrentCulture = Culture;

                if (setUiCulture)
                {
                    var UICulture = new CultureInfo(uiCulture);
                    Thread.CurrentThread.CurrentUICulture = UICulture;
                }
            }
            catch { }
        }

    }
}
