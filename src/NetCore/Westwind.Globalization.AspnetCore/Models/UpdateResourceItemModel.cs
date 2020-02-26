namespace Westwind.Globalization.AspnetCore.Models
{
    public class UpdateResourceItemModel
    {
        public string Value { get; set; }
        public string ResourceId { get; set; }
        public string ResourceSet { get; set; }
        public string LocaleId { get; set; }
        public string Comment { get; set; }
        public int ValueType { get; set; }
        /// <summary>
        /// Type of the data if not a string
        /// </summary>
        public string Type { get; set; }

        public string TextFile { get; set; }
        public byte[] BinFile { get; set; }
        public string FileName { get; set; }
        public bool IsRtl { get; set; }
    }
}
