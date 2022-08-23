using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Theoremone.SmartAc.Application.Common.Exceptions;

namespace Theoremone.SmartAc.Api.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(UnauthorizedAccessException), HandleUnauthorizedException },
              
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUnauthorizedException(ExceptionContext context)
        {
       
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Detail = context.Exception.Message,
                Title = "An error occurred while processing your request.",
            };

            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;

        }


        private void HandleUnknownException(ExceptionContext context)
        {
            if (context.Exception is AggregateException)
            {
                foreach (var innerException in (context.Exception as AggregateException).Flatten().InnerExceptions)
                {
                    _logger.LogError(innerException, innerException.Message);
                }
            }
            else
            {
                _logger.LogError(context.Exception, context.Exception.Message);
            }

           
            _logger.LogError(context.Exception, "SmartAc Request:HttpResponse Exception");

            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Detail = context.Exception.Message,
                Title = "An error occurred while processing your request.",
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}
