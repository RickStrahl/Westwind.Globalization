using System;
using System.IO;
using NUnit.Framework;

namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class JavaScriptresourcesTests
    {
        [Test]
        public void GenerateResources()
        {
            var js = new JavaScriptResources(".\\");
            bool result = js.ExportJavaScriptResources(".\\JavascriptResources\\","global.resources");
            Assert.IsTrue(result);
            Console.WriteLine(File.ReadAllText(".\\JavascriptResources\\" + "LocalizationForm.de.js"));
        }
    }
}
