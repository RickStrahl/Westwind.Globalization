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

using System.Web.Compilation;
using Westwind.Utilities;

namespace Westwind.Globalization
{
    /// <summary>
    /// Provider factory that instantiates the individual provider. The provider
    /// passes a 'classname' which is the ResourceSet id or how a resource is identified.
    /// For global resources it's the name of hte resource file, for local resources
    /// it's the full Web relative virtual path
    /// </summary>
   // [DesignTimeResourceProviderFactoryAttribute(typeof(DbDesignTimeResourceProviderFactory))]
    public class DbSimpleResourceProviderFactory : ResourceProviderFactory
    {
        /// <summary>
        /// ASP.NET sets up provides the global resource name which is the 
        /// resource ResX file (without any extensions). This will become
        /// our ResourceSet id. ie. Resource.resx becomes "Resources"
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public override IResourceProvider CreateGlobalResourceProvider(string classname)
        {
            return new DbSimpleResourceProvider(null, classname);
        }

        /// <summary>
        /// ASP.NET passes the full page virtual path (/MyApp/subdir/test.aspx) wich is
        /// the effective ResourceSet id. We'll store only an application relative path
        /// (subdir/test.aspx) by stripping off the base path.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            // DEPENDENCY HERE: use Configuration class to strip off Virtual path leaving
            //                      just a page/control relative path for ResourceSet Ids

            // ASP.NET passes full virtual path: Strip out the virtual path 
            // leaving us just with app relative page/control path
            string ResourceSetName = WebUtils.GetAppRelativePath(virtualPath);
            
            DbSimpleResourceProvider provider = new DbSimpleResourceProvider(null, ResourceSetName.ToLower());
            
            return provider;
        }
    }
}
