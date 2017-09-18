#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2009-2015
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
using System.Collections;
using System.Collections.Generic;
using Westwind.Utilities.Data;

namespace Westwind.Globalization
{
    public interface IDbResourceDataManager
    {
        /// <summary>
        /// Instance of the DbResourceConfiguration that can be overridden
        /// Defaults to the default instance - DbResourceConfiguration.Current
        /// </summary>
        DbResourceConfiguration Configuration { get; set; }

        /// <summary>
        /// Error message that can be checked after a method complets
        /// and returns a failure result.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Creates an instance of the DataAccess Data provider
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        DataAccessBase GetDb(string connectionString = null);

        /// <summary>
        /// Returns a specific set of resources for a given culture and 'resource set' which
        /// in this case is just the virtual directory and culture.
        /// </summary>
        /// <param name="cultureName">name of the culture Id (de, de-de) to retrieve</param>
        /// <param name="resourceSet">Name of the resource set to retrieve</param>
        /// <returns></returns>
        IDictionary GetResourceSet(string cultureName, string resourceSet);

        /// <summary>
        /// Returns a fully normalized list of resources that contains the most specific
        /// locale version for the culture provided.
        ///                 
        /// This means that this routine walks the resource hierarchy and returns
        /// the most specific value in this order: de-ch, de, invariant.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        Dictionary<string, object> GetResourceSetNormalizedForLocaleId(string cultureName, string resourceSet);

        /// <summary>
        /// Returns a data table of all the resources for all locales. The result is in a 
        /// table called TResources that contains all fields of the table. The table is
        /// ordered by LocaleId.
        /// 
        /// This version returns either local or global resources in a Web app
        /// 
        /// Fields:
        /// ResourceId,Value,LocaleId,ResourceSet,Type
        /// </summary>
        /// <param name="localResources">return local resources if true</param>        
        /// <returns></returns>
        List<ResourceItem> GetAllResources(bool localResources = false, bool applyValueConverters = false, string resourceSet = null);

        /// <summary>
        /// Returns all available resource ids for a given resource set in all languages.
        /// 
        /// Returns a ResourceIdItem object with ResourecId and HasValue fields.
        /// HasValue returns whether there are any entries in any culture for this
        /// resourceId
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        List<ResourceIdItem> GetAllResourceIds(string resourceSet);

        /// <summary>
        /// Returns an DataTable called TResourceIds with ResourceId and HasValues fields
        /// where the ResourceId is formatted for HTML display.
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        List<ResourceIdListItem> GetAllResourceIdListItems(string resourceSet);

        /// <summary>
        /// Returns all available resource sets
        /// </summary>
        /// <returns></returns>
        List<string> GetAllResourceSets(ResourceListingTypes type);

        /// <summary>
        /// Gets all the locales for a specific resource set.
        /// 
        /// Returns a table named TLocaleIds (LocaleId field)
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        List<string> GetAllLocaleIds(string resourceSet);

        /// <summary>
        /// Gets all the Resourecs and ResourceIds for a given resource set and Locale
        /// 
        /// returns a table "TResource" ResourceId, Value fields
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        List<ResourceIdItem> GetAllResourcesForCulture(string resourceSet, string cultureName);

        /// <summary>
        /// Returns an individual Resource String from the database
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>       
        /// <param name="cultureName"></param>
        /// <returns></returns>
        string GetResourceString(string resourceId, string resourceSet, string cultureName);

        /// <summary>
        /// Returns an object from the Resources. Attempts to convert the object to its
        /// original type.  Use this for any non-string  types. Useful for binary resources
        /// like images, icons etc.
        /// 
        /// While this method can be used with strings, GetResourceString()
        /// is much more efficient.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>
        /// <param name="cultureName">required. Null or Empty culture returns invariant</param>
        /// <returns></returns>
        object GetResourceObject(string resourceId, string resourceSet, string cultureName);

        /// <summary>
        /// Returns a resource item that returns both the Value and Comment to the
        /// fields to the client.
        /// </summary>
        /// <param name="resourceId">The ID of the resource to retrieve</param>
        /// <param name="resourceSet">Name of the ResourceSet to return</param>
        /// <param name="cultureName">required. Null or Empty returns invariant</param>
        /// <returns></returns>
        ResourceItem GetResourceItem(string resourceId, string resourceSet, string cultureName);

        /// <summary>
        /// Returns all resource items for a given resource ID in all locales.
        /// Returned as full ResourceItem objects
        /// </summary>
        /// <param name="resourceId">The resource Id to return for</param>
        /// <param name="resourceSet">Resourceset to look in</param>
        /// <param name="forAllResourceSetLocales">When true returns empty entries for missing resources of locales in this resource set</param>
        /// <returns>List of resource items or null</returns>
        IEnumerable<ResourceItem> GetResourceItems(string resourceId, string resourceSet, bool forAllResourceSetLocales = false);

        /// <summary>
        /// Returns all the resource strings for all cultures for a specific resource Id.
        /// Returned as a dictionary.
        /// </summary>
        /// <param name="resourceId">Resource Id to retrieve strings for</param>
        /// <param name="resourceSet">Resource Set on which to retrieve strings</param>
        /// <param name="forAllResourceSetLocales">If true returns empty entries for each locale that exists but has no value in this resource set</param>
        /// <returns></returns>
        Dictionary<string, string> GetResourceStrings(string resourceId, string resourceSet, bool forAllResourceSetLocales = false);

        List<string> GetAllLocalesForResourceSet(string resourceSet);

        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resource">Resource to update</param>
        int UpdateOrAddResource(ResourceItem resource);

        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>        
        int UpdateOrAddResource(string resourceId, object value, string cultureName, string resourceSet,
            string comment = null, bool valueIsFileName = false, int valueType = 0);

