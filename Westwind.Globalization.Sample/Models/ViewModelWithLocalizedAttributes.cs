using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Westwind.Globalization.Sample
{
    public class ViewModelWithLocalizedAttributes
    {
        [Required(ErrorMessageResourceName = "NameIsRequired", ErrorMessageResourceType = typeof(Resources))]
        public string Name { get; set;  }

        [Required(ErrorMessageResourceName = "AddressIsRequired", ErrorMessageResourceType = typeof(Resources))]
        public string Address { get; set;  }
    }
}