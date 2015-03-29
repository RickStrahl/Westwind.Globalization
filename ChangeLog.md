# West Wind Globalization Changelog

### Version 2.0
<small><i>currently in Beta</i></small>

* **New DbResourceDataManager Model to allow for pluggable Data Providers**<br/>
New DbResourceDataManager interface and adpater model for supporting multiple data providers. You can easily implement new DataManager implementations to handle the data access to read and write data into the provider for all supported features.

* **Add SqlCompact, MySql, SqlLite DbResourceManagers**<br/>
Added custom DbResourceDataManager implementations for each of these data engines and created separate assemblies for each of them to isolate dependencies. You can specify which DataManager is used via the Configuration.DataManagerType property.

* **New Administration Form**<br/>
Complete rewrote the Administration form using a pure client side model using Angular JS and a CallbackHandler service on the backend. The new UI provides much easier navigation of resources and shows all resources available for a resource key for easy editing. The editing UI is also keyboard friendly to allow for quick hands on the keyboard editing of resources.

* **Improved support for Resource File Uploads**<br/>
You can now more easily import and export file resoures from and to Resx files and also create new file resources for things like images and other non-plain string items. When exporting Resources are created in the same folder as the Resx files generated.

* **Improved Resource Import and Export**<br/>
The Web Interface now supports importing and exporting resources optionally from virtually any path, not just the path of the actual Web application. This means you can now easily edit and update resources from any project on your local machine.

* **Strongly Typed Class Export Improvements**
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