using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Security2.Web
{
    public class ActionTestFilter : Attribute, IActionFilter
    {
        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}