<%@ Page Language="C#" AutoEventWireup="True" EnableViewState="false"   
         Codebehind="StronglyTypedGlobalResources.aspx.cs"
         Inherits="Westwind.GlobalizationWeb.LocalizationAdmin_StronglyTypedGlobalResources" 
         Culture="auto" meta:resourcekey="Page" 
         UICulture="auto"  
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Generate Strongly Typed Resources</title>
    <link href="LocalizeAdminForm.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    
    <div class="gridheaderbig" style=""><asp:Label ID="lblHeader" runat="server" Text="Create Strongly Typed Global Resources" meta:resourcekey="lblHeader"></asp:Label></div>
    
    <div class="toolbarcontainer">
        <asp:HyperLink runat="server" ID="lnkAdmin" Text="Resource Administration" NavigateUrl="~/LocalizationAdmin/default.aspx" meta:resourcekey="lnkAdmin" class="hoverbutton"></asp:HyperLink> | 
        <asp:LinkButton runat="server" ID="lnkRefresh"  meta:resourcekey="lnkRefresh" class="hoverbutton">
            Refresh
        </asp:LinkButton>
    </div>
    
    <div class="containercontent">
        
    <asp:Localize runat="server" ID="lblWelcomeMessage" meta:resourcekey="lblWelcomeMessage" Text="&#13;&#10;    This page allows you to create one or more strongly typed resource classes from your global resources.&#13;&#10;    Please specify an output file path for the resulting class for this project. Use ASP.NET style syntax&#13;&#10;    to specify.&#13;&#10;    "></asp:Localize>
   <br />
   <br />
            <asp:Label ID="lblOutputFile" runat="server" 
                       Text="Output File Location:  (.cs or .vb file - project relative)" 
                       meta:resourcekey="lblOutputFile"></asp:Label>
                       
            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;
            
            <asp:RadioButtonList ID="lstExportFrom" runat="server" 
                                 RepeatDirection="Horizontal" 
                                 RepeatLayout="Flow" meta:resourcekey="lstExportFrom">
                <asp:ListItem Selected="True" meta:resourcekey="ListItem">ResX</asp:ListItem>
                <asp:ListItem meta:resourcekey="ListItem1">DbResourceManager</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            
            <asp:TextBox ID="txtOutputFile" runat="server" 
                         style="width: 675px" 
                         meta:resourcekey="txtOutputFile">~/_Classes/Resources.cs</asp:TextBox>
             <br />
                         
            <asp:Button ID="btnGenerate" runat="server" Text="Generate" OnClick="btnGenerate_Click" meta:resourcekey="btnGenerate" />        
            
            <hr />
            
            <div class="errordisplay" class="padding: 15px">
                <pre>            
                <asp:Label ID="lblGenetatedCode" runat="server"                       
                            style="font-family:Monospace;font-size:10pt;color:darkblue;" meta:resourcekey="lblGenetatedCode"></asp:Label>
                </pre>
            </div>
            <br />
    </div>
    </form>
<body>
</html>
