<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResourceEditingWebForms.aspx.cs"
     Inherits="Westwind.Globalization.Sample.ResourceEditingWebForms" ViewStateMode="Disabled" %>

<%@ Register assembly="Westwind.Globalization.Web" namespace="Westwind.Globalization" tagprefix="loc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>    
    <link href="LocalizationAdmin/bower_components/bootstrap/dist/css/bootstrap.css"
        rel="stylesheet" />
   <%-- <link href="LocalizationAdmin/css/localizationAdmin.css" rel="stylesheet" />   --%>
    <link href="LocalizationAdmin/bower_components/fontawesome/css/font-awesome.min.css"
        rel="stylesheet" />
    <style>            
        .resource-editor-icon, 
        .resource-editor-icon:hover,  
        .resource-editor-icon:visited {
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
            color: red;                      
        }
    </style>        
</head>
<body data-resource-set="ResourceEditingWebForms.aspx">
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
            <label runat="server" id="localizeControl_Label" meta:resourcekey="localizeControl_Label">Localize Control Tag (key lblHelloWorldLabel.Text):</label><br />
            <asp:Localize runat="server" ID="localizeControl" meta:resourcekey="localizeControl">
                This is some default text.
            </asp:Localize>
        </div>
            
        
        <div class="form-group">
            <label runat="server" id="literalControl_Label" meta:resourcekey="literalControl_Label">Literal Control Tag (key lblHelloWorldLabel.Text):</label>
            <br />
            <asp:Literal runat="server" id="literalControl" meta:resourcekey="literalControl">
                This is some default text.
            </asp:Literal>
        </div>
        
        <asp:Button runat="server" id="btnSubmit" meta:resourcekey="btnSubmit" 
            OnClientClick="return false;"
            Text="Toggle Resource Link Mode" />
        
                     

        
        <hr />
        
        <asp:GridView ID="dgResources" runat="server" Width="90%" AutoGenerateColumns="False" 
                                         cssclass="table table-striped" 
                                         EnableViewState="False"
                                         GridLines="None"
                                         ShowFooter="True"                                                                                   
                                         PageSize="10"
                                         AllowPaging="true"
                                         meta:resourcekey="dgOrders">
                                          
                 
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
                                    <asp:label runat="server" ID="dgResources_Label"
                                               meta:resourcekey="dgResources_Label">Test s</asp:label>
                                </ItemTemplate> 
                            </asp:TemplateField>
                            </Columns>
                  </asp:GridView>
        <br />
    </div>
    
<loc:DbResourceControl ID="DbResourceControl1" runat="server" EnableResourceLinking="true" /> 
        
<script src="LocalizationAdmin/bower_components/jquery/dist/jquery.min.js"></script>

<% if (DbResourceControl1.EnableResourceLinking) { %>        
<script src="LocalizationAdmin/scripts/ww.resourceEditor.js"></script>
<script>
    ww.resourceEditor.showResourceIcons({
        adminUrl: "./LocalizationAdmin/"
    });

    var editingResources = true;
    $("#btnSubmit").click(function() {
        editingResources = !editingResources;
        if (editingResources) {
            ww.resourceEditor.showResourceIcons({
                adminUrl: "./LocalizationAdmin/"
            });
        } else
            ww.resourceEditor.removeResourceIcons();
    });
</script>
<% } %>
        

    </form>
</body>
</html>
