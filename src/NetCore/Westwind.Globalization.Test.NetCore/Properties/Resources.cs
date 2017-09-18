using System;
using System.Resources;
using System.Web;
using Westwind.Globalization;

namespace Westwind.Globalization.Test
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.DbResourceManager;
    }

    public class Resources
	{
	    public static ResourceManager ResourceManager
	    {
	        get
	        {
	            if (object.ReferenceEquals(resourceMan, null))
	            {
	                var temp = new ResourceManager("Resources", typeof(Resources).Assembly);
	                resourceMan = temp;
	            }
	            return resourceMan;
	        }
	    }
	    private static ResourceManager resourceMan = null;


        public static System.String NameIsRequired
		{
			get
			{
			    return GeneratedResourceHelper.GetResourceString("Resources", "NameIsRequired", ResourceManager, GeneratedResourceSettings.ResourceAccessMode);
            }
		}

		public static System.String Today
		{
			get
			{
			    return GeneratedResourceHelper.GetResourceString("Resources", "Today", ResourceManager, GeneratedResourceSettings.ResourceAccessMode);
            }
		}

		public static System.String Yesterday
		{
			get
			{
			    return GeneratedResourceHelper.GetResourceString("Resources", "Yesterday", ResourceManager, GeneratedResourceSettings.ResourceAccessMode);
            }
		}

		public static System.String HelloWorld
		{
			get
			{
			    return GeneratedResourceHelper.GetResourceString("Resources", "HelloWorld", ResourceManager, GeneratedResourceSettings.ResourceAccessMode);
            }
		}

		public static System.String Ready
		{
			get
			{
			    return GeneratedResourceHelper.GetResourceString("Resources", "Ready", ResourceManager, GeneratedResourceSettings.ResourceAccessMode);
            }
		}

		public static System.String AddressIsRequired
		{
			get
			{
			    return GeneratedResourceHelper.GetResourceString("Resources", "Ready", ResourceManager, GeneratedResourceSettings.ResourceAccessMode);
            }
		}

	}

}
