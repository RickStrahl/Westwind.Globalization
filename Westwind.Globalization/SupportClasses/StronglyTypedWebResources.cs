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


namespace Westwind.Globalization
{
    /// <summary>
    /// Class that handles generating strongly typed resources 
    /// for global Web resource files. This feature is not supported
    /// in ASP.NET stock projects and doesn't support custom resource
    /// providers in WAP.
    /// </summary>
    public class StronglyTypedWebResources
    {
                
        public StronglyTypedWebResources(string WebPhysicalPath)
        {
            this.WebPhysicalPath = WebPhysicalPath;
        }

        /// <summary>
        /// The physical path for the Web application
        /// </summary>
        public string WebPhysicalPath
        {
            get { return _WebPhysicalPath; }
            set { _WebPhysicalPath = value; }
        }
        private string _WebPhysicalPath = "";


        /// <summary>
        /// An error message set on a failure result
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
        private string _ErrorMessage = "";

        /// <summary>
        /// Internally track File type once we've read it from the
        /// top level method.
        /// </summary>
        private bool IsVb = false;


        


        /// <summary>
        /// Creates a class containing strongly typed resources of all resource keys
        /// in all global resource ResX files. A single class file with multiple classes
        /// is created.
        /// 
        /// The extension of the output file determines whether VB or CS is generated       
        /// </summary>        
        /// <param name="Namespace"></param>
        /// <param name="FileName">Output file name for the generated class. .cs and .vb generate appropriate languages</param>
        /// <returns>Generated class as a string</returns>
        public string CreateClassFromFromAllGlobalResXResources(string Namespace, string FileName)
        {
            if (!WebPhysicalPath.EndsWith("\\"))
                WebPhysicalPath += "\\";
            
            bool IsVb = IsFileVb(FileName);

            string ResPath = WebPhysicalPath + "app_globalresources\\";

            string[] Files = Directory.GetFiles(ResPath, "*.resx");

            StringBuilder sbClasses = new StringBuilder();

            foreach (string CurFile in Files)
            {                
                string[] tokens = Path.GetFileName(CurFile).Split('.');

                // If there's more than two parts is a culture specific file
                // We're only interested in the invariant culture
                if (tokens.Length > 2)
                    continue;

                // ResName: admin/default.aspx or default.aspx or resources (global or assembly resources)
                string ResName = Path.GetFileNameWithoutExtension(tokens[0]);
                ResName = ResName.Replace(".", "_");                

                string Class = CreateClassFromResXResource(CurFile, Namespace, ResName, null);
                sbClasses.Append(Class);
            }

            string Output = CreateNameSpaceWrapper(Namespace, IsVb, sbClasses.ToString());
            File.WriteAllText(FileName,Output);

            return Output;
        }

        /// <summary>
        /// Creates a strongly typed resource class that uses the ASP.NET Resource Manager
        /// rather than using Resx .NET resources which basically results in duplicated 
        /// resource sets loaded. Overwrites the generated.
        /// </summary>
        /// <param name="Namespace"></param>        
        /// <returns></returns>
        public void CreateGlobalResxResourceDesignerFiles(string Namespace, CodeGenerationLanguage lang)
        {            
            if (!WebPhysicalPath.EndsWith("\\"))
                WebPhysicalPath += "\\";
            
            string ResPath = WebPhysicalPath + "app_globalresources\\";

            string[] Files = Directory.GetFiles(ResPath, "*.resx");

            foreach (string CurFile in Files)
            {
                string file = CurFile;
                string[] tokens = Path.GetFileName(file).Split('.');

                // If there's more than two parts is a culture specific file
                // We're only interested in the invariant culture
                if (tokens.Length > 2)
                    continue;

                // ResName: admin/default.aspx or default.aspx or resources (global or assembly resources)
                string ResName = Path.GetFileNameWithoutExtension(tokens[0]);
                ResName = ResName.Replace(".", "_");

                CreateClassFromResXResource(file, Namespace, ResName,
                    ResPath + ResName + ".Designer." + 
                    (lang == CodeGenerationLanguage.Vb ? "vb" :"cs") );                
            }
        }

