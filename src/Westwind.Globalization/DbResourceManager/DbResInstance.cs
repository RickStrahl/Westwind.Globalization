#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2009-2017
 *          http://www.west-wind.com/
 * 
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
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Web;

namespace Westwind.Globalization
{

    /// <summary>
    /// Helper class that handles access to the DbResourceManager
    /// more easily with single method access. The T() method provides
    /// an easy way to embed resources into applications using the
    /// resource key.
    /// 
    /// Also allows for resource reading, writing (new and updates transparently), 
    /// deleting and clearing of resources from memory.
    /// 
    /// This class uses the DbResourceManager class to access
    /// resources and still uses the standard ResourceManager 
    /// infrastructure of .NET to cache resources efficiently
    /// in memory. Data access occurs only on intial access of
    /// each resource set/locale.
    /// </summary>
    public class DbResInstance
    {
        /// <summary>
        /// Internal dictionary that holds instances of resource managers
        /// for each resourceset defined in the application. Lazy loaded
        /// as resources are accessed.
        /// </summary>
        protected static Dictionary<string, DbResourceManager> ResourceManagers =
            new Dictionary<string, DbResourceManager>();

        /// <summary>
        /// Determines whether resources that fail in a lookup are automatically
        /// added to the resource table
        /// </summary>
        public static bool AutoAddResources { get; set; }

        public DbResourceConfiguration Configuration { get; set; }

        public DbResInstance(DbResourceConfiguration configuration = null)
        {
            AutoAddResources = DbResourceConfiguration.Current.AddMissingResources;
            if (Configuration != null)
                Configuration = configuration;
            else
                Configuration = DbResourceConfiguration.Current;
        }

        /// <summary>
        /// Localization helper function that Translates a resource
        /// Id to a resource value string. Easy access that allows full
        /// control over the resource to retrieve or default UiCulture
        /// locale retrieval.
        /// </summary>
        /// <param name="resId">The Resource Id to retrieve
        /// Note resource Ids can be *any* string and if no
        /// matching resource is found the id is returned.
        /// </param>
        /// <param name="resourceSet">Name of the ResourceSet that houses this resource. If null or empty resources are used.</param>
        /// <param name="lang">5 letter or 2 letter language ieetf code: en-US, de-DE or en, de etc.</param>
        /// <returns>
        /// Localized resource or the resource Id if no match is found. 
        /// This value *always* returns a string unless you pass in null.
        /// </returns>
        public string T(string resId, string resourceSet = null, string lang = null)
        {
            if (string.IsNullOrEmpty(resId))
                return resId;

            if (resourceSet == null)
                resourceSet = string.Empty;

#if NETFULL
            if (DbResourceConfiguration.Current.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider && HttpContext.Current != null)
            {
                var translated = HttpContext.GetGlobalResourceObject(resourceSet, resId) as string;
                if (string.IsNullOrEmpty(translated))
                    return resId;

                return translated;
            }
#endif

            var manager = GetResourceManager(resourceSet);
            if (manager == null)
                return resId;

            CultureInfo ci;
            if (string.IsNullOrEmpty(lang))
                ci = CultureInfo.CurrentUICulture;
            else
                ci = new CultureInfo(lang);

            string result = manager.GetObject(resId, ci) as string;

            if (string.IsNullOrEmpty(result))
                return resId;

            return result;
        }


