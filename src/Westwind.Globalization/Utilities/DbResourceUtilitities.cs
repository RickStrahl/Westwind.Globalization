using System;
using System.Collections.Generic;
using System.Text;

namespace Westwind.Globalization.Utilities
{
    public class DbResourceUtils
    {
        /// <summary>
        /// Normalizes a file path to the operating system default
        /// slashes.
        /// </summary>
        /// <param name="path"></param>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            char slash = System.IO.Path.DirectorySeparatorChar;
            path = path.Replace('/', slash).Replace('\\', slash);
            return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
        }
    }
}