        /// <summary>
        /// Creates strongly typed classes from all global resources in the current application
        /// from the active DbResourceManager. One class is created which contains each of the
        /// resource classes. Classnames are not necessarily named with
        /// 
        /// Uses the default DbResourceConfiguration.Current settings for connecting
        /// to the database.
        /// </summary>
        /// <param name="Namespace">Optional namespace for the generated file</param>
        /// <param name="FileName">Output class file. .cs or .vb determines code language</param>
        /// <returns>Generated class as a string</returns>
        public string CreateClassFromAllDatabaseResources(string Namespace, string FileName)
        {
            bool IsVb = IsFileVb(FileName);

            DbResourceDataManager man = new DbResourceDataManager();
            DataTable Resources = man.GetAllResourceSets(ResourceListingTypes.GlobalResourcesOnly);

            StringBuilder sbClasses = new StringBuilder();
            foreach (DataRow row in Resources.Rows)
            {
                string ResourceSet = row["resourceset"] as string;
                string Class = CreateClassFromDatabaseResource(ResourceSet, null, ResourceSet, null);
                sbClasses.Append(Class);
            }

            string Output = CreateNameSpaceWrapper(Namespace, IsVb, sbClasses.ToString());
            File.WriteAllText(FileName, Output );

            return Output;
        }


        
        /// <summary>
        /// Creates an ASP.NET compatible strongly typed resource from a ResX file in ASP.NET.
        /// The class generated works only for Global Resources by calling GetGlobalResourceObject.
        /// 
        /// This routine parses the raw ResX files since you can't easily get access to the active
        /// ResourceManager in an ASP.NET application since the assembly is dynamically named and not
        /// easily accessible.
        /// </summary>
        /// <param name="ResourceSetFileName"></param>
        /// <param name="Namespace"></param>
        /// <param name="FileName">Output filename for the CSharp class. If null no file is generated and only the class is returned</param>
        /// <returns></returns>
        public string CreateClassFromResXResource(string ResXFile, string Namespace, string Classname, string FileName)
        {
            XmlDocument Dom = new XmlDocument();

            bool IsVb = IsFileVb(FileName);
            
            try
            {
                Dom.Load(ResXFile);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }

            string Indent = "\t\t";
            StringBuilder sbClass = new StringBuilder();

            CreateClassHeader(Classname, IsVb, sbClass);
            XmlNodeList nodes = Dom.DocumentElement.SelectNodes("data");

            foreach (XmlNode Node in nodes)
            {
                string value = Node.ChildNodes[0].InnerText;
                string resourceId = Node.Attributes["name"].Value;
                string varName = SafeVarName(resourceId);
                

                string typeName = null;
                if (Node.Attributes["type"] != null)
                    typeName = Node.Attributes["type"].Value;

                if (!string.IsNullOrEmpty(typeName))
                {
                    // File based resources are formatted: filename;full type name
                    string[] tokens = value.Split(';');
                    if (tokens.Length > 0)
                        // Grab the type and get the full name
                        typeName = Type.GetType(tokens[1]).FullName;
                }
                else
                    typeName = "System.String";

                // It's a string
                if (!IsVb)
                {
                    sbClass.Append(Indent + "public static " + typeName + " " + varName + "\r\n" + Indent + "{\r\n");
                    sbClass.AppendFormat(Indent + "\tget {{ return ({2}) HttpContext.GetGlobalResourceObject(\"{0}\",\"{1}\"); }}\r\n",
                                         Classname, resourceId,typeName);
                    sbClass.Append(Indent + "}\r\n\r\n");                    
                }
                else
                {
                    sbClass.Append(Indent + "Public Shared Property " + resourceId + "() as " + typeName + "\r\n");
                    sbClass.AppendFormat(Indent + "\tGet\r\n" + Indent + "\t\treturn CType( HttpContext.GetGlobalResourceObject(\"{0}\",\"{1}\"), {2})\r\n",
                                         Classname, resourceId,typeName);
                    sbClass.Append(Indent + "\tEnd Get\r\n");
                    sbClass.Append(Indent + "End Property\r\n\r\n");
                }
            }

            if (!IsVb)
                sbClass.Append("\t}\r\n\r\n");
            else
                sbClass.Append("End Class\r\n\r\n");
            

            if (!string.IsNullOrEmpty(FileName))
            {
                string FileContent = CreateNameSpaceWrapper(Namespace, IsVb, sbClass.ToString());
                File.WriteAllText(FileName, FileContent);
                return FileContent;
            }

            return sbClass.ToString();

        }