        /// <summary>
        /// Localization helper function that Translates a resource
        /// Id to a resource value string. If id is not found default text is returned. Easy access that allows full
        /// control over the resource to retrieve or default UiCulture
        /// locale retrieval.
        /// </summary>
        /// <param name="resId">The Resource Id to retrieve</param>
        /// <param name="defaultText">Default text that is returned when resource with given resId is not found</param>
        /// <param name="resourceSet">Name of the ResourceSet that houses this resource. If null or empty resources are used.</param>
        /// <param name="lang">5 letter or 2 letter language ieetf code: en-US, de-DE or en, de etc.</param>
        /// <returns>
        /// Localized resource or the resource Id if no match is found. 
        /// This value *always* returns a string unless you pass in null in defaultText.
        /// </returns>
        /// 
        public string TDefault(string resId, string defaultText, string resourceSet, string lang = null)
        {
            if (string.IsNullOrEmpty(resId))
                return defaultText;

            if (resourceSet == null)
                resourceSet = string.Empty;

#if NETFULL
            if (DbResourceConfiguration.Current.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider && HttpContext.Current != null)
            {
                var translated = HttpContext.GetGlobalResourceObject(resourceSet, resId) as string;
                if (string.IsNullOrEmpty(translated))
                    return defaultText;

                return translated;
            }
#endif

            var manager = GetResourceManager(resourceSet);
            if (manager == null)
                return defaultText;

            CultureInfo ci;
            if (string.IsNullOrEmpty(lang))
                ci = CultureInfo.CurrentUICulture;
            else
                ci = new CultureInfo(lang);

            string result = manager.GetObject(resId, ci) as string;

            if (string.IsNullOrEmpty(result))
                return defaultText;

            return result;
        }

#if NETFULL /// <summary>
/// Localization helper function that Translates a resource
/// Id to a resource value to an HtmlStringg. Easy access that allows full
/// control over the resource to retrieve or default UiCulture
/// locale retrieval.
/// 
/// Use this version for HTML content that needs to be embedded in Razor
/// views or other server tools that can use pre-encoded HTML content.
/// </summary>
/// <param name="resId">The Resource Id to retrieve
/// Note resource Ids can be *any* string and if no
/// matching resource is found the id is returned.
/// </param>
/// <param name="resourceSet">Name of the ResourceSet that houses this resource. If null or empty resources are used.</param>
/// <param name="lang">5 letter or 2 letter language ieetf code: en-US, de-DE or en, de etc.</param>
/// <returns>
/// Localized resource or the resource Id if no match is found. 
/// This value *always* returns a string unless you pass in null.
/// </returns>
        public HtmlString THtml(string resId, string resourceSet = null, string lang = null)
        {
            return new HtmlString(T(resId, resourceSet, lang));
        }
#endif
        

        /// <summary>
        /// Creates a localized format string that is transformed using the 
        /// specified resource id. The translated text is the first parameter 
        /// in the format
        /// string
        /// </summary>
        /// <param name="format">Format string that is to be localized</param>
        /// <param name="resId">Resource id to localize from</param>
        /// <param name="resourceSet">Resource set to localize from</param>
        /// <param name="localeId">Language code</param>
        /// <param name="args">Any arguments for the format string</param>
        /// <returns></returns>
        public string TFormat(string format, string resId, string resourceSet, params object[] args)
        {
            var val = T(resId, resourceSet);

            // insert value into args array
            if (args == null || args.Length == 0)
                args = new object[] {val};
            else
                args = (new object[] {val}).Concat(args).ToArray();

            return string.Format(format, args);
        }


        /// <summary>
        /// Localization helper function that Translates a resource
        /// Id to a resource value object. Use this function if you're
        /// retrieving non-string values - for string values just use T.
        /// </summary>
        /// <param name="resId">The Resource Id to retrieve
        /// Note resource Ids can be *any* string and if no
        /// matching resource is found the id is returned.
        /// </param>
        /// <param name="resourceSet">Name of the ResourceSet that houses this resource. If null or empty resources are used.</param>
        /// <param name="lang">5 letter or 2 letter language ieetf code: en-US, de-DE or en, de etc.</param>
        /// <param name="autoAdd">If true if a resource cannot be found a new entry is added in the invariant locale</param>
        /// <returns>
        /// The resource as an object.    
        /// </returns>
        public object TObject(string resId, string resourceSet = null, string lang = null, bool autoAdd = false)
        {
            if (string.IsNullOrEmpty(resId))
                return resId;

            if (resourceSet == null)
                resourceSet = string.Empty;

