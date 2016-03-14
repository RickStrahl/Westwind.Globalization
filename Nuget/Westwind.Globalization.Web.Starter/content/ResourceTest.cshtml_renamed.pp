@*
    This test page has to be renamed to be used in order to avoid project startup
    issues if ASP.NET Web Pages/MVC are not present. 
    
    Use requires:
        * Have ASP.NET WebPages or MVC 4/5 installed 
        * Or: install-package Microsoft.AspNet.WebPages and compile
        * <appSettings>
              <add key="webPages:Version" value="3.0.0.0"/>
              <add key="webpages:Enabled" value="true"/>
          </appSettings>       
        * Rename this file to ResourceTest.cshtml
*@
@using System.Drawing.Imaging
@using System.Threading
@using Westwind.Globalization
@using $rootnamespace$.Properties
@{    
    var localeId = Request.Params["LocaleId"];
    if (!string.IsNullOrEmpty(localeId))
    {
        Westwind.Utilities.WebUtils.SetUserLocale(localeId, localeId, "$", true, "en,de,fr");
    }  
}
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
    <meta name="description" content="Localization Administration Form">

    <link rel="shortcut icon" href="localizationAdmin\favicon.ico" type="image/x-icon" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black" />

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
    <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
    <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
     <![endif]-->

    <title>DbResourceManager Test Page</title>

    <link href="localizationAdmin/bower_components/bootstrap/dist/css/bootstrap.min.css"
          rel="stylesheet" />
    <link href="localizationAdmin/bower_components/fontawesome/css/font-awesome.min.css"
          rel="stylesheet" />

    <link href="localizationAdmin/css/localizationAdmin.css" rel="stylesheet" />
    <style>
        label {
            display: block;
            margin-top: 10px;
            margin-bottom: 0;
        }

        hr {
            margin: 10px 0 -10px;
        }
    </style>
    
</head>
<body data-resource-set="Resources">
    <div class="banner">
        <div id="TitleBar">
            <a href="./">
                <img src="localizationAdmin/images/Westwind.Localization_128.png"
                     style="height: 35px; float: left" />
                <div style="float: left; margin-left: 5px; line-height: 1.2">
                    <i style="color: steelblue; font-size: 0.8em; font-weight: bold;">West Wind Globalization</i><br />
                    <i style="color: whitesmoke; font-size: 1.25em; font-weight: bold;">
                        Razor DbResourceManager Test
                    </i>
                </div>
            </a>
        </div>
    </div>

    <nav class="menubar " style="background: #727272;">
        <ul id="MenuButtons" class="nav navbar-nav pull-right navbar">
            <li>
                <a href="localizationAdmin/">
                    <i class="fa fa-gears"></i> Resource Editor
                </a>
            </li>
            <li>
                <a href="resourcetest.aspx">
                    <i class="fa fa-check-circle"></i> WebForms Test Page
                </a>
            </li>
            <li>
                <a href="javascript:{}" id="EditResources">
                    <i class="fa fa-flag"></i> Edit Resources
                </a>
            </li>  
            <li>
                <a href="~/">
                    <i class="fa fa-home"></i>
                </a>
            </li>         
        </ul>
        <div class="clearfix"></div>
    </nav>


<div class="well container" style="margin-top: 30px;">
    <section class="pull-right" style="text-align: right">
        <div>
            Current Culture: <b>@Thread.CurrentThread.CurrentCulture.IetfLanguageTag</b>
        </div>
        <div>
            Current UI Culture: <b>@Thread.CurrentThread.CurrentUICulture.IetfLanguageTag</b>
        </div>
        <div>
            <form id="form1" name="form1" action="ResourceTest.cshtml" method="GET">
                <select id="localeId" name="localeId"
                        onchange=" document.forms['form1'].submit(); "
                        class="form-control">
                    <option value="">Browser Default</option>
                    <option value="en">English</option>
                    <option value="de">German</option>
                    <option value="fr">French</option>
                </select>
                <script>
                    var localeId = '@localeId';
                    if (localeId) {
                        var el = document.getElementById("localeId");
                        for (var i = 0; i < el.options.length; i++) {
                            var opt = el.options[i];
                            if (opt.value === localeId)
                                opt.selected = true;
                            else
                                opt.selected = false;
                        }
                    }
                </script>
            </form>
        </div>
    </section>


    <div class="container">
        <h3>DbRes</h3>
        <label>Using DbRes Direct Access Provider (default locale):</label>
        <span data-resource-id="HelloWorld">@DbRes.T("HelloWorld", "Resources")</span>

        <label>Using DbRes Force to German:</label>
        @DbRes.T("HelloWorld", "Resources", "de")

        <label>Missing Resource:</label>
        <span data-resource-id="MissingResource">@DbRes.T("MissingResource", "Resources")</span>
    </div>

    <div class="container">

        <h3>ASP.NET ResourceProvider</h3>
        <label>Get GlobalResource Object (default locale):</label>
        @HttpContext.GetGlobalResourceObject("Resources","HelloWorld")

        <h3>.NET ResourceManager</h3>
        @Resources.ResourceManager.GetString("HelloWorld")
    </div>

    <div class="container">
        <h3>Strongly Typed Resource (generated)</h3>

        <label>Strongly typed Resource Generated from Db (uses ASP.NET ResourceProvider)</label>
        @Resources.HelloWorld
        
        <label>Strongly typed image resource: <small>(if available)</small></label> 
        <div data-resource-id="FlagPng">
            @try
            {
                @Html.Raw(GeneratedResourceHelper.BitmapToEmbeddedHtmlImage(Resources.FlagPng, ImageFormat.Png))
            }
            catch { }
        </div>

    </div>

    <div class="container">
        <h3>Expanded Markdown Text</h3>

        <label>Auto Expanded Markdown text:</label>
        <div data-resource-id="MarkdownText">
            @DbRes.THtml("MarkdownText", "Resources")
        </div>
    </div>

    <div class="container">
        <h3>JavaScript Resource Handler</h3>
        <label>Localized JavaScript Variable (assigned in JavaScript code from resources.HelloWorld):</label>
        <div id="JavaScriptHelloWorld"></div>
    </div>
</div>


    
<!-- Generates a resources = { "HelloWorld": "Hello World", "Today": "Today" } style variable that contains all server side resources translated for this resource set-->
    <script src="@JavaScriptResourceHandler.GetJavaScriptResourcesUrl("resources","Resources")"></script>
    <script>
        document.querySelector("#JavaScriptHelloWorld").innerText = resources.HelloWorld;
    </script>

    <script src="LocalizationAdmin/bower_components/jquery/dist/jquery.min.js"></script>   
    <script src="LocalizationAdmin/scripts/ww.resourceEditor.js"></script>
    <script>
        var toggleEditMode = false;
        $("#EditResources").click(function() {
            toggleEditMode = !toggleEditMode;
            if (toggleEditMode)
                ww.resourceEditor.showResourceIcons({
                    adminUrl: "./localizationAdmin/",
                    editorWindowOpenOptions: "height=600,width=900,top=20,left=20"
                });
            else
                ww.resourceEditor.removeResourceIcons();
        });
    </script>

</body>
</html>
