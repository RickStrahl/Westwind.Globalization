
using System.IO;
using NUnit.Framework;

#if NETFULL
using System;
using System.Drawing;
using System.Drawing.Imaging;
#endif

namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class GeneratedResourceHelperTests
    {
        private string PngBitMapPath = @".\data\germanflag.png";
        private string JpgBitMapPath = @".\data\germanflag.png";

#if NETFULL
        [Test]
        public void BitmapToEmbeddedImageFromFioleTest()
        {
            var bitmap = new Bitmap(PngBitMapPath);
            string output = GeneratedResourceHelper.BitmapToEmbeddedHtmlImage(bitmap, ImageFormat.Png);
            Assert.IsNotNull(output);
        }


        [Test]
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

        [Test]
        public void RawDataEmbeddedImageFromFileTest()
        {
            var data = File.ReadAllBytes(JpgBitMapPath);
            string output = GeneratedResourceHelper.BitmapToEmbeddedHtmlImage(data,"image/jpeg");
            Assert.IsNotNull(output);
            Console.WriteLine(output);
        }
#endif
   
    }
}
