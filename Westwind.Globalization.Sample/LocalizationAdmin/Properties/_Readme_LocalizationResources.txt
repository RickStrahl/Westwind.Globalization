*** DO NOT DELETE THESE RESOURCE FILES UNLESS YOU GENERATE NEW RESX FILES! ***

Please leave these resource files in this folder intact as this will allow the localization admin
form to work even if  the DbResourceProvider is not hooked up initially. If Resx Resources are 
used these resources will include  in the project and work as Resx resources so the Admin 
form displays properly.

If you start a new project you should import from Resx to pull these resources  - as well as 
any other existing resources into your project. Once imported the resources will then work
 from the DbResource file.

The localization admin form is a pure client side application so all resources are only 
served from the JavaScriptResourceHandler.ashx request.

