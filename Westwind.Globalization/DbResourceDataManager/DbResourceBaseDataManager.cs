using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Web.JsonSerializers;

namespace Westwind.Globalization
{
    public abstract class DbResourceBaseDataManager : IDbResourceDataManager
    {
        /// <summary>
        /// Internally used Transaction object
        /// </summary>
        protected DbTransaction Transaction = null;

        /// <summary>
        /// Instance of the DbResourceConfiguration that can be overridden
        /// Defaults to the default instance.
        /// </summary>
        public DbResourceConfiguration Configuration { get; set;  }
                
        /// <summary>
        /// Code used to create a database (if required) for the
        /// given data provider.
        /// </summary>
        protected virtual string TableCreationSql { get; set; }

        

        /// <summary>
        /// Error message that can be checked after a method complets
        /// and returns a failure result.
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        private string _ErrorMessage = string.Empty;


        public DbResourceBaseDataManager()
        {
            Configuration = DbResourceConfiguration.Current;
        }

        /// <summary>
        /// TODO: figure out how to instantiate different providers
        /// </summary>
        /// <returns></returns>
        public static IDbResourceDataManager GetDataManager()
        {            
            throw new NotImplementedException();
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
            if (connectionString == null)
                connectionString = Configuration.ConnectionString;

            return new SqlDataAccess(connectionString);
        }

