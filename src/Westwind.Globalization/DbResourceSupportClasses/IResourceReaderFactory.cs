using System.Globalization;
using System.Resources;

namespace Westwind.Globalization
{
	/// <summary>
	/// Factory that creates instances of <see cref="IResourceReader"/> for a specific <see cref="ResourceSet"/>
	/// and <see cref="CultureInfo">culture</see>.
	/// </summary>
	public interface IResourceReaderFactory
    {
		/// <summary>
		/// Create a <see cref="IResourceReader"/> instance for the given <see cref="CultureInfo"/>.
		/// </summary>
		/// <param name="resourceSet">ResourceSet name to create the reader for.</param>
		/// <param name="culture">Culture for which the resource reader should be created.</param>
		/// <param name="config"><see cref="DbResourceConfiguration"/> to further configure the reader.</param>
		/// <returns>Instance of an IResourceReader</returns>
		IResourceReader Create(string resourceSet, CultureInfo culture, DbResourceConfiguration config);
	}
}
