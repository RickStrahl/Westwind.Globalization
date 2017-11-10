#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 2009-2015
 *          http://www.west-wind.com/
 * 
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Westwind.Globalization
{

    /// <summary>
    /// Returns a resource item that contains both Value and Comment
    /// </summary>
    [DebuggerDisplay("Lang: {Type} - {ResourceId} - {Value} - {ResourceSet}")]
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
        /// Determines what type of value the Value field represents
        /// Mainly used to differentiate between text and Markdown text
        /// </summary> 
        public int ValueType
        {
	        get { return _ValueType ; }
            set
            {
                SendPropertyChanged("ValueType");
                _ValueType = value;
            }
        }
        private int _ValueType = (int) ValueTypes.Text;
        

        public DateTime Updated
        {
            get { return _Updated; }
            set
            {
                _Updated = value;
                SendPropertyChanged("Updated");
            }
        }

        private DateTime _Updated = DateTime.UtcNow;


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
        private string _ResourceSet = string.Empty;


        public string TextFile { get; set; }
        public byte[] BinFile { get; set; }
        public string FileName { get; set; }


        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// initializes the resource item properties from
        /// the active data reader item.
        /// </summary>
        /// <param name="reader"></param>
        public void FromDataReader(IDataReader reader)
        {
            ResourceId = reader["ResourceId"] as string;
            Value = reader["Value"];
            ResourceSet = reader["ResourceSet"] as string;
            LocaleId = reader["LocaleId"] as string;
            Type = reader["Type"] as string;
            FileName = reader["Filename"] as string;
            TextFile = reader["TextFile"] as string;
            BinFile = reader["BinFile"] as byte[];
            Comment = reader["Comment"] as string;
            ValueType = Convert.ToInt32(reader["ValueType"]);
            try
            {
                Updated = (DateTime) reader["Updated"];
            }
            catch { }
            
        }    
    }

    public enum ValueTypes
    {
        Text = 0,
        Binary = 1,
        MarkDown = 2
    }
}
