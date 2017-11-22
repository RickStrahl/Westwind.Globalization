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


#define IncludeWebFormsControls

using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Westwind.Utilities;
using Westwind.Utilities.Configuration;

namespace Westwind.Globalization
{
    /// <summary>
    /// The configuration class that is used to configure the Resource Provider.
    /// This class contains various configuration settings that the provider requires
    /// to operate both at design time and runtime.
    /// 
    /// The application uses the static Current property to access the actual
    /// configuration settings object. By default it reads the configuration settings
    /// from web.config (at runtime). You can override this behavior by creating your
    /// own configuration object and assigning it to the DbResourceConfiguration.Current property.
    /// </summary>
    public class DbResourceConfiguration : AppConfiguration
    {
        /// <summary>
        /// A global instance of the current configuration. By default this instance reads its
        /// configuration values from web.config at runtime, but it can be overridden to
        /// assign specific values or completely replace this object. 
        /// 
        /// NOTE: Any assignment made to this property should be made at Application_Start
        /// or other 'application initialization' event so that these settings are applied
        /// BEFORE the resource provider is used for the first time.
        /// </summary>
        public static DbResourceConfiguration Current = null;

        /// <summary>
        /// Determines how configuration information is stored: Config, Json or XML
        /// Default uses .NET configuration files.
        /// </summary>
#if NETFULL
        public static ConfigurationModes ConfigurationMode  = ConfigurationModes.ConfigFile;
#else
        public static ConfigurationModes ConfigurationMode = ConfigurationModes.JsonFile;
#endif

        /// <summary>
        /// Static constructor for the Current property - guarantees this
        /// code fires exactly once giving us a singleton instance
        /// of the configuration object.
        /// </summary>
        static DbResourceConfiguration()
        {
            Current = new DbResourceConfiguration();
            Current.Initialize(sectionName: "DbResourceConfiguration");
            Current.Read();
        }

        /// <summary>
        /// Database connection string to the resource data.
        /// 
        /// The string can either be a full connection string or an entry in the 
        /// ConnectionStrings section of web.config.
        /// <seealso>Class DbResource
        /// Compiling Your Applications with the Provider</seealso>
        /// </summary>
        public string ConnectionString { get; set; } = "*** ENTER A CONNECTION STRING OR connectionStrings ENTRY HERE ***";

        /// <summary>
        /// Determines which database provider is used. internally sets the DbResourceDataManagerType
        /// when set.
        /// </summary>
        public DbResourceProviderTypes DataProvider
        {
            get { return _dataAcessProviderType; }
            set
            {
                if (value == DbResourceProviderTypes.SqlServer)
                    DbResourceDataManagerType = typeof(DbResourceSqlServerDataManager);
                else if (value == DbResourceProviderTypes.MySql)
                    DbResourceDataManagerType = typeof(DbResourceMySqlDataManager);
                else if (value == DbResourceProviderTypes.SqLite)
                    DbResourceDataManagerType = typeof(DbResourceSqLiteDataManager);
                else if (value == DbResourceProviderTypes.SqlServerCompact)
                    DbResourceDataManagerType = typeof(DbResourceSqlServerCeDataManager);

                _dataAcessProviderType = value;
            }
        }
        private DbResourceProviderTypes _dataAcessProviderType = DbResourceProviderTypes.SqlServer;

        /// <summary>
        /// Database table name used in the database
        /// </summary>
        public string ResourceTableName { get; set; } = "Localizations";


        /// <summary>
        /// Name of a LocalizationConfiguration entry that is loaded from the database
        /// if available. Defaults to null - if set reads these configuration settings
        /// other than the database connection string from an entry in the 
        /// LocalizationConfigurations table.
        /// </summary>
        public string ActiveConfiguration { get; set; }

        /// <summary>
        /// Path of an optionally generated strongly typed resource
        /// which is created when exporting to ResX resources.
        /// 
        /// Leave this value blank if you don't want a strongly typed resource class
        /// generated for you.
        /// 
        /// Otherwise format is: 
        /// ~/App_Code/Resources.cs
        /// </summary>
        public string StronglyTypedGlobalResource { get; set; } = "~/Properties/Resources.cs";


        /// <summary>
        /// The namespace used for exporting and importing resources 
        /// </summary>
        public string ResourceBaseNamespace { get; set; } = "AppResources";


        /// <summary>
        /// Determines how what type of project we are working with
        /// </summary>
        public GlobalizationResxExportProjectTypes ResxExportProjectType { get; set; } = GlobalizationResxExportProjectTypes.Project;


        /// <summary>
        /// The base physical path used to read and write RESX resources for resource imports
        /// and exports. This path can either be a virtual path in Web apps or a physical disk
        /// path. Used only by the Web Admin form. All explicit API imports and exports are
        /// can pass in the base path explicitly.
        /// </summary>
        public string ResxBaseFolder { get; set; } = "~/Properties/";

        /// <summary>
        /// The ResourcePath used for IStringLocalizer as configured in .AddLocalization()
        /// defaults to Properties.
        /// </summary>
        public string StringLocalizerResourcePath
        {
            get { return StringUtils.ExtractString(ResxBaseFolder, "/", "/", allowMissingEndDelimiter: true); }
        }

        /// <summary>
        /// Determines whether any resources that are not found are automatically
        /// added to the resource file.
        /// 
        /// Note only applies to the Invariant culture.
        /// </summary>
        public bool AddMissingResources { get; set; } = true;


        /// <summary>
        /// Default mechanism used to access resources in DbRes.T().           
        /// This setting is global and used by all resources running through
        /// the DbResourceManage/Provider.
        /// 
        /// This doesn't not affect Generated REsources which have their own 
        /// ResourceAccesssMode override that can be explicitly overridden.    
        /// </summary>
        public ResourceAccessMode ResourceAccessMode { get; set; } = ResourceAccessMode.DbResourceManager;

    

        /// <summary>
        /// Determines the location of the Localization form in a Web relative path.
        /// This form is popped up when clicking on Edit Resources in the 
        /// DbResourceControl
        /// </summary>        
        public string LocalizationFormWebPath { get; set; } = "~/LocalizationAdmin/";


        /// <summary>
        /// API key for Bing Translate API in the 
        /// Administration API.
        /// https://www.microsoft.com/en-us/translator/getstarted.aspx
        /// </summary>
        public string BingClientId { get; set; }
        
        /// <summary>
        /// Google Translate API Key used to access Translate API.
        /// Note this is a for pay API!
        /// </summary>
        public string GoogleApiKey { get; set; }

     
        [JsonIgnore]
        public List<IResourceSetValueConverter> ResourceSetValueConverters = new List<IResourceSetValueConverter>();              

        /// <summary>
        /// Allows you to override the data provider used to access resources.
        /// Defaults to Sql Server. To override set this value during application
        /// startup - typical on DbResourceConfiguration.Current.DbResourceDataManagerType
        /// 
        /// This type instance is used to instantiate the actual provider.       
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [NonSerialized]
        public Type DbResourceDataManagerType = typeof(DbResourceSqlServerDataManager); 

        /// <summary>
        /// Internally used handler that is generically set to execute authorization
        /// when accessing the Localization handler
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [NonSerialized]
        public object OnAuthorizeLocalizationAdministration = null;
        
        /// <summary>
        /// Base constructor that doesn't do anything to the default values.
        /// </summary>
        public DbResourceConfiguration()
        {
            AddResourceSetValueConverter(new MarkdownResourceSetValueConverter());
        }

        public void AddResourceSetValueConverter(IResourceSetValueConverter converter)
        {
            ResourceSetValueConverters.Add(converter);
        }        

        /// <summary>
        /// Override this method to create the custom default provider. Here we allow for different 
        /// configuration providers so we don't have to rely on .NET configuration classed (for vNext)
        /// </summary>
        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            if (string.IsNullOrEmpty(sectionName))
                sectionName = "DbResourceConfiguration";

            IConfigurationProvider provider = null;

            if (ConfigurationMode == ConfigurationModes.JsonFile)
            {
                provider = new JsonFileConfigurationProvider<DbResourceConfiguration>()
                {
                    JsonConfigurationFile =
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbResourceConfiguration.json")
                };
            }
            else if (ConfigurationMode == ConfigurationModes.XmlFile)
            {
                provider = new XmlFileConfigurationProvider<DbResourceConfiguration>()
                {
                    XmlConfigurationFile =
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbResourceConfiguration.xml")
                };
            }
#if NETFULL
            else
            {
                provider = new ConfigurationFileConfigurationProvider<DbResourceConfiguration>()
                {
                    ConfigurationSection = sectionName
                };
            }
#else
            else 
            {
             provider = new JsonFileConfigurationProvider<DbResourceConfiguration>()
                {
                    JsonConfigurationFile =
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbResourceConfiguration.json")
                };
            }
#endif

            return provider;
        }

        ///// <summary>
        ///// Default constructor used to read the configuration section to retrieve its values
        ///// on startup.
        ///// </summary>
        ///// <param name="readConfigurationSection"></param>
        //public DbResourceConfiguration(bool readConfigurationSection)
        //{
        //    if (readConfigurationSection)
        //        ReadConfigurationSection();
        //}


        ///// <summary>
        ///// Reads the DbResourceProvider Configuration Section and assigns the values 
        ///// to the properties of this class
        ///// </summary>
        ///// <returns></returns>
        //public bool ReadConfigurationSection()
        //{            
        //    //TSection = WebConfigurationManager.GetWebApplicationSection("DbResourceProvider");
        //    object TSection = ConfigurationManager.GetSection("DbResourceProvider");
        //    if (TSection == null)
        //        return false;

