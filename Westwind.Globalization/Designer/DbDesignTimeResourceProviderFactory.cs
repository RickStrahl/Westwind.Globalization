/*
 **************************************************************
 * DbDesignTimeProvider Class
 **************************************************************
 *  Author: Rick Strahl 
 *          (c) West Wind Technologies
 *          http://www.west-wind.com/
 * 
 * Credits: based heavily on sample code from Eilon Lipton
 *          http://www.leftslipper.com/ShowFaq.aspx?FaqId=9
 *      
 *          Modified to use DbResourceDataManager and to
 *          read the full Web application path using the
 *          IWebApplication interface to provide full
 *          page pathing
 * 
 * Created:  10/10/2006
 **************************************************************  
*/
namespace Westwind.Globalization
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Resources;
    using System.Web.Compilation;
    using System.Web.UI;
    using System.Web.UI.Design;

    /// <remarks>
    /// Design-time resource provider to provide Generate Local Resources functionality.
    /// </remarks>
    public sealed class DbDesignTimeResourceProviderFactory : DesignTimeResourceProviderFactory
    {
        private IResourceProvider _localResourceProvider;
        
        public DbDesignTimeResourceProviderFactory()
        {
        }

        public override IResourceProvider CreateDesignTimeGlobalResourceProvider(IServiceProvider serviceProvider, string applicationKey)
        {
      
            return new DesignTimeGlobalResourceProvider(applicationKey);
        }

        public override IResourceProvider CreateDesignTimeLocalResourceProvider(IServiceProvider serviceProvider)
        {
            // Resource reader is cached for performance reasons, otherwise a new one
            // would be created for every property of every control that was localized.
            if (_localResourceProvider == null)
            {
                _localResourceProvider = new DbDesignTimeLocalResourceProvider(serviceProvider);
            }
            return _localResourceProvider;
        }

        public override IDesignTimeResourceWriter CreateDesignTimeLocalResourceWriter(IServiceProvider serviceProvider)
        {
            // Resource writer is never cached because it is generally called
            // only once, and should always use fresh resources anyway.
            return new DbDesignTimeLocalResourceProvider(serviceProvider);
        }


        /// <remarks>
        /// Design-time resource provider for global resources.
        /// </remarks>
        private sealed class DesignTimeGlobalResourceProvider : IResourceProvider
        {
            public DesignTimeGlobalResourceProvider(string applicationKey)
            {
            }

            object IResourceProvider.GetObject(string resourceKey, CultureInfo culture)
            {
                return null;
            }

            IResourceReader IResourceProvider.ResourceReader
            {
                get
                {
                    return null;
                }
            }
        }


        /// <remarks>
        /// Design-time resource provider and writer for local resources.
        /// </remarks>
        private sealed class DbDesignTimeLocalResourceProvider : IResourceProvider, IDesignTimeResourceWriter
        {
            private IServiceProvider _serviceProvider;
            private IResourceDictionary _localResources;
            private IDictionary _reader = null;


            public DbDesignTimeLocalResourceProvider(IServiceProvider serviceProvider)
            {
                // Have to force the configuration to read Web.Config settings explicitly at design time.
                if (string.IsNullOrEmpty(DbResourceConfiguration.Current.ConnectionString))
                    DbResourceConfiguration.Current.ReadDesignTimeConfiguration(serviceProvider);

                _serviceProvider = serviceProvider;
            }


            /// <summary>
            /// RAS Modified to read values from configiuration section
            /// </summary>
            /// <returns></returns>
            private string GetFullPagePath()
            {
                //// read the virtual base path - Configuration object reads from 
                //// custom web.config section in the designer
                //string BasePath = DbResourceConfiguration.Current.DesignTimeVirtualPath;
                //if (BasePath == null)
                //    BasePath = "";

                //if (BasePath.EndsWith("/"))
                //    BasePath.TrimEnd('/');

                IDesignerHost host = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
                WebFormsRootDesigner rootDesigner = host.GetDesigner(host.RootComponent) as WebFormsRootDesigner;

                return rootDesigner.DocumentUrl.TrimStart('~', '/'); // Replace("~", "");
            }

            private void Load()
            {
                // RAS Modified: Read the full page path ie. /internationalization/test.aspx  
                string ResourceSet = GetFullPagePath();
                
                // Load IDictionary data using the DataManager (same code as provider)
                DbResourceDataManager Manager = new DbResourceDataManager();                
                this._reader = Manager.GetResourceSet("", ResourceSet);
            }

            private void Flush()
            {
                if (LocalResources != null && LocalResources.Persistable)
                {
                    string ResourceSet = this.GetFullPagePath();

                    bool refreshLocalResource = false;

                    foreach (DictionaryEntry de in LocalResources)
                    {
                        AddResourceToStore((string)de.Key, de.Value, ResourceSet, _serviceProvider);
                        if (!_reader.Contains(de.Key))
                        {
                            _reader.Add(de.Key, de.Value);
                            refreshLocalResource = true;
                        }
                    }
                    if (refreshLocalResource && _localResources != null)
                    {
                        _localResources.Clear();
                        _localResources = null;
                    }
                }
            }

            private static void AddResourceToStore(string key, object value, string resourceSet, IServiceProvider serviceProvider)
            {
                // Use custom data manager to write the values into the database
                DbResourceDataManager Manager = new DbResourceDataManager();
                if (Manager.UpdateOrAdd(key, value, "", resourceSet, null) == -1)
                    throw new InvalidOperationException("Resource update error: " + Manager.ErrorMessage);
            }

            private IResourceDictionary LocalResources
            {
                get
                {
                    if (_localResources == null)
                    {
                        string resourceName = null; // application key
                        if (resourceName == null || resourceName.Length == 0)
                        {
                            Load();
                            _localResources = new ResourceDictionary(_reader);
                        }
                    }
                    return _localResources;
                }
            }

            private string CreateResourceKey(string resourceName, object obj)
            {
                if (resourceName == null || resourceName.Length == 0)
                {
                    return LocalResources.CreateResourceKey(obj);
                }
                return null;
            }

            object IResourceProvider.GetObject(string resourceKey, CultureInfo culture)
            {
                // this should never be called
                if (culture != CultureInfo.InvariantCulture)
                {
                    // TODO: String resource
                    throw new ArgumentException("Only the InvariantCulture is supported.", "culture");
                }
                if (LocalResources == null)
                {
                    return null;
                }
                return LocalResources[resourceKey];
            }

            IResourceReader IResourceProvider.ResourceReader
            {
                get
                {
                    if (LocalResources == null)
                    {
                        return null;
                    }
                    return new DictionaryResourceReader(LocalResources);
                }
            }

            string IDesignTimeResourceWriter.CreateResourceKey(string resourceName, object obj)
            {
                return CreateResourceKey(resourceName, obj);
            }

            void IResourceWriter.AddResource(string name, byte[] value)
            {
                if (LocalResources == null)
                {
                    return;
                }
                LocalResources[name] = value;
            }

            void IResourceWriter.AddResource(string name, object value)
            {
                if (LocalResources == null)
                {
                    return;
                }
                LocalResources[name] = value;
            }

            void IResourceWriter.AddResource(string name, string value)
            {
                if (LocalResources == null)
                {
                    return;
                }
                LocalResources[name] = value;
            }

            void IResourceWriter.Generate()
            {
                Flush();
            }

            void IResourceWriter.Close()
            {
            }

            void IDisposable.Dispose()
            {
            }
        }

        private sealed class DictionaryResourceReader : IResourceReader
        {
            private IDictionary _resources;

            public DictionaryResourceReader(IDictionary resources)
            {
                _resources = resources;
            }

            IDictionaryEnumerator IResourceReader.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }

            void IResourceReader.Close()
            {
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }

            void IDisposable.Dispose()
            {
            }
        }

        private interface IResourceDictionary : IDictionary
        {
            string CreateResourceKey(object obj);
            string CreateResourceKey(string key);
            bool Persistable { get; }
        }

        private sealed class ResourceDictionary : IResourceDictionary
        {
            private IDictionary _resources;
            private IDictionary _resourceKeys;
            private bool _persistable;


            public ResourceDictionary(IDictionary resources)
            {
                _resources = new Hashtable(resources);
                _resourceKeys = new HybridDictionary();
            }


            /// <summary>
            /// Creates a new ResourceKey value. By default uses the
            /// name of the control plus an incrementor.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            string IResourceDictionary.CreateResourceKey(object obj)
            {
                string ResourceKey = null;

                // RAS modified: Fixed so Control Id is used rather than type name (ie. lblMessage rather than Label1Resource1)
                Control Ctl = obj as Control;
                if (Ctl != null)
                    ResourceKey = Ctl.ID;

                if (string.IsNullOrEmpty(ResourceKey) )
                    ResourceKey = obj.GetType().Name;

                // Visual Studio adds Resource post-fix plus a number
                if (DbResourceConfiguration.Current.UseVsNetResourceNaming)
                    ResourceKey += "Resource";                

                return CreateKey(ResourceKey);
            }

            string IResourceDictionary.CreateResourceKey(string key)
            {
                return CreateKey(key);
            }

            bool IResourceDictionary.Persistable
            {
                get
                {
                    return _persistable;
                }
            }


            object IDictionary.this[object key]
            {
                get
                {
                    object val = null;
                    if (_resources != null && key != null)
                    {
                        val = _resources[key];
                    }
                    return val;
                }
                set
                {
                    if (_resources != null && key != null)
                    {
                        _persistable = true;
                        _resources[key] = value;
                    }
                }
            }

            ICollection IDictionary.Keys
            {
                get
                {
                    return _resources.Keys;
                }
            }

            ICollection IDictionary.Values
            {
                get
                {
                    return _resources.Values;
                }
            }

            bool IDictionary.IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            bool IDictionary.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            void IDictionary.Add(object key, object value)
            {
                ((IDictionary)this)[key] = value;
            }

            void IDictionary.Clear()
            {
                if (_resources != null)
                {
                    _resources.Clear();
                    _resources = null;
                }
            }

            bool IDictionary.Contains(object key)
            {
                return _resources.Contains(key);
            }

            void IDictionary.Remove(object key)
            {
                _resources.Remove(key);
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }


            void ICollection.CopyTo(Array array, int index)
            {
                _resources.CopyTo(array, index);
            }

            int ICollection.Count
            {
                get
                {
                    return _resources.Count;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _resources.GetEnumerator();
            }

            /// <summary>
            /// Creates a string name key for a given meta:Resource attribute.
            /// The key is the name of hte property with a 1,2,3 postfix if there's duplication
            /// 
            /// NOTE: This is slightly different than the default behavior which will always
            /// use a post fix value. This version only creates a postfix if there's dupes.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            private string CreateKey(string key)
            {
                int count = 1;
                
                string resourceKey = key + count;

                if (!DbResourceConfiguration.Current.UseVsNetResourceNaming)
                {
                    // Default with no numeric postfix first
                    resourceKey = key;
                }

                while (IsUsedKey(resourceKey))
                {
                    resourceKey = key + count;
                    count++;
                }

                _resourceKeys[resourceKey] = String.Empty;

                return resourceKey;
            }

            /// <summary>
            /// Checks to see if a resource key already exists
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            private bool IsUsedKey(string key)
            {
                bool used = false;
                used = _resourceKeys.Contains(key);
                if (!used)
                {
                    IEnumerator enumKeys = _resources.Keys.GetEnumerator();
                    while (enumKeys.MoveNext())
                    {
                        string reskey = (string)enumKeys.Current;
                        if (reskey != null)
                        {
                            int index = reskey.IndexOf(key);
                            if (index >= 0)
                            {
                                used = true;
                                break;
                            }
                        }
                    }
                }
                return used;
            }

        }
    }
}