using Harrison314.PassiveMQ.Models;
using Harrison314.PassiveMQ.Models.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Infrastructure
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment enviroment;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment enviroment, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.enviroment = enviroment;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            this.logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            if (context.Exception is PassiveMQException)
            {
                ErrorResponseDto json = new ErrorResponseDto()
                {
                    ExceptionName = context.Exception.GetType().Name,
                    Messages = new[] { context.Exception.Message }
                };

                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                ErrorResponseDto json = new ErrorResponseDto()
                {
                    Messages = new[] { "An error ocurr.Try it again." }
                };

                if (this.enviroment.IsDevelopment())
                {
                    json.DeveloperMeesage = context.Exception.ToString();
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;
        }
    }
}
