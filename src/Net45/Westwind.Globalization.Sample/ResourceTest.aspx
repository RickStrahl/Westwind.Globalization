<%@ Page Language="C#" %>

<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Resources" %>
<%@ Import Namespace="System.Threading" %>
<%--Import the strongly typed resource namespace - if this fails strongly typed resources don't exist--%>
<%@ Import Namespace="Westwind.Globalization.Sample.Properties" %>

<script runat="server">
    private string LocaleId;

    protected override void InitializeCulture()
    {
        base.InitializeCulture();

        LocaleId = Request.Params["LocaleId"];

        // explicitly reset the UserLocale
        if (!string.IsNullOrEmpty(LocaleId))
            Westwind.Utilities.WebUtils.SetUserLocale(LocaleId, LocaleId, "$", true, "en,de,fr");
    }
    
</script>
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

    <title>DbResourceManager Test Page (WebForms)</title>

    <link href="localizationAdmin/scripts/vendor/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="localizationAdmin/scripts/vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" />

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
                            Web Forms DbResourceManager Test
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
                    <a href="/">
                        <i class="fa fa-home"></i>
                    </a>
                </li>         
            </ul>
            <div class="clearfix"></div>
        </nav>

    
<form runat="server" id="form1" method="GET" 
     style="height: 0; padding: 0; margin: 0;" enableviewstate="false">


    <div class="well container" style="margin-top: 30px;">
        <section class="pull-right" style="text-align: right">
            <div>
                Current Culture: <b><%= Thread.CurrentThread.CurrentCulture.IetfLanguageTag %></b>
            </div>
            <div>
                Current UI Culture: <b><%= Thread.CurrentThread.CurrentUICulture.IetfLanguageTag %></b>
            </div>
            <div>
                    <select id="localeId" name="localeId"
                        value="<%= LocaleId %>"                        
                        onchange="document.forms['form1'].submit();"
                        class="form-control">
                        <option value="">Browser Default</option>
                        <option value="en">English</option>
                        <option value="de">German</option>
                        <option value="fr">French</option>
                    </select>
                    <script>
                        var localeId = '<%= LocaleId %>';
                        var el = document.getElementById("localeId");
                        for (var i = 0; i < el.options.length; i++) {
                            var opt = el.options[i];
                            if (opt.value == localeId)
                                opt.selected = true;
                            else
                                opt.selected = false;
                        }
                    </script>
               
                
            </div>
        </section>

        <div class="container">
            <h3>DbRes</h3>
          
            <label >Using DbRes Direct Access Provider (default locale):</label>                                  
            <span data-resource-id="HelloWorld">
                <%= DbRes.T("HelloWorld","Resources") %>
            </span>

            <label>Using DbRes Force to German:</label>
            <%= DbRes.T("HelloWorld","Resources","de") %>
            
            <label>Missing Resource:</label>
            <span data-resource-id="MissingResource">
                <%= @DbRes.T("MissingResource", "Resources") %>
            </span>    
        </div>

        <hr />

        <div class="container">

            <h3>ASP.NET ResourceProvider</h3>
            <label>Get GlobalResource Object (default locale):</label>
            <%= GetGlobalResourceObject("Resources","HelloWorld") %>

			<label>GetLocalResourceObject:</label>
            <span data-resource-id="lblHelloWorldLabel.Text" data-resource-set="ResourceTest.aspx">
                <%= GetLocalResourceObject("lblHelloWorldLabel.Text") %>
            </span>
			
            <label>Meta Tag (meta:resourcekey= lblHelloWorldLabel.Text):</label>
            <asp:Label ID="lblHelloLabel" runat="server" meta:resourcekey="lblHelloWorldLabel"></asp:Label>            

            <label>Resource Expressions (Global Resources):</label>
            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resources,HelloWorld %>"></asp:Label>
            
            <label>Resource Expressions in Lists (Global Resources):</label>
            <asp:DropDownList runat="server" id="lstNames" CssClass="form-control" style="width: 200px;" >
                <asp:ListItem Value="1" Text="<%$ Resources:Resources,Today %>"></asp:ListItem>
                <asp:ListItem Value="2" Text="<%$ Resources:Resources,Yesterday %>"></asp:ListItem>
            </asp:DropDownList>
            
        </div>

    
        <hr />

        <div class="container">
            <h3>Strongly Typed Resource (generated)</h3>

            <label data-resource-id="StronglyTypedDbResource">Strongly typed Resource Generated from Db (uses ASP.NET ResourceProvider)</label>
            <span data-resource-id="HelloWorld">
                <%= Resources.HelloWorld %>      
            </span>
            
            <label data-resource-id="StronglyTypedDbResource">Strongly typed Resource using ResourceManager.GetString</label>
            <span data-resource-id="HelloWorld">
                <%= Resources.ResourceManager.GetString("HelloWorld") %>      
            </span>
                                         
			
            <label>Strongly typed image resource:</label>
            <div data-resource-id="FlagPng">
                <% try { %>
                <%= GeneratedResourceHelper.BitmapToEmbeddedHtmlImage(Resources.FlagPng, "image/png") %>
                <% } catch {} %>
            </div>
		</div>

        <div class="container">
            <h3>Expanded Markdown Text</h3>
            
            <label>Auto Expanded Markdown text:</label>
            <span data-resource-id="MarkdownText">
                <%= DbRes.T("MarkdownText","Resources") %>
            </span>
        </div>

        <hr />

        <div class="container">            
            <h3>JavaScript Resource Handler</h3>
            <label>Localized JavaScript Variable (assigned in JavaScript code from resources.HelloWorld):</label>
            
            <div id="JavaScriptHelloWorld"></div>
        </div>                
    </div>

    </form>        

    <script>
        global = {};
    </script>
    <!-- Generates a resources variable that contains all server side resources translated for this resource set-->
    <script src="<%= JavaScriptResourceHandler.GetJavaScriptResourcesUrl("global.resources","Resources") %>"></script>
     
    <%-- *** You can also use raw script tags to embed which is more verbose ***
    <script src="/JavaScriptResourceHandler.axd?ResourceSet=Resources&LocaleId=en&VarName=resources&ResourceType=resdb&ResourceMode=1"></script>
    --%>

    <script>
        document.querySelector("#JavaScriptHelloWorld").innerText = global.resources.HelloWorld;
    </script>
    
    
    <!-- Enable Resource Editing --> 


    <script src="LocalizationAdmin/scripts/vendor/jquery.min.js"></script>   
    <script src="LocalizationAdmin/scripts/ww.resourceEditor.js"></script>
    <script>
        var toggleEditMode = false;
        $("#EditResources").click(function () {
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
