# West Wind Globalization Changelog

### Version 3.0
<small>January 24th, 2018</small>

* **.NET Core and ASP.NET Core Support**  
Westwind.Globalization now works on .NET Core and ASP.NET Core. We now provide full framework and .NET Standard 2.0 assemblies for the core package and separate ASP.NET and ASP.NET Core Web packages. .NET Core support includes standard `ILocalizer` support, `IOptions` configuration support as well as full compatibility with v2.0 `dbRes` and strongly typed resource syntax.

* **Updated Localization Admin UI for ASP.NET Core**   
The Localization Admin application has been updated to work with a ASP.NET Core MVC controller on the backend. The ASP.NET 4.5+ continues to use `[CallbackHandler]` for compactness and doesn't have a dependency on MVC or WebAPI. The .NET Core version has a number of small UI updates.


* **DbResInstance Class**   
Previous versions only had a static `DbRes` class to access content directly off the resource manager/provider. There's now an instance class that has an associated configuration that is more easily testable and supports multiple configurations. It's also a better fit for ASP.NET Core and the dynamic configuration which allows for injection of DbResInstance classes.

* **dbRes.TDefault() to get resource with Default fallback**  
New method that allows passing of a default string if a resources ID can't be found. 

* **Easier saving of Resources without explicit save operations**  
Make Admin form resource entry easier by not requiring explicit saving of resources. Just tabbing across fields saves values now.

* **Add DeepL Translation Service**   
Added the [DeepL](https://www.deepl.com/translator) Translation service to the translation helpers.

* **DataProvider Configuration Switch**   
You can now set the database provider used as part of configuration using the `DataProvider` configuration option. Specify `SqlServer`, `SqLite`, `MySql` or `SqlCompact` (full framework only) as part of configuration. This setting can now be set via configuration switch rather than requiring explicitly setting the `DbResourceManagerType` property in code. The new setting simply delegates to `DbResourceManagerType` so you can still set that manually to change providers as well.

* **Fix: Bing Translation**   
Fixed Bing Translation for new Azure Translation APIs and using new logon mechanism. You'll need a new key when updating.



### Version 2.12
<small>May 19th, 2016</small>

* **Miscellaneous UI fixes**  
Translations are now immediately saved instead of just updating the field to avoid accidentally losing resource updates. The check for whether the resource table exists in the database now actually checks back in the db which allows doing imports when no reosurce sets are loaded. Fix various UI refresh problems. 

* **Fix Resource Import for stale Resources**  
When importing resources, the resource set imported to is cleared out before resources are imported to avoid stale resources. 

### Version 2.10
<small>March 15th, 2016</small>

* **Fix Google Translation**
Added support for official Google Translate API v2 which requires an API key and is a for pay API. Updated the 'free' Google API hack that works through the online translator API links which are not 'officially' supported by Google and may limit access at any time. If no API key is provided the free API is used, otherwise the full Translate API is used, using the API key provided in the .config file.

* **Fix Grid Record Limit**  
Fix internal filtering of the grid resource view in the localization editor.

* **Switch to FlexBox Layout for Localization Admin Form**  
The LocalizationAdmin form now uses FlexBox for a much cleaner, responsive and resizable panel and more reliable responsive layout of the main form.

### Version 2.2.1
<small>September 30th, 2015</small>

* **Add Comment Editing**  
Added support for editing and previewing comments on individual resources. Comment button brings up a modal dialog that allows editing of comments which are updated immediately. Comments can be previewed by hovering the button and the comment button uses different icons for filled and empty comments.

### Version 2.2
<small>September 24th, 2015</small>

* **Add Grid Editor for quick Editing of Resources**  
Added a grid view that allows editing of all resources in a resource set in a grid view that displays resources for all locales simultaneously. It's great way to quickly edit resources but may not work well for very large locales.

* **Add support for exporting specific ResourceSets to Resx**  
You can now choose to export either all ResourceSets (old behavior) or select one or more ResourceSets to export to Resx resources. This allows for much more flexibility in importing resources for editing from arbitrary projects on disk, then exporting them back to their original folders. Great for editing resources in Class Library projects.

* **Add support for generating Strongly Typed Classes for specific ResourceSets**  
You can now choose to export either all ResourceSets (old behavior) or select one or more ResourceSets when generating strongly typed classes. Like for ResourceSets this allows for strongly typed resources to be exported to arbitrary lcoatations and usage in all types of projects external to the Web site.

* **Add Support for generating standard Resx Designer files from ResourceSets**  
ALthough we already supported strongly typed resources with Resx through the dynamicly generated strongly typed resource export, you can now export classic .designer.cs/vb files. By doing so you can easily work with resources that don't take a dependency on Westwind.Globalization such as when editing resources files externally.


### Version 2.1 Release
<small>June 24th, 2015</small>

* **[Markdown support for Text Resources](https://github.com/RickStrahl/Westwind.Globalization/wiki/Markdown-For-Resource-Values)**<br/>
You can indicate that you would like text resources to be automatically parsed as markdown. Simply enter your text as Markdown and set the 'Type' option to Markdown causes the text resources to automatically parsed to HTML when added to the ResourceSet dictionary. Markdown conversion is also applied for Resx exports (converted to HTML) and the JavaScript Resource Handler.

* **Full screen Resource Editor**<br/>
You can now double click the resource editing field to display a full screen editor that provides more room for editing larger content. Works especially well when editing Markdown content.

* **[Simplified Resource Linking Activation via pre-configured Icon](https://github.com/RickStrahl/Westwind.Globalization/wiki/Interactive-Resource-Editing#resource-editing-toggle-button)**<br/>
You can now more easily add resource linking to any page that uses the `data-resource-id` resource linking features. Using `ww.resourceEditor.showEditButton()` is now a one liner that displays a clickable icon that toggles the edit mode. 

* **Support for Right To Left Languages in Resource Editor**<br/>
Individual resources displayed in the resource edit textarea now reflect the RTL setting of the language specified. So if you have a resource in Hebrew or Arabic for example, the edit box will let you type using RTL now.

* **[Improved Resource Caching for JavaScript Resources](https://github.com/RickStrahl/Westwind.Globalization/wiki/JavaScript-Resource-Handler---Serve-JavaScript-Resources-from-the-Server)**<br/>
Refactored code in the  resource HttpHandler that serves JavaScript resources. The query string based interface now supports ASP.NET ResourceProvider, .NET ResourceManager and Resx resources for serving server resources to JavaScript.

* **AlbumViewer MVC Localization Sample**<br/>
Added a small ASP.NET MVC sample application that demonstrates resource editing and usage in the user interface.

* **Fix WebForms Designer Issue with Meta-Resource Attributes**
Fix bug that caused controls to not render on the design time surface due to an error with designtime loading of database resources.

* **Separate Sample Resources and Test Page into Westwind.Globalization.Web.Starter NuGet Package**
In order to reduce the artifacts added to Web projects we've broken out the sample resources and test pages into a separate .Starter package which can be removed from the project after initial loading.

### Version 2.0 Beta6
<small>May 10th, 2014</small><br/>
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
