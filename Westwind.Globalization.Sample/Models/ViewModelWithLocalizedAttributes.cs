using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AppResources;

namespace Westwind.Globalization.Sample
{
    public class ViewModelWithLocalizedAttributes
    {
        [Required(ErrorMessageResourceName = "NameIsRequired", ErrorMessageResourceType = typeof(AppResources.Resources))]
        public string Name { get; set;  }

        [Required(ErrorMessageResourceName = "AddressIsRequired", ErrorMessageResourceType = typeof(AppResources.Resources))]
        public string Address { get; set;  }
    }
}