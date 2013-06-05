using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Compilation;
using System.Collections;
using System.Resources;
using System.Globalization;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Configuration;

namespace Westwind.Globalization
{
    /// <summary>
    /// Required simple IResourceReader implementation. A ResourceReader
    /// is little more than an Enumeration interface that allows 
    /// parsing through the Resources in a Resource Set which
    /// is passed in the constructor.
    /// </summary>
    public class DbSimpleResourceReader : IResourceReader
    {
        private IDictionary _resources;

        public DbSimpleResourceReader(IDictionary resources)
        {
            _resources = resources;
        }
        IDictionaryEnumerator IResourceReader.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }
        void IResourceReader.Close()
        {
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }
        void IDisposable.Dispose()
        {
        }
    }
}
