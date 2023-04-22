using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Westwind.Globalization.Errors
{
    /// <summary>
    /// Unhandled Exception filter attribute for API controllers.
    /// Fires back a common JSON response of type ApiErrorResponse
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class UnhandledApiExceptionFilter : ExceptionFilterAttribute
    {

        public override void OnException(ExceptionContext context)
        {
            // show all exceptions in the Visual Studio output window.
            // Can also be redirected to the console with some configuration...
            var baseException = context.Exception.GetBaseException();
            System.Diagnostics.Trace.WriteLine(baseException.ToString());

            // Inner exceptions don't always include the full stacktrace
            if (baseException != context.Exception)
                System.Diagnostics.Trace.WriteLine(context.Exception.StackTrace); 
            
            ApiError apiError = null;
            if (context.Exception is ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(ex.Message);
                apiError.errors = ex.Errors;

                context.HttpContext.Response.StatusCode = ex.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;

                // handle logging here
            }
            else
            {
                // Unhandled errors
#if !DEBUG
                var msg = "An unhandled error occurred.";
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
#endif

                apiError = new ApiError(msg);
                apiError.detail = stack;

                context.HttpContext.Response.StatusCode = 500;

                // handle logging here
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }


}
