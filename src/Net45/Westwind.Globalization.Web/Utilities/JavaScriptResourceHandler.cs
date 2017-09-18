#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2008 - 2009
 *          http://www.west-wind.com/
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Globalization;
using System.Resources;
using System.Threading;
using Westwind.Web;
using Westwind.Utilities;

namespace Westwind.Globalization
{
    /// <summary>
    /// Http Handler that returns ASP.NET Local and Global Resources as JavaScript
    /// objects. Supports both plain Resx Resources as well as DbResourceProvider 
    /// driven resources.
    /// 
    /// Objects are generated in the form of:
    /// 
    /// &lt;&lt;code lang="JavaScript"&gt;&gt;var localRes  = {
    ///    BackupFailed: "Backup was not completed",
    ///    Loading: "Loading"
    /// );&lt;&lt;/code&gt;&gt;
    /// 
    /// where the resource key becomes the property name with a string value.
    /// 
    /// The handler is driven through query string variables determines which 
    /// resources are returned:
    /// 
    /// ResourceSet      -  Examples: "resources" (global), "admin/somepage.aspx" "default.aspx" (local)
    /// LocaleId         -  Examples: "de-de","de",""  (empty=invariant)
    /// ResourceType     -  Resx,ResDb
    /// IncludeControls  -  if non-blank includes control values (. in name)
    /// VarName          -  name of hte variable generated - if omitted localRes or globalRes is created.    
    /// ResourceMode -  Flag required to find Resx resources on disk 0 - Local 1 - global 2 - plain resx
    /// 
    /// Resources retrieved are aggregated for the locale Id (ie. de-de returns 
    /// de-de,de and invariant) whichever matches first.
    /// </summary>
    public class JavaScriptResourceHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest Request = HttpContext.Current.Request;
            HttpResponse Response = HttpContext.Current.Response;

            string resourceSet = Request.Params["ResourceSet"];
            string localeId = Request.Params["LocaleId"] ?? "auto";
            string resourceType = Request.Params["ResourceType"] ?? "Resx";   // Resx/ResDb
            bool includeControls = (Request.Params["IncludeControls"] ?? "") != "";
            string varname = Request.Params["VarName"] ?? "resources";
            string resourceMode = (Request.Params["ResourceMode"] ?? "0");

            // varname is embedded into script so validate to avoid script injection
            // it's gotta be a valid C# and valid JavaScript name
            Match match = Regex.Match(varname, @"^[\w|\d|_|$|@|\.]*$");
            if (match.Length < 1 || match.Groups[0].Value != varname)
                SendErrorResponse("Invalid variable name passed.");

            if (string.IsNullOrEmpty(resourceSet))
                SendErrorResponse("Invalid ResourceSet specified.");

            // pick current UI Culture
            if (localeId == "auto")
                localeId = Thread.CurrentThread.CurrentUICulture.IetfLanguageTag;

            Dictionary<string, object> resDict = null;

            if (string.IsNullOrEmpty(resourceType) || resourceType == "auto")
            {
                if (DbResourceProvider.ProviderLoaded || DbSimpleResourceProvider.ProviderLoaded)
                    resourceType = "resdb";
                else
                    resourceType = "resx";
            }


