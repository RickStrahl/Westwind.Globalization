
using System.Collections.Generic;
using System.Data.Common;
using Westwind.Utilities;
using Westwind.Utilities.Data;

namespace Westwind.Globalization
{
    /// <summary>
    /// MySql implementation of the Db SQL data resource provider.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: In order to use this provider make sure you add
    /// the MySql.Data NuGet Package to your project.
    /// </remarks>
    public class DbResourceMySqlDataManager : DbResourceDataManager
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
                    @"select ResourceId, if(MAX(length(Value)) > 0,true,false) as HasValue
	  	            from {0}
                    where ResourceSet=@ResourceSet 
		            group by 1", Configuration.ResourceTableName);

                // have to use a reader as bool values are coming back as longs that 
                // aren't automatically parsed into bool
                var reader = data.ExecuteReader(sql, data.CreateParameter("@ResourceSet", resourceSet));               
                if (reader == null)
                {
                    SetError(data.ErrorMessage);
                    return null;
                }

                var list = new List<ResourceIdItem>();
                while (reader.Read())
                {
                    bool val = ((long) reader["HasValue"]) == 1 ? true : false;

                    list.Add(new ResourceIdItem()
                    {
                        ResourceId = reader["ResourceId"] as string,
                        HasValue = val
                    });
                }
                
                return list;
            }
        }

        /// <summary>
        /// Create a backup of the localization database.
        /// 
        /// Note the table used is the one specified in the Configuration.ResourceTableName
        /// </summary>
        /// <param name="BackupTableName">Table of the backup table. Null creates a _Backup table.</param>
        /// <returns></returns>
        public override bool CreateBackupTable(string BackupTableName)
        {
            if (BackupTableName == null)
                BackupTableName = Configuration.ResourceTableName + "_Backup";

            using (var data = GetDb())
            {
                data.ExecuteNonQuery("drop table " + BackupTableName);
                CreateLocalizationTable(BackupTableName);
                data.ExecuteNonQuery("delete from " + BackupTableName);
                if (data.ExecuteNonQuery("insert into " + BackupTableName + " select * from " + Configuration.ResourceTableName) < 0)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks to see if the LocalizationTable exists
        /// </summary>
        /// <param name="tableName">Table name or the configuration.ResourceTableName if not passed</param>
        /// <returns></returns>
        public override bool IsLocalizationTable(string tableName = null)
        {
            if (tableName == null)
                tableName = Configuration.ResourceTableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = "Localizations";

            string sql = "SHOW TABLES LIKE @0";

            using (var data = GetDb())
            {
                var tables = data.ExecuteReader(sql, tableName);

                if (tables == null || !tables.HasRows)
                {
                    SetError(data.ErrorMessage);
                    return false;
                }
            }

            return true;
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
                SetError(Westwind.Globalization.Properties.Resources.LocalizationTable_Localization_Table_exists_already);
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
                provider = DataUtils.GetDbProviderFactory(DataAccessProviderTypes.MySql);
            }
            catch
            {
                   throw new System.InvalidOperationException(
                          "Unable to load MySQL Data Provider. Make sure you have a reference to MySql.Data.");
            }

            var db = new SqlDataAccess(connectionString, provider);
            return db;
        }




        protected override string TableCreationSql
        {
            get
            {
                return
                    @"CREATE TABLE `{0}` (
  pk int(11) NOT NULL AUTO_INCREMENT,
  ResourceId varchar(1024) DEFAULT NULL,
  Value varchar(2048) DEFAULT NULL,
  LocaleId varchar(10) DEFAULT NULL,
  ResourceSet varchar(512) DEFAULT NULL,
  Type varchar(512) DEFAULT NULL,
  BinFile blob,
  TextFile text,
  Filename varchar(128) DEFAULT NULL,
  Comment varchar(512) DEFAULT NULL,
  ValueType int(2) DEFAULT 0,
  Updated datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`pk`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hello Cruel World (MySql)','','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hallo schnöde Welt (MySql)','de','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Bonjour tout le monde','fr','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Yesterday (invariant)','','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Gestern','de','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Hier','fr','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Today (invariant)','','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Heute','de','Resources');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Aujourd''hui','fr','Resources');

INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','This is **MarkDown** formatted *HTML Text*','','Resources',2);
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Hier ist **MarkDown** formatierter *HTML Text*','de','Resources',2);
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Ceci est **MarkDown** formaté *HTML Texte*','fr','Resources',2);

INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hello Cruel World (local)','','ResourceTest.aspx');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hallo Welt (lokal)','de','ResourceTest.aspx');
INSERT INTO `{0}` (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Bonjour monde (local)','fr','ResourceTest.aspx');
";
            }

        }
    }
}