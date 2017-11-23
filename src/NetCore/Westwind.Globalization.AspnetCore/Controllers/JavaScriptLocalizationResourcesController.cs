using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Westwind.Globalization;
using Westwind.Globalization.AspnetCore;
using Westwind.Globalization.AspNetCore.Extensions;
using Westwind.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WestWind.Globalization.AspNetCore.Controllers
{

    /// <summary>
    /// Controller that serves JavaScript resources to client side applications in the format of:
    /// 
    /// http://localhost:5000/api/JavaScriptLocalizationResources?ResourceSet=LocalizationForm&LocaleId=auto&VarName=resources&ResourceMode=resdb
    /// 
    /// Also supports legacy syntax:
    /// http://localhost:5000/JavaScriptResourceHandler.axd?ResourceSet=LocalizationForm&LocaleId=auto&VarName=resources&ResourceMode=resdb
    /// </summary>
    public class JavaScriptLocalizationResourcesController : Controller
    {

        protected DbResourceConfiguration Config { get;  }

        protected IHostingEnvironment Host { get; }

        protected IStringLocalizer Localizer { get;  }

        public JavaScriptLocalizationResourcesController(IHostingEnvironment host, DbResourceConfiguration config, IStringLocalizer<JavaScriptLocalizationResourcesController> localizer)
        {
            Config = config;
            Host = host;
            Localizer = localizer;
        }

        // http://localhost:5000/JavaScriptResourceHandler.axd?ResourceSet=LocalizationForm&LocaleId=auto&VarName=resources&ResourceMode=resdb
        // http://localhost:5000/api/JavaScriptLocalizationResources?ResourceSet=LocalizationForm&LocaleId=auto&VarName=resources&ResourceMode=resdb
        [Route("api/JavaScriptLocalizationResources")]
        [Route("JavaScriptResourceHandler.axd")]
        public ActionResult JavaScriptLocalizationResources()
        {
            return ProcessRequest();            
        }


        public ActionResult ProcessRequest()
        {

            var Request = HttpContext.Request;
           
            string resourceSet = Request.Query["ResourceSet"];


                string localeId = Request.Query["LocaleId"];
            if (string.IsNullOrEmpty(localeId))
                localeId = "auto";
            string resourceMode = Request.Query["ResourceMode"];
            if (string.IsNullOrEmpty(resourceMode))
                resourceMode = "Resx"; // Resx/ResDb/Auto          
            string varname = Request.Query["VarName"];
            if (string.IsNullOrEmpty(varname))
                varname = "resources";

                // varname is embedded into script so validate to avoid script injection
                // it's gotta be a valid C# and valid JavaScript name
                Match match = Regex.Match(varname, @"^[\w|\d|_|$|@|\.]*$");
            if (match.Length < 1 || match.Groups[0].Value != varname)
               SendErrorResponse("Invalid variable name passed.");

            if (string.IsNullOrEmpty(resourceSet))
               SendErrorResponse("Invalid ResourceSet specified.");

            // pick current UI Culture
            if (localeId == "auto")
            {
                try
                {
                    // Use ASP.NET Core RequestLocalization Mapping
                    var cultureProvider = HttpContext.Features.Get<IRequestCultureFeature>();
                    if(cultureProvider != null)
                        localeId = cultureProvider.RequestCulture.UICulture.IetfLanguageTag;
                    else
                        localeId = Thread.CurrentThread.CurrentUICulture.IetfLanguageTag;
                }
                catch
                {
                    localeId = Thread.CurrentThread.CurrentUICulture.IetfLanguageTag;
                }
            }            

            Dictionary<string, object> resDict = null;

            resourceMode = string.IsNullOrEmpty(resourceMode) ? "auto" : resourceMode.ToLower();

            ResourceAccessMode mode = ResourceAccessMode.Resx;
            if (resourceMode == "resdb")
                mode = ResourceAccessMode.DbResourceManager;
            else if (resourceMode == "auto")
                mode = DbResourceConfiguration.Current.ResourceAccessMode;
            

            if (mode == ResourceAccessMode.DbResourceManager)
            { 
                var resManager = DbResourceDataManager.CreateDbResourceDataManager(
                 Config.DbResourceDataManagerType);   
                resDict = resManager.GetResourceSetNormalizedForLocaleId(localeId, resourceSet);
                if (resDict == null || resDict.Count == 0)
                    mode = ResourceAccessMode.Resx; // try Resx resources from disk instead
            }
            if (mode != ResourceAccessMode.DbResourceManager) // Resx Resources loaded from disk
            {
                string basePath = Request.MapPath(DbResourceConfiguration.Current.ResxBaseFolder, basePath: Host.ContentRootPath);
                DbResXConverter converter = new DbResXConverter(basePath);

                resDict = converter.GetCompiledResourcesNormalizedForLocale(resourceSet,
                    DbResourceConfiguration.Current.ResourceBaseNamespace,
                    localeId);

                if (resDict == null)
                {
                    // check for .resx disk resources
                    string resxPath = converter.FormatResourceSetPath(resourceSet);
                    resDict = converter.GetResXResourcesNormalizedForLocale(resxPath, localeId);
                }
                else
                    resDict = resDict.OrderBy(kv => kv.Key).ToDictionary(k => k.Key, v => v.Value);
            }
            
            // return all resource strings
            resDict = resDict.Where(res => res.Value is string)
                    .ToDictionary(dict => dict.Key, dict => dict.Value);
            
            string javaScript = SerializeResourceDictionary(resDict, varname);

            

#if NETFULL // client cache
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                Response.ExpiresAbsolute = DateTime.UtcNow.AddDays(1);
                Response.AppendHeader("Accept-Ranges", "bytes");
                Response.AppendHeader("Vary", "Accept-Encoding");
                Response.Cache.SetETag("\"" + javaScript.GetHashCode().ToString("x") + "\"");
                Response.Cache.SetLastModified(DateTime.UtcNow);

                // OutputCache settings
                HttpCachePolicy cache = Response.Cache;

                cache.VaryByParams["ResourceSet"] = true;
                cache.VaryByParams["LocaleId"] = true;
                cache.VaryByParams["ResoureType"] = true;
                cache.VaryByParams["IncludeControls"] = true;
                cache.VaryByParams["VarName"] = true;
                cache.VaryByParams["ResourceMode"] = true;
                //cache.SetOmitVaryStar(true);

                DateTime now = DateTime.Now;
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetExpires(now + TimeSpan.FromDays(1));
                cache.SetValidUntilExpires(true);
                cache.SetLastModified(now);
            }
#endif

            return SendTextOutput(javaScript, "text/javascript; charset=utf-8");
        }

        public Dictionary<string, object> GetResourceSetFromCompiledResources(string resourceSet,
            string baseNamespace)
        {
            if (string.IsNullOrEmpty(baseNamespace))
                baseNamespace = DbResourceConfiguration.Current.ResourceBaseNamespace;

            var resourceSetName = baseNamespace + "." + resourceSet.Replace("/", ".").Replace("\\", ".");
            var type = ReflectionUtils.GetTypeFromName(resourceSetName);
            var resMan = new ResourceManager(resourceSetName, type.Assembly);
            var resDict = new Dictionary<string, object>();

            try
            {
                IDictionaryEnumerator enumerator;
                using (var resSet = resMan.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true))
                {
                    enumerator = resSet.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        var resItem = (DictionaryEntry) enumerator.Current;
                        resDict.Add((string) resItem.Key, resItem.Value);
                    }
                }
            }
            catch
            {
                return null;
            }

            return resDict;
        }

        /// <summary>
        /// Generates the actual JavaScript object map string makes up the
        /// handler's result content.
        /// </summary>
        /// <param name="resxDict"></param>
        /// <param name="varname"></param>
        /// <returns></returns>
        private string SerializeResourceDictionary(Dictionary<string, object> resxDict, string varname)
        {
            StringBuilder sb = new StringBuilder(2048);

            sb.Append(varname + " = {\r\n");

            int anonymousIdCounter = 0;
            foreach (KeyValuePair<string, object> item in resxDict)
            {
                string value = item.Value as string;
                if (value == null)
                    continue; // only encode string values

                string key = item.Key;
                if (string.IsNullOrEmpty(item.Key))
                    key = "__id" + anonymousIdCounter++.ToString();

                key = key.Replace(".", "_");
                if (key.Contains(" "))
                    key = StringUtils.ToCamelCase(key);

                sb.Append("\t\"" + key + "\": ");
                sb.Append(JsonConvert.SerializeObject(value));
                sb.Append(",\r\n");
            }

            // add dbRes function
            sb.AppendFormat(
                "\t" + @"""dbRes"": function dbRes(resId) {{ return {0}[resId] || resId; }}      
}}
", varname);


            return sb.ToString();
        }


        /// <summary>
        /// Returns an error response to the client. Generates a 404 error
        /// </summary>
        /// <param name="Message">Error message to display</param>
        private ActionResult SendErrorResponse(string Message)
        {
            if (!string.IsNullOrEmpty(Message))
                Message = "Invalid Web Resource";

            return NotFound(Message);
        }


        /// <summary>
        /// Writes text output to server using UTF-8 encoding and GZip encoding
        /// if supported by the client
        /// </summary>
        /// <param name="text"></param>
        /// <param name="useGZip"></param>
        /// <param name="contentType"></param>
        private ActionResult SendTextOutput(string text, string contentType = "text/javascript; charset=utf-8")
        {
            var Response = HttpContext.Response;
            Response.ContentType = contentType;

            //Response.Charset = "utf-8";
            //// Trigger Gzip encoding and headers if supported
            //if (text.Length > 2000)
            //    WebUtils.GZipEncodePage();

            return Content(text, new MediaTypeHeaderValue("text/javascript") { Charset = "utf-8"});
        }


        /// <summary>
        /// Returns a URL to the JavaScriptResourceHandler.axd handler that retrieves
        /// normalized resources for a given resource set and localeId and creates
        /// a JavaScript object with the name specified.
        /// 
        /// This function returns only the URL - you're responsible for embedding
        /// the URL into the page as a script tag to actually load the resources.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="localeId"></param>
        /// <param name="resourceMode></param>
        /// <returns></returns>
        public static string GetJavaScriptResourcesUrl(string varName, string resourceSet,
            string localeId = null,
            ResourceAccessMode resourceMode = ResourceAccessMode.AutoConfiguration)
        {
            if (resourceMode == ResourceAccessMode.AutoConfiguration)
                resourceMode = DbResourceConfiguration.Current.ResourceAccessMode;
            
            
            StringBuilder sb = new StringBuilder(512);
            string resType = resourceMode == ResourceAccessMode.DbResourceManager ? "resdb" : "resx";

            sb.Append("/api/JavaScriptLocalizationResources");
            sb.Append($"?ResourceSet={resourceSet}&LocaleId={localeId}&VarName={varName}&ResourceMode={resType}");
            

            return sb.ToString();
        }

        /// <summary>
        /// normalized SCRIPT tag to reference localized resources for a specific 
        /// ResourceSet.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="localeId"></param>
        /// <param name="resourceMode></param>
        /// <example>
        /// @LocalizationResourceController.GetJavaScriptResourcesScriptTag("myResources","MyServerResources")
        /// @LocalizationResourceController.GetJavaScriptResourcesScriptTag("myResources","MyServerResources",resourceMode: ResourceAccessMode.AutoConfiguration)
        /// @LocalizationResourceController.GetJavaScriptResourcesScriptTag("myResources","MyServerResources","de-DE,ResourceAccessMode.AutoConfiguration)
        /// </example>
        /// <returns></returns>

        public static HtmlString GetJavaScriptResourcesScriptTag(string varName, string resourceSet,
            string localeId = null,
            ResourceAccessMode resourceMode = ResourceAccessMode.AutoConfiguration)
        {
            var url = GetJavaScriptResourcesUrl(varName, resourceSet, localeId, resourceMode);

            return new HtmlString($"<script src=\"{url}\"></script>");
        }
    }

}
