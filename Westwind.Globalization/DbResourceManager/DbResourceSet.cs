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
    /// The ResourceSet doesn't do any work - it serves merely as a coordinator. The
    /// actual reading of resources is managed by the ResourceReader which eventually
    /// calls into the database to retrieve the resources for the ResourceSet.
    /// </summary>
    public class DbResourceSet : ResourceSet
    {
        string _BaseName = null;
        CultureInfo _Culture = null;

        /// <summary>
        /// Core constructore. Gets passed a baseName (which is the ResourceSet Id - 
        /// either a local or global resource group) and a culture. 
        /// 
        /// This constructor basically creates a new ResourceReader and uses that
        /// reader's IEnumerable interface to provide access to the underlying
        /// resource data.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="culture"></param>
        public DbResourceSet(string baseName, CultureInfo culture)
            : base(new DbResourceReader(baseName, culture))
        {
            this._BaseName = baseName;
            this._Culture = culture;
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

#if false  // Factory methods for ResourceReader and ResourceWriter objects - not used
		        /// <summary>
		        /// Custom implementations that allow calling code to get an instance of 
		        /// ResourceReader.
		        /// </summary>
		        /// <returns>An IResourceReader to read the resources</returns>
		        public IResourceReader CreateDefaultReader()
		        {
		            return new DbResourceReader(this._BaseName,this._Culture);
		        }

		        /// <summary>
		        /// CreateDefaultWriter creates an IResourceWriter to write the resources
		        /// </summary>
		        /// <returns>An IResourceWriter to write the resources</returns>
		        public IResourceWriter CreateDefaultWriter()
		        {
		            return new DbResourceWriter(this._BaseName,this._Culture);
		        }
#endif
    }
}
