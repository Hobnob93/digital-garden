using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DigitalGarden.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    public const string RequestPathLogContextName = "RequestPath";
    public const string TraceIdentifierLogContextName = "TraceIdentifier";

    [Route("/error")]
    public IActionResult Handle()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (feature?.Error is not null)
        {
            Log.ForContext(RequestPathLogContextName, feature.Path)
               .ForContext(TraceIdentifierLogContextName, HttpContext.TraceIdentifier)
               .Error(feature.Error, "Unhandled exception");
        }

        HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return Problem(
            title: "An unexpected error occurred",
            instance: HttpContext.TraceIdentifier,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
}
