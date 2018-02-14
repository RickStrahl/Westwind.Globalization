using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Westwind.Globalization.AspnetCore;

namespace Westwind.Globalization.Test.AspnetCore
{
    [TestFixture]
    public class DbResStringLocalizerFactoryTests
    {
        [Test]
        public void StripsOffLocationInResourceName()
        {
            var factory = new DbResStringLocalizerFactory(new DbResourceConfiguration(), null);
            var localizer = (DbResStringLocalizer)factory.Create("Company.Product.Test.Localizer", "Company.Product");

            Assert.AreEqual("Test.Localizer", localizer.ResourceSet);
        }

        [Test]
        public void DoNotStripsOffResourceSetWhenLocationIsempty()
        {
            var factory = new DbResStringLocalizerFactory(new DbResourceConfiguration(), null);
            var localizer = (DbResStringLocalizer)factory.Create("Company.Product.Test.Localizer", "");

            Assert.AreEqual("Company.Product.Test.Localizer", localizer.ResourceSet);
        }
    }
}
