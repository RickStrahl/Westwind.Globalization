
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
    public class StronglyTypedResourceTests
    {

    
        public StronglyTypedResourceTests()
        {
            //DbResourceConfiguration.Current.ConnectionString = "SqLiteLocalizations";
            //DbResourceConfiguration.Current.DbResourceDataManagerType = typeof (DbResourceSqLiteDataManager);
        }

        [Test]
        public void GenerateStronglyTypedResourceClassFilteredTest()
        {
            var str = new StronglyTypedResources(@"c:\temp");
            var res = str.CreateClassFromAllDatabaseResources("ResourceExport", @"resources.cs",new string[] { "Resources" });

            Console.WriteLine(res);
        }


        [Test]
        public void GenerateStronglyTypedResourceResxDesignerFilteredTest()
        {
            var str = new StronglyTypedResources("c:\temp");
            var res = str.CreateResxDesignerClassesFromAllDatabaseResources("ResourceExport", @"c:\temp\resourceTest", new string[] { "Resources" });

            Console.WriteLine(res);
        }

                [Test]
        public void GenerateStronglyTypedResourceResxDesignerAllResourcesTest()
        {
            
            var str = new StronglyTypedResources("c:\temp\resourceTest");
            var res = str.CreateResxDesignerClassesFromAllDatabaseResources("ResourceExport", @"c:\temp\resourceTest");

            Console.WriteLine(res);
        }

        [Test]
        public void GenerateStronglyTypedDesignerClassFromResxFile()
        {
            string filename = @"c:\temp\resourceTest\LocalizationForm.resx";
            string designerFile = Path.ChangeExtension(filename, "designer.cs");
            if (File.Exists(designerFile))
                File.Delete(designerFile);

            var str = new StronglyTypedResources(@"c:\temp\resourceTest");
#if NETFULL
            str.CreateResxDesignerClassFromResxFile(filename,"LocalizationAdmin","Westwind.Globalization.Sample");
#else
            str.CreateResxDesignerClassFromResourceSet("LocalizationAdmin", "Westwind.Globalization.Sample", "LocalizationAdmin", filename);
#endif
            Assert.IsTrue(File.Exists(designerFile));

        }
    }
}

