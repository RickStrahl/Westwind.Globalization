using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Westwind.Globalization
{

    /// <summary>
    /// Returns a resource item that contains both Value and Comment
    /// </summary>
    public class ResourceItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The Id of the resource
        /// </summary>
        public string ResourceId
        {
            get { return _ResourceId; }
            set
            {
                _ResourceId = value;
                SendPropertyChanged("ResourceId");
            }
        }
        private string _ResourceId = null;

        /// <summary>
        /// The value of this resource
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                SendPropertyChanged("Value");
            }
        }
        private object _Value = null;


        /// <summary>
        /// The optional comment for this resource
        /// </summary>
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                SendPropertyChanged("Comment");
            }
        }
        private string _Comment = null;

        /// <summary>
        /// Type of the data if not a string
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The localeId ("" invariant or "en-US", "de" etc). Note
        /// Empty means invariant or default locale.
        /// </summary>
        public string LocaleId
        {
            get { return _LocaleId; }
            set
            {
                _LocaleId = value;
                SendPropertyChanged("LocaleId");
            }
        }
        private string _LocaleId = string.Empty;


        /// <summary>
        /// The resource set (file) that this resource belongs to
        /// </summary>
        public string ResourceSet
        {
            get { return _ResourceSet; }
            set
            {
                _ResourceSet = value;
                SendPropertyChanged("ResourceSet");
            }
        }

        public string TextFile { get; set; }
        public byte[] BinFile { get; set; }
        public string FileName { get; set; }


        private string _ResourceSet = string.Empty;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