        /// <summary>
        /// Creates a strongly typed resource class for a ResoureSet from the DbResourceManager.
        /// 
        /// Note: Uses the default ResourceProvider settings as set in the DbResourceConfiguration.Current 
        /// property for opening the database and otherwise setting values.
        /// </summary>
        /// <param name="ResourceSetName">The name of the resource set. Should be a GLOBAL resource</param>
        /// <param name="Namespace">The namespace for the generated class. Null or string.Empty to not generate a namespace</param>
        /// <param name="Classname">Name of the class to be generated</param>
        /// <param name="FileName">Output filename for the CSharp class. If null no file is generated and only the class is returned</param>
        /// <returns>string of the generated class</returns>
        public string CreateClassFromDatabaseResource(string ResourceSetName, string Namespace, string Classname, string FileName)
        {
            // Use the custom ResourceManage to retrieve a ResourceSet
            DbResourceManager Man = new DbResourceManager(ResourceSetName);           
            ResourceSet ResourceSet = Man.GetResourceSet(CultureInfo.InvariantCulture, false, false);
            return CreateClassFromResourceSet(ResourceSet, Namespace, Classname, FileName);
        }

        /// <summary>
        /// Creates a strongly typed resource from a ResourceSet object. The ResourceSet passed should always
        /// be the invariant resourceset that contains all the ResourceIds.
        /// 
        /// Creates strongly typed keys for each of the keys/values.
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <param name="Namespace">Namespace of the generated class. Pass Null or string.Empty to not generate a namespace</param>
        /// <param name="classname">Name of the class to generate. Pass null to use the ResourceSet name</param>
        /// <param name="fileName">Output filename for the CSharp class. If null no file is generated and only the class is returned</param>
        /// <returns></returns>
        public string CreateClassFromResourceSet(ResourceSet resourceSet, string nameSpace, string classname, string fileName)
        {
            IsVb = IsFileVb(fileName);

            StringBuilder sbClass = new StringBuilder();

            CreateClassHeader(classname, IsVb, sbClass);

            string indent = "\t\t";

            // Any resource set that contains a '.' is considered a Local Resource
            bool IsGlobalResource = !classname.Contains(".");

            // We'll enumerate through the Recordset to get all the resources
            IDictionaryEnumerator Enumerator = resourceSet.GetEnumerator();

            // We have to turn into a concrete Dictionary            
            while (Enumerator.MoveNext())
            {
                DictionaryEntry item = (DictionaryEntry)Enumerator.Current;
                if (item.Value == null)
                    item.Value = string.Empty;

                string typeName = item.Value.GetType().FullName;
                string key = item.Key as string;                
                if (string.IsNullOrEmpty(key))
                    continue;

                string varName = SafeVarName(key);

                // It's a string
                if (!IsVb)
                {
                    sbClass.Append(indent + "public static " + typeName + " " + varName + "\r\n" + indent + "{\r\n");
                    sbClass.AppendFormat(indent + "\tget\r\n" +
                                         indent + "\t{{\r\n" +
                                         indent + "\t\tif (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)\r\n" +
                                         indent + "\t\t\treturn ({2}) HttpContext.GetGlobalResourceObject(\"{0}\",\"{1}\");\r\n" +                                         
                                         
                                         indent + "\t\treturn " +
                                         (typeName == "System.String" ? "DbRes.T(\"{1}\",\"{0}\");" : "({2}) DbRes.TO(\"{1}\",\"{0}\");") + "\r\n" +
                                         indent + "\t}}\r\n",
                                         classname, key,typeName);
                    sbClass.Append(indent + "}\r\n\r\n");                    
                }
                else
                {
                    sbClass.Append(indent + "Public Shared Property " + varName + "() as " + typeName + "\r\n");
                    sbClass.AppendFormat(indent + "\tGet\r\n" + indent + "\t\treturn CType( HttpContext.GetGlobalResourceObject(\"{0}\",\"{1}\"), {2})\r\n",
                                         classname, key,typeName);
                    sbClass.Append(indent + "\tEnd Get\r\n");
                    sbClass.Append(indent + "End Property\r\n\r\n");
                }
            }

            if (!IsVb)
                sbClass.Append("\t}\r\n\r\n");
            else
                sbClass.Append("End Class\r\n\r\n");

            string Output = CreateNameSpaceWrapper(nameSpace,IsVb,sbClass.ToString() );

            if (!string.IsNullOrEmpty(fileName))
            {
                File.WriteAllText(fileName, Output);
                return Output;
            }
                
            return sbClass.ToString();
        }

        /// <summary>
        /// Creates the class header for a page
        /// </summary>
        /// <param name="Classname"></param>
        /// <param name="IsVb"></param>
        /// <param name="sbClass"></param>
        private void CreateClassHeader(string Classname, bool IsVb, StringBuilder sbClass)
        {
            if (Classname.Contains("/") || Classname.Contains("\\"))
            {
                Classname = Classname.Replace("\\", "/");
                Classname = Classname.Substring(Classname.LastIndexOf("/")+1);
            }

            if (!IsVb)
                sbClass.Append("\tpublic class " + Classname + "\r\n\t{\r\n");
            else
                sbClass.Append("Public Class " + Classname + "\r\n");
        }

