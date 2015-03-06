using System;
using System.Web;
using Westwind.Globalization;

namespace Westwind.Globalization.Web
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.AspNetResourceProvider;
    }

	public class LocalizationForm
	{
		public static System.String LocalizationTableHasBeenCreated
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","LocalizationTableHasBeenCreated");
				return DbRes.T("LocalizationTableHasBeenCreated","LocalizationForm");
			}
		}

		public static System.String ResourceProviderInfo
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceProviderInfo");
				return DbRes.T("ResourceProviderInfo","LocalizationForm");
			}
		}

		public static System.String AddTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Add.Title");
				return DbRes.T("Add.Title","LocalizationForm");
			}
		}

		public static System.String AreYouSureYouWantToDoThis
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","AreYouSureYouWantToDoThis");
				return DbRes.T("AreYouSureYouWantToDoThis","LocalizationForm");
			}
		}

		public static System.String Delete
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Delete");
				return DbRes.T("Delete","LocalizationForm");
			}
		}

		public static System.String Comment
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Comment");
				return DbRes.T("Comment","LocalizationForm");
			}
		}

		public static System.String RenameTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Rename.Title");
				return DbRes.T("Rename.Title","LocalizationForm");
			}
		}

		public static System.String ExportResxTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ExportResx.Title");
				return DbRes.T("ExportResx.Title","LocalizationForm");
			}
		}

		public static System.String CreateClass
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateClass");
				return DbRes.T("CreateClass","LocalizationForm");
			}
		}

		public static System.String Rename
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Rename");
				return DbRes.T("Rename","LocalizationForm");
			}
		}

		public static System.String ImportResx
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ImportResx");
				return DbRes.T("ImportResx","LocalizationForm");
			}
		}

		public static System.String SearchResources
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","SearchResources");
				return DbRes.T("SearchResources","LocalizationForm");
			}
		}

		public static System.String YouAreAboutToDeleteThisResourceSet
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","YouAreAboutToDeleteThisResourceSet");
				return DbRes.T("YouAreAboutToDeleteThisResourceSet","LocalizationForm");
			}
		}

		public static System.String StronglyTypedClassCreated
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","StronglyTypedClassCreated");
				return DbRes.T("StronglyTypedClassCreated","LocalizationForm");
			}
		}

		public static System.String DeleteTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Delete.Title");
				return DbRes.T("Delete.Title","LocalizationForm");
			}
		}

		public static System.String DeleteResourceSetTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Delete.ResourceSet.Title");
				return DbRes.T("Delete.ResourceSet.Title","LocalizationForm");
			}
		}

		public static System.String ResourcesHaveBeenReloaded
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourcesHaveBeenReloaded");
				return DbRes.T("ResourcesHaveBeenReloaded","LocalizationForm");
			}
		}

		public static System.String ExportResx
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ExportResx");
				return DbRes.T("ExportResx","LocalizationForm");
			}
		}

		public static System.String AnotherLongTestMessage
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Another Long Test Message");
				return DbRes.T("Another Long Test Message","LocalizationForm");
			}
		}

		public static System.String ReloadResources
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ReloadResources");
				return DbRes.T("ReloadResources","LocalizationForm");
			}
		}

		public static System.String HelloWorld
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","HelloWorld");
				return DbRes.T("HelloWorld","LocalizationForm");
			}
		}

		public static System.String CreateClassTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateClass.Title");
				return DbRes.T("CreateClass.Title","LocalizationForm");
			}
		}

		public static System.String Cancel
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Cancel");
				return DbRes.T("Cancel","LocalizationForm");
			}
		}

		public static System.String HomeTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Home.Title");
				return DbRes.T("Home.Title","LocalizationForm");
			}
		}

		public static System.String ImportResxTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ImportResx.Title");
				return DbRes.T("ImportResx.Title","LocalizationForm");
			}
		}

		public static System.String Backup
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Backup");
				return DbRes.T("Backup","LocalizationForm");
			}
		}

		public static System.String ResourceSetRenamed
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceSetRenamed");
				return DbRes.T("ResourceSetRenamed","LocalizationForm");
			}
		}

		public static System.String BackupTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Backup.Title");
				return DbRes.T("Backup.Title","LocalizationForm");
			}
		}

		public static System.String ResourcesHaveBeenBackedUp
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourcesHaveBeenBackedUp");
				return DbRes.T("ResourcesHaveBeenBackedUp","LocalizationForm");
			}
		}

		public static System.String ReloadResourcesTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ReloadResources.Title");
				return DbRes.T("ReloadResources.Title","LocalizationForm");
			}
		}

		public static System.String LocalizationAdministration
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","LocalizationAdministration");
				return DbRes.T("LocalizationAdministration","LocalizationForm");
			}
		}

		public static System.String Add
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Add");
				return DbRes.T("Add","LocalizationForm");
			}
		}

		public static System.String ResourceSaved
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceSaved");
				return DbRes.T("ResourceSaved","LocalizationForm");
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

		public static System.String Today
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","Today");
				return DbRes.T("Today","Resources");
			}
		}

		public static System.String NameIsRequired
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Resources","NameIsRequired");
				return DbRes.T("NameIsRequired","Resources");
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
