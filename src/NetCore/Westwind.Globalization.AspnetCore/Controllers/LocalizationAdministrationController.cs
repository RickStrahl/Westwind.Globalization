using System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Westwind.Globalization.AspNetCore.Extensions;
using Westwind.Globalization.Errors;
using Westwind.Globalization.Utilities;
using Westwind.Utilities;

namespace Westwind.Globalization.Administration
{
    /// <summary>
    /// Handles the Administration requires from the Localization Admin
    /// Form. This service is self contained.
    /// </summary>
    [Route("api/LocalizationAdministration")]
    [UnhandledApiExceptionFilter]        
    public class LocalizationAdministrationController : Controller
    {
        public const string STR_RESOURCESET = "LocalizationForm";

        protected DbResourceDataManager Manager;
        protected Formatting EnsureJsonNet = Formatting.Indented;
        protected IHostingEnvironment Host;
        protected DbResourceConfiguration Config;

        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver()  // no proper case Results for backwards compatibilty
        };

        DbResInstance DbIRes { get;  }


        public LocalizationAdministrationController(IHostingEnvironment host, DbResourceConfiguration config)
        {
            Host = host;
            Config = config;
            DbIRes = new DbResInstance(config);
            Manager = DbResourceDataManager.CreateDbResourceDataManager(config.DbResourceDataManagerType);
        }

