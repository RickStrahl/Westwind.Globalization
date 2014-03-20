using System.Collections.Generic;
using System.Globalization;

namespace Westwind.Globalization
{

/// <summary>
/// Helper class that handles access to the dbResourceManager
/// more easily with single method access.
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
    /// Internal dictionary that
    /// </summary>
    static Dictionary<string, DbResourceManager> ResourceManagers = new Dictionary<string, DbResourceManager>();

    /// <summary>
    /// Determines whether resources that fail in a lookup are automatically
    /// added to the resource table
    /// </summary>
    public static bool AutoAddResources { get; set; }

    /// <summary>
    /// Localization function
    /// </summary>
    /// <param name="resId"></param>
    /// <param name="resourceSet"></param>
    /// <param name="lang">Language as ieetf code: en-US, de-DE etc.</param>
    /// <returns></returns>
    public static string T(string resId, string resourceSet = null, string lang = null, bool autoAdd = false)
    {
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
                    if (manager != null)
                    {                                              
                        ResourceManagers.Add(resourceSet, manager);
                    }
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
        string result = manager.GetObject(resId, ci) as string;

        if (string.IsNullOrEmpty(result))
            return resId;

        return result;
    }

    /// <summary>
    /// Writes a resource either creating or updating an existing resource 
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="value"></param>
    /// <param name="lang"></param>
    /// <param name="resourceSet"></param>
    /// <returns></returns>
    public static bool WriteResource(string resourceId, string value = null, string lang = null, string resourceSet = null)
    {
        if (lang == null)
            lang = string.Empty;
        if (resourceSet == null)
            resourceSet = string.Empty;
        if (value == null)
            value = resourceId;

        var db = new DbResourceDataManager();
        return db.UpdateOrAdd(resourceId, value, lang, resourceSet, null) > -1;
    }

    /// <summary>
    /// Deletes a resource entry
    /// </summary>
    /// <param name="resourceId">The resource to delete</param>
    /// <param name="lang">The language Id - if empty or null deletes all languages</param>
    /// <param name="resourceSet">The resource set to apply</param>
    /// <returns></returns>
    public static bool DeleteResource(string resourceId,  string resourceSet = null, string lang = null)
    {
        var db = new DbResourceDataManager();
        return db.DeleteResource(resourceId, lang, resourceSet);
    }

    /// <summary>
    /// Clears resources from memory and forces reloading
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