using System;
using NUnit.Framework;
using Westwind.Utilities.Data;

namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class DbResTests
    {

        [Test]
        public void CreateDb()
        {
            SqlDataAccess db = new SqlDataAccess("MySqlLocalizations");
            string sql = @"
--Drop table Localizations;
CREATE TABLE Localizations ( `pk`  int,
        `ResourceId` varchar(1024),
        `Value`           varchar(max)
        `LocaleId`        varchar(10) ,
        `ResourceSet`     varchar(512),
        `Type`            varchar(512),
        `BinFile`         varbinary(max),
        `TextFile`        varchar(max),
        `Filename`        varchar(128),
        `Comment`         varchar(512) NULL )
";
           int result =    db.ExecuteNonQuery(sql);

            Assert.IsTrue(result > -1, db.ErrorMessage);

        }
        [Test]
        public void DataBase()
        {
            SqlDataAccess db = new SqlDataAccess("MySqlLocalizations");
            var tb = db.ExecuteTable("localizations","select * from localizations");
            Console.WriteLine(tb.Rows.Count);
        }

        [Test]
        public void DbResSimpleValues()
        {
            Console.WriteLine(DbRes.T("Today", "CommonPhrases", "de-de"));
            Console.WriteLine(DbRes.T("Yesterday", "CommonPhrases", "de-de"));
            Console.WriteLine(DbRes.T("Save", "CommonPhrases", "de-de"));

            Console.WriteLine(DbRes.T("Today","CommonPhrases","en-US"));
            Console.WriteLine(DbRes.T("Yesterday", "CommonPhrases","en-US"));
            Console.WriteLine(DbRes.T("Save", "CommonPhrases","en-US"));
        }
    }
}
