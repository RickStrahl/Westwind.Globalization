using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Westwind.Utilities;
using Westwind.Web;
using Westwind.Web.JsonSerializers;

namespace Westwind.Globalization.Sample.LocalizationAdministration
{
    /// <summary>
    /// Localization form Admin service that provides JSON data for 
    /// the admin interface.
    /// </summary>
    public class LocalizationService : CallbackHandler
    {
        
        protected DbResourceDataManager Manager = DbResourceDataManager.CreateDbResourceDataManager();

        public LocalizationService()
        {
            JSONSerializer.DefaultJsonParserType = SupportedJsonParserTypes.JsonNet;
        }

        [CallbackMethod]
        public IEnumerable<ResourceIdItem> GetResourceList(string resourceSet)
        {
            var ids = Manager.GetAllResourceIds(resourceSet);
            if (ids == null)
                throw new ApplicationException(WebUtils.LRes("ResourceSetLoadingFailed") + ":" + Manager.ErrorMessage);

            return ids;
        }

        [CallbackMethod]
        public IEnumerable<ResourceIdListItem> GetResourceListHtml(string resourceSet)
        {
            var ids = Manager.GetAllResourceIdListItems(resourceSet);
            if (ids == null)
                throw new ApplicationException(WebUtils.LRes("ResourceSetLoadingFailed") + ":" + Manager.ErrorMessage);

            return ids;
        }

        [CallbackMethod]
        public IEnumerable<string> GetResourceSets()
        {
            return Manager.GetAllResourceSets(ResourceListingTypes.AllResources);
        }

        [CallbackMethod]
        public IEnumerable<object> GetAllLocaleIds(string resourceSet)
        {
            var ids =  Manager.GetAllLocaleIds(resourceSet);
            if (ids == null)
                throw new ApplicationException(WebUtils.LRes("LocaleIdsFailedToLoad") + ":" + Manager.ErrorMessage);

            var list = new List<object>();

            foreach (string localeId in ids)
            {
                CultureInfo ci = CultureInfo.GetCultureInfo(localeId.Trim());

                string language = "Invariant";
                if (!string.IsNullOrEmpty(localeId))
                    language = ci.DisplayName + " (" + ci.Name + ")";
                list.Add(new {LocaleId = localeId, Name = language});
            }

            return list;
        }

        [CallbackMethod]
        public string GetResourceString(dynamic parm)
        {            
            string resourceId = parm.ResourceId;
            string resourceSet = parm.ResourceSet;
            string cultureName = parm.CultureName;
            string value = Manager.GetResourceString(resourceId,
                resourceSet, cultureName); 
                                

            if (value == null && !string.IsNullOrEmpty(Manager.ErrorMessage))
                throw new ArgumentException(Manager.ErrorMessage);

            return value;
        }

        [CallbackMethod()]
        public ResourceItemEx GetResourceItem(dynamic parm)
        {
            string resourceId = parm.ResourceId;
            string resourceSet = parm.ResourceSet;
            string cultureName = parm.CultureName;

            var item = Manager.GetResourceItem(resourceId, resourceSet, cultureName);
            if (item == null)
                throw new ArgumentException(Manager.ErrorMessage);

            var itemEx = new ResourceItemEx(item);
            itemEx.ResourceList = GetResourceStrings(resourceId, resourceSet).ToList();            

            return itemEx;
        }

        /// <summary>
        /// Gets all resources for a given ResourceId for all cultures from
        /// a resource set.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>
        /// <returns>Returns an array of Key/Value objects to the client</returns>
        [CallbackMethod]
        public IEnumerable<ResourceString> GetResourceStrings(string resourceId, string resourceSet)
        {
            Dictionary<string, string> resources = Manager.GetResourceStrings(resourceId, resourceSet);

            if (resources == null)
                throw new ApplicationException(Manager.ErrorMessage);

            // transform into an array
            return resources.Select(kv => new ResourceString
            {
                LocaleId = kv.Key, Value = kv.Value
            });            
        }

