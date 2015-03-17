using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;
using System.Collections;
using System.CodeDom.Compiler;
using System.Resources.Tools;
using System.CodeDom;
using Westwind.Utilities;


namespace Westwind.Globalization
{
    /// <summary>
    /// Class that handles generating strongly typed resources 
    /// for global Web resource files. This feature is not supported
    /// in ASP.NET stock projects and doesn't support custom resource
    /// providers in WAP.
    /// </summary>
    public class JavaScriptResources
    {                
        public JavaScriptResources(string outputPath)
        {
            this.OutputPath = outputPath;
        }

        /// <summary>
        /// The physical path for the Web application
        /// </summary>
        public string OutputPath
        {
            get { return _outputPath; }
            set { _outputPath = value; }
        }
        private string _outputPath = "";


        /// <summary>
        /// An error message set on a failure result
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
        private string _ErrorMessage = "";


        public bool ExportJavaScriptResources(string path, string baseVarname = "resources")
        {            
            var man = DbResourceDataManager.CreateDbResourceDataManager();
            var resourceSets = man.GetAllResourceSets(ResourceListingTypes.GlobalResourcesOnly);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var resSet in resourceSets)
            {
                var locales = man.GetAllLocaleIds(resSet);
                foreach (var locale in locales)
                {
                    var resourceSet = man.GetResourceSetNormalizedForLocaleId(locale, resSet);
                    string js = SerializeResourceDictionary(resourceSet, baseVarname + "." + resSet,locale);

                    var filePath = Path.Combine(path, SafeVarName(resSet)) +
                        (string.IsNullOrEmpty(locale) ? "" : "." + locale)  +  
                        ".js";
                    File.WriteAllText(filePath, js);
                }
            }

            return true;
        }


        /// <summary>
        /// Generates the actual JavaScript object map string makes up the
        /// handler's result content.
        /// </summary>
        /// <param name="resxDict"></param>
        /// <param name="resourceSetName"></param>
        /// <returns></returns>
        private string SerializeResourceDictionary(Dictionary<string, object> resxDict, string resourceSetName, string localeId)
        {
            StringBuilder sb = new StringBuilder(2048);

            sb.Append(resourceSetName + " = {\r\n");
            sb.AppendLine("\t\"__localeId\": \"" + localeId + "\";");

            int anonymousIdCounter = 0;
            foreach (KeyValuePair<string, object> item in resxDict)
            {
                string value = item.Value as string;
                if (value == null)
                    continue; // only encode string values

                string key = item.Key;
                if (string.IsNullOrEmpty(item.Key))
                    key = "__id" + anonymousIdCounter++.ToString();

                key = key.Replace(".", "_");
                if (key.Contains(" "))
                    key = StringUtils.ToCamelCase(key);

                sb.Append("\t\"" + key + "\": ");
                sb.Append(WebUtils.EncodeJsString(value));
                sb.Append(",\r\n");
            }

            // add dbRes function
            sb.AppendFormat(
"\t" + @"""dbRes"": function dbRes(resId) {{ return {0}[resId] || resId; }}      
}}
", resourceSetName);


            return sb.ToString();
        }

        


        public static string SafeVarName(string phrase)
        {
            if (phrase == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder(phrase.Length);

            // First letter is always upper case
            bool nextUpper = false;
            bool isFirst = true;

            foreach (char ch in phrase)
            {
                if (isFirst && char.IsDigit(ch))
                    sb.Append("_"); // prefix
                isFirst = false;

                // skip
                if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || char.IsSeparator(ch) || char.IsControl(ch) || char.IsSymbol(ch) )
                {
                    nextUpper = true;
                    continue;
                }

                sb.Append(nextUpper ? char.ToUpper(ch) : ch);

                nextUpper = false;
            }

            return sb.ToString();
        }
    }

}
