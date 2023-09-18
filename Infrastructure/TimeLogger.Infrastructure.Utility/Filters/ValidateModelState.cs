using TimeLogger.Infrastructure.Utility.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Filters
{
    public class ValidateModelState : ActionFilterAttribute
    {
        public ValidateModelState()
        {
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Keys
                               .SelectMany(key => context.ModelState[key].Errors.Select(x => x.ErrorMessage)).ToList();

                context.Result = new BadRequestObjectResult(new
                {
                    success = false,
                    message = JsonSerializer.Serialize(errors).Replace("\"", "")
                });
            }
        }
    }
}
