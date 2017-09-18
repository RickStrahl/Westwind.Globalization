using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
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

    
        public DbResXConverterTests()
        {
            //DbResourceConfiguration.Current.ConnectionString = "SqLiteLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof (DbResourceSqLiteDataManager);
        }

        /// <summary>
        ///  convert Resx file to a resource dictionary
        /// </summary>
        [Test]
        public void GetResXResourcesTest()
        {
            string path = @"c:\temp\resources";
            DbResXConverter converter = new DbResXConverter(path);
            Dictionary<string, object> items = converter.GetResXResourcesNormalizedForLocale(@"C:\Temp\Westwind.Globalizations\Westwind.Globalization.Sample\LocalizationAdmin\App_LocalResources\LocalizationAdmin.aspx", "de-de");
            WriteResourceDictionary(items,"ResX Resources");
        }

        [Test]
        public void GetDbResourcesTest()
        {
            // create manager based on configuration
            var manager = DbResourceDataManager.CreateDbResourceDataManager();

            Dictionary<string,object> items = manager.GetResourceSetNormalizedForLocaleId("de-de", "Resources");

            WriteResourceDictionary(items, "DB Resources");            
        }

        [Test]
        public void WriteResxFromDbResources()
        {
            DbResXConverter converter = new DbResXConverter(@"c:\temp\resources");
            Assert.IsTrue(converter.GenerateResXFiles(), converter.ErrorMessage);
        }

        [Test]
        public void ImportResxResources()
        {
            bool result = false;
            //var manager = Activator.CreateInstance(DbResourceConfiguration.Current.DbResourceDataManagerType) as IDbResourceDataManager;
            //result = manager.CreateLocalizationTable("Localizations");
            //Assert.IsTrue(result, manager.ErrorMessage);
            
            string physicalPath = Path.GetFullPath(@"..\..\..\Westwind.Globalization.Sample");
            DbResXConverter converter = new DbResXConverter(physicalPath);
            result = converter.ImportWebResources();

            Assert.IsTrue(result, converter.ErrorMessage);
        }


        private void WriteResourceDictionary(Dictionary<string,object> items, string title)
        {
            Console.WriteLine("*** " + title);
            foreach (var item in items)
            {
                Console.WriteLine(item.Key + ": " + item.Value.ToString());
            }

            Dictionary<string,string> its = new Dictionary<string, string> { { "rick","strahl" }, { "frank", "hovell"} };

            Dictionary<string, string> sss = its.Where(dd => dd.Key.Contains('.')).ToDictionary( dd=> dd.Key, dd=> dd.Value);

        }


    }
}
