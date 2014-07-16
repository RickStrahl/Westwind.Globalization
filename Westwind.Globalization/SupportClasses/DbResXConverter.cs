using System;
using System.Text;
using System.Data;
using System.Xml;
using System.Linq;
using System.IO;
using System.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Web;

namespace Westwind.Globalization
{
    /// <summary>
    /// This class can be used to export resources from the database to ASP.NET
    /// compatible resources (Resx). This class takes all the resources in 
    /// the database and creates RESX files that match these resources.
    /// 
    /// Please note that it will overrwrite any existing resource files
    /// if they already exist, so please use this class with care if
    /// you have existing ResX resources.
    /// 
    /// Note this class is primarily ASP.NET specific in that it looks at
    /// ASP.NET specific directory structures for ResX imports and strongly
    /// typed resource creation.
    /// </summary>
    public class DbResXConverter
    {

        /// <summary>
        /// Creates new instance with the default Web Application
        /// base path set to the current Web application's path.
        /// </summary>
        public DbResXConverter() : this(null) { }

        /// <summary>
        /// Pass in the base phyiscal path for the project. 
        /// 
        /// For Web Projects this will be the Web root dir for
        /// non-Web projects this will be the project base path 
        /// as a string.
        /// </summary>
        /// <param name="basePhsyicalPath"></param>
        public DbResXConverter(string basePhsyicalPath)
        {
            if (string.IsNullOrEmpty(basePhsyicalPath))
                basePhsyicalPath = HttpContext.Current.Request.PhysicalApplicationPath;

            this.BasePhysicalPath = basePhsyicalPath;
        }

        /// <summary>
        /// The physical path of the Web application. This path serves as 
        /// the root path to write resources to.
        /// 
        /// Example: c:\projects\MyWebApp
        /// </summary>
        public string BasePhysicalPath
        {
            get { return _basePhysicalPath; }
            set 
            {
                if (value != null && !value.EndsWith("\\"))
                    value += "\\";

                _basePhysicalPath = value; 
            }
        }
        private string _basePhysicalPath = "";



        /// <summary>
        /// Error message if an operation fails
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
        private string _ErrorMessage = "";


        /// <summary>
        /// Genereates Local Web Resource ResX files from the DbResourceDataManager
        /// </summary>
        /// <returns></returns>
        public bool GenerateLocalWebResourceResXFiles()
        {
            return this.GenerateWebResourceResXFiles(true);
        }
        /// <summary>
        /// Genereates Local Web Resource ResX files from the DbResourceDataManager
        /// </summary>
        /// <returns></returns>
        public bool GenerateGlobalWebResourceResXFiles()
        {
            return this.GenerateWebResourceResXFiles(false);
        }


        /// <summary>
        /// Dumps resources from the DbResourceProvider database
        /// out to Resx resources in an ASP.NET application
        /// creating the appropriate APP_LOCAL_RESOURCES/APP_GLOBAL_RESOURCES
        /// folders and resx files.
        /// IMPORTANT: will overwrite existing files
        /// </summary>
        /// <param name="localResources"></param>
        /// <returns></returns>
        protected bool GenerateWebResourceResXFiles(bool localResources)
        {
            DbResourceDataManager Data = new DbResourceDataManager();
            
            // Retrieve all resources for a ResourceSet for all cultures
            // The data is ordered by ResourceSet, LocaleId and resource ID as each
            // ResourceSet or Locale changes a new file is written
            DataTable dtResources = Data.GetAllResources(localResources);
            
            if (dtResources == null)
                return false;

            string LastSet = "";
            string LastLocale = "@!";

            // Load the document schema
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ResXDocumentTemplate);

            XmlWriter xWriter = null;
            XmlWriterSettings XmlSettings = new XmlWriterSettings();

            //// Make sure we use fragment syntax so there's no validation
            //// otherwise loading the original string will fail
            XmlSettings.ConformanceLevel = ConformanceLevel.Document;            
            XmlSettings.IndentChars = "   ";
            XmlSettings.Indent = true;
                       