        /// <summary>
        /// Adds a resource to the Localization Table
        /// </summary>
        /// <param name="resource">Resource to update</param>        
        int AddResource(ResourceItem resource);

        /// <summary>
        /// Adds a resource to the Localization Table
        /// </summary>
        /// <param name="resourceId">The resource key name</param>
        /// <param name="value">Value to set it to. Can also be a file which is loaded as binary data when valueIsFileName is true</param>
        /// <param name="cultureName">name of the culture or null for invariant/default</param>
        /// <param name="resourceSet">The ResourceSet to which the resource id is added</param>
        /// <param name="comment">Optional comment for the key</param>
        /// <param name="valueIsFileName">if true the Value property is a filename to import</param>
        int AddResource(string resourceId, object value,
            string cultureName, string resourceSet,
            string comment = null, bool valueIsFileName = false, int valueType = 0);

        /// <summary>
        /// Updates an existing resource in the Localization table
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>
        int UpdateResource(string resourceId, object value, 
            string cultureName, string resourceSet,
            string comment = null, bool valueIsFileName = false, int valueType=0);

        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resource">Resource to update</param>
        int UpdateResource(ResourceItem resource);

        /// <summary>
        /// Deletes a specific resource ID based on ResourceId, ResourceSet and Culture.
        /// If an empty culture is passed the entire group is removed (ie. all locales).
        /// </summary>
        /// <param name="resourceId">Resource Id to delete</param>
        /// <param name="resourceSet">The resource set to remove</param>
        /// <param name="cultureName">language ID - if empty all languages are deleted</param>
        /// e
        /// <returns></returns>
        bool DeleteResource(string resourceId, string resourceSet = null, string cultureName = null);

        /// <summary>
        /// Renames a given resource in a resource set. Note all languages will be renamed
        /// </summary>
        /// <param name="ResourceId"></param>
        /// <param name="NewResourceId"></param>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        bool RenameResource(string ResourceId, string NewResourceId, string ResourceSet);

        /// <summary>
        /// Renames all property keys for a given property prefix. So this routine updates
        /// lblName.Text, lblName.ToolTip to lblName2.Text, lblName2.ToolTip if the property
        /// is changed from lblName to lblName2.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="NewProperty"></param>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        bool RenameResourceProperty(string Property, string NewProperty, string ResourceSet);

        /// <summary>
        /// Deletes an entire resource set from the database. Careful with this function!
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        bool DeleteResourceSet(string ResourceSet, string cultureName = null);

        /// <summary>
        /// Renames a resource set. Useful if you need to move a local page resource set
        /// to a new page. ResourceSet naming for local resources is application relative page path:
        /// 
        /// test.aspx
        /// subdir/test.aspx
        /// 
        /// Global resources have a simple name
        /// </summary>
        /// <param name="OldResourceSet">Name of the existing resource set</param>
        /// <param name="NewResourceSet">Name to set the resourceset name to</param>
        /// <returns></returns>
        bool RenameResourceSet(string OldResourceSet, string NewResourceSet);

        /// <summary>
        /// Checks to see if a resource exists in the resource store
        /// </summary>
        /// <param name="ResourceId"></param>
        /// <param name="Value"></param>
        /// <param name="CultureName"></param>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        bool ResourceExists(string ResourceId, string CultureName, string ResourceSet);

        /// <summary>
        /// Returns true or false depending on whether the two letter country code exists
        /// </summary>
        /// <param name="IetfTag">two or four letter IETF tag (examples: de, de-DE,fr,fr-CA)</param>
        /// <returns>true or false</returns>
        bool IsValidCulture(string IetfTag);

        /// <summary>
        /// Persists resources to the database - first wipes out all resources, then writes them back in
        /// from the ResourceSet
        /// </summary>
        /// <param name="resourceList"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        bool GenerateResources(IDictionary resourceList, string cultureName, string resourceSet, bool deleteAllResourceFirst);

        /// <summary>
        /// Creates an global JavaScript object object that holds all non-control 
        /// local string resources as property values.
        /// 
        /// All resources are returned in normalized fashion from most specifc
        /// to more generic (de-ch,de,invariant depending on availability)
        /// </summary>
        /// <param name="javaScriptVarName">Name of the JS object variable to createBackupTable</param>
        /// <param name="resourceSet">ResourceSet name. Pass NULL for locale Resources</param>
        /// <param name="localeId"></param>
        string GetResourcesAsJavascriptObject(string javaScriptVarName, string resourceSet, string localeId);

        /// <summary>
        /// Checks to see if the LocalizationTable exists
        /// </summary>
        /// <param name="tableName">Table name or the configuration.ResourceTableName if not passed</param>
        /// <returns></returns>
        bool IsLocalizationTable(string tableName = null);

        /// <summary>
        /// Create a backup of the localization database.
        /// 
        /// Note the table used is the one specified in the Configuration.ResourceTableName
        /// </summary>
        /// <param name="BackupTableName">Table of the backup table. Null creates a _Backup table.</param>
        /// <returns></returns>
        bool CreateBackupTable(string BackupTableName);

        /// <summary>
        /// Restores the localization table from a backup table by first wiping out 
        /// </summary>
        /// <param name="backupTableName"></param>
        /// <returns></returns>
        bool RestoreBackupTable(string backupTableName);

        /// <summary>
        /// Creates the Localization table on the current connection string for
        /// the provider.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool CreateLocalizationTable(string tableName = null);

        void SetError();
        void SetError(string message);
        void SetError(Exception ex);
    }
}