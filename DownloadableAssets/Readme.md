# Localization Admin UI and Sample Downloads
.NET Core no longer allows content as part of NuGet packages, and these zip files provide the additional content files you can add to your project.

This folder contains two sets of support files in zipped format:

* **Localization Administration UI for ASP.NET Core**  
This Zip file contains the HTML, CSS, Scripts and Images to run the Localization Administration UI for ASP.NET Core. 
<small>[Download](https://github.com/RickStrahl/Westwind.Globalization/blob/master/DownloadableAssets/LocalizationAdministrationHtml_AspNetCore.zip?raw=true)</small>

* **Sample Page and Resources for ASP.NET Core**   
This zip file contains a sample page and resources you can play with when first setting up Westwind.Globalization in a ASP.NET Core site.  <small>[Download](https://github.com/RickStrahl/Westwind.Globalization/blob/master/DownloadableAssets/LocalizationSample_AspNetCore.zip?raw=true)</small>

> #### The Admin UI requires ASP.NET Core MVC
> Both of these HTML support addons rely on features of **ASP.NET MVC** in order to run. Make sure your ASP.NET Core app has `app.UseMvc()` and `app.UseStaticFiles()` as part of its startup code in order for the Admin UI to work.

## Setting up the Localization Admin UI for ASP.NET Core
To set up the Localization Admin UI a few steps are required:

* Download the additional assets
* Unzip and copy them into the **root folder of the project**
* Setup provider configuration via code or configuration files
* Recompile your Project
* Import the localization resources for the examples

### Detailed Setup Steps
Here are the detailed steps: 

* Download [LocalizationAdministration_AspNetCore.zip](https://github.com/RickStrahl/Westwind.Globalization/blob/master/DownloadableAssets/LocalizationAdministrationHtml_AspNetCore.zip?raw=true)
* Unzip the package into the **root folder** of your ASP.NET Core MVC application
* [Hook up Configuration for Westwind.Globalization](https://github.com/RickStrahl/Westwind.Globalization#enabling-west-wind-globalization-in-aspnet-core)
* Recompile your project
* Set up your Configuration  
  *Set the `ConnectionString` to an existing database, and set `ResourceTableName` to a table that you want to create. Use either `DbResourceConfiguration.json`, `appsettings.json`, or via Startup code configuration (see [main docs](https://github.com/RickStrahl/Westwind.Globalization#aspnet-core-configuration))*
    * If you use `DbResourceConfiguration.json`:  
    Set **Copy to Output Folder** to **Copy if newer**
* Open the Admin Page at `localizationAdmin/index.html`
* Click on **Create Table**
* You should see some sample resources in the admin interface
* Click **Import or Export Resx**
* Select **Import Resources** from **~/Properties/**
* Click **Import** - you should now see the LocalizationAdmin ResourceSet


### What the Zip File Contains
The zip file provides the Administration UI client side assests and .NET Resources required to run the Admin form.

* **Copies Admin UI into wwwroot/LocalizationAdmin**  
Copies files into `wwwroot/LocalizationAdmin`, which contains the **static** client side Localization Admin UI HTML and assets into your application. **You can move this folder** to any location you want. If you generate your `wwwroot` folder as part of a client side build, you can move the `LocalizationAdmin` folder into the client side build directory.

* **Copies Localization Admin Resources into Properties**   
Copies `Properties\LocalizationAdminForm.resx` and `Properties\LocalizationAdminForm.de.resx` files into the `Properties` folder of your project, these are the resources required for the LocalizationAdmin backend application which serves JavaScript resources to the client.

### What it Runs
Once you run the application and access teh admin UI, `Westwind.Globalization.AspNetCore` exposes two api services which use their own set of hard coded routes:

* **api/LocalizationAdministration/XXXXXXX** 
These calls calls handle the Localization Administration backend for showing and editing  resources. 

* **api/JavaScriptLocalizationResources**  
Handles JavaScript resource localization by serving resources from the server as a JavaScript object.

### Securing the Administration UI
By default the Localization Administration UI is **always accessible**!

In order to configure access to the API you can implement a startup configuration delegate with the signature of `Func<ControllerContext, bool>` which needs to return `true` or `false` depending on whether you want to allow access. At minimum you probably want to check for authenticated users, or group membership.

Here's how you can do this as part of the ASP.NET Core Startup in the `Configure()` method of Startup:

```cs
services.AddWestwindGlobalization(opt =>
{                
    // the default settings comme from DbResourceConfiguration.json if exists
    // Customize opt with  any customizations here...

    // *** THIS ***
    // Set up security for Localization Administration form
    opt.ConfigureAuthorizeLocalizationAdministration(controllerContext =>
    {
        // return true or false whether this request is authorized
        return controllerContext.HttpContext.User.Identity.IsAuthenticated;
    });
});

// if checking for users you need to hook up ASP.NET Auth of some sort
services.AddAuthentication(...);
```

The code you run here can be anything at all, but typically you will check access against your authentication scheme defined as part of your application.

> Note: In ASP.NET Core Authentication features only work if you have actually hooked up an Authentication mechanism as part of the application in `Startup` with `app.AddAuthentication()`.
More info can be on the [ASP.NET Docs Authentication Topics](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)



## Setting up ResourceTest Sample Page
The ResourceTest Sample page allows you to check out Westwind Globalization in your application and to easily check and see if the localization configuration features are set up properly.

To install:

* Download the [zip file](https://github.com/RickStrahl/Westwind.Globalization/blob/master/DownloadableAssets/LocalizationSample_AspNetCore.zip?raw=true)
* Unzip in the root folder of your ASP.NET Core MVC application
* Recompile your project

### What it Does
The zip file copies a sample page and resources:

* `Pages/ResourceTest.cshtml`
* Copies several Resource files into `Properties` folder

In order to use these:

* Unzip the files into the project root
* Recompile your project
* Start the Localization UI
* Import Export Resx
* Import Resources
* Navigate to /ResourceTest