            // check if the res manager exists
            ResourceManager manager = GetResourceManager(resourceSet);

            // no manager no resources
            if (manager == null)
                return resId;

            CultureInfo ci = null;
            if (string.IsNullOrEmpty(lang))
                ci = CultureInfo.CurrentUICulture;
            else
                ci = new CultureInfo(lang);

            if (manager is DbResourceManager)
                ((DbResourceManager)manager).AutoAddMissingEntries = AutoAddResources;

            object result = manager.GetObject(resId, ci);

            if (result == null)
                return resId;

            return result;
        }

        /// <summary>
        /// Writes a resource either creating or updating an existing resource 
        /// </summary>
        /// <param name="resourceId">Resource Id to write. Resource Ids can be any string up to 1024 bytes in length</param>
        /// <param name="value">Value to set the resource to</param>
        /// <param name="lang">Language as ieetf code: en-US, de-DE etc. 
        /// Value can be left blank for Invariant/Default culture to set.
        /// </param>
        /// <param name="resourceSet">The resourceSet to store the resource on. 
        /// If no resource set name is provided a default empty resource set is used.</param>
        /// <returns>true or false</returns>
        public bool WriteResource(string resourceId, string value = null, string lang = null,
            string resourceSet = null)
        {
            if (lang == null)
                lang = string.Empty;
            if (resourceSet == null)
                resourceSet = string.Empty;
            if (value == null)
                value = resourceId;

            var db = DbResourceDataManager.CreateDbResourceDataManager();
            return db.UpdateOrAddResource(resourceId, value, lang, resourceSet, null) > -1;
        }

        /// <summary>
        /// Deletes a resource entry
        /// </summary>
        /// <param name="resourceId">The resource to delete</param>
        /// <param name="lang">The language Id - Be careful:  If empty or null deletes matching keys for all languages</param>
        /// <param name="resourceSet">The resource set to apply</param>
        /// <returns>true or false</returns>
        public bool DeleteResource(string resourceId, string resourceSet = null, string lang = null)
        {
            var db = DbResourceDataManager.CreateDbResourceDataManager();
            return db.DeleteResource(resourceId, resourceSet: resourceSet, cultureName: lang);
        }

        /// <summary>
        /// Returns an instance of a DbResourceManager
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public ResourceManager GetResourceManager(string resourceSet)
        {
            // check if the res manager exists
            DbResourceManager manager = null;
            ResourceManagers.TryGetValue(resourceSet, out manager);

            // if not we have to create it and add it to static collection
            if (manager == null)
            {

                lock (ResourceManagers)
                {
                    ResourceManagers.TryGetValue(resourceSet, out manager);
                    if (manager == null)
                    {
                        manager = new DbResourceManager(resourceSet);
                        ResourceManagers.Add(resourceSet, manager);
                    }
                }
            }

            return manager;
        }

        /// <summary>
        /// Returns a resource set for a given resource
        /// </summary>
        /// <param name="resourceSet">The name of the resource set to return.</param>
        /// <param name="lang">The language code (en-US,de-DE). Pass null to use the current ui culture</param>
        /// <returns></returns>
        public ResourceSet GetResourceSet(string resourceSet, string lang = null)
        {
            var manager = GetResourceManager(resourceSet);
            if (manager == null)
                return null;

            CultureInfo ci = null;
            if (lang == null)
                ci = CultureInfo.CurrentUICulture;
            else if (lang == string.Empty)
                ci = CultureInfo.InvariantCulture;
            else
                ci = new CultureInfo(lang);

            return manager.GetResourceSet(ci, false, true);
        }

        /// <summary>
        /// Clears resources from memory and forces reloading of all ResourceSets.
        /// Effectively unloads the ResourceManager and ResourceProvider.
        /// </summary>
        public void ClearResources()
        {
            lock (ResourceManagers)
            {
                ResourceManagers = new Dictionary<string, DbResourceManager>();
            }
        }

    }
}
