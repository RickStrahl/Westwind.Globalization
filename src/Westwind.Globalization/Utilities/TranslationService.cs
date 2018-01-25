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
using System.Text;
using System.Web;
using Westwind.Utilities;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Westwind.Globalization.Properties;

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
        /// <param name="googleApiKey">Google Api key - if not specified it's read from the configuration</param>
        public string TranslateGoogle(string text, string fromCulture, string toCulture, string googleApiKey = null)
        {
            fromCulture = fromCulture.ToLower();
            toCulture = toCulture.ToLower();

            if (!string.IsNullOrEmpty(googleApiKey))
            {
                googleApiKey = DbResourceConfiguration.Current.GoogleApiKey;
                if (!string.IsNullOrEmpty(googleApiKey))

                    return TranslateGoogleApi(text, fromCulture, toCulture, googleApiKey);
            }

            // normalize the culture in case something like en-us was passed 
            // retrieve only en since Google doesn't support sub-locales
            string[] tokens = fromCulture.Split('-');
            if (tokens.Length > 1)
                fromCulture = tokens[0];

            // normalize ToCulture
            tokens = toCulture.Split('-');
            if (tokens.Length > 1)
                toCulture = tokens[0];

            string format = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={1}&tl={2}&dt=t&q={0}";

            string url = string.Format(format,
                text, fromCulture, toCulture);

            // Retrieve Translation with HTTP GET call
            string jsonString;
            try
            {
                WebClient web = new WebClient();
                web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

                // Make sure we have response encoding to UTF-8
                web.Encoding = Encoding.UTF8;
                jsonString = web.DownloadString(url);
            }
            catch (Exception ex)
            {
                ErrorMessage = Resources.ConnectionFailed + ": " +
                               ex.GetBaseException().Message;
                return null;
            }


            // format:
            //[[["Hallo grausame Welt","Hello Cruel world",,,0]],,"en"]
            //[ [ ["Hallo grausame Welt","Hello Cruel world",,,0]],,"en"]
            dynamic json = JArray.Parse(jsonString);
            string result = json[0][0][0];

            if (string.IsNullOrEmpty(result))
            {
                ErrorMessage = Resources.InvalidSearchResult;
                return null;
            }

            result = WebUtility.HtmlDecode(result);
            return result;
        }



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
        /// <param name="googleApiKey">Google Api key - if not specified it's read from the configuration</param>
        public string TranslateGoogleApi(string text, string fromCulture, string toCulture, string googleApiKey = null)
        {

            if (string.IsNullOrEmpty(googleApiKey))
                googleApiKey = DbResourceConfiguration.Current.GoogleApiKey;

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

            string format = "https://www.googleapis.com/language/translate/v2?key={3}&source={1}&target={2}&q={0}";

            string url = string.Format(format,
                text, fromCulture, toCulture, googleApiKey);

            // Retrieve Translation with HTTP GET call
            string jsonString;
            try
            {
                WebClient web = new WebClient();
                web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

                // Make sure we have response encoding to UTF-8
                web.Encoding = Encoding.UTF8;
                jsonString = web.DownloadString(url);
            }
            catch (Exception ex)
            {
                ErrorMessage = Resources.ConnectionFailed + ": " +
                               ex.GetBaseException().Message;
                return null;
            }


            // format:
            //{
            //   "data": {
            //       "translations": [
            //       {
            //          "translatedText": "Wo bist du"
            //   }
            //  ]
            // }
            //}
            dynamic json = JValue.Parse(jsonString);
            string result = json.data.translations[0].translatedText;

            if (string.IsNullOrEmpty(result))
            {
                ErrorMessage = Resources.InvalidSearchResult;
                return null;
            }

            result = WebUtility.HtmlDecode(result);
            return result;
        }

        public string TranslateDeepL(string text, string fromCulture, string toCulture)
        {

            fromCulture = fromCulture.ToUpper();
            toCulture = toCulture.ToUpper();

            string url = "https://www.deepl.com/jsonrpc";
            string res;
            try
            {
                var json = @"{
    ""jsonrpc"": ""2.0"",
    ""method"": ""LMT_handle_jobs"",
    ""params"": {
        ""jobs"": [
            {
                ""kind"":""default"",
                ""raw_en_sentence"": ##jsonText##
            }
        ],
        ""lang"": {
            ""user_preferred_langs"": [
                ##fromLanguage##,
                ##toLanguage##
            ],
            ""source_lang_user_selected"": ##fromLanguage##,
            ""target_lang"": ##toLanguage##
        },
        ""priority"": -1,
        ""id"": 1
    }
}"
                    .Replace("##jsonText##", JsonConvert.SerializeObject(text))
                    .Replace("##fromLanguage##", JsonConvert.SerializeObject(fromCulture))
                    .Replace("##toLanguage##", JsonConvert.SerializeObject(toCulture));

                var web = new WebClient();
                var jsonResult = web.UploadString(url, json);

                dynamic jval = JValue.Parse(jsonResult);
                string translatedText = jval.result.translations[0].beams[0].postprocessed_sentence;
                return translatedText;
            }
            catch (Exception e)
            {
                ErrorMessage = e.GetBaseException().Message;
                return null;
            }            
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

            if (accessToken == null)
            {
                accessToken = GetBingAuthToken();
                if (accessToken == null)
                    return null;
            }

            string serviceUrl = "https://api.microsofttranslator.com/v2/Http.svc/Translate?" +
                                "&text=" + text +
                                "&from=" + fromCulture +
                                "&to=" + toCulture +
                                "&contentType=text/plain";
            string res;
            try
            {
                var web = new WebClient();
                web.Headers.Add("Authorization", "Bearer " + accessToken);
                res = web.DownloadString(serviceUrl);
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
        /// <param name="apiKey">The client ID of your application</param>        
        /// <returns></returns>
        public string GetBingAuthToken(string apiKey = null, string ignored = null)
        {

            string authBaseUrl = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            if (string.IsNullOrEmpty(apiKey))
                apiKey = DbResourceConfiguration.Current.BingClientId;

            if (string.IsNullOrEmpty(apiKey))
            {
                ErrorMessage = "Subscription key must be provided";
                return null;
            }
            // POST Auth data to the oauth API
            string res;
            try
            {
                var web = new HttpUtilsWebClient(new HttpRequestSettings()
                {
                    HttpVerb = "POST"
                });
                web.Encoding = Encoding.UTF8;
                web.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
                res = web.UploadString(authBaseUrl, "");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.GetBaseException().Message;
                return null;
            }
            return res;
        }

    }
}
