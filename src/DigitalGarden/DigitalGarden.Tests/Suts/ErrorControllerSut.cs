using DigitalGarden.Controllers;
using DigitalGarden.Tests.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
namespace DigitalGarden.Tests.Suts;

public static class ErrorControllerSut
{
    /// <summary>
    /// A class to represent the SUT when testing the <see cref="ErrorController"/> instance
    /// </summary>
    /// <param name="Controller">The actual controller instance being tested</param>
    public record Sut
    (
        ErrorController Controller
    );

    /// <summary>
    /// Creates a working "out-of-the-box" instance of the controller along with default-mocked dependency instances
    /// </summary>
    /// <param name="testException">A dummy exception that caused the error endpoint to be hit</param>
    /// <param name="erroredPath">The path at which the pretend-exception occurred</param>
    /// <returns>The SUT instance</returns>
    public static Sut Create(
        Exception? testException = null,
        string? erroredPath = null,
        Guid? traceIdentifier = null)
    {
        testException ??= new Exception("This is a test error");
        erroredPath ??= "/boom";
        traceIdentifier ??= Guid.NewGuid();

        var controller = ControllerTestHelper.CreateController<ErrorController>();
        controller.HttpContext.TraceIdentifier = traceIdentifier.Value.ToString("N");
        controller.HttpContext.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature
        {
            Path = erroredPath,
            Error = testException
        });

        return new Sut(controller);
    }
}
