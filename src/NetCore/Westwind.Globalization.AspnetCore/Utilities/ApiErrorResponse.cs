using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Westwind.AspNetCore.Extensions
{
    /// <summary>
    /// Class that represents an error returned to
    /// the client caller. Can be explicitly returned or
    /// as part of the UnhandledExceptionFilter.
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// The text message for the errors
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Flag value that indicates to the client that this
        /// is an error response
        /// </summary>
        public bool IsCallbackError { get; set; }

        /// <summary>
        /// An optional list of errors that can be set on the
        /// error object. Automatically set when passing in 
        /// a model dictionary with errors.
        /// </summary>
        public List<string> Errors { get; set; }


        /// <summary>
        /// Optional StackTrace
        /// </summary>
        public string StackTrace { get; set; }


        /// <summary>
        /// Optional data item that can be attached to the error
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Default constructor creates empty error object
        /// </summary>
        public ApiErrorResponse()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Pass in a message string for the exception.
        /// </summary>
        /// <param name="errorMessage"></param>
        public ApiErrorResponse(string errorMessage)
        {
            IsCallbackError = true;
            Errors = new List<string>();
            Message = errorMessage;
        }

        /// <summary>
        /// Pass in an exception and pick up the message.
        /// </summary>
        /// <param name="ex"></param>
        public ApiErrorResponse(Exception ex)
        {
            IsCallbackError = true;
            Errors = new List<string>();
            Message = ex.Message;
            StackTrace = ex.StackTrace + "\r\n" + ex.Source;
            
        }

        /// <summary>
        /// Pass in a modelState dictionary to create a list of
        /// binding errors from the API error message
        /// </summary>
        /// <param name="modelState"></param>
        public ApiErrorResponse(ModelStateDictionary modelState)
        {
            IsCallbackError = true;
            Errors = new List<string>();
            Message = "Model is invalid.";

            // add errors into our client error model for client
            foreach (var modelItem in modelState)
            {
                var modelError = modelItem.Value.Errors.FirstOrDefault();
                if (!string.IsNullOrEmpty(modelError.ErrorMessage))
                    Errors.Add(modelItem.Key + ": " +
                               ParseModelStateErrorMessage(modelError.ErrorMessage));
                else
                    Errors.Add(modelItem.Key + ": " +
                               ParseModelStateErrorMessage(modelError.Exception.Message));
            }
        }

        /// <summary>
        /// Strips off anything after period - line number etc. info
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        string ParseModelStateErrorMessage(string msg)
        {
            int period = msg.IndexOf('.');
            if (period < 0 || period > msg.Length - 1)
                return msg;

            // strip off 
            return msg.Substring(0, period);
        }
    }

}
