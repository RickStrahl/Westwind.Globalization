using System;
using System.Threading;
using NUnit.Framework;
using Westwind.Utilities.Data;

namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class DbResourceDataManagerWithSqlTests
    {
        [Test]
        public void DataBase()
        {
            SqlDataAccess db = new SqlDataAccess("DevSamples");
            var tb = db.ExecuteTable("localizations","select * from localizations");
            Console.WriteLine(tb.Rows.Count);
        }

        [Test]
        public void DbResSimpleValues()
        {
            Console.WriteLine(DbRes.T("Today", "Resources", "de-de"));
            Console.WriteLine(DbRes.T("Yesterday", "Resources", "de-de"));
            Console.WriteLine(DbRes.T("Save", "Resources", "de-de"));

            Console.WriteLine(DbRes.T("Today", "Resources", "en-US"));
            Console.WriteLine(DbRes.T("Yesterday", "Resources", "en-US"));
            Console.WriteLine(DbRes.T("Save", "Resources", "en-US"));
        }

        [Test]
        public void DbResHeavyLoad()
        {
            var dt = DateTime.Now;
            for (int i = 0; i < 1500; i++)
            {
                var t = new Thread(threadedDbRes);
                t.Start(dt);                
            }


            Thread.Sleep(5000);
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
