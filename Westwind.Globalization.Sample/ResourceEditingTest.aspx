<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResourceEditingTest.aspx.cs" Inherits="Westwind.Globalization.Sample.ResourceEditingTest" %>

<%@ Register assembly="Westwind.Globalization.Web" namespace="Westwind.Globalization" tagprefix="loc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>    
    <link href="LocalizationAdmin/bower_components/bootstrap/dist/css/bootstrap.css"
        rel="stylesheet" />
    <link href="LocalizationAdmin/css/localizationAdmin.css" rel="stylesheet" />   
    <link href="LocalizationAdmin/bower_components/fontawesome/css/font-awesome.min.css"
        rel="stylesheet" />
            <style>            
             .resource-editor-icon, .resource-editor-icon:hover,  .resource-editor-icon:visited {
                 position: absolute;
                 display: inline;
                 height: 13px;
                 width: 13px;     
                 text-decoration: none;
                 zIndex: 999999;                 
                 margin: -14px 0 0 -2px;
                 cursor: pointer; 
                 opacity: 0.45;                                   
             }
            .resource-editor-icon:hover {
                opacity: 1;         
            }
            .resource-editor-icon:before {
                font-family: fontawesome;
                content: "\f024";   /*flag*/ 
                /*font-family: Arial;
                content: "#";*/
                font-size: 9pt;                                    
            }
        </style>
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
            <asp:Localize runat="server" ID="localizeControl" meta:resourcekey="localizeControl">
                This is some default text.
            </asp:Localize>
        </div>
            
        
        <div class="form-group">
            <label>Localize Control Tag (key lblHelloWorldLabel.Text):</label>
            <asp:Literal runat="server" id="literalControl" meta:resourcekey="literalControl">
                This is some default text.
            </asp:Literal>
        </div>
        
             <asp:GridView ID="dgResources" runat="server" Width="90%" AutoGenerateColumns="False" 
                                         cssclass="table table-striped" 
                                         EnableViewState="False"
                                         GridLines="None"
                                         ShowFooter="True"                                          
                                         cellpadding="2" meta:resourcekey="dgOrders">
                                          
                 
                            <AlternatingRowStyle CssClass="gridalternate" />
                            <FooterStyle CssClass="gridheader" />
                            <HeaderStyle CssClass="gridheader" />
                            
                            <Columns>                                
                            <asp:BoundField  DataField="ResourceId"  HeaderText="ResourceId" 
                                             meta:resourcekey="BoundField" HtmlEncode="false">
                                <headerstyle font-italic="False" font-overline="False" font-strikeout="False"
                                    font-underline="False" horizontalalign="Center" width="200px"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="ResourceSet" HeaderText="ResourceSet" meta:resourcekey="BoundField1">
                                <itemstyle  font-italic="False" font-overline="False" font-strikeout="False"
                                    font-underline="False" horizontalalign="Center" />
                                <headerstyle font-italic="False" font-overline="False" font-strikeout="False"
                                    font-underline="False" horizontalalign="Center" width="200px" />
                                <footerstyle horizontalalign="Right" />                                        
                            </asp:BoundField>
                            <asp:BoundField DataField="LocaleId"  HeaderText="LocaleId" meta:resourcekey="BoundField2">
                                <itemstyle  horizontalalign="Right" />
                                <headerstyle horizontalalign="Right" />
                                <footerstyle horizontalalign="Right" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:label runat="server" ID="dgResources_Label" meta:resourcekey="dgResources_label">Test</asp:label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            </Columns>
                  </asp:GridView>
        <br />
    </div>
        <loc:DbResourceControl ID="DbResourceControl1" runat="server" 
             ShowIconsInitially="Show" Visible="true"/>
    
    <p>
&nbsp;</p>
   


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
