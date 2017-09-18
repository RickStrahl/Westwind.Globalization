<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebFormDbResourceControlSample.aspx.cs"
    Inherits="Westwind.Globalization.Sample.WebFormDbResourceControlSample" %>

<%@ Register Assembly="Westwind.Globalization.Web" Namespace="Westwind.Globalization"
    TagPrefix="loc" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
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
<body>
    <form id="form1" runat="server">


        <div class="banner">
            <div id="TitleBar">
                <a href="./">
                    <img src="localizationAdmin/images/Westwind.Localization_128.png"
                        style="height: 35px; float: left" />
                    <div style="float: left; margin-left: 5px; line-height: 1.2">
                        <i style="color: steelblue; font-size: 0.8em; font-weight: bold;">West Wind Globalization</i><br />
                        <i style="color: whitesmoke; font-size: 1.25em; font-weight: bold;">WebForms DbResourceManager
                            Test</i>
                    </div>
                </a>
            </div>
        </div>

        <nav class="menubar " style="background: #727272;">
            <ul id="MenuButtons" class="nav navbar-nav pull-right navbar">
                <li>
                    <a href="localizationAdmin/">
                        <i class="fa fa-gears"></i>Resource Editor
                    </a>
                </li>
                <li>
                    <a href="resourcetest.aspx">
                        <i class="fa fa-check-circle"></i>WebForms Test Page
                    </a>
                </li>
                <li>
                    <a href="./">
                        <i class="fa fa-home"></i>
                    </a>
                </li>
            </ul>
            <div class="clearfix"></div>
        </nav>

        <div class="alert alert-info">
            <i class="fa fa-info-circle fa-3x" style="float: left; margin: 0 10px 10px 0"></i>
            <asp:Localize runat="server" ID="lblNotice" meta:resourcekey="lblNotice">                
                This page uses the DbResourceControl to automatically add resource editing links to any ASP.NET controls. Click the Resource Edit button to see the resource edit icons pop up. Any control added to the page, automatically gets resource editing links.
            </asp:Localize>
        </div>


        <div class="container" style="margin-top: 30px;">
            <div class="form-group">
                <label class="control-label" for="form-group-input">
                    <asp:Label runat="server" ID="lblName" Text="Name" meta:resourcekey="lblName" /></label>
                <asp:TextBox runat="server" ID="txtName" Text="" class="form-control" />
            </div>

            <div class="form-group">
                <label class="control-label" for="form-group-input">
                    <asp:Label runat="server" ID="lblCompany" Text="Company" meta:resourcekey="lblCompany" /></label>
                <asp:TextBox runat="server" ID="txtCompany" Text="" class="form-control" />
            </div>

            <div class="well-sm well">
                <asp:Button runat="server" ID="btnSumbit" Text="Save" CssClass="btn btn-primary" />
            </div>
        </div>

        <loc:DbResourceControl ID="DbResourceControl1" runat="server" EnableResourceLinking="true" />

        <script src="LocalizationAdmin/bower_components/jquery/dist/jquery.min.js"></script>
        <script src="LocalizationAdmin/scripts/ww.resourceEditor.js"></script>
        <script>
            ww.resourceEditor.showEditButton(
                {
                    adminUrl: "./localizationAdmin"
                }
            );
        </script>

    </form>
</body>
</html>
