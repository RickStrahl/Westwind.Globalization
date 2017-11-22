#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          (c) West Wind Technologies, 2009-2015
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
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Westwind.Globalization.Properties;
using Westwind.Utilities;
using Westwind.Utilities.Data;

namespace Westwind.Globalization
{
    public abstract class DbResourceDataManager : IDbResourceDataManager
    {     
        /// <summary>
        /// Instance of the DbResourceConfiguration that can be overridden
        /// Defaults to the default instance - DbResourceConfiguration.Current
        /// </summary>
        public DbResourceConfiguration Configuration { get; set;  }                        

        /// <summary>
        /// Error message that can be checked after a method complets
        /// and returns a failure result.
        /// </summary>
        public string ErrorMessage { get; set;  }
        
        /// <summary>
        /// Code used to create a database (if required) for the
        /// given data provider.
        /// </summary>
        protected virtual string TableCreationSql { get; set; }

        /// <summary>
        /// Internally used Transaction object
        /// </summary>
        protected virtual DbTransaction Transaction { get; set; }


        protected DbResourceDataManager()
        {
            // assign default configuration from configuration file
            Configuration = DbResourceConfiguration.Current;
        }

        /// <summary>
        /// Creates an instance of the DbResourceDataManager based on configuration settings
        /// </summary>
        /// <returns></returns>
        public static DbResourceDataManager CreateDbResourceDataManager(Type managerType = null, 
                                                                        DbResourceConfiguration configuration = null)
        {
            if (managerType == null)
                managerType = DbResourceConfiguration.Current.DbResourceDataManagerType;
            if (managerType == null)
                managerType = typeof (DbResourceSqlServerDataManager);

            DbResourceDataManager manager = ReflectionUtils.CreateInstanceFromType(managerType) as DbResourceDataManager;
            if (configuration != null)
                manager.Configuration = configuration;
            else
                manager.Configuration = DbResourceConfiguration.Current;

            return manager;
        }

        /// <summary>
        /// Create an instance of the provider based on the resource type
        /// </summary>
        /// <returns></returns>
        public static DbResourceDataManager CreateDbResourceDataManager(DbResourceProviderTypes type, 
                                                                        DbResourceConfiguration configuration = null)
        {
            DbResourceDataManager manager;

            if (type == DbResourceProviderTypes.SqlServer)
                manager = new DbResourceSqlServerDataManager();
            else if (type == DbResourceProviderTypes.MySql)
                manager = new DbResourceMySqlDataManager();
            else if (type == DbResourceProviderTypes.SqLite)
                manager = new DbResourceSqLiteDataManager();
            else
                return null;

            if(configuration != null)
                manager.Configuration = configuration;

            return manager;
        }

        /// <summary>
        /// Creates an instance of the DataAccess Data provider
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual DataAccessBase GetDb(string connectionString = null)
        {
            if (connectionString == null)
                connectionString = Configuration.ConnectionString;

            return new SqlDataAccess(connectionString,DataAccessProviderTypes.SqlServer);
        }

        /// <summary>
        /// Returns a specific set of resources for a given culture and 'resource set' which
        /// in this case is just the virtual directory and culture.
        /// </summary>
        /// <param name="cultureName">name of the culture Id (de, de-de) to retrieve</param>
        /// <param name="resourceSet">Name of the resource set to retrieve</param>
        /// <returns></returns>
        public virtual IDictionary GetResourceSet(string cultureName, string resourceSet)
        {
            if (cultureName == null)
                cultureName = string.Empty;
            if (resourceSet == null)
                resourceSet = string.Empty;
            
            string resourceFilter;
            resourceFilter = " ResourceSet=@ResourceSet";

            var resources = new Dictionary<string, object>();

            using (var data = GetDb())
            {
                DbDataReader reader;

                if (string.IsNullOrEmpty(cultureName))
                    reader = data.ExecuteReader("select ResourceId,Value,Type,BinFile,TextFile,Filename,ValueType from " + Configuration.ResourceTableName + " where " + resourceFilter + " and (LocaleId is null OR LocaleId = '') order by ResourceId",
                        data.CreateParameter("@ResourceSet", resourceSet));
                else
                    reader = data.ExecuteReader("select ResourceId,Value,Type,BinFile,TextFile,Filename,ValueType from " + Configuration.ResourceTableName + " where " + resourceFilter + " and LocaleId=@LocaleId order by ResourceId",
                        data.CreateParameter("@ResourceSet", resourceSet),
                        data.CreateParameter("@LocaleId", cultureName));

                if (reader == null)
                {
                    SetError(data.ErrorMessage);
                    return resources;
                }

                try
                {
                    while (reader.Read())
                    {
                        object resourceValue = reader["Value"] as string;
                        string resourceType = reader["Type"] as string;
                        int valueType = 0;
                        
                        valueType = Convert.ToInt32(reader["ValueType"]);                           
                        
                        if (!string.IsNullOrWhiteSpace(resourceType))
                        {
                            try
                            {
                                // FileResource is a special type that is raw file data stored
                                // in the BinFile or TextFile data. Value contains
                                // filename and type data which is used to create: String, Bitmap or Byte[]
                                if (resourceType == "FileResource")
                                    resourceValue = LoadFileResource(reader);
                                else
                                {
                                    resourceValue =  DeserializeValue(resourceValue as string, resourceType);
                                }

                            }
                            catch
                            {
                                // ignore this error
                                resourceValue = null;
                            }
                        }
                        else
                        {
                            if (resourceValue == null)
                                resourceValue = string.Empty;
                        }
                        var key = reader["ResourceId"].ToString();


                        OnResourceSetValueConvert(ref resourceValue, key, valueType);

                        if (!resources.ContainsKey(key))
                            resources.Add(key, resourceValue);
                    }
                }
                catch (Exception ex)
                {
                    SetError(ex.GetBaseException().Message);
                    return resources;
                }
                finally
                {
                    // close reader and connection
                    reader.Close(); 
                }
            }

            return resources;
        }

        protected virtual void OnResourceSetValueConvert(ref object resourceValue, string key, int valueType)
        {
            foreach(var resourceSetValueConverter in DbResourceConfiguration.Current.ResourceSetValueConverters)
            {
                if (valueType == resourceSetValueConverter.ValueType)
                    resourceValue = resourceSetValueConverter.Convert(resourceValue, key);
            }            
        }


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
        public virtual Dictionary<string, object> GetResourceSetNormalizedForLocaleId(string cultureName, string resourceSet)
        {
            if (cultureName == null)
                cultureName = string.Empty;

            Dictionary<string, object> resDictionary = new Dictionary<string, object>();

            using (var data = GetDb())
            {
                Trace.WriteLine("GetResourceSetNormalizedForId: " + cultureName + " - " + resourceSet + "\r\n" +
                                      "\t" + data.ConnectionString);


                  DbDataReader reader = null;

                string sql =
                    @"select resourceId, LocaleId, Value, Type, BinFile, TextFile, FileName
    from " + Configuration.ResourceTableName + @"
	where ResourceSet=@ResourceSet and (LocaleId = '' {0} )
    order by ResourceId, LocaleId DESC";


                // use like parameter or '' if culture is empty/invariant
                string localeFilter = string.Empty;

                List<DbParameter> parameters =
                    new List<DbParameter> {data.CreateParameter("@ResourceSet", resourceSet)};

                if (!string.IsNullOrEmpty(cultureName))
                {
                    localeFilter += " OR LocaleId = @LocaleId";
                    parameters.Add(data.CreateParameter("@LocaleId", cultureName));

                    // *** grab shorter version
                    if (cultureName.Contains("-"))
                    {
                        localeFilter += " OR LocaleId = @LocaleId1";
                        parameters.Add(data.CreateParameter("@LocaleId1", cultureName.Split('-')[0]));
                    }
                }

                sql = string.Format(sql, localeFilter);

                reader = data.ExecuteReader(sql, parameters.ToArray());

                if (reader == null)
                {
                    SetError(data.ErrorMessage);
                    return resDictionary;
                }

                try
                {
                    string lastResourceId = "xxxyyy";

                    while (reader.Read())
                    {
                        // only pick up the first ID returned - the most specific locale
                        string resourceId = reader["ResourceId"].ToString();
                        if (resourceId == lastResourceId)
                            continue;
                        lastResourceId = resourceId;

                        // Read the value into this                        
                        object resourceValue = reader["Value"] as string;
                        string resourceType = reader["Type"] as string;

                        if (!string.IsNullOrWhiteSpace(resourceType))
                        {
                            // FileResource is a special type that is raw file data stored
                            // in the BinFile or TextFile data. Value contains
                            // filename and type data which is used to create: String, Bitmap or Byte[]
                            if (resourceType == "FileResource")
                                resourceValue = LoadFileResource(reader);
                            else
                                DeserializeValue(resourceValue as string, resourceType);
                        }
                        else
                        {
                            if (resourceValue == null)
                                resourceValue = string.Empty;
                        }

                        resDictionary.Add(resourceId, resourceValue);
                    }
                }
                catch { }
                finally
                {
                    // close reader and connection
                    reader.Close();
                    data.CloseConnection();
                }
            }

            return resDictionary;
        }

        public static ResourceItem  SetFileDataOnResourceItem(ResourceItem item, byte[] data, string fileName)
        {
            if (data == null || item == null || string.IsNullOrEmpty(fileName))
                throw new ArgumentException(Resources.ResourceItemMissingFileUploadData);

            string ext = Path.GetExtension(fileName).TrimStart('.').ToLower();
            const string filter = ",bmp,ico,gif,jpg,jpeg,png,css,js,txt,html,htm,xml,wav,mp3,";
            if (!filter.Contains("," + ext + ","))
                throw new ArgumentException(Resources.InvalidFileExtensionForFileResource);

            string type;
            if ("txt,css,htm,html,xml,js".Contains(ext))            
                type = typeof(string).AssemblyQualifiedName;
            

#if NETFULL
            else if ("jpg,jpeg,png,gif,bmp".Contains(ext))
                type = typeof (Bitmap).AssemblyQualifiedName;
            else if("ico" == ext)
                type = typeof(Icon).AssemblyQualifiedName;
#endif
            else
            {
#if NETFULL
                type = typeof(byte[]).AssemblyQualifiedName;
#else
                // TODO: Have to do this to make Visual Studio happy when compiling resources
                type = "System.Byte[], mscorlib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089";
#endif
            }

            using (var ms = new MemoryStream())
            {
                item.Value = fileName + ";" + type;
                item.BinFile = data;
                item.Type = "FileResource";
                item.FileName = fileName;
            }

            return item;
        }


        /// <summary>
        /// Internal method used to parse the data in the database into a 'real' value.
        /// 
        /// Value field hold filename and type string
        /// TextFile,BinFile hold the actual file content
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private object LoadFileResource(IDataReader reader)
        {
            object value = null;

            try
            {
                string TypeInfo = reader["Value"] as string;

                if (TypeInfo.IndexOf("System.String") > -1)
                {
                    value = reader["TextFile"] as string;
                }
#if NETFULL
                else if (TypeInfo.Contains("System.Drawing.Bitmap"))
                {
                    // IMPORTANT: don't release the mem stream or Jpegs won't render/save
                    if (TypeInfo.Contains(".jpg") || TypeInfo.Contains(".jpeg"))
                    {
                        // Some JPEGs require that the memory stream stays open in order
                        // to use the Bitmap later. Let CLR worry about garbage collection
                        // Prefer: Don't store jpegs
                        var ms = new MemoryStream(reader["BinFile"] as byte[]);
                        value = new Bitmap(ms);                        
                    }
                    else
                    {
                        using (var ms = new MemoryStream(reader["BinFile"] as byte[]))
                        {
                            value = new Bitmap(ms);
                        }
                    }
                }
                else if (TypeInfo.Contains("System.Drawing.Icon"))
                {
                    // IMPORTANT: don't release the mem stream 
                    var ms = new MemoryStream(reader["BinFile"] as byte[]);
                    value = new Icon(ms);
                }
#endif
                else
                {
                    value = reader["BinFile"] as byte[];
                }
            }
            catch (Exception ex)
            {
                SetError(reader["ResourceKey"].ToString() + ": " + ex.Message);
            }

            return value;
        }

        /// <summary>
        /// Returns a list of all the resources for all locales. The result is in a 
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
        public virtual List<ResourceItem> GetAllResources(bool localResources = false, bool applyValueConverters = false, string resourceSet = null)
        {
            IEnumerable<ResourceItem> items;
            using (var data = GetDb())
            {

                string resourceSetFilter = "";
                if (!string.IsNullOrEmpty(resourceSet))
                    resourceSetFilter = " AND resourceset = @ResourceSet2 ";

                string sql = "select ResourceId,Value,LocaleId,ResourceSet,Type,TextFile,BinFile,Filename,Comment,ValueType,Updated " +
                             "from " + Configuration.ResourceTableName + " " +
                             "where ResourceSet Is Not Null " +
                              resourceSetFilter +
                             " ORDER BY ResourceSet,LocaleId, ResourceId";

                //var parms = new List<IDbDataParameter> { data.CreateParameter("@ResourceSet", "%.%") };

                var parms = new List<IDbDataParameter>();
                if (!string.IsNullOrEmpty(resourceSetFilter))
                    parms.Add(data.CreateParameter("@ResourceSet2", resourceSet));

                items = data.Query<ResourceItem>(sql, parms.ToArray());


                
                if (items == null)
                {
                    ErrorMessage = data.ErrorMessage;
                    return null;
                }

                var itemList = items.ToList();

                if (applyValueConverters && DbResourceConfiguration.Current.ResourceSetValueConverters.Count > 0)
                {
                    foreach (var resourceItem in itemList)
                    {
                        foreach (var convert in DbResourceConfiguration.Current.ResourceSetValueConverters)
                        {
                            if (resourceItem.ValueType == convert.ValueType)
                                resourceItem.Value = convert.Convert(resourceItem.Value, resourceItem.ResourceId);
                        }
                    }
                }
                
                return itemList;
            }
        }

        
        /// <summary>
        /// Returns all available resource ids for a given resource set in all languages.
        /// 
        /// Returns a ResourceIdItem object with ResourecId and HasValue fields.
        /// HasValue returns whether there are any entries in any culture for this
        /// resourceId
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public virtual List<ResourceIdItem> GetAllResourceIds(string resourceSet)
        {                      
            using (var data = GetDb())
            {
                string sql = string.Format(
//                    @"select ResourceId, CAST(MAX(len(Value)) as bit)  as HasValue
//	  	from {0}
//        where ResourceSet=@ResourceSet
//		group by ResourceId", Configuration.ResourceTableName);
                @"select ResourceId,CAST( MAX( 
	  case  
		WHEN len( CAST(Value as nvarchar(10))) > 0 THEN 1
		ELSE 0
	  end ) as Bit) as HasValue
	  	from {0}
        where ResourceSet=@ResourceSet 
	    group by ResourceId",Configuration.ResourceTableName);

                var items = data.Query<ResourceIdItem>(sql,
                    data.CreateParameter("@ResourceSet", resourceSet));
                if (items == null)
                {
                    SetError(data.ErrorMessage);
                    return null;
                }

                return items.ToList();
            }
        }

