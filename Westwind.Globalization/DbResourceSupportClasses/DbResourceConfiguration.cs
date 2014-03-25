using System;
using System.Web.UI.Design;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;

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
    public class DbResourceConfiguration
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
        /// Static constructor for the Current property - guarantees this
        /// code fires exactly once giving us a singleton instance
        /// of the configuration object.
        /// </summary>
        static DbResourceConfiguration()
        {
            Current = new DbResourceConfiguration(true);
        }

        /// <summary>
        /// Database connection string to the resource data.
        /// 
        /// The string can either be a full connection string or an entry in the 
        /// ConnectionStrings section of web.config.
        /// <seealso>Class DbResource
        /// Compiling Your Applications with the Provider</seealso>
        /// </summary>
        public string ConnectionString
        {
            get 
            {
                // If no = in the string assume it's a ConnectionStrings entry instead
                if (!_ConnectionString.Contains("="))
                {
                    try
                    {
                        return ConfigurationManager.ConnectionStrings[_ConnectionString].ConnectionString;
                    }
                    catch { }
                }
                return _ConnectionString; 
            }
            set { _ConnectionString = value; }
        }
        private string _ConnectionString = "";

        /// <summary>
        /// Database table name used in the database
        /// </summary>
        public string ResourceTableName
        {
            get { return _ResourceTableName; }
            set { _ResourceTableName = value; }
        }
        private string _ResourceTableName = "Localizations";

        /// <summary>
        /// The virtual path for the Web application. This value is used at design time.
        /// </summary>
        public string DesignTimeVirtualPath
        {
            get { return _DesignTimeVirtualPath; }
            set { _DesignTimeVirtualPath = value; }
        }
        private string _DesignTimeVirtualPath = "";

        /// <summary>
        /// Determines whether the DbResourceControl shows its localization options on the
        /// page. 
        /// </summary>
        public bool ShowLocalizationControlOptions
        {
            get { return _ShowLocalizationOptions; }
            set { _ShowLocalizationOptions = value; }
        }
        private bool _ShowLocalizationOptions = false;

        /// <summary>
        /// Determines whether page controls show icons when a 
        /// DbResourceControl is active. Note requires that ShowLocalizationControlOptions
        /// is true as well.
        /// </summary>
        public bool ShowControlIcons
        {
            get { return _ShowControlIcons; }
            set { _ShowControlIcons = value; }
        }
        private bool _ShowControlIcons = false;



        /// <summary>
        /// Determines the location of the Localization form in a Web relative path.
        /// This form is popped up when clicking on Edit Resources in the 
        /// DbResourceControl
        /// </summary>        
        public string LocalizationFormWebPath
        {
            get { return _LocalizationFormWebPath; }
            set { _LocalizationFormWebPath = value; }
        }
        private string _LocalizationFormWebPath = "~/LocalizationAdmin/LocalizationAdmin.aspx";

        /// <summary>
        /// Determines whether any resources that are not found are automatically
        /// added to the resource file.
        /// 
        /// Note only applies to the Invariant culture.
        /// </summary>
        public bool AddMissingResources
        {
            get { return _AddMissingResources; }
            set { _AddMissingResources = value; }
        }
        private bool _AddMissingResources = true;

        /// <summary>
        /// API key for Bing Translate API in the 
        /// Administration API.
        /// </summary>
        public string BingClientId { get; set; }

        /// <summary>
        /// Bing Secret Key for Bing Translate API Access
        /// </summary>
        public string BingClientSecret { get; set; }


        /// <summary>
        /// Determines whether generated Resource names use the same syntax
        /// as VS.Net uses. Defaults to false, which uses simple control
        /// name syntax (no ResourceX value) if possible. The dfeault value
        /// is shown without a number and numbers are only used on duplication.
        /// </summary>
        public bool UseVsNetResourceNaming
        {
            get { return _UseVsNetResourceNaming; }
            set { _UseVsNetResourceNaming = value; }
        }
        private bool _UseVsNetResourceNaming = false;


        /// <summary>
        /// Path and Name space of an optionally generated strongly typed resource
        /// which is created when exporting to ResX resources.
        /// 
        /// Leave this value blank if you don't want a strongly typed resource class
        /// generated for you.
        /// 
        /// Otherwise format is: (File Path,Namespace)
        /// ~/App_Code/Resources.cs,AppResources
        /// </summary>
        public string StronglyTypedGlobalResource
        {
            get { return _StronglyTypedGlobalResource; }
            set { _StronglyTypedGlobalResource = value; }
        }
        private string _StronglyTypedGlobalResource = "~/App_Code/Resources.cs,AppResources";


        /// <summary>
        /// Determines how what type of project we are working with
        /// </summary>
        public GlobalizationProjectTypes ProjectType
        {
            get { return _ProjectType; }
            set { _ProjectType = value; }
        }

        private GlobalizationProjectTypes _ProjectType = GlobalizationProjectTypes.WebForms;
 


        /// <summary>
        /// Base constructor that doesn't do anything to the default values.
        /// </summary>
        public DbResourceConfiguration()
        {
        }

        /// <summary>
        /// Default constructor used to read the configuration section to retrieve its values
        /// on startup.
        /// </summary>
        /// <param name="readConfigurationSection"></param>
        public DbResourceConfiguration(bool readConfigurationSection)
        {
            if (readConfigurationSection)
                this.ReadConfigurationSection();
        }


        /// <summary>
        /// Reads the DbResourceProvider Configuration Section and assigns the values 
        /// to the properties of this class
        /// </summary>
        /// <returns></returns>
        public bool ReadConfigurationSection()
        {
            object TSection = null;
            TSection = WebConfigurationManager.GetWebApplicationSection("DbResourceProvider");
            if (TSection == null)
                return false;

            var section = TSection as DbResourceProviderSection;
            ReadSectionValues(section);

            return true;
        }

        /// <summary>
        /// Handle design time access to the configuration settings - used for the 
        /// DbDesignTimeResourceProvider - when loaded we re-read the settings
        /// </summary>
        /// <param name="serviceHost"></param>
        public bool ReadDesignTimeConfiguration(IServiceProvider serviceProvider )
        {
            IWebApplication webApp = serviceProvider.GetService(typeof(IWebApplication)) as IWebApplication;

            // Can't get an application instance - can only exit
            if (webApp == null)
                return false;

            object TSection = webApp.OpenWebConfiguration(true).GetSection("DbResourceProvider");
            if (TSection == null)
                return false;

            var section = TSection as DbResourceProviderSection;
            ReadSectionValues(section);

            // If the connection string doesn't contain = then it's
            // a ConnectionString key from .config. This is handled in
            // in the propertyGet of the resource configration, but it uses
            // ConfigurationManager which is not available at design time
            //  So we have to duplicate the code here using the WebConfiguration.
            if (!this.ConnectionString.Contains("="))
            {
                try
                {
                    string conn = webApp.OpenWebConfiguration(true).ConnectionStrings.ConnectionStrings[this.ConnectionString].ConnectionString;
                    ConnectionString = conn;
                }
                catch { }                
            }
                
            return true;
        }

        /// <summary>
        /// Reads the actual section values
        /// </summary>
        /// <param name="section"></param>
        private void ReadSectionValues(DbResourceProviderSection section)
        {
            ConnectionString = section.ConnectionString;
            ResourceTableName = section.ResourceTableName;
            DesignTimeVirtualPath = section.DesignTimeVirtualPath;
            LocalizationFormWebPath = section.LocalizationFormWebPath;
            ShowLocalizationControlOptions = section.ShowLocalizationControlOptions;
            ShowControlIcons = section.ShowControlIcons;
            AddMissingResources = section.AddMissingResources;
            StronglyTypedGlobalResource = section.StronglyTypedGlobalResource;
            ProjectType = section.ProjectType;
            BingClientId = section.BingClientId;
            BingClientSecret = section.BingClientSecret;
        }



#region Allow for provider unloading

        /// <summary>
        /// Keep track of loaded providers so we can unload them
        /// </summary>
        internal static List<IWestWindResourceProvider> LoadedProviders = new List<IWestWindResourceProvider>();

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
#endregion

    }

    public enum GlobalizationProjectTypes 
    {        
        /// <summary>
        /// WebForms project that use App_LocalResource/App_GlobalResources
        /// </summary>
        WebForms,

        /// <summary>
        /// Any .NET project other than WebForms that 
        /// uses normal directory based resources and 
        /// not App_LocalResources/AppGlobalResources
        /// </summary>
        Class

    }

    public enum CodeGenerationLanguage
    {
        CSharp,
        Vb
    }
}
