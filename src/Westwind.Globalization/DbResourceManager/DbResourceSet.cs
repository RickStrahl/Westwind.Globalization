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
using System.Resources;
using System.Globalization;

namespace Westwind.Globalization
{
    /// <summary>
    /// DbResourceSet is the ResourceSet implementation for the database driven
    /// Resource manager. A ResourceSet is a IEnumerable list of all resources
    /// in set for a given specific culture. Each culture has a separate resource
    /// set. The ResourceManager caches the InternalResourceSets and figures out how to
    /// return the resources from this ResourceSet using the IEnumerable interface.
    /// 
    /// The ResourceSet doesn't do any work - it serves merely as a coordinator with an
    /// enumeration interface that passes the data to the Resource Manager. The
    /// actual reading of resources is managed by the ResourceReader which eventually
    /// calls into the database to retrieve the resources for the ResourceSet.
    /// </summary>
    public class DbResourceSet : ResourceSet
    {        
        
        /// <summary>
        /// Core constructor. Gets passed a baseName (which is the ResourceSet Id - 
        /// either a local or global resource group) and a culture. 
        /// 
        /// This constructor basically creates a new ResourceReader and uses that
        /// reader's IEnumerable interface to provide access to the underlying
        /// resource data.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="culture"></param>
        /// <param name="configuration"></param>
        public DbResourceSet(string baseName, CultureInfo culture, DbResourceConfiguration configuration) 
            : base(new DbResourceReader(baseName, culture, configuration))
        {            
        }

        /// <summary>
        /// Marker method that provides the type used for the ResourceReader.
        /// Not used.
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultReader()
        {
            return typeof(DbResourceReader);
        }

        /// <summary>
        /// Marker method that provides the type used for a ResourceWriter.
        /// Not used.
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultWriter()
        {
            return typeof(DbResourceWriter);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
