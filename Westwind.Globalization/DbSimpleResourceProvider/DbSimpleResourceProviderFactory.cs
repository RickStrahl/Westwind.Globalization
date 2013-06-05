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
    [DesignTimeResourceProviderFactoryAttribute(typeof(DbDesignTimeResourceProviderFactory))]
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
