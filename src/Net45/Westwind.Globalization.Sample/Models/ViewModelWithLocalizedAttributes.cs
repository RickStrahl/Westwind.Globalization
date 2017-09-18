using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Westwind.Globalization.Sample.Properties;
using Westwind.Globalization.Web;


namespace Westwind.Globalization.Sample.Models
{
    public class ViewModelWithLocalizedAttributes
    {
        [Required(ErrorMessageResourceName = "NameIsRequired", ErrorMessageResourceType = typeof(Resources))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "AddressIsRequired", ErrorMessageResourceType = typeof(Resources))]
        public string Address { get; set; }
    }
}
