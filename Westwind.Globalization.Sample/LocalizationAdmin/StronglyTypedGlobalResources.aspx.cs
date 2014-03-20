// #define OnlineDemo

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Westwind.Globalization;
using System.IO;

namespace Westwind.GlobalizationWeb
{

    public partial class LocalizationAdmin_StronglyTypedGlobalResources : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

#if (OnlineDemo )
            this.ClientScript.RegisterStartupScript(typeof(Page), "outfile_disable",
                     "document.getElementById('" + this.txtOutputFile.ClientID + "').disabled = true;", true);
#endif
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            StronglyTypedWebResources Exp = new StronglyTypedWebResources(Request.PhysicalApplicationPath);
            string OutputFile = this.Server.MapPath(this.txtOutputFile.Text);
            string Output = "";

#if (OnlineDemo )
            OutputFile = Server.MapPath("~/_classes/Resources.cs");
            if (Path.GetExtension(OutputFile).ToLower() != ".cs")
            {
                this.lblGenetatedCode.Text = "Only C# output is supported for this resource demo.";
                return;
            }
#endif
            Output += "Output file: " + OutputFile + "\r\n\r\n";

            if (this.lstExportFrom.SelectedValue == "ResX")
                Output += Exp.CreateClassFromFromAllGlobalResXResources("AppResources", OutputFile);
            else
                Output += Exp.CreateClassFromAllDatabaseResources("AppResources", OutputFile);

            this.lblGenetatedCode.Text = Output;
        }
    }

}