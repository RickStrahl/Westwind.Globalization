using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Westwind.Globalization.AspnetCore;

namespace Westwind.Globalization.Test.AspnetCore
{
    [TestFixture]
    public class DbResHtmlLocalizerFactoryTests
    {
        [Test]
        public void StripsOffLocationInResourceName()
        {
            var factory = new DbResHtmlLocalizerFactory(new DbResourceConfiguration(), null);
            var localizer = (DbResHtmlLocalizer)factory.Create("Company.Product.Test.Localizer", "Company.Product");

            Assert.AreEqual("Test.Localizer", localizer.ResourceSet);
        }

        [Test]
        public void DoNotStripsOffResourceSetWhenLocationIsempty()
        {
            var factory = new DbResHtmlLocalizerFactory(new DbResourceConfiguration(), null);
            var localizer = (DbResHtmlLocalizer)factory.Create("Company.Product.Test.Localizer", "");

            Assert.AreEqual("Company.Product.Test.Localizer", localizer.ResourceSet);
        }
    }
}
