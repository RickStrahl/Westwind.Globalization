using System;
using System.Web;
using Westwind.Globalization;

namespace AppResources
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.AspNetResourceProvider;
    }

	public class CommonWords2
	{
		public static System.String Cancel
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords2","Cancel");
				return DbRes.T("Cancel","CommonWords2");
			}
		}

		public static System.String NewResource
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords2","NewResource");
				return DbRes.T("NewResource","CommonWords2");
			}
		}

		public static System.String BrandNewResource
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords2","BrandNewResource");
				return DbRes.T("BrandNewResource","CommonWords2");
			}
		}

		public static System.String Edit
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords2","Edit");
				return DbRes.T("Edit","CommonWords2");
			}
		}

		public static System.String AnotherNewResource
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords2","AnotherNewResource");
				return DbRes.T("AnotherNewResource","CommonWords2");
			}
		}

		public static System.String Save
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords2","Save");
				return DbRes.T("Save","CommonWords2");
			}
		}

		public static System.String AnotherNewResourceas
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords2","AnotherNewResourceas");
				return DbRes.T("AnotherNewResourceas","CommonWords2");
			}
		}

	}

	public class CommonWords5
	{
		public static System.String asddAnotherNewResource
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords5","asddAnotherNewResource");
				return DbRes.T("asddAnotherNewResource","CommonWords5");
			}
		}

	}

	public class CommonWords522
	{
		public static System.String SasddAnotherNewResourceasdasd
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("CommonWords522","SasddAnotherNewResourceasdasd");
				return DbRes.T("SasddAnotherNewResourceasdasd","CommonWords522");
			}
		}

	}

	public class NewResourceSet1
	{
		public static System.String Bogus
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("NewResourceSet1","Bogus!");
				return DbRes.T("Bogus!","NewResourceSet1");
			}
		}

	}

	public class Resources
	{
		public static System.String NameIsRequired
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","NameIsRequired");
				return DbRes.T("NameIsRequired","Resources");
			}
		}

		public static System.String NewResource
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","NewResource");
				return DbRes.T("NewResource","Resources");
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

		public static System.String Yesterday
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Yesterday");
				return DbRes.T("Yesterday","Resources");
			}
		}

		public static System.String HelloWorld
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","HelloWorld");
				return DbRes.T("HelloWorld","Resources");
			}
		}

		public static System.String AnotherNewResource
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","AnotherNewResource");
				return DbRes.T("AnotherNewResource","Resources");
			}
		}

		public static System.String Ready2
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Ready2");
				return DbRes.T("Ready2","Resources");
			}
		}

		public static System.String AddressIsRequired
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","AddressIsRequired");
				return DbRes.T("AddressIsRequired","Resources");
			}
		}

	}

}
