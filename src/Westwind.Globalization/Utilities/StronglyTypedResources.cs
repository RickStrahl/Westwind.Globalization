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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Data;
using System.IO;

using System.Xml;
using System.Collections;
using System.Linq;

#if NETFULL
using System.Resources.Tools;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
#endif

namespace Westwind.Globalization
{
    /// <summary>
    /// Class that handles generating strongly typed resources 
    /// for global Web resource files. This feature is not supported
    /// in ASP.NET stock projects and doesn't support custom resource
    /// providers in WAP.
    /// </summary>
    public class StronglyTypedResources
    {
                
        public StronglyTypedResources(string webPhysicalPath)
        {
            this.WebPhysicalPath = webPhysicalPath;
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

            string Output = CreateNameSpaceWrapper(Namespace, IsVb, sbClasses.ToString(), true);
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
        public string CreateClassFromAllDatabaseResources(string Namespace, string FileName, IEnumerable<string> resourceSets = null)
        {
            var man = DbResourceDataManager.CreateDbResourceDataManager();  
            var resources = man.GetAllResourceSets(ResourceListingTypes.GlobalResourcesOnly);

            if (resourceSets != null)
            {
                if (resourceSets != null)
                    resources = resources.Where(rs => resourceSets.Any(rs1 => rs1 == rs))
                                         .ToList();
            }

            StringBuilder sbClasses = new StringBuilder();
            foreach (var resourceSet in resources)
            {                
                string Class = CreateClassFromDatabaseResource(resourceSet, Namespace, resourceSet, null);
                sbClasses.Append(Class);
            }

            string Output = CreateNameSpaceWrapper(Namespace, IsVb, sbClasses.ToString(), true);
            File.WriteAllText(FileName, Output );

            return Output;
        }

        /// <summary>
        /// Creates strongly typed classes from all global resources in the current application
        /// from the active DbResourceManager. One class is created which contains each of the
        /// resource classes. Classnames are not necessarily named with
        /// 
        /// Uses the default DbResourceConfiguration.Current settings for connecting
        /// to the database.
        /// </summary>
        /// <param name="ns">Optional namespace for the generated file</param>
        /// <param name="fileName">Output class file. .cs or .vb determines code language</param>
        /// <returns>Generated class as a string</returns>
        public string CreateResxDesignerClassesFromAllDatabaseResources(string ns, string outputPath, IEnumerable<string> resourceSets = null)
        {               
            var man = DbResourceDataManager.CreateDbResourceDataManager();
            var resources = man.GetAllResourceSets(ResourceListingTypes.GlobalResourcesOnly);

            if (resourceSets != null)
            {
                if (resourceSets != null)
                    resources = resources.Where(rs => resourceSets.Any(rs1 => rs1 == rs))
                                         .ToList();
            }

            string classCode = null;
            StringBuilder sbClasses = new StringBuilder();
            foreach (var resourceSet in resources)
            {

                classCode = CreateResxDesignerClassFromResourceSet(resourceSet, ns, resourceSet, null);
                StringBuilder sb = new StringBuilder(classCode);
                classCode = CreateResxDesignerNameSpaceWrapper(ns, IsVb, sb.ToString());

                string outputFile = Path.Combine(outputPath, resourceSet + ".designer." + (IsVb ? "vb" : "cs"));
                File.WriteAllText(outputFile, classCode);
            }

            return classCode;
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
            

            CreateClassHeader(Classname, Namespace, IsVb, sbClass);
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
                    sbClass.Append(Indent + "Public Shared ReadOnly Property " + resourceId + "() as " + typeName + "\r\n");
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
                string FileContent = CreateNameSpaceWrapper(Namespace, IsVb, sbClass.ToString(),true);
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
        /// <param name="resourceSetName"></param>
        /// <param name="nameSpace">Namespace of the generated class. Pass Null or string.Empty to not generate a namespace</param>
        /// <param name="classname">Name of the class to generate. Pass null to use the ResourceSet name</param>
        /// <param name="fileName">Output filename for the CSharp class. If null no file is generated and only the class is returned</param>
        /// <returns></returns>
        public string CreateResxDesignerClassFromResourceSet(string resourceSetName, string nameSpace, string classname, string fileName)
        {
            classname = SafeClassName(classname);        

            // Use the custom ResourceManage to retrieve a ResourceSet
            var man = new DbResourceManager(resourceSetName);
            var resourceSet = man.GetResourceSet(CultureInfo.InvariantCulture, false, false);
            
            IsVb = IsFileVb(fileName);

            StringBuilder sbClass = new StringBuilder();

            CreateResxDesignerClassHeader(classname, nameSpace, IsVb, sbClass);

            string indent = "\t\t";

            
            // We'll enumerate through the Recordset to get all the resources
            IDictionaryEnumerator enumerator = resourceSet.GetEnumerator();

            // We have to turn into a concrete Dictionary            
            while (enumerator.MoveNext())
            {
                DictionaryEntry item = (DictionaryEntry)enumerator.Current;
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
                    sbClass.AppendLine(indent + "public static " + typeName + " " + varName + "\r\n" + indent + "{");
                    sbClass.AppendFormat(indent + "\tget\r\n" +
                                         indent + "\t{{\r\n" + 
                                         indent + "\t\t" +
                                         (string.IsNullOrEmpty(typeName) || typeName == "System.String" 
                                          ? "return ResourceManager.GetString(\"{0}\", resourceCulture);\r\n"
                                          : "return ({1})ResourceManager.GetObject(\"{0}\", resourceCulture);\r\n")  +                                     
                                         indent + "\t}}\r\n",
                                         key,typeName);
                    sbClass.AppendLine(indent + "}\r\n");                    
                }
                else
                {
                    sbClass.Append(indent + "Public Shared Property " + varName + "() as " + typeName + "\r\n");
                    sbClass.AppendFormat(indent + "\tGet\r\n" + indent + "\t\treturn CType( ResourceManager.GetObject(\"{1}\",resourceCulture)\r\n",
                                         classname, key,typeName);
                    sbClass.Append(indent + "\tEnd Get\r\n");
                    sbClass.Append(indent + "End Property\r\n\r\n");
                }
            }

            if (!IsVb)
                sbClass.Append("\t}\r\n\r\n");
            else
                sbClass.Append("End Class\r\n\r\n");

            string Output = CreateNameSpaceWrapper(nameSpace,IsVb,sbClass.ToString(), false );

            if (!string.IsNullOrEmpty(fileName))
            {
                File.WriteAllText(fileName + ".designer" + (IsVb ? ".vb" : ".cs"), Output);
                return Output;
            }
                
            return sbClass.ToString();
        }

#if NETFULL

        /// <summary>
        /// Creates a StronglyTyped class from a REsx file. Can be used after a Resx fi
        /// </summary>
        /// <param name="resxFile"></param>
        /// <param name="resourceSetName"></param>
        /// <param name="namespaceName"></param>
        public void CreateResxDesignerClassFromResxFile(string resxFile, string resourceSetName, string namespaceName, bool vb = false)
        {

            string outfile;
            CodeDomProvider provider;
            if (!vb)
            {
                provider = new CSharpCodeProvider();
                outfile = Path.ChangeExtension(resxFile, "designer.cs");
            }
            else
            {
                provider = new VBCodeProvider();
                outfile = Path.ChangeExtension(resxFile, "designer.vb");
            }

            using (StreamWriter sw = new StreamWriter(outfile))
            {
                string[] errors = null;

                var curPath = Environment.CurrentDirectory;
                Directory.SetCurrentDirectory(Path.GetDirectoryName(resxFile));

                var code = StronglyTypedResourceBuilder.Create(resxFile, resourceSetName, namespaceName, provider, false,
                    out errors);

                Directory.SetCurrentDirectory(curPath);

                if (errors.Length > 0)
                {
                    foreach (var error in errors)
                    {
                        ErrorMessage += error + "\r\n";
                    }
                    throw new ApplicationException(ErrorMessage);
                }

                provider.GenerateCodeFromCompileUnit(code, sw, new CodeGeneratorOptions());                                                         
            }
        }
#endif

        public string CreateClassFromResourceSet(ResourceSet resourceSet, string nameSpace, string classname, string fileName)
        {
            IsVb = IsFileVb(fileName);

            StringBuilder sbClass = new StringBuilder();

            CreateClassHeader(classname, nameSpace, IsVb, sbClass);

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
                                         indent +
                                         (string.IsNullOrEmpty(typeName) || typeName == "System.String"
                                         ? "\t\t" + @"return GeneratedResourceHelper.GetResourceString(""{0}"",""{1}"",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);" + "\r\n"
                                         : "\t\t" + @"return ({2}) GeneratedResourceHelper.GetResourceObject(""{0}"",""{1}"",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);" + "\r\n")
                                         +
                                         indent + "\t}}\r\n",
                                         classname, key, typeName);
                    sbClass.Append(indent + "}\r\n\r\n");
                }
                else
                {
                    sbClass.Append(indent + "Public Shared Property " + varName + "() as " + typeName + "\r\n");
                    sbClass.AppendFormat(indent + "\tGet\r\n" + indent + "\t\treturn CType( HttpContext.GetGlobalResourceObject(\"{0}\",\"{1}\"), {2})\r\n",
                                         classname, key, typeName);
                    sbClass.Append(indent + "\tEnd Get\r\n");
                    sbClass.Append(indent + "End Property\r\n\r\n");
                }
            }

