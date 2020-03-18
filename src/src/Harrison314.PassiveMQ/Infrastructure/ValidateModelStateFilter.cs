using Harrison314.PassiveMQ.Models;
using Harrison314.PassiveMQ.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public ValidateModelStateFilter()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            string[] validationErrors = context.ModelState
                .Keys
                .SelectMany(k => context.ModelState[k].Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            ErrorResponseDto json = new ErrorResponseDto()
            {
                ExceptionName = nameof(PassiveMQValidationException),
                Messages = validationErrors
            };

            context.Result = new BadRequestObjectResult(json);
        }
    }
}