        /// <summary>
        /// Wraps the body of a class (or multiple classes) into a namespace
        /// and adds teh appropriate using/imports statements. If no namespace is
        /// passed the using/imports are still added, but no namespace is assigned
        /// </summary>
        /// <param name="Namespace"></param>
        /// <param name="IsVb"></param>
        /// <param name="Class"></param>
        /// <returns></returns>
        private string CreateNameSpaceWrapper(string Namespace, bool IsVb, string Class)
        {
            StringBuilder sbOutput = new StringBuilder();

            if (!IsVb)
               sbOutput.Append("using System;\r\nusing System.Web;\r\nusing Westwind.Globalization;\r\n\r\n");
            else
                sbOutput.Append("Imports System\r\nImports System.Web\r\nImports Westwind.Globalization\r\n\r\n");

            if (!string.IsNullOrEmpty(Namespace))
            {
                if (!IsVb)
                {
                    sbOutput.Append("namespace " + Namespace + "\r\n{\r\n");

                    sbOutput.AppendLine(
@"    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.AspNetResourceProvider;
    }
");

                    sbOutput.Append(Class);
                    sbOutput.Append("}\r\n");
                }
                else
                {
                    sbOutput.Append("Namespace " + Namespace + "\r\n\r\n");
                    sbOutput.Append(Class);
                    sbOutput.Append("End NameSpace\r\n");
                }
            }
            else
                sbOutput.Append(Class);

            return sbOutput.ToString();
        }

        /// <summary>
        /// Checks to see if the file extension is .vb and if so 
        /// returns true
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private bool IsFileVb(string FileName)
        {
            if (FileName == null)
                return IsVb;

            IsVb = false;
            if (FileName.ToLower().EndsWith("vb"))
                IsVb = true;

            return IsVb;
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
                
                if (nextUpper)
                    sb.Append(char.ToUpper(ch));
                else
                    sb.Append(ch);

                nextUpper = false;
            }

            return sb.ToString();
        }

#if false
            /// <summary>
        /// Generates a strongly typed assembly from the resources
        /// 
        /// UNDER CONSTRUCTION. 
        /// Doesn't work correctly for Web forms due to hard coded resource managers.
        /// </summary>
        /// <param name="ResourceSetName"></param>
        /// <param name="Namespace"></param>
        /// <param name="Classname"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public bool CreateStronglyTypedResource(string ResourceSetName,string Namespace, string Classname, string FileName)
        {
            try
            {
                //wwDbResourceDataManager Data = new wwDbResourceDataManager();
                //IDictionary ResourceSet = Data.GetResourceSet("", ResourceSetName);

                // Use the custom ResourceManage to retrieve a ResourceSet
                DbResourceManager Man = new DbResourceManager(ResourceSetName);
                ResourceSet rs = Man.GetResourceSet(CultureInfo.InvariantCulture, false, false);
                IDictionaryEnumerator Enumerator = rs.GetEnumerator();

                // We have to turn into a concret Dictionary
                Dictionary<string, object> Resources = new Dictionary<string, object>();
                while (Enumerator.MoveNext())
                {
                    DictionaryEntry Item = (DictionaryEntry) Enumerator.Current;
                    Resources.Add(Item.Key as string, Item.Value);
                }
                
                string[] UnmatchedElements;
                CodeDomProvider CodeProvider = null;

                string FileExtension = Path.GetExtension(FileName).TrimStart('.').ToLower();
                if (FileExtension == "cs")
                    CodeProvider = new Microsoft.CSharp.CSharpCodeProvider();
                else if(FileExtension == "vb")
                    CodeProvider = new Microsoft.VisualBasic.VBCodeProvider();

                CodeCompileUnit Code = StronglyTypedResourceBuilder.Create(Resources,
                                                   ResourceSetName, Namespace, CodeProvider, false, out UnmatchedElements);

                StreamWriter writer = new StreamWriter(FileName);
                CodeProvider.GenerateCodeFromCompileUnit(Code, writer, new CodeGeneratorOptions());
                writer.Close();
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }

            return true;
        }
#endif


    }

    public enum ResourceAccessMode
    {
        DbResourceManager,
        AspNetResourceProvider
    }
}