            if (resourceType.ToLower() == "resdb")
            {                
                // use existing/cached resource manager if previously used
                // so database is accessed only on first hit
                var resManager = DbRes.GetResourceManager(resourceSet);

                DbResXConverter converter = new DbResXConverter(context.Server.MapPath(DbResourceConfiguration.Current.ResxBaseFolder));
                resDict = converter.GetResourcesNormalizedForLocale(resManager, localeId);

                //resDict = manager.GetResourceSetNormalizedForLocaleId(localeId, resourceSet);
                if (resDict == null || resDict.Keys.Count < 1)
                {
                    // try resx instead
                    string resxPath = converter.FormatResourceSetPath(resourceSet);
                    resDict = converter.GetResXResourcesNormalizedForLocale(resxPath, localeId);
                }
            }
            else  // Resx Resources
            {
                string basePath = context.Server.MapPath(DbResourceConfiguration.Current.ResxBaseFolder);
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


            if (resourceMode == "0" && !includeControls)
            {
                // filter the list to strip out controls (anything that contains a . in the ResourceId 
                // is considered a control value
                resDict = resDict.Where(res => !res.Key.Contains('.') && res.Value is string)
                                 .ToDictionary(dict => dict.Key, dict => dict.Value);
            }
            else
            {
                // return all resource strings
                resDict = resDict.Where(res => res.Value is string)
                           .ToDictionary(dict => dict.Key, dict => dict.Value);
            }

            string javaScript = SerializeResourceDictionary(resDict, varname);


            // client cache
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

            SendTextOutput(javaScript, "text/javascript");
        }

