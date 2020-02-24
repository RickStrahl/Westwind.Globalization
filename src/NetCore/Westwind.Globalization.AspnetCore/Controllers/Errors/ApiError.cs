using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Westwind.Utilities;

namespace Westwind.Globalization.Errors
{

    /// <summary>
    /// An API error result model class that returns error information
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// An error message to display
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Markder flag to allow client to identify this as an error object
        /// </summary>
        public bool isError { get; set; }

        /// <summary>
        /// Additional error information
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// A collection of validation errors for holding model validation
        /// and business object validation errors
        /// </summary>
        public ValidationErrorCollection errors { get; set; }

        
        public ApiError(string message)
        {
            this.message = message;
            isError = true;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            this.isError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                message = "Please correct the specified errors and try again.";
                //errors = modelState.SelectMany(m => m.Value.Errors).ToDictionary(m => m.Key, m=> m.ErrorMessage);
                //errors = modelState.SelectMany(m => m.Value.Errors.Select( me => new KeyValuePair<string,string>( m.Key,me.ErrorMessage) ));
                //errors = modelState.SelectMany(m => m.Value.Errors.Select(me => new ModelError { FieldName = m.Key, ErrorMessage = me.ErrorMessage }));
            }
        }
    }
}
