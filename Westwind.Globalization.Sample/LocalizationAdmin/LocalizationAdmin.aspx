<%@ Page Language="C#" AutoEventWireup="true" 
         Codebehind="LocalizationAdmin.aspx.cs" 
         Inherits="Westwind.GlobalizationWeb.LocalizeAdmin" 
         EnableViewState="false" 
         ValidateRequest="false"          
         Theme="" 
         Culture="auto" meta:resourcekey="Page1" 
         UICulture="auto"       
%>
<script runat="server">
    // Allow hiding of 'Write Operations' Menu Items
    const bool safeMode = false;    
</script>
<%@ Register Assembly="Westwind.Globalization" Namespace="Westwind.Globalization" TagPrefix="ww" %>
<%@ Register Assembly="Westwind.Web.WebForms" Namespace="Westwind.Web.Controls" TagPrefix="ww" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Application Resource Localization</title>    
    <link href="LocalizeAdminForm.css" rel="stylesheet" type="text/css" /> 
    <style>
        #lstResourceIds option {
            padding: 3px;
        }
    </style>
</head>
<body>    
    <form id="form1" runat="server">    
    <br />        
        <ww:ErrorDisplay ID="ErrorDisplay" runat="server" 
                    DisplayTimeout="5000" width="600px" 
                    meta:resourcekey="ErrorDisplay" 
                    UseFixedHeightWhenHiding="False" />        
                
    <center>
    <div class="blackborder" style="background:#eeeeee;width:960px;text-align:left;">
    
    <div class="gridheader" style="font-size:large">
        <asp:Label runat="server" id="lblPageHeader" Text="Resource Localization" 
                   Title="Resource List (Alt-L)" meta:resourcekey="lblPageHeader" />
    </div>

    <div style="width: 300px; float: left;padding: 3px 10px 0 3px">    
           <asp:TextBox runat="server" ID="txtResSearch"  AccessKey="E" PlaceHolder="Search" ToolTip="Search (Alt-E)" meta:resourcekey="txtResSearch"
                         style="width: 290px; margin: 5px 0px; border-collapse: collapse; padding-right: 5px;background-image: url(images/search.gif); background-position: right; background-repeat: no-repeat;" />
            <br />
                 
             <asp:ListBox runat="server" ID="lstResourceIds"  
                          style="Height:535px;width:300px;padding: 3px;" 
                          onchange="GetValue();" AccessKey="L" meta:resourcekey="lstResourceIds" >
             </asp:ListBox>
    </div>

    <div style="margin-left: 310px;">
        
            <div id="Toolbar" class="toolbarcontainer">
           
                <!-- WRITE options can be disabled with the safeMode switch -->
                <div id="divToolbar" style="margin-bottom: 10px;" class="safemode">            
                    <span class="hoverbutton">
                        <img runat="server" id="imgExportResources" />
                        <asp:LinkButton runat="server" ID="btnExportResources" Title="Export to Resource Files" Text="Export to Resource Files"  
                                        OnClick="btnExportResources_Click" OnClientClick="if (!confirm(ResC('ResourceGenerationConfirmation'))) return false;" meta:resourcekey="btnExportResources" />
                     </span>
            
                    <span class="hoverbutton"> 
                        <img runat="server" id="imgImport"  />
                        <asp:LinkButton runat="server" ID="btnImport" Title="Import Resources" Text="Import" onclick="btnImport_Click" meta:resourcekey="btnImport" />
                    </span>                 
                      
                    <span class="hoverbutton">
                        <img src="images/codegen.png" />
                        <asp:LinkButton runat="server" id="btnGenerateStronglyTypedResources" Text="Generate Strong Resources" 
                                        meta:resourcekey="btnGenerateStronglyTypedResources"
                                        onclick="btnGenerateStronglyTypedResources_Click" />
                    </span>            
                                
                    <span class="hoverbutton" >
                        <img runat="server" id="imgCreateTable" visible="false" />
                        <asp:LinkButton runat="server" ID="btnCreateTable" Title="Create" Text="Create Table" OnClick="btnCreateTable_Click"  meta:resourcekey="btnCreateTable" />
                    </span>
                </div>


                <div id="divToolbar2" >
                 <span class="hoverbutton">            
                    <img runat="server" id="imgRefresh" />
                    <asp:LinkButton runat="server" ID="btnRefresh" Title="Refresh Page" Text="Refresh Page" meta:resourcekey="btnRefresh"></asp:LinkButton>                    
                </span>
            
                <span class="hoverbutton">            
                    <img runat="server" id="imgRecycleApp" />
                    <asp:LinkButton runat="server" ID="btnRecycleApp" Title="Recycle Application" AccessKey="R" Text="Recyle App" OnClientClick="ReloadResources();return false;" meta:resourcekey="btnRecycleApp"></asp:LinkButton>&nbsp;&nbsp;
                </span>
            
                <span class="hoverbutton">            
                    <img runat="server" id="imgBackup" />
                    <asp:LinkButton runat="server" ID="btnBackup" Title="Backup Resource Database" Text="Backup Table" OnClientClick="Backup();return false;" meta:resourcekey="btnBackup"></asp:LinkButton>
                </span>
            
                </div>
            </div>
            <hr /> 

            <style>
                #ResourceSetBlock a.hoverbutton {
                    margin-left: 2px; margin-right: 2px;
                }
            </style>
            <div id="ResourceSetBlock">
                <b><asp:Label runat="server" id="lblResourceSetLabel" Text="ResourceSet:" meta:resourcekey="lblResourceSetLabel"></asp:Label></b>
                <br />
            
                <asp:DropDownList runat="server" ID="lstResourceSet" width="450px" AutoPostBack="True" meta:resourcekey="lstResourceSet"></asp:DropDownList>
                
                <a href="javascript:ShowNewResourceDisplay(true);" class="hoverbutton"><asp:Image  runat="server" ID="imgAddResourceSet" Title="Add ResourceSet" meta:resourcekey="imgAddResourceSet" /></a>
                <a href="javascript:ShowResourceSetRenameDisplay();" class="hoverbutton"><asp:Image  runat="server"  ID="imgRenameResourceSet" Title="Rename ResourceSet" meta:resourcekey="imgRenameResourceSet" /></a>
                <a href="javascript:DeleteResourceSet();" class="hoverbutton"><asp:Image  runat="server"  ID="imgDeleteResourceSet" Title="Delete ResourceSet" meta:resourcekey="imgDeleteResourceSet" /></a>
            </div>
            
            <br />

            <asp:Label runat="server" ID="lblLanguages" Font-Bold="True" meta:resourcekey="lblLanguages" Text="Languages:"></asp:Label>
            <br />
            <asp:ListBox ID="lstLanguages" runat="server" style="" Height="90px" Width="605px" onchange="GetValue()" meta:resourcekey="lstLanguages">
            </asp:ListBox>
            <br />
            <br />
        
            <div>
                
            </div>
             

            <asp:Label runat="server" ID="lblValue" Font-Bold="True" meta:resourcekey="lblValue" Text="Value:"></asp:Label>
            <br />
            <asp:TextBox runat="server" ID="txtValue" TextMode="MultiLine" Height="75px" Width="600px" AccessKey="V" Tooltip="Resource Value (Alt-V)" meta:resourcekey="txtValue"></asp:TextBox>
            <br />
            <asp:Button runat="server" id="btnSaveValue" Text="Save" UseSubmitBehavior="False" AccessKey="S" OnClientClick="SaveValue(); return false;" meta:resourcekey="btnSaveValue" />
            
            <asp:Button runat="server" ID="btnAddResourceDisplay"  Text="Add" OnClientClick="ShowNewResourceDisplay();return false;" AccessKey="a" meta:resourcekey="btnAddResourceDisplay"  />
            <asp:Button runat="server" ID="btnDelete"  Text="Delete" OnClientClick="DeleteResource();return false;" accesskey="d" meta:resourcekey="btnDelete" />
            <asp:Button runat="server" ID="btnRename"  Text="Rename" OnClientClick="ShowRenameResourceDisplay();return false;" meta:resourcekey="btnRename" />
            <asp:Button runat="server" id="btnTranslate" Text="Translate" OnClientClick="ShowTranslationDisplay();return false" meta:resourcekey="btnTranslate" />
            &nbsp; &nbsp;&nbsp; &nbsp;
            <asp:Button runat="server" ID="btnShowComment" Text="Comment" meta:resourcekey="btnShowComment"  onclientClick="$('#panelComment').show(); return false;"  />
            <br />
            <br />            
            <div id="divValues">
            
            </div>            
    </div>
    <div class="clearfix"></div>
    
    <div id="lblMessages" class="dialog-statusbar">    
        <%= WebUtils.LRes("Ready") %> 
    </div> 
    
    
    </div>  
    </center>

    <%-- panelComment  --%>
    <ww:DragPanel runat="server" ID="panelComment"
         CssClass="dialog" 
         DragHandleID="panelCommentHeader"
         Closable="true" 
          ShadowOffset="5"
         style="background: white; position: absolute; left: 480px; top: 320px; display: none;"
         
     >
        <div id="panelCommentHeader" class="dialog-header"><asp:Label runat="server" ID="lblComment" Text="Comments" meta:resourcekey="txtComment" /></div>         
        <asp:TextBox runat="server" ID="txtComment" TextMode="MultiLine" style="height: 150px; width: 350px;"></asp:TextBox>               
    </ww:DragPanel>   
    <%-- End panelComment  --%>    
     
     <%--New Resource Panel--%>
     <ww:DragPanel ID="panelNewResource" runat="server" CssClass="dialog" 
                     style="position: absolute; top:200px;left:180px;width:530px;display:none;"                       
                     ShadowOffset="5"
                     DragHandleId="NewResourceHeader"
                     meta:resourcekey="panelNewResource">
        <div class="dialog-header"  runat="server" id="NewResourceHeader">        
        <asp:Label runat="server" ID="lblNewResourceHeader" Text="Add Resource" meta:resourcekey="lblNewResourceHeader"></asp:Label>
        </div>
        <div class="dialog-content">
            <table>
            <tr>
                <td><asp:Label runat="server" ID="lblNewLanguage" Text="Lang:" meta:resourcekey="lblNewLanguage"></asp:Label></td>
                <td>&nbsp;&nbsp;&nbsp;<asp:Label runat="server" ID="lblNewResourceId" Text="Resource Id:" meta:resourcekey="lblNewResourceId"></asp:Label></td></tr>
            <tr>
                <td><asp:TextBox ID="txtNewLanguage" runat="server" Width="56px" meta:resourcekey="txtNewLanguage" /></td>
                <td>&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtNewResourceId" runat="server" Width="390px" meta:resourcekey="txtNewResourceId" /></td>
            </tr>
            </table>
            <br />
          
            <asp:Label ID="lblNewValue" runat="server" Text="Value:" meta:resourcekey="lblNewValue"></asp:Label><br />
            <asp:TextBox ID="txtNewValue" runat="server" TextMode="MultiLine" Width="480px" meta:resourcekey="txtNewValue"></asp:TextBox><br />
            <br />
            <asp:Label ID="lblNewResourceSet" runat="server" Text="ResourceSet:" meta:resourcekey="lblNewResourceSet"></asp:Label>
            <asp:TextBox runat="server" ID="txtNewResourceSet" Width="480px" meta:resourcekey="txtNewResourceSet"></asp:TextBox>
            
            <asp:Button runat="server" ID="btnAddValue" Text="Add Resource" onClientClick="AddResource();return false;" meta:resourcekey="btnAddValue" />
            <asp:Button runat="server" ID="btnAddValueAndKeepOpen" Text="Add and keep open" onClientClick="AddResource(true);return false;"  meta:resourcekey="btnAddValueAndKeepOpen" />            
            
            <hr />
            <asp:Label runat="server" ID="lblFileUpload" Text="File Resource Upload:" meta:resourcekey="lblFileUpload"></asp:Label><br />
            <asp:FileUpload ID="FileUpload" runat="server" style="width:410px"  meta:resourcekey="FileUpload" />
            <asp:Button runat="server" ID="btnFileUpload" OnClick="btnFileUpload_Click" Text="Upload" meta:resourcekey="btnFileUpload"/>    
        </div>                
    </ww:DragPanel> 
    <%--End New Resource Panel--%>
    
    <%--Rename Resource Panel--%>
    <ww:DragPanel ID="panelRename" runat="server" CssClass="dialog gridalternate" DragHandleID="Header2"  Closable="true"
               style="position: absolute; top:230px;left:140px;width:330px;display:none;" meta:resourcekey="panelRename1"  ShadowOffset="5" >
        <div class='dialog-header' runat="server" id="Header2">        
        <asp:Label runat="server" ID="lblRenameResourceHeader" Text="Rename Resource" meta:resourcekey="lblRenameResourceHeader" ></asp:Label></div>
        <div style="padding:5px;">
        <b><asp:Label runat="server" ID="lblResourceToRename_Label" meta:resourcekey="lblResourceToRename_Label" Text="ResourceKey to Rename:"></asp:Label></b><br />
        <asp:TextBox runat="server" ID="txtResourceToRename" width="300px" meta:resourcekey="txtResourceToRename"></asp:TextBox>
        <br />
        <br />
        <b><asp:Label runat="server" ID="lblRenamedResource" meta:resourcekey="lblRenamedResource" Text="ResourceKey to Rename:"></asp:Label></b><br />
        <asp:TextBox runat="server" ID="txtRenamedResource" Width="300px" meta:resourcekey="txtRenamedResource"></asp:TextBox><br />
        <asp:CheckBox ID="chkPropertyRename" runat="server" Text="Rename all properties for key"
                Width="300px" meta:resourcekey="chkPropertyRename"/><br />
        <hr />
        <asp:Button runat="server" ID="btnRenameResourceKey" Text="Rename" UseSubmitBehavior="False" OnClientClick="RenameResource();return false;" meta:resourcekey="btnRenameResourceKey" />
        </div>
    </ww:DragPanel>                
    <%--End Rename Panel--%>       
    
    <%--  Add ResourceSet Panel--%>
    <ww:DragPanel runat="server" id="panelAddResourceSet" 
                  CssClass="dialog gridalternate" 
                  DragHandleID="panelAddResourceSetHeader"                  
                  style="display: none;"
                  >
        <div id="panelAddResourceSetHeader" class="dialog-header">Add new ResourceSet</div>
        <div class="containercontent>
            <asp:Label runat="server" ID="lblNewResourceSetLabel" Text="New Resource Set Name" meta:resourcekey="lblNewResourceSetLabel" />:
            <br />
            <asp:TextBox runat="server" ID="txtNewResourceSetName" Text=""  width="300px" />
            <hr />
            <asp:Button runat="server" ID="btnAddResourceSet" 
                        Text="Add ResourceSet"
                        meta:resourcekey="btnAddResourceSet"   />                    
            
        </div>
    </ww:DragPanel>
    
    <%-- Translate Panel --%>
    <ww:DragPanel ID="panelTranslate" runat="server" CssClass="dialog" 
               DragHandleID="TranslateHeader" ShadowOffset="5"
               CloseBoxImage="../css/images/closeX.png"
               style="position: absolute; top: 150px;left:10px;width:400px;display:none;" meta:resourcekey="panelTranslate">
        <div class='dialog-header' runat="server" id="TranslateHeader">
        <%--<div style="float:right;color:White;font-size:15px;font-weight:bolder;color:White;" onclick="HideTranslationDisplay();" title="Close Window">X</div>--%>
        <asp:label runat="server" ID="lblTranslateHeader" Text="Web Service Translation" meta:resourcekey="lblTranslateHeader"></asp:label>
        </div>
        <div class="dialog-content">
        <asp:Label runat="server" ID="lblTranslateFrom" Text="From:" meta:resourcekey="lblTranslateFrom"></asp:Label>
            <asp:TextBox runat="server" ID="txtTranslateFrom" Text="en" Width="40px" meta:resourcekey="txtTranslateFrom"></asp:TextBox>&nbsp;
            <asp:Label runat="server" ID="lblTranslateTo" Text="To:" meta:resourcekey="lblTranslateTo"></asp:Label>
            <asp:TextBox runat="server" ID="txtTranslateTo" Text="de" Width="40px" meta:resourcekey="txtTranslateTo"></asp:TextBox>
            <br />
            <br />
            <asp:Label runat="server" ID="lblTranslationInputText" Text="Input Text:" meta:resourcekey="lblTranslationInputText" ></asp:Label><br />
            <asp:TextBox runat="server" ID="txtTranslationInputText" TextMode="MultiLine" Width="300px" meta:resourcekey="txtTranslationInputText"></asp:TextBox>
            <input type="button" runat="server" ID="btnTranslateSubmit" value="Go" onclick="Translate()"/>
            <hr />                    
            <asp:HyperLink runat="server" ID="lblGoogle" Text="Google Translation:" Target="_Translate" 
                           NavigateUrl="http://www.google.com/translate_t" meta:resourcekey="lblGoogle" />
            <asp:TextBox runat="server" ID="txtGoogle" TextMode="MultiLine" Width="300px" meta:resourcekey="txtGoogle"></asp:TextBox>
            <input type="button" runat="server" ID="btnUseGoogle" value="Use"  onclick="UseTranslation('Google');"/>
            <br />            
            <asp:HyperLink runat="server" ID="lblBing" Text="Bing Translation:" Target="_Translation" NavigateUrl="http://www.bing.com/translator" meta:resourcekey="lblBabelFish" style="margin-top:8px; display:block;"/>
            <asp:TextBox runat="server" ID="txtBing" TextMode="MultiLine" Width="300px" meta:resourcekey="txtBabelFish" ></asp:TextBox>
            <input runat="server" type="button" ID="btnBing" value="Use"  onclick="UseTranslation('Bing');"/>
        </div>
    </ww:DragPanel>        
    <%-- End Translate Panel --%>
    
    <%--  Rename Resource Set Window --%>
    <ww:DragPanel ID="panelRenameResourceSet" runat="server" 
               CssClass="dialog gridalternate" 
               Closable="true" 
               ShadowOffset="5"
               DragHandleId="RenameResourceSetHeader"
               style="position:absolute;top:135px;left:300px;width:365px;display:none;font-size:8pt;" meta:resourcekey="panelRenameResourceSet">
        
        <div runat="server" id="RenameResourceSetHeader" class='dialog-header'>
        <%--<div style="float:right;color:White;font-size:15px;font-weight:bolder;color:White;" onclick="HideResourceSetRenameDisplay();" title="Close Window">X</div>--%>
        <asp:label runat="server" ID="lblRenameResourceSetHeader" Text="Rename ResourceSet" meta:resourcekey="lblRenameResourceSetHeader"></asp:label>
        </div>
        <div style="padding:15px">
            <asp:Label runat="server" ID="lblOldResourceSet" Text="Old ResourceSet:" meta:resourcekey="lblOldResourceSet"></asp:Label><br />
            <asp:TextBox runat="server" id="txtOldResourceSet" width="300px" meta:resourcekey="txtOldResourceSet"></asp:TextBox>              
            <br />
            <br />
            <asp:Label runat="server" ID="lblRenamedResourceSet" Text="New ResourceSet:" meta:resourcekey="lblRenamedResourceSet"></asp:Label><br />
            <asp:TextBox runat="server" id="txtRenamedResourceSet" width="300px" meta:resourcekey="txtRenamedResourceSet"></asp:TextBox>                                      
            <hr />
            <asp:Button runat="server" ID="btnRenameResourceSet" Text="Rename ResourceSet" onclick="btnRenameResourceSet_Click" meta:resourcekey="btnRenameResourceSet" />
         </div>
    </ww:DragPanel>
    <%--  End Rename Resource Set Window --%>
 
    
    <ww:DbResourceControl id="ResourceAdmin" runat="server" meta:resourcekey="ResourceAdmin" 
                            CssClass="errordisplay" Width="250px" 
                            ShowIconsInitially="DontShow"
                            visible="true"> 
    </ww:DbResourceControl>
    <br />
        
    
                
    <!-- Page Scripts to be loaded. REQUIRED to get load order correct -->
    <ww:ScriptContainer runat="server" ID="ScriptContainer">
        <Scripts>
            <script src="~/scripts/jquery.js" rendermode="HeaderTop"></script>
            <script src="~/scripts/ww.jQuery.js" rendermode="Header"></script>
            <script src="LocalizeAdminForm.js" type="text/javascript" rendermode="Header"></script> 
        </Scripts>
    </ww:ScriptContainer>

    <!-- The Ajax Callback control that drives all the method callbacks to page methods -->
    <ww:AjaxMethodCallback ID="Callback" runat="server" PageProcessingMode="PageInit"  />   
    
    <%--<div id="divDebug" class="errordisplay" style="font: normal normal 8pt;"></div> --%>
    
    <script>
        var safeMode = <%= safeMode.ToString().ToLower() %>;
        if (safeMode) {
            $(".safemode").hide();
        }
    </script>
    </form>
</body>
</html>
