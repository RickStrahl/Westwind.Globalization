using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class DbResourceManagerTests
    {

        [Test]
        public void DbResourceManagerBasic()        
        {
            var res = new DbResourceManager("Resources");

            Console.WriteLine(JsonConvert.SerializeObject(res.Configuration, Formatting.Indented));

            string german = res.GetObject("Today", new CultureInfo("de-de")) as string;                       
            Assert.IsNotNull(german);
            Assert.AreEqual(german, "Heute");

            string english = res.GetObject("Today", new CultureInfo("en-us")) as string;
            Assert.IsNotNull(english);
            Assert.IsTrue(english.StartsWith("Today"));

            // should fallback to invariant/english
            string unknown = res.GetObject("Today", new CultureInfo("es-mx")) as string;
            Assert.IsNotNull(unknown);
            Assert.IsTrue(unknown.StartsWith("Today"));

            Console.WriteLine(german);
            Console.WriteLine(english);
            Console.WriteLine(unknown);
        }


        [Test]
        public void DbResourceManagerStronglyTypedResources()
        {
            // must force the resource manager into non-ASP.NET mode
            GeneratedResourceSettings.ResourceAccessMode = ResourceAccessMode.DbResourceManager;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

            string english = Resources.Today;
            Assert.IsNotNull(english);
            Console.WriteLine("English: " + english);
            Assert.IsTrue(english.StartsWith("Today"));
            

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-de");
            string german = Resources.Today;
            Console.WriteLine("German: " + german);
            Assert.IsNotNull(german);
            Assert.AreEqual("Heute",german);
            

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-mx");
            string unknown = Resources.Today;
            Assert.IsNotNull(unknown);
            Assert.IsTrue(unknown.StartsWith("Today"));

            Console.WriteLine(german);
            Console.WriteLine(english);
            Console.WriteLine(unknown);
        }

        static ResourceManager resManager;
        static bool start = false;

        [Test]
        public void DbResourceManagerHeavyLoad()
        {
            resManager = new DbResourceManager("Resources");

            var dt = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                var t = new Thread(threadedDbSimpleResourceProvider);
                t.Start(dt);
            }
            
            Thread.Sleep(150);
            start = true;
            Console.WriteLine("Started:  " + DateTime.Now.Ticks);

            // allow threads to run
            Thread.Sleep(3000);
        }


        void threadedDbSimpleResourceProvider(object dt)
        {
            while (!start)
            {
                Thread.Sleep(1);
            }

            try
            {                
                Console.WriteLine(resManager.GetObject("Today", new CultureInfo("de-de")) + " - " + Thread.CurrentThread.ManagedThreadId + " - " + DateTime.Now.Ticks);
                Console.WriteLine(resManager.GetObject("Today", new CultureInfo("en-us")) + " - " + Thread.CurrentThread.ManagedThreadId + " - " + DateTime.Now.Ticks);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("*** ERROR: " + ex.Message);
            }
        }
    }
}
