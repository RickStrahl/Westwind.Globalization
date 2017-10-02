using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Westwind.AspNetCore.Extensions
{
    /// <summary>
    /// Unhandled Exception filter attribute for API controllers.
    /// Fires back a common JSON response of type ApiErrorResponse
    /// </summary>
    public class UnhandledApiExceptionFilter : ExceptionFilterAttribute
    {

        public override void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            var exType = context.Exception.GetType();

            if (exType == typeof(UnauthorizedAccessException))
                status = HttpStatusCode.Unauthorized;
            else if (exType == typeof(ArgumentException))
                status = HttpStatusCode.NotFound;

            var apiError = new ApiErrorResponse()
            {
                Message = context.Exception.Message
            };

            context.HttpContext.Response.StatusCode = (int) status;
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}
