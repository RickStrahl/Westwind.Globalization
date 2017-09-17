using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Extensions
{
    public static class HttpRequestExtensions
    {
        static string WebRootPath { get; set; }

        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream
        /// </summary>
        /// <param name="request">Request instance to apply to</param>
        /// <param name="encoding">Optional - Encoding, defaults to UTF8</param>
        /// <param name="inputStream">Optional - Pass in the stream to retrieve from. Other Request.Body</param>
        /// <returns></returns>
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null, Stream inputStream = null)
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


        /// <summary>
        /// Maps a virtual or relative path to a physical path in a Web site
        /// </summary>
        /// <param name="request"></param>
        /// <param name="relativePath"></param>
        /// <param name="host">Optional - IHostingEnvironment instance. If not passed retrieved from RequestServices DI</param>
        /// <param name="basePath">Optional - Optional physical base path. By default host.WebRootPath</param>
        /// <returns></returns>
        public static string MapPath(this HttpRequest request, string relativePath, IHostingEnvironment host = null, string basePath = null)
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
    }
}
