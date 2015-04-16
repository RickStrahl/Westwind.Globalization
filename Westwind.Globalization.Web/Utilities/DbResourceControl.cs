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
using System.Web.UI.HtmlControls;
using Westwind.Globalization.Properties;
using Westwind.Utilities;
using Westwind.Web;
using Westwind.Web.Controls;
using System.Linq;

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

            // hook up pre-render so we can inject controls
            // into the page
            Page.PreRender += Page_PreRender;
        }

        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                if (ShowIconsInitially == ShowLocalizationStates.DontShow)
                    ShowIcons = false;
                else if (ShowIconsInitially == ShowLocalizationStates.Show)
                    ShowIcons = true;
                else if (ShowIconsInitially == ShowLocalizationStates.InheritFromProvider)
                    ShowIcons = DbResourceConfiguration.Current.ShowControlIcons &&
                                DbResourceConfiguration.Current.ShowLocalizationControlOptions;

                if (ShowIcons)
                    chkShowIcons.Checked = true;
            }
            else
            {
                ShowIcons = chkShowIcons.Checked;
            }
        }

        /// <summary>
        /// Hook to Page Pre-Render to allow injecting controls at runtime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_PreRender(object sender, EventArgs e)
        {
            if (ShowIcons && Visible)
                AddLocalizationIcons(Page, true);
        }


        /// <summary>
        /// Goes through the form and returns a list of all control on a form
        /// that are marked as [Localizable]
        /// </summary>
        /// <param name="control">Base container to start the parsing from. Usually this will be the current form but could be a control.</param>
        /// <returns></returns>
        public List<LocalizableProperty> GetAllLocalizableControls(Control ContainerControl)
        {
            return GetAllLocalizableControls(ContainerControl, null);
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
            return GetAllLocalizableControls(control, ResourceList, true);
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
                control = Page;

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
            }

            if (!noControlRecursion)
            {
                // Now drill into the any contained controls
                foreach (Control ctl in control.Controls)
                {
                    // Recurse into child controls
                    if (ctl != null)
                        GetAllLocalizableControls(ctl, ResourceList);
                }
            }

            return ResourceList;
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
                control = Page;

            string localizationAdminPath = ResolveUrl(DbResourceConfiguration.Current.LocalizationFormWebPath);            
 
            // Don't localize ourselves
            if (control is DbResourceControl)
                return;
            
            string resourceSet = control.TemplateControl.AppRelativeVirtualPath.Replace("~/", "");

            // 'generated' controls don't have an ID and don't need to be localized
            if (control.ID != null)
            {
                // Get all Localizable properties for the current control
                List<LocalizableProperty> properties = GetAllLocalizableControls(control, null, true);
                if (properties == null || properties.Count < 1)
                {
                    // do nothing
                }
                else if (properties.Where(itm => itm.Property == "Text").Any())
                    AddResourceAttributesToControl(control, "Text",resourceSet);
                else
                    AddResourceAttributesToControl(control, properties.First().Property,resourceSet);
            }

            // Now loop through all child controls
            foreach (Control ctl in control.Controls)
            {
                // Recurse into child controls
                if (ctl != null)
                    AddLocalizationIcons(ctl, false);
            }

        }

        void AddResourceAttributesToControl(Control control, string property, string resourceSet)
        {

                    if (control is WebControl)
                    {
                        var ctl = control as WebControl;
                        ctl.Attributes["data-resource-id"] = control.ID + "." + property;
                        ctl.Attributes["data-resource-set"] = resourceSet;
                    }
                    else if (control is Literal)  // literal and localize don't have wrapping tags so add them
                    {
                        var ctl = control as Literal;                        
                        ctl.Text = string.Format("<span data-resource-id=\"{0}\" data-resoure-set=\"{1}\">" +
                                   ctl.Text +
                                   "</span>", control.ID + "." + property, resourceSet);
                    }
                    else if (control is HtmlControl)
                    {
                        var ctl = control as HtmlControl;
                        ctl.Attributes["data-resource-id"] = control.ID + "." + property;
                        ctl.Attributes["data-resource-set"] = resourceSet;
                    }
                    else
                    {
                        if (control.HasControls() && !(control is Page || control is HtmlForm))
                        {
                            control.Controls.AddAt(0, new Literal()
                            {
                                Text = string.Format("<span data-resource-id=\"{0}\" data-resoure-set=\"{1}\">",control.ID + "." + property, resourceSet)                            
                            });
                            control.Controls.Add(new Literal() {Text = "</span>" });
                        }
                    }

        }


#if false
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
           

            // Make sure we use block mode instead of stock inline-block
            this.Style.Add(HtmlTextWriterStyle.Display, "block");

            this.Style.Add(HtmlTextWriterStyle.Margin, "10px");
            this.Controls.Add(new LiteralControl(string.Format("<b>{0}</b><hr/>", Resources.LocalizationOptions)));

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

            this.Controls.Add(new LiteralControl("<br/>"));

            CheckBox cb = this.chkShowIcons;
            cb.ID = "chkShowIcons";
            cb.Text = Resources.ShowLocalizationIcons;
            cb.AutoPostBack = true;
            cb.Checked = this.ShowIcons;
            cb.Attributes.Add("onclick","if(ww.resourceEditor.isResourceEditingEnabled ? : )")
            //cb.CheckedChanged += new EventHandler(this.OnShowIcons);

            this.Controls.Add(cb);
        }

        [CallbackMethod]
        public Dictionary<string, string> GetResourcesForId(string resourceID, string resourceSet)
        {
            var manager = DbResourceDataManager.CreateDbResourceDataManager();  
            var resourceStrings =  manager.GetResourceStrings(resourceID, resourceSet);
            return resourceStrings;
        }
#endif
        

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


    ///// <summary>
    ///// Control designer used so we get a grey button display instead of the 
    ///// default label display for the control.
    ///// </summary>
    //internal class DbResourceControlDesigner : ControlDesigner
    //{
    //    public override string GetDesignTimeHtml()
    //    {
    //        StringWriter sb = new StringWriter();
    //        HtmlTextWriter writer = new HtmlTextWriter(sb);

    //        DbResourceControl Ctl = new DbResourceControl();
    //        Ctl.RenderControl(writer);

    //        return sb.ToString();
    //    }
    //}

    public enum ShowLocalizationStates
    {
        Show,
        DontShow,
        InheritFromProvider
    }

}
