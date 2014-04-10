<%@ Page Language="C#" Culture="auto:en-US" UICulture="auto:en-US"  %>
<%@ Import Namespace="System.Threading" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/LocalizationAdmin/LocalizeAdminForm.css" rel="stylesheet" />
    <style>
        label {
            display: block;
            margin-top: 10px;
            margin-bottom: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>DbResourceManager Simple Resource Test</h1>

        <div class="well container" style="padding: 20px;">
            <div>
                Current Culture: <%= Thread.CurrentThread.CurrentCulture.IetfLanguageTag %>
            </div>
            <div>
                Current UI Culture: <%= Thread.CurrentThread.CurrentUICulture.IetfLanguageTag %>
            </div>

            <div class="container">
                <fieldset>
                    <legend>DbRes</legend>

                    <label>Using DbRes Direct Access Provider (default locale):</label>
                    <%= DbRes.T("HelloWorld","Resources") %>

                    <label>Using DbRes Force to German:</label>
                    <%= DbRes.T("HelloWorld","Resources","de") %>
                </fieldset>
            </div>

            <div class="container">
                <fieldset>
                    <legend>ASP.NET ResourceProvider</legend>
                    <label>Get GlobalResource Object (default locale):</label>
                    <%= GetGlobalResourceObject("Resources","HelloWorld") %>

                    <label>Meta Tag (key lblHelloWorldLabel.Text):</label>
                    <asp:Label ID="lblHelloLabel" runat="server" meta:resourcekey="lblHelloWorldLabel"></asp:Label>

                    <label>GetLocalResourceObject via Expression:</label>
                    <%= GetLocalResourceObject("lblHelloWorldLabel.Text") %>
                </fieldset>
            </div>

			<%-- Strongly typed resources will work once you create them and
			     add the appropriate namespaces --%>
            <%-- <div class="container">
                <fieldset>
                    <legend>Strongly Typed Resource (generated)</legend>

                    <label>Strongly typed Resource Generated from Db (uses ASP.NET ResourceProvider)</label>
                    <%= Resources.Helloworld %>
                </fieldset>
            </div> --%>
        </div>

    </form>
</body>
</html>
