using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Westwind.Globalization.Test
{
    /// <summary>
    /// Summary description for DbResXConverterTests
    /// </summary>
    [TestFixture]
    public class DbResXConverterTests
    {
   

        [Test]
        public void GetResXResourcesTest()
        {
            DbResXConverter converter = new DbResXConverter(null);            
            Dictionary<string,object> items = converter.GetResXResourcesNormalizedForLocale(@"C:\projects2008\Westwind.GlobalizationWeb\App_GlobalResources\resources","de-de");
            this.WriteResourceDictionary(items,"ResX Resources");
        }

        [Test]
        public void GetDbResourcesTest()
        {
            DbResourceDataManager manager = new DbResourceDataManager();
            Dictionary<string,object> items = manager.GetResourceSetNormalizedForLocaleId("de-de", "resources");

            WriteResourceDictionary(items, "DB Resources");            
        }


        private void WriteResourceDictionary(Dictionary<string,object> items, string title)
        {
            Console.WriteLine("*** " + title);
            foreach (var item in items)
            {
                Console.WriteLine(item.Key + " " + item.Value.ToString());
            }

            Dictionary<string,string> its = new Dictionary<string, string> { { "rick","strahl" }, { "frank", "hovell"} };

            Dictionary<string, string> sss = its.Where(dd => dd.Key.Contains('.')).ToDictionary( dd=> dd.Key, dd=> dd.Value);

        }
    }
}
