using System;
using System.Web;
using System.Resources;
using Westwind.Globalization;

namespace Westwind.Globalization.Sample.AspNetCore.Properties
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "2.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AlbumViewerResources
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
                    var temp = new ResourceManager("Westwind.Globalization.Sample.AspNetCore.Properties.AlbumViewerResources", typeof(AlbumViewerResources).Assembly);
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

		public static System.String AlbumDescription
		{
			get
			{
				return ResourceManager.GetString("AlbumDescription", resourceCulture);
			}
		}

		public static System.String Buy
		{
			get
			{
				return ResourceManager.GetString("Buy", resourceCulture);
			}
		}

		public static System.String AlbumDescription_Placeholder
		{
			get
			{
				return ResourceManager.GetString("AlbumDescription.Placeholder", resourceCulture);
			}
		}

		public static System.String Albums
		{
			get
			{
				return ResourceManager.GetString("Albums", resourceCulture);
			}
		}

		public static System.String ArtistName
		{
			get
			{
				return ResourceManager.GetString("ArtistName", resourceCulture);
			}
		}

		public static System.String Artists
		{
			get
			{
				return ResourceManager.GetString("Artists", resourceCulture);
			}
		}

		public static System.String Show
		{
			get
			{
				return ResourceManager.GetString("Show", resourceCulture);
			}
		}

		public static System.String By
		{
			get
			{
				return ResourceManager.GetString("By", resourceCulture);
			}
		}

		public static System.String MoreFrom
		{
			get
			{
				return ResourceManager.GetString("MoreFrom", resourceCulture);
			}
		}

		public static System.String Edit
		{
			get
			{
				return ResourceManager.GetString("Edit", resourceCulture);
			}
		}

		public static System.String AlbumViewer
		{
			get
			{
				return ResourceManager.GetString("AlbumViewer", resourceCulture);
			}
		}

		public static System.String YearReleased
		{
			get
			{
				return ResourceManager.GetString("YearReleased", resourceCulture);
			}
		}

		public static System.String AddSong
		{
			get
			{
				return ResourceManager.GetString("AddSong", resourceCulture);
			}
		}

		public static System.String AddAlbum
		{
			get
			{
				return ResourceManager.GetString("AddAlbum", resourceCulture);
			}
		}

		public static System.String EditResources
		{
			get
			{
				return ResourceManager.GetString("EditResources", resourceCulture);
			}
		}

		public static System.String Test
		{
			get
			{
				return ResourceManager.GetString("Test", resourceCulture);
			}
		}

		public static System.String NewResource
		{
			get
			{
				return ResourceManager.GetString("NewResource", resourceCulture);
			}
		}

		public static System.String AlbumName
		{
			get
			{
				return ResourceManager.GetString("AlbumName", resourceCulture);
			}
		}

		public static System.String Remove
		{
			get
			{
				return ResourceManager.GetString("Remove", resourceCulture);
			}
		}

	}

}
