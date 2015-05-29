// this resource file exists only for sample purposes - so that the demo pages work
// you can safely remove these files
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
                    var temp = new ResourceManager("Westwind.Globalization.Sample.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String Cancel
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","Cancel",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String MarkdownText
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","MarkdownText",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Today
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","Today",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Testing
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","Testing",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String NameIsRequired
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","NameIsRequired",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Yesterday
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","Yesterday",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String Save
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","Save",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String HelloWorld
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","HelloWorld",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.Drawing.Bitmap FlagPng
		{
			get
			{
				return (System.Drawing.Bitmap) GeneratedResourceHelper.GetResourceObject("Resources","FlagPng",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

		public static System.String AddressIsRequired
		{
			get
			{
				return GeneratedResourceHelper.GetResourceString("Resources","AddressIsRequired",ResourceManager,GeneratedResourceSettings.ResourceAccessMode);
			}
		}

    }
}
