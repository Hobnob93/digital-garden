using DigitalGarden.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DigitalGarden.Attributes;

public class SessionTokenAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var session = context.HttpContext.Session;

        var sessionToken = session.GetString(ApiConstants.SessionTokenName);
        var requestToken = request.Headers[ApiConstants.SessionTokenHeader].FirstOrDefault();

        if (string.IsNullOrEmpty(sessionToken) || sessionToken != requestToken)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}
