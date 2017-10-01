#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2009-2015
 *          http://www.west-wind.com/
 * 
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion


using System;
using System.IO;
using System.Resources;
using System.Text;
using System.Web;
using Westwind.Utilities;


#if NETFULL
using System.Drawing;
using System.Drawing.Imaging;
using Encoder = System.Drawing.Imaging.Encoder;
#endif

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
#if NETFULL
            if (resourceMode == ResourceAccessMode.AspNetResourceProvider)
                return GetAspNetResourceProviderValue(resourceSet, resourceId) as string;
#endif
            if (resourceMode == ResourceAccessMode.Resx)
                return manager.GetString(resourceId);

            return DbRes.T(resourceId, resourceSet);
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
#if NETFULL
            if (resourceMode == ResourceAccessMode.AspNetResourceProvider)
                return GetAspNetResourceProviderValue(resourceSet, resourceId);
#endif

            if (resourceMode == ResourceAccessMode.Resx)
                return manager.GetObject(resourceId);
            return DbRes.TObject(resourceId, resourceSet);
        }


#if NETFULL
        /// <summary>
        /// Helper method called to retrieve ASP.NET ResourceProvider based
        /// resources. Isolated into a separate method to avoid referencing
        /// HttpContext from the StronglyTyped resource file so non-Web
        /// projects don't pull in System.Web.
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

            string contentType = "image/jpeg";

            if (format == ImageFormat.Png)
                contentType = "image/png";
            else if (format == ImageFormat.Gif)
                contentType = "image/gif";
            else if (format == ImageFormat.Bmp)
                contentType = "image/bmp";
            else if (format == ImageFormat.Icon)
                contentType = "image/vnd.microsoft.icon";

            return BitmapToEmbeddedHtmlImage(data, contentType, extraAttributes);
        }        
#endif
        
        /// <summary>
        /// Renders an HTML IMG tag that contains a raw byte stream's image content
        /// inline of the HTML document. Userful for resources.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="s"></param>
        /// <param name="extraAttributes"></param>
        /// <returns></returns>
        public static string BitmapToEmbeddedHtmlImage(byte[] data, string contentType = null, string extraAttributes = null)
        {
            if(string.IsNullOrEmpty(contentType) || !contentType.StartsWith("image/"))
                contentType =  "image/jpeg";

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
