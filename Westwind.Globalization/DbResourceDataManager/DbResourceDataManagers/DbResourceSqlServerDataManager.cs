using System.ComponentModel;

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
    public class DbResourceSqlServerDataManager : DbResourceDataManager
    {
        protected override string TableCreationSql
        {
            get
            {
                return
                    @"SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING OFF
GO
CREATE TABLE [{0}] (
		pk              int NOT NULL IDENTITY(1, 1),
		ResourceId      nvarchar(1024) NOT NULL,
		Value           nvarchar(max) NULL,
		LocaleId        nvarchar(10) NULL,
		ResourceSet     nvarchar(512) NULL,
		Type            nvarchar(512) NULL,
		BinFile         varbinary(max) NULL,
		TextFile        nvarchar(max) NULL,
		Filename        nvarchar(128) NULL,
        Comment         nvarchar(512) NULL,
        ValueType       int NOT NULL,
        Updated         datetime
)
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [PK_{0}]
	PRIMARY KEY
	([pk])
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
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [DF_{0}_ValueType]
	DEFAULT (0) FOR [ValueType]
GO
ALTER TABLE [{0}]
	ADD
	CONSTRAINT [DF_{0}_Updated]
	DEFAULT (getUtcDate()) FOR [Updated]
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
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','This is **MarkDown** formatted *HTML Text*','','Resources',2)
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Hier ist **MarkDown** formatierter *HTML Text*','de','Resources',2)
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet,ValueType) VALUES ('MarkdownText','Ceci est **MarkDown** formaté *HTML Texte*','fr','Resources',2)
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hello Cruel World (local)','','ResourceTest.aspx')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Hallo Welt (lokal)','de','ResourceTest.aspx')
INSERT INTO [{0}] (ResourceId,Value,LocaleId,ResourceSet) VALUES ('lblHelloWorldLabel.Text','Bonjour monde (local)','fr','ResourceTest.aspx')
GO
";
            }

        }
    }
}