        //    var section = TSection as DbResourceProviderSection;
        //    ReadSectionValues(section);

        //    return true;
        //}

        ///// <summary>
        ///// Handle design time access to the configuration settings - used for the 
        ///// DbDesignTimeResourceProvider - when loaded we re-read the settings
        ///// </summary>
        ///// <param name="serviceHost"></param>
        //public bool ReadDesignTimeConfiguration(IServiceProvider serviceProvider )
        //{
        //    IWebApplication webApp = serviceProvider.GetService(typeof(IWebApplication)) as IWebApplication;

        //    // Can't get an application instance - can only exit
        //    if (webApp == null)
        //        return false;

        //    object TSection = webApp.OpenWebConfiguration(true).GetSection("DbResourceProvider");
        //    if (TSection == null)
        //        return false;

        //    var section = TSection as DbResourceProviderSection;
        //    ReadSectionValues(section);

        //    // If the connection string doesn't contain = then it's
        //    // a ConnectionString key from .config. This is handled in
        //    // in the propertyGet of the resource configration, but it uses
        //    // ConfigurationManager which is not available at design time
        //    //  So we have to duplicate the code here using the WebConfiguration.
        //    if (!ConnectionString.Contains("="))
        //    {
        //        try
        //        {
        //            string conn = webApp.OpenWebConfiguration(true).ConnectionStrings.ConnectionStrings[ConnectionString].ConnectionString;
        //            ConnectionString = conn;
        //        }
        //        catch { }                
        //    }
                
        //    return true;
        //}

        ///// <summary>
        ///// Reads the actual section values
        ///// </summary>
        ///// <param name="section"></param>
        //private void ReadSectionValues(DbResourceProviderSection section)
        //{
        //    ConnectionString = section.ConnectionString;
        //    ResourceTableName = section.ResourceTableName;
        //    DesignTimeVirtualPath = section.DesignTimeVirtualPath;
        //    LocalizationFormWebPath = section.LocalizationFormWebPath;
        //    ShowLocalizationControlOptions = section.ShowLocalizationControlOptions;
        //    ShowControlIcons = section.ShowControlIcons;
        //    AddMissingResources = section.AddMissingResources;
        //    StronglyTypedGlobalResource = section.StronglyTypedGlobalResource;
        //    ResourceBaseNamespace = section.ResourceBaseNamespace;
        //    ResxExportProjectType = section.ResxExportProjectType;
        //    ResxBaseFolder = section.ResxBaseFolder;
        //    BingClientId = section.BingClientId;
        //    BingClientSecret = section.BingClientSecret;
        //}


        /// <summary>
        /// Creates an instance of the DbResourceDataManager based on configuration settings
        /// </summary>
        /// <returns></returns>
        public static DbResourceDataManager CreateDbResourceDataManager()
        {
            return ReflectionUtils.CreateInstanceFromType(Current.DbResourceDataManagerType) as
                DbResourceDataManager;
        }

        /// <summary>
        /// Keep track of loaded providers so we can unload them
        /// </summary>
        public static List<IWestWindResourceProvider> LoadedProviders = new List<IWestWindResourceProvider>();

        /// <summary>
        /// This static method clears all resources from the loaded Resource Providers 
        /// and forces them to be reloaded the next time they are requested.
        /// 
        /// Use this method after you've edited resources in the database and you want 
        /// to refresh the UI to show the newly changed values.
        /// 
        /// This method works by internally tracking all the loaded ResourceProvider 
        /// instances and calling the IwwResourceProvider.ClearResourceCache() method 
        /// on each of the provider instances. This method is called by the Resource 
        /// Administration form when you explicitly click the Reload Resources button.
        /// <seealso>Class DbResourceConfiguration</seealso>
        /// </summary>
        public static void ClearResourceCache()
        {
            foreach (IWestWindResourceProvider provider in LoadedProviders)
            {
                provider.ClearResourceCache();
            }

            // clear any resource managers
            DbRes.ClearResources();
        }


    }

    public enum ConfigurationModes
    {
        ConfigFile,
        JsonFile,
        XmlFile
    }

    /// <summary>
    /// Project types for Resx Exports. Either WebForms using 
    /// local and global resources files, or project
    /// </summary>
    public enum GlobalizationResxExportProjectTypes 
    {        
        /// <summary>
        /// WebForms project that use App_LocalResource/App_GlobalResources
        /// to store local and global resources
        /// </summary>
        WebForms,

        /// <summary>
        /// Any .NET project other than WebForms that 
        /// uses a single directory (Properties) for 
        ///  Resx resources
        /// </summary>
        Project

    }

    public enum CodeGenerationLanguage
    {
        CSharp,
        Vb
    }

    
}
