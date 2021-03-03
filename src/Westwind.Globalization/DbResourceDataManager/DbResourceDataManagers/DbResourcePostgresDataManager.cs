
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
    public class DbResourcePostgreSqlDataManager : DbResourceDataManager
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
                 @"select ResourceId, (MAX(length(VALUE)) > 0) as HasValue
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
                tableName = "localizations";

            tableName = tableName.ToLower();

            string sql = @"SELECT EXISTS(
            SELECT * 
                FROM information_schema.tables 
            WHERE                 
                table_name = @0
            )";

            object tableExists = false;
            using (var data = GetDb())
            {
                tableExists = data.ExecuteScalar(sql,tableName);

                if (!string.IsNullOrEmpty(data.ErrorMessage))
                {
                    SetError(data.ErrorMessage);
                    return false;
                }

                if (!(tableExists is bool))
                {
                    SetError("Table retrieval failed.");
                    return false;
                }
            }

            return (bool) tableExists;
        }

        public override bool CreateLocalizationTable(string tableName = null)
        {
            if (tableName == null)
                tableName = Configuration.ResourceTableName;
            if (string.IsNullOrEmpty(tableName))
                tableName = "localizations";

            tableName = tableName.ToLower();

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
                provider = DataUtils.GetDbProviderFactory(DataAccessProviderTypes.PostgreSql);
            }
            catch
            {
                   throw new System.InvalidOperationException(
                          "Unable to load PostgreSql Data Provider. Make sure you have a reference to npgsql.");
            }

            var db = new SqlDataAccess(connectionString, provider);
            return db;
        }


        protected override string TableCreationSql
        {
            get
            {
                return
                    @"CREATE TABLE ""{0}"" (
		pk SERIAL NOT NULL,
		ResourceId VARCHAR(1024) NOT NULL,
		Value TEXT,
		LocaleId VARCHAR(10),
		ResourceSet VARCHAR(512), 
		Type VARCHAR(512),
		BinFile BYTEA,
		TextFile TEXT,
		Filename VARCHAR(128), 
		Comment VARCHAR(512),
	   ValueType INTEGER DEFAULT 0,
	   Updated TIMESTAMP DEFAULT NOW()
);
GO

INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hello Cruel World','','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hallo schnöde Welt','de','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Bonjour tout le monde','fr','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Yesterday (invariant)','','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Gestern','de','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Hier','fr','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Today (invariant)','','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Heute','de','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Aujourd''hui','fr','Resources');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','This is **MarkDown** formatted *HTML Text*','','Resources',2);
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Hier ist **MarkDown** formatierter *HTML Text*','de','Resources',2);
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Ceci est **MarkDown** formaté *HTML Texte*','fr','Resources',2);
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hello Cruel World (local)','','ResourceTest.aspx');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hallo Welt (lokal)','de','ResourceTest.aspx');
INSERT INTO ""{0}"" (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Bonjour monde (local)','fr','ResourceTest.aspx');
GO
";
            }

        }
    }
}
