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
using System.Collections;
using System.Globalization;
using System.Resources;

namespace Westwind.Globalization
{
    // DbResourceWriter is not used, but fully implemented here
    // This interface is too limited to be really useful for a sophisticated
    // Resource Provider so the admin interfaces talk directly to the database
    // instead.
    //
    // The ResourceWriter would be more useful if it were directly tied to a
    // ResourceManager, but this is not the case.
    // Provided mainly for completeness here.


    /// <summary>
    /// DbResourceWriter is an IResourceWriter for writing resources to a database
    /// </summary>
    public class DbResourceWriter : IResourceWriter
    {
        private string baseName;
        private CultureInfo cultureInfo;

        /// <summary>
        /// List of resources we want to add
        /// </summary>
        private IDictionary resourceList = new Hashtable();

        /// <summary>
        /// Constructs a DbResourceWriter object
        /// </summary>
        /// <param name="baseNameField">The base name of the resource writer</param>
        /// <param name="cultureInfo">The CultureInfo identifying the culture of the resources to be written</param>
        public DbResourceWriter(string baseName, CultureInfo cultureInfo)
        {
            this.baseName = baseName;
            this.cultureInfo = cultureInfo;

            string CultureName = "";
            if (cultureInfo != null && !cultureInfo.IsNeutralCulture)
                CultureName = cultureInfo.Name;
        }

        /// <summary>
        /// Override that reads existing resources into the list
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="baseName"></param>
        /// <param name="cultureInfo"></param>
        public DbResourceWriter(IResourceReader reader, string baseName, CultureInfo cultureInfo)
        {
            this.baseName = baseName;
            this.cultureInfo = cultureInfo;

            resourceList = reader as IDictionary;
        }

        /// <summary>
        /// Closes the resource writer after releasing any resources associated with it
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }
        /// <summary>
        /// Releases all resources used by the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        /// Releases all resources used by the object
        /// </summary>
        /// <param name="disposing">True if the object is being disposed</param>
        private void Dispose(bool disposing)
        {
            if (disposing && resourceList != null)
                Generate();
        }

        /// <summary>
        /// Adds a resource to the list of resources to be written to an output file or output stream
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <param name="value">The value of the resource</param>
        public void AddResource(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(name);
            if (resourceList == null)
                throw new InvalidOperationException("No Resources");

            resourceList[name] = value;
        }
        /// <summary>
        /// Adds a resource to the list of resources to be written to an output file or output stream
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <param name="value">The value of the resource</param>
        public void AddResource(string name, string value)
        {
            AddResource(name, (Object)value);
        }
        /// <summary>
        /// Adds a resource to the list of resources to be written to an output file or output stream
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <param name="value">The value of the resource</param>
        public void AddResource(string name, byte[] value)
        {
            AddResource(name, (Object)value);
        }

        /// <summary>
        /// Writes all the resources added by the AddResource method to the output file or stream
        /// </summary>
        public void Generate()
        {
            Generate(false);
        }

        /// <summary>
        /// Writes all resources out to the resource store. Optional flag that
        /// allows deleting all resources for a given culture and basename first
        /// so that you get 'clean set' of resource with no orphaned values.
        /// </summary>
        /// <param name="DeleteAllRowsFirst"></param>
        public void Generate(bool DeleteAllRowsFirst)
        {
            // DEPENDENCY HERE
            var data = DbResourceDataManager.CreateDbResourceDataManager();               
            data.GenerateResources(resourceList, cultureInfo.Name, baseName, DeleteAllRowsFirst);
        }
    }
}