        /// <summary>
        /// Returns a list with ResourceId and HasValues fields
        /// where the ResourceId is formatted for HTML display.
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public virtual List<ResourceIdListItem> GetAllResourceIdListItems(string resourceSet)
        {
            var resourceIds = GetAllResourceIds(resourceSet);
            if (resourceIds == null)
                return null;

            var listItems = resourceIds.Select(id => new ResourceIdListItem
            {
                 ResourceId = id.ResourceId,
                 HasValue = id.HasValue,
                 Value = id.Value as string
            }).ToList();
            
            string lastId = "xx";
            foreach (var resId in listItems)
            {
                string resourceId = resId.ResourceId;
               
                string[] tokens = resourceId.Split('.');
                if (tokens.Length == 1)
                {
                    lastId = tokens[0];
                }
                else
                {
                    if (lastId == tokens[0])
                    {
                        resId.Style = "color: maroon; margin-left: 20px;";                        
                    }
                    lastId = tokens[0];
                }
                
            }

            return listItems;
        }

        /// <summary>
        /// Returns all available resource sets
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetAllResourceSets(ResourceListingTypes type)
        {
            using (var data = GetDb())
            {
                DbDataReader dt = null;

                if (type == ResourceListingTypes.AllResources)
                    dt = data.ExecuteReader("select ResourceSet as ResourceSet from " +
                                            Configuration.ResourceTableName + " group by ResourceSet");
                else if (type == ResourceListingTypes.LocalResourcesOnly)
                    dt = data.ExecuteReader(
                        "select ResourceSet as ResourceSet from " +
                        Configuration.ResourceTableName +
                        " where resourceset like '%.aspx' or resourceset like '%.ascx' or resourceset like '%.master' or resourceset like '%.sitemap' group by ResourceSet",
                        data.CreateParameter("@ResourceSet", "%.%"));
                else if (type == ResourceListingTypes.GlobalResourcesOnly)
                    dt = data.ExecuteReader("select ResourceSet as ResourceSet from " +
                                            Configuration.ResourceTableName +
                                            " where resourceset not like '%.aspx' and resourceset not like '%.ascx' and resourceset not like '%.master' and resourceset not like '%.sitemap' group by ResourceSet");

                if (dt == null)
                {
                    ErrorMessage = data.ErrorMessage;
                    return null;
                }

                var items = new List<string>();                

                while (dt.Read())
                {
                    string id = dt["ResourceSet"] as string;
                    if (!string.IsNullOrEmpty(id))
                        items.Add(id);
                }

                return items;
            }
        }

        /// <summary>
        /// Gets all the locales for a specific resource set.
        /// 
        /// Returns a table named TLocaleIds (LocaleId field)
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public virtual List<string> GetAllLocaleIds(string resourceSet)
        {
            if (resourceSet == null)
                resourceSet = string.Empty;

            using (var data = GetDb())
            {
                var reader = data.ExecuteReader("select LocaleId,'' as Language from " + Configuration.ResourceTableName +
                                                " where ResourceSet=@ResourceSet group by LocaleId",
                    data.CreateParameter("@ResourceSet", resourceSet));

                if (reader == null)
                    return null;

                var ids = new List<string>();
                

                while (reader.Read())
                {
                    string id = reader["LocaleId"] as string;
                    if (id != null)
                        ids.Add(id);
                }

                return ids;
            }
        }

        /// <summary>
        /// Gets all the Resourecs and ResourceIds for a given resource set and Locale
        /// 
        /// returns a table "TResource" ResourceId, Value fields
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public virtual List<ResourceIdItem> GetAllResourcesForCulture(string resourceSet, string cultureName)
        {
            if (cultureName == null)
                cultureName = string.Empty;

            using (var data = GetDb())
            {
                var reader = 
                    data.ExecuteReader(
                        "select ResourceId, Value from " + Configuration.ResourceTableName + " where ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                        data.CreateParameter("@ResourceSet", resourceSet),
                        data.CreateParameter("@LocaleId", cultureName));

                if (reader == null)
                    return null;

                var ids = new List<ResourceIdItem>();
                
                while (reader.Read())
                {
                    string id = reader["ResourceId"] as string;
                    if (id != null)
                        ids.Add(new ResourceIdItem()
                        {
                            ResourceId = id,
                            Value = reader["Value"]
                        });
                }

                return ids;
            }
        }


