using System;
using System.Web;
using Westwind.Globalization;

namespace Westwind.Globalization.Sample
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.AspNetResourceProvider;
    }

	public class Commonwords
	{
		public static System.String Save
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Commonwords","Save");
				return DbRes.T("Save","Commonwords");
			}
		}

		public static System.String Cancel
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Commonwords","Cancel");
				return DbRes.T("Cancel","Commonwords");
			}
		}

		public static System.String Edit
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Commonwords","Edit");
				return DbRes.T("Edit","Commonwords");
			}
		}

	}

	public class Resources
	{
		public static System.String Yesterday
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Yesterday");
				return DbRes.T("Yesterday","Resources");
			}
		}

		public static System.String Ready
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Ready");
				return DbRes.T("Ready","Resources");
			}
		}

		public static System.String Today
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Today");
				return DbRes.T("Today","Resources");
			}
		}

		public static System.String Helloworld
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","HelloWorld");
				return DbRes.T("HelloWorld","Resources");
			}
		}

	}

}
