using CommonMark;

namespace Westwind.Globalization
{
    public class MarkdownResourceSetValueConverter : IResourceSetValueConverter
    {
        /// <summary>
        /// The value type that you would like to act on.
        /// This value can be the standard value type defined
        /// in the ValueTypes enumeration or any other integer
        /// value that is stored in the ValueType property of
        /// database.
        /// </summary>
        public int ValueType { get; set; }

        public MarkdownResourceSetValueConverter()
        {
            ValueType = (int) ValueTypes.MarkDown;
        }

        /// <summary>
        /// The method used to convert or transform the value
        /// that will be stored in the ResourceSet fed for resources.
        /// </summary>
        /// <param name="resourceValue">The actual value to convert</param>
        /// <param name="key">key of the value to convert</param>        
        /// <returns></returns>
        public object Convert(object resourceValue, string key)
        {
            if (resourceValue != null)
                resourceValue = ConvertMarkDown(resourceValue as string);

            return resourceValue;
        }

        string ConvertMarkDown(string resourceValue)
        {
            string html = CommonMarkConverter.Convert(resourceValue);
            return html;
        }

    }   
}