        /// <summary>
        /// Returns an individual Resource String from the database
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>       
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public virtual string GetResourceString(string resourceId, string resourceSet, string cultureName)
        {
            SetError();

            if (cultureName == null)
                cultureName = string.Empty;

            object result;
            using (var data = GetDb())
            {
                result = data.ExecuteScalar("select Value from " + Configuration.ResourceTableName +
                                            " where ResourceId=@ResourceId and ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                    data.CreateParameter("@ResourceId", resourceId),
                    data.CreateParameter("@ResourceSet", resourceSet),
                    data.CreateParameter("@LocaleId", cultureName));
            }

            return result as string;
        }


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
        public virtual object GetResourceObject(string resourceId, string resourceSet, string cultureName)
        {
            object result = null;
            SetError();

            if (cultureName == null)
                cultureName = string.Empty;

            DbDataReader reader;
            using (var data = GetDb())
            {
                reader =
                    data.ExecuteReader(
                        "select Value,Type from " + Configuration.ResourceTableName +
                        " where ResourceId=@ResourceId and ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                        data.CreateParameter("@ResourceId", resourceId),
                        data.CreateParameter("@ResourceSet", resourceSet),
                        data.CreateParameter("@LocaleId", cultureName));

                if (reader == null)
                    return null;


                if (reader.Read())
                {

                    string resourceType = reader["Type"] as string;
                    object value = reader["Value"];

                    if (string.IsNullOrEmpty(resourceType))
                        result = value;
                    else
                        DeserializeValue(value as string, resourceType);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a resource item that returns both the Value and Comment to the
        /// fields to the client.
        /// </summary>
        /// <param name="resourceId">The ID of the resource to retrieve</param>
        /// <param name="resourceSet">Name of the ResourceSet to return</param>
        /// <param name="cultureName">required. Null or Empty returns invariant</param>
        /// <returns></returns>
        public virtual ResourceItem GetResourceItem(string resourceId, string resourceSet, string cultureName)
        {
            ErrorMessage = string.Empty;

            if (cultureName == null)
                cultureName = string.Empty;

            ResourceItem item;
            using (var data = GetDb())
            {
                using (IDataReader reader =
                    data.ExecuteReader(
                        "select * from " + Configuration.ResourceTableName +
                        " where ResourceId=@ResourceId and ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                        data.CreateParameter("@ResourceId", resourceId),
                        data.CreateParameter("@ResourceSet", resourceSet),
                        data.CreateParameter("@LocaleId", cultureName)))
                {
                    if (reader == null || !reader.Read())
                        return null;

                    item = new ResourceItem();
                    item.FromDataReader(reader);

                    reader.Close();
                }
            }
            
            return item;
        }

        /// <summary>
        /// Returns all resource items for a given resource ID in all locales.
        /// Returned as full ResourceItem objects
        /// </summary>
        /// <param name="resourceId">The resource Id to return for</param>
        /// <param name="resourceSet">Resourceset to look in</param>
        /// <param name="forAllResourceSetLocales">When true returns empty entries for missing resources of locales in this resource set</param>
        /// <returns>List of resource items or null</returns>
        public virtual IEnumerable<ResourceItem> GetResourceItems(string resourceId, string resourceSet, bool forAllResourceSetLocales = false)
        {
            ErrorMessage = string.Empty;

            if (resourceSet == null)
                resourceSet = string.Empty;
            
            List<ResourceItem> items = null;

            using (var data = GetDb())
            {
                using (IDataReader reader =
                    data.ExecuteReader(
                        "select * from " + Configuration.ResourceTableName +
                        " where ResourceId=@ResourceId and ResourceSet=@ResourceSet " +
                        " order by LocaleId",
                        data.CreateParameter("@ResourceId", resourceId),
                        data.CreateParameter("@ResourceSet", resourceSet)))
                {
                    if (reader == null)
                    {
                        SetError(data.ErrorMessage);
                        return null;
                    }
                        

                    items = new List<ResourceItem>();
                    while (reader.Read())
                    {
                        var item = new ResourceItem();
                        item.FromDataReader(reader);
                        items.Add(item);
                    }

                    reader.Close();
                }

                if (forAllResourceSetLocales)
                {
                    var locales = GetAllLocalesForResourceSet(resourceSet);
                    if (locales != null)
                    {
                        var usedLocales = items.Select(i => i.LocaleId);
                        var emptyLocales = locales.Where(s => !usedLocales.Contains(s));
                        foreach (var locale in emptyLocales)
                        {
                            items.Add(new ResourceItem(){ 
                                 LocaleId = locale,
                                 Value = "",
                                 ResourceSet = resourceSet
                            });
                        }
                    }
                }
            }

            return items;
        }


        /// <summary>
        /// Returns all the resource strings for all cultures for a specific resource Id.
        /// Returned as a dictionary.
        /// </summary>
        /// <param name="resourceId">Resource Id to retrieve strings for</param>
        /// <param name="resourceSet">Resource Set on which to retrieve strings</param>
        /// <param name="forAllResourceSetLocales">If true returns empty entries for each locale that exists but has no value in this resource set</param>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetResourceStrings(string resourceId, string resourceSet, bool forAllResourceSetLocales = false)
        {
            var Resources = new Dictionary<string, string>();
            using (var data = GetDb())
            {
                using (DbDataReader reader = data.ExecuteReader("select Value,LocaleId from " + Configuration.ResourceTableName +
                                                                " where ResourceId=@ResourceId and ResourceSet=@ResourceSet order by LocaleId",
                    data.CreateParameter("@ResourceId", resourceId),
                    data.CreateParameter("@ResourceSet", resourceSet)))
                {
                    if (reader == null)
                        return null;

                    while (reader.Read())
                    {
                        Resources.Add(reader["LocaleId"] as string, reader["Value"] as string);
                    }
                    reader.Dispose();
                }

                if (forAllResourceSetLocales)
                {
                    var locales = GetAllLocalesForResourceSet(resourceSet);
                    if (locales != null)
                    {
                        var usedLocales = Resources.Select(kv => kv.Key);
                        var emptyLocales = locales.Where(s => !usedLocales.Contains(s));
                        foreach (var locale in emptyLocales)
                        {
                            Resources.Add(locale, "");
                        }
                    }
                }

            }

            

            return Resources;
        }

        public virtual List<string> GetAllLocalesForResourceSet(string resourceSet)
        {
            var locales = new List<string>();

            using (var data = GetDb())
            {
                var localeTable = data.ExecuteTable("TLocales",
                    "select localeId from " + Configuration.ResourceTableName +
                    " where ResourceSet=@0 group by localeId", resourceSet);
                if (localeTable != null)
                {
                    foreach (DataRow row in localeTable.Rows)
                    {
                        var val = row["localeId"] as string ;
                        if (val != null)
                            locales.Add(val);
                    }
                    return locales;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resource">Resource to update</param>
        public virtual int UpdateOrAddResource(ResourceItem resource)
        {
            if (!IsValidCulture(resource.LocaleId))
            {
                ErrorMessage = string.Format(Resources.Can_t_save_resource__Invalid_culture_id_passed, resource.LocaleId);
                return -1;
            }

            int result = 0;
            result = UpdateResource(resource);

            // We either failed or we updated
            if (result != 0)
                return result;

            // We have no records matched in the Update - Add instead
            result = AddResource(resource);

            if (result == -1)
                return -1;

            return 1;
        }

        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>        
        public virtual int UpdateOrAddResource(string resourceId, object value, string cultureName, string resourceSet,
            string comment = null, bool valueIsFileName = false, int valueType = 0)
        {
            if (!IsValidCulture(cultureName))
            {
                ErrorMessage = string.Format(Resources.Can_t_save_resource__Invalid_culture_id_passed, cultureName);
                return -1;
            }

            int result = 0;
            result = UpdateResource(resourceId, value, cultureName, resourceSet, comment, valueIsFileName);

            // We either failed or we updated
            if (result != 0)
                return result;

            // We have no records matched in the Update - Add instead
            result = AddResource(resourceId, value, cultureName, resourceSet, comment, valueIsFileName);

            if (result == -1)
                return -1;

            return 1;
        }


        /// <summary>
        /// Adds a resource to the Localization Table
        /// </summary>
        /// <param name="resource">Resource to update</param>        
        public virtual int AddResource(ResourceItem resource)
        {
            string Type = string.Empty;

            if (resource.LocaleId == null)
                resource.LocaleId = string.Empty;

            if (string.IsNullOrEmpty(resource.ResourceId))
            {
                ErrorMessage = Resources.NoResourceIdSpecifiedCantAddResource;
                return -1;
            }

            if (resource.Value != null && !(resource.Value is string))
            {
                Type = resource.Value.GetType().AssemblyQualifiedName;
                try
                {
                    SerializeValue(resource.Value);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return -1;
                }
            }
            else
                Type = string.Empty;


            if (resource.Value == null)
                resource.Value = string.Empty;

            using (var data = GetDb())
            {
                if (Transaction != null)
                    data.Transaction = Transaction;

                DbParameter BinFileParm = data.CreateParameter("@BinFile", resource.BinFile, DbType.Binary);
                DbParameter TextFileParm = data.CreateParameter("@TextFile", resource.TextFile);

                string Sql = "insert into " + Configuration.ResourceTableName +
                             " (ResourceId,Value,LocaleId,Type,ResourceSet,BinFile,TextFile,Filename,Comment,ValueType,Updated) Values (@ResourceId,@Value,@LocaleId,@Type,@ResourceSet,@BinFile,@TextFile,@FileName,@Comment,@ValueType,@Updated)";
                if (data.ExecuteNonQuery(Sql,
                    data.CreateParameter("@ResourceId", resource.ResourceId),
                    data.CreateParameter("@Value", resource.Value),
                    data.CreateParameter("@LocaleId", resource.LocaleId),
                    data.CreateParameter("@Type", resource.Type),
                    data.CreateParameter("@ResourceSet", resource.ResourceSet),
                    BinFileParm, TextFileParm,
                    data.CreateParameter("@FileName", resource.FileName),
                    data.CreateParameter("@Comment", resource.Comment),
                    data.CreateParameter("@ValueType", resource.ValueType),
                    data.CreateParameter("@Updated", DateTime.UtcNow)) == -1)
                {
                    ErrorMessage = data.ErrorMessage;
                    return -1;
                }
            }

            return 1;
        }


        /// <summary>
        /// Adds a resource to the Localization Table
        /// </summary>
        /// <param name="resourceId">The resource key name</param>
        /// <param name="value">Value to set it to. Can also be a file which is loaded as binary data when valueIsFileName is true</param>
        /// <param name="cultureName">name of the culture or null for invariant/default</param>
        /// <param name="resourceSet">The ResourceSet to which the resource id is added</param>
        /// <param name="comment">Optional comment for the key</param>
        /// <param name="valueIsFileName">if true the Value property is a filename to import</param>
        public virtual int AddResource(string resourceId, object value,
                                       string cultureName, string resourceSet,
                                       string comment = null, bool valueIsFileName = false, 
                                       int valueType = 0)
        {
            string Type = string.Empty;

            if (cultureName == null)
                cultureName = string.Empty;

            if (string.IsNullOrEmpty(resourceId))
            {
                ErrorMessage = Resources.NoResourceIdSpecifiedCantAddResource;
                return -1;
            }

            if (value != null && !(value is string))
            {
                Type = value.GetType().AssemblyQualifiedName;
                try
                {
                    SerializeValue(value);                                        
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return -1;
                }
            }
            else
                Type = string.Empty;

            byte[] BinFile = null;
            string TextFile = null;
            string FileName = string.Empty;

            if (valueIsFileName)
            {
                FileInfoFormat FileData = null;
                try
                {
                    FileData = GetFileInfo(value as string);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return -1;
                }

                Type = "FileResource";
                value = FileData.ValueString;
                FileName = FileData.FileName;

                if (FileData.FileFormatType == FileFormatTypes.Text)
                    TextFile = FileData.TextContent;
                else
                    BinFile = FileData.BinContent;
            }

            if (value == null)
                value = string.Empty;

            using (var data = GetDb())
            {
                if (Transaction != null)
                    data.Transaction = Transaction;

                DbParameter BinFileParm = data.CreateParameter("@BinFile", BinFile, DbType.Binary);
                DbParameter TextFileParm = data.CreateParameter("@TextFile", TextFile);

                string Sql = "insert into " + Configuration.ResourceTableName +
                             " (ResourceId,Value,LocaleId,Type,ResourceSet,BinFile,TextFile,Filename,Comment,ValueType,Updated) " +
                             "Values (@ResourceId,@Value,@LocaleId,@Type,@ResourceSet,@BinFile,@TextFile,@Filename,@Comment,@ValueType,@Updated)";
                if (data.ExecuteNonQuery(Sql,
                    data.CreateParameter("@ResourceId", resourceId),
                    data.CreateParameter("@Value", value),
                    data.CreateParameter("@LocaleId", cultureName),
                    data.CreateParameter("@Type", Type),
                    data.CreateParameter("@ResourceSet", resourceSet),
                    BinFileParm, TextFileParm,
                    data.CreateParameter("@Filename", FileName),
                    data.CreateParameter("@Comment", comment),
                    data.CreateParameter("@ValueType",valueType),
                    data.CreateParameter("@Updated", DateTime.UtcNow)) == -1)
                {
                    ErrorMessage = data.ErrorMessage;
                    return -1;
                }
            }

            return 1;
        }


        ///// <summary>
        ///// Updates an existing resource in the Localization table
        ///// </summary>
        ///// <param name="ResourceId">The Resource id to update</param>
        ///// <param name="Value">The value to set it to</param>
        ///// <param name="CultureName">The 2 (en) or 5 character (en-us)culture. Or "" for Invariant </param>
        ///// <param name="ResourceSet">The name of the resourceset.</param>
        ///// <param name="Type"></param>
        ///// <
        //public virtual int UpdateResource(string ResourceId, object Value, string CultureName, string ResourceSet, string Comment)
        //{
        //    return UpdateResource(ResourceId, Value, CultureName, ResourceSet, Comment, false);
        //}

        /// <summary>
        /// Updates an existing resource in the Localization table
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>
        public virtual int UpdateResource(string resourceId, object value, 
                                          string cultureName, string resourceSet,
                                          string comment = null, bool valueIsFileName = false,
                                          int valueType = 0)
        {
            string type;
            if (cultureName == null)
                cultureName = string.Empty;


            if (value != null && !(value is string))
            {
                type = value.GetType().AssemblyQualifiedName;
                try
                {
                    value = SerializeValue(value);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return -1;
                }
            }
            else
            {
                type = string.Empty;

                if (value == null)
                    value = string.Empty;
            }

            byte[] BinFile = null;
            string TextFile = null;
            string FileName = string.Empty;

            if (valueIsFileName)
            {
                FileInfoFormat FileData = null;
                try
                {
                    FileData = GetFileInfo(value as string);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return -1;
                }

                type = "FileResource";
                value = FileData.ValueString;
                FileName = FileData.FileName;

                if (FileData.FileFormatType == FileFormatTypes.Text)
                    TextFile = FileData.TextContent;
                else
                    BinFile = FileData.BinContent;
            }

            if (value == null)
                value = string.Empty;


            int result;
            using (var data = GetDb())
            {
                if (Transaction != null)
                    data.Transaction = Transaction;

                // Set up Binfile and TextFile parameters which are set only for
                // file values - otherwise they'll pass as Null values.
                var binFileParm = data.CreateParameter("@BinFile", BinFile, DbType.Binary);
                var textFileParm = data.CreateParameter("@TextFile", TextFile);

                string sql = "update " + Configuration.ResourceTableName +
                             " set Value=@Value, Type=@Type, BinFile=@BinFile,TextFile=@TextFile,Filename=@FileName, Comment=@Comment, ValueType=@ValueType, Updated=@Updated " +
                             "where LocaleId=@LocaleId AND ResourceSet=@ResourceSet and ResourceId=@ResourceId";
                result = data.ExecuteNonQuery(sql,
                    data.CreateParameter("@ResourceId", resourceId),
                    data.CreateParameter("@Value", value),
                    data.CreateParameter("@Type", type),
                    data.CreateParameter("@LocaleId", cultureName),
                    data.CreateParameter("@ResourceSet", resourceSet),
                    binFileParm, textFileParm,
                    data.CreateParameter("@FileName", FileName),
                    data.CreateParameter("@Comment", comment),
                    data.CreateParameter("@ValueType", valueType),
                    data.CreateParameter("@Updated",DateTime.UtcNow)
                    );
                if (result == -1)
                {
                    ErrorMessage = data.ErrorMessage;
                    return -1;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resource">Resource to update</param>
        public virtual int UpdateResource(ResourceItem resource)
        {
            if (resource == null)
            {
                SetError("Resource passed cannot be null.");
                return -1;
            }

            string type = null;

            if (resource.LocaleId == null)
                resource.LocaleId = string.Empty;


            if (resource.Value != null && !(resource.Value is string))
            {
                type = resource.Value.GetType().AssemblyQualifiedName;
                try
                {
                    resource.Value = SerializeValue(resource.Value);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return -1;
                }
            }
            else if (resource.BinFile != null && string.IsNullOrEmpty(resource.Type))
                type = "FileResource";
            else
            {
                type = string.Empty;

                if (resource.Value == null)
                    resource.Value = string.Empty;
            }


          
            if (resource.Value == null)
                resource.Value = string.Empty;


            int result;
            using (var data = GetDb())
            {
                if (Transaction != null)
                    data.Transaction = Transaction;

                // Set up Binfile and TextFile parameters which are set only for
                // file values - otherwise they'll pass as Null values.
                var binFileParm = data.CreateParameter("@BinFile", resource.BinFile, DbType.Binary);
                var textFileParm = data.CreateParameter("@TextFile", resource.TextFile);

                string sql = "update " + Configuration.ResourceTableName +
                             " set Value=@Value, Type=@Type, BinFile=@BinFile,TextFile=@TextFile,Filename=@FileName, Comment=@Comment, ValueType=@ValueType, Updated=@Updated " +
                             "where LocaleId=@LocaleId AND ResourceSet=@ResourceSet and ResourceId=@ResourceId";
                result = data.ExecuteNonQuery(sql,
                    data.CreateParameter("@ResourceId", resource.ResourceId),
                    data.CreateParameter("@Value", resource.Value),
                    data.CreateParameter("@Type", resource.Type),
                    data.CreateParameter("@LocaleId", resource.LocaleId),
                    data.CreateParameter("@ResourceSet", resource.ResourceSet),
                    binFileParm, textFileParm,
                    data.CreateParameter("@FileName", resource.FileName),
                    data.CreateParameter("@Comment", resource.Comment),
                    data.CreateParameter("@ValueType", resource.ValueType),
                    data.CreateParameter("@Updated", DateTime.UtcNow)
                    );
                if (result == -1)
                {
                    ErrorMessage = data.ErrorMessage;
                    return -1;
                }
            }

            return result;

        }
        

        /// <summary>
        /// Internal routine that looks at a file and based on its
        /// extension determines how that file needs to be stored in the
        /// database. Returns FileInfoFormat structure
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileInfoFormat GetFileInfo(string fileName, bool noPhysicalFile = false)
        {
            FileInfoFormat fileInfo = new FileInfoFormat();

            FileInfo fi = new FileInfo(fileName);
            if (!noPhysicalFile && !fi.Exists)
                throw new InvalidOperationException("Invalid Filename");

            string Extension = fi.Extension.ToLower().TrimStart('.');
            fileInfo.FileName = fi.Name;

            if (Extension == "txt" || Extension == "css" || Extension == "js" || Extension.StartsWith("htm") || Extension == "xml")
            {
                fileInfo.FileFormatType = FileFormatTypes.Text;
                fileInfo.Type = "FileResource";

                if (!noPhysicalFile)
                {
                    using (StreamReader sr = new StreamReader(fileName, Encoding.Default, true))
                    {
                        fileInfo.TextContent = sr.ReadToEnd();
                    }
                }
                fileInfo.ValueString = fileInfo.FileName + ";" + typeof(string).AssemblyQualifiedName + ";" + Encoding.Default.HeaderName;
            }
#if NETFULL
            else if (Extension == "gif" || Extension == "jpg" || Extension == "jpeg" || Extension == "bmp" || Extension == "png")
            {
                fileInfo.FileFormatType = FileFormatTypes.Image;
                fileInfo.Type = "FileResource";
                if(!noPhysicalFile)
                    fileInfo.BinContent = File.ReadAllBytes(fileName);
                fileInfo.ValueString = fileInfo.FileName + ";" + typeof(Bitmap).AssemblyQualifiedName;
            }
            else if (Extension == "ico")
            {
                fileInfo.FileFormatType = FileFormatTypes.Image;
                fileInfo.Type = "System.Drawing.Icon, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
                if (!noPhysicalFile)
                    fileInfo.BinContent = File.ReadAllBytes(fileName);
                fileInfo.ValueString = fileInfo.FileName + ";" + typeof(Icon).AssemblyQualifiedName;
            }
#endif
            else
            {
                fileInfo.FileFormatType = FileFormatTypes.Binary;
                fileInfo.Type = "FileResource"; 
                if (!noPhysicalFile)
                    fileInfo.BinContent = File.ReadAllBytes(fileName);                
                fileInfo.ValueString = fileInfo.FileName + ";" + typeof(Byte[]).AssemblyQualifiedName;
            }

            return fileInfo;
        }

    


        /// <summary>
        /// Deletes a specific resource ID based on ResourceId, ResourceSet and Culture.
        /// If an empty culture is passed the entire group is removed (ie. all locales).
        /// </summary>
        /// <param name="resourceId">Resource Id to delete</param>
        /// <param name="resourceSet">The resource set to remove</param>
        /// <param name="cultureName">language ID - if empty all languages are deleted</param>
        /// e
        /// <returns></returns>
        public virtual bool DeleteResource(string resourceId, string resourceSet = null, string cultureName = null)
        {
            int Result = 0;

            if (cultureName == null)
                cultureName = string.Empty;
            if (resourceSet == null)
                resourceSet = string.Empty;

            using (var data = GetDb())
            {
                if (!string.IsNullOrEmpty(cultureName))
                    // Delete the specific entry only
                    Result = data.ExecuteNonQuery("delete from " + Configuration.ResourceTableName +
                                                  " where ResourceId=@ResourceId and LocaleId=@LocaleId and ResourceSet=@ResourceSet",
                        data.CreateParameter("@ResourceId", resourceId),
                        data.CreateParameter("@LocaleId", cultureName),
                        data.CreateParameter("@ResourceSet", resourceSet));
                else
                // If we're deleting the invariant entry - delete ALL of the languages for this key
                    Result = data.ExecuteNonQuery("delete from " + Configuration.ResourceTableName +
                                                  " where ResourceId=@ResourceId and ResourceSet=@ResourceSet",
                        data.CreateParameter("@ResourceId", resourceId),
                        data.CreateParameter("@ResourceSet", resourceSet));

                if (Result == -1)
                {
                    ErrorMessage = data.ErrorMessage;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Renames a given resource in a resource set. Note all languages will be renamed
        /// </summary>
        /// <param name="ResourceId"></param>
        /// <param name="NewResourceId"></param>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public virtual bool RenameResource(string ResourceId, string NewResourceId, string ResourceSet)
        {
            using (var data = GetDb())
            {
                var result = data.ExecuteNonQuery("update " + Configuration.ResourceTableName + 
                                                  " set ResourceId=@NewResourceId where ResourceId=@ResourceId AND ResourceSet=@ResourceSet", 
                    data.CreateParameter("@ResourceId", ResourceId), 
                    data.CreateParameter("@NewResourceId", NewResourceId),
                    data.CreateParameter("@ResourceSet", ResourceSet));
                if (result == -1)
                {
                    ErrorMessage = data.ErrorMessage;
                    return false;
                }
                if (result == 0)
                {
                    ErrorMessage = "Invalid ResourceId";
                    return false;
                }
            }

            

            return true;
        }

        /// <summary>
        /// Renames all property keys for a given property prefix. So this routine updates
        /// lblName.Text, lblName.ToolTip to lblName2.Text, lblName2.ToolTip if the property
        /// is changed from lblName to lblName2.
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="NewProperty"></param>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public virtual bool RenameResourceProperty(string Property, string NewProperty, string ResourceSet)
        {
            using (var data = GetDb())
            {
                Property += ".";
                NewProperty += ".";
                string PropertyQuery = Property + "%";
                int Result = data.ExecuteNonQuery("update " + Configuration.ResourceTableName + " set ResourceId=replace(resourceid,@Property,@NewProperty) where ResourceSet=@ResourceSet and ResourceId like @PropertyQuery",
                    data.CreateParameter("@Property", Property),
                    data.CreateParameter("@NewProperty", NewProperty),
                    data.CreateParameter("@ResourceSet", ResourceSet),
                    data.CreateParameter("@PropertyQuery", PropertyQuery));
                if (Result == -1)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes an entire resource set from the database. Careful with this function!
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public virtual bool DeleteResourceSet(string ResourceSet, string cultureName = null)
        {
            if (string.IsNullOrEmpty(ResourceSet))
                return false;

            using (var data = GetDb())
            {
                int result;
                if (cultureName == null)
                    result = data.ExecuteNonQuery("delete from " + Configuration.ResourceTableName + 
                                                " where ResourceSet=@ResourceSet",
                                                data.CreateParameter("@ResourceSet", ResourceSet));
                else
                    result = data.ExecuteNonQuery("delete from " + Configuration.ResourceTableName +
                                                    " where ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                                                    data.CreateParameter("@ResourceSet", ResourceSet),
                                                    data.CreateParameter("@LocaleId",cultureName));
                if (result < 0)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
                if (result == 0)
                {
                    SetError(Resources.No_matching_Recordset_found);
                    return false;
                }

                return true;
            }
        }

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
        public virtual bool RenameResourceSet(string OldResourceSet, string NewResourceSet)
        {
            using (var  data = GetDb())
            {
                int result = data.ExecuteNonQuery("update " + Configuration.ResourceTableName + " set ResourceSet=@NewResourceSet where ResourceSet=@OldResourceSet",
                    data.CreateParameter("@NewResourceSet", NewResourceSet),
                    data.CreateParameter("@OldResourceSet", OldResourceSet));
                if (result == -1)
                {
                    SetError( data.ErrorMessage);
                    return false;
                }
                if (result == 0)
                {
                    SetError(Resources.No_matching_Recordset_found);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks to see if a resource exists in the resource store
        /// </summary>
        /// <param name="ResourceId"></param>
        /// <param name="Value"></param>
        /// <param name="CultureName"></param>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public virtual bool ResourceExists(string ResourceId, string CultureName, string ResourceSet)
        {
            if (CultureName == null)
                CultureName = string.Empty;

            using (var Data = GetDb())
            {
                var result = Data.ExecuteScalar("select ResourceId from " + Configuration.ResourceTableName + 
                                                " where ResourceId=@ResourceId and LocaleID=@LocaleId and ResourceSet=@ResourceSet group by ResourceId",
                    Data.CreateParameter("@ResourceId", ResourceId),
                    Data.CreateParameter("@LocaleId", CultureName),
                    Data.CreateParameter("@ResourceSet", ResourceSet));

                if (result == null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true or false depending on whether the two letter country code exists
        /// </summary>
        /// <param name="IetfTag">two or four letter IETF tag (examples: de, de-DE,fr,fr-CA)</param>
        /// <returns>true or false</returns>
        public virtual bool IsValidCulture(string IetfTag)
        {
            try
            {
                CultureInfo culture = CultureInfo.GetCultureInfoByIetfLanguageTag(IetfTag);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Persists resources to the database - first wipes out all resources, then writes them back in
        /// from the ResourceSet
        /// </summary>
        /// <param name="resourceList"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        public virtual bool GenerateResources(IDictionary resourceList, string cultureName, string resourceSet, bool deleteAllResourceFirst)
        {
            if (resourceList == null)
                throw new InvalidOperationException("No Resources");

            if (cultureName == null)
                cultureName = string.Empty;

            using (var data =GetDb())
            {
                if (!data.BeginTransaction())
                    return false;

                // Set transaction to be shared by other methods
                Transaction = data.Transaction;
                try
                {
                    // First delete all resources for this resource set
                    if (deleteAllResourceFirst)
                    {
                        int result = data.ExecuteNonQuery("delete " + Configuration.ResourceTableName + " where LocaleId=@LocaleId and ResourceSet=@ResourceSet",
                            data.CreateParameter("@LocaleId", cultureName),
                            data.CreateParameter("@ResourceSet", resourceSet));
                        if (result == -1)
                        {
                            data.RollbackTransaction();
                            return false;
                        }
                    }
                    // Now add them all back in one by one
                    foreach (DictionaryEntry Entry in resourceList)
                    {
                        if (Entry.Value != null)
                        {
                            int Result = 0;
                            if (deleteAllResourceFirst)
                                Result = AddResource(Entry.Key.ToString(), Entry.Value, cultureName, resourceSet, null);
                            else
                                Result = UpdateOrAddResource(Entry.Key.ToString(), Entry.Value, cultureName, resourceSet, null);
                            if (Result == -1)
                            {
                                data.RollbackTransaction();
                                return false;
                            }
                        }
                    }
                }
                catch
                {
                    data.RollbackTransaction();
                    return false;
                }
                data.CommitTransaction();
            }

            // Clear out the resources
            resourceList = null;

            return true;
        }


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
        public virtual string GetResourcesAsJavascriptObject(string javaScriptVarName, string resourceSet, string localeId)
        {
            if (localeId == null)
                localeId = CultureInfo.CurrentUICulture.IetfLanguageTag;
            if (resourceSet == null)
                resourceSet = string.Empty;

            IDictionary resources = GetResourceSetNormalizedForLocaleId(
                localeId, resourceSet);

            // Filter the list to non-control resources 
            Dictionary<string, string> localRes = new Dictionary<string, string>();
            foreach (string key in resources.Keys)
            {
                // We're only interested in non control local resources 
                if (!key.Contains(".") && resources[key] is string)
                    localRes.Add(key, resources[key] as string);
            }

            var json = JsonConvert.SerializeObject(localRes, Formatting.Indented);            
            return "var " + javaScriptVarName + " = " + json + ";\r\n";
        }


        /// <summary>
        /// Checks to see if the LocalizationTable exists
        /// </summary>
        /// <param name="tableName">Table name or the configuration.ResourceTableName if not passed</param>
        /// <returns></returns>
        public virtual bool IsLocalizationTable(string tableName = null)
        {
            if (tableName == null)
                tableName = Configuration.ResourceTableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = "Localizations";
            
            string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME=@0";
        
            using (var data = GetDb())
            {
                var tables = data.ExecuteTable("TTables", sql, tableName);

                if (tables == null || tables.Rows.Count < 1)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Create a backup of the localization database.
        /// 
        /// Note the table used is the one specified in the Configuration.ResourceTableName
        /// </summary>
        /// <param name="BackupTableName">Table of the backup table. Null creates a _Backup table.</param>
        /// <returns></returns>
        public virtual bool CreateBackupTable(string BackupTableName)
        {
            if (BackupTableName == null)
                BackupTableName = Configuration.ResourceTableName + "_Backup";

            using (var data = GetDb())
            {
                data.ExecuteNonQuery("drop table " + BackupTableName);
                if (data.ExecuteNonQuery("select * into " + BackupTableName + " from " + Configuration.ResourceTableName) < 0)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Restores the localization table from a backup table by first wiping out 
        /// </summary>
        /// <param name="backupTableName"></param>
        /// <returns></returns>
        public virtual bool RestoreBackupTable(string backupTableName)
        {
            if (backupTableName == null)
                backupTableName = Configuration.ResourceTableName + "_Backup";

            using (var data = GetDb())
            {

                data.BeginTransaction();

                if (data.ExecuteNonQuery("delete from " + Configuration.ResourceTableName) < 0)
                {
                    data.RollbackTransaction();
                    ErrorMessage = data.ErrorMessage;
                    return false;
                }

                string sql =
                    @"insert into {0}
  (ResourceId,Value,LocaleId,ResourceSet,Type,BinFile,TextFile,FileName,Comment) 
   select ResourceId,Value,LocaleId,ResourceSet,Type,BinFile,TextFile,FileName,Comment from {1}";

                sql = string.Format(sql, Configuration.ResourceTableName, backupTableName);

                if (data.ExecuteNonQuery(sql) < 0)
                {
                    data.RollbackTransaction();
                    SetError(data.ErrorMessage);
                    return false;
                }

                data.CommitTransaction();
            }

            return true;
        }


        /// <summary>
        /// Creates the Localization table on the current connection string for
        /// the provider.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual bool CreateLocalizationTable(string tableName = null)
        {
            if (tableName == null)
                tableName = Configuration.ResourceTableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = "Localizations";

            string Sql = string.Format(TableCreationSql, tableName);

            
            // Check for table existing already
            if (IsLocalizationTable(tableName))
            {
                SetError(Resources.LocalizationTable_Localization_Table_exists_already);
                return false;
            }

            SetError();

            using (var data = GetDb())
            {
                if (!data.RunSqlScript(Sql, false, false))
                {
                    ErrorMessage = data.ErrorMessage;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Serializes a value to string that can be stored in
        /// data storage.
        /// Used for serializing arbitrary objects to store in the application
        /// </summary>
        /// <param name="value"></param>
        /// <returns>JSON string or null (no exceptions thrown on error)</returns>
        protected virtual string SerializeValue(object value){
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Deserializes serialized data in JSON format based on a
        /// type name provided in the resource type parameter.        
        /// </summary>
        /// <param name="serializedValue">JSON encoded string</param>
        /// <param name="resourceType">Type name to deserialize - type must be referenced by the app</param>
        /// <returns>value or null on failure (no exceptions thrown)</returns>
        protected virtual object DeserializeValue(string serializedValue, string resourceType)
        {
            var type = ReflectionUtils.GetTypeFromName(resourceType);
            if (type == null)
                return null;

            return JsonConvert.DeserializeObject(serializedValue,type);
        }
       
        public void SetError()
        {
            SetError("CLEAR");
        }

        public void SetError(string message)
        {
            if (message == null || message == "CLEAR")
            {
                ErrorMessage = string.Empty;
                return;
            }
            ErrorMessage += message;
        }

        public void SetError(Exception ex)
        {
            if (ex == null)
            {
                ErrorMessage = string.Empty;
                return;
            }

            ErrorMessage = ex.GetBaseException().Message;
        }


    }

    /// <summary>
    /// The data managers supported by this library
    /// </summary>
    public enum DbResourceDataManagerTypes
    {
        SqlServer,
        SqlServerCe,
        MySql,
        SqlLite,
        MongoDb, // not implemented yet
        None
    }

    /// <summary>
    /// Short form ResourceItem for passing Ids
    /// </summary>    
    public class ResourceIdItem
    {
        public string ResourceId { get; set; }
        public bool HasValue { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return ResourceId + " - " + Value;
        }
    }

    public class BasicResourceItem
    {
        public string ResourceId { get; set; }
        public string LocaleId { get; set; }
        public string ResourceSet { get; set; }
        public string Value { get; set; }
    }

    public class ResourceIdListItem : ResourceIdItem
    {
        public string Text { get; set;  }
        public bool Selected { get; set; }
        public string Style { get; set; }
    }

    /// <summary>
    /// Determines how hte GetAllResourceSets method returns its data
    /// </summary>
    public enum ResourceListingTypes
    {
        LocalResourcesOnly,
        GlobalResourcesOnly,
        AllResources
    }

    public enum FileFormatTypes
    {
        Text,
        Image,
        Binary
    }

    /// <summary>
    /// Sets the DbResourceProviderType based on a simple enum value. Provided
    /// merely as a proxy for setting the actual type.
    /// 
    /// Use **Custom** if you want to use a custom provider that you created
    /// and that's not listed here.
    /// </summary>
    public enum DbResourceProviderTypes
    {
        SqlServer,
        MySql,
        SqLite,
        SqlServerCompact,
        Custom,
        NotSet        
    }

    /// <summary>
    /// Internal structure that contains format information about a file
    /// resource. Used internally to figure out how to write 
    /// a resource into the database
    /// </summary>
    public class FileInfoFormat
    {
        public string FileName = string.Empty;
        public string Encoding = string.Empty;
        public byte[] BinContent = null;
        public string TextContent = string.Empty;
        public FileFormatTypes FileFormatType = FileFormatTypes.Binary;
        public string ValueString = string.Empty;
        public string Type = "File";
    }


}