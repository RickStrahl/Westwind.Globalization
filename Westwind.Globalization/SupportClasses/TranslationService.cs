using System;
using System.Text;
using System.Web;
using Westwind.Utilities;
using Westwind.Web.JsonSerializers;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace Westwind.Globalization
{
    /// <summary>
    /// Provides basic translation features via several Web interfaces
    /// 
    /// NOTE: These services may change their format or otherwise fail.
    /// </summary>
    public class TranslationServices
    {
        /// <summary>
        /// Error message set when an error occurs in the translation service
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
        private string _ErrorMessage = "";

        /// <summary>
        /// Timeout for how long to wait for a translation
        /// </summary>
        public int TimeoutSeconds
        {
            get { return _TimeoutSeconds; }
            set { _TimeoutSeconds = value; }
        }
        private int _TimeoutSeconds = 10;


        /// <summary>
        /// Translates a string into another language using Google's translate API JSON calls.
        /// <seealso>Class TranslationServices</seealso>
        /// </summary>
        /// <param name="Text">Text to translate. Should be a single word or sentence.</param>
        /// <param name="FromCulture">
        /// Two letter culture (en of en-us, fr of fr-ca, de of de-ch)
        /// </param>
        /// <param name="ToCulture">
        /// Two letter culture (as for FromCulture)
        /// </param>
        public string TranslateGoogle(string text, string fromCulture, string toCulture)
        {
            fromCulture = fromCulture.ToLower();
            toCulture = toCulture.ToLower();

            // normalize the culture in case something like en-us was passed 
            // retrieve only en since Google doesn't support sub-locales
            string[] tokens = fromCulture.Split('-');
            if (tokens.Length > 1)
                fromCulture = tokens[0];
            
            // normalize ToCulture
            tokens = toCulture.Split('-');
            if (tokens.Length > 1)
                toCulture = tokens[0];
            
            string url = string.Format(@"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}",                                     
                                       HttpUtility.UrlEncode(text),fromCulture,toCulture);

            // Retrieve Translation with HTTP GET call
            string html = null;
            try
            {
                WebClient web = new WebClient();

                // MUST add a known browser user agent or else response encoding doen't return UTF-8 (WTF Google?)
                web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

                // Make sure we have response encoding to UTF-8
                web.Encoding = Encoding.UTF8;
                html = web.DownloadString(url);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = Westwind.Globalization.Resources.Resources.ConnectionFailed + ": " +
                                    ex.GetBaseException().Message;
                return null;
            }

            // Extract out trans":"...[Extracted]...","from the JSON string
            string result = Regex.Match(html, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value;            

            if (string.IsNullOrEmpty(result))
            {
                this.ErrorMessage = Westwind.Globalization.Resources.Resources.InvalidSearchResult;
                return null;
            }

            // result string must be JSON decoded
            return WebUtils.DecodeJsString(result);

            // Result is a JavaScript string so we need to deserialize it properly
            //JavaScriptSerializer ser = new JavaScriptSerializer();
            //return ser.Deserialize(result, typeof(string)) as string;            
        }


        /// <summary>
        /// Uses the Bing API service to perform translation
        /// Bing can translate up to 1000 characters. 
        /// 
        /// Requires that you provide a CLientId and ClientSecret
        /// or set the configuration values for these two.
        /// 
        /// More info on setup:
        /// http://weblog.west-wind.com/posts/2013/Jun/06/Setting-up-and-using-Bing-Translate-API-Service-for-Machine-Translation
        /// </summary>
        /// <param name="text">Text to translate</param>
        /// <param name="fromCulture">Two letter culture name</param>
        /// <param name="toCulture">Two letter culture name</param>
        /// <param name="accessToken">Pass an access token retrieved with GetBingAuthToken.
        /// If not passed the default keys from .config file are used if any</param>
        /// <returns></returns>
        public string TranslateBing(string text, string fromCulture, string toCulture,
                                    string accessToken = null)
        {
            string serviceUrl = "http://api.microsofttranslator.com/V2/Http.svc/Translate";

            if (accessToken == null)
            {
                accessToken = GetBingAuthToken();
                if (accessToken == null)
                    return null;
            }
            
            string res;

            try
            {
                var web = new WebClient();                
                web.Headers.Add("Authorization", "Bearer " + accessToken);                        
                string ct = "text/plain";
                string postData = string.Format("?text={0}&from={1}&to={2}&contentType={3}",
                                         HttpUtility.UrlEncode(text),
                                         fromCulture, toCulture,
                                         HttpUtility.UrlEncode(ct));

                web.Encoding = Encoding.UTF8;
                res = web.DownloadString(serviceUrl + postData);
            }
            catch (Exception e)
            {
                ErrorMessage = e.GetBaseException().Message;
                return null;
            }

            // result is a single XML Element fragment
            var doc = new XmlDocument();
            doc.LoadXml(res);            
            return doc.DocumentElement.InnerText;          
        }

        /// <summary>
        /// Retrieves an oAuth authentication token to be used on the translate
        /// API request. The result string needs to be passed as a bearer token
        /// to the translate API.
        /// 
        /// You can find client ID and Secret (or register a new one) at:
        /// https://datamarket.azure.com/developer/applications/
        /// </summary>
        /// <param name="clientId">The client ID of your application</param>
        /// <param name="clientSecret">The client secret or password</param>
        /// <returns></returns>
        public string GetBingAuthToken(string clientId = null, string clientSecret = null)
        {
            string authBaseUrl = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

            if (string.IsNullOrEmpty(clientId))
                clientId = DbResourceConfiguration.Current.BingClientId;
            if (string.IsNullOrEmpty(clientSecret))
                clientSecret = DbResourceConfiguration.Current.BingClientSecret;

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                ErrorMessage = Resources.Resources.Client_Id_and_Client_Secret_must_be_provided;
                return null;
            }
            
            var postData = string.Format("grant_type=client_credentials&client_id={0}" +
                                         "&client_secret={1}" +
                                         "&scope=http://api.microsofttranslator.com",
                                         HttpUtility.UrlEncode(clientId),
                                         HttpUtility.UrlEncode(clientSecret));

            // POST Auth data to the oauth API
            string res, token;

            try
            {
                var web = new WebClient();
                web.Encoding = Encoding.UTF8;
                res = web.UploadString(authBaseUrl, postData);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.GetBaseException().Message;
                return null;
            }

            var ser = new JSONSerializer();
            var auth = ser.Deserialize<BingAuth>(res);
            if (auth == null)
                return null;
            
            token = auth.access_token;

            return token;
        }


        private class BingAuth
        {
            public string token_type { get; set; }
            public string access_token { get; set; }
        }

    }
}
