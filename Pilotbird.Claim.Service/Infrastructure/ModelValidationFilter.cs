using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Pilotbird.Claim.Service.Infrastructure.ApiResponses;

namespace Pilotbird.Claim.Service.Infrastructure
{
    public class ModelValidationFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ValidationException exception)
            {
                if (context.HttpContext.Response != null)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                context.Result = new ObjectResult
                 (
                     new ApiModelStateInvalidBadRequestResponse(exception.Errors)
                 );

                context.ExceptionHandled = true;
            }
            else if (context.Exception is System.ComponentModel.DataAnnotations.ValidationException validationException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new ObjectResult
                 (
                     new ApiBadRequestResponse(validationException.Message)
                 );

                context.ExceptionHandled = true;
            }
        }
    }
}
