using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Westwind.Globalization
{
    public static class DbResourceConfigurationExtensions
    {

        public static void ConfigureAuthorizeLocalizationAdministration(this DbResourceConfiguration config,
            Func<ControllerContext, bool> onAuthorizeLocalizationAdministration)
        {
            config.OnAuthorizeLocalizationAdministration = onAuthorizeLocalizationAdministration;
        }
    }
}