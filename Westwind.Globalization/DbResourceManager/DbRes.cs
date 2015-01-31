using System.Collections.Generic;
using System.Globalization;

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
    public class DbRes
    {
        /// <summary>
        /// Internal dictionary that holds instances of resource managers
        /// for each resourceset defined in the application. Lazy loaded
        /// as resources are accessed.
        /// </summary>
        private static Dictionary<string, DbResourceManager> ResourceManagers =
            new Dictionary<string, DbResourceManager>();

        /// <summary>
        /// Determines whether resources that fail in a lookup are automatically
        /// added to the resource table
        /// </summary>
        public static bool AutoAddResources { get; set; }

        static DbRes()
        {
            AutoAddResources = DbResourceConfiguration.Current.AddMissingResources;
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
        public static string T(string resId, string resourceSet = null, string lang = null)
        {
            if (string.IsNullOrEmpty(resId))
                return resId;

            if (resourceSet == null)
                resourceSet = string.Empty;

            // check if the res manager exists
            DbResourceManager manager;
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
                        manager.AutoAddMissingEntries = AutoAddResources;
                    }
                }
            }

            // no manager no resources
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
        public static object TO(string resId, string resourceSet = null, string lang = null, bool autoAdd = false)
        {
            if (string.IsNullOrEmpty(resId))
                return resId;

            if (resourceSet == null)
                resourceSet = string.Empty;

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

            // no manager no resources
            if (manager == null)
                return resId;

            CultureInfo ci = null;
            if (string.IsNullOrEmpty(lang))
                ci = CultureInfo.CurrentUICulture;
            else
                ci = new CultureInfo(lang);

            manager.AutoAddMissingEntries = AutoAddResources;
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
        public static bool WriteResource(string resourceId, string value = null, string lang = null,
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
        public static bool DeleteResource(string resourceId, string resourceSet = null, string lang = null)
        {
            var db = DbResourceDataManager.CreateDbResourceDataManager();  
            return db.DeleteResource(resourceId, lang, resourceSet);
        }

        /// <summary>
        /// Clears resources from memory and forces reloading of all ResourceSets.
        /// Effectively unloads the ResourceManager and ResourceProvider.
        /// </summary>
        public static void ClearResources()
        {
            lock (ResourceManagers)
            {
                ResourceManagers = new Dictionary<string, DbResourceManager>();
            }
        }

    }
}