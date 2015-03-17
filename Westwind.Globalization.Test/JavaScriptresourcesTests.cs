using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Westwind.Globalization.Test
{
    [TestClass]
    public class JavaScriptresourcesTests
    {
        [TestMethod]
        public void GenerateResources()
        {
            var js = new JavaScriptResources(".\\");
            bool result = js.ExportJavaScriptResources(".\\JavascriptResources\\","global.resources");
            Assert.IsTrue(result);
            Console.WriteLine(File.ReadAllText(".\\JavascriptResources\\" + "LocalizationForm.de.js"));
        }
    }
}
