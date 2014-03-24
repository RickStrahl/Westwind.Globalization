#define OnLineDemo

using System;
using System.Data;
using System.Web.UI.WebControls;
using Westwind.Globalization;
using System.Globalization;
using Westwind.Web.Controls;
using System.Collections.Generic;
using System.IO;

using Westwind.Web;
using Westwind.Utilities;
using System.Web;
using System.Configuration;

namespace Westwind.GlobalizationWeb
{
    public partial class LocalizeAdmin : System.Web.UI.Page
    {
        /// <summary>
        /// We talk directly to the Db resource manager (bus object) here rather than
        /// through the provider or resource manager, as we don't have the flexibility
        /// with the core resource managers.
        /// </summary>
        protected DbResourceDataManager Manager  = new DbResourceDataManager();

        public string ResourceSet {get; set; }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);


            // *** On callbacks we don't need to populate any data since they are
            // *** raw method calls. Callback routes to parser from here
            if (this.Callback.IsCallback)
                return;

            Response.Expires = 0;

            if (!Manager.IsLocalizationTable(null))
            {
                this.ErrorDisplay.ShowError( WebUtils.LRes("ResourceTableDoesntExist"));
                this.btnCreateTable.Visible = true;
                return;
            }

            GetResourceSet();

            ListItem[] items = Manager.GetAllResourceIdsForHtmlDisplay(ResourceSet);
            this.lstResourceIds.Items.AddRange(items);


            //DataTable dt = Manager.GetAllResourceIds(ResourceSet);
            //if (dt == null)
            //{
            //    this.ErrorDisplay.ShowError("Couldn't load resources: " + Manager.ErrorMessage);
            //    return;
            //}
            //this.lstResourceIds.DataSource = dt;
            //this.lstResourceIds.DataValueField = "ResourceId";            
            //this.lstResourceIds.DataBind();


            if (this.lstResourceIds.Items.Count > 0)
                this.lstResourceIds.SelectedIndex = 0;

            DataTable dt = Manager.GetAllLocaleIds(ResourceSet);
            if (dt == null)
            {
                this.ErrorDisplay.ShowError("Couldn't load resources: " + Manager.ErrorMessage);
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                string Code = row["LocaleId"] as string;
                CultureInfo ci = CultureInfo.GetCultureInfo(Code.Trim());

                if (Code != "")
                    row["Language"] = ci.DisplayName + " (" + ci.Name + ")";
                else
                    row["Language"] = "Invariant";
            }

            this.lstLanguages.DataSource = dt;
            this.lstLanguages.DataValueField = "LocaleId";
            this.lstLanguages.DataTextField = "Language";
            this.lstLanguages.DataBind();

            if (this.lstLanguages.Items.Count > 0)
                this.lstLanguages.SelectedIndex = 0;
            else
                this.lstLanguages.Items.Add(new ListItem("Invariant", ""));            
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            // *** On callbacks we don't need to populate any data since they are
            // *** raw method calls. Callback routes to parser from here
            if (this.Callback.IsCallback)
                return;

            // create a localRes object in JavaScript so reosurces are available 
            // on the client
            JavaScriptResourceHandler.RegisterJavaScriptLocalResources(this, "localRes");

            this.SetControlId();

