/*
 **************************************************************
 * DbResourceManager Class
 **************************************************************
 *  Author: Rick Strahl 
 *          (c) West Wind Technologies
 *          http://www.west-wind.com/
 * 
 * Created: 10/10/2006
 * 
 * based in part on code provided in:
 * ----------------------------------
 * .NET Internationalization
 *      Addison Wesley Books
 *      by Guy Smith Ferrier 
 **************************************************************  
*/

using System;
using System.Resources;
using System.Globalization;
using System.Reflection;
using System.Collections.Generic;

namespace Westwind.Globalization
{
    /// <summary>
    /// This class provides a databased implementation of a ResourceManager.
    /// 
    /// A ResourceManager holds each of the InternalResourceSets for a given group
    /// of resources. In ResX files a group is a file group wiht the same name
    /// (ie. Resources.resx, Resources.en.resx, Resources.de.resx). In this
    /// database driven provider the group is determined by the ResourceSet
    /// and the LocaleId as stored in the database. This class is instantiated
    /// and gets passed both of these values for identity.
    /// 
    /// An application can have many ResourceManagers - one for each localized
    /// page and one for each global resource with each hold multiple resourcesets
    /// for each of the locale's that are part of that resourceSet.
    /// 
    /// This class implements only the GetInternalResourceSet method to
    /// provide the ResourceSet from a database. It also implements all the
    /// base class constructors and captures only the BaseName which 
    /// is the name of the ResourceSet (ie. a global or local resource group)
    /// 
    /// Dependencies:
    /// DbResourceDataManager for data access
    /// DbResourceConfiguration which holds and reads config settings
    /// 
    /// DbResourceSet
    /// DbResourceReader
    /// </summary>
    public class DbResourceManager : ResourceManager
    {
        /// <summary>
        /// Configuration used to access the Db Resources.
        /// If not set uses the static global configuration, otherwise you can
        /// pass in a customized configuration that is used by this provider
        /// </summary>
        public DbResourceConfiguration Configuration;

        /// <summary>
        /// Internally dictionary to keep the resourceSets cached. 
        /// One resourceSet per languageId.
        /// </summary>
        protected Dictionary<string, ResourceSet> InternalResourceSets;

        /// <summary>
        /// Internally keep track of the resource set name since BaseName 
        /// is read-only and we can't assign to it. 
        /// </summary>
        protected string ResourceSetName;

        // Duplicate the Resource Manager Constructors below
        // Key feature of these overrides is to set up the BaseName
        // which is the name of the resource set (either a local
        // or global resource. Each ResourceManager controls one set
        // of resources (global or local) and manages the ResourceSet
        // for each of cultures that are part of that ResourceSet

        /// <summary>
        /// Critical Section lock used for loading/adding resource sets
        /// </summary>
        private static readonly object SyncLock = new object();
        private static readonly object AddSyncLock = new object();

        /// <summary>
        /// If true causes any entries that aren't found to be added
        /// </summary>
        public bool AutoAddMissingEntries { get; set; }        
        
        public override Type  ResourceSetType => typeof(DbResourceSet);

        /// <summary> 
        /// Constructs a DbResourceManager object
        /// </summary>
        /// <param name="baseName">The qualified base name which the resources represent</param>
        public DbResourceManager(string baseName)
        {            
			Initialize(baseName, null);                        
		}
        
        /// <summary>
        /// Constructs a DbResourceManager object. Match base constructors.
        /// </summary>
        /// <param name="resourceType">The Type for which resources should be read/written</param>
		public DbResourceManager(Type resourceType)
		{
			Initialize(resourceType.Name, resourceType.Assembly);
		}

        /// <summary>
        /// Constructs a DbResourceManager object. 
        /// Match base constructors, but not actually used
        /// </summary>
        /// <param name="baseName">The qualified base name which the resources represent</param>
        /// <param name="assembly">Assembly that hosts the resources. Not used.</param>
		
        public DbResourceManager(string baseName, Assembly assembly) //: base(baseName, assembly)
        {
            Initialize( baseName,assembly);
        }

