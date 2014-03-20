/*
 **************************************************************
 * DbReourceManager Class
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
 * 
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
        Dictionary<string, ResourceSet> InternalResourceSets;

        // Duplicate the Resource Manager Constructors below
        // Key feature of these overrides is to set up the BaseName
        // which is the name of the resource set (either a local
        // or global resource. Each ResourceManager controls one set
        // of resources (global or local) and manages the ResourceSet
        // for each of cultures that are part of that ResourceSet

        /// <summary>
        /// Critical Section lock used for loading/adding resource sets
        /// </summary>
        private static object SyncLock = new object();

        /// <summary>
        /// If true causes any entries that aren't found to be added
        /// </summary>
        public bool AutoAddMissingEntries { get; set; }

        /// <summary> 
        /// Constructs a DbResourceManager object
        /// </summary>
        /// <param name="baseName">The qualified base name which the resources represent</param>
        public DbResourceManager(string baseName)
		{
			Initialize(baseName, null);
		}

        public override Type  ResourceSetType
        {
        	get 
        	{ 
        		 return typeof(DbResourceSet);
        	}
        }

        /// <summary>
        /// Constructs a DbResourceManager object. Match base constructors.
        /// </summary>
        /// <param name="resourceType">The Type for which resources should be read/written</param>
		public DbResourceManager(Type resourceType)
		{
			Initialize(resourceType.Name, resourceType.Assembly);
		}

        public DbResourceManager(string baseName, Assembly assembly)
        {
            Initialize( baseName,null);
        }
        public DbResourceManager(string baseName, Assembly assembly, Type usingResourceSet)
        {
            Initialize(baseName, null);
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
            BaseNameField = baseName;
            
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
            var resourceSets = this.InternalResourceSets;

            // retrieve cached instance - outside of lock for perf
            if (resourceSets.ContainsKey(culture.Name))
                return resourceSets[culture.Name];

            lock(SyncLock)
            {
                // have to check again to ensure still not existing
                if (resourceSets.ContainsKey(culture.Name))
                    return resourceSets[culture.Name];
            
                // Otherwise create a new instance, load it and return it
                DbResourceSet rs = new DbResourceSet(BaseNameField, culture);                
                
                // Add the resource set to the cached set
                resourceSets.Add(culture.Name, rs);
                
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

            if (AutoAddMissingEntries && value == null)            
                AddMissingResource(name,name);
            
            return value;
        }

        /// <summary>
        /// Core worker method that returnsa  resource value for a
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

            if (AutoAddMissingEntries && value == null)
            {     
                AddMissingResource(name, name, null);
            }

            return value;
        }

        /// <summary>
        /// Add a new resource to the base resource set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddMissingResource(string name, string value, CultureInfo culture = null)
        {
            DbResourceDataManager man = new DbResourceDataManager();

            string cultureName = string.Empty;
            if (culture != null)
                cultureName = culture.IetfLanguageTag;

            // double check if culture neutral version exists
            string item = man.GetResourceObject(name, BaseName, cultureName) as string;
            if (item != null)
                return;
            
            man.AddResource(name, value, cultureName, BaseName, null);
        }

    } 

}
