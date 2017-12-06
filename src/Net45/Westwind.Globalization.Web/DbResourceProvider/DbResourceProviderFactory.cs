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
using System.Collections.Generic;
using System.Collections;
using System.Resources;
using System.Web.Compilation;
using System.Globalization;
using Westwind.Utilities;

namespace Westwind.Globalization
{
    /// <summary>
    /// Provider Factory class that needs to be set in web.config in order for ASP.NET to instantiate
    /// this class for all resource related tasks.
    /// </summary>
   // [DesignTimeResourceProviderFactoryAttribute(typeof(DbDesignTimeResourceProviderFactory))]
    public class DbResourceProviderFactory : ResourceProviderFactory
    {
        /// <summary>
        /// Core Factory method that returns an instance of our DbResourceProvider 
        /// database Resource provider for Global Resources. This method gets
        /// passed simple a ResourceSet which is equivalent to a Resource file in
        /// Resx and here maps to the ResourceSet id in the database.
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public override IResourceProvider CreateGlobalResourceProvider(string classname)
        {
            return new DbResourceProvider(null, classname);
        }

        /// <summary>
        /// Creates a local resource provider for a given Page or Template Resource.
        /// 
        /// We'll create local resources by application relative names. This routine
        /// gets passed a full virtual path to the page or template control and we'll
        /// strip off the virtual and use only the virtual relative path. 
        /// 
        /// So: /myapp/test.aspx becomes test.aspx and
        ///     /myapp/subdir/test.aspx becomes subdir/test.aspx
        /// 
        /// for our ResourceSet naming of local resources. The provider is 
        /// created with this ResourceSet name.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            // Strip out the virtual path leaving us just with page
            string ResourceSetName = WebUtils.GetAppRelativePath(virtualPath);

            // Create Provider with the ResourceSetname
            return new DbResourceProvider(ResourceSetName.ToLower(), ResourceSetName.ToLower());
        }


    }
}