        /// <summary>
        /// Constructs a DbResourceManager object. Match base constructors.
        /// </summary>
        /// <param name="baseName">The qualified base name which the resources represent</param>
        /// <param name="assembly">Assembly that hosts the resources. Not used.</param>
		/// <param name="resourceType">Associated resource type. Not used.</param>
        public DbResourceManager(string baseName, Assembly assembly, Type resourceType)  
        {
            Initialize(baseName, assembly);
        }

        /// <summary>
        /// Core Configuration method that sets up the ResourceManager. For this 
        /// implementation we only need the baseName which is the ResourceSet id
        /// (ie. the local or global resource set name) and the assembly name is
        /// simply ignored.
        /// 
        /// This method essentially sets up the ResourceManager and holds all
        /// of the culture specific resource sets for a single ResourceSet. With
        /// ResX files each set is a file - in the database a ResourceSet is a group
        /// with the same ResourceSet Id.
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="assembly"></param>
        protected void Initialize(string baseName, Assembly assembly)
        {
            // default configuration is static but you can override the configuration explicitly
            Configuration = DbResourceConfiguration.Current;
            
            ResourceSetName = baseName;

            AutoAddMissingEntries = DbResourceConfiguration.Current.AddMissingResources;
            
            // InternalResourceSets contains a set of resources for each locale
            InternalResourceSets = new Dictionary<string, ResourceSet>();            
        }
                
        

        /// <summary>
        /// This is the only method that needs to be overridden as long as we
        /// provide implementations for the ResourceSet/ResourceReader/ResourceWriter
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="createIfNotExists"></param>
        /// <param name="tryParents"></param>
        /// <returns></returns>
        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            
            // retrieve cached instance - outside of lock for perf
            if (InternalResourceSets.ContainsKey(culture.Name))
                return InternalResourceSets[culture.Name];

            lock(SyncLock)
            {
                // have to check again to ensure still not existing
                if (InternalResourceSets.ContainsKey(culture.Name))
                    return InternalResourceSets[culture.Name];
            
                // Otherwise create a new instance, load it and return it
                DbResourceSet rs = new DbResourceSet(ResourceSetName, culture, Configuration);
                
                // Add the resource set to the cached set
                InternalResourceSets.Add(culture.Name, rs);
                
                // And return an instance
                return rs;
            }            
        }

        /// <summary>
        /// Clears all resource sets and forces reloading
        /// on next resource set retrieval. Effectively
        /// this refreshes resources if the source has
        /// changed. Required to see DB changes in the
        /// live UI.
        /// </summary>
        public override void ReleaseAllResources()
        {
            base.ReleaseAllResources();
            InternalResourceSets.Clear();
        }


        // GetObject implementations to retrieve values - not required but useful to see operation
        /// <summary>
        /// Core worker method on the manager that returns resource. This
        /// override returns the resource for the currently active UICulture
        /// for this manager/resource set.
        /// 
        /// If resource is not found it returns null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetObject(string name)
        {
            object value = base.GetObject(name);

            if (value == null && AutoAddMissingEntries)            
                AddMissingResource(name,name);
            
            return value;
        }

        /// <summary>
        /// Core worker method that returns a  resource value for a
        /// given culture from the this resourcemanager/resourceset.
        /// 
        /// If resource is not found it returns the null
        /// </summary>
        /// <param name="name"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object GetObject(string name, CultureInfo culture)
        {
            object value = base.GetObject(name, culture);

            if (value == null && AutoAddMissingEntries)
                AddMissingResource(name, name);

            return value;
        }

        /// <summary>
        /// Add a new resource to the base resource set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddMissingResource(string name, string value, CultureInfo culture = null)
        {
            var manager = DbResourceDataManager.CreateDbResourceDataManager();

            string cultureName = string.Empty;
            if (culture != null)
                cultureName = culture.IetfLanguageTag;

            lock (AddSyncLock)
            {
                // double check if culture neutral version exists
                string item = manager.GetResourceObject(name, ResourceSetName, cultureName) as string;
                if (item != null)
                    return;

                manager.AddResource(name, value, cultureName, ResourceSetName,null);
            }
        }

        
    } 

}
