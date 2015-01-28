using System.Collections;

namespace Westwind.Globalization
{
    /// <summary>
    /// This interface is all that's required to use the Db resource provider
    /// or resource manager operation. It's a subset of the IDbResourceDataManager
    /// minimized just for ResourceSet operation.
    /// 
    /// All DataManagers implement this interface.
    /// </summary>
    public interface IDbResourceSetDataManager
    {
        /// <summary>
        /// Returns a specific set of resources for a given culture and 'resource set' which
        /// in this case is just the virtual directory and culture.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <param name="resourceSet"></param>
        /// <returns></returns>
        IDictionary GetResourceSet(string cultureName, string resourceSet);
    }
}