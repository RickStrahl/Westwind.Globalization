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
		public static System.String Ready
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Commonwords","Ready");
				return DbRes.T("Ready","Commonwords");
			}
		}

		public static System.String ThisIsALongLineOfText
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Commonwords","This is a long line of text");
				return DbRes.T("This is a long line of text","Commonwords");
			}
		}

	}

	public class Resources
	{
		public static System.String Tomorrow
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Tomorrow");
				return DbRes.T("Tomorrow","Resources");
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

		public static System.String Today
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Today");
				return DbRes.T("Today","Resources");
			}
		}

		public static System.String VeryLongLineOfText
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Very long line of text");
				return DbRes.T("Very long line of text","Resources");
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

	public class Resourcetest
	{
		public static System.String LblhelloworldText
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resourcetest","lblHelloWorld.Text");
				return DbRes.T("lblHelloWorld.Text","Resourcetest");
			}
		}

	}

}
