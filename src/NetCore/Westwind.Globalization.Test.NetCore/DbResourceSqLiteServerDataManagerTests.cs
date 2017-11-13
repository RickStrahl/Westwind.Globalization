using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Westwind.Utilities;
using Westwind.Utilities.Data;


namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class DbResourceSqLiteDataManagerTests 
    {
        string DataPath
        {
            get => FileUtils.NormalizePath(Path.Combine(TestContext.CurrentContext.TestDirectory,
                "data/SqLiteLocalizations.db"));
        }

        private IDbResourceDataManager GetManager()
        {
            var manager = new DbResourceSqLiteDataManager();
           
            manager.Configuration.ConnectionString = "Data Source=" + DataPath;

            //manager.Configuration.ResourceTableName = "Localizations";
            return manager;
        }
        
        public DbResourceSqLiteDataManagerTests()
        {
            //if (File.Exists(DataPath))
            //    File.Delete(DataPath);

           //CreateTable();
        }

        [Test]
        public void CreateTable()
        {
            if (File.Exists(DataPath))
                File.Delete(DataPath);

            var manager = GetManager();
            
            bool result = manager.CreateLocalizationTable();

            // no assertions as table can exist - to test explicitly remove the table
            if (result)
                Console.WriteLine("Table created.");
            else
                Console.WriteLine(manager.ErrorMessage);
        }

        // Demonstrate Microsoft.Data.SqLite returns dates as strings
        //[Test]
        //public void ReadData()
        //{
        //    var db = new SqlDataAccess("Data Source=" + DataPath, DataAccessProviderTypes.SqLite);
        //    using (var reader = db.ExecuteReader("select * from Localizations"))
        //    {
        //        while (reader.Read())
        //        {
        //            var updated = reader["Updated"];
        //            Console.WriteLine(updated);
        //            Assert.IsTrue(updated != null, "Updated shouldn't be null");
        //            Assert.IsTrue(updated.GetType() == typeof(DateTime),"Invalid updated type: " + updated.GetType());
        //        }
        //    }
        //}
        

        [Test]
        public void IsLocalizationTable()
        {
            var manager = GetManager();            
            Assert.IsTrue(manager.IsLocalizationTable("Localizations"), manager.ErrorMessage);
        }

        

        [Test]
        public void GetAllResources()
        {
            var manager = GetManager();

            var items = manager.GetAllResources(false);
            Assert.IsNotNull(items,manager.ErrorMessage);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);    
        }

        [Test]
        public void GetResourceSet()
        {
            var manager = GetManager();

            var items = manager.GetResourceSet("de","Resources");
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);
        }

        [Test]
        public void GetResourceSetNormalizedForLocaleId()
        {
            var manager = GetManager();

            var items = manager.GetResourceSetNormalizedForLocaleId("de", "Resources");
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);
        }

        [Test]
        public void GetAllResourceIds()
        {
            var manager = GetManager();

            var items = manager.GetAllResourceIds("Resources");
            Assert.IsNotNull(items,manager.ErrorMessage);
            Assert.IsTrue(items.Count > 0);
        }


        [Test]
        public void GetAllResourceIdsForHtmlDisplay()
        {
            var manager = GetManager();
            var items = manager.GetAllResourceIdListItems("Resources");

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            foreach (var item in items)
            {
                Console.WriteLine(item.ResourceId + ": " + item.Text + " " + (item.Selected ? "* " : "") );
            }
        }
        
        [Test]
        public void GetAllResourceSets()
        {
            var manager = GetManager();

            var items = manager.GetAllResourceSets(ResourceListingTypes.AllResources);
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            foreach (var item in items)
            {
                Console.WriteLine(item);
            }

            items = manager.GetAllResourceSets(ResourceListingTypes.LocalResourcesOnly);
            Assert.IsNotNull(items);            

            Console.WriteLine("--- Local ---");
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }

            items = manager.GetAllResourceSets(ResourceListingTypes.GlobalResourcesOnly);
            Assert.IsNotNull(items);            

            Console.WriteLine("--- Global ---");
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        [Test]
        public void GetAllLocaleIds()
        {
            var manager = GetManager();

            
            var items = manager.GetAllLocaleIds("Resources");
            Assert.IsNotNull(items, manager.ErrorMessage);
            Assert.IsTrue(items.Count > 0);

            foreach (var localeId in items)
            {
                Console.WriteLine(":" + localeId);
            }
            
        }

        [Test]
        public void GetAllResourcesForCulture()
        {
            var manager = GetManager();

            var items = manager.GetAllResourcesForCulture("Resources","de");
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            foreach (var localeId in items)
            {
                Console.WriteLine(":" + localeId);
            }
        }

        [Test]
        public void GetResourceString()
        {
            var manager = GetManager();

            var item = manager.GetResourceString("Today", "Resources", "de");

            Assert.IsNotNull(item);
            Assert.IsTrue(item == "Heute");
            Console.WriteLine(item);
        }

        [Test]
        public void GetResourceItem()
        {
            var manager = GetManager();

            var item = manager.GetResourceItem("Today", "Resources", "de");

            Assert.IsNotNull(item);
            Assert.IsTrue(item.Value.ToString() == "Heute");
        }

        [Test]
        public void GetResourceObject()
        {
            var manager = GetManager();

            // this method allows retrieving non-string values as their
            // underlying type - demo data doesn't include any binary data.
            var item = manager.GetResourceObject("Today", "Resources", "de");

            Assert.IsNotNull(item);
            Assert.IsTrue(item.ToString() == "Heute");
        }

        [Test]
        public void GetResourceStrings()
        {
            var manager = GetManager();

            var items = manager.GetResourceStrings("Today", "Resources");

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);

        }


        [Test]
        public void UpdateResourceString()
        {
            var manager = GetManager();

            string updated = "Heute Updated";
            int count =  manager.UpdateOrAddResource("Today",updated,"de","Resources",null,false);

            Assert.IsFalse(count == -1, manager.ErrorMessage);
            string check = manager.GetResourceString("Today", "Resources", "de");

            Assert.AreEqual(check, updated);
            Console.WriteLine(check);

            manager.UpdateOrAddResource("Today", "Heute", "de", "Resources",null,false);
        }

        [Test]
        public void UpdateInvalidResourceString()
        {
            var manager = GetManager();

            string resourceId = "NewlyAddedTestThatDoesntExist";
            string text = "Newly Added Test";

            int count = manager.UpdateResource(resourceId, text, "de", "Resources");

            Assert.IsTrue(string.IsNullOrEmpty(manager.ErrorMessage));
            Assert.IsTrue(count == 0,"Shouldn't update a non-existing record");
            Console.WriteLine(count);
        }



        [Test]
        public void AddAndDeleteResourceString()
        {
            var manager = GetManager();

            string resourceId = "NewlyAddedTest";
            string text = "Newly Added Test";

            int count = manager.AddResource(resourceId, text, "de", "Resources");

            Assert.IsFalse(count == -1, manager.ErrorMessage);
            string check = manager.GetResourceString(resourceId, "Resources", "de");

            Assert.AreEqual(check, text);
            Console.WriteLine(check);

            bool result = manager.DeleteResource(resourceId,resourceSet: "Resources", cultureName: "de");
            Assert.IsTrue(result, manager.ErrorMessage);

            check = manager.GetResourceString(resourceId, "Resources", "de");
            Assert.IsNull(check, manager.ErrorMessage);
        }




        private void ShowResources(IDictionary items)
        {
            foreach (DictionaryEntry resource in items)
            {
                Console.WriteLine(resource.Key + ": " + resource.Value);
            }
        }
        private void ShowResources(IEnumerable<ResourceItem> items)
        {
            foreach (var resource in items)
            {
                Console.WriteLine(resource.ResourceId + " - " + resource.LocaleId + ": " + resource.Value);
            }
        }
    }
}
