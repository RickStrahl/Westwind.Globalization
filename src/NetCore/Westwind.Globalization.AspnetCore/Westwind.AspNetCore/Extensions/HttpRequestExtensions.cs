using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Westwind.Globalization.AspNetCore.Extensions
{
    public static class HttpRequestExtensions
    {
        static string WebRootPath { get; set; }

        #region Content Retrieval
        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream
        /// </summary>
        /// <param name="request">Request instance to apply to</param>
        /// <param name="encoding">Optional - Encoding, defaults to UTF8</param>
        /// <param name="inputStream">Optional - Pass in the stream to retrieve from. Other Request.Body</param>
        /// <returns></returns>
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null,
            Stream inputStream = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (inputStream == null)
                inputStream = request.Body;

            using (StreamReader reader = new StreamReader(inputStream, encoding))
                return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Retrieves the raw body as a byte array from the Request.Body stream
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request, Stream inputStream = null)
        {
            if (inputStream == null)
                inputStream = request.Body;

            using (var ms = new MemoryStream(2048))
            {
                await inputStream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        #endregion


        #region Path Functions

        /// <summary>
        /// Maps a virtual or relative path to a physical path in a Web site
        /// </summary>
        /// <param name="request"></param>
        /// <param name="relativePath"></param>
        /// <param name="host">Optional - IHostingEnvironment instance. If not passed retrieved from RequestServices DI</param>
        /// <param name="basePath">Optional - Optional physical base path. By default host.WebRootPath</param>
        /// <returns></returns>
        public static string MapPath(this HttpRequest request, string relativePath, IHostingEnvironment host = null,
            string basePath = null)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            if (basePath == null)
            {
                if (string.IsNullOrEmpty(WebRootPath))
                {
                    if (host == null)
                        host =
                            request.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)) as
                                IHostingEnvironment;
                    WebRootPath = host.WebRootPath;
                }

                if (string.IsNullOrEmpty(relativePath))
                    return WebRootPath;

                basePath = WebRootPath;
            }

            relativePath = relativePath.TrimStart('~').TrimStart('/', '\\');

            if (relativePath.StartsWith("~"))
                relativePath = relativePath.TrimStart('~');

            string path = Path.Combine(basePath, relativePath);

            string slash = Path.DirectorySeparatorChar.ToString();
            return path
                .Replace("/", slash)
                .Replace("\\", slash)
                .Replace(slash + slash, slash);
        }

        #endregion


        #region Intrinsic Object helpers

        /// <summary>
        /// Returns a single value as a string from:
        /// Form, Query, Session        
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Params(this HttpRequest request, string id)
        {
            string result = null;
            var method = request.Method.ToLower();
            if (request.Method == "post" || request.Method == "put")
                result = request.Form[id];

            if (result == null)
                result = request.Query[id];


            //if (result == null)            
            //    result = request.HttpContext.Session.GetString("id");

            return result;
        }

        #endregion


        #region Globalization

        /// <summary>
        /// Returns an array of user languages passed in the browser
        /// Works off the 'accept-language' header
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Array of languages or null if not available</returns>
        public static string[] GetUserLanguages(HttpContext context)
        {
            HttpRequest Request = context.Request;

            var langs = Request.Headers.GetCommaSeparatedValues("accept-language");

            // if no user lang leave existing but make writable
            if (langs == null || langs.Length == 0)
                return null;

            return langs;
        }


        /// <summary>
        /// Returns the first user language or null if not available.
        /// Works of 'accept-language' header
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserLanguage(HttpContext context)
        {
            HttpRequest Request = context.Request;

            var langs = Request.Headers.GetCommaSeparatedValues("accept-language");

            // if no user lang leave existing but make writable
            if (langs == null || langs.Length == 0)
                return null;

            return langs[0];
        }

        #endregion

    }
}