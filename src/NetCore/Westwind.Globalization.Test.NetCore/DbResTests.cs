using System;
using System.Data.Common;
using System.Threading;
using NUnit.Framework;
using Westwind.Utilities.Data;

namespace Westwind.Globalization.Test
{
    [TestFixture]
    public partial class DbResourceDataManagerWithSqlTests
    {

        private string STR_ConnectionString = DbResourceConfiguration.Current.ConnectionString;
        
        [Test]
        public void CheckDataBase()
        {
            SqlDataAccess db = new SqlDataAccess(STR_ConnectionString);
            var tb = db.ExecuteTable("localizations","select * from localizations");
            Assert.IsNotNull(tb, "Table is null. Invalid connection string most likely: " + STR_ConnectionString + "\r\n" + db.ErrorMessage);
            Console.WriteLine(tb.Rows.Count);
        }
        

        [Test]
        public void DbResSimpleValues()
        {            
            string val = DbRes.T("HelloWorld", "Resources", "en-US");
            Assert.AreNotEqual(val, "HelloWorld","Helloworld was not translated");
            Console.WriteLine(val);

            string val2 = DbRes.T("HelloWorld", "Resources");
            Assert.AreNotEqual(val, "HelloWorld", "Helloworld was not translated");

            Console.WriteLine(DbRes.T("HelloWorld", "Resources", "en-US"));
            Console.WriteLine(DbRes.T("Today", "Resources", "en-US"));
            Console.WriteLine(DbRes.T("Yesterday", "Resources", "en-US"));
            Console.WriteLine(DbRes.T("Save", "Resources", "en-US"));

            Console.WriteLine(DbRes.T("HelloWorld", "Resources", "de-DE"));
            Assert.AreNotEqual(val, "HelloWorld", "Helloworld was not translated in German");
            Console.WriteLine(DbRes.T("Today", "Resources", "de-DE"));
            Console.WriteLine(DbRes.T("Yesterday", "Resources", "de-de"));
            Console.WriteLine(DbRes.T("Save", "Resources", "de-de"));
        }

        [Test]
        public void DbResFormatValues()
        {
            Console.WriteLine(DbRes.TFormat("#1 {0}","Today", "Resources"));
            Console.WriteLine(DbRes.TFormat("#2 {0}","Yesterday", "Resources"));
            Console.WriteLine(DbRes.TFormat("#3 {0}","Save", "Resources"));

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");

            Console.WriteLine(DbRes.TFormat("#1 {0}", "Today", "Resources"));
            Console.WriteLine(DbRes.TFormat("#2 {0}", "Yesterday", "Resources"));
            Console.WriteLine(DbRes.TFormat("#3 {0}", "Save", "Resources"));            
        }


        [Test]
        public void DbResHeavyLoad()
        {
            var dt = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                var t = new Thread(threadedDbRes);
                t.Start(dt);                
            }


            Thread.Sleep(2000);
        }


        void threadedDbRes(object dt)
        {                                    
            try
            {
                Console.WriteLine(DbRes.T("Today", "Resources", "de-de") + " - " + Thread.CurrentThread.ManagedThreadId + " - " + DateTime.Now.Ticks);
                Console.WriteLine(DbRes.T("Today", "Resources", "en-US") + " - " + Thread.CurrentThread.ManagedThreadId + " - " + DateTime.Now.Ticks);
            }
            catch (Exception ex)
            {
                Console.WriteLine("*** ERROR: "  + ex.Message);
            }
        }
    }
}
