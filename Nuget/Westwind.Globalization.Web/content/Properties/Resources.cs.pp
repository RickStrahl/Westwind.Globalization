using System;
using System.Web;
using System.Resources;
using Westwind.Globalization;

namespace $rootnamespace$.Properties
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.AspNetResourceProvider;
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "2.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources
    {
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var temp = new ResourceManager("Westwind.Globalization.Sample.LocalizationAdmin.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String Yesterday
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Yesterday");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Yesterday");

				return DbRes.T("Yesterday","Resources");
			}
		}

		public static System.String Today
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Today");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Today");

				return DbRes.T("Today","Resources");
			}
		}

		public static System.String NameIsRequired
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","NameIsRequired");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("NameIsRequired");

				return DbRes.T("NameIsRequired","Resources");
			}
		}

		public static System.String HelloWorld
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","HelloWorld");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("HelloWorld");

				return DbRes.T("HelloWorld","Resources");
			}
		}

		public static System.String AddressIsRequired
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","AddressIsRequired");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("AddressIsRequired");

				return DbRes.T("AddressIsRequired","Resources");
			}
		}

	}

}
