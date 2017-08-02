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

using System.Resources;
using System.Globalization;
using System.Collections;

namespace Westwind.Globalization
{
    /// <summary>
    /// DbResourceReader is an IResourceReader for reading resources from a database.
    /// The ResourceReader is the actual Resource component that accesses the underlying datasource
    /// to retrieve the resource data. 
    /// 
    /// This databased manager uses the DbResourceDataManager to query the database and retrieve
    /// a list of resources for a given baseName (ResourceSet) and Culture and returns that result
    /// as an IEnumerable list (via a HashTable). This process - other than the data access - results
    /// in the same structures as resources read from ResX files.
    /// </summary>
    public class DbResourceReader : IResourceReader
    {
        /// <summary>
        /// Name of the ResourceSet
        /// </summary>
        private string baseNameField;

        /// <summary>
        /// The culture that applies to to this reader
        /// </summary>
        private CultureInfo cultureInfo;

        /// <summary>
        /// Cached instance of items. The ResourceManager will often be called repeatedly
        /// for the same data so this caching avoids multiple repetitive calls to the
        /// database.
        /// </summary>
        IDictionary Items = null;

        /// <summary>
        /// Critcal section for loading resource items
        /// </summary>
        static object _SyncLock = new object();

        /// <summary>
        /// Core constructor for DbResourceReader. This ctor is passed the name of the
        /// ResourceSet and a culture that is to be loaded.
        /// </summary>
        /// <param name="baseNameField">The base name of the resource reader</param>
        /// <param name="cultureInfo">The CultureInfo identifying the culture of the resources to be read</param>
        public DbResourceReader(string baseNameField, CultureInfo cultureInfo)
        {
            this.baseNameField = baseNameField;
            this.cultureInfo = cultureInfo;
        }

        /// <summary>
        /// This is the worker method responsible for actually retrieving resources from the resource
        /// store. This method goes out queries the database by asking for a specific ResourceSet and 
        /// Culture and it returns a Hashtable (as IEnumerable) to use as a ResourceSet.
        /// 
        /// The ResourceSet manages access to resources via IEnumerable access which is ultimately used
        /// to return resources to the front end.
        /// 
        /// Resources are read once and cached into an internal Items field. A ResourceReader instance
        /// is specific to a ResourceSet and Culture combination so there should never be a need to
        /// reload this data, except when explicitly clearing the reader/resourceset (in which case
        /// Items can be set to null via ClearResources()).
        /// </summary>
        /// <returns>An IDictionaryEnumerator of the resources for this reader</returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            if (Items != null)
                return Items.GetEnumerator();

            lock (_SyncLock)
            {
                // Check again to ensure we still don't have items
                if (Items != null)
                    return Items.GetEnumerator();

                // DEPENDENCY HERE
                // Here's the only place we really access the database and return
                // a specific ResourceSet for a given ResourceSet Id and Culture
                DbResourceDataManager manager = DbResourceDataManager.CreateDbResourceDataManager();
                // check if default project is set then access the data from project's/client's  specific resources
                if (!string.IsNullOrEmpty(DbResourceConfiguration.Current.DefaultProjectName))
                {
                    Items = manager.GetResourceSet(cultureInfo.Name, baseNameField, DbResourceConfiguration.Current.DefaultProjectName);
                }

                if (Items.Count == 0)
                {
                    // populate the resources from global or default data means project/client's value is null or blank in the database
                    Items = manager.GetResourceSet(cultureInfo.Name, baseNameField);
                }
                return Items.GetEnumerator();
            }
        }


        /// <summary>
        /// Returns an IEnumerator of the resources for this reader. Simply returns 
        /// the IDictionary enumerator from the overload.
        /// </summary>
        /// <returns>An IEnumerator of the resources for this reader</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Closes the resource reader after releasing any resources associated with it
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the object. Ultimately this is called
        /// by ResourceManager.ReleaseAllResources which calls on the ResourceSet
        /// and then down into the reader to close its resources. 
        /// 
        /// This code cleans up the internally created dictionary which in turn
        /// comes from a Hashtable.
        /// </summary>
        public void Dispose()
        {
            // Clear the Resource Items
            Items = null;
        }

    }
}
