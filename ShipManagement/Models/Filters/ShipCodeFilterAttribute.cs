
using Microsoft.AspNetCore.Mvc.Filters;
using ShipManagement.Models.Attributes;

namespace ShipManagement.Models.Filters
{
    public class ShipCodeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("shipCode", out var value))
            {
                if (!ShipCodeValidation.IsValid(value, out var errorMessage))
                {
                    context.Result = new BadRequestObjectResult(errorMessage);
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult(ShipCodeValidation.REQUIRED_MESSAGE);
            }
        }
    }
}