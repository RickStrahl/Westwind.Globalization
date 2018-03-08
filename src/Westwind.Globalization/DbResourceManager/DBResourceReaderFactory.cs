using System.Globalization;
using System.Resources;

namespace Westwind.Globalization
{
	/// <summary>
	/// Creates instances of <see cref="DbResourceReader"/>s for a given <see cref="ResourceSet"/> and <see cref="CultureInfo"/>.
	/// </summary>
	public class DBResourceReaderFactory : IResourceReaderFactory
	{
		/// <inheritdoc />
		public IResourceReader Create(string resourceSet, CultureInfo culture, DbResourceConfiguration config)
		{
			return new DbResourceReader(resourceSet, culture, config);
		}
	}
}
