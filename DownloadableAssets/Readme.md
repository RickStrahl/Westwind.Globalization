# Localization Admin UI and Sample Support Files
.NET Core no longer allows content as part of NuGet packages, and these zip files provide the additional content files you can add to your project.

> #### Requires ASP.NET Core MVC
> Both of these addons rely on features of **ASP.NET MVC** in order to run. Make sure your ASP.NET Core app has ` app.UseMvc()` as part of its startup code. Note: It's only these addons that require MVC - **running** `Westwind.Globalization` and `Westwind.Globalization.AspNetCore` in an application doesn't require MVC - it's only the Admin UI and Sample that do.

This folder contains two sets of support files:

* **Localization Administration UI for .NET Core**  
This Zip file contains the HTML, CSS, Scripts and Images to run the Localization Administration UI for .NET Core. These files are not required to **run** `Westwind.Globalization.AspNetCore`, but you do need them if you want your site to run the Localization Admin UI.  
Download

* **Sample Page and Resources**   
When you first set up Westwind.Globalization in a Web site you might want to have a sample page to test operation and make sure it works. This zip file contains, `ResourceSample.cshtml` which is stored in the `Pages` folder of an MVC application along with Resx resources for that page you can import into the Localization database for testing.


## Setting up the Localization Admin UI
To set up the Localization Admin UI is a two step process:

* Download [LocalizationAdministration_AspNetCore.zip](https://github.com/RickStrahl/Westwind.Globalization/blob/AspNetCore_Support/master/LocalizationAdministrationHtml_AspNetCore.zip?raw=true)
* Unzip the package into the **root folder** of your ASP.NET Core MVC application
* Recompile your project

#### What the Zip File Contains
The zip file provides the Administration UI client side assests and .NET Resources required to run the Admin form.

* **Copies Admin UI HTML and Resources**  
Copies files into `wwwroot/LocalizationAdmin`, which contains the client side Localization Admin UI HTML and assets into your application. You can move this folder to any location you want. If you generate your `wwwroot` folder as part of a client side build, you can move the `LocalizationAdmin` folder into the client side build directory.

* **Copies Localization Admin Resources into Properties**   
Copies `Properties\LocalizationAdminForm.resx` and `Properties\LocalizationAdminForm.de.resx` files into the `Properties` folder of your project, these are the resources required for the LocalizationAdmin backend application which serves JavaScript resources to the client.

#### What it Runs
Once you run the application and access teh admin UI, `Westwind.Globalization.AspNetCore` exposes two api services which use their own set of hard coded routes:

* **api/LocalizationAdministration/XXXXXXX** 
These calls calls handle the Localization Administration backend for showing and editing  resources. 

* **api/JavaScriptLocalizationResources**  
Handles JavaScript resource localization by serving resources from the server as a JavaScript object.

#### Securing the Administration UI
By default the Localization Administration UI is **always accessible**!

In order to configure access to the API you can implement a startup configuration delegate with the signature of `Func<ControllerContext, bool>` which needs to return `true` or `false` depending on whether you want to allow access. At minimum you probably want to check for authenticated users, or group membership.

Here's how you can do this as part of the ASP.NET Core Startup in the `Configure()` method of Startup:

```cs
services.AddWestwindGlobalization(opt =>
{                
    // the default settings comme from DbResourceConfiguration.json if exists
    // Customize opt with  any customizations here...

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


