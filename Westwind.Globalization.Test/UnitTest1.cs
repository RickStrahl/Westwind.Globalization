using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Westwind.Globalization.Test
{
    [TestClass]
    public class GeneratedResourceHelperTests
    {
        private string PngBitMapPath = @".\data\germanflag.png";
        private string JpgBitMapPath = @".\data\germanflag.png";

        [TestMethod]
        public void BitmapToEmbeddedImageFromFioleTest()
        {
            var bitmap = new Bitmap(PngBitMapPath);
            string output = GeneratedResourceHelper.BitmapToEmbeddedHtmlImage(bitmap, ImageFormat.Png);
            Assert.IsNotNull(output);
        }


        [TestMethod]
        public void JpgBitmapToEmbeddedImageFromFileTest()
        {
            var data = File.ReadAllBytes(JpgBitMapPath);

            Bitmap bitmap;

            // THIS FAILS on some jpeg files
            using (var ms = new MemoryStream(data))
            {
                bitmap = new Bitmap(ms);
            }

            // THIS WORKS - but causes extra memory usage
            // var ms = new MemoryStream(data);
            // bitmap = new Bitmap(ms);
            
            string output = GeneratedResourceHelper.BitmapToEmbeddedHtmlImage(bitmap, ImageFormat.Jpeg);
            Assert.IsNotNull(output);
            Console.WriteLine(output);
        }

        [TestMethod]
        public void RawDataEmbeddedImageFromFileTest()
        {
            var data = File.ReadAllBytes(JpgBitMapPath);
            string output = GeneratedResourceHelper.BitmapToEmbeddedHtmlImage(data, ImageFormat.Jpeg);
            Assert.IsNotNull(output);
            Console.WriteLine(output);
        }
   
   
    }
}