        /// <summary>
        /// Returns a specific set of resources for a given culture and 'resource set' which
        /// in this case is just the virtual directory and culture.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public virtual IDictionary GetResourceSet(string cultureName, string resourceSet)
        {
            if (cultureName == null)
                cultureName = string.Empty;

            string resourceFilter;
            resourceFilter = " ResourceSet=@ResourceSet";

            var resources = new Dictionary<string, object>();

            using (var data = GetDb())
            {
                DbDataReader reader;

                if (string.IsNullOrEmpty(cultureName))
                    reader = data.ExecuteReader("select ResourceId,Value,Type,BinFile,TextFile,FileName from " + Configuration.ResourceTableName + " where " + resourceFilter + " and (LocaleId is null OR LocaleId = '') order by ResourceId",
                        data.CreateParameter("@ResourceSet", resourceSet));
                else
                    reader = data.ExecuteReader("select ResourceId,Value,Type,BinFile,TextFile,FileName from " + Configuration.ResourceTableName + " where " + resourceFilter + " and LocaleId=@LocaleId order by ResourceId",
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
                                    LosFormatter formatter = new LosFormatter();
                                    resourceValue = formatter.Deserialize(resourceValue as string);
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
                DbDataReader reader = null;

                string sql =
                    @"select resourceId, LocaleId, Value, Type, BinFile, TextFile, FileName
    from " + Configuration.ResourceTableName + @"
	where ResourceSet=@ResourceSet and (LocaleId = '' {0} )
    order by ResourceId, LocaleId DESC";


                // use like parameter or '' if culture is empty/invariant
                string localeFilter = string.Empty;

                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(data.CreateParameter("@ResourceSet", resourceSet));

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
                        object resourceValue = null;
                        resourceValue = reader["Value"] as string;

                        string resourceType = reader["Type"] as string;

                        if (!string.IsNullOrWhiteSpace(resourceType))
                        {
                            // FileResource is a special type that is raw file data stored
                            // in the BinFile or TextFile data. Value contains
                            // filename and type data which is used to create: String, Bitmap or Byte[]
                            if (resourceType == "FileResource")
                                resourceValue = LoadFileResource(reader);
                            else
                            {
                                LosFormatter Formatter = new LosFormatter();
                                resourceValue = Formatter.Deserialize(resourceValue as string);
                            }
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
                else if (TypeInfo.IndexOf("System.Drawing.Bitmap") > -1)
                {
                    MemoryStream ms = new MemoryStream(reader["BinFile"] as byte[]);
                    value = new Bitmap(ms);
                    ms.Close();
                }
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
        public virtual List<ResourceItem> GetAllResources(bool localResources = false)
        {
            IEnumerable<ResourceItem> items;
            using (var data = GetDb())
            {
                
                string sql = "select ResourceId,Value,LocaleId,ResourceSet,Type,TextFile,BinFile,FileName,Comment from " + Configuration.ResourceTableName +
                             " where ResourceSet " +
                             (!localResources ? "not" : string.Empty) + " like @ResourceSet ORDER by ResourceSet,LocaleId";

                items = data.Query<ResourceItem>(sql, data.CreateParameter("@ResourceSet", "%.%"));

                if (items == null)
                {
                    ErrorMessage = data.ErrorMessage;
                    return null;
                }

                return items.ToList();
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
                string sql =
                    @"select ResourceId,CAST( MAX( 
	  case  
		WHEN len( CAST(Value as nvarchar(10))) > 0 THEN 1
		ELSE 0
	  end ) as Bit) as HasValue
	  	from " + Configuration.ResourceTableName +
                    @" where ResourceSet=@ResourceSet 
	group by ResourceId";

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
        /// Returns an DataTable called TResourceIds with ResourceId and HasValues fields
        /// where the ResourceId is formatted for HTML display.
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public virtual List<ListItem> GetAllResourceIdsForHtmlDisplay(string ResourceSet)
        {
            var resourceIds = GetAllResourceIds(ResourceSet);
            if (resourceIds == null)
                return null;

            List<ListItem> items = new List<ListItem>();

            string lastId = "xx";
            foreach (var resId in resourceIds)
            {
                string resourceId = resId.ResourceId;
                ListItem item = new ListItem(resourceId);                

                string[] tokens = resourceId.Split('.');
                if (tokens.Length == 1)
                {
                    lastId = tokens[0];
                }
                else
                {
                    if (lastId == tokens[0])
                    {
                        item.Attributes.Add("style", "color: maroon; margin-left: 20px;");                        
                    }
                    lastId = tokens[0];
                }

                items.Add(item);
            }

            return items;
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

            using (var data = new SqlDataAccess(Configuration.ConnectionString))
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

                    string Type = reader["Type"] as string;

                    if (string.IsNullOrEmpty(Type))
                        result = reader["Value"] as string;
                    else
                    {
                        LosFormatter Formatter = new LosFormatter();
                        result = Formatter.Deserialize(reader["Value"] as string);
                    }
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
                        "select ResourceId, Value,Comment from " + Configuration.ResourceTableName +
                        " where ResourceId=@ResourceId and ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                        data.CreateParameter("@ResourceId", resourceId),
                        data.CreateParameter("@ResourceSet", resourceSet),
                        data.CreateParameter("@LocaleId", cultureName)))
                {
                    if (reader == null || !reader.Read())
                        return null;

                    item = new ResourceItem()
                    {
                        ResourceId = reader["ResourceId"] as string,
                        Value = reader["Value"] as string,
                        Comment = reader["Comment"] as string
                    };

                    reader.Close();
                }
            }
            
            return item;
        }

        /// <summary>
        /// Returns all the resource strings for all cultures for a specific resource Id.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetResourceStrings(string resourceId, string resourceSet)
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
            }

