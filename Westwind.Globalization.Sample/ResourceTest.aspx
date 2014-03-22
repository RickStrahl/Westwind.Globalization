<%@ Page Language="C#" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <style>
        label { display: block; margin-top: 10px; margin-bottom: 0 }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <h1>DbResourceManager Simple Resource Test</h1>        
    <div class="container">
        <label>Get GlobalResource Object (default locale):</label>
        <%= GetGlobalResourceObject("CommonPhrases","Yesterday") %>
        
        <label>Using DbRes Direct Access Provider (default locale):</label>
        <%= DbRes.T("Yesterday","CommonPhrases") %>
        
       <label>Using DbRes Force to German:</label>
        <%= DbRes.T("Yesterday","CommonPhrases","de") %>
    </div>
    </form>
</body>
</html>
