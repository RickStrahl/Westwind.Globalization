namespace Westwind.Globalization
{
    /// <summary>
    /// Resource Provider marker interface. Also provides for clearing resources.
    /// </summary>
    public interface IWestWindResourceProvider
    {
        /// <summary>
        /// Interface method used to force providers to register themselves
        /// with DbResourceConfiguration.
        /// </summary>
        void ClearResourceCache();
    }
}