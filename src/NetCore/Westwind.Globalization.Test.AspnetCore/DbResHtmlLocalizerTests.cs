using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Westwind.Globalization.AspnetCore;

namespace Westwind.Globalization.Test.AspnetCore
{
    [TestFixture]
    public class DbResHtmlLocalizerTests
    {
        [Test]
        public void ResourceSetRemainsOnWithCultureCall()
        {
            var localizer = new DbResHtmlLocalizer(new DbResourceConfiguration()) { ResourceSet = "Test" };
            var newCultureLocalizer = (DbResHtmlLocalizer)localizer.WithCulture(new System.Globalization.CultureInfo("en-US"));

            Assert.AreEqual(localizer.ResourceSet, newCultureLocalizer.ResourceSet);
        }
    }
}
