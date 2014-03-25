using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Drawing;
using System.Data.Common;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using System.Web.UI.WebControls;
using Westwind.Web.JsonSerializers;
using Westwind.Web;

namespace Westwind.Globalization
{

    /// <summary>
    /// This class provides the Data Access to the database
    /// for the DbResourceManager, Provider and design time
    /// services. This class acts as a Business layer
    /// and uses the SqlDataAccess DAL for its data access.
    /// 
    /// Dependencies:
    /// DbResourceConfiguration   (holds and reads all config data from .Current)
    /// SqlDataAccess             (provides a data access (DAL))
    /// </summary>
    public class DbResourceDataManager
    {
        //public static string ConnectionString = string.Empty;
        //public static string ResourceTableName = "Localizations";

        protected DbTransaction Transaction = null;

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

        /// <summary>
        /// Default constructor. Instantiates with the default connection string
        /// which is loaded from the configuration section.
        /// </summary>
        public DbResourceDataManager()
        {
        }

        /// <summary>
        /// Returns a specific set of resources for a given culture and 'resource set' which
        /// in this case is just the virtual directory and culture.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public IDictionary GetResourceSet(string cultureName, string resourceSet)
        {
            if (cultureName == null)
                cultureName = string.Empty;

            string resourceFilter;
            resourceFilter = " ResourceSet=@ResourceSet";

            var resources = new Dictionary<string, object>();

            using (var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                DbDataReader reader;

                if (string.IsNullOrEmpty(cultureName))
                    reader = data.ExecuteReader("select ResourceId,Value,Type,BinFile,TextFile,FileName from " + DbResourceConfiguration.Current.ResourceTableName + " where " + resourceFilter + " and (LocaleId is null OR LocaleId = '') order by ResourceId",
                                                data.CreateParameter("@ResourceSet", resourceSet));
                else
                    reader = data.ExecuteReader("select ResourceId,Value,Type,BinFile,TextFile,FileName from " + DbResourceConfiguration.Current.ResourceTableName + " where " + resourceFilter + " and LocaleId=@LocaleId order by ResourceId",
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

                        resources.Add(reader["ResourceId"].ToString(), resourceValue);
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
        public Dictionary<string, object> GetResourceSetNormalizedForLocaleId(string cultureName, string resourceSet)
        {
            if (cultureName == null)
                cultureName = string.Empty;

            Dictionary<string, object> resDictionary = new Dictionary<string, object>();

            SqlDataAccess data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);
            DbDataReader reader = null;

            string sql =
            @"select resourceId, LocaleId, Value, Type, BinFile, TextFile, FileName
    from " + DbResourceConfiguration.Current.ResourceTableName + @"
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
            object Value = null;

            try
            {
                string TypeInfo = reader["Value"] as string;

                if (TypeInfo.IndexOf("System.String") > -1)
                {
                    Value = reader["TextFile"] as string;
                }
                else if (TypeInfo.IndexOf("System.Drawing.Bitmap") > -1)
                {
                    MemoryStream ms = new MemoryStream(reader["BinFile"] as byte[]);
                    Value = new Bitmap(ms);
                    ms.Close();
                }
                else
                {
                    Value = reader["BinFile"] as byte[];
                }
            }
            catch (Exception ex)
            {
                SetError(reader["ResourceKey"].ToString() + ": " + ex.Message);
            }



            return Value;
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
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public DataTable GetAllResources(bool LocalResources)
        {
            SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            string Sql = string.Empty;

            Sql = "select ResourceId,Value,LocaleId,ResourceSet,Type,TextFile,BinFile,FileName,Comment from " + DbResourceConfiguration.Current.ResourceTableName +
                  " where ResourceSet " +
                  (!LocalResources ? "not" : string.Empty) + " like @ResourceSet ORDER by ResourceSet,LocaleId";

            DataTable dt = Data.ExecuteTable("TResources",
                                             Sql,
                                            Data.CreateParameter("@ResourceSet", "%.%"));

            if (dt == null)
            {
                ErrorMessage = Data.ErrorMessage;
                return null;
            }

            return dt;
        }



        /// <summary>
        /// Returns a data table of all the resources for all locales. The result is in a 
        /// table called TResources that contains all fields of the table. The table is
        /// ordered by LocaleId.
        /// 
        /// This version returns ALL resources
        /// 
        /// Fields:
        /// ResourceId,Value,LocaleId,ResourceSet,Type
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllResources()
        {
            DataTable dt;
            using (SqlDataAccess data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                string sql = "select ResourceId,Value,LocaleId,ResourceSet,Type,TextFile,BinFile,FileName,Comment from " +
                             DbResourceConfiguration.Current.ResourceTableName +
                             " ORDER by ResourceSet,LocaleId";

                dt = data.ExecuteTable("TResources", sql, data.CreateParameter("@ResourceSet", "%.%"));

                if (dt == null)
                {
                    SetError(data.ErrorMessage);
                    return null;
                }
            }

            return dt;
        }


        /// <summary>
        /// Returns all available resource ids for a given resource set in all languages.
        /// 
        /// Returns a DataTable called TResoureIds with ResourecId and HasValue fields
        /// HasValue returns whether there are any entries in any culture for this
        /// resourceId
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public DataTable GetAllResourceIds(string resourceSet)
        {
            using (var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                string sql =
                    @"select resourceId,CAST( MAX( 
	  case  
		WHEN len( CAST(Value as varchar(max))) > 0 THEN 1
		ELSE 0
	  end ) as Bit) as HasValue
	  	from " + DbResourceConfiguration.Current.ResourceTableName +
                    @" where ResourceSet=@ResourceSet 
	group by ResourceId";

                var dt = data.ExecuteTable("TResourceIds", sql,
                                         data.CreateParameter("@ResourceSet", resourceSet));
                if (dt == null)
                {
                    SetError(data.ErrorMessage);
                    return null;
                }

                return dt;
            }
        }

        /// <summary>
        /// Returns an DataTable called TResourceIds with ResourceId and HasValues fields
        /// where the ResourceId is formatted for HTML display.
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public ListItem[] GetAllResourceIdsForHtmlDisplay(string ResourceSet)
        {
            DataTable dt = GetAllResourceIds(ResourceSet);
            if (dt == null)
                return null;

            List<ListItem> items = new List<ListItem>();

            string lastId = "xx";
            foreach (DataRow row in dt.Rows)
            {
                string resourceId = row["ResourceId"] as string;
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

            return items.ToArray();
        }

        /// <summary>
        /// Returns all available resource sets
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllResourceSets(ResourceListingTypes Type)
        {
            SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);
            DataTable dt = null;

            if (Type == ResourceListingTypes.AllResources)
                dt = Data.ExecuteTable("TResourcesets", "select ResourceSet as ResourceSet from " + DbResourceConfiguration.Current.ResourceTableName + " group by ResourceSet");
            else if (Type == ResourceListingTypes.LocalResourcesOnly)
                dt = Data.ExecuteTable("TResourcesets", "select ResourceSet as ResourceSet from " + DbResourceConfiguration.Current.ResourceTableName + " where resourceset like @ResourceSet group by ResourceSet",
                                 Data.CreateParameter("@ResourceSet", "%.%"));
            else if (Type == ResourceListingTypes.GlobalResourcesOnly)
                dt = Data.ExecuteTable("TResourcesets", "select ResourceSet as ResourceSet from " + DbResourceConfiguration.Current.ResourceTableName + " where resourceset not like @ResourceSet group by ResourceSet",
                                 Data.CreateParameter("@ResourceSet", "%.%"));

            if (dt == null)
                ErrorMessage = Data.ErrorMessage;

            return dt;
        }

        /// <summary>
        /// Gets all the locales for a specific resource set.
        /// 
        /// Returns a table named TLocaleIds (LocaleId field)
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public DataTable GetAllLocaleIds(string resourceSet)
        {
            if (resourceSet == null)
                resourceSet = string.Empty;

            using (SqlDataAccess data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                return data.ExecuteTable("TLocaleIds", "select LocaleId,'' as Language from " + DbResourceConfiguration.Current.ResourceTableName +
                                                       " where ResourceSet=@ResourceSet group by LocaleId",
                                         data.CreateParameter("@ResourceSet", resourceSet));
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
        public DataTable GetAllResourcesForCulture(string resourceSet, string cultureName)
        {
            if (cultureName == null)
                cultureName = string.Empty;

            using (var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                return data.ExecuteTable("TResources",
                                         "select ResourceId, Value from " + DbResourceConfiguration.Current.ResourceTableName + " where ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                                         data.CreateParameter("@ResourceSet", resourceSet),
                                         data.CreateParameter("@LocaleId", cultureName));
            }
        }


        /// <summary>
        /// Returns an individual Resource String from the database
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>       
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public string GetResourceString(string resourceId, string resourceSet, string cultureName)
        {
            SetError();

            if (cultureName == null)
                cultureName = string.Empty;

            object result;
            using (var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                result = data.ExecuteScalar("select Value from " + DbResourceConfiguration.Current.ResourceTableName +
                                            " where ResourceId=@ResourceId and ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                                            data.CreateParameter("@ResourceId", resourceId),
                                            data.CreateParameter("@ResourceSet", resourceSet),
                                            data.CreateParameter("@LocaleId", cultureName));
            }

            return result as string;
        }

        /// <summary>
        /// Returns a resource item that returns both the Value and Comment to the
        /// fields to the client.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public ResourceItem GetResourceItem(string resourceId, string resourceSet, string cultureName)
        {
            ErrorMessage = string.Empty;

            if (cultureName == null)
                cultureName = string.Empty;

            using (SqlDataAccess data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {

                using (IDataReader reader =
                               data.ExecuteReader("select ResourceId, Value,Comment from " + DbResourceConfiguration.Current.ResourceTableName + " where ResourceId=@ResourceId and ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                                   data.CreateParameter("@ResourceId", resourceId),
                                   data.CreateParameter("@ResourceSet", resourceSet),
                                   data.CreateParameter("@LocaleId", cultureName)))
                {
                    if (reader == null || !reader.Read())
                        return null;

                    ResourceItem item = new ResourceItem()
                    {
                        ResourceId = reader["ResourceId"] as string,
                        Value = reader["Value"] as string,
                        Comment = reader["Comment"] as string
                    };

                    reader.Close();

                    return item;
                }
            }
        }

        /// <summary>
        /// Returns all the resource strings for all cultures.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetResourceStrings(string resourceId, string resourceSet)
        {
            var Resources = new Dictionary<string, string>();
            var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            using (DbDataReader reader = data.ExecuteReader("select Value,LocaleId from " + DbResourceConfiguration.Current.ResourceTableName +
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

            return Resources;
        }

        /// <summary>
        /// Returns an object from the Resources. Use this for any non-string
        /// types. While this method can be used with strings GetREsourceString
        /// is much more efficient.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceSet"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public object GetResourceObject(string resourceId, string resourceSet, string cultureName)
        {
            object result = null;
            SetError();

            if (cultureName == null)
                cultureName = string.Empty;

            var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            DbDataReader reader = data.ExecuteReader("select Value,Type from " + DbResourceConfiguration.Current.ResourceTableName + " where ResourceId=@ResourceId and ResourceSet=@ResourceSet and LocaleId=@LocaleId",
                               data.CreateParameter("@ResourceId", resourceId),
                               data.CreateParameter("@ResourceSet", resourceSet),
                               data.CreateParameter("@LocaleId", cultureName));
            if (reader == null)
                return null;

            if (reader.HasRows)
            {
                reader.Read();

                string Type = reader["Type"] as string;

                if (string.IsNullOrEmpty(Type))
                    result = reader["Value"] as string;
                else
                {
                    LosFormatter Formatter = new LosFormatter();
                    result = Formatter.Deserialize(reader["Value"] as string);
                }
            }

            reader.Dispose();

            return result;
        }

        /// <summary>
        /// Updates a resource if it exists, if it doesn't one is created
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>
        public int UpdateOrAdd(string resourceId, object value, string cultureName, string resourceSet, string comment)
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
        public int UpdateOrAdd(string resourceId, object value, string cultureName, string resourceSet,
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
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>
        /// <param name="Filename"></param>
        public int AddResource(string resourceId, object value, string cultureName, string resourceSet, string comment)
        {
            return AddResource(resourceId, value, cultureName, resourceSet, comment, false);
        }

        /// <summary>
        /// Adds a resource to the Localization Table
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <param name="Type"></param>
        /// <param name="Filename"></param>
        /// <param name="valueIsFileName">if true the Value property is a filename to import</param>
        public int AddResource(string resourceId, object value, string cultureName, string resourceSet, string comment, bool valueIsFileName)
        {
            string Type = string.Empty;

            if (cultureName == null)
                cultureName = string.Empty;

            if (string.IsNullOrEmpty(resourceId))
            {
                ErrorMessage = "No ResourceId specified. Can't add resource";
                return -1;
            }

            SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            if (Transaction != null)
                Data.Transaction = Transaction;

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

            DbParameter BinFileParm = Data.CreateParameter("@BinFile", BinFile, DbType.Binary);
            DbParameter TextFileParm = Data.CreateParameter("@TextFile", TextFile);

            string Sql = "insert into " + DbResourceConfiguration.Current.ResourceTableName + " (ResourceId,Value,LocaleId,Type,Resourceset,BinFile,TextFile,Filename,Comment) Values (@ResourceID,@Value,@LocaleId,@Type,@ResourceSet,@BinFile,@TextFile,@FileName,@Comment)";
            if (Data.ExecuteNonQuery(Sql,
                                   Data.CreateParameter("@ResourceId", resourceId),
                                   Data.CreateParameter("@Value", value),
                                   Data.CreateParameter("@LocaleId", cultureName),
                                   Data.CreateParameter("@Type", Type),
                                   Data.CreateParameter("@ResourceSet", resourceSet),
                                   BinFileParm, TextFileParm,
                                   Data.CreateParameter("@FileName", FileName),
                                   Data.CreateParameter("@Comment", comment)) == -1)
            {
                ErrorMessage = Data.ErrorMessage;
                return -1;
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
        public int UpdateResource(string ResourceId, object Value, string CultureName, string ResourceSet, string Comment)
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
        public int UpdateResource(string ResourceId, object Value, string CultureName, string ResourceSet, string Comment, bool ValueIsFileName)
        {
            string Type = string.Empty;
            if (CultureName == null)
                CultureName = string.Empty;

            SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);
            if (Transaction != null)
                Data.Transaction = Transaction;

            if (Value != null && !(Value is string))
            {
                Type = Value.GetType().AssemblyQualifiedName;
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
                Type = string.Empty;

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

                Type = "FileResource";
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
            DbParameter BinFileParm = Data.CreateParameter("@BinFile", BinFile, DbType.Binary);

            DbParameter TextFileParm = Data.CreateParameter("@TextFile", TextFile);

            int Result = 0;

            string Sql = "update " + DbResourceConfiguration.Current.ResourceTableName + " set Value=@Value, Type=@Type, BinFile=@BinFile,TextFile=@TextFile,FileName=@FileName, Comment=@Comment " +
                         "where LocaleId=@LocaleId AND ResourceSet=@ResourceSet and ResourceId=@ResourceId";
            Result = Data.ExecuteNonQuery(Sql,
                               Data.CreateParameter("@ResourceId", ResourceId),
                               Data.CreateParameter("@Value", Value),
                               Data.CreateParameter("@Type", Type),
                               Data.CreateParameter("@LocaleId", CultureName),
                               Data.CreateParameter("@ResourceSet", ResourceSet),
                                BinFileParm, TextFileParm,
                               Data.CreateParameter("@FileName", FileName),
                               Data.CreateParameter("@Comment", Comment)
                               );
            if (Result == -1)
            {
                ErrorMessage = Data.ErrorMessage;
                return -1;
            }

            return Result;
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

            using (SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                if (!string.IsNullOrEmpty(cultureName))
                    // Delete the specific entry only
                    Result = Data.ExecuteNonQuery("delete from " + DbResourceConfiguration.Current.ResourceTableName +
                                                  " where ResourceId=@ResourceId and LocaleId=@LocaleId and ResourceSet=@ResourceSet",
                                                  Data.CreateParameter("@ResourceId", resourceId),
                                                  Data.CreateParameter("@LocaleId", cultureName),
                                                  Data.CreateParameter("@ResourceSet", resourceSet));
                else
                    // If we're deleting the invariant entry - delete ALL of the languages for this key
                    Result = Data.ExecuteNonQuery("delete from " + DbResourceConfiguration.Current.ResourceTableName +
                                                  " where ResourceId=@ResourceId and ResourceSet=@ResourceSet",
                                                  Data.CreateParameter("@ResourceId", resourceId),
                                                  Data.CreateParameter("@ResourceSet", resourceSet));

                if (Result == -1)
                {
                    ErrorMessage = Data.ErrorMessage;
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
            SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            int Result = Data.ExecuteNonQuery("update " + DbResourceConfiguration.Current.ResourceTableName + " set ResourceId=@NewResourceId where ResourceId=@ResourceId AND ResourceSet=@ResourceSet",
                               Data.CreateParameter("@ResourceId", ResourceId),
                               Data.CreateParameter("@NewResourceId", NewResourceId),
                               Data.CreateParameter("@ResourceSet", ResourceSet));
            if (Result == -1)
            {
                ErrorMessage = Data.ErrorMessage;
                return false;
            }
            if (Result == 0)
            {
                ErrorMessage = "Invalid ResourceId";
                return false;
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
            using (SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                Property += ".";
                NewProperty += ".";
                string PropertyQuery = Property + "%";
                int Result = Data.ExecuteNonQuery("update " + DbResourceConfiguration.Current.ResourceTableName + " set ResourceId=replace(resourceid,@Property,@NewProperty) where ResourceSet=@ResourceSet and ResourceId like @PropertyQuery",
                                                  Data.CreateParameter("@Property", Property),
                                                  Data.CreateParameter("@NewProperty", NewProperty),
                                                  Data.CreateParameter("@ResourceSet", ResourceSet),
                                                  Data.CreateParameter("@PropertyQuery", PropertyQuery));
                if (Result == -1)
                {
                    ErrorMessage = Data.ErrorMessage;
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

            int Result = -1;

            using (SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                Result = Data.ExecuteNonQuery("delete from " + DbResourceConfiguration.Current.ResourceTableName + " where ResourceSet=@ResourceSet",
                                              Data.CreateParameter("@ResourceSet", ResourceSet));
                if (Result < 0)
                {
                    ErrorMessage = Data.ErrorMessage;
                    return false;
                }
                if (Result > 0)
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
            using (SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                int Result = Data.ExecuteNonQuery("update " + DbResourceConfiguration.Current.ResourceTableName + " set ResourceSet=@NewResourceSet where ResourceSet=@OldResourceSet",
                                                  Data.CreateParameter("@NewResourceSet", NewResourceSet),
                                                  Data.CreateParameter("@OldResourceSet", OldResourceSet));
                if (Result == -1)
                {
                    ErrorMessage = Data.ErrorMessage;
                    return false;
                }
                if (Result == 0)
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

            object Result = null;
            using (SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                Result = Data.ExecuteScalar("select ResourceId from " + DbResourceConfiguration.Current.ResourceTableName + " where ResourceId=@ResourceId and LocaleID=@LocaleId and ResourceSet=@ResourceSet group by ResourceId",
                                            Data.CreateParameter("@ResourceId", ResourceId),
                                            Data.CreateParameter("@LocaleId", CultureName),
                                            Data.CreateParameter("@ResourceSet", ResourceSet));
                if (Result == null)
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
        /// <param name="ResourceList"></param>
        /// <param name="CultureName"></param>
        /// <param name="BaseName"></param>
        public bool GenerateResources(IDictionary ResourceList, string CultureName, string BaseName, bool DeleteAllResourceFirst)
        {
            if (ResourceList == null)
                throw new InvalidOperationException("No Resources");

            if (CultureName == null)
                CultureName = string.Empty;


            using (SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {
                if (!Data.BeginTransaction())
                    return false;
                // Set transaction to be shared by other methods
                Transaction = Data.Transaction;
                try
                {
                    // First delete all resources for this resource set
                    if (DeleteAllResourceFirst)
                    {
                        int Result = Data.ExecuteNonQuery("delete " + DbResourceConfiguration.Current.ResourceTableName + " where LocaleId=@LocaleId and ResourceSet=@ResourceSet",
                                                          Data.CreateParameter("@LocaleId", CultureName),
                                                          Data.CreateParameter("@ResourceSet", BaseName));
                        if (Result == -1)
                        {
                            Data.RollbackTransaction();
                            return false;
                        }
                    }
                    // Now add them all back in one by one
                    foreach (DictionaryEntry Entry in ResourceList)
                    {
                        if (Entry.Value != null)
                        {
                            int Result = 0;
                            if (DeleteAllResourceFirst)
                                Result = AddResource(Entry.Key.ToString(), Entry.Value, CultureName, BaseName, null);
                            else
                                Result = UpdateOrAdd(Entry.Key.ToString(), Entry.Value, CultureName, BaseName, null);
                            if (Result == -1)
                            {
                                Data.RollbackTransaction();
                                return false;
                            }
                        }
                    }
                }
                catch
                {
                    Data.RollbackTransaction();
                    return false;
                }
                Data.CommitTransaction();
            }

            // Clear out the resources
            ResourceList = null;

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
        /// Creates an global JavaScript object object that holds all non-control 
        /// local string resources as property values and embeds this object
        /// directly into an ASP.NET page.
        /// </summary>
        public void EmbedResourcesAsJavascriptObject(string javaScriptVarName, string ResourceSet, Page page)
        {
            string script = GetResourcesAsJavascriptObject(javaScriptVarName, ResourceSet, null);
            ClientScriptProxy.Current.RegisterClientScriptBlock(page, typeof(Page), javaScriptVarName + "_res", script, true);
        }



        /// <summary>
        /// Checks to see if the LocalizationTable exists
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool IsLocalizationTable(string TableName)
        {
            if (TableName == null)
                TableName = DbResourceConfiguration.Current.ResourceTableName;

            SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            // Check for table existing already
            object Pk = Data.ExecuteScalar("select count(pk) from " + TableName);
            if (Pk is int)
                return true;

            return false;
        }


        /// <summary>
        /// Create a backup of the localization database.
        /// 
        /// Note the table used is the one specified in the DbResourceConfiguration.Current.ResourceTableName
        /// </summary>
        /// <param name="BackupTableName">Table of the backup table. Null creates a _Backup table.</param>
        /// <returns></returns>
        public bool CreateBackupTable(string BackupTableName)
        {
            if (BackupTableName == null)
                BackupTableName = DbResourceConfiguration.Current.ResourceTableName + "_Backup";

            var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            data.ExecuteNonQuery("drop table " + BackupTableName);
            if (data.ExecuteNonQuery("select * into " + BackupTableName + " from " + DbResourceConfiguration.Current.ResourceTableName) < 0)
            {
                ErrorMessage = data.ErrorMessage;
                return false;
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
                backupTableName = DbResourceConfiguration.Current.ResourceTableName + "_Backup";

            using (var data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString))
            {

                data.BeginTransaction();

                if (data.ExecuteNonQuery("delete from " + DbResourceConfiguration.Current.ResourceTableName) < 0)
                {
                    data.RollbackTransaction();
                    ErrorMessage = data.ErrorMessage;
                    return false;
                }

                string sql =
    @"insert into {0}
  (ResourceId,Value,LocaleId,ResourceSet,Type,BinFile,TextFile,FileName,Comment) 
   select ResourceId,Value,LocaleId,ResourceSet,Type,BinFile,TextFile,FileName,Comment from {1}";

                sql = string.Format(sql, DbResourceConfiguration.Current.ResourceTableName, backupTableName);

                if (data.ExecuteNonQuery(sql) < 0)
                {
                    data.RollbackTransaction();
                    ErrorMessage = data.ErrorMessage;
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
        public bool CreateLocalizationTable(string tableName = null)
        {
            if (tableName == null)
                tableName = DbResourceConfiguration.Current.ResourceTableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = "Localizations";

            string Sql = string.Format(TableCreationSql, tableName);

            // Check for table existing already
            if (IsLocalizationTable(tableName))
            {
                SetError(Resources.Resources.LocalizationTable_Localization_Table_exists_already);
                return false;
            }

            SqlDataAccess Data = new SqlDataAccess(DbResourceConfiguration.Current.ConnectionString);

            // Now execute the script one batch at a time
            if (!Data.RunSqlScript(Sql, false, false))
            {
                ErrorMessage = Data.ErrorMessage;
                return false;
            }

            return true;
        }

        public const string TableCreationSql =
@"SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING OFF
GO
CREATE TABLE [{0}] (
		[pk]              int NOT NULL IDENTITY(1, 1),
		[ResourceId]      nvarchar(1024) NOT NULL,
		[Value]           nvarchar(max) NULL,
		[LocaleId]        nvarchar(10) NULL,
		[ResourceSet]     nvarchar(512) NULL,
		[Type]            nvarchar(512) NULL,
		[BinFile]         varbinary(max) NULL,
		[TextFile]        nvarchar(max) NULL,
		[Filename]        nvarchar(128) NULL,
        [Comment]         nvarchar(512) NULL
)
ON [PRIMARY]
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [PK_{0}]
	PRIMARY KEY
	([pk])
	ON [PRIMARY]
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [DF_{0}_Filename]
	DEFAULT ('') FOR [Filename]
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [DF_{0}_LocaleId]
	DEFAULT ('') FOR [LocaleId]
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [DF_{0}_PageId]
	DEFAULT ('') FOR [ResourceSet]
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [DF_{0}_Text]
	DEFAULT ('') FOR [Value]
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [DF_{0}_Type]
	DEFAULT ('') FOR [Type]
GO

INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hello Cruel World','','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hallo schnöde Welt','de','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Bonjour tout le monde','fr','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Yesterday (invariant)','','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Gestern','de','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Hier','fr','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Today (invariant)','','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Heute','de','Resources')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Aujourd''hui','fr','Resources')
GO
";



        protected void SetError()
        {
            SetError("CLEAR");
        }

        protected void SetError(string message)
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

    /// <summary>
    /// Determines how hte GetAllResourceSets method returns its data
    /// </summary>
    public enum ResourceListingTypes
    {
        LocalResourcesOnly,
        GlobalResourcesOnly,
        AllResources
    }

    /// <summary>
    /// Returns a resource item that contains both Value and Comment
    /// </summary>
    public class ResourceItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The Id of the resource
        /// </summary>
        public string ResourceId
        {
            get { return _ResourceId; }
            set
            {
                _ResourceId = value;
                SendPropertyChanged("ResourceId");
            }
        }
        private string _ResourceId = null;

        /// <summary>
        /// The value of this resource
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                SendPropertyChanged("Value");
            }
        }
        private object _Value = null;


        /// <summary>
        /// The optional comment for this resource
        /// </summary>
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                SendPropertyChanged("Comment");
            }
        }
        private string _Comment = null;

        /// <summary>
        /// The localeId ("" invariant or "en-US", "de" etc). Note
        /// Empty means invariant or default locale.
        /// </summary>
        public string LocaleId
        {
            get { return _LocaleId; }
            set
            {
                _LocaleId = value;
                SendPropertyChanged("LocaleId");
            }
        }
        private string _LocaleId = string.Empty;


        /// <summary>
        /// The resource set (file) that this resource belongs to
        /// </summary>
        public string ResourceSet
        {
            get { return _ResourceSet; }
            set
            {
                _ResourceSet = value;
                SendPropertyChanged("ResourceSet");
            }
        }
        private string _ResourceSet = string.Empty;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
