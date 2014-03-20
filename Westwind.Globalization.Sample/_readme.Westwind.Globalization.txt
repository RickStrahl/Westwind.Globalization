Westwind Globalization Configuration
------------------------------------
The NuGet package installs all the required assemblies and configures most 
web.config configuration settings.

You will need to edit web.config and the DbResourceProvider section to configure 
the connection string or connection string name:

  <DbResourceProvider 
    connectionString="*** PUT YOUR CONNECTION STRING OR ConnectionStrings KEY NAME HERE ***" 
    resourceTableName="Localizations" 
    projectType="WebForms" 
    designTimeVirtualPath="/internationalization" 
    showLocalizationControlOptions="true" 
    showControlIcons="true" 
    localizationFormWebPath="~/localizationadmin/LocalizationAdmin.aspx" 
    addMissingResources="false" 
    useVsNetResourceNaming="false" 
    stronglyTypedGlobalResource="~/Properties/Resources.cs,AppResources"  
   />

manually edit the connection string that points at the database that will contain the 
localization table. This table will be auto-created for you when the globalization resources 
are accessed or you are running the localization admin form. Please make sure that the database 
has appropriate rights to create a table.

Resource Provider is enabled by default
---------------------------------------
Once the NuGet package has been installed the provider is immediately enabled with:

<globalization resourceProviderFactoryType="Westwind.Globalization.DbSimpleResourceProviderFactory,Westwind.Globalization" />

If you are using Resx resources currently this will likely cause your resources to fail and if so you'll 
want to uncomment the line above in web.config and first import resources into the resource table. Once
imported you can then turn on the new resource provider.


Getting Started with Localization
---------------------------------
If you have no resources in your project:

* Navigate to ./LocalizationAdmin/LocalizationAdmin.aspx
* Use the Create Table button to create the resource for the database
* Import resources from project (this will import Resx resources for the Admin form)

To create new Resources:

Click on teh New icon next to the resource list list. This will let you add a new resource.
You can specify a resource set name (anything for a global resource file, or 
"Page.aspx" or "relativePath/Page.aspx" for local resources), a language ID  (en or en-us for example)
and a resource id. Then specify a value for the resource. 

When adding a new resource set or language you'll have to press the Refresh Page button to see the 
new resource set and language show up in the dialog. Once there you can use the buttons below the
Translated Text box to add new resources.

Note this resource editing scheme works for any resources and type of project including
MVC projects with raw RESX files.

Importing ASPX Resources using Visual Studio
--------------------------------------------
You can also import resources using the Visual Studio designer using the standard localization
tools - ASSUMING THE PROVIDER IS WORKING in your project. Use Tools | Generate Local Resources
to generate any local resources for localizable controls on a given ASPX page. When you do
resources will be imported into the database and you can use the Web resource editor to edit 
your resources interactively.

Importing exising ResX Resources
--------------------------------
If you have existing ResX resources you can import them from your project using the Import
button on the localization admin form. This should make all your existing resources available
in the resource editor.

If you choose at a later point you can also export your resources from the database back into
Resx resources.


