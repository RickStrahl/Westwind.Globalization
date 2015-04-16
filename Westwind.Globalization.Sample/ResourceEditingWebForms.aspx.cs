using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Westwind.Globalization.Sample
{
    public partial class ResourceEditingWebForms : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var manager = DbResourceDataManager.CreateDbResourceDataManager();

            var resources = manager.GetAllResources();

            this.dgResources.DataSource = resources;
            this.dgResources.DataBind();
        }
    }
}