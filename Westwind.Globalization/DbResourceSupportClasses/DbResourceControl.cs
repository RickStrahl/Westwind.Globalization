using System;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.IO;
using System.Reflection;
using Westwind.Utilities;
using Westwind.Web;
using Westwind.Web.Controls;


namespace Westwind.Globalization
{
    /// <summary>
    /// The DbResourceControl class provides Page level Resource Administration
    /// support to localizable ASP.NET pages. This control allows bringing up
    /// the localization Administration Form that shows all localizable resources
    /// for editing and translation.
    /// 
    /// The control also provides the ability to show icons next to each control
    /// to jump directly to the appropriate control in the Admin form. The control
    /// can automatically detect Page, Control, Master Page (any template control)
    /// resources and jump directly to the appropriate resource entry if it exists.
    /// 
    /// Note the control shows all Localizable controls, but there's no guarantee
    /// that the controls are actually hooked up for localization in the ASP.NET
    /// page, control, master etc. resource. You need to ensure either implicit
    /// or explicit resources are actually configured on the pages.
    /// 
    /// For the control to work it merely should be placed on any form that is
    /// localizable. Display of the control is globally controlled via the 
    /// DbResourceConfiguration object (and the DbResourceConfigurationSection in
    /// Web.config by default) which allows toggling display of the control in the UI
    /// and toggling the display of the individual resource icons.
    /// 
    /// The Administration form relies on the availability of the Administration
    /// form (LocalizeForm.aspx) and a configuration entry that points at this
    /// control. This form must be distributed with your Web application.
    /// </summary>
    [ToolboxData("<{0}:DbResourceControl runat=server />")]
    [Localizable(false)]
    public class DbResourceControl : CompositeControl
    {
        protected LinkButton btnEditResources = new LinkButton();
        protected CheckBox chkShowIcons = new CheckBox();

        protected bool ShowIcons = true;

        /// <summary>
        /// The default control constructor.
        /// </summary>
        public DbResourceControl()
        {
        }

        protected new bool DesignMode
        {
            get { return (HttpContext.Current == null); }
        }


        /// <summary>
        ///  Determines the initial state of the ShowLocalization Icons Text box.
        /// </summary>
        [Description("Determines the initial state of the ShowLocalization Icons Text box.")]
        [Category("Localization"), DefaultValue(typeof(ShowLocalizationStates), "InheritFromProvider")]
        public ShowLocalizationStates ShowIconsInitially
        {
            get { return _ShowLocalizationIcons; }
            set { _ShowLocalizationIcons = value; }
        }
        private ShowLocalizationStates _ShowLocalizationIcons = ShowLocalizationStates.InheritFromProvider;

        /// <summary>
        /// Optional override for Client OnLocalization Handler that when set is fired
        /// in response to a click on one of the localization icons. Gets passed
        /// an event object plus the resource name and resource set name.
        /// 
        /// If set OnLocalization is called first which in turn fires the handler
        /// of your choice.
        /// </summary>
        public string ClientOnLocalizationIconHandler
        {
            get { return _ClientOnLocalizationIconHandler; }
            set { 
                _ClientOnLocalizationIconHandler = value;
                // Don't allow OnLocalization which is the default handler
                // otherwise we end up with recursion
                if (_ClientOnLocalizationIconHandler == "OnLocalization")
                    _ClientOnLocalizationIconHandler = string.Empty;
            }
        }
        private string _ClientOnLocalizationIconHandler = string.Empty;

        
        protected override void OnInit(EventArgs e)
        {
            
            base.OnInit(e);            
        }
 

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack)
            {
                if (this.ShowIconsInitially == ShowLocalizationStates.DontShow)
                    this.ShowIcons = false;
                else if (this.ShowIconsInitially == ShowLocalizationStates.Show)
                    this.ShowIcons = true;
                else if (this.ShowIconsInitially == ShowLocalizationStates.InheritFromProvider)
                    ShowIcons = DbResourceConfiguration.Current.ShowControlIcons &&
                                     DbResourceConfiguration.Current.ShowLocalizationControlOptions;

                if (ShowIcons)
                    this.chkShowIcons.Checked = true;
            }
            else
            {
                this.ShowIcons = this.chkShowIcons.Checked;
            }
        }


        /// <summary>
        /// Event handler that is called and can be hooked when the Edit Resources
        /// option is clicked. This interface brings up the Administration form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEditResources(object sender, EventArgs e)
        {
            // Now redirect and display the resource items
            string Url = this.ResolveUrl(DbResourceConfiguration.Current.LocalizationFormWebPath);

            string ResourceSet = WebUtils.GetAppRelativePath();

            Url += "?ResourceSet=" + HttpUtility.UrlEncode(ResourceSet, Encoding.Default);

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "t", "window.open('" + Url + "','_Localization','width=850,height=650,resizable=yes');", true);
        }

        /// <summary>
        /// Event handler that is called when the Show Icons checkbox is
        /// checked or unchecked. The default behavior sets the current
        /// DbResourceConfiguration.ShowControlIcons setting which 
        /// is global.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnShowIcons(object sender, EventArgs e)
        {
            //if (this.ShowIcons)
            //    DbResourceConfiguration.Current.ShowControlIcons = true;
            //else
            //    DbResourceConfiguration.Current.ShowControlIcons = false;
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.ShowIcons && this.Visible)
                this.AddLocalizationIcons(this.Page, true);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (HttpContext.Current != null &&
                !DbResourceConfiguration.Current.ShowLocalizationControlOptions)
                return;

            if (this.Width == Unit.Empty)
                this.Width = Unit.Pixel(250);

            base.Render(writer);
        }

        /// <summary>
        /// Generates the Database resources for a given form
        /// </summary>
        /// <param name="ParentControl"></param>
        /// <param name="ResourcePrefix"></param>
        public void AddResourceToResourceFile(Control ParentControl, string ResourcePrefix, string ResourceSet)
        {
            if (ResourcePrefix == null)
                ResourcePrefix = "Resource1";

            if (ResourceSet == null)
                ResourceSet = this.Context.Request.ApplicationPath + this.Parent.TemplateControl.AppRelativeVirtualPath.Replace("~", "");


            var Data = DbResourceBaseDataManager.CreateDbResourceDataManager();  

            List<LocalizableProperty> ResourceList = this.GetAllLocalizableControls(ParentControl);

            foreach (LocalizableProperty Resource in ResourceList)
            {
                string ResourceKey = Resource.ControlId + ResourcePrefix + "." + Resource.Property;

                if (!Data.ResourceExists(ResourceKey, "", ResourceSet))
                    Data.AddResource(ResourceKey, Resource.Value, "", ResourceSet,null);
            }
        }

        /// <summary>
        /// Goes through the form and returns a list of all control on a form
        /// that are marked as [Localizable]
        /// </summary>
        /// <param name="control">Base container to start the parsing from. Usually this will be the current form but could be a control.</param>
        /// <returns></returns>
        public List<LocalizableProperty> GetAllLocalizableControls(Control ContainerControl)
        {
            return this.GetAllLocalizableControls(ContainerControl, null);
        }


        /// <summary>
        /// Goes through the form and returns a list of all control on a form
        /// that are marked as [Localizable]
        /// 
        /// This internal method does all the work and drills into child containers
        /// recursively.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="ResourceList"></param>
        /// <returns></returns>
        protected List<LocalizableProperty> GetAllLocalizableControls(Control control, List<LocalizableProperty> ResourceList)
        {
            return this.GetAllLocalizableControls(control, ResourceList, true);
        }


        /// <summary>
        /// Goes through the form and returns a list of all control on a form
        /// that are marked as [Localizable]
        /// 
        /// This internal method does all the work and drills into child containers
        /// recursively.
        /// </summary>
        /// <param name="control">The control at which to start parsing usually Page</param>
        /// <param name="ResourceList">An instance of the resource list. Pass null to create</param>
        /// <returns></returns>
        protected List<LocalizableProperty> GetAllLocalizableControls(Control control, List<LocalizableProperty> ResourceList, bool noControlRecursion)
        {
            if (control == null)
                control = this.Page;

            // On the first hit create the list - recursive calls pass in the list
            if (ResourceList == null)
                ResourceList = new List<LocalizableProperty>();

            // 'generated' controls don't have an ID and don't need to be localized
            if (control.ID != null)
            {
                // Read all public properties and search for Localizable Attribute
                PropertyInfo[] pi = control.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo property in pi)
                {
                    object[] Attributes = property.GetCustomAttributes(typeof(LocalizableAttribute), true);
                    if (Attributes.Length < 1)
                        continue;

                    LocalizableProperty lp = new LocalizableProperty();

                    lp.ControlId = control.ID;

                    if (lp.ControlId.StartsWith("__"))
                        lp.ControlId = lp.ControlId.Substring(2);

                    lp.Property = property.Name;
                    lp.Value = property.GetValue(control, null) as string;

                    ResourceList.Add(lp);
                }

                /// Check for special properties that not marked as [Localizable]
                GetNonLocalizableControlProperties(control, ResourceList);
            }

            if (!noControlRecursion)
            {
                // Now drill into the any contained controls
                foreach (Control ctl in control.Controls)
                {
                    // Recurse into child controls
                    if (ctl != null)
                        this.GetAllLocalizableControls(ctl, ResourceList);
                }
            }

            return ResourceList;
        }



        /// <summary>
        /// This method is used to override special controls that don't
        /// have LocalizableProperties but in fact should have them by
        /// explicitly marking certain properties as overridable.
        /// </summary>
        /// <param name="control">The control </param>
        /// <param name="ResourceList"></param>        
        protected virtual void GetNonLocalizableControlProperties(Control control, List<LocalizableProperty> ResourceList)
        {
            // The following have no effect because Literal and Localize don't render control ids
            //if (control is Localize)
            //{
            //    LocalizableProperty lp = new LocalizableProperty();
            //    Localize lc = (Localize)control;
            //    lp.ControlId = lc.ID;
            //    lp.Property = "Text";
            //    lp.Value = lc.Text;
            //    ResourceList.Add(lp);
            //}
            //else if (control is Literal)
            //{
            //    LocalizableProperty lp = new LocalizableProperty();
            //    Literal lc = (Literal)control;
            //    lp.ControlId = lc.ID;
            //    lp.Property = "Text";
            //    lp.Value = lc.Text;
            //    ResourceList.Add(lp);
            //}
        }


        /// <summary>
        /// This method is responsible for showing localization icons next to every control
        /// that has localizable properties.
        /// 
        /// The icons are resource based and also display the control's ID. Note icons are
        /// placed only next to any controls that are marked as [Localizable]. Some contained
        /// controls like GridVIew/DataGrid Columns are not marked as [Localizable] yet
        /// the ASP.NET designer creates implicit resources for them anyway - these controls
        /// will not show icons.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="TopLevel"></param>
        public void AddLocalizationIcons(Control control, bool TopLevel)
        {
            if (control == null)
                control = this.Page;

            string IconUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), GlobalizationResources.INFO_ICON_LOCALIZE);

            if (TopLevel)
            {
                this.AddIconScriptFunctions();

                // Embed the IconUrl
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), control.ID + "_iu",
                        "var _IconUrl = '" + IconUrl + "';\r\n", true);
            }

            // Don't localize ourselves
            if (control is DbResourceControl)
                return;


            // 'generated' controls don't have an ID and don't need to be localized
            if (control.ID != null)
            {
                // Get all Localizable properties for the current control
                List<LocalizableProperty> properties = this.GetAllLocalizableControls(control, null, true);
                if (properties == null)
                    return;

                foreach (LocalizableProperty lp in properties)
                {
                    string ResourceSet = control.TemplateControl.AppRelativeVirtualPath.Replace("~/", "");

                    //string Html = "<img src='"  + IconUrl +
                    //           "' onclick=\"OnLocalization('" + control.ID + "','" + ResourceSet + "','"+ lp.Property + "');\" style='margin-left:3px;border:none;' title='" + control.ID + "' >";

                    string Html = "<img src={0} onclick=\"OnLocalization(event,'" + control.ID + "','" + ResourceSet + "','" + lp.Property + "');\" style='margin-left:3px;border:none;' title='" + control.ID + "' />";
                    Html = string.Format(Html.Replace("'", @"\'"), "' + _IconUrl + '");



                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), control.ClientID + "_ahac",
                    string.Format("AddHtmlAfterControl('{0}','{1}');\r\n", control.ClientID, Html), true);


                    break;
                }




                //    // Read all public properties and search for Localizable Attribute
                //    PropertyInfo[] pi = control.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                //    foreach (PropertyInfo property in pi)
                //    {
                //        // Check for localizable Attribute on the property
                //        object[] Attributes = property.GetCustomAttributes(typeof(LocalizableAttribute), true);
                //        if (Attributes.Length < 1)
                //            continue;

                //        LocalizableProperty lp = new LocalizableProperty();
                //        lp.ControlId = control.ID;

                //        if (lp.ControlId.StartsWith("__"))
                //            lp.ControlId = lp.ControlId.Substring(2);

                //        lp.Property = property.Name;
                //        lp.Value = property.GetValue(control, null) as string;

                //        string ResourceSet = control.TemplateControl.AppRelativeVirtualPath.Replace("~/","");

                //        //string Html = "<img src='"  + IconUrl +
                //        //           "' onclick=\"OnLocalization('" + control.ID + "','" + ResourceSet + "','"+ lp.Property + "');\" style='margin-left:3px;border:none;' title='" + control.ID + "' >";

                //        string Html = "<img src={0} onclick=\"OnLocalization('" + control.ID + "','" + ResourceSet + "','" + lp.Property + "');\" style='margin-left:3px;border:none;' title='" + control.ID + "' >";
                //        Html = string.Format(Html.Replace("'", @"\'"), "' + _IconUrl + '");

                //         this.Page.ClientScript.RegisterStartupScript(this.GetType(), control.ClientID + "_ahac",
                //         string.Format("AddHtmlAfterControl('{0}','{1}');\r\n", control.ClientID,Html), true);

                //        break;
                //    }
            }

            // Now loop through all child controls
            foreach (Control ctl in control.Controls)
            {
                // Recurse into child controls
                if (ctl != null)
                    this.AddLocalizationIcons(ctl, false);
            }

        }


        /// <summary>
        /// Internal method to add the page level script routines
        /// that are used to inject the icons next to controls. These routines
        /// inject HTML into the DOM rather than adding controls in order
        /// to avoid problems with Controls.Add() and ASP.NET script expressions.
        /// </summary>
        private void AddIconScriptFunctions()
        {
            // get current page resource set name
            string ResourceSet = WebUtils.GetAppRelativePath();

            // resolve the the URL to look up a specific resource
            // NOTE: this is embedded in the Javascript below in a 'weird' format (no trailing single quote)
            //       because the string ends in a variable not a 'string'
            string Url = this.ResolveUrl(DbResourceConfiguration.Current.LocalizationFormWebPath) +
                        "?CtlId=' + CtlId + '.' + Property  +" +
                        "'&ResourceSet=' + ResourceSet";
            
    //script=
    //            var ctlId = CtlId + '.' + Property;
    //_resourceCallback.GetResources(ctlId,ResourceSet,
    //    function(dict)
    //    {
    //   $('#_resourcePanel').empty().append( 
    //                        $('<div></div>')
    //                            .addClass('errormessage')
    //                            .text(ctlId)
    //                            .css( { 'border-bottom': 'solid 1px navy', 'margin-bottom': 4 } )
    //    );
         
    //        for( var i=0; i < dict.length; i++)
    //        {
    //            var item = dict[i];
    //            if (item.key == '') item.key = 'Invariant';
    //            $('#_resourcePanel').append( 
    //                        $('<div></div>')
    //                            .html( '<b>' + item.key + '</b>' + ' - ' + item.value )
    //                            .css( 'border-bottom','dotted 1px teal')
    //            );
    //        }

    //        $('#_resourcePanel').show().moveToMousePosition(e);

    //    },function (error) {alert(error.message); } );        
    //return;
            
            
            string Script =
@"
function OnLocalization(e,CtlId,ResourceSet,Property) {   
";

            if (!string.IsNullOrEmpty(this.ClientOnLocalizationIconHandler))
                Script += "    " + this.ClientOnLocalizationIconHandler + "(e,CtlId,ResourceSet,Property); " +
                          "     return;\r\n}";
            else
            Script+= @"
    if (CtlId == null || CtlId == """")
       return;

	var win =  window.open('" + Url + @",
	            ""_Localization"",
	            ""width=865,height=650,resizable=1,scrollbars=1"",""_Localization"");		    
    if (win.focus)
       win.focus();
}";
            Script += @"
function AddHtmlAfterControl(ControlId,HtmlMarkup)
{
    var Ctl = document.getElementById(ControlId);
    if (Ctl == null)
     return;
     
    var Insert = document.createElement('span');
    Insert.innerHTML = HtmlMarkup;

    var Sibling = Ctl.nextSibling;
    if (Sibling != null)
     Ctl.parentNode.insertBefore(Insert,Sibling);
    else
     Ctl.parentNode.appendChild(Insert);
}";

            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "IconScript", Script, true);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            // Make sure we use block mode instead of stock inline-block
            this.Style.Add(HtmlTextWriterStyle.Display, "block");

            this.Style.Add(HtmlTextWriterStyle.Margin, "10px");
            this.Controls.Add(new LiteralControl(string.Format("<b>{0}</b><hr/>", Resources.Resources.LocalizationOptions)));

            Image img = new Image();

            if (this.Page != null)
                img.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), GlobalizationResources.INFO_ICON_EDITRESOURCES);

            img.Style[HtmlTextWriterStyle.MarginRight] = "10px";
            this.Controls.Add(img);

            // Add an Ajax Callback control to handle inline display
            AjaxMethodCallback callback = new AjaxMethodCallback();
            callback.PageProcessingMode = CallbackProcessingModes.PageLoad;
            callback.TargetInstance = this;
            callback.ID = "_resourceCallback";
            callback.ClientProxyTargetType = this.GetType();
            callback.GenerateClientProxyClass = ProxyClassGenerationModes.Inline;
            this.Controls.Add(callback);



            LinkButton button = this.btnEditResources;
            button.ID = "btnEditResources";
            button.Text = Resources.Resources.EditPageResources;
            //but.Attributes.Add("target", "LocalizationForm");
            button.Click += new EventHandler(OnEditResources);
            this.Controls.Add(button);

            this.Controls.Add(new LiteralControl("<br/>"));

            CheckBox cb = this.chkShowIcons;
            cb.ID = "chkShowIcons";
            cb.Text = Resources.Resources.ShowLocalizationIcons;
            cb.AutoPostBack = true;
            cb.Checked = this.ShowIcons;
            cb.CheckedChanged += new EventHandler(this.OnShowIcons);

            this.Controls.Add(cb);
        }

        [CallbackMethod]
        public Dictionary<string, string> GetResourcesForId(string resourceID, string resourceSet)
        {
            var manager = DbResourceBaseDataManager.CreateDbResourceDataManager();  
            var resourceStrings =  manager.GetResourceStrings(resourceID, resourceSet);
            return resourceStrings;
        }

        

    }

    /// <summary>
    /// simple object that holds the value structure of each
    /// saved resource item on a form
    /// </summary>
    [Serializable]
    public class LocalizableProperty
    {
        public string ControlId = "";
        public string Property = "";
        public string Value = "";
    }


    /// <summary>
    /// Control designer used so we get a grey button display instead of the 
    /// default label display for the control.
    /// </summary>
    internal class DbResourceControlDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            StringWriter sb = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(sb);

            DbResourceControl Ctl = new DbResourceControl();
            Ctl.RenderControl(writer);

            return sb.ToString();
        }
    }

    public enum ShowLocalizationStates
    {
        Show,
        DontShow,
        InheritFromProvider
    }

}