            return Resources;
        }


        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>
        public virtual int UpdateOrAdd(string resourceId, object value, string cultureName, string resourceSet, string comment)
        {
            return UpdateOrAdd(resourceId, value, cultureName, resourceSet, comment, false);
        }


        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>
        public virtual int UpdateOrAdd(string resourceId, object value, string cultureName, string resourceSet,
            string comment, bool valueIsFileName)
        {
            if (!IsValidCulture(cultureName))
            {
                ErrorMessage = string.Format(Resources.Resources.Can_t_save_resource__Invalid_culture_id_passed, cultureName);
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
        /// <param name="resourceId">The resource key name</param>
        /// <param name="value">Value to set it to. Can also be a file which is loaded as binary data when valueIsFileName is true</param>
        /// <param name="cultureName">name of the culture or null for invariant/default</param>
        /// <param name="resourceSet">The ResourceSet to which the resource id is added</param>
        /// <param name="comment">Optional comment for the key</param>
        /// <param name="valueIsFileName">if true the Value property is a filename to import</param>
        public virtual int AddResource(string resourceId, object value, string cultureName, string resourceSet, string comment = null, bool valueIsFileName = false)
        {
            string Type = string.Empty;

            if (cultureName == null)
                cultureName = string.Empty;

            if (string.IsNullOrEmpty(resourceId))
            {
                ErrorMessage = Resources.Resources.NoResourceIdSpecifiedCantAddResource;
                return -1;
            }

            using (var data = GetDb())
            {
                if (Transaction != null)
                    data.Transaction = Transaction;

                if (value != null && !(value is string))
                {
                    Type = value.GetType().AssemblyQualifiedName;
                    try
                    {
                        LosFormatter output = new LosFormatter();
                        StringWriter writer = new StringWriter();
                        output.Serialize(writer, value);
                        value = writer.ToString();
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

                DbParameter BinFileParm = data.CreateParameter("@BinFile", BinFile, DbType.Binary);
                DbParameter TextFileParm = data.CreateParameter("@TextFile", TextFile);

                string Sql = "insert into " + Configuration.ResourceTableName + " (ResourceId,Value,LocaleId,Type,Resourceset,BinFile,TextFile,Filename,Comment) Values (@ResourceID,@Value,@LocaleId,@Type,@ResourceSet,@BinFile,@TextFile,@FileName,@Comment)";
                if (data.ExecuteNonQuery(Sql,
                    data.CreateParameter("@ResourceId", resourceId),
                    data.CreateParameter("@Value", value),
                    data.CreateParameter("@LocaleId", cultureName),
                    data.CreateParameter("@Type", Type),
                    data.CreateParameter("@ResourceSet", resourceSet),
                    BinFileParm, TextFileParm,
                    data.CreateParameter("@FileName", FileName),
                    data.CreateParameter("@Comment", comment)) == -1)
                {
                    ErrorMessage = data.ErrorMessage;
                    return -1;
                }
            }

            return 1;
        }



        /// <summary>
        /// Updates an existing resource in the Localization table
        /// </summary>
        /// <param name="ResourceId">The Resource id to update</param>
        /// <param name="Value">The value to set it to</param>
        /// <param name="CultureName">The 2 (en) or 5 character (en-us)culture. Or "" for Invariant </param>
        /// <param name="ResourceSet">The name of the resourceset.</param>
        /// <param name="Type"></param>
        /// <
        public virtual int UpdateResource(string ResourceId, object Value, string CultureName, string ResourceSet, string Comment)
        {
            return UpdateResource(ResourceId, Value, CultureName, ResourceSet, Comment, false);
        }

        /// <summary>
        /// Updates an existing resource in the Localization table
        /// </summary>
        /// <param name="ResourceId"></param>
        /// <param name="Value"></param>
        /// <param name="CultureName"></param>
        /// <param name="ResourceSet"></param>
        /// <param name="Type"></param>
        public virtual int UpdateResource(string ResourceId, object Value, string CultureName, string ResourceSet, string Comment, bool ValueIsFileName)
        {
            string type;
            if (CultureName == null)
                CultureName = string.Empty;

            int result;
            using (var data = GetDb())
            {
                if (Transaction != null)
                    data.Transaction = Transaction;

                if (Value != null && !(Value is string))
                {
                    type = Value.GetType().AssemblyQualifiedName;
                    try
                    {
                        LosFormatter output = new LosFormatter();
                        StringWriter writer = new StringWriter();
                        output.Serialize(writer, Value);
                        Value = writer.ToString();
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

                    if (Value == null)
                        Value = string.Empty;
                }

                byte[] BinFile = null;
                string TextFile = null;
                string FileName = string.Empty;

                if (ValueIsFileName)
                {
                    FileInfoFormat FileData = null;
                    try
                    {
                        FileData = GetFileInfo(Value as string);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ex.Message;
                        return -1;
                    }

                    type = "FileResource";
                    Value = FileData.ValueString;
                    FileName = FileData.FileName;

                    if (FileData.FileFormatType == FileFormatTypes.Text)
                        TextFile = FileData.TextContent;
                    else
                        BinFile = FileData.BinContent;
                }

                if (Value == null)
                    Value = string.Empty;

                // Set up Binfile and TextFile parameters which are set only for
                // file values - otherwise they'll pass as Null values.
                var binFileParm = data.CreateParameter("@BinFile", BinFile, DbType.Binary);

                var textFileParm = data.CreateParameter("@TextFile", TextFile);
                

                string sql = "update " + Configuration.ResourceTableName + " set Value=@Value, Type=@Type, BinFile=@BinFile,TextFile=@TextFile,FileName=@FileName, Comment=@Comment " +
                             "where LocaleId=@LocaleId AND ResourceSet=@ResourceSet and ResourceId=@ResourceId";
                result = data.ExecuteNonQuery(sql,
                    data.CreateParameter("@ResourceId", ResourceId),
                    data.CreateParameter("@Value", Value),
                    data.CreateParameter("@Type", type),
                    data.CreateParameter("@LocaleId", CultureName),
                    data.CreateParameter("@ResourceSet", ResourceSet),
                    binFileParm, textFileParm,
                    data.CreateParameter("@FileName", FileName),
                    data.CreateParameter("@Comment", Comment)
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
        /// <param name="FileName"></param>
        /// <returns></returns>
        private FileInfoFormat GetFileInfo(string FileName)
        {
            FileInfoFormat Details = new FileInfoFormat();

            FileInfo fi = new FileInfo(FileName);
            if (!fi.Exists)
                throw new InvalidOperationException("Invalid Filename");

            string Extension = fi.Extension.ToLower().TrimStart('.');
            Details.FileName = fi.Name;

            if (Extension == "txt" || Extension == "css" || Extension == "js")
            {
                Details.FileFormatType = FileFormatTypes.Text;
                using (StreamReader sr = new StreamReader(FileName, Encoding.Default, true))
                {
                    Details.TextContent = sr.ReadToEnd();
                }
                Details.ValueString = Details.FileName + ";" + typeof(string).AssemblyQualifiedName + ";" + Encoding.Default.HeaderName;
            }
            else if (Extension == "gif" || Extension == "jpg" || Extension == "bmp" || Extension == "png")
            {
                Details.FileFormatType = FileFormatTypes.Image;
                Details.BinContent = File.ReadAllBytes(FileName);
                Details.ValueString = Details.FileName + ";" + typeof(Bitmap).AssemblyQualifiedName;
            }
            else
            {
                Details.BinContent = File.ReadAllBytes(FileName);
                Details.ValueString = Details.FileName + ";" + typeof(Byte[]).AssemblyQualifiedName;
            }

            return Details;
        }

        internal enum FileFormatTypes
        {
            Text,
            Image,
            Binary
        }

        /// <summary>
        /// Internal structure that contains format information about a file
        /// resource. Used internally to figure out how to write 
        /// a resource into the database
        /// </summary>
        internal class FileInfoFormat
        {
            public string FileName = string.Empty;
            public string Encoding = string.Empty;
            public byte[] BinContent = null;
            public string TextContent = string.Empty;
            public FileFormatTypes FileFormatType = FileFormatTypes.Binary;
            public string ValueString = string.Empty;
            public string Type = "File";
        }


        /// <summary>
        /// Deletes a specific resource ID based on ResourceId, ResourceSet and Culture.
        /// If an empty culture is passed the entire group is removed (ie. all locales).
        /// </summary>
        /// <param name="resourceId">Resource Id to delete</param>
        /// <param name="cultureName">language ID - if empty all languages are deleted</param>e
        /// <param name="resourceSet">The resource set to remove</param>
        /// <returns></returns>
        public bool DeleteResource(string resourceId, string cultureName = null, string resourceSet = null)
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
        public bool RenameResource(string ResourceId, string NewResourceId, string ResourceSet)
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
        public bool RenameResourceProperty(string Property, string NewProperty, string ResourceSet)
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
        public bool DeleteResourceSet(string ResourceSet)
        {
            if (string.IsNullOrEmpty(ResourceSet))
                return false;

            using (var data = GetDb())
            {
                var result = data.ExecuteNonQuery("delete from " + Configuration.ResourceTableName + 
                                                  " where ResourceSet=@ResourceSet",
                    data.CreateParameter("@ResourceSet", ResourceSet));
                if (result < 0)
                {
                    ErrorMessage = data.ErrorMessage;
                    return false;
                }
                if (result > 0)
                    return true;
            }

            return false;
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
        public bool RenameResourceSet(string OldResourceSet, string NewResourceSet)
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
                    SetError(Resources.Resources.No_matching_Recordset_found_);
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
        public bool ResourceExists(string ResourceId, string CultureName, string ResourceSet)
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
        public bool IsValidCulture(string IetfTag)
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
        public bool GenerateResources(IDictionary resourceList, string cultureName, string resourceSet, bool deleteAllResourceFirst)
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
                                Result = UpdateOrAdd(Entry.Key.ToString(), Entry.Value, cultureName, resourceSet, null);
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
        /// <param name="ResourceSet">ResourceSet name. Pass NULL for locale Resources</param>
        /// <param name="LocaleId"></param>
        public string GetResourcesAsJavascriptObject(string javaScriptVarName, string ResourceSet, string LocaleId)
        {
            if (LocaleId == null)
                LocaleId = CultureInfo.CurrentUICulture.IetfLanguageTag;
            if (ResourceSet == null)
                ResourceSet = WebUtils.GetAppRelativePath();

            IDictionary resources = GetResourceSetNormalizedForLocaleId(
                LocaleId, ResourceSet);

            // Filter the list to non-control resources 
            Dictionary<string, string> localRes = new Dictionary<string, string>();
            foreach (string key in resources.Keys)
            {
                // We're only interested in non control local resources 
                if (!key.Contains(".") && resources[key] is string)
                    localRes.Add(key, resources[key] as string);
            }

            JSONSerializer ser = new JSONSerializer();
            ser.FormatJsonOutput = HttpContext.Current.IsDebuggingEnabled;
            string json = ser.Serialize(localRes);

            return "var " + javaScriptVarName + " = " + json + ";\r\n";
        }


        /// <summary>
        /// Checks to see if the LocalizationTable exists
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool IsLocalizationTable(string TableName)
        {
            if (TableName == null)
                TableName = Configuration.ResourceTableName;

            using (var data = GetDb())
            {
                var Pk = data.ExecuteScalar("select count(pk) from " + TableName);

                if (Pk is int)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Create a backup of the localization database.
        /// 
        /// Note the table used is the one specified in the Configuration.ResourceTableName
        /// </summary>
        /// <param name="BackupTableName">Table of the backup table. Null creates a _Backup table.</param>
        /// <returns></returns>
        public bool CreateBackupTable(string BackupTableName)
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
        public bool RestoreBackupTable(string backupTableName)
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
                SetError(Resources.Resources.LocalizationTable_Localization_Table_exists_already);
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

       

        protected void SetError()
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

        protected void SetError(Exception ex, bool checkInner = false)
        {
            if (ex == null)
                ErrorMessage = string.Empty;

            Exception e = ex;
            if (checkInner)
                e = e.GetBaseException();

            ErrorMessage = e.Message;
        }

        protected void SetError(Exception ex)
        {
            SetError(ex, false);
        }


    }
}