            foreach(DataRow dr in dtResources.Rows)
            {
                // Read into vars for easier usage below
                string ResourceId = dr["ResourceId"] as string;                
                string Value = dr["Value"] as string;
                string Comment = dr["Comment"] as string;

                string Type = dr["Type"] as string;
                string TextFile = dr["TextFile"] as string;
                byte[] BinFile = dr["BinFile"] as byte[];
                string FileName = dr["FileName"] as string;

                string ResourceSet = dr["ResourceSet"] as string;                

                string LocaleId = dr["LocaleId"] as string;
                LocaleId = LocaleId.ToLower();
                                 
                // Create a new output file if the resource set or locale changes
                if (ResourceSet != LastSet || LocaleId != LastLocale )
                {
                    if (xWriter != null)
                    {
                        //xWriter.WriteRaw("\r\n</root>");
                        xWriter.WriteEndElement();
                        xWriter.Close();
                    }

                    string Loc = ".resx";
                    if (LocaleId != "")
                        Loc = "." + LocaleId + ".resx";

                    //xWriter = XmlWriter.Create( this.FormatResourceSetPath(ResourceSet,LocalResources) + Loc,XmlSettings) ;
                    string resourceFilename = this.FormatWebResourceSetPath(ResourceSet, localResources) + Loc;
                    XmlTextWriter Writer = new XmlTextWriter(resourceFilename, Encoding.UTF8);
                    Writer.Indentation = 3;
                    Writer.IndentChar = ' ';
                    Writer.Formatting = Formatting.Indented;
                    xWriter = Writer as XmlWriter;
                    
                    xWriter.WriteStartElement("root");

                    // Write out the schema
                    doc.DocumentElement.ChildNodes[0].WriteTo(xWriter);
                        
                    // Write out the leading resheader elements
                    XmlNodeList Nodes = doc.DocumentElement.SelectNodes("resheader");
                    foreach(XmlNode Node in Nodes)
                    {
                        Node.WriteTo(xWriter);
                    }
                    
                    LastSet = ResourceSet;
                    LastLocale = LocaleId;
                }
                
                if (Type == "")  // plain string value
                {
                    //<data name="LinkButton1Resource1.Text" xml:space="preserve">
                    //    <value>LinkButton</value>
                    //</data>
                    xWriter.WriteStartElement("data");
                    xWriter.WriteAttributeString("name", ResourceId);                    
                    xWriter.WriteAttributeString("xml","space",null,"preserve");                                        
                    xWriter.WriteElementString("value", Value);
                    if (!string.IsNullOrEmpty(Comment))
                        xWriter.WriteElementString("comment", Comment);
                    xWriter.WriteEndElement(); // data
                }
                // File Resources get written to disk
                else if (Type == "FileResource")  
                {
                    string resourceFilenname = this.FormatWebResourceSetPath(ResourceSet,localResources);
                    string resourcePath = new FileInfo(resourceFilenname).DirectoryName;

                    if (Value.IndexOf("System.String") > -1)
                    {
                        string[] Tokens = Value.Split(';');
                        Encoding Encode = Encoding.Default;
                        try
                        {
                            if (Tokens.Length == 3)
                                Encode = Encoding.GetEncoding(Tokens[2]);

                            // Write out the file to disk
                            File.WriteAllText(resourcePath + "\\" + FileName, TextFile, Encode);
                        }
                        catch
                        {
                        }
                    }
                    else 
                    {
                        File.WriteAllBytes(resourcePath + "\\" + FileName,BinFile);
                    }

                  //<data name="Scratch" type="System.Resources.ResXFileRef, System.Windows.Forms">
                  //  <value>Scratch.txt;System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;Windows-1252</value>
                  //</data>
                    xWriter.WriteStartElement("data");
                    xWriter.WriteAttributeString("name", ResourceId);
                    xWriter.WriteAttributeString("type", "System.Resources.ResXFileRef, System.Windows.Forms");
                    
                    // values are already formatted in the database
                    xWriter.WriteElementString("value", Value);
                    if (!string.IsNullOrEmpty(Comment))
                        xWriter.WriteElementString("comment", Comment); 
                    
                    xWriter.WriteEndElement(); // data
                }                            

            } // foreach dr

            if (xWriter != null)
            {
                xWriter.WriteEndElement();
                //xWriter.WriteRaw("\r\n</root>");
                xWriter.Close();
            }

            return true;
        }

        /// <summary>
        /// Generates Resx Files for standard non-Web Resource files        
        /// based on the BasePhysicalPath
        /// </summary>
        /// <returns></returns>
        public bool GenerateResXFiles()
        {
            DbResourceDataManager Data = new DbResourceDataManager();

            // Retrieve all resources for a ResourceSet for all cultures
            // The data is ordered by ResourceSet, LocaleId and resource ID as each
            // ResourceSet or Locale changes a new file is written
            DataTable dtResources = Data.GetAllResources();

            if (dtResources == null)
                return false;

            string lastSet = "";
            string lastLocale = "@!";

            // Load the document schema
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ResXDocumentTemplate);

            XmlWriter xWriter = null;
            var xmlSettings = new XmlWriterSettings();

            //// Make sure we use fragment syntax so there's no validation
            //// otherwise loading the original string will fail
            xmlSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlSettings.IndentChars = "   ";
            xmlSettings.Indent = true;

            foreach (DataRow dr in dtResources.Rows)
            {
                // Read into vars for easier usage below
                string resourceId = dr["ResourceId"] as string;
                string value = dr["Value"] as string;
                string comment = dr["Comment"] as string;

                string type = dr["Type"] as string;
                string textFile = dr["TextFile"] as string;
                byte[] binFile = dr["BinFile"] as byte[];
                string fileName = dr["FileName"] as string;

                string resourceSet = dr["ResourceSet"] as string;
                //ResourceSet = ResourceSet.ToLower();

                string localeId = dr["LocaleId"] as string;
                localeId = localeId.ToLower();

                // Create a new output file if the resource set or locale changes
                if (resourceSet != lastSet || localeId != lastLocale)
                {
                    if (xWriter != null)
                    {
                        xWriter.WriteEndElement();
                        xWriter.Close();
                    }

                    string localizedExtension = ".resx";
                    if (localeId != "")
                        localizedExtension = "." + localeId + ".resx";

                    var fullFileName = this.FormatResourceSetPath(resourceSet) + localizedExtension;

                    XmlTextWriter writer = new XmlTextWriter(fullFileName,Encoding.UTF8);
                    writer.Indentation = 3;
                    writer.IndentChar = ' ';
                    writer.Formatting = Formatting.Indented;
                    xWriter = writer as XmlWriter;

                    xWriter.WriteStartElement("root");

                    // Write out the schema
                    doc.DocumentElement.ChildNodes[0].WriteTo(xWriter);

                    // Write out the leading resheader elements
                    XmlNodeList Nodes = doc.DocumentElement.SelectNodes("resheader");
                    foreach (XmlNode Node in Nodes)
                    {
                        Node.WriteTo(xWriter);
                    }

                    lastSet = resourceSet;
                    lastLocale = localeId;
                }

                if (type == "")  // plain string value
                {
                    //<data name="LinkButton1Resource1.Text" xml:space="preserve">
                    //    <value>LinkButton</value>
                    //</data>
                    xWriter.WriteStartElement("data");
                    xWriter.WriteAttributeString("name", resourceId);
                    xWriter.WriteAttributeString("xml", "space", null, "preserve");
                    xWriter.WriteElementString("value", value);
                    if (!string.IsNullOrEmpty(comment))
                        xWriter.WriteElementString("comment", comment);
                    xWriter.WriteEndElement(); // data
                }
                // File Resources get written to disk
                else if (type == "FileResource")
                {
                    string ResourceFilePath = this.FormatResourceSetPath(resourceSet);
                    string ResourcePath = new FileInfo(ResourceFilePath).DirectoryName;

                    if (value.IndexOf("System.String") > -1)
                    {
                        string[] Tokens = value.Split(';');
                        Encoding Encode = Encoding.Default;
                        try
                        {
                            if (Tokens.Length == 3)
                                Encode = Encoding.GetEncoding(Tokens[2]);

                            // Write out the file to disk
                            File.WriteAllText(ResourcePath + "\\" + fileName, textFile, Encode);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        File.WriteAllBytes(ResourcePath + "\\" + fileName, binFile);
                    }

                    //<data name="Scratch" type="System.Resources.ResXFileRef, System.Windows.Forms">
                    //  <value>Scratch.txt;System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;Windows-1252</value>
                    //</data>
                    xWriter.WriteStartElement("data");
                    xWriter.WriteAttributeString("name", resourceId);
                    xWriter.WriteAttributeString("type", "System.Resources.ResXFileRef, System.Windows.Forms");

                    // values are already formatted in the database
                    xWriter.WriteElementString("value", value);
                    if (!string.IsNullOrEmpty(comment))
                        xWriter.WriteElementString("comment", comment);

                    xWriter.WriteEndElement(); // data
                }

            } // foreach dr

            if (xWriter != null)
            {
                xWriter.WriteEndElement();
                //xWriter.WriteRaw("\r\n</root>");
                xWriter.Close();
            }

            return true;
        }



        /// <summary>
        /// Translates the resource set path ASP.NET WebForms Global 
        /// or local resource path base (ie. without the .resx and localeId extension).
        /// </summary>
        /// <param name="ResourceSet"></param>
        /// <returns></returns>
        public string FormatWebResourceSetPath(string ResourceSet, bool LocalResources)
        {
               // Make sure our slashes are right
               ResourceSet = ResourceSet.Replace("/","\\");

               if (LocalResources)
               {
                   // Inject App_LocalResource
                   ResourceSet = ResourceSet.Insert(ResourceSet.LastIndexOf('\\')+1, "App_LocalResources\\");
                   ResourceSet = this.BasePhysicalPath + ResourceSet;
               }
               else
               {
                   ResourceSet = this.BasePhysicalPath + "App_GlobalResources\\" + ResourceSet;
               }

               FileInfo fi = new FileInfo(ResourceSet);
               if (!fi.Directory.Exists)
                   fi.Directory.Create();
            
               return ResourceSet;
        }


        /// <summary>
        /// Determines if a resourceset is a local resource based
        /// on the extension of the resource set
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        public static bool IsLocalResourceSet(string resourceSet)
        {
            var lres = resourceSet.ToLower();
            if (lres.EndsWith(".aspx") || lres.EndsWith(".ascx") || lres.EndsWith(".master") || lres.EndsWith(".sitemap"))
                return true;

            return false;
        }

        /// <summary>
        /// Returns the path the resource file withouth the resx and localeId extension
        /// </summary>
        /// <param name="resourceSet"></param>
        /// <param name="LocalResources"></param>
        /// <returns></returns>
        public string FormatResourceSetPath(string resourceSet)
        {
            // Make sure our slashes are right
            resourceSet = this.BasePhysicalPath +  resourceSet;
            resourceSet = resourceSet.Replace("/", "\\");

            if (IsLocalResourceSet(resourceSet) && !resourceSet.Contains("App_LocalResources"))
            {
                string pathOnly = Path.GetDirectoryName(resourceSet);
                string fileOnly = Path.GetFileName(resourceSet);

                resourceSet = pathOnly + "\\App_LocalResources\\" + fileOnly;
            }

            FileInfo fi = new FileInfo(resourceSet);
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            return resourceSet;
        }


        /// <summary>
        /// Imports ResX Web Resources of a Web application by parsing through
        /// the App_GlobalResources and App_LocalResources directories of 
        /// a Web site.
        /// 
        /// Note: Requires that WebPhysicalPath is set to point at the 
        /// Web root directory.
        /// </summary>
        /// <returns></returns>
        public bool ImportWebResources()
        {
            return this.ImportWebResources(null);
        }


        /// <summary>
        /// Internal worker method that parses through the directories
        /// </summary>
        /// <param name="WebPath">Physical path of the directory to pars</param>
        /// <returns></returns>
        protected bool ImportWebResources(string WebPath)
        {
            if (WebPath == null)
                WebPath = this.BasePhysicalPath;

            WebPath = WebPath.ToLower();
            if (!WebPath.EndsWith("\\"))
                WebPath += "\\";

            string[] Directories = Directory.GetDirectories(WebPath);

            foreach (string Dir in Directories)
            {                
                string dir = Path.GetFileName(Dir);

                if (string.Compare(dir,"app_localresources",StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // We need to create a Web relative path (ie. admin/default.aspx)
                    string RelPath = WebPath.Replace(this.BasePhysicalPath.ToLower(), "");
                    RelPath = RelPath.Replace("\\","/");

                    ImportDirectoryResources(WebPath + dir + "\\",RelPath) ;
                }
                else if (string.Compare(dir,"app_globalresources",StringComparison.OrdinalIgnoreCase) == 0)
                    ImportDirectoryResources(WebPath + dir + "\\","");
                
                else if (  !("bin|app_code|app_themes|app_data|".Contains(dir.ToLower() + "|" )) )
                    // Recurse through child directories
                    ImportWebResources(WebPath + dir + "\\"); 
            }

            return true;
        }

        /// <summary>
        /// Imports Resources recursively from a non-Web project
        /// </summary>
        /// <param name="basePhysicalPath">The physical path to the directory</param>
        /// <param name="baseNamespace">The base namespace in the project to prefix InternalResourceSets with</param>
        /// <returns></returns>
        public bool ImportWinResources(string basePhysicalPath)
        {
            if (basePhysicalPath == null)
                basePhysicalPath = this.BasePhysicalPath;

            // basePhysicalPath = basePhysicalPath.ToLower();
            if (!basePhysicalPath.EndsWith("\\"))
                basePhysicalPath += "\\";

            // We need to create a Web relative path (ie. admin/myresources.resx)
            string relPath = basePhysicalPath.Replace(this.BasePhysicalPath, "");
            relPath = relPath.Replace("\\", "/");

            // Import the base path first
            ImportDirectoryResources(basePhysicalPath, relPath);
            
            // Recurse into child folders
            string[] Directories = Directory.GetDirectories(basePhysicalPath);
            foreach (string Dir in Directories)
            {
                DirectoryInfo directory = new DirectoryInfo(Dir);
                
                string dir = directory.Name;
                
                if (dir == "" || ("|bin|obj|.git|.svn|_svn|app_data|Migrations|".Contains("|" + dir.ToLower() + "|")))
                    continue;

                ImportWinResources(basePhysicalPath + dir + "\\");
            }

            return true;
        }

        /// <summary>
        /// Imports all resources from a given directory. This method works for any resources.
        /// 
        /// When using LocalResources, make sure to provide an app relative path as the second
        /// parameter if the resources live in non root folder. So if you have resources in off
        /// an Admin folder use "admin/" as the parameter. Otherwise for web root resources or
        /// global or assembly level assemblies pass string.Empty or null.   
        /// </summary>
        /// <param name="path">Physical Path for the Resources</param>
        /// <param name="relativePath">Optional - relative path prefix for Web App_LocalResources (ie. admin/)</param>
        /// <returns></returns>
        public bool ImportDirectoryResources(string path, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                relativePath = "";

            string[] Files = Directory.GetFiles(path, "*.resx");
         
            foreach (string CurFile in Files)
            {
                string file = CurFile;//.ToLower();
                
                //string[] tokens = file.Replace(".resx","").Split('.');                
                string[] tokens = Path.GetFileName(file).Replace(".resx", "").Split('.');
                

                // ResName: admin/default.aspx or default.aspx or resources (global or assembly resources)
                string localeId = "";
                string resName = relativePath + Path.GetFileNameWithoutExtension(tokens[0]);


                if (resName.Contains("App_LocalResources/"))
                    resName = resName.Replace("App_LocalResources/", "");
                else if (resName.Contains("App_GlobalResources/"))
                    resName = resName.Replace("App_GlobalResources/", "");


                if (tokens.Length > 1)
                {
                    string extension = tokens[1];
                    if ("aspx|ascx|master|sitemap|".Contains(extension.ToLower() + "|") )
                        resName += "." + extension;
                    else
                        localeId = extension;
                }
                if (tokens.Length > 2)
                {
                    localeId = tokens[2];
                }

                this.ImportResourceFile(file, resName, localeId);
            }

            return true;
        }

    /// <summary>
    /// Imports an individual ResX Resource file into the database
    /// </summary>
    /// <param name="FileName">Full path to the the ResX file</param>
    /// <param name="ResourceSetName">Name of the file or for local resources the app relative path plus filename (admin/default.aspx or default.aspx)</param>
    /// <param name="LocaleId">Locale Id of the file to import. Use "" for Invariant</param>
    /// <returns></returns>
    public bool ImportResourceFile(string FileName,string ResourceSetName,string LocaleId)
    {
        string FilePath = Path.GetDirectoryName(FileName) + "\\";
        
        DbResourceDataManager Data = new DbResourceDataManager();
        
        XmlDocument Dom = new XmlDocument();

        try
        {
            Dom.Load(FileName);
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.Message;
            return false;
        }

        XmlNodeList nodes = Dom.DocumentElement.SelectNodes("data");

        foreach (XmlNode Node in nodes)
        {
            string Value; // = Node.ChildNodes[0].InnerText;

            XmlNodeList valueNodes = Node.SelectNodes("value");
            if (valueNodes.Count == 1)
                Value = valueNodes[0].InnerText;
            else
                Value = Node.InnerText;

            string Name = Node.Attributes["name"].Value;
            string Type = null;
            if (Node.Attributes["type"] != null)
                Type = Node.Attributes["type"].Value;

            string Comment = null;
            XmlNode commentNode = Node.SelectSingleNode("comment");
            if (commentNode != null)
                Comment = commentNode.InnerText;


            if (string.IsNullOrEmpty(Type))
                Data.UpdateOrAdd(Name, Value, LocaleId, ResourceSetName,Comment);
            else
            {
                // File based resources are formatted: filename;full type name
                string[] tokens = Value.Split(';');
                if (tokens.Length > 0)
                {
                    string ResFileName = FilePath + tokens[0];
                    if (File.Exists(ResFileName) )
                        // DataManager knows about file resources and can figure type info
                        Data.UpdateOrAdd(Name, ResFileName, LocaleId, ResourceSetName, Comment,true);
                }
            }

        }

        return true;
    }

    /// <summary>
    /// Gets a specific List of resources as a list of ResxItems.
    /// This list only retrieves items for a specific locale. No
    /// resource normalization occurs.
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    internal List<ResxItem> GetResXResources(string FileName)
    {
        string FilePath = Path.GetDirectoryName(FileName) + "\\";

        XmlDocument Dom = new XmlDocument();

        try
        {
            Dom.Load(FileName);
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.Message;
            return null;
        }

        List<ResxItem> resxItems = new List<ResxItem>();
        
        XmlNodeList nodes = Dom.DocumentElement.SelectNodes("data");

        foreach (XmlNode Node in nodes)
        {
            string Value = null;

            XmlNodeList valueNodes = Node.SelectNodes("value");
            if (valueNodes.Count == 1)
                Value = valueNodes[0].InnerText;
            else
                Value = Node.InnerText;

            string Name = Node.Attributes["name"].Value;
            string Type = null;
            if (Node.Attributes["type"] != null)
                Type = Node.Attributes["type"].Value;

            ResxItem resxItem = new ResxItem() { Name = Name, Type = Type, Value = Value };
            resxItems.Add(resxItem);
        }
        return resxItems;
    }

    /// <summary>
    /// Returns all resources for a given locale normalized down the hierarchy for 
    /// a given resource file. The resource file should be specified without the
    /// .resx and locale identifier extensions.
    /// </summary>
    /// <param name="baseFile">The base Resource file without .resx and locale extensions</param>
    /// <param name="LocaleId"></param>
    /// <returns>Dictionary of resource keys and values</returns>
    public Dictionary<string, object> GetResXResourcesNormalizedForLocale(string baseFile, string LocaleId)
    {
        string LocaleId1 = null;
        if (LocaleId.Contains('-'))
            LocaleId1 = LocaleId.Split('-')[0];
        
        List<ResxItem> localeRes = new List<ResxItem>();
        List<ResxItem> locale1Res = new List<ResxItem>();
        List<ResxItem> invariantRes = null;

        if (!string.IsNullOrEmpty(LocaleId))
        {
            localeRes = this.GetResXResources(baseFile + "." + LocaleId + ".resx");
            if (localeRes == null)
                localeRes = new List<ResxItem>();
        }
        if (!string.IsNullOrEmpty(LocaleId1))
        {
            locale1Res = this.GetResXResources(baseFile + "." + LocaleId1 + ".resx");
            if (locale1Res == null)
                locale1Res = new List<ResxItem>();
        }

        invariantRes = this.GetResXResources(baseFile + ".resx");        
        if (invariantRes == null)
            invariantRes = new List<ResxItem>();

        IEnumerable<ResxItem> items =
                from loc in localeRes
                        .Concat(from loc1 in locale1Res select loc1)
                        .Concat(from invariant in invariantRes select invariant)
                        .OrderBy(loc => loc.Name)
                select loc;

        Dictionary<string, object> resxDict = new Dictionary<string, object>();
        string lastName = "@#XX";
        foreach (ResxItem item in items)
        {
            if (lastName == item.Name)
                continue;
            lastName = item.Name;
            
            resxDict.Add(item.Name, item.Value);
        }
        
        return resxDict;
    }


        public const string ResXDocumentTemplate =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
</root>";

    }

    [DebuggerDisplay(@" Name: {Name}, Value={Value} ")]
    public class ResxItem
    {
        public string Name = string.Empty;
        public object Value = null;
        public string Type = string.Empty;
        public string LocaleId = string.Empty;
    }

    public enum wwResourceExportLanguages 
    {
        CSharp, VB
    }
}