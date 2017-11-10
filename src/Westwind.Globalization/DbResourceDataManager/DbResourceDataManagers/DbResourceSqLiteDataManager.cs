using System.Collections.Generic;
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
                
                if (reader == null || !reader.HasRows)
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

            var provider = DataUtils.GetSqlProviderFactory(DataAccessProviderTypes.SqLite);
            if (provider == null)
                throw new System.ArgumentException("Unable to load SqLite Data Provider. Make sure you have a reference to Microsoft.Data.Sqlite (.NET Core) or System.Data.SQLite (.NET 4.5) and you've referenced a type out of this assembly during application startup.");

            return new SqlDataAccess(connectionString, provider);
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
, [Updated] datetime NULL
);

INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hello Cruel World (SqlLite)','','Resources');
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
