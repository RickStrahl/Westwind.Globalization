using System;
using System.Web;
using System.Resources;
using Westwind.Globalization;

namespace Westwind.Globalization.Sample.AspNetCore.Properties
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "2.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources
    {    
        /// <summary>
        /// ResourceManager instance used to retrieve resources in Resx mode.
        /// You can replace this resource manager with your own 
        /// but it applies only in Resx mode.
        /// </summary>
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var temp = new ResourceManager("Westwind.Globalization.Sample.AspNetCore.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan ;

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        private static System.Globalization.CultureInfo resourceCulture;

		public static System.String Today
		{
			get
			{
				return ResourceManager.GetString("Today", resourceCulture);
			}
		}

		public static System.String NameIsRequired
		{
			get
			{
				return ResourceManager.GetString("NameIsRequired", resourceCulture);
			}
		}

		public static System.String HelloWorld
		{
			get
			{
				return ResourceManager.GetString("HelloWorld", resourceCulture);
			}
		}

		public static System.String Yesterday
		{
			get
			{
				return ResourceManager.GetString("Yesterday", resourceCulture);
			}
		}

		public static System.String Testing
		{
			get
			{
				return ResourceManager.GetString("Testing", resourceCulture);
			}
		}

		public static System.String ErrorColon
		{
			get
			{
				return ResourceManager.GetString("ErrorColon", resourceCulture);
			}
		}

		public static System.String AddressIsRequired
		{
			get
			{
				return ResourceManager.GetString("AddressIsRequired", resourceCulture);
			}
		}

	}

}
