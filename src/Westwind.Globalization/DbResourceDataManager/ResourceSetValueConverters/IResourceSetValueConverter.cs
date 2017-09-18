namespace Westwind.Globalization
{
    public interface IResourceSetValueConverter
    {
        /// <summary>
        /// The value type that you would like to act on.
        /// This value can be the standard value type defined
        /// in the ValueTypes enumeration or any other integer
        /// value that is stored in the ValueType property of
        /// database.
        /// </summary>
        int ValueType { get; set; }

        /// <summary>
        /// The method used to convert or transform the value
        /// that will be stored in the ResourceSet fed for resources.
        /// </summary>
        /// <param name="resourceValue">The actual value to convert</param>
        /// <param name="key">key of the value to convert</param>        
        /// <returns></returns>
        object Convert(object resourceValue, string key);
    }
}