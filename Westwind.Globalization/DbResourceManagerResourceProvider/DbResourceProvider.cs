/*
 **************************************************************
 * DbReourceManager Class
 **************************************************************
 *  Author: Rick Strahl 
 *          (c) West Wind Technologies
 *          http://www.west-wind.com/
 * 
 * Created: 10/10/2006
 * Updated: 3/10/2008 
 **************************************************************  
*/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Resources; 
using System.Web.Compilation;
using System.Globalization;

using Westwind.Utilities;

namespace Westwind.Globalization
{
    /// <summary>
    /// Resource Provider marker interface. Also provides for clearing resources.
    /// </summary>
    public interface IWestWindResourceProvider
    {
        /// <summary>
        /// Interface method used to force providers to register themselves
        /// with DbResourceConfiguration.
        /// </summary>
        void ClearResourceCache();
    }

    /// <summary>
    /// The DbResourceProvider class is an ASP.NET Resource Provider implementation
    /// that retrieves its resources from a database. It works in conjunction with a
    /// DbResourceManager object and so uses standard .NET Resource mechanisms to 
    /// retrieve its data. The provider should be fairly efficient and other than
    /// initial load time standard .NET resource caching is used to hold resource sets
    /// in memory.
    /// 
    /// The Resource Provider class provides the base interface for accessing resources.
    /// This provider interface handles loading resources, caching them (using standard
    /// Resource Manager functionality) and allowing access to resources via GetObject.
    /// 
    /// This provider supports global and local resources, explicit expressions
    /// as well as implicit expressions (IImplicitResourceProvider).
    /// 
    /// There's also a design time implementation to provide Generate LocalResources
    /// support from ASP.NET Web Form designer.
    /// </summary>
    public class DbResourceProvider : IResourceProvider, IImplicitResourceProvider , IWestWindResourceProvider
    {
        /// <summary>
        /// 
        /// </summary>
        string _className;

        object _SyncLock = new object();

        /// <summary>
        /// Flag that can be read to see if the resource provider is loaded
        /// </summary>
        public static bool ProviderLoaded = false;

        private DbResourceProvider()
        {             
            if (!ProviderLoaded)
                ProviderLoaded = true;
        }

        /// <summary>
        /// Default constructor - only captures the parameter values
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="classname"></param>
        public DbResourceProvider(string virtualPath, string classname)
        {
            if (!ProviderLoaded)
                ProviderLoaded = true;

            lock (_SyncLock)
            {
                //  _virtualPath = virtualPath;
                _className = classname;
                DbResourceConfiguration.LoadedProviders.Add(this);
            }
        }

        /// <summary>
        /// IResourceProvider interface - required to provide an instance to an
        /// ResourceManager object.
        /// 
        /// Note that the resource manager is not tied to a specific culture by
        /// default. The Provider uses the UiCulture without explicitly passing
        /// culture info.
        /// </summary>
        public DbResourceManager ResourceManager
        {
            get
            {
                if (this._ResourceManager == null)
                {
                    DbResourceManager manager = new DbResourceManager(this._className);
                    manager.IgnoreCase = true;
                    this._ResourceManager = manager;                    
                }
                return _ResourceManager;
            }
        }
        private DbResourceManager _ResourceManager = null;


        /// <summary>
        /// Releases all resources and forces resources to be reloaded
        /// from storage on the next GetResourceSet
        /// </summary>
        public void ClearResourceCache()
        {
            this.ResourceManager.ReleaseAllResources(); 
        }

        /// <summary>
        /// The main method to retrieve a specific resource key. The provider
        /// internally handles resource fallback based on the ResourceSet implementation.
        /// </summary>
        /// <param name="ResourceKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object IResourceProvider.GetObject(string ResourceKey, CultureInfo culture)
        {
            
            object value = this.ResourceManager.GetObject(ResourceKey, culture);

            // If the value is still null and we're at the invariant culture
            // let's add a marker that the value is missing
            // this also allows the pre-compiler to work and never return null
            if (value == null && (culture == null || culture == CultureInfo.InvariantCulture) )
            {
                // No entry there
                value =  "";

                if (DbResourceConfiguration.Current.AddMissingResources)
                {
                    // Add invariant resource
                    DbResourceDataManager Data = new DbResourceDataManager();
                    if (!Data.ResourceExists(ResourceKey,"",this._className))
                        Data.UpdateOrAdd(ResourceKey,"*** Missing","",this._className,null);
                }                
            }

            return value;
        }


        /// <summary>
        /// Required instance of the ResourceReader for this provider. Part of
        /// the IResourceProvider interface. The reader is responsible for feeding
        /// the Resource data from a ResourceSet. The interface basically walks
        /// an enumerable interface by ResourceId.
        /// </summary>
        public IResourceReader ResourceReader
        {
            get
            {
                if (this._ResourceReader == null)
                    this._ResourceReader = new DbResourceReader(this._className, CultureInfo.InvariantCulture);

                return this._ResourceReader;
            }
        }
        private DbResourceReader _ResourceReader = null;
       

        #region IImplicitResourceProvider Members and helpers


        /// <summary>
        /// Implicit ResourceKey GetMethod that is called off meta:Resource key values.
        /// Note that if a value is missing at compile time this method is never called
        /// at runtime as the key isn't added to the Implicit key dictionary
        /// </summary>
        /// <param name="implicitKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object IImplicitResourceProvider.GetObject(ImplicitResourceKey implicitKey, CultureInfo culture)
        {
            return this.ResourceManager.GetObject(ConstructFullKey(implicitKey), culture);
        }

        /// <summary>
        /// Routine that generates a full resource key string from
        /// an Implicit Resource Key value
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static string ConstructFullKey(ImplicitResourceKey entry)
        {
            string text = entry.KeyPrefix + "." + entry.Property;
            if (entry.Filter.Length > 0)
                text = entry.Filter + ":" + text;

            return text;
        }

        
        /// <summary>
        /// Retrieves all keys for from the resource store that match the given key prefix.
        /// The value here is generally a property name (or resourceId) and this routine
        /// retrieves all matching property values.
        /// 
        /// So, lnkSubmit as the prefix finds lnkSubmit.Text, lnkSubmit.ToolTip and
        /// returns both of those keys.
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        ICollection IImplicitResourceProvider.GetImplicitResourceKeys(string keyPrefix)
        {
            List<ImplicitResourceKey> keys = new List<ImplicitResourceKey>(); 

            foreach (DictionaryEntry dictentry in this.ResourceReader)
            { 
                string key = (string)dictentry.Key;

                if (key.StartsWith(keyPrefix + ".", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    string keyproperty = String.Empty;
                    if (key.Length > (keyPrefix.Length + 1)) 
                    { 
                        int pos = key.IndexOf('.');
                        if ((pos > 0) && (pos  == keyPrefix.Length))
                        {
                            keyproperty = key.Substring(pos + 1);
                            if (String.IsNullOrEmpty(keyproperty) == false)
                            {
                                ImplicitResourceKey implicitkey = new ImplicitResourceKey(String.Empty, keyPrefix, keyproperty);
                                keys.Add(implicitkey);
                            }
                        }
                    }
                } 
            }
            return keys;
        }  
        #endregion

    }

}
