
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Westwind.Utilities;
using Westwind.Utilities.Data;

namespace Westwind.Globalization
{
    /// <summary>
    /// PostgreSql implementation of the Db SQL data resource provider.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: In order to use this provider make sure you add
    /// the Npgsql NuGet Package to your project.
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
                    @"select ResourceId,CAST( MAX( 
	              case  
		            WHEN LENGTH( CAST(Value as varchar(10))) > 0 THEN 1
		            ELSE 0
	              end ) as Bit) as HasValue
	  	            from {0}
                    where ResourceSet=@ResourceSet 
	                group by ResourceId", Configuration.ResourceTableName);

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

            return base.IsLocalizationTable(tableName.ToLower());
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
                          "Unable to load PostgreSql Data Provider. Make sure you have a reference to Npgsql.");
            }

            var db = new SqlDataAccess(connectionString, provider);
            return db;
        }




        protected override string TableCreationSql
        {
            get
            {
                return
                    @"CREATE TABLE {0} (
  pk SERIAL NOT NULL,
  ResourceId varchar(1024) DEFAULT NULL,
  Value varchar(2048) DEFAULT NULL,
  LocaleId varchar(10) DEFAULT NULL,
  ResourceSet varchar(512) DEFAULT NULL,
  Type varchar(512) DEFAULT NULL,
  BinFile BYTEA,
  TextFile text,
  Filename varchar(128) DEFAULT NULL,
  Comment varchar(512) DEFAULT NULL,
  ValueType int DEFAULT 0,
  Updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (pk)
);

INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hello Cruel World (PostgreSql)','','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hallo schnצde Welt (PostgreSql)','de','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Bonjour tout le monde','fr','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Yesterday (invariant)','','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Gestern','de','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Hier','fr','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Today (invariant)','','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Heute','de','Resources');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Aujourd''hui','fr','Resources');

INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','This is **MarkDown** formatted *HTML Text*','','Resources',2);
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Hier ist **MarkDown** formatierter *HTML Text*','de','Resources',2);
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Ceci est **MarkDown** formatי *HTML Texte*','fr','Resources',2);

INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hello Cruel World (local)','','ResourceTest.aspx');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hallo Welt (lokal)','de','ResourceTest.aspx');
INSERT INTO {0} (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Bonjour monde (local)','fr','ResourceTest.aspx');
";
            }

        }
    }
}
