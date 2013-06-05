using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.Design;


namespace Westwind.Globalization.Design
{
    /// <summary>
    /// Control designer used so we get a grey button display instead of the 
    /// default label display for the control.
    /// </summary>
    internal class LocalizationExtenderDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            return base.CreatePlaceHolderDesignTimeHtml("Control Extender");
        }

        
    }
}
