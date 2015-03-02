using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
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

        public const string STR_RESOURCESET = "localizationadmin/LocalizationAdmin.aspx";
        protected DbResourceDataManager Manager = DbResourceDataManager.CreateDbResourceDataManager();        

        public LocalizationService()
        {
            var ensureJsonNet = Formatting.Indented;            
            JSONSerializer.DefaultJsonParserType = SupportedJsonParserTypes.JsonNet;               
        }

        [CallbackMethod]
        public IEnumerable<ResourceIdItem> GetResourceList(string resourceSet)
        {
            var ids = Manager.GetAllResourceIds(resourceSet);
            if (ids == null)
                throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET,"ResourceSetLoadingFailed") + ":" + Manager.ErrorMessage);

            return ids;
        }

        [CallbackMethod]
        public IEnumerable<ResourceIdListItem> GetResourceListHtml(string resourceSet)
        {
            var ids = Manager.GetAllResourceIdListItems(resourceSet);
            if (ids == null)
                throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET,"ResourceSetLoadingFailed") + ":" + Manager.ErrorMessage);

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
                throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET,"LocaleIdsFailedToLoad") + ":" + Manager.ErrorMessage);

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
        public IEnumerable<ResourceItem> GetResourceItems(dynamic parm)
        {
            string resourceId = parm.ResourceId;
            string resourceSet = parm.ResourceSet;            

            
            var items = Manager.GetResourceItems(resourceId, resourceSet,true).ToList();
            if (items == null)
            {
                throw new InvalidOperationException(Manager.ErrorMessage);
                return null;
            }

            return items;
        }

        [CallbackMethod()]
        public ResourceItemEx GetResourceItem(dynamic parm)
        {
            string resourceId = parm.ResourceId;
            string resourceSet = parm.ResourceSet;
            string cultureName = parm.CultureName;

            var item = Manager.GetResourceItem(resourceId, resourceSet, "");
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
            Dictionary<string, string> resources = Manager.GetResourceStrings(resourceId, resourceSet,true);

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
                return Manager.DeleteResource(resourceId, resourceSet: resourceSet, cultureName: localeId);

            if (Manager.UpdateOrAddResource(resourceId, value, localeId, resourceSet, null) == -1)
                return false;

            return true;
        }

        /// <summary>
        /// Updates or Adds a resource if it doesn't exist
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [CallbackMethod]
        public bool UpdateResource(ResourceItem resource)
        {
            if (resource == null)
                throw new ArgumentException("No resource passed to add or update.");

            if (resource.Value == null)
            {
                return Manager.DeleteResource(resource.ResourceId,resourceSet: resource.ResourceSet, cultureName: resource.LocaleId);
            }

            int result =  Manager.UpdateOrAddResource(resource);
            if (result == -1)
                throw new InvalidOperationException(Manager.ErrorMessage);

            return true;
        }


        [CallbackMethod]
        public bool DeleteResource(dynamic parm)
        {

#if OnlineDemo        
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            string resourceId = parm.resourceId;
            string resourceSet = parm.resourceSet;
            string localeId = null;
            try
            {
                // localeId is optional
                localeId = parm.LocaleId;
            } catch{}

            if (!Manager.DeleteResource(resourceId,resourceSet, localeId))
                throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET, "ResourceUpdateFailed") + ": " + Manager.ErrorMessage);

            return true;
        }

        /// <summary>
        /// Renames a resource key to a new name.
        /// 
        /// Requires a JSON object with the following properties:
        /// {
        /// }
        /// 
        /// 
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [CallbackMethod]
        public bool RenameResource(dynamic parms)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            string resourceId =parms["resourceId"];
            string newResourceId = parms["newResourceId"];
            string resourceSet = parms["resourceSet"];

            
            if (!Manager.RenameResource(resourceId, newResourceId, resourceSet))
                throw new ApplicationException(WebUtils.GRes("localizationadmin/LocalizationAdmin.aspx","InvalidResourceId"));

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
                throw new ApplicationException(WebUtils.GRes("InvalidResourceId"));

            return true;
        }

        [CallbackMethod]
        public string Translate(dynamic parm)
        {
            string text = parm.text;
            string from = parm.from;
            string to = parm.to;
            string service = parm.service;

            service = service.ToLower();

            var translate = new TranslationServices();
            translate.TimeoutSeconds = 10;

            string result = null;
            if (service == "google")
                result = translate.TranslateGoogle(text, @from, to);
            else if (service == "bing")
            {
                if (string.IsNullOrEmpty(DbResourceConfiguration.Current.BingClientId))
                    result = ""; // don't do anything -  just return blank 
                else
                    result = translate.TranslateBing(text, @from, to);
            }

            if (result == null)
                result = translate.ErrorMessage;

            return result;
        }

        [CallbackMethod]
        public bool DeleteResourceSet(string resourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif

            if (!Manager.DeleteResourceSet(resourceSet))
                throw new ApplicationException(Manager.ErrorMessage);

            return true;
        }

        [CallbackMethod]
        public bool RenameResourceSet(string oldResourceSet, string newResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            if (!Manager.RenameResourceSet(oldResourceSet, newResourceSet))
                throw new ApplicationException(Manager.ErrorMessage);

            return true;
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
            throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            return Manager.CreateBackupTable(null);
        }

        [CallbackMethod]
        public bool CreateTable()
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif

            if (!Manager.CreateLocalizationTable(null))
                throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET, "LocalizationTableNotCreated") + "\r\n" +
                                                                              Manager.ErrorMessage);
            return true;
        }

        [CallbackMethod]
        public bool CreateClass()
        {
            var config = DbResourceConfiguration.Current;

            StronglyTypedResources strongTypes =
                new StronglyTypedResources(Context.Request.PhysicalApplicationPath);

            strongTypes.CreateClassFromAllDatabaseResources(config.ResourceBaseNamespace,
                HttpContext.Current.Server.MapPath(config.StronglyTypedGlobalResource));

            
            if (!string.IsNullOrEmpty(strongTypes.ErrorMessage))
                throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET, "StronglyTypedGlobalResourcesFailed"));

            return true;
        }

        [CallbackMethod]
        public bool ExportResxResources(string outputBasePath = null)
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            if (string.IsNullOrEmpty(outputBasePath))
                outputBasePath = DbResourceConfiguration.Current.ResxBaseFolder;

            if (outputBasePath.Contains("~"))
                outputBasePath = Context.Server.MapPath(outputBasePath);

            outputBasePath = outputBasePath.Replace("/", "\\").Replace("\\\\", "\\");


            DbResXConverter exporter = new DbResXConverter(outputBasePath);

            if (DbResourceConfiguration.Current.ResxExportProjectType == GlobalizationResxExportProjectTypes.WebForms)
            {
                if (!exporter.GenerateLocalWebResourceResXFiles())
                    throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET, "ResourceGenerationFailed"));
                if (!exporter.GenerateGlobalWebResourceResXFiles())
                    throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET, "ResourceGenerationFailed"));
            }
            else
            {
                if (!exporter.GenerateResXFiles())
                    throw new ApplicationException(WebUtils.GRes(STR_RESOURCESET, "ResourceGenerationFailed"));
            }

            return true;
        }

        [CallbackMethod]
        public bool ImportResxResources(string inputBasePath = null)
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif

            if (string.IsNullOrEmpty(inputBasePath))
                inputBasePath = DbResourceConfiguration.Current.ResxBaseFolder;

            if (inputBasePath.Contains("~"))
                inputBasePath = Context.Server.MapPath(inputBasePath);
            
            inputBasePath = inputBasePath.Replace("/", "\\").Replace("\\\\","\\");

            DbResXConverter converter = new DbResXConverter(inputBasePath);

            bool res = false;

            if (DbResourceConfiguration.Current.ResxExportProjectType == GlobalizationResxExportProjectTypes.WebForms)
                res = converter.ImportWebResources(inputBasePath);
            else
                res = converter.ImportWinResources(inputBasePath);

            if (!res)
               new ApplicationException(WebUtils.LRes("ResourceImportFailed"));

            return true;
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