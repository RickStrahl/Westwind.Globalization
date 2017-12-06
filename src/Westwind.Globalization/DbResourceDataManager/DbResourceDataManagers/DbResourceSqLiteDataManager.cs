using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Westwind.Globalization.Properties;
using Westwind.Utilities;
using Westwind.Utilities.Data;

namespace Westwind.Globalization
{
    /// <summary>
    /// Sql CE implementation of the Db SQL data provider
    /// </summary>
    /// <remarks>
    /// IMPORTANT: Make sure you add the System.Data.SQLite.Core
    /// NuGet Package to your project in order to have access 
    /// SqlLite
    /// </remarks>
    public class DbResourceSqLiteDataManager : DbResourceDataManager
    {

        /// <summary>
        /// Returns all available resource ids for a given resource set in all languages.
        /// 
        /// Returns a ResourceIdItem object with ResourecId and HasValue fields.
        /// HasValue returns whether there are any entries in any culture for this
        /// resourceId
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public override List<ResourceIdItem> GetAllResourceIds(string resourceSet)
        {
            using (var data = GetDb())
            {
                string sql = string.Format(
                    @"select ResourceId, CAST( MAX(length(Value)) > 0 as bit )   as HasValue 
	  	            from {0}
                    where ResourceSet=@ResourceSet 
		            group by 1", Configuration.ResourceTableName);

                var list = data.Query<ResourceIdItem>(sql, data.CreateParameter("@ResourceSet", resourceSet));
                if (list == null)
                {
                    SetError(data.ErrorMessage);
                    return null;
                }
                return list.ToList();
            }
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
        public override List<ResourceItem> GetAllResources(bool localResources = false, bool applyValueConverters = false, string resourceSet = null)
        {
            List<ResourceItem> items;
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


                using (var reader = data.ExecuteReader(sql, parms.ToArray()))
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
                        item.ResourceId = reader["ResourceId"] as string;
                        item.ResourceSet = reader["ResourceSet"] as string;
                        item.Value = reader["Value"];
                        item.LocaleId = reader["LocaleId"] as string;
                        item.Type = reader["Type"] as string;
                        item.TextFile = reader["TextFile"] as string;
                        item.BinFile = reader["BinFile"] as byte[];
                        item.Comment = reader["Comment"] as string;

                        var number = reader["ValueType"];  // int64 returned from Microsoft.Data.SqLite
                        if (number is int)
                            item.ValueType = (int) number;
                        else
                            item.ValueType = Convert.ToInt32(number);                        

                        var time = reader["Updated"];     // string return from Microsoft.Data.SqLite               
                        if (time == null)
                            item.Updated = DateTime.MinValue;
                        
                        if (time is DateTime)
                            item.Updated = (DateTime) time;
                        else
                            item.Updated = Convert.ToDateTime(time);

                        items.Add(item);
                    }
                }
                
                if (applyValueConverters && DbResourceConfiguration.Current.ResourceSetValueConverters.Count > 0)
                {
                    foreach (var resourceItem in items)
                    {
                        foreach (var convert in DbResourceConfiguration.Current.ResourceSetValueConverters)
                        {
                            if (resourceItem.ValueType == convert.ValueType)
                                resourceItem.Value = convert.Convert(resourceItem.Value, resourceItem.ResourceId);
                        }
                    }
                }

                return items;
            }
        }

        public override bool IsLocalizationTable(string tableName = null)
        {
            if (tableName == null)
                tableName = Configuration.ResourceTableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = "Localizations";

            string sql = "SELECT name FROM sqlite_master WHERE type = 'table' AND name='" + tableName + "'";            

            using (var data = GetDb())
            {
                
                var reader = data.ExecuteReader(sql, tableName);

                if (reader == null)
                    throw new InvalidOperationException(Resources.ConnectionFailed + ": " + data.ErrorMessage);

                if (!reader.HasRows)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;            
        }

        /// <summary>
        /// Creates an instance of the DataAccess Data provider
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override DataAccessBase GetDb(string connectionString = null)
        {
            if (connectionString == null)
                connectionString = Configuration.ConnectionString;

            DbProviderFactory provider = null;
            try
            {
                provider = DataUtils.GetDbProviderFactory(DataAccessProviderTypes.SqLite);
            }
            catch
            {
                throw new InvalidOperationException("Unable to load SqLite Data Provider. Make sure you have a reference to Microsoft.Data.Sqlite (.NET Core) or System.Data.SQLite (.NET 4.5).");
            }

            var db = new SqlDataAccess(connectionString, provider);            
            db?.ExecuteNonQuery("PRAGMA journal_mode=WAL;");

            return db;
        }

        public override bool CreateLocalizationTable(string tableName = null)
        {
            if (tableName == null)
                tableName = Configuration.ResourceTableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = "Localizations";

            string sql = string.Format(TableCreationSql, tableName);

            // Check for table existing already
            if (IsLocalizationTable(tableName))
            {
                SetError(Resources.LocalizationTable_Localization_Table_exists_already);
                return false;
            }

            SetError();

            using (var data = GetDb())
            {

                if (!data.RunSqlScript(sql, false, false))
                {       
                    SetError(data.ErrorMessage);
                    return false;                    
                }
            }

            return true;
        }
        
        protected override string TableCreationSql
        {
            get
            {
                return
                    @"CREATE TABLE [{0}] (
 [Pk] INTEGER PRIMARY KEY 
, [ResourceId] nvarchar(1024) COLLATE NOCASE NOT NULL
, [Value] ntext  NULL
, [LocaleId] nvarchar(10) COLLATE NOCASE DEFAULT '' NULL
, [ResourceSet] nvarchar(512) COLLATE NOCASE DEFAULT ''  NULL
, [Type] nvarchar(512) DEFAULT '' NULL
, [BinFile] image NULL
, [TextFile] ntext NULL
, [Filename] nvarchar(128) NULL
, [Comment] nvarchar(512) NULL
, [ValueType] unsigned integer(2) DEFAULT 0
, [Updated] datetime  DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hello Cruel World (SqlLite) 1','','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hallo schnöde Welt (SqlLite)','de','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Bonjour tout le monde (SqlLite)','fr','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Yesterday (invariant)','','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Gestern','de','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Hier','fr','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Today (invariant)','','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Heute','de','Resources');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Aujourd''hui','fr','Resources');

INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','This is **MarkDown** formatted *HTML Text*','','Resources',2);
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Hier ist **MarkDown** formatierter *HTML Text*','de','Resources',2);
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Ceci est **MarkDown** formaté *HTML Texte*','fr','Resources',2);

INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hello Cruel World (local)','','ResourceTest.aspx');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hallo Welt (lokal)','de','ResourceTest.aspx');
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Bonjour monde (local)','fr','ResourceTest.aspx');
";
            }

        }
    }
}
