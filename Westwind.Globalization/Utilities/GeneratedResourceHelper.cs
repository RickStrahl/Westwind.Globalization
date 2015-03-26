using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Westwind.Globalization
{
    /// <summary>
    /// Class that returns resources 
    /// </summary>
    public static class GeneratedResourceHelper
    {
        public static object GetResourceObject(string resourceSet, string resourceId, 
            ResourceManager manager,
            ResourceAccessMode resourceMode)
        {
            if (resourceMode == ResourceAccessMode.AspNetResourceProvider)
                return GetAspNetResourceProviderValue(resourceSet, resourceId);
            if (resourceMode == ResourceAccessMode.Resx)
                return manager.GetObject(resourceId);
            return DbRes.T(resourceSet, "LocalizationForm");
        }

        public static string GetResourceString(string resourceSet, string resourceId, 
            ResourceManager manager,
            ResourceAccessMode resourceMode)
        {
            if (resourceMode == ResourceAccessMode.AspNetResourceProvider)
                return GetAspNetResourceProviderValue(resourceSet, resourceId) as string;
            if (resourceMode == ResourceAccessMode.Resx)
                return manager.GetString(resourceId);

            return DbRes.T(resourceSet, "LocalizationForm");
        }


        static object GetAspNetResourceProviderValue(string resourceSet, string resourceId)
        {
            return HttpContext.GetGlobalResourceObject(resourceSet, resourceId);
        }
    }
}
