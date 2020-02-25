#if NETCORE2
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Hosting
{

    /// <summary>
    /// Implementation of a IWebHostEnvironment using locally faked
    /// IWebHostEnvironment and IHostEnvironment interfaces.
    ///
    /// This allows .NET Core 2.x to use IWebHostEnvironment without
    /// conditional code.
    ///
    /// This class simply forwards IHostingEnvironment properties
    /// into this class' properties.
    /// </summary>
    public class LegacyHostEnvironment : IWebHostEnvironment
    {
        public LegacyHostEnvironment(IHostingEnvironment environment)

        {
            ApplicationName = environment.ApplicationName;
            ContentRootFileProvider = environment.ContentRootFileProvider;
            ContentRootPath = environment.ContentRootPath;
            EnvironmentName = environment.EnvironmentName;
            WebRootFileProvider = environment.WebRootFileProvider;
            WebRootPath = environment.WebRootPath;
        }

        /// <summary>
        /// Name of the application - typically the package name (namespace)
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Content file provider that handles retrieving files from the content root
        /// </summary>
        public IFileProvider ContentRootFileProvider { get; set; }

        /// <summary>
        /// Path to the content root - folder where binaries live. Install folder.
        /// </summary>
        public string ContentRootPath { get; set; }

        /// <summary>
        /// Environment name: Production, Development, Staging etc.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// File provider that retrieves files and folder information for the WebRoot path.
        /// </summary>
        public IFileProvider WebRootFileProvider { get; set; }

        /// <summary>
        /// Path to the Web root where Web Content lives. Typically the `wwwroot` folder, but it can be different if changed during startup.
        /// </summary>
        public string WebRootPath { get; set; }
    }
    
    /// <summary>
    /// Fake IWebHostEnvironment for .NET Core 2.x
    /// </summary>
    public interface IWebHostEnvironment : IHostEnvironment
    {
        /// <summary>
        /// Gets or sets an <see cref="T:Microsoft.Extensions.FileProviders.IFileProvider" /> pointing at <see cref="P:Microsoft.AspNetCore.Hosting.IWebHostEnvironment.WebRootPath" />.
        /// </summary>
        IFileProvider WebRootFileProvider { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the web-servable application content files.
        /// </summary>
        string WebRootPath { get; set; }
    }


    /// <summary>
    /// Fake IHostEnvironment for .NET Core 2.x
    /// </summary>
    public interface IHostEnvironment
    {
        /// <summary>
        /// Gets or sets the name of the application. This property is automatically set by the host to the assembly containing
        /// the application entry point.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="T:Microsoft.Extensions.FileProviders.IFileProvider" /> pointing at <see cref="P:Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
        /// </summary>
        IFileProvider ContentRootFileProvider { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the application content files.
        /// </summary>
        string ContentRootPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the environment. The host automatically sets this property to the value of the
        /// of the "environment" key as specified in configuration.
        /// </summary>
        string EnvironmentName { get; set; }
    }

}
#endif


