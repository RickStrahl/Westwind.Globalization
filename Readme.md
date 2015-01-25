# West Wind Globalization#
### Database Resource Localization for .NET###
Easily create ASP.NET resources stored iSn a Sql database for editing and updating 
at runtime using an interactive editor or programmatically using code.
The library uses the standard .NET and ASP.NET infrastructure of ResourceSets and 
ResourceManagers with a generic data backend to feed resource sets and provide 
edit and update capabilities to  localization resources stored in a database.

Additional tools provide the ability to import and export Resx resources and
the ability to serve server side resources as JSON resources to JavaScript clients.

Requirements:
* .NET 4.5
* Sql Server, Sql Express or SQL CE 4 or later

### Resources:
* [Westwind.Globalization Home Page](http://west-wind.com/westwind.globalization/)
* [Nuget Package](https://www.nuget.org/packages/Westwind.Globalization/)
* [Getting Started Video](https://www.youtube.com/watch?v=jHg4hlnZNoA)
* [Original Article for Database Driven Resource Provider](http://www.west-wind.com/presentations/wwdbresourceprovider/)
* [Documentation](http://west-wind.com/westwind.globalization/docs/)
* [Class Reference](http://west-wind.com/westwind.globalization/docs/?page=_40y0vh66q.htm)
* [Change Log](https://github.com/RickStrahl/Westwind.Globalization/blob/ResourceImportAndExportChanges/ChangeLog.md)
* [License](http://west-wind.com/Westwind.Globalization/docs/_2lp0u0i9b.htm)


### Features
* .NET Resources stored in Sql Server, Sql Compact Database
  (other providers in the future)  
* ASP.NET Database ResourceProvider 
* Standard .NET Database ResourceManager
* Use resources same way as with ResX or use our DbRes helper
* Interactive Web resource editor
* Use code to manipulate resources
* Import and Export Resx resources 
* Generate strongly typed resource classes
* Resource Handler to serve server side resources to JavaScript
* Release and reload resources at runtime
* DbRes helper to easily embed resources into markup and code

### Web Resource Editor
One of the main reasons people want to use Database resources rather
than Resx resources is that it allows for dynamic updates of resources. Resx
resources are static and compiled into an application and so are typically
tied to the development process, while dynamic resources can be updated
separately even after the application has been completed and deployed.

Since data is stored in a database it's easy to create editing front ends
or programmatic tools that simply manipulate the database. This library
ships with a Web interface that allows editing of resources interactively
and an easy to use data API to update resources programmatically.

![Web Resource Editor](https://raw.github.com/RickStrahl/Westwind.Globalization/master/WebResourceLocalizationForm.png)

![Web Resource Translator Dialog](https://raw.github.com/RickStrahl/Westwind.Globalization/master/WebResourceTranslateDialog.png)

The resource editor is an easy way to localize resources interactively,
but it's not the only way you can do this of course. Since you have access 
to the data API underneath it as well as the database itself, it's 
easy to create your own customized UI or data driven API that suits your
application needs exactly.

### How the database Providers work
This library works by implementing a custom .NET ResourceManager and 
ASP.NET ResourceProvider that are tied to a database provider. This 
means you can access resources using the same mechanisms that you
use with standard Resx Resources in your .NET applications. It also
means although resources are initially loaded from the database
for the first load of each ResourceSet, resources are cached for
each individual ResourceSet and locale the same way that Resx 
resources are read from the assembly resources and then cached. 
The database is hit only for the first read of a given 
ResourceSet/Locale combination. IOW, not every resource access
hits the database!

Underneath the .NET providers lies a the Westwind.Globalization data
access layer that provides the data interface to provide resources. 
This API is accessed by the Resource Provider and Resource Manager
implementations to read the resources from the database.

Additionally the API can be directly accessed to provide resource 
access, and the DbRes helper class provides very easy access to
these resources using the DbRes.T() method which can be thought
of as a high level translation method.

This interface is also directly accessible and allows your code as well as
support code like the UI Web Resource editor to easily access and manipulate
resources in real-time at runtime.


This library includes quite a proliferation of classes most of it due
to the implementation requirements for the .NET providers which require
implementation of a host of interface based classes for customization.

There are three distinct resource access mechanisms supported:

* ASP.NET Resource Provider (best used with WebForms)
* .NET Resource Manager and strongly typed resources 
  (Non-Web apps, and or MVC apps where you already use Resx)
* Direct Db Provider access using DbRes helper (easiest overall - works everywhere)

### Installation and Configuration###
The easiest way to use data driven resources with this library is to install the NuGet
package into an ASP.NET application.

```
pm> Install-Package Westwind.Globalization
```

This installs the required assemblies, adds a few configuration entries in
web.config and enables the resource provider by default. It also installs
the localization administration form shown above, so you can create the
resource table and manage resources in it.

#### Configuration Settings
The key configuration items set are the DbResourceProvider section in
the config file which tells the provider where to find the database
resources:

```Xml
<configuration>
  <configSections>
    <section name="DbResourceProvider" type="Westwind.Globalization.DbResourceProviderSection,Westwind.Globalization" requirePermission="false" />
  </configSections>
  <DbResourceProvider connectionString="server=.;database=localizations;integrated security=true"
                      resourceTableName="Localizations"
                      addMissingResources="false"
                      stronglyTypedGlobalResource="~/Properties/Resources.cs"
                      resourceNamespace="AppResources"
                      localizationFormWebPath="~/localizationadmin/LocalizationAdmin.aspx"                      
                      designTimeVirtualPath="/internationalization"
                      showLocalizationControlOptions="true"
                      showControlIcons="true"
                      useVsNetResourceNaming="false"
                      bingClientId=""
                      bingClientSecret="" />
</configuration>
```

The two most important keys are the connectionString and resourceTableName which point at your
database and a table within it. You can use either a raw connectionstring as above or a connection
string name in the ConnectionStrings section of your config file.

#### Run the Web Resource Editor
In order to use database resources you'll actually have to create some resources in a database.
Make sure you've first added a valid connection string in the config file in the last step! 
Then open the /LocalizationAdmin/LocalizationAdmin.aspx page in your browser and click on the
*Create Localization Table* button in the toolbar.

Once the table's been created you can now start creation of resources interactively, by directly
adding values to the database table, or by using the DbResourceDataManager API to manipulate the
data programmatically.

I recommend you start by adding a 'Resources' resource set that's the 'main' resource set used
to hold common and global reusable values. Create additional resource sets to break out individual
forms or sections or other related content.

Note if you're using WebForms, ASP.NET uses the concept of global and remote resources. The resource
editor will treat any ResourceSets that have a 'file extension' like .aspx as a local resource. To 
match local resources mapped to an ASPX page use (Path)/Page.aspx. Path can be blank in which case
you don't specify a leading slash. For example the localization admin page local resources live in
a ResourceSet named: 

LocalizationAdmin/LocalizationAdmin.aspx

Adding and manipulating resources in the Web editor should be pretty straight forward. One thing
to note however is:

* If you add a new ResourcesSet or Language in a resource you have to refresh the entire form
  otherwise the new language or ResourceSet doesn't show up in lists.

#### Setting ASP.NET Locale based on Browser Locale
In order to do automatic localization based on a browser's language used you can
sniff the browser's default language and set the UiCulture in the Begin_Request 
handler of your ASP.NET application class. A helper method to provide this 
functionality automatically is provided.

```C#
protected void Application_BeginRequest()
{
    // Automatically set the user's locale to what the browser returns
    // and optionally only allow certain locales/locale-prefixes
    WebUtils.SetUserLocale(allowLocales: "en,de");
}
```

This forces the user's Culture and UI Culture to whatever the browser is using,
and explicitly. Now when a page is rendered it will use the UiCulture of the browser.
The optional allowLocales enforces that only certain locales can be set - anything
not matched is defaulted to the server's default locale.

The way .NET resource managers work, if there's no match for the locale the user
provides, resources fall back to the closest matching locale or the invariant locale.
So if the user comes in with it-IT but you don't support it or it-IT in your resources
the user will see resources for invariant. Likewise if a user comes in with 
de-CH (Swiss german) and you de (without a locale specific suffix) the de German
version will be returned. Resource Fallback tries to ensure that always something
is returned.

### Using Resources in your Application
There are a number of different ways to access resources from this provider.

* Direct access with DbRes 
* ASP.NET Resource provider
* .NET Resource Manager

#### DbRes Helper Class
The DbRes Helper class is a wrapper around the DbResourceManager and DbResouceDataManager
object. The DbRes class contains a handful of common use static methods that are used to 
retrieve and manipulate resources.

In an ASP.NET Web MVC (or WebPages) application you can use:

```C#
// Using current UiCulture - empty resource set
DbRes.T("HelloWorld")

// Exact match with resource - Hallo Welt
DbRes.T("HelloWorld","Resources","de")

// Resource Fallback to de if de-CH doesn't exist - Hallo Welt
DbRes.T("HelloWorld","Resources","de-CH")
```

This is an easy mechanism that's tied closely to the database
resources created and can be applied with minimal fuss in any
kind of .NET application.

#### ASP.NET MVC or ASP.NET WebPages
```HTML
Say Hello: @DbRes.T("HelloWorld") at @DateTime.Now
```

#### ASP.NET WebForms
```HTML
Say Hello: <%: DbRes.T("HelloWorld") %> at <%= DateTime.Now %>
```

#### In .NET code
```HTML
string value = DbRes.T("HelloWorld");
```

### Using the ASP.NET Resource Provider 
If you're using an existing WebForms application or you want to
use the ASP.NET based Resource Provider model for accessing resources
you can use the DbSimpleResourceProvider. This implementation is an
ASP.NET Resource Provider implementation that directly accesses the
DbResourceDataManager to retrieve resources. A second Resource Provider
implementation that uses the DbResourceProvider uses the DbResourceManager
to indirectly access resources. Typically the DbSimpleResourceProvider is
the more efficient interface.

To use this provider you have to enable it in web.config. To do so:

```XML
<configuration>  
  <system.web>
       <globalization resourceProviderFactoryType="Westwind.Globalization.DbSimpleResourceProviderFactory,Westwind.Globalization" />    
       <!--<globalization resourceProviderFactoryType="Westwind.Globalization.DbResourceProviderFactory,Westwind.Globalization" />-->    
  </system.web>
</configuration>
```

Once enabled you can use all the standard ASP.NET Resource Provider
features:

* GetGlobalResourceObject, GetLocalResourceObject on Page and HttpContext
* Using meta:resourcekey attributes on Web Controls

#### Page.GetGlobalResourceObject() or HttpContext.GetGlobalResourceObject() 
```HTML
<legend>ASP.NET ResourceProvider</legend>
<label>Get GlobalResource Object (default locale):</label>
<%: Page.GetGlobalResourceObject("Resources","HelloWorld") %>
```

#### Page.GetLocalResourceObject()
```HTML
<label>GetLocalResourceObject via Expression:</label>                 
<%: GetLocalResourceObject("lblHelloWorldLabel.Text") %>
```

####WebForms Control meta:resourcekey attribute
```HTML
<label>Meta Tag (key lblHelloWorldLabel.Text):</label>
<asp:Label ID="lblHelloLabel" runat="server" meta:resourcekey="lblHelloWorldLabel"></asp:Label>
```

#### WebForms Resource Expressions
```HTML
<label>Resource Expressions (Global Resources):</label>
<asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resources,HelloWorld %>"></asp:Label>
```


#### Strongly typed Resources
The Web Resource Editor form allows you to create strongly typed resources
for any global resources in your application. Basically it'll go through all the 
non-local resources in your file and create strongly type .NET classes in a file that
is specified in the DbResourceProvider configuration settings.

```
stronglyTypedGlobalResource="~/Properties/Resources.cs,WebApplication1"
```
You specify the filename in your project and the namespace to generate it to. 
The generated resources can use either the ASP.NET resource provider (which uses
whatever provider is configured - Resx or DbResourceProvider) or the 
DbResourceManager which only uses the DbResourceManager. Using the latter allows
you to also generate resources for use in non-Web applications.

Here's what generated resources look like:
```C#
namespace WebApplication1
{
    public class GeneratedResourceSettings
    {
        // You can change the ResourceAccess Mode globally in Application_Start        
        public static ResourceAccessMode ResourceAccessMode = ResourceAccessMode.AspNetResourceProvider;
    }

	public class Commonwords
	{
		public static System.String Ready
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Commonwords","Ready");
				return DbRes.T("Ready","Commonwords");
			}
		}

		public static System.String ThisIsALongLineOfText
		{
			get
			{
				if (GeneratedResourceSettings.ResourceAccessMode == ResourceAccessMode.AspNetResourceProvider)
					return (System.String) HttpContext.GetGlobalResourceObject("Commonwords","This is a long line of text");
				return DbRes.T("This is a long line of text","Commonwords");
			}
		}
	}
}
```

These can then be used in any ASP.NET application:

#### ASP.NET MVC or WebPages
```HTML
<div class="statusbar">@CommonWords.Ready</div>
```

#### ASP.NET WebForms
```HTML
<div class="statusbar"><%: WebApplication2.CommonWords.Ready %></div>
```

Note that strongly typed resources must be recreated whenever you add new
resources, so this is an ongoing process. As with the Resx Generator if
you remove or rename a resource you may break your code. This is the 
reason we use a single file, rather than a file per resource set to 
keep the file management as simple as possible.


#### JavaScript Resource Handler
Localization doesn't stop with server templates - if you're building applications
that include JavaScript logic it's likely that you also need to access resources
on the client that are localized. This library provides a JavaScript Resource 
HttpHandler that can serve resources in the proper localized locale to your
client application.

**Configuration:**
To configure the Resource Handler it has to be registered in web.config as follows:

```xml
<configuration>
<system.webServer>
<handlers>
    <add name="JavaScriptResourceHandler"
        verb="GET"
        path="JavascriptResourceHandler.axd"
        type="Westwind.Globalization.JavaScriptResourceHandler,Westwind.Globalization" />
</handlers>
</system.webServer>
</configuration>
```

Note this entry is automatically made for you when you install the NuGet package.

**Usage:**
The resource handler is then accessed as a script resource in your code by calling the
static JavaScriptResourceHandler.GetJavaScriptResourcesUrl() method:

```html
<!-- Generates a resources variable that contains all server side resources translated for this resource set-->
<script src="@JavaScriptResourceHandler.GetJavaScriptResourcesUrl("resources","Resources")"></script>
<script>
    document.querySelector("#JavaScriptHelloWorld").innerText = resources.HelloWorld;
</script>
```
You pass in the name of the variable you want to have created which in this case is `resources`. 

This generates a fairly verbose and ugly URL:

```
http://localhost:7894/JavaScriptResourceHandler.axd?ResourceSet=Resources
      &LocaleId=de-DE&VarName=resources&ResourceType=resdb&ResourceMode=1
```

which in turn generates the following script code (shown here localized in German):

```javascript
resources = {
	"HelloWorld": "Hallo schn\u00F6de Welt",
	"Ready": "Los",
	"Today": "Heute",
	"Yesterday": "Gestern"
};
```

The localization by default uses the active locale of the current request, so if you switch
the locale using WebUtils.SetUserLocale() as shown earlier, the resources are localized to
that locale as well.

Note that the variable name is generated in global scope by default, so `resources` is generated
in global scope. However, you can pass in any variable name. For example, if you have a previously
declared object that you want to attach the resources to you can use that name. For example:

```html
<script>
    global = {};  //  declare your own global object on the page
</script>
<!-- generated resources to global.resources -->
<script src="@JavaScriptResourceHandler.GetJavaScriptResourcesUrl("global.resources","Resources")"></script>
<script>
    // use global.resources object to access resource values
    document.querySelector("#JavaScriptHelloWorld").innerText = global.resources.HelloWorld;
</script>
```

#### License
The Westwind.Globalization library is licensed under the
[MIT License](http://opensource.org/licenses/MIT) and there's no charge to use, 
integrate or modify the code for this project. You are free to use it in personal, 
commercial, government and any other type of application. 

[Commercial Licenses](http://west-wind.com/Westwind.Globalization/docs/?page=_2lp0u0i9b.htm) are 
also available as an option. If you are using these tools in a commercial application
please consider purchasing one of our reasonably priced commercial licenses that help support 
this project's development.

All source code is copyright West Wind Technologies, regardless of changes made to them. 
Any source code modifications must leave the original copyright code headers intact.


#### Warranty Disclaimer: No Warranty!
IN NO EVENT SHALL THE AUTHOR, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR REDISTRIBUTE 
THIS PROGRAM AND DOCUMENTATION, BE LIABLE FOR ANY COMMERCIAL, SPECIAL, INCIDENTAL, 
OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM 
INCLUDING, BUT NOT LIMITED TO, LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR 
LOSSES SUSTAINED BY YOU OR LOSSES SUSTAINED BY THIRD PARTIES OR A FAILURE OF THE 
PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS, EVEN IF YOU OR OTHER PARTIES HAVE 
BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.