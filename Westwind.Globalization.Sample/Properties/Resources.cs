using System;
using System.Web;
using System.Resources;
using Westwind.Globalization;

namespace Westwind.Globalization.Sample.Properties
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.AspNetResourceProvider;
    }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("Westwind.Globalization.StronglyTypedResources", "2.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class LocalizationForm
    {
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var temp = new ResourceManager("Westwind.Globalization.Sample.Properties.LocalizationForm", typeof(LocalizationForm).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        private static ResourceManager resourceMan = null;

		public static System.String LocalizationTableHasBeenCreated
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","LocalizationTableHasBeenCreated");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("LocalizationTableHasBeenCreated");

				return DbRes.T("LocalizationTableHasBeenCreated","LocalizationForm");
			}
		}

		public static System.String CreateClassInfo2
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateClassInfo2");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("CreateClassInfo2");

				return DbRes.T("CreateClassInfo2","LocalizationForm");
			}
		}

		public static System.String ResourceProviderInfo
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceProviderInfo");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourceProviderInfo");

				return DbRes.T("ResourceProviderInfo","LocalizationForm");
			}
		}

		public static System.String ResxResourcesHaveBeenImported
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxResourcesHaveBeenImported");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxResourcesHaveBeenImported");

				return DbRes.T("ResxResourcesHaveBeenImported","LocalizationForm");
			}
		}

		public static System.String AddTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Add.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Add.Title");

				return DbRes.T("Add.Title","LocalizationForm");
			}
		}

		public static System.String AreYouSureYouWantToDoThis
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","AreYouSureYouWantToDoThis");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("AreYouSureYouWantToDoThis");

				return DbRes.T("AreYouSureYouWantToDoThis","LocalizationForm");
			}
		}

		public static System.String ImportOrExportResxResources
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ImportOrExportResxResources");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ImportOrExportResxResources");

				return DbRes.T("ImportOrExportResxResources","LocalizationForm");
			}
		}

		public static System.String ResourceImportFailed
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceImportFailed");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourceImportFailed");

				return DbRes.T("ResourceImportFailed","LocalizationForm");
			}
		}

		public static System.String ResourceSetLoadingFailed
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceSetLoadingFailed");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourceSetLoadingFailed");

				return DbRes.T("ResourceSetLoadingFailed","LocalizationForm");
			}
		}

		public static System.String Delete
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Delete");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Delete");

				return DbRes.T("Delete","LocalizationForm");
			}
		}

		public static System.String Comment
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Comment");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Comment");

				return DbRes.T("Comment","LocalizationForm");
			}
		}

		public static System.String RenameTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Rename.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Rename.Title");

				return DbRes.T("Rename.Title","LocalizationForm");
			}
		}

		public static System.String ExportResxTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ExportResx.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ExportResx.Title");

				return DbRes.T("ExportResx.Title","LocalizationForm");
			}
		}

		public static System.String CreateClass
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateClass");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("CreateClass");

				return DbRes.T("CreateClass","LocalizationForm");
			}
		}

		public static System.String ResourceUpdateFailed
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceUpdateFailed");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourceUpdateFailed");

				return DbRes.T("ResourceUpdateFailed","LocalizationForm");
			}
		}

		public static System.String ResxResourcesHaveBeenCreated
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxResourcesHaveBeenCreated");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxResourcesHaveBeenCreated");

				return DbRes.T("ResxResourcesHaveBeenCreated","LocalizationForm");
			}
		}

		public static System.String Rename
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Rename");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Rename");

				return DbRes.T("Rename","LocalizationForm");
			}
		}

		public static System.String CreateClassInfo
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateClassInfo");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("CreateClassInfo");

				return DbRes.T("CreateClassInfo","LocalizationForm");
			}
		}

		public static System.String ResxExportInfoProject
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxExportInfo.Project");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxExportInfo.Project");

				return DbRes.T("ResxExportInfo.Project","LocalizationForm");
			}
		}

		public static System.String ImportResx
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ImportResx");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ImportResx");

				return DbRes.T("ImportResx","LocalizationForm");
			}
		}

		public static System.String SearchResources
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","SearchResources");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("SearchResources");

				return DbRes.T("SearchResources","LocalizationForm");
			}
		}

		public static System.String LocalizationTableNotCreated
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","LocalizationTableNotCreated");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("LocalizationTableNotCreated");

				return DbRes.T("LocalizationTableNotCreated","LocalizationForm");
			}
		}

		public static System.String Resources
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Resources");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Resources");

				return DbRes.T("Resources","LocalizationForm");
			}
		}

		public static System.String YouAreAboutToDeleteThisResourceSet
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","YouAreAboutToDeleteThisResourceSet");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("YouAreAboutToDeleteThisResourceSet");

				return DbRes.T("YouAreAboutToDeleteThisResourceSet","LocalizationForm");
			}
		}

		public static System.String HelloWorld
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","HelloWorld");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("HelloWorld");

				return DbRes.T("HelloWorld","LocalizationForm");
			}
		}

		public static System.String NoResourcePassedToAddOrUpdate
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","NoResourcePassedToAddOrUpdate");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("NoResourcePassedToAddOrUpdate");

				return DbRes.T("NoResourcePassedToAddOrUpdate","LocalizationForm");
			}
		}

		public static System.String StronglyTypedClassCreated
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","StronglyTypedClassCreated");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("StronglyTypedClassCreated");

				return DbRes.T("StronglyTypedClassCreated","LocalizationForm");
			}
		}

		public static System.String ReloadResources
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ReloadResources");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ReloadResources");

				return DbRes.T("ReloadResources","LocalizationForm");
			}
		}

		public static System.String DeleteTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Delete.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Delete.Title");

				return DbRes.T("Delete.Title","LocalizationForm");
			}
		}

		public static System.String DeleteResourceSetTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Delete.ResourceSet.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Delete.ResourceSet.Title");

				return DbRes.T("Delete.ResourceSet.Title","LocalizationForm");
			}
		}

		public static System.String ResourceSetRenamed
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceSetRenamed");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourceSetRenamed");

				return DbRes.T("ResourceSetRenamed","LocalizationForm");
			}
		}

		public static System.String CreateTable
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateTable");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("CreateTable");

				return DbRes.T("CreateTable","LocalizationForm");
			}
		}

		public static System.String ResxImportInfo
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxImportInfo");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxImportInfo");

				return DbRes.T("ResxImportInfo","LocalizationForm");
			}
		}

		public static System.String Filename
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Filename");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Filename");

				return DbRes.T("Filename","LocalizationForm");
			}
		}

		public static System.String NoProviderConfigured
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","NoProviderConfigured");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("NoProviderConfigured");

				return DbRes.T("NoProviderConfigured","LocalizationForm");
			}
		}

		public static System.String Export
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Export");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Export");

				return DbRes.T("Export","LocalizationForm");
			}
		}

		public static System.String ReloadResourcesTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ReloadResources.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ReloadResources.Title");

				return DbRes.T("ReloadResources.Title","LocalizationForm");
			}
		}

		public static System.String LocaleIdsFailedToLoad
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","LocaleIdsFailedToLoad");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("LocaleIdsFailedToLoad");

				return DbRes.T("LocaleIdsFailedToLoad","LocalizationForm");
			}
		}

		public static System.String CreateStronglyTypedClasses
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateStronglyTypedClasses");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("CreateStronglyTypedClasses");

				return DbRes.T("CreateStronglyTypedClasses","LocalizationForm");
			}
		}

		public static System.String ResourcesHaveBeenReloaded
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourcesHaveBeenReloaded");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourcesHaveBeenReloaded");

				return DbRes.T("ResourcesHaveBeenReloaded","LocalizationForm");
			}
		}

		public static System.String CreateTableTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateTable.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("CreateTable.Title");

				return DbRes.T("CreateTable.Title","LocalizationForm");
			}
		}

		public static System.String CreateClassTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","CreateClass.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("CreateClass.Title");

				return DbRes.T("CreateClass.Title","LocalizationForm");
			}
		}

		public static System.String Cancel
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Cancel");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Cancel");

				return DbRes.T("Cancel","LocalizationForm");
			}
		}

		public static System.String ResxImportInfoProject
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxImportInfo.Project");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxImportInfo.Project");

				return DbRes.T("ResxImportInfo.Project","LocalizationForm");
			}
		}

		public static System.String HomeTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Home.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Home.Title");

				return DbRes.T("Home.Title","LocalizationForm");
			}
		}

		public static System.String ResxExportInfo
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxExportInfo");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxExportInfo");

				return DbRes.T("ResxExportInfo","LocalizationForm");
			}
		}

		public static System.String ResourceGenerationFailed
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceGenerationFailed");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourceGenerationFailed");

				return DbRes.T("ResourceGenerationFailed","LocalizationForm");
			}
		}

		public static System.String ResxImportInfoWebForms
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxImportInfo.WebForms");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxImportInfo.WebForms");

				return DbRes.T("ResxImportInfo.WebForms","LocalizationForm");
			}
		}

		public static System.String ImportExportResxTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ImportExportResx.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ImportExportResx.Title");

				return DbRes.T("ImportExportResx.Title","LocalizationForm");
			}
		}

		public static System.String StronglyTypedGlobalResourcesFailed
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","StronglyTypedGlobalResourcesFailed");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("StronglyTypedGlobalResourcesFailed");

				return DbRes.T("StronglyTypedGlobalResourcesFailed","LocalizationForm");
			}
		}

		public static System.String Backup
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Backup");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Backup");

				return DbRes.T("Backup","LocalizationForm");
			}
		}

		public static System.String Folder
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Folder");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Folder");

				return DbRes.T("Folder","LocalizationForm");
			}
		}

		public static System.String BackupTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Backup.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Backup.Title");

				return DbRes.T("Backup.Title","LocalizationForm");
			}
		}

		public static System.String ImportExportResx
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ImportExportResx");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ImportExportResx");

				return DbRes.T("ImportExportResx","LocalizationForm");
			}
		}

		public static System.String ResourcesHaveBeenBackedUp
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourcesHaveBeenBackedUp");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourcesHaveBeenBackedUp");

				return DbRes.T("ResourcesHaveBeenBackedUp","LocalizationForm");
			}
		}

		public static System.String ImportResxTitle
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ImportResx.Title");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ImportResx.Title");

				return DbRes.T("ImportResx.Title","LocalizationForm");
			}
		}

		public static System.String InvalidResourceId
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","InvalidResourceId");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("InvalidResourceId");

				return DbRes.T("InvalidResourceId","LocalizationForm");
			}
		}

		public static System.String ResxExportInfoWebForms
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResxExportInfo.WebForms");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResxExportInfo.WebForms");

				return DbRes.T("ResxExportInfo.WebForms","LocalizationForm");
			}
		}

		public static System.String LocalizationAdministration
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","LocalizationAdministration");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("LocalizationAdministration");

				return DbRes.T("LocalizationAdministration","LocalizationForm");
			}
		}

		public static System.String Add
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","Add");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("Add");

				return DbRes.T("Add","LocalizationForm");
			}
		}

		public static System.String ResourceSaved
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("LocalizationForm","ResourceSaved");
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.Resx)
					return ResourceManager.GetString("ResourceSaved");

				return DbRes.T("ResourceSaved","LocalizationForm");
			}
		}

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
