/// <reference path="~/scripts/jquery.js" />
/// <reference path="~/scripts/ww.jquery.js" />

var localRes = null;
var panelNewResource;
var panelRename;
var panelTranslate;
var panelRenameResourceSet;
var lstResourceIds;
var txtResSearch;

$(document).ready(function() {
    panelNewResource = $("#panelNewResource");
    panelRename = $("#panelRename");
    panelTranslate = $("#panelTranslate");
    panelRenameResourceSet = $("#panelRenameResourceSet");
    lstResourceIds = $("#lstResourceIds");
    txtResSearch = $("#txtResSearch");

    if (lstResourceIds.get(0).options.length > 0)
        GetValue();

    txtResSearch.keypress(function(e) {
        var entered = txtResSearch.val().toLowerCase();
        var list = lstResourceIds.get(0);

        if (e.keyCode == 13 || e.keyCode == 9) {
            $("#txtValue").focus();
            e.preventDefault();
            e.stopPropagation();
            return false;
        }

        for (var i = 0; i < list.options.length; i++) {
            var opt = list.options[i];
            var optval = opt.value.toLowerCase();

            if (entered.length > optval.length)
                continue;

            if (entered == optval || entered == optval.substr(0, entered.length)) {
                if (!opt.selected) {
                    opt.selected = true;
                    GetValue();
                }
                return;
            }
        }
    }).keyup(function(e) {
        if (e.keyCode == 27) {
            $(this).val("");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
    });

});

function ShowNewResourceDisplay(clearFields)
{
    var Ctl = panelNewResource;    
    Ctl.show().closable({ cssClass: "closebox-container" });
    
    var ResourceId = lstResourceIds.val();
    if (ResourceId == null || ResourceId == "")
       ResourceId = $("#lstResourceIds").get(0).options[0].value;
       
    var Language = $('#lstLanguages').val();

    if (!clearFields) {
        $("#txtNewLanguage").val(Language);
        $("#txtNewResourceId").val(ResourceId);
        $("#txtNewResourceSet").val($("#lstResourceSet").val());
    }
    else {
        $("#txtNewLanguage").val("");
        $("#txtNewValue").val("");
        $("#txtNewResourceId").val("");
        $("#txtNewResourceSet").val("");
    }

}
function ShowRenameResourceDisplay() {
    panelRename.show();
    
    var ResourceId = lstResourceIds.val();
    if (ResourceId == null || ResourceId == "")
       ResourceId = lstResourceIds.get(0).options[0].value;
       
    $("#txtResourceToRename").val(ResourceId);
    $("#txtRenamedResource").val(ResourceId);    
}
function ShowTranslationDisplay() {

    var Ctl1 = $("#txtValue");
    var Ctl2 = $("#txtTranslationInputText");
    Ctl2.val( Ctl1.val() );

    Ctl1 = $("#lstLanguages");
    Ctl2 = $("#txtTranslateFrom");        
    
    var Text = Ctl1.val();
    if (Text == "")
       Text = "en";
    if (Text.length > 2)
       Text = Text.substring(0,2);
   
    Ctl2.val(Text);

    $("#txtGoogle").val("");
    $("#txtBing").val("");
                    
    panelTranslate.show().closable({cssClass: "closebox-container"});

    // center on first display - after that keep page position
    var pos = panelTranslate.position(); 
    if (pos.left == 0 && pos.top == 0)
        panelTranslate.centerInClient();
}
function ShowResourceSetRenameDisplay()
{
    var OldResourceSet = $("#lstResourceSet").val();
    if (OldResourceSet == null || OldResourceSet == "")
       return;
       
    $("#txtOldResourceSet").val( OldResourceSet );
    
    panelRenameResourceSet.show().centerInClient();
}
function HideResourceSetRenameDisplay()
{
      panelRenameResourceSet.hide(); 
}
function PreviewPanelDisplay(Show)
{
    if (Show)
        $("#divValues").show();
    else
        $("#divValues").hide();
}
function GetValue()
{            
    var ResourceId = lstResourceIds.val();
    if (ResourceId == null || ResourceId == "")
       ResourceId = lstResourceIds.get(0).options[0].value;

    var ResourceSet = $("#lstResourceSet").val();
           
    var LocaleId = $("#lstLanguages").val();
    if (LocaleId == null)
       LocaleId = "";

   // *** Also load the preview list
   GetResourceStrings();

   Callback.GetResourceItem(ResourceId, ResourceSet, LocaleId,
        function(result) {
            if (result == null)
                result = { Value: ""};

            $("#txtValue").val(result.Value);
            $("#txtComment").val(result.Comment);
            if (result.Comment)
                $("#btnShowComment").css("font-weight", "bold");
            else
                $("#btnShowComment").css("font-weight", "normal");
        });
}
function GetResourceStrings()
{
    var resourceId = lstResourceIds.val();
    if (resourceId == null || resourceId == "")
       resourceId = lstResourceIds.get(0).options[0].value;

    var resourceSet = $("#lstResourceSet").val();

    Callback.GetResourceStrings(resourceId, resourceSet,
        function(resources) {
            // returns an array with key/value prop objects
            if (resources == null) {
                this.ShowMessage(ResC("NoResourcesAvailable"));
                return;
            }

            var el = $("<div>").addClass("resourcestringheader").text(resourceId);
            var divValues = $("#divValues");
            divValues.empty().append(el);
            
            for (var x = 0; x < resources.length; x++) {
                var resource = resources[x];
                if (resource.key == "")
                    resource.key = "Invariant";

                var resItem = "<div class='resourcestringitem'><a href='javascript:ChangeLanguage(\"" + resource.key + "\");' >" + resource.key + "</a>: " + resource.value + "</div>";
                divValues.append(resItem);
            }

            PreviewPanelDisplay(true);
        });
}    
function ChangeLanguage(lang)
{
    if (lang == "Invariant")
        lang = "";
    var list = $("#lstLanguages");
    list.val(lang);  
    list.change();
}
function SaveValue()
{
    var Value = $("#txtValue").val();
    var Comment = $("#txtComment").val();
    
    if ( (Value == null || Value == "") && !confirm(ResC("AreYouSureYouWantToRemoveValue") ) )
        return;

    if (!Comment)
        Comment = null;
    
    var ResourceId = lstResourceIds.val();
    if (ResourceId == null || ResourceId == "")
       ResourceId = lstResourceIds.get(0).options[0].value;

    var ResourceSet = $("#lstResourceSet").val();

    var LocaleId = $("#lstLanguages").val();
    if (LocaleId == null)
       LocaleId = "";
           
    Callback.UpdateResourceWithComment(Value,Comment,ResourceId,ResourceSet,LocaleId,SaveValue_Callback,OnPageError); 
    
    SetResourceIdValue = ResourceId;    
}
function SaveValue_Callback(result)
{
    if (result)    
    {
        ShowMessage(ResC("ResourceUpdated") ,5000);            
            
        // *** Select item
        if (SetResourceIdValue) {            
            lstResourceIds.val(SetResourceIdValue);
            GetValue(); 
            SetResourceIdValue = null;
        }
    }  
    else
        ShowMessage(ResC("ResourceUpdateFailed") ,5000); 
   

}
function AddResource(keepOpen) {    
    var Value = $("#txtNewValue").val();    
    
    if ( (Value == null || Value == "") )
    {
        alert(ResC("NoValueEntered") );
        return;
    }
    
    var ResourceId = $("#txtNewResourceId").val();
    if ( (ResourceId == null ||  ResourceId == "") )
    {
        alert(ResC("NoValueEntered") );
        return;
    }

    var ResourceSet = $("#txtNewResourceSet").val();
           
    var LocaleId = $("#txtNewLanguage").val();
    if (LocaleId == null)
       LocaleId = "";

   // *** Force list to update
   SetResourceIdValue = ResourceId;

   Callback.UpdateResourceString(Value, ResourceId, ResourceSet, LocaleId,
    function(result) {
        if (result) {
            ShowMessage(ResC("ResourceUpdated"), 5000);

            // if (the resource doesn't exist yet in the resource list add it
            var found = false;
            list = lstResourceIds.get(0);
            for (var i = 0; i < list.options.length; i++) {

                var opt = list.options[i];
                
                // already exists - don't add
                if (opt.value.toLowerCase() == SetResourceIdValue.toLowerCase()) {
                    opt.selected = true;
                    break;
                }
                // doesn't exist - add a new list item
                else if (opt.value.toLowerCase() > SetResourceIdValue.toLowerCase()) {

                    var newOpt = $("<option>" + SetResourceIdValue + "</option>");
                    $(opt).before(newOpt);
                    newOpt.get(0).selected = true;
                    lstResourceIds.change();
                    break;
                }
            }

            GetResourceStrings(); // redraw list
            //GetResourceList();

            if (!keepOpen)
                panelNewResource.hide();
        }
        else
            ShowMessage(ResC("ResourceUpdateFailed"), 5000);
    }, OnPageError);        
}

function DeleteResource() {
    var ResourceId = $("#lstResourceIds").val();
    if (!ResourceId)
        ResourceId = $("#lstResourceIds").options[0].value;

    var ResourceSet = $("#lstResourceSet").val();

    var LocaleId = $("#lstLanguages").val();
    if (LocaleId == null)
        LocaleId = "";

    Callback.DeleteResource(ResourceId, ResourceSet, LocaleId,
        function(result) {
            if (result) {
                ShowMessage("Resource has been deleted", 3000);
                GetResourceStrings();
                GetResourceList();
                $("#txtValue").val("");
            }
        }, OnPageError);
}
function RenameResource()
{
    var ResourceId = $("#txtResourceToRename").val();
    if (ResourceId == null || ResourceId == "")
    {
       ShowError( ResC("InvalidResourceId")  );
       return; 
    }
    
    var RenameProperty = $("#chkPropertyRename").checked;
    
    var ResourceSet = $("#lstResourceSet").val();    
    var NewResourceId = $("#txtRenamedResource").val();
    
    if (RenameProperty)
    {    
        Callback.RenameResourceProperty(ResourceId,NewResourceId,ResourceSet,RenameResource_Callback,OnPageError);
        
        // *** Force list to update
        SetResourceIdValue = NewResourceId + ".Text";
    }
    else
    {
        Callback.RenameResource(ResourceId,NewResourceId,ResourceSet,RenameResource_Callback,OnPageError);
        
        // *** Force list to update
        SetResourceIdValue = NewResourceId;
    }
}
function RenameResource_Callback(result)
{
    panelRename.hide();         

    if (result)
    {
        ShowMessage(ResC("ResourceUpdated") ,5000);
        GetResourceList();
    }
    else
        ShowMessage(ResC("ResourceUpdateFailed") ,5000);
   
}
var SetResourceIdValue = null;
function GetResourceList()
{
    var ResourceSet = $("#lstResourceSet").val();
    Callback.GetResourceList(ResourceSet,
        function(table) {            
            // *** Callback result is a DataTable
            lstResourceIds
                .listSetData(table, { dataValueField: "resourceId", dataTextField: "resourceId" });

            // *** Select item
            if (SetResourceIdValue) {
                lstResourceIds
                    .listSelectItem(SetResourceIdValue);
                lstResourceIds.change();

                SetResourceIdValue = null;
            }

        }, OnPageError);
}
function DeleteResourceSet()
{
    var ResourceSet = $("#lstResourceSet").val();
    
    if (!confirm("Are you sure you want to delete this resource set?\r\n" + ResourceSet) )
       return;

   Callback.DeleteResourceSet(ResourceSet,
            function(result) {
                if (result) {
                    ShowMessage("ResourceSet deleted...", 5000);
                    var $el = $("#lstResourceSet option[value='" + ResourceSet + "']");
                    $el.remove();                
                }
                else
                    ShowMessage("ResourceSet deletion failed...", 5000);
            }, OnPageError);      
}
function Translate()
{
    var Value = $("#txtTranslationInputText").val();
    var From = $("#txtTranslateFrom").val();
    var To = $("#txtTranslateTo").val();
    
    if (To == From)
    {
       ShowMessage("Please select two separate languages to translate")
       return;
    }
    
    var Loading = ResC("Loading")   + "...";       
    $("#txtGoogle").val( Loading );
    $("#txtBing").val( Loading );
    
    // *** Use two separate instances here so there won't be any interference in callbackss   
    Callback.Translate(Value,From,To,"Google", 
        function(result) { $("#txtGoogle").val(result); },OnPageError);
    Callback.Translate(Value, From, To, "Bing",
        function(result) { $("#txtBing").val(result); },OnPageError);
}
function UseTranslation(Service)
{
    var Ctl = $("#txt" + Service);
    var Text = Ctl.val();
    if (Text == null || Text == "")
       return;
           
    Lang = $("#txtTranslateTo").val();
    
    var CtlValue = $("#txtValue");
    
    Ctl = $("#lstLanguages");
    
    if (Ctl.val() == Lang)
    {
        CtlValue.val(Text);
        HideTranslationDisplay();
        return;
    }
    
    for(var x = 0; x < Ctl.get(0).options.length; x++)
    {
        var Item = Ctl.get(0).options[x];
        if (Item.value == Lang )
        {
           Ctl.val(Lang);
           CtlValue.val(Text);
           HideTranslationDisplay();
           return;
        }
    }   
    
    ShowMessage(ResC("NoMatchingLanguage") );
}
function HideTranslationDisplay()
{
    panelTranslate.hide();
}
function ReloadResources() {
    Callback.ReloadResources(
        function(result) {
            ShowMessage(ResC("ResourcesReloaded"), 5000);
        }, OnPageError);
}
function Backup()
{
    if ( !confirm( ResC("BackupNotification")  ) )
        return;
        
    Callback.Backup(Backup_Callback,OnPageError);        
}
function Backup_Callback(result)
{
    if (result)
       ShowMessage(  ResC("BackupComplete") ,5000 );
    else
       ShowMessage( ResC("BackupFailed")  ,5000);    
}
// look up Local Resources embedded into the page (JSON Serialization from server page)
function ResC(resId) {    
     if (!localRes)
        return resId;
    
    if (localRes[resId])
        return localRes[resId];
        
    return resId;
}
function OnPageError(Error) {    
    ShowError(Error.message,7000);
}
function ShowMessage(Message,Timeout)
{

    var Ctl = $("#lblMessages");
    if (Message == null || Message == "")
       Message = " ";
    Ctl.text(Message);
    
    if (Timeout)
    {
        Ctl.addClass("statusbarhighlight");
        setTimeout(HideMessage,Timeout);
    }
}

function ShowError(Message,Timeout)
{
    var Ctl = $("#lblMessages");
    Ctl.text(Message);
    
    if (Timeout)
    {
        Ctl.addClass("statusbarhighlight");
        setTimeout(HideMessage,Timeout);
    }
}

function HideMessage()
{
    $("#lblMessages")
        .text(ResC("Ready"))
        .removeClass("statusbarhighlight");
}