       /// <summary>
       /// Handle custom authorization and hook into Config.OnAuthorizeLocalizationAdministration
       /// </summary>
       /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            if (DbIRes.Configuration.OnAuthorizeLocalizationAdministration != null)
            {
                var func = Config.OnAuthorizeLocalizationAdministration as Func<ActionExecutingContext, bool>;
                if  (func != null)
                {                                     
                    if (!func.Invoke(context))                    
                        throw new UnauthorizedAccessException();                    
                }
            }
        }

        #region Retrieve Resources

        [HttpGet]
        [Route("GetResourceList")]
        //public IEnumerable<ResourceIdItem> GetResourceList(string resourceSet)
        public ActionResult GetResourceList(string resourceSet)
        {
            var ids = Manager.GetAllResourceIds(resourceSet);
            if (ids == null)
                throw new ApplicationException(DbIRes.T("ResourceSetLoadingFailed", STR_RESOURCESET) + ":" +
                                               Manager.ErrorMessage);

            
            //return ids;
            return Json(ids, jsonSettings);
        }


   
        /// <summary>
        /// Returns a shaped objects that can be displayed in an editable grid the grid view for locale ids
        /// of resources.
        /// 
        //{
        //  "Locales": [
        //    "",
        //    "de",
        //    "fr"
        //  ],
        //  "Resources": [
        //    {
        //      "ResourceId": "AddressIsRequired",
        //      "Resources": [
        //        {
        //          "ResourceId": "AddressIsRequired",
        //          "LocaleId": "",
        //          "ResourceSet": "Resources",
        //          "Value": "An address is required."
        //        },
        //        {
        //          "ResourceId": "AddressIsRequired",
        //          "LocaleId": "de",
        //          "ResourceSet": "Resources",
        //          "Value": "Eine Addresse muss angegeben werden."
        //        },
        //        {
        //          "ResourceId": "AddressIsRequired",
        //          "LocaleId": "fr",
        //          "ResourceSet": "Resources",
        //          "Value": "Une adresse doit Ãªtre saisi."
        //        }
        //      ]
        //    },
        //    {}
        //]
        //}
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllResourcesForResourceGrid")]
        public JsonResult GetAllResourcesForResourceGrid(string resourceSet)
        //public object GetAllResourcesForResourceGrid(string resourceSet)
        {
            var items = Manager.GetAllResources(resourceSet: resourceSet);

            if (items == null)
                throw new ApplicationException(Manager.ErrorMessage);

            // reorder and reshape the data
            var itemList = items
                .OrderBy(it => it.ResourceId + "_" + it.LocaleId)
                .Select(it => new BasicResourceItem()
                {
                    ResourceId = it.ResourceId,
                    LocaleId = it.LocaleId,
                    ResourceSet = it.ResourceSet,
                    Value = it.Value as string
                }).ToList();

            var totalLocales = itemList.GroupBy(it => it.LocaleId).Select(it => it.Key).ToList();

            foreach (var item in itemList.GroupBy(it => it.ResourceId))
            {
                string resid = item.Key;
                var resItems = itemList.Where(it => it.ResourceId == resid).ToList();
                if (resItems.Count < totalLocales.Count)
                {
                    foreach (string locale in totalLocales)
                    {
                        if (!resItems.Any(ri => ri.LocaleId == locale))
                        {
                            itemList.Add(new BasicResourceItem
                            {
                                ResourceId = resid,
                                LocaleId = locale,
                                ResourceSet = resourceSet                                
                            });
                        }
                    }
                }
            }
            itemList = itemList.OrderBy(it => it.ResourceId + "_" + it.LocaleId).ToList();

            var resultList = new List<object>();
            foreach (var item in itemList.GroupBy(it => it.ResourceId))
            {
                var resId = item.Key;
                var newItem = new
                {
                    ResourceId = resId,
                    Resources = itemList
                        .Where(it => it.ResourceId == resId)
                        .OrderBy(it => it.LocaleId)
                };
                resultList.Add(newItem);
            }

            // final projection
            var result = new
            {
                ResourceSet = resourceSet,
                Locales = totalLocales,
                Resources = resultList
            };

            return Json(result, jsonSettings);            
        }


        [HttpGet]
        [Route("GetResourceListHtml")]
        public JsonResult GetResourceListHtml(string resourceSet)
        //public IEnumerable<ResourceIdListItem> GetResourceListHtml(string resourceSet)
        {
            var ids = Manager.GetAllResourceIdListItems(resourceSet);
            if (ids == null)
                throw new ApplicationException(DbIRes.T("ResourceSetLoadingFailed", STR_RESOURCESET) + ":" +
                                               Manager.ErrorMessage);

            //return ids;
            return Json(ids, jsonSettings);
        }

        /// <summary>
        /// Returns a list of all ResourceSets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetResourceSets")]
        public JsonResult GetResourceSets()
        //public IEnumerable<string> GetResourceSets()
        {
            return Json(Manager.GetAllResourceSets(ResourceListingTypes.AllResources), jsonSettings);
        }


        /// <summary>
        /// Returns a list of the all the LocaleIds used in a given resource set
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>        
        [Route("GetLocaleIds")]
        public JsonResult GetAllLocaleIds(string resourceSet)
        // public IEnumerable<object> GetAllLocaleIds(string resourceSet)
        {
            var ids = Manager.GetAllLocaleIds(resourceSet);
            if (ids == null)
                throw new ApplicationException(DbIRes.T("LocaleIdsFailedToLoad", STR_RESOURCESET) + ":" +
                                               Manager.ErrorMessage);

            var list = new List<object>();

            foreach (string localeId in ids)
            {
                CultureInfo ci = CultureInfo.GetCultureInfo(localeId.Trim());

                string language = "Invariant";
                if (!string.IsNullOrEmpty(localeId))
                    language = ci.DisplayName + " (" + ci.Name + ")";
                list.Add(new { LocaleId = localeId, Name = language });
            }

            return Json(list,jsonSettings);
        }


        /// <summary>
        /// Returns a resource string based on  resourceId, resourceSet,cultureName
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetResourceString")]
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

        /// <summary>
        /// Returns all resources for a given Resource ID. Pass resourceId, and resourceSet
        /// in an object map.
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetResourceItems")]
        public JsonResult GetResourceItems([FromBody] dynamic parm)
        //public IEnumerable<ResourceItemEx> GetResourceItems([FromBody] dynamic  parm)
        {
            string resourceSet = parm.ResourceSet;
            string resourceId = parm.ResourceId;            
            
            var items = Manager.GetResourceItems(resourceId, resourceSet, true).ToList();
            var itemList = new List<ResourceItemEx>();

            // strip file data for size
            for (int i = 0; i < items.Count; i++)
            {
                var item = new ResourceItemEx(items[i]);
                item.BinFile = null;
                item.TextFile = null;
                itemList.Add(item);
            }

            return Json(itemList,jsonSettings);
        }

        /// <summary>
        /// Returns an individual ResourceIdtem for a resource ID and specific culture.
        /// pass resourceId, resourceSet, cultureName in an object map.
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetResourceItem")]
        public JsonResult GetResourceItem([FromBody] dynamic parm)
        //public ResourceItemEx GetResourceItem([FromBody] dynamic parm)
        {
            string resourceId = parm.ResourceId;
            string resourceSet = parm.ResourceSet;
            string cultureName = parm.CultureName;

            var item = Manager.GetResourceItem(resourceId, resourceSet, "");
            if (item == null)
                throw new ArgumentException(Manager.ErrorMessage);

            var itemEx = new ResourceItemEx(item);
            itemEx.ResourceList = GetResourceStringsInternal(resourceId, resourceSet).ToList();

            return Json(itemEx, jsonSettings);
        }

        /// <summary>
        /// Gets all resources for a given ResourceId for all cultures from
        /// a resource set.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>
        /// <returns>Returns an array of Key/Value objects to the client</returns>
        [Route("GetResourceStrings")]
        public ActionResult GetResourceStrings(string resourceId, string resourceSet)
        {
            var res = GetResourceStringsInternal(resourceId, resourceSet);

            return Json(res, jsonSettings);
        }


        protected IEnumerable<ResourceString> GetResourceStringsInternal(string resourceId, string resourceSet)        
        {
            Dictionary<string, string> resources = Manager.GetResourceStrings(resourceId, resourceSet, true);

            if (resources == null)
                throw new ApplicationException(Manager.ErrorMessage);

            // transform into an array
            return resources.Select(kv => new ResourceString
            {
                LocaleId = kv.Key,
                Value = kv.Value
            });
        }
        #endregion

        #region Update Resources

        /// <summary>
        /// Adds or updates a resource. Pass value, resourceId,resourceSet,localeId,comment
        /// as a map.
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateResourceString")]
        public bool UpdateResourceString([FromBody] dynamic parm)
        {
            string value = parm.value;
            string resourceId = parm.resourceId;
            string resourceSet = parm.resourceSet;
            string localeId = parm.localeId;
            string comment = parm.comment;

            var item = Manager.GetResourceItem(resourceId, resourceSet, localeId);
            if (item == null)
            {
                item = new ResourceItem()
                {
                    ResourceId = resourceId,
                    LocaleId = localeId,
                    ResourceSet = resourceSet,
                    Comment = comment
                };
            }

            if (string.IsNullOrEmpty(value))
                return Manager.DeleteResource(resourceId, resourceSet: resourceSet, cultureName: localeId);

            item.Value = value;
            item.Type = null;
            item.FileName = null;
            item.BinFile = null;
            item.TextFile = null;

            if (Manager.UpdateOrAddResource(item) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Updates just a comment for an individual resourceId. Pass resourceId, resourceSet and localeId
        /// in a map.
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateComment")]
        public bool UpdateComment([FromBody] dynamic parm)
        {
            string comment = parm.comment;
            string resourceId = parm.resourceId;
            string resourceSet = parm.resourceSet;
            string localeId = parm.localeId;

            var item = Manager.GetResourceItem(resourceId, resourceSet, localeId);
            if (item == null)
            {
                // can't update a comment on non-existing resource
                return false;
            }
            item.Comment = comment;

            if (Manager.UpdateOrAddResource(item) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Updates or Adds a resource if it doesn't exist
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateResource")]
        public bool UpdateResource([FromBody] ResourceItem resource)
        {
            if (resource == null)
                throw new ArgumentException("NoResourcePassedToAddOrUpdate");

            if (resource.Value == null)
            {
                return Manager.DeleteResource(resource.ResourceId, resourceSet: resource.ResourceSet,
                    cultureName: resource.LocaleId);
            }

            int result = Manager.UpdateOrAddResource(resource);
            if (result == -1)
                throw new InvalidOperationException(Manager.ErrorMessage);

            return true;
        }


        /// <summary>
        /// Updates or adds a binary file resource based on form variables.
        /// ResourceId,ResourceSet,LocaleId and a single file upload.
        /// </summary>
        /// <returns></returns>
        [Route("UploadResource")]
        public bool UploadResource()
        {
            if (Request.Form.Files.Count < 1)
                return false;

            var file = Request.Form.Files[0];
            var resourceId = Request.Form["ResourceId"];
            var resourceSet = Request.Form["ResourceSet"];
            var localeId = Request.Form["LocaleId"];

            

            if (string.IsNullOrEmpty(resourceId) || string.IsNullOrEmpty(resourceSet))
                throw new ApplicationException("Resourceset or ResourceId are not provided for upload.");

            var item = Manager.GetResourceItem(resourceId, resourceSet, localeId);
            if (item == null)
            {
                item = new ResourceItem()
                {
                    ResourceId = resourceId,
                    ResourceSet = resourceSet,
                    LocaleId = localeId,
                    ValueType = (int)ValueTypes.Binary
                };
            }

            using (var fs = file.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                fs.CopyTo(ms);                
                ms.Flush();

                if (DbResourceDataManager.SetFileDataOnResourceItem(item, ms.ToArray(), file.FileName) == null)
                    return false;

                int res = Manager.UpdateOrAddResource(item);
                if (res < 0)                    
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Deletes an entire resource set.
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpDelete]
        [Route("DeleteResourceSet")]
        public bool DeleteResourceSet(string resourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            if (!Manager.DeleteResourceSet(resourceSet))
                throw new ApplicationException(Manager.ErrorMessage);

            return true;
        }

        /// <summary>
        /// Delete an individual resource. Pass resourceId, resourceSet and localeId 
        /// as a map. If localeId is null all the resources are deleted.
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteResource")]
        public bool DeleteResource([FromBody] dynamic parm)
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
            }
            catch
            {
            }

            if (!Manager.DeleteResource(resourceId, resourceSet, localeId))
                throw new ApplicationException(DbIRes.T("ResourceUpdateFailed", STR_RESOURCESET) + ": " +
                                               Manager.ErrorMessage);

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
        [HttpPost]
        [Route("RenameResource")]
        public bool RenameResource([FromBody] dynamic parms)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            string resourceId = parms["resourceId"];
            string newResourceId = parms["newResourceId"];
            string resourceSet = parms["resourceSet"];


            if (!Manager.RenameResource(resourceId, newResourceId, resourceSet))
                throw new ApplicationException(DbIRes.T("InvalidResourceId", STR_RESOURCESET));

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
        [Route("RenameResource")]
        public bool RenameResourceProperty(string Property, string NewProperty, string ResourceSet)
        {
            if (!Manager.RenameResourceProperty(Property, NewProperty, ResourceSet))
                throw new ApplicationException(DbIRes.T("InvalidResourceId", STR_RESOURCESET));

            return true;
        }

        /// <summary>
        /// Renames a resource set to a new name.
        /// </summary>
        /// <param name="oldResourceSet"></param>
        /// <param name="newResourceSet"></param>
        /// <returns></returns>
        [Route("RenameResourceSet")]
        public bool RenameResourceSet(string oldResourceSet, string newResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            if (!Manager.RenameResourceSet(oldResourceSet, newResourceSet))
                throw new ApplicationException(Manager.ErrorMessage);

            return true;
        }

        #endregion

        #region Table Administration

        /// <summary>
        /// Creates a new localization table as defined int he configuration if it doesn't
        /// exist. If the table exists an error is returned.
        /// </summary>
        /// <returns></returns>
        [Route("CreateTable")]
        public bool CreateTable()
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif

            if (!Manager.CreateLocalizationTable(null))
                throw new ApplicationException(DbIRes.T("LocalizationTableNotCreated", STR_RESOURCESET) + "\r\n" +
                                               Manager.ErrorMessage);
            return true;
        }

        /// <summary>
        /// Backs up the resource table into a new table with the same name + _backup
        /// </summary>
        /// <returns></returns>
        [Route("Backup")]
        public bool Backup()
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            return Manager.CreateBackupTable(null);
        }

        /// <summary>
        /// Clears the resource cache. Works only if using one of the Westwind
        /// ASP.NET resource providers or managers.
        /// </summary>
        [Route("ReloadResources")]
        public void ReloadResources()
        {
            //Westwind.Globalization.Tools.wwWebUtils.RestartWebApplication();
            DbResourceConfiguration.ClearResourceCache(); // resource provider
            DbIRes.ClearResources(); // resource manager
        }

        #endregion


        #region Miscellaneous helpers
        [HttpPost]
        [Route("Translate")]        
        public string Translate([FromBody] dynamic parm)
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
                    result = translate.TranslateBing(text, from, to);
            }
            else if (service == "deepl")
            {
                result = translate.TranslateDeepL(text, from, to);
            }
            
            if (result == null)
                result = translate.ErrorMessage;

            return result;
        }

        /// <summary>
        /// checks to see if the localiztion table exists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("IsLocalizationTable")]
        public bool IsLocalizationTable()
        {
            return Manager.IsLocalizationTable();
        }


        /// <summary>
        /// Determines whether a locale is an RTL language
        /// </summary>
        /// <param name="localeId"></param>
        /// <returns></returns>
        [Route("IsRtl")]
        public bool IsRtl(string localeId)
        {
            try
            {
                var li = localeId;
                if (string.IsNullOrEmpty(localeId))
                    li = CultureInfo.InstalledUICulture.IetfLanguageTag;

                var ci = CultureInfo.GetCultureInfoByIetfLanguageTag(localeId);
                return ci.TextInfo.IsRightToLeft;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Small helper that turns a camel case string into
        /// space separated string.
        /// </summary>
        /// <param name="camelCaseString"></param>
        /// <returns></returns>
        [Route("FromCamelCase")]
        public string FromCamelCase([FromBody] string camelCaseString)
        {
            return StringUtils.FromCamelCase(camelCaseString);
        }
        /// <summary>
        /// Returns configuration information so the UI can display this info on the configuration
        /// page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLocalizationInfo")]
        //public object GetLocalizationInfo()
        public ActionResult GetLocalizationInfo()
        {
            //// Get the Web application configuration object.
            //var webConfig = WebConfigurationManager.OpenWebConfiguration("~/web.config");

            //// Get the section related object.
            //GlobalizationSection configSection =
            //    (GlobalizationSection)webConfig.GetSection("system.web/globalization");

            //string providerFactory = configSection.ResourceProviderFactoryType;
            //if (string.IsNullOrEmpty(providerFactory))
            //    providerFactory = WebUtils.GRes(STR_RESOURCESET, "NoProviderConfigured");

            var config = DbResourceConfiguration.Current;


            var rt = typeof(IHostingEnvironment)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>();
            var v = new Version(rt.Version);

            return Json(new
            {
                //ProviderFactory = providerFactory,
                config.ConnectionString,
                config.ResourceTableName,
                DbResourceProviderType = config.DbResourceDataManagerType.Name,
                DataProvider = config.DataProvider.ToString(),
                config.ResxExportProjectType,
                config.ResxBaseFolder,
                config.ResourceBaseNamespace,
                config.StronglyTypedGlobalResource,
                config.GoogleApiKey,
                config.BingClientId,
                config.AddMissingResources,
                ResourceAccessMode = config.ResourceAccessMode.ToString(),

                Os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                AspNetVersion = v.ToString()
                
            },jsonSettings);
        }

        #endregion


        #region Import/Export

        /// <summary>
        /// Creates .NET strongly typed class from the resources. Pass:
        /// fileName, namespace, classType, resourceSets as a map.
        /// </summary>
        /// <remarks>
        /// Requires that the application has rights to write output into
        /// the location specified by the filename.
        /// </remarks>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateClass")]
        public bool CreateClass([FromBody] dynamic parms)
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            var config = DbResourceConfiguration.Current;

            // { filename: "~/properties/resources.cs, nameSpace: "WebApp1", resourceSets: ["rs1","rs2"],classType: "DbRes|Resx"]
            string filename = parms["fileName"];
            string nameSpace = parms["namespace"];
            string classType = parms["classType"];
            JArray rs = parms["resourceSets"] as JArray;

            string[] resourceSets = null;
            if (rs != null)
            {
                resourceSets = rs.ToObject<string[]>();
                if (resourceSets != null && resourceSets.Length == 1 && string.IsNullOrEmpty(resourceSets[0]))
                    resourceSets = null;
            }


            StronglyTypedResources strongTypes =
                new StronglyTypedResources(Request.MapPath("~/", basePath: Host.ContentRootPath));

            if (string.IsNullOrEmpty(filename))
                filename = Request.MapPath(config.StronglyTypedGlobalResource, basePath: Host.ContentRootPath);

            else if (filename.StartsWith("~"))
                filename = Request.MapPath(filename, basePath: Host.ContentRootPath);

            filename = DbResourceUtils.NormalizePath(filename);

            if (string.IsNullOrEmpty(nameSpace))
                nameSpace = config.ResourceBaseNamespace;
            
            if (!string.IsNullOrEmpty(strongTypes.ErrorMessage))
                throw new ApplicationException(DbIRes.T("StronglyTypedGlobalResourcesFailed", STR_RESOURCESET));

            if (classType != "Resx")
                strongTypes.CreateClassFromAllDatabaseResources(nameSpace, filename, resourceSets);
            else             
            {
                string outputBasePath = filename;

                if (resourceSets == null || resourceSets.Length < 1)
                    resourceSets = Manager.GetAllResourceSets(ResourceListingTypes.AllResources).ToArray();

                foreach (var resource in resourceSets)
                {
                    string file = Path.Combine(outputBasePath, resource + ".resx");
                    if (!System.IO.File.Exists(file))
                        continue;


                    var str = new StronglyTypedResources(null);
#if NETFULL
                    // Use automated generation
                    str.CreateResxDesignerClassFromResxFile(file, resource, nameSpace, false);
#else                    
                    // Manual code generation
                    str.CreateResxDesignerClassFromResourceSet(resource, nameSpace,resource, file);
#endif

                }
            }

            return true;
        }


        /// <summary>
        /// Export resources from database to Resx files.
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExportResxResources")]
        public bool ExportResxResources([FromBody] dynamic parms)
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif
            // Post:  {outputBasePath: "~\Properties", resourceSets: ["rs1","rs2"] }
            string outputBasePath = parms["outputBasePath"];

            string[] resourceSets = null;
            JArray t = parms["resourceSets"] as JArray;
            if (t != null)
            {
                resourceSets = t.ToObject<string[]>();
                if (resourceSets != null && resourceSets.Length == 1 && string.IsNullOrEmpty(resourceSets[0]))
                    resourceSets = null;
            }

            if (string.IsNullOrEmpty(outputBasePath))
                outputBasePath = DbResourceConfiguration.Current.ResxBaseFolder;

            if (outputBasePath.StartsWith("~"))
                outputBasePath = Request.MapPath(outputBasePath, basePath: Host.ContentRootPath );
            
            outputBasePath = FileUtils.NormalizePath(outputBasePath);
                                           
            DbResXConverter exporter = new DbResXConverter(outputBasePath);
                      


            // if resourceSets is null all resources are generated
            if (!exporter.GenerateResXFiles(resourceSets))
                throw new ApplicationException(DbIRes.T("ResourceGenerationFailed", STR_RESOURCESET));
          
            return true;
        }



        /// <summary>
        /// Import resources from Resx files into database
        /// </summary>
        /// <param name="inputBasePath"></param>
        /// <returns></returns>
        [Route("ImportResxResources")]
        public bool ImportResxResources(string inputBasePath = null)
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.GRes("FeatureDisabled"));
#endif

            if (string.IsNullOrEmpty(inputBasePath))
                inputBasePath = DbResourceConfiguration.Current.ResxBaseFolder;

            if (inputBasePath.Contains("~"))
                inputBasePath = Request.MapPath(inputBasePath,basePath: Host.ContentRootPath);

            inputBasePath = DbResourceUtils.NormalizePath(inputBasePath);

            DbResXConverter converter = new DbResXConverter(inputBasePath);

            bool res = false;

            if (DbResourceConfiguration.Current.ResxExportProjectType == GlobalizationResxExportProjectTypes.WebForms)
                res = converter.ImportWebResources(inputBasePath);
            else
                res = converter.ImportWinResources(inputBasePath);

            if (!res)
                new ApplicationException(DbIRes.T("ResourceImportFailed", STR_RESOURCESET));

            return true;
        }

        #endregion
    }



    /// <summary>
    /// Class that holds a resource value including the locale id
    /// </summary>
    public class ResourceString
    {
        public string LocaleId { get; set; }
        public string Value { get; set; }
    }


    /// <summary>
    /// Class that holds a resource item with all of its detail
    /// information.
    /// </summary>
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
            ValueType = item.ValueType;
        }

        public bool IsRtl
        {
            get
            {
                if (_isRtl != null)
                    return _isRtl.Value;

                _isRtl = false;
                try
                {
                    var li = LocaleId;
                    if (string.IsNullOrEmpty(LocaleId))
                        li = CultureInfo.InstalledUICulture.IetfLanguageTag;

                    var ci = CultureInfo.GetCultureInfoByIetfLanguageTag(LocaleId);
                    _isRtl = ci.TextInfo.IsRightToLeft;

                    return _isRtl.Value;
                }
                catch { }

                return _isRtl.Value;
            }
            set
            {
                _isRtl = value;
            }
        }
        private bool? _isRtl;

        public List<ResourceString> ResourceList { get; set; }
    }
}