        [CallbackMethod]
        public bool UpdateResourceString(dynamic parm)
        {
            string value = parm.value;
            string resourceId = parm.resourceId;
            string resourceSet = parm.resourceSet;
            string localeId = parm.localeId;

            if (string.IsNullOrEmpty(value))
                return Manager.DeleteResource(resourceId, localeId, resourceSet);

            if (Manager.UpdateOrAddResource(resourceId, value, localeId, resourceSet, null) == -1)
                return false;

            return true;
        }

        [CallbackMethod]
        public bool UpdateResourceWithComment(string value, string comment, string resourceId, string resourceSet, string localeId)
        {
            if (string.IsNullOrEmpty(value))
                return Manager.DeleteResource(resourceId, localeId, resourceSet);

            if (Manager.UpdateOrAddResource(resourceId, value, localeId, resourceSet, comment) == -1)
                return false;

            return true;
        }


        [CallbackMethod]
        public bool DeleteResource(string resourceId, string resourceSet, string localeId)
        {

#if OnlineDemo        
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            if (!Manager.DeleteResource(resourceId, localeId, resourceSet))
                throw new ApplicationException(WebUtils.LRes("ResourceUpdateFailed") + ": " + Manager.ErrorMessage);

            return true;
        }

        [CallbackMethod]
        public bool RenameResource(string ResourceId, string NewResourceId, string ResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            if (!Manager.RenameResource(ResourceId, NewResourceId, ResourceSet))
                throw new ApplicationException(WebUtils.LRes("InvalidResourceId"));

            return true;
        }

        /// <summary>
        /// Renames all resource keys that match a property (ie. lblName.Text, lblName.ToolTip)
        /// at once. This is useful if you decide to rename a meta:resourcekey in the ASP.NET
        /// markup.
        /// </summary>
        /// <param name="Property">Original property prefix</param>
        /// <param name="NewProperty">New Property prefix</param>
        /// <param name="ResourceSet">The resourceset it applies to</param>
        /// <returns></returns>
        [CallbackMethod]
        public bool RenameResourceProperty(string Property, string NewProperty, string ResourceSet)
        {
            if (!Manager.RenameResourceProperty(Property, NewProperty, ResourceSet))
                throw new ApplicationException(WebUtils.LRes("InvalidResourceId"));

            return true;
        }

        [CallbackMethod]
        public string Translate(string Text, string From, string To, string Service)
        {
            Service = Service.ToLower();

            var translate = new TranslationServices();
            translate.TimeoutSeconds = 10;

            string result = null;
            if (Service == "google")
                result = translate.TranslateGoogle(Text, From, To);
            else if (Service == "bing")
            {
                if (string.IsNullOrEmpty(DbResourceConfiguration.Current.BingClientId))
                    result = ""; // don't do anything -  just return blank 
                else
                    result = translate.TranslateBing(Text, From, To);
            }

            if (result == null)
                result = translate.ErrorMessage;

            return result;
        }

        [CallbackMethod]
        public bool DeleteResourceSet(string ResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            return Manager.DeleteResourceSet(ResourceSet);
        }

        [CallbackMethod]
        public bool RenameResourceSet(string OldResourceSet, string NewResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            return Manager.RenameResourceSet(OldResourceSet, NewResourceSet);
        }

        [CallbackMethod]
        public void ReloadResources()
        {
            //Westwind.Globalization.Tools.wwWebUtils.RestartWebApplication();
            DbResourceConfiguration.ClearResourceCache(); // resource provider
            DbRes.ClearResources();  // resource manager
        }

        [CallbackMethod]
        public bool Backup()
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif
            return Manager.CreateBackupTable(null);
        }

    }

    public class ResourceString
    {
        public string LocaleId { get; set; }
        public string Value { get; set; }
    }
    public class ResourceItemEx : ResourceItem
    {
        public ResourceItemEx()
        {
            
        }
        public ResourceItemEx(ResourceItem item)
        {
            ResourceId = item.ResourceId;
            LocaleId = item.LocaleId;
            Value = item.Value;
            ResourceSet = item.ResourceSet;
            Type = item.Type;
            FileName = item.FileName;
            TextFile = item.TextFile;
            BinFile = item.BinFile;
            Comment = item.Comment;

        }
        public List<ResourceString> ResourceList { get; set; }
    }
}