            // Need to manually load images from resources
            this.imgExportResources.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_EXTRACTRESOURCES);
            this.imgRefresh.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_REFRESH);
            this.imgRecycleApp.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_RECYCLE);

            if (this.btnCreateTable.Visible)
            {
                this.imgCreateTable.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_CREATETABLE);
                this.imgCreateTable.Visible = true;
            }

            this.imgDeleteResourceSet.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_DELETE);
            this.imgRenameResourceSet.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_RENAME);
            this.imgAddResourceSet.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_NEW);

            this.imgImport.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_IMPORT);
            this.imgBackup.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(GlobalizationResources), GlobalizationResources.INFO_ICON_BACKUP);            

            //// *** Check if resources are properly active if not we have a 
            ////     problem and need to let user know
            //if (WebUtils.LRes("BackupComplete") == "BackupComplete")
            //{
            //    // *** Not localized so it's always visible!!!
            //    this.ErrorDisplay.DisplayTimeout = 0;
            //    this.ErrorDisplay.ShowError("Resources are not available for this site. Most likely this means you have enabled the DbResourceProvider without first importing resources or that your database connection is not properly configured.<p/>" +
            //                                "Please make sure you run this form without the DbResourceProvider enabled and ensure you have created the resource table and imported Resx resources of the site. <p />" +
            //                                "For more information please check the following configuration link: <p />" +
            //                                "<a href='http://www.west-wind.com/WestwindWebToolkit/docs/?page=_1xl057dta.htm'>Using the the DbResourceProvider Documentation</a>");
            //}

        }

        private void GetResourceSet()
        {
            this.ResourceSet = Request.Form[this.lstResourceSet.UniqueID];
            if (ResourceSet == null)
                this.ResourceSet = Request.QueryString["ResourceSet"];
            if (this.ResourceSet == null)
                this.ResourceSet = ViewState["ResourceSet"] as string;

            if (this.ResourceSet == null)
                this.ResourceSet = "";

            this.ResourceSet = this.ResourceSet.ToLower();

            if (!string.IsNullOrEmpty(this.ResourceSet))
                ViewState["ResourceSet"] = this.ResourceSet;

            // *** Clear selections
            //lstResourceIds.Items.Clear();
            lstResourceIds.ClearSelection();
            lstResourceSet.Items.Clear();
            lstResourceSet.ClearSelection();   

            DataTable dt = Manager.GetAllResourceSets(ResourceListingTypes.AllResources);
            this.lstResourceSet.DataSource = dt;
            this.lstResourceSet.DataValueField = "ResourceSet";

            try
            {
                this.lstResourceSet.DataBind();
            }
            catch 
            { 
                // this fails for some unknown reason on rebinds (off a btn submission)
                // which in effect rebinds an already bound list.
                //
                // First pass through always works fine - only rebind fails.
                //
                // complains about SelectedValue being set to a value
                // that doesn't exist even though selection is already
                // cleared AND the value that it actually is set after
                // DataBind() DOES exist in the data.
            }

            if (!string.IsNullOrEmpty(this.ResourceSet))
            {
                // find item to select w/o case
                foreach (ListItem itm in lstResourceSet.Items)
                {
                    if (itm.Value.ToLower() == ResourceSet)
                    {
                        itm.Selected = true;
                        break;
                    }
                }                
            }
            else
            {
                if (this.lstResourceSet.Items.Count > 0)
                {
                    this.ResourceSet = this.lstResourceSet.Items[0].Value;
                    this.lstResourceSet.SelectedValue = this.ResourceSet;
                }
            }
        }

        private void SetControlId()
        {
            string CtlId = null;
            if (this.IsPostBack)
                CtlId = Request.Form[this.lstResourceIds.UniqueID];

            if (CtlId == null)
                CtlId = Request.QueryString["CtlId"];

            if (string.IsNullOrEmpty(CtlId))
                return;

            string Id = CtlId;

            // *** Search for .Text first
            string[] Tokens = Id.Split('.');
            if (Tokens.Length == 2)
                Id = Tokens[0] + ".Text";

            for (int x = 0; x < 2; x++)
            {
                if (x == 1)
                    // *** No match for .text - find passed property
                    Id = CtlId;

                foreach (ListItem li in this.lstResourceIds.Items)
                {
                    if (li.Value.ToLower() == Id.ToLower())
                    {
                        this.lstResourceIds.SelectedValue = Id;
                        return;
                    }
                }
            }

        }
       

        protected void btnFileUpload_Click(object sender, EventArgs e)
        {

#if OnlineDemo
        this.ErrorDisplay.ShowError(WebUtils.LRes("FeatureDisabled"));
        return;   
#endif


            if (!this.FileUpload.HasFile)
                return;

            //FileInfo fi = new FileInfo(this.FileUpload.FileName);
            string Extension = Path.GetExtension(this.FileUpload.FileName).TrimStart('.');  // fi.Extension.TrimStart('.');

            string Filter = ",bmp,ico,gif,jpg,png,css,js,txt,wav,mp3,";
            if (Filter.IndexOf("," + Extension + ",") == -1)
            {
                this.ErrorDisplay.ShowError(WebUtils.LRes("InvalidFileUploaded"));
                return;
            }

            string FilePath = Server.MapPath(this.FileUpload.FileName);

            File.WriteAllBytes(FilePath, FileUpload.FileBytes);

            string ResourceId = this.txtNewResourceId.Text;

            // *** Try to add the file
            DbResourceDataManager Data = new DbResourceDataManager();
            if (Data.UpdateOrAdd(ResourceId, FilePath, this.txtNewLanguage.Text, this.ResourceSet, null, true) == -1)
                this.ErrorDisplay.ShowError(WebUtils.LRes("ResourceUpdateFailed") + "<br/>" + Data.ErrorMessage);
            else
                this.ErrorDisplay.ShowMessage(WebUtils.LRes("ResourceUpdated"));

            File.Delete(FilePath);

            this.lstResourceIds.Items.Add(ResourceId);
            this.lstResourceIds.SelectedValue = ResourceId;
        }


        protected void btnCreateTable_Click(object sender, EventArgs e)
        {
#if OnlineDemo
        this.ErrorDisplay.ShowError(WebUtils.LRes("FeatureDisabled"));
        return;
#endif
            if (!this.Manager.CreateLocalizationTable(null))
                this.ErrorDisplay.ShowError(WebUtils.LRes("LocalizationTableNotCreated") + "<br />" +
                                            this.Manager.ErrorMessage);
            else
            {                
                ErrorDisplay.ShowMessage(WebUtils.LRes("LocalizationTableCreated"));
                Response.AddHeader("Refresh", "3;" + Request.Url.ToString());
            }
        }

        


        protected void btnExportResources_Click(object sender, EventArgs e)
        {
#if OnlineDemo
        this.ErrorDisplay.ShowError(WebUtils.LRes("FeatureDisabled"));
        return;
#endif

            DbResXConverter Exporter = new DbResXConverter(this.Context.Request.PhysicalApplicationPath);

            if (DbResourceConfiguration.Current.ProjectType == GlobalizationProjectTypes.WebForms)
            {
                if (!Exporter.GenerateLocalWebResourceResXFiles())
                {
                    ErrorDisplay.ShowError(WebUtils.LRes("ResourceGenerationFailed"));
                    return;
                }
                if (!Exporter.GenerateGlobalWebResourceResXFiles())
                {
                    ErrorDisplay.ShowError(WebUtils.LRes("ResourceGenerationFailed"));
                    return;
                }
                
            }
            else
            {
                if (!Exporter.GenerateResXFiles())
                {
                    ErrorDisplay.ShowError(WebUtils.LRes("ResourceGenerationFailed"));
                    return;
                }
            }


            this.ErrorDisplay.ShowMessage(WebUtils.LRes("ResourceGenerationComplete"));
        }


        protected void btnGenerateStronglyTypedResources_Click(object sender, EventArgs e)
        {
            string[] tokens = DbResourceConfiguration.Current.StronglyTypedGlobalResource.Split(',');
            if (tokens.Length == 2)
            {
                StronglyTypedWebResources strongTypes = new StronglyTypedWebResources(Context.Request.PhysicalApplicationPath);
                strongTypes.CreateClassFromAllDatabaseResources(tokens[1], HttpContext.Current.Server.MapPath(tokens[0]));
                 
                if (string.IsNullOrEmpty(strongTypes.ErrorMessage))                
                    ErrorDisplay.ShowMessage(string.Format( WebUtils.LRes("StronglyTypedGlobalResourcesCreated"), tokens[1]) );
                else
                    ErrorDisplay.ShowMessage(WebUtils.LRes("StronglyTypedGlobalResourcesFailed"));
                
            }
        }


        protected void btnImport_Click(object sender, EventArgs e)
        {
#if OnlineDemo
        this.ErrorDisplay.ShowError(WebUtils.LRes("FeatureDisabled"));
        return;
#endif

            DbResXConverter Converter = new DbResXConverter(this.Context.Request.PhysicalApplicationPath);            
            
            bool res = false;

            if (DbResourceConfiguration.Current.ProjectType == GlobalizationProjectTypes.WebForms)
                res = Converter.ImportWebResources();
            else
                res = Converter.ImportWinResources(Server.MapPath("~/"), "CodePasteMvc");       
            

            if (res)
                this.ErrorDisplay.ShowMessage(WebUtils.LRes("ResourceImportComplete"));
            else
                this.ErrorDisplay.ShowError(WebUtils.LRes("ResourceImportFailed"));

            this.lstResourceIds.ClearSelection();
            this.lstResourceSet.ClearSelection();

            this.GetResourceSet();
        }

        protected void btnRenameResourceSet_Click(object sender, EventArgs e)
        {
#if OnlineDemo
        this.ErrorDisplay.ShowError(WebUtils.LRes("FeatureDisabled"));
        return;
#endif

            if (!this.Manager.RenameResourceSet(this.txtOldResourceSet.Text, this.txtRenamedResourceSet.Text))
                this.ErrorDisplay.ShowError(this.Manager.ErrorMessage);
            else
            {
                // *** Force the selected value to be set
                this.lstResourceSet.Items.Add(new ListItem("", this.txtRenamedResourceSet.Text.ToLower()));
                this.lstResourceSet.SelectedValue = this.txtRenamedResourceSet.Text.ToLower();

                //this.lstResourceSet.SelectedValue = string.Empty;   // null; 

                // *** Refresh and reset the resource list
                this.GetResourceSet();

                this.ErrorDisplay.ShowMessage(WebUtils.LRes("ResourceSetRenamed"));
            }
        }


        #region CallbackMethods

        [CallbackMethod]
        public DataTable GetResourceList(string ResourceSet)
        {
            DataTable dt = Manager.GetAllResourceIds(ResourceSet);
            if (Manager == null)
                throw new ApplicationException(WebUtils.LRes("ResourceSetLoadingFailed") + ":" + Manager.ErrorMessage);

            foreach (DataRow row in dt.Rows)
            {

            }

            return dt;
        }

        [CallbackMethod]
        public string GetResourceString(string ResourceId, string ResourceSet, string CultureName)
        {
            string Value = this.Manager.GetResourceString(ResourceId, ResourceSet, CultureName);

            if (Value == null && !string.IsNullOrEmpty(Manager.ErrorMessage))
                throw new ArgumentException(this.Manager.ErrorMessage);

            return Value;
        }

        [CallbackMethod]
        public ResourceItem GetResourceItem(string ResourceId, string ResourceSet, string CultureName)
        {
            return this.Manager.GetResourceItem(ResourceId, ResourceSet, CultureName);
        }

        /// <summary>
        /// Gets all resources for a given ResourceId for all cultures from
        /// a resource set.
        /// </summary>
        /// <param name="ResourceId"></param>
        /// <param name="ResourceSet"></param>
        /// <returns>Returns an array of Key/Value objects to the client</returns>
        [CallbackMethod]
        public object GetResourceStrings(string ResourceId, string ResourceSet)
        {
            Dictionary<string, string> Resources = this.Manager.GetResourceStrings(ResourceId, ResourceSet);

            if (Resources == null)
                throw new ApplicationException(this.Manager.ErrorMessage);

            return Resources;
        }


        [CallbackMethod]
        public bool UpdateResourceString(string Value, string ResourceId, string ResourceSet, string LocaleId)
        {
            if (string.IsNullOrEmpty(Value))            
                return this.Manager.DeleteResource(ResourceId, LocaleId, ResourceSet);
            
            if (this.Manager.UpdateOrAdd(ResourceId, Value, LocaleId, ResourceSet,null) == -1)
                return false;

            return true;
        }

        [CallbackMethod]
        public bool UpdateResourceWithComment(string Value, string Comment, string ResourceId, string ResourceSet, string LocaleId)
        {
            if (string.IsNullOrEmpty(Value))
                return this.Manager.DeleteResource(ResourceId, LocaleId, ResourceSet);

            if (this.Manager.UpdateOrAdd(ResourceId, Value, LocaleId, ResourceSet, Comment) == -1)
                return false;

            return true;
        }


        [CallbackMethod]
        public bool DeleteResource(string ResourceId, string ResourceSet, string LocaleId)
        {

#if OnlineDemo        
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            if (!this.Manager.DeleteResource(ResourceId, LocaleId, ResourceSet))
                throw new ApplicationException(WebUtils.LRes("ResourceUpdateFailed") + ": " + this.Manager.ErrorMessage);

            return true;
        }

        [CallbackMethod]
        public bool RenameResource(string ResourceId, string NewResourceId, string ResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            if (!this.Manager.RenameResource(ResourceId, NewResourceId, ResourceSet))
                throw new ApplicationException(WebUtils.LRes("InvalidResourceId"));

            return true;
        }

        /// <summary>
        /// Renames all resource keys that match a property (ie. lblName.Text, lblName.ToolTip)
        /// at once. This is useful if you decide to rename a meta:resourcekey in the ASP.NET
        /// markup.
        /// </summary>
        /// <param name="Property">Original property prefix</param>
        /// <param name="NewProperty">New Property prefix</param>
        /// <param name="ResourceSet">The resourceset it applies to</param>
        /// <returns></returns>
        [CallbackMethod]
        public bool RenameResourceProperty(string Property, string NewProperty, string ResourceSet)
        {
            if (!this.Manager.RenameResourceProperty(Property, NewProperty, ResourceSet))
                throw new ApplicationException(WebUtils.LRes("InvalidResourceId"));

            return true;
        }

        [CallbackMethod]
        public string Translate(string Text, string From, string To, string Service)
        {
            Service = Service.ToLower();

            var translate = new TranslationServices();
            translate.TimeoutSeconds = 10;

            string result = null;
            if (Service == "google")
                result = translate.TranslateGoogle(Text, From, To);
            else if (Service == "bing")
            {
                if (string.IsNullOrEmpty(DbResourceConfiguration.Current.BingClientId))
                    result = ""; // don't do anything -  just return blank 
                else
                    result = translate.TranslateBing(Text, From, To);
            }

            if (result == null)
                result = translate.ErrorMessage;

            return result;
        }

        [CallbackMethod]
        public bool DeleteResourceSet(string ResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            return this.Manager.DeleteResourceSet(ResourceSet);
        }

        [CallbackMethod]
        public bool RenameResourceSet(string OldResourceSet, string NewResourceSet)
        {
#if OnlineDemo
        throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif

            return this.Manager.RenameResourceSet(OldResourceSet, NewResourceSet);
        }

        [CallbackMethod]
        public void ReloadResources()
        {
            //Westwind.Globalization.Tools.wwWebUtils.RestartWebApplication();
            //DbResourceConfiguration.ClearResourceCache();
            DbRes.ClearResources();
        }

        [CallbackMethod]
        public bool Backup()
        {
#if OnlineDemo
            throw new ApplicationException(WebUtils.LRes("FeatureDisabled"));
#endif
            return this.Manager.CreateBackupTable(null);
        }



        #endregion

    }

    

}