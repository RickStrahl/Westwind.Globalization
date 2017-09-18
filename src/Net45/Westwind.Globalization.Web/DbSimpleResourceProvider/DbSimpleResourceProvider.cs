#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2009-2012
 *          http://www.west-wind.com/
 * 
 * Created: 02/10/2009
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

/**************************************************************************************************************
 * This file contains a simplified ASP.NET Resource Provider that doesn't create a custom Resource Manager.
 * This implementation is much simpler than the full resource provider, but it's also not as integrated as
 * the full implementation. You can use this provider safely to serve resources, but for resource
 * editing and Visual Studio integration preferrably use the full provider.
 * 
 * This class shows how the Provider model works a little more clearly because this class is
 * self contained with the exception of the data access code and you can use this as a starting
 * point to build a custom provider. There are no ResourceReaders/Writers just a nested collection 
 * of resources.
 * 
 * This class uses DbResourceDataManager to retrieve and write resources in exactly two
 * places of the code. If you prefer you can replace these two locations with your own custom
 * Resource implementation. They are marked with:
 * 
 * // DEPENDENCY HERE
 * 
 * However, I would still recommend going with the full resource manager based implementation
 * because it works in any .NET application, not just ASP.NET. But a full resource manager
 * based implementation is much more complicated to create.
**************************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Compilation;
using System.Collections;
using System.Resources;
using System.Globalization;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace Westwind.Globalization
{

    /// <summary>
    /// Implementation of a very simple database Resource Provider. This implementation
    /// is self contained and doesn't use a custom ResourceManager. Instead it
    /// talks directly to the data resoure business layer (DbResourceDataManager).
    /// 
    /// Dependencies:
    /// DbResourceDataManager
    /// DbResourceConfiguration
    /// 
    /// You can replace those depencies (marked below in code) with your own data access
    /// management. The two dependcies manage all data access as well as configuration 
    /// management via web.config configuration section. It's easy to remove these
    /// and instead use custom data access code of your choice.
    /// </summary>
    [DebuggerDisplay("ResourceSet: {_ResourceSetName}")]
    public class DbSimpleResourceProvider : IResourceProvider, IWestWindResourceProvider //, IImplicitResourceProvider
    {
        /// <summary>
        /// Keep track of the 'className' passed by ASP.NET
        /// which is the ResourceSetId in the database.
        /// </summary>
        private string _ResourceSetName;

        /// <summary>
        /// Cache for each culture of this ResourceSet. Once
        /// loaded we just cache the resources.
        /// </summary>
        private IDictionary _resourceCache;


        public static bool ProviderLoaded = false;

        /// <summary>
        /// Critical section for loading Resource Cache safely
        /// </summary>
        private static object _SyncLock = new object();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath">The virtual path to the Web application</param>
        /// <param name="resourceSet">Name of the resource set to load</param>
        public DbSimpleResourceProvider(string virtualPath, string resourceSet)
        {
            lock (_SyncLock)
            {
                ProviderLoaded = true;
                _ResourceSetName = resourceSet;
                DbResourceConfiguration.LoadedProviders.Add(this);
            }              
        }

        /// <summary>
        /// Manages caching of the Resource Sets. Once loaded the values are loaded from the 
        /// cache only.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        private IDictionary GetResourceCache(string cultureName)
        {
            if (cultureName == null)
                cultureName = "";
             
            if (_resourceCache == null)
                _resourceCache = new ListDictionary();

            IDictionary resources = _resourceCache[cultureName] as IDictionary;
            if (resources == null)
            {
                // DEPENDENCY HERE (#1): Using DbResourceDataManager to retrieve resources

                // Use datamanager to retrieve the resource keys from the database
                var data = DbResourceDataManager.CreateDbResourceDataManager();                                 

                lock (_SyncLock)
                {
                    if (resources == null)
                    {
                        if (_resourceCache.Contains(cultureName))
                            resources = _resourceCache[cultureName] as IDictionary;
                        else
                        {
                            resources = data.GetResourceSet(cultureName as string, _ResourceSetName);
                            _resourceCache[cultureName] = resources;
                        }
                    }
                }
            }

            return resources;
        }

        /// <summary>
        /// Clears out the resource cache which forces all resources to be reloaded from
        /// the database.
        /// 
        /// This is never actually called as far as I can tell
        /// </summary>
        public void ClearResourceCache()
        {
            lock (_SyncLock)
            {
                _resourceCache.Clear();
            }
        }

        /// <summary>
        /// The main worker method that retrieves a resource key for a given culture
        /// from a ResourceSet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        object IResourceProvider.GetObject(string ResourceKey, CultureInfo Culture)
        {
            string cultureName;
            if (Culture != null)
                cultureName = Culture.Name;
            else
                cultureName = CultureInfo.CurrentUICulture.Name;

            return GetObjectInternal(ResourceKey, cultureName);
        }

        /// <summary>
        /// Internal lookup method that handles retrieving a resource
        /// by its resource id and culture. Realistically this method
        /// is always called with the culture being null or empty
        /// but the routine handles resource fallback in case the
        /// code is manually called.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        object GetObjectInternal(string resourceKey, string cultureName)
        {
            IDictionary resources = GetResourceCache(cultureName);

            object value = null;
            if (resources != null)
                value = resources[resourceKey];

            // If we're at a specific culture (en-Us) and there's no value fall back
            // to the generic culture (en)
            if (value == null && cultureName.Length > 3)
            {
                // try again with the 2 letter locale
                return GetObjectInternal(resourceKey, cultureName.Substring(0, 2));
            }

            // If the value is still null get the invariant value
            if (value == null)
            {
                resources = GetResourceCache("");
                if (resources != null)
                    value = resources[resourceKey];
            }

            // If the value is still null and we're at the invariant culture
            // let's add a marker that the value is missing
            // this also allows the pre-compiler to work and never return null
            if (value == null)
            {
                // No entry there
                value = resourceKey;

                // DEPENDENCY HERE (#2): using DbResourceConfiguration and DbResourceDataManager to optionally
                //                           add missing resource keys

                // Add a key in the repository at least for the Invariant culture
                // Something's referencing but nothing's there
                if (DbResourceConfiguration.Current.AddMissingResources)
                {
                    lock (_SyncLock)
                    {
                        if (resources[resourceKey] == null)
                        {
                            var data = DbResourceDataManager.CreateDbResourceDataManager();  
                            if (!data.ResourceExists(resourceKey,"",_ResourceSetName))
                                data.AddResource(resourceKey, resourceKey,"",
                                                 _ResourceSetName, null);

                            // add to current invariant resource set
                            resources.Add(resourceKey, resourceKey);
                        }
                    }
                }

            }

            return value;
        }

        /// <summary>
        /// The Resource Reader is used parse over the resource collection
        /// that the ResourceSet contains. It's basically an IEnumarable interface
        /// implementation and it's what's used to retrieve the actual keys
        /// </summary>
        public IResourceReader ResourceReader  // IResourceProvider.ResourceReader
        {
            get
            {
                if (_ResourceReader != null)
                    return _ResourceReader as IResourceReader;

                _ResourceReader = new DbSimpleResourceReader(GetResourceCache(null));
                return _ResourceReader as IResourceReader;
            }
        }
        private DbSimpleResourceReader _ResourceReader = null;



        // IImplict Resource Provider implementation is purely optional
        //     If not provided ASP.NET uses a default implementation.
#if false
        #region IImplicitResourceProvider Members

        /// <summary>
        /// Called when an ASP.NET Page is compiled asking for a collection
        /// of keys that match a given control name (keyPrefix). This
        /// routine for example returns control.Text,control.ToolTip from the
        /// Resource collection if they exist when a request for "control"
        /// is made as the key prefix.
        /// </summary>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public ICollection GetImplicitResourceKeys(string keyPrefix)
        {
            List<ImplicitResourceKey> keys = new List<ImplicitResourceKey>();

            IDictionaryEnumerator Enumerator = this.ResourceReader.GetEnumerator();
            if (Enumerator == null)
                return keys; // Cannot return null!

            foreach (DictionaryEntry dictentry in this.ResourceReader)
            {
                string key = (string)dictentry.Key;

                if (key.StartsWith(keyPrefix + ".", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    string keyproperty = String.Empty;
                    if (key.Length > (keyPrefix.Length + 1))
                    {
                        int pos = key.IndexOf('.');
                        if ((pos > 0) && (pos == keyPrefix.Length))
                        {
                            keyproperty = key.Substring(pos + 1);
                            if (String.IsNullOrEmpty(keyproperty) == false)
                            {
                                //Debug.WriteLine("Adding Implicit Key: " + keyPrefix + " - " + keyproperty);
                                ImplicitResourceKey implicitkey = new ImplicitResourceKey(String.Empty, keyPrefix, keyproperty);
                                keys.Add(implicitkey);
                            }
                        }
                    }
                }
            }
            return keys;
        }


        /// <summary>
        /// Returns an Implicit key value from the ResourceSet. 
        /// Note this method is called only if a ResourceKey was found in the
        /// ResourceSet at load time. If a resource cannot be located this
        /// method is never called to retrieve it. IOW, GetImplicitResourceKeys
        /// determines which keys are actually retrievable.
        /// 
        /// This method simply parses the Implicit key and then retrieves
        /// the value using standard GetObject logic for the ResourceID.
        /// </summary>
        /// <param name="implicitKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object GetObject(ImplicitResourceKey implicitKey, CultureInfo culture)
        {
            string ResourceKey = ConstructFullKey(implicitKey);

            string CultureName = null;
            if (culture != null)
                CultureName = culture.Name;
            else
                CultureName = CultureInfo.CurrentUICulture.Name;

            return this.GetObjectInternal(ResourceKey, CultureName);
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
            {
                text = entry.Filter + ":" + text;
            }
            return text;
        }

        #endregion
#endif

    }
}