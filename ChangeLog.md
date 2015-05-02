# West Wind Globalization Changelog

### Version 2.0 Beta6

* **Rework WebForms Resource Editing Links**<br/>
Updated the DbResourceControl to automatically generate resource editing icons for WebForms pages using the new client side resource icon logic. The control now generates icons from most Web Controls automatically. Removed logic for auto-embedding of scripts - Webforms now manaully have to add the styles and scripts just like any other HTML platform, but using the control provides the automatic linking without additional markup. 

* **Web Resource Editor pops up Add Resource Dialog on new Resource Queries**<br/>
This is a small but very productive enhancement - when you access the resource editor via page resource links and a resource doesn't exist, the Add Resource dialog now pops up with the resourceset/resourceid preset. This makes it quick and easy to add new resources to your resource file. Existing resources are still shown in the standard modeless interface as usual.

* **QueryString based Resource Display for Resource Editor**<br/>
You can now use query strings (ResourceSet and ResourceId) to 'jump' to a specific id in the resource editor. This allows direct linking to resources for editing or adding of new resources.

* **Add ValueType field to Database Tables**<br/>
Added a `int` `ValueType` field to the database and ResourceItem class that allows specifying what type of content is stored in the value field. Added to allow for value transformations. One transformation provided is MarkDown conversion.

> This is a breaking change if you have an existing localization resource table. You need to add a `ValueType` field of type `INTEGER` to any existing localization tables.

* **Add Support for ResourceSetValueConverters**<br/>
Add the ability to transform resource set values as they are assigned to a resource set. By creating a ResourceSetValueConverter you can essentially transform a value from one value to another. One example is a MarkDown converter which converts the translated text from markdown to plain HTML.

### [Version 2.0 Beta5](http://www.nuget.org/packages/Westwind.Globalization.Web/2.0.20-beta5)
<small><i>April 14th, 2015</i></small>

* **[Add support for Resource Editing to HTML/MVC Projects](https://github.com/RickStrahl/Westwind.Globalization/wiki/Interactive-Resource-Editing)**<br/>
You can now set up resource editing for plain HTML pages by adding resource attributes to DOM elements. Using `data-resource-id` at the element level and `data-resource-set` at the element or parent container level, elements are marked up with clickable links that jump to the resource editor for those items. 

* **Refactor Non Sql Server Providers into the Main Assembly**<br/>
Refactored `DbResourceDataManagers` so that no separate assemblies are required for other database providers any longer. All providers are now dynamically loaded so Westwind.Globalization doesn't have direct dependencies on any data providers. Dependencies have to be loaded explicitly via NuGet as described in the documentation.

* **Improved support for Resource File Uploads**<br/>
You can now more easily import and export file resoures from and to Resx files and also create new file resources for things like images and other non-plain string items. When exporting Resources are created in the same folder as the Resx files generated.

* **[Wiki Documentation](https://github.com/RickStrahl/Westwind.Globalization/wiki)**<br/>
Moved over legacy operational documentation to the GitHub Wiki for easier access and common interface on GitHub.  

### Version 2.0 Beta4
<small><i>March 27th, 2015</i></small>

* **New DbResourceDataManager Model to allow for pluggable Data Providers**<br/>
New DbResourceDataManager interface and adpater model for supporting multiple data providers. You can easily implement new DataManager implementations to handle the data access to read and write data into the provider for all supported features.

* **Add SqlCompact, MySql, SqlLite DbResourceManagers**<br/>
Added custom DbResourceDataManager implementations for each of these data engines and created separate assemblies for each of them to isolate dependencies. You can specify which DataManager is used via the Configuration.DataManagerType property.

* **New Administration Form**<br/>
Complete rewrote the Administration form using a pure client side model using Angular JS and a CallbackHandler service on the backend. The new UI provides much easier navigation of resources and shows all resources available for a resource key for easy editing. The editing UI is also keyboard friendly to allow for quick hands on the keyboard editing of resources.

* **Improved Resource Import and Export**<br/>
The Web Interface now supports importing and exporting resources optionally from virtually any path, not just the path of the actual Web application. This means you can now easily edit and update resources from any project on your local machine.

* **Strongly Typed Class Export Improvements**<br/>
The Web interface now allows you to choose the output path and namespace for strongly typed resource exports as part of the export process. The values default to configured values, but you can override the behavior.

* **Export JavaScript Resource files from Server Resources**
There's a new JavaScriptResources class that can export server resources as JavaScript files.

* **DbRes.THtml() Method**<br/>
Added DbRes.THtml() method in addition to DbRes.T() to allow for creating HTML pre-encoded content with less typing and function nesting (`@Html.Raw(DbRes.T(...))`) in Razor code.

* **DbRes.TFormat() Method**<br/>
Added DbRes.TFormat() to allow formatting of strings with fewer nested functions when using in Razor markup.


### Version 1.998
<small><i>January 26, 2015</i></small>

* **Add Basic Unit Tests for Testing Resource Managers and Providers**<br/>
Added high level integration tests for testing the DbResourceManager
and ASP.NET providers. Also added load tests for each of these to detect
possible ResourceSet locking conflicts.


* **Update Latest Dependencies**<br/>
Updated latest dependencies to current versions of West Wind tools
(Westwind.Utilities, Westwind.Web) as well as latest ASP.NET
dependencies for the samples. 

* **Fix: Add Processing Instruction for exported .resx Resources**<br/>
Added code to add missing processing instruction when exporting 
Db resources back to .resx resources in DbResXConverter.
Additionally the ability to specify a base path where resources
can be read from in addition to the default of the physical 
ASP.NET path.

### Version 1.997
<small><i>June 23, 2014</i></small>

* **Single Import and Export Mode**<br/>
Removed separation between WebForms and Project mode Resx Imports and
exports - there's now only a single mode that works for both. All
resources are now imported with a project relative path Id that
is also respected for exports. The provider now checks explicitly
for WebForms resources (.aspx/.ascx/.master/.sitemap) files and
explicitly exports them to App_LocalResources folders in their
respective directories. All other resources are automatically
exported to the relative folder specified in their resourceset id.

* **Removed Web Resources used for Admin Form**<br/>
Remove Web Resources used for the admin form and referenced
those resources directly with disk resources. This reduces
the assembly size and removes clutter from project.

* **Fix: Skip over Migrations folder .resx files**<br/>
Some customers have reported importing of .resx files from 
the 

* **Fix: Case strongly typed ResourceIds is preserved in Properties**<br/>
Previous versions generated the strongly typed property names using a
parsing algorithm. New code uses original resource ID names and only
fixes up Camel casing for spaces/dahses and symbols.

### Version 1.995
<small><i>April 15, 2014</i></small>

* **DbRes Provider for direct Resource Access**<br/>
Added a new DbRes resource provider that bypasses the traditional
.NET resource providers and goes directly to the database provider.
It uses the same in-memory structures for caching, but provides
a much simpler direct access mechanism to resources without
provider configuration. This provider also allows for using
resource values as keys, so if no matches are found the resource
values are displayed as is.

* **Synced to 2.x versions Westwind Libraries**<br/>
This version is synced to version 2.50 of Westwind.Utilities and
Westwind.Web/WebForms which are more modular and have smaller
footprint.

* **Fix bug in JavaScriptResourceHandler**<br/>
The JavaScriptResourceHandler allows use of .NET resources from
clientside JavaScript code, by mapping resources to JSON exported
resource objects that match the active locale (or explicitly). Bug
fixes scoping of the variable and now properly handles both DbRes
and standard Resx resources (or any other custom ResourceProvider).

* **Improved ASP.NET MVC Support**<br/>
DBRes.T() provides a simple way to embed resources into MVC
applications based on string values. It'll use the active Resource 
Provider and inject resources as strings. Think of it as a localization
helper that allows resource access. Alternately you can also just 
generate strongly typed resources and reference those.

* **Improved Strongly Typed Resource Generator**<br/>
Updated the strongly typed resource generator to allow to create 
resources that can switch between the ASP.NET ResourceProvider or
plain ResourceManager for retrieving resources. Allows generated
resources to be used in non-Web applications (services, console etc.)

### Version 1.990
<small><i>January 11, 2014</i></small>

* **Updated Admin UI**<br/>
Tweaked the Admin UI so it works better as a standalone html
form with its own HTML resources and dependencies. Added switch
to allow turning safemode on and off which prevents access to
the write functions that import/export resources or create
table.
