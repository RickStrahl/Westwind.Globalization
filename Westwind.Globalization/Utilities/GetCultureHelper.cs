using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Westwind.Globalization.Utilities
{
    public static class GetCultureHelper
    {
        public static CultureInfo GetCurrentThreadUICulture()
        {
            return Thread.CurrentThread.CurrentUICulture;
        }
        public static CultureInfo GetCurrentCultureInfoUICulture()
        {
            return CultureInfo.CurrentUICulture;
        }
    }
}
