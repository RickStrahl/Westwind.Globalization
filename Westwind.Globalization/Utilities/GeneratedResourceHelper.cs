using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;
using System.Text;
using System.Web;
using Westwind.Utilities;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Westwind.Globalization
{
    /// <summary>
    /// Class that returns resources 
    /// </summary>
    public static class GeneratedResourceHelper
    {

        /// <summary>
        /// Helper function called from strongly typed resources to retrieve 
        /// string based resource values.
        /// 
        /// This method returns a resource string based on the active 
        /// Generated ResourceAccessMode.
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <param name="resourceId"></param>
        /// <param name="manager"></param>
        /// <param name="resourceMode"></param>
        /// <returns></returns>
        public static string GetResourceString(string resourceSet, string resourceId,
                             ResourceManager manager,
                             ResourceAccessMode resourceMode)
        {
            if (resourceMode == ResourceAccessMode.AspNetResourceProvider)
                return GetAspNetResourceProviderValue(resourceSet, resourceId) as string;
            if (resourceMode == ResourceAccessMode.Resx)
                return manager.GetString(resourceId);

            return DbRes.T(resourceSet, "LocalizationForm");
        }

        /// <summary>
        /// Helper function called from strongly typed resources to retrieve 
        /// non-string based resource values.
        /// 
        /// This method returns a resource value based on the active 
        /// Generated ResourceAccessMode.
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <param name="resourceId"></param>
        /// <param name="manager"></param>
        /// <param name="resourceMode"></param>
        /// <returns></returns>
        public static object GetResourceObject(string resourceSet, string resourceId,
            ResourceManager manager,
            ResourceAccessMode resourceMode)
        {
            if (resourceMode == ResourceAccessMode.AspNetResourceProvider)
                return GetAspNetResourceProviderValue(resourceSet, resourceId);
            if (resourceMode == ResourceAccessMode.Resx)
                return manager.GetObject(resourceId);
            return DbRes.TObject(resourceSet, "LocalizationForm");
        }



        /// <summary>
        /// Helper method called to retrieve ASP.NET ResourceProvider based
        /// resources. Isolted into a separate method to avoid referencing
        /// HttpContext.
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        static object GetAspNetResourceProviderValue(string resourceSet, string resourceId)
        {
            return HttpContext.GetGlobalResourceObject(resourceSet, resourceId);
        }

        /// <summary>
        /// Renders an HTML IMG tag that contains the bitmaps embedded image content
        /// inline of the HTML document. Userful for resources.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="format"></param>
        /// <param name="extraAttributes"></param>
        /// <returns></returns>
        public static string BitmapToEmbeddedHtmlImage(Bitmap bitmap, ImageFormat format, string extraAttributes = null)
        {
            byte[] data;
            using (var ms = new MemoryStream(1024))
            {
                if (format == ImageFormat.Jpeg)
                {
                    EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, (long)85);
                    ImageCodecInfo jpegCodec = ImageUtils.Encoders["image/jpeg"];
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = qualityParam;
                    bitmap.Save((MemoryStream)ms, jpegCodec, encoderParams);
                }
                else
                    bitmap.Save(ms, format);

                data = ms.ToArray();
            }

            return BitmapToEmbeddedHtmlImage(data, format, extraAttributes);
        }

        /// <summary>
        /// Renders an HTML IMG tag that contains a raw byte stream's image content
        /// inline of the HTML document. Userful for resources.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        /// <param name="extraAttributes"></param>
        /// <returns></returns>
        public static string BitmapToEmbeddedHtmlImage(byte[] data, ImageFormat format, string extraAttributes = null)
        {
            string contentType = "image/jpeg";
            if (format == ImageFormat.Png)
                contentType = "image/png";
            else if (format == ImageFormat.Gif)
                contentType = "image/gif";
            else if (format == ImageFormat.Bmp)
                contentType = "image/bmp";


            StringBuilder sb = new StringBuilder();
            sb.Append("<img src=\"data:" + contentType + ";base64,");
            sb.Append(Convert.ToBase64String(data));
            sb.Append("\"");

            if (!string.IsNullOrEmpty(extraAttributes))
                sb.Append(" " + extraAttributes);

            sb.Append(" />");
            return sb.ToString();
        }
    }
}
