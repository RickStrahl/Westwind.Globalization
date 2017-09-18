
using System.Data.Common;
using System.IO;
using Westwind.Globalization.Properties;
using Westwind.Utilities;

namespace Westwind.Globalization
{
    /// <summary>
    /// Sql CE implementation of the Db SQL data provider
    /// </summary>
    public class DbResourceSqlServerCeDataManager : DbResourceDataManager
    {
        public override bool CreateLocalizationTable(string tableName = null)
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
                    // database doesn't exist - try to create the file
                    if (data.ErrorNumber == -2147467259)
                    {
                        var conn = data.Connection;
                        if (!File.Exists(conn.Database))
                        {                            
                            // use dynamic to avoid pulling in SqlCe ref into project reference
                            dynamic engine = ReflectionUtils.CreateInstanceFromString("System.Data.SqlServerCe.SqlCeEngine",data.ConnectionString);
                            engine.CreateDatabase();
                            engine.Dispose();

                            data.Connection.Open();
                            if (!data.RunSqlScript(Sql, false, false))
                            {
                                SetError(data.ErrorMessage);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        SetError(data.ErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        
        protected override string TableCreationSql
        {
            get
            {
                return
                    @"
CREATE TABLE [{0}] (
  [pk] int IDENTITY (1193,1) NOT NULL
, [ResourceId] nvarchar(1024) NOT NULL
, [Value] ntext DEFAULT ('') NULL
, [LocaleId] nvarchar(10) DEFAULT ('') NULL
, [ResourceSet] nvarchar(512) DEFAULT ('') NULL
, [Type] nvarchar(512) DEFAULT ('') NULL
, [BinFile] image NULL
, [TextFile] ntext NULL
, [Filename] nvarchar(128) DEFAULT ('') NULL
, [Comment] nvarchar(512) NULL
, [ValueType] int DEFAULT(0) NOT NULL
, [Updated] datetime DEFAULT(getDate()) NULL
);
GO
ALTER TABLE [{0}] ADD CONSTRAINT [PK_Localizations] PRIMARY KEY ([pk]);
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hello Cruel World (SqlCe)','','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Hallo schnöde Welt (SqlCe)','de','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('HelloWorld','Bonjour tout le monde','fr','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Yesterday (invariant)','','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Gestern','de','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Yesterday','Hier','fr','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Today (invariant)','','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Heute','de','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Aujourd''hui','fr','Resources')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('Today','Aujourd''hui','fr','Resources')
GO

INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','This is **MarkDown** formatted *HTML Text*','','Resources',2)
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Hier ist **MarkDown** formatierter *HTML Text*','de','Resources',2)
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Ceci est **MarkDown** formaté *HTML Texte*','fr','Resources',2)
GO

INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hello Cruel World (local)','','ResourceTest.aspx')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hallo Welt (lokal)','de','ResourceTest.aspx')
GO
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Bonjour monde (locale)','fr','ResourceTest.aspx')
GO
";
            }

        }
    }
}
