using NUnit.Framework;
using Westwind.Globalization.AspnetCore;

namespace Westwind.Globalization.Test.AspnetCore
{
    [TestFixture]
    public class DbResStringLocalizerTests
    {
        [Test]
        public void ResourceSetRemainsOnWithCultureCall()
        {
            var localizer = new DbResStringLocalizer(new DbResourceConfiguration()) { ResourceSet = "Test" };
            var newCultureLocalizer = (DbResStringLocalizer)localizer.WithCulture(new System.Globalization.CultureInfo("en-US"));

            Assert.AreEqual(localizer.ResourceSet, newCultureLocalizer.ResourceSet);
        }
    }
}