        public Dictionary<string, object> GetResourceSetFromCompiledResources(string resourceSet, string baseNamespace)
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
            catch (Exception ex)
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
                sb.Append(WebUtils.EncodeJsString(value));
                sb.Append(",\r\n");
            }

            // add dbRes function
            sb.AppendFormat(
"\t" + @"""dbRes"": function dbRes(resId) {{ return {0}[resId] || resId; }}      
}}
",varname);                

 
            return sb.ToString();
        }


        /// <summary>
        /// Returns an error response to the client. Generates a 404 error
        /// </summary>
        /// <param name="Message">Error message to display</param>
        private void SendErrorResponse(string Message)
        {
            if (!string.IsNullOrEmpty(Message))
                Message = "Invalid Web Resource";

            HttpContext Context = HttpContext.Current;

            Context.Response.StatusCode = 404;
            Context.Response.StatusDescription = Message;
            Context.Response.End();
        }


        /// <summary>
        /// Writes text output to server using UTF-8 encoding and GZip encoding
        /// if supported by the client
        /// </summary>
        /// <param name="text"></param>
        /// <param name="useGZip"></param>
        /// <param name="contentType"></param>
        private void SendTextOutput(string text, string contentType = "text/javascript")
        {
            HttpResponse Response = HttpContext.Current.Response;
            Response.ContentType = contentType;
            Response.Charset = "utf-8";

            // Trigger Gzip encoding and headers if supported
            if (text.Length > 2000)
                WebUtils.GZipEncodePage();       

            Response.Write(text);            
        }

        /// <summary>
        /// Inserts global resources into the current page.
        /// </summary>
        /// <param name="control">A control (typically) page needed to embed into the page</param>
        /// <param name="resourceSet">Name of the resourceSet to load</param>
        /// <param name="localeId">The Locale for which to load resources. Normalized from most specific to Invariant</param>
        /// <param name="varName">Name of the variable generated</param>
        /// <param name="resourceType">Resx or DbResourceProvider (database)</param>
        /// <param name="includeControls">Determines whether control ids are included</param>
        public static void RegisterJavaScriptGlobalResources(Control control, string varName, string resourceSet, string localeId,
                                                           ResourceProviderTypes resourceType)
        {
            string url = GetJavaScriptGlobalResourcesUrl(varName, resourceSet, localeId, resourceType);

            ClientScriptProxy.Current.RegisterClientScriptInclude(control, typeof(JavaScriptResourceHandler),
                                                                  url, ScriptRenderModes.Header);
        }


        /// <summary>
        /// Embed global JavaScript resources into the page.
        /// 
        /// This version returns resources of the active Resx or DB Resource Provider
        /// and includes no controls and creates a variable named "globalRes"
        /// and uses the page's current UI culture
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resourceSet"></param>       
        public static void RegisterJavaScriptGlobalResources(Control control, string varName, string resourceSet)
        {
            RegisterJavaScriptGlobalResources(control, varName, resourceSet,
                                              CultureInfo.CurrentUICulture.IetfLanguageTag,
                                              ResourceProviderTypes.AutoDetect);
        }

        /// <summary>
        /// Inserts local resources into the current page.
        /// </summary>
        /// <param name="control">A control (typically) page needed to embed into the page</param>
        /// <param name="resourceSet">Name of the resourceSet to load</param>
        /// <param name="localeId">The Locale for which to load resources. Normalized from most specific to Invariant</param>
        /// <param name="varName">Name of the variable generated</param>
        /// <param name="resourceType">Resx or DbResourceProvider (database)</param>
        /// <param name="includeControls">Determines whether control ids are included</param>
        public static void RegisterJavaScriptLocalResources(Control control, string varName, string localeId, string resourceSet,
                                                            ResourceProviderTypes resourceType, bool includeControls)
        {
            string url = GetJavaScriptLocalResourcesUrl(varName, localeId, resourceSet, resourceType, includeControls);

            ClientScriptProxy.Current.RegisterClientScriptInclude(control, typeof(JavaScriptResourceHandler),
                                                                   url, ScriptRenderModes.Header);
        }

        /// <summary>
        /// Embed global JavaScript resources into the page.
        /// 
        /// This version returns resources of the active Resx or DB Resource Provider,
        /// includes no controls and uses the CurrentUICulture's locale id
        /// </summary>
        /// <param name="control">A control or page instance required to </param>
        /// <param name="varName">Name of the JavaScript object variable created</param>        
        public static void RegisterJavaScriptLocalResources(Control control, string varName)
        {
            ResourceProviderTypes type = ResourceProviderTypes.AutoDetect;

            // translate current page path into resource path
            string resourceSet = WebUtils.GetAppRelativePath();

            RegisterJavaScriptLocalResources(control, varName,
                                             CultureInfo.CurrentUICulture.IetfLanguageTag, resourceSet, type, false);
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
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static string GetJavaScriptGlobalResourcesUrl(string varName, string resourceSet, string localeId = null,
                                                           ResourceProviderTypes resourceType = ResourceProviderTypes.AutoDetect)
        {
            if (resourceType == ResourceProviderTypes.AutoDetect)
            {
                if (DbSimpleResourceProvider.ProviderLoaded || DbResourceProvider.ProviderLoaded)
                    resourceType = ResourceProviderTypes.DbResourceProvider;
            }
            

            StringBuilder sb = new StringBuilder(512);
            sb.Append(WebUtils.ResolveUrl("~/") + "JavaScriptResourceHandler.axd?");
            sb.AppendFormat("ResourceSet={0}&LocaleId={1}&VarName={2}&ResourceType={3}",
                             resourceSet, localeId, varName,
                             resourceType == ResourceProviderTypes.DbResourceProvider ? "resdb" : "resx");
            sb.Append("&ResourceMode=1");

            return sb.ToString();
        }


        /// <summary>
        /// Returns a URL to the JavaScriptResourceHandler.axd handler that retrieves
        /// normalized resources for a given resource set and localeId and creates
        /// a JavaScript object with the name specified.
        /// 
        /// This version assumes the current UI Culture and auto-detects the
        /// provider type (Resx or DbRes) currently active.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public static string GetJavaScriptGlobalResourcesUrl(string varName, string resourceSet)
        {
            string localeId = CultureInfo.CurrentUICulture.IetfLanguageTag;
            return GetJavaScriptGlobalResourcesUrl(varName, resourceSet, localeId, ResourceProviderTypes.AutoDetect);
        }

        /// <summary>
        /// Inserts local resources into the current page.
        /// </summary>
        /// <param name="control">A control (typically) page needed to embed into the page</param>
        /// <param name="resourceSet">Name of the resourceSet to load</param>
        /// <param name="localeId">The Locale for which to load resources. Normalized from most specific to Invariant</param>
        /// <param name="varName">Name of the variable generated</param>
        /// <param name="resourceType">Resx or DbResourceProvider (database)</param>
        /// <param name="includeControls">Determines whether control ids are included</param>
        public static string GetJavaScriptLocalResourcesUrl(string varName, string localeId, string resourceSet,
                                                            ResourceProviderTypes resourceType, bool includeControls)
        {
            if (resourceType == ResourceProviderTypes.AutoDetect)
            {
                if (DbSimpleResourceProvider.ProviderLoaded || DbResourceProvider.ProviderLoaded)
                    resourceType = ResourceProviderTypes.DbResourceProvider;
            }

            StringBuilder sb = new StringBuilder(512);

            sb.Append(WebUtils.ResolveUrl("~/") + "JavaScriptResourceHandler.axd?");
            sb.AppendFormat("ResourceSet={0}&LocaleId={1}&VarName={2}&ResourceType={3}&ResourceMode=0",
                resourceSet, localeId, varName, 
                (resourceType == ResourceProviderTypes.DbResourceProvider ? "resdb" : "resx") );
            if (includeControls)
                sb.Append("&IncludeControls=1");            

            return sb.ToString();
        }

        /// <summary>
        /// Returns a URL to embed local resources into the page via JavaScriptResourceHandler.axd. 
        /// This method returns only a URL - you're responsible for embedding the script tag into the page
        /// to actually load the resources.
        /// 
        /// This version assumes the local resource set for the current request/page and autodetected
        /// resources (resdb or resx). It also uses the CurrentUICulture as the locale.
        /// </summary>
        /// <param name="varName">The name of the JavaScript variable to create</param>        
        /// <param name="includeControls"></param>
        /// <returns></returns>
        public static string GetJavaScriptLocalResourcesUrl(string varName, bool includeControls)
        {
            string resourceSet = WebUtils.GetAppRelativePath();
            string localeId = CultureInfo.CurrentUICulture.IetfLanguageTag;
            return GetJavaScriptLocalResourcesUrl(varName, localeId, resourceSet,
                                                  ResourceProviderTypes.AutoDetect, includeControls);
        }

        /// <summary>
        /// Returns a standard Resx resource based on it's . delimited resourceset name
        /// </summary>
        /// <param name="varName">The name of the JavaScript variable to create</param>
        /// <param name="resourceSet">The name of the resource set
        /// 
        /// Example:
        /// CodePasteMvc.Resources.Resources  (~/Resources/Resources.resx in CodePasteMvc project)
        /// </param>
        /// <param name="localeId">IETF locale id (2 or 4 en or en-US or empty)</param>
        /// <param name="resourceType">ResDb or ResX</param>
        /// <returns></returns>
        public static string GetJavaScriptResourcesUrl(string varName, string resourceSet,
                                                      string localeId = null,
                                                      ResourceProviderTypes resourceType = ResourceProviderTypes.AutoDetect)
        {
            if (localeId == null)
                localeId = CultureInfo.CurrentUICulture.IetfLanguageTag;
            
            if (resourceType == ResourceProviderTypes.AutoDetect)
            {
                if (DbSimpleResourceProvider.ProviderLoaded || DbResourceProvider.ProviderLoaded)
                    resourceType = ResourceProviderTypes.DbResourceProvider;
            }

            StringBuilder sb = new StringBuilder(512);
            sb.Append(WebUtils.ResolveUrl("~/") + "JavaScriptResourceHandler.axd?");
            sb.AppendFormat("ResourceSet={0}&LocaleId={1}&VarName={2}&ResourceType={3}",
                             resourceSet, localeId, varName,
                             resourceType == ResourceProviderTypes.DbResourceProvider ? "resdb" : "resx");
            sb.Append("&ResourceMode=1");

            return sb.ToString();
        }

    }


    /// <summary>
    /// Determines the resource provider type used
    /// to retrieve resources.
    /// 
    /// Note only applies to the stock ResX provider
    /// or the DbResourceProviders of this assembly.
    /// Other custom resource providers are not supported.
    /// </summary>
    public enum ResourceProviderTypes
    {
        Resx,
        DbResourceProvider,
        AutoDetect
    }

}
