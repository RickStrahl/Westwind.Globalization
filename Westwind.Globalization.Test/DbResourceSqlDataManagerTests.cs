using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Westwind.Utilities.Data;


namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class DbResourceSqlDataManagerTests
    {
        [Test]
        public void GetAllResources()
        {
            var manager = new DbResourceSqlServerDataManager();

            var items = manager.GetAllResources(false);
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);    
        }

        [Test]
        public void GetResourceSet()
        {
            var manager = new DbResourceSqlServerDataManager();

            var items = manager.GetResourceSet("de","Resources");
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);
        }

        [Test]
        public void GetResourceSetNormalizedForLocaleId()
        {
            var manager = new DbResourceSqlServerDataManager();

            var items = manager.GetResourceSetNormalizedForLocaleId("de", "Resources");
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);
        }

        [Test]
        public void GetAllResourceIds()
        {
            var manager = new DbResourceSqlServerDataManager();

            var items = manager.GetAllResourceIds("Resources");
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);
        }


        [Test]
        public void GetAllResourceIdsForHtmlDisplay()
        {
            var manager = new DbResourceSqlServerDataManager();
            var items = manager.GetAllResourceIdsForHtmlDisplay("Resources");

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            foreach (var item in items)
            {
                Console.WriteLine(item.Value + ": " + item.Text + " " + (item.Selected ? "* " : "") + (item.Attributes.Count > 0) );
            }
        }
        
        [Test]
        public void GetAllResourceSets()
        {
            var manager = new DbResourceSqlServerDataManager();

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
            var manager = new DbResourceSqlServerDataManager();

            var items = manager.GetAllLocaleIds("Resources");
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            foreach (var localeId in items)
            {
                Console.WriteLine(":" + localeId);
            }
            
        }

        [Test]
        public void GetAllResourcesForCulture()
        {
            var manager = new DbResourceSqlServerDataManager();

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
            var manager = new DbResourceSqlServerDataManager();

            var item = manager.GetResourceString("Today", "Resources", "de");

            Assert.IsNotNull(item);
            Assert.IsTrue(item == "Heute");
        }

        [Test]
        public void GetResourceItem()
        {
            var manager = new DbResourceSqlServerDataManager();

            var item = manager.GetResourceItem("Today", "Resources", "de");

            Assert.IsNotNull(item);
            Assert.IsTrue(item.Value.ToString() == "Heute");
        }

        [Test]
        public void GetResourceObject()
        {
            var manager = new DbResourceSqlServerDataManager();

            // this method allows retrieving non-string values as their
            // underlying type - demo data doesn't include any binary data.
            var item = manager.GetResourceObject("Today", "Resources", "de");

            Assert.IsNotNull(item);
            Assert.IsTrue(item.ToString() == "Heute");
        }

        [Test]
        public void GetResourceStrings()
        {
            var manager = new DbResourceSqlServerDataManager();

            var items = manager.GetResourceStrings("Today", "Resources");

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count > 0);

            ShowResources(items);

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