            if (!IsVb)
                sbClass.Append("\t}\r\n\r\n");
            else
                sbClass.Append("End Class\r\n\r\n");

            string Output = CreateNameSpaceWrapper(nameSpace, IsVb, sbClass.ToString(),false);

            if (!string.IsNullOrEmpty(fileName))
            {
                File.WriteAllText(fileName + ".designer" + (IsVb ? ".vb" : ".cs"), Output);
                return Output;
            }

            return sbClass.ToString();
        }
        
        /// <summary>
        /// Creates the class header for a page
        /// </summary>
        /// <param name="classname"></param>
        /// <param name="IsVb"></param>
        /// <param name="sbClass"></param>
        private void CreateClassHeader(string classname, string nameSpace, bool IsVb, StringBuilder sbClass)
        {
            if (classname.Contains("/") || classname.Contains("\\"))
            {
                classname = classname.Replace("\\", "/");
                classname = classname.Substring(classname.LastIndexOf("/")+1);
            }

            if (!IsVb)
            {                
                sbClass.Append(
$@"
    [System.CodeDom.Compiler.GeneratedCodeAttribute(""Westwind.Globalization.StronglyTypedResources"", ""3.0"")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class {classname}
    {{
        public static ResourceManager ResourceManager
        {{
            get
            {{
                if (object.ReferenceEquals(resourceMan, null))
                {{
                    var temp = new ResourceManager(""{nameSpace}.{classname}"", typeof({classname}).Assembly);
                    resourceMan = temp;
                }}
                return resourceMan;
            }}
        }}
        private static ResourceManager resourceMan = null;

");

            }
            else
            {
                sbClass.Append("Public Class " + classname + "\r\n");
            }
        }

        /// <summary>
        /// Creates the class header for a page
        /// </summary>
        /// <param name="Classname"></param>
        /// <param name="IsVb"></param>
        /// <param name="sbClass"></param>
        private void CreateResxDesignerClassHeader(string Classname, string nameSpace, bool IsVb, StringBuilder sbClass)
        {
            if (Classname.Contains("/") || Classname.Contains("\\"))
            {
                Classname = Classname.Replace("\\", "/");
                Classname = Classname.Substring(Classname.LastIndexOf("/") + 1);
            }

            if (!IsVb)
            {
                sbClass.Append(
$@"
    [System.CodeDom.Compiler.GeneratedCodeAttribute(""Westwind.Globalization.StronglyTypedResources"", ""2.0"")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class {Classname}
    {{    
        /// <summary>
        /// ResourceManager instance used to retrieve resources in Resx mode.
        /// You can replace this resource manager with your own 
        /// but it applies only in Resx mode.
        /// </summary>
        public static ResourceManager ResourceManager
        {{
            get
            {{
                if (object.ReferenceEquals(resourceMan, null))
                {{
                    var temp = new ResourceManager(""{nameSpace}.{Classname}"", typeof({Classname}).Assembly);
                    resourceMan = temp;
                }}
                return resourceMan;
            }}
        }}
        private static ResourceManager resourceMan ;

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {{
            get {{
                return resourceCulture;
            }}
            set {{
                resourceCulture = value;
            }}
        }}
        private static System.Globalization.CultureInfo resourceCulture;

");

            }
            else
            {
                sbClass.Append("Public Class " + Classname + "\r\n");
            }
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
        private string CreateNameSpaceWrapper(string Namespace, bool IsVb, string Class, bool generateGeneratedResourceSettings)
        {
            StringBuilder sbOutput = new StringBuilder();

            if (!IsVb)
               sbOutput.Append(@"using System;
using System.Web;
using System.Resources;
using Westwind.Globalization;

");
            else
                sbOutput.Append("Imports System\r\nImports System.Web\r\nImports System.Resources\r\nImports Westwind.Globalization\r\n\r\n");

            if (!string.IsNullOrEmpty(Namespace))
            {
                if (!IsVb)
                {
                    sbOutput.Append("namespace " + Namespace + "\r\n{\r\n");

                    if (generateGeneratedResourceSettings)
                    {
                        sbOutput.AppendLine(
                            @"    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = DbResourceConfiguration.Current.ResourceAccessMode;
    }
");
                    }
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
        /// Wraps the body of a class (or multiple classes) into a namespace
        /// and adds teh appropriate using/imports statements. If no namespace is
        /// passed the using/imports are still added, but no namespace is assigned
        /// </summary>
        /// <param name="Namespace"></param>
        /// <param name="IsVb"></param>
        /// <param name="Class"></param>
        /// <returns></returns>
        private string CreateResxDesignerNameSpaceWrapper(string Namespace, bool IsVb, string Class)
        {
            StringBuilder sbOutput = new StringBuilder();

            if (!IsVb)
                sbOutput.Append(@"using System;
");
            else
                sbOutput.Append("Imports System\r\n\r\n");

            if (!string.IsNullOrEmpty(Namespace))
            {
                if (!IsVb)
                {
                    sbOutput.Append("namespace " + Namespace + "\r\n{\r\n");
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


        /// <summary>
        /// Creates a safe variable name
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static string SafeVarName(string phrase)
        {
            if (phrase == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder(phrase.Length);

            // First letter is always upper case alpha char
            bool nextUpper = false;
            bool isFirst = true;

            foreach (char ch in phrase)
            {
                if (isFirst && !char.IsLetter(ch))
                    sb.Append("_"); // prefix
                

                // skip
                if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || char.IsSeparator(ch) || char.IsControl(ch) || char.IsSymbol(ch) )
                {
                    sb.Append("_");
                    nextUpper = true;
                    continue;
                }

                isFirst = false;
                sb.Append(nextUpper ? char.ToUpper(ch) : ch);

                nextUpper = false;
            }

            return sb.ToString();
        }
        
        private string SafeClassName(string classname)
        {

            StringBuilder sb = new StringBuilder();

            foreach (char c in classname)
            {
                if (c == ' ' || c == '-')
                    sb.Append('_');                    
                else if ( (c >= 'A' && c <= 'Z') || ( c >= 'a' && c <= 'z') ||  (c >= '0' && c <= '9') )
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
