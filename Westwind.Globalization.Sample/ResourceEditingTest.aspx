<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResourceEditingTest.aspx.cs" Inherits="Westwind.Globalization.Sample.ResourceEditingTest" %>

<%@ Register assembly="Westwind.Globalization.Web" namespace="Westwind.Globalization" tagprefix="loc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>    
    <link href="LocalizationAdmin/bower_components/bootstrap/dist/css/bootstrap.css"
        rel="stylesheet" />
    <link href="LocalizationAdmin/css/localizationAdmin.css" rel="stylesheet" />   
</head>
<body>
    <form id="form1" runat="server">
    <div class="container" style="margin-top: 20px;">
    
        <div class="form-group">
            <label>Meta Tag (key lblHelloWorldLabel.Text):</label>
             
            <asp:Label ID="lblHelloWorldLabel" runat="server"
                 meta:resourcekey="lblHelloWorldLabel"
                class="form-control">                
            </asp:Label>
        </div>
        
        <div class="form-group">
            <label>Localize Control Tag (key lblHelloWorldLabel.Text):</label>
            <asp:Localize runat="server" id="localizeControl" meta:resourcekey="localizeControl">
                This is some text.
            </asp:Localize>
        </div>
            

            <br />
    </div>
        <loc:DbResourceControl ID="DbResourceControl1" runat="server" 
             ShowIconsInitially="Show" Visible="true"/>
    
    
    

    <p>
&nbsp;</p>
   

        <style>            
             .resource-editor-icon, .resource-editor-icon:hover,  .resource-editor-icon:visited {
                 position: absolute;
                 display: inline;
                 height: 13px;
                 width: 13px;     
                 text-decoration: none;
                 zIndex: 999999;
                 opacity: 0.65;
                 margin: -14px 0 0 -2px;
                 cursor: pointer;                 
                 color: green;
             }
            .resource-editor-icon:hover {
                opacity: 1;         
            }
            .resource-editor-icon:before {
                /*font-family: fontawesome;*/
                /*content: "\f024"; */ /* flag */
                font-family: Arial;
                content: "#";
                font-size: 9pt;                
            }
        </style>
        <script src="LocalizationAdmin/bower_components/jquery/dist/jquery.min.js"></script>
        <script src="LocalizationAdmin/scripts/ww.resourceEditor.js"></script>
        <script>
            ww.resourceEditor.showResourceIcons({
                adminUrl: "./LocalizationAdmin/"
            });
        </script>
    </form>
</body>
</html>
