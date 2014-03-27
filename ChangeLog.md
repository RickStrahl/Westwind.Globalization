#West Wind Globalization Changelog

###Version 2.0
*not released yet*

* **DbRes Provider for direct Resource Access**
Added a new DbRes resource provider that bypasses the traditional
.NET resource providers and goes directly to the database provider.
It uses the same in-memory structures for caching, but provides
a much simpler direct access mechanism to resources without
provider configuration. This provider also allows for using
resource values as keys, so if no matches are found the resource
values are displayed as is.

* **Synced to 2.x versions Westwind Libraries**
This version is synced to version 2.50 of Westwind.Utilities and
Westwind.Web/WebForms which are more modular and have smaller
footprint.

* **Fix bug in JavaScriptResourceHandler**
The JavaScriptResourceHandler allows use of .NET resources from
clientside JavaScript code, by mapping resources to JSON exported
resource objects that match the active locale (or explicitly). Bug
fixes scoping of the variable and now properly handles both DbRes
and standard Resx resources (or any other custom ResourceProvider).

* **Improved ASP.NET MVC Support**
DBRes.T() provides a simple way to embed resources into MVC
applications based on string values. It'll use the active Resource 
Provider and inject resources as strings. Think of it as a localization
helper that allows resource access. Alternately you can also just 
generate strongly typed resources and reference those.

* **Improved Strongly Typed Resource Generator**
Updated the strongly typed resource generator to allow to create 
resources that can switch between the ASP.NET ResourceProvider or
plain ResourceManager for retrieving resources. Allows generated
resources to be used in non-Web applications (services, console etc.)

* **Updated Admin UI**
Tweaked the Admin UI so it works better as a standalone html
form with its own HTML resources and dependencies. Added switch
to allow turning safemode on and off which prevents access to
the write functions that import/export resources or create
table.