using Castle.Core.Internal;
using DigitalGarden.Controllers;
using DigitalGarden.Tests.Helpers;
using DigitalGarden.Tests.Suts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Sinks.InMemory;

namespace DigitalGarden.Tests.ControllerTests;

public class ErrorControllerTests
{
    [Fact]
    public void Controller_HiddenFromApiExplorers()
    {
        var apiExplorerAttribute = typeof(ErrorController)
            .GetAttributes<ApiExplorerSettingsAttribute>()
            .Single();

        apiExplorerAttribute.IgnoreApi.Should().BeTrue();
    }

    [Fact]
    public void Handle_RoutesToAbsoluteSlashError()
    {
        var handleMethod = ControllerTestHelper.GetControllerMethod<ErrorController>(nameof(ErrorController.Handle));

        var routeAttribute = handleMethod
            .GetAttributes<RouteAttribute>()
            .Single();

        routeAttribute.Template.Should().BeEquivalentTo("/error", because: "Error handler set up in Program.cs is expecting this address");
    }

    [Fact]
    public void Handle_ReturnsError500InResponse_WhenCalled()
    {
        var sut = ErrorControllerSut.Create();

        sut.Controller.Handle();

        sut.Controller.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void Handle_ReturnsProblemDetailsResult_WhenCalled()
    {
        var sut = ErrorControllerSut.Create();

        var result = sut.Controller.Handle();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objectResult.Value);
    }

    [Theory]
    [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
    [InlineData("C7122A38-F084-4788-9627-7D86707F52D1")]
    [InlineData("AEAFCE11-67DA-4680-8850-CC37D469D76C")]
    public void Handle_ContainsTraceId_InProblemDetails(string traceGuidString)
    {
        var expectedTraceId = new Guid(traceGuidString);
        var sut = ErrorControllerSut.Create(traceIdentifier: expectedTraceId);

        var result = ((ObjectResult)(sut.Controller.Handle())).Value;
        var problemDetails = Assert.IsType<ProblemDetails>(result);

        problemDetails.Instance.Should().NotBeNullOrWhiteSpace();
        var actualTraceId = new Guid(problemDetails.Instance);
        
        actualTraceId.Should().Be(expectedTraceId);
    }

    [Fact]
    public void Handle_ContainsTitle_InProblemDetails()
    {
        var sut = ErrorControllerSut.Create();

        var result = ((ObjectResult)(sut.Controller.Handle())).Value;
        var problemDetails = Assert.IsType<ProblemDetails>(result);

        problemDetails.Title.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Handle_ContainsError500_InProblemDetails()
    {
        var sut = ErrorControllerSut.Create();

        var result = ((ObjectResult)(sut.Controller.Handle())).Value;
        var problemDetails = Assert.IsType<ProblemDetails>(result);

        problemDetails.Status.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Theory]
    [InlineData("/big-boom")]
    [InlineData("/bigger/boom")]
    [InlineData("/boom?v=the-biggest")]
    public void Handle_ContainsTheErroredPath_InLoggedContextData(string expectedPath)
    {
        // Arrange
        var originalLogger = SetupInMemoryLogger();

        try
        {
            // Arrange some more...
            var sut = ErrorControllerSut.Create(erroredPath: expectedPath);

            // Act
            var result = sut.Controller.Handle();

            // Assert
            var recentEvent = InMemorySink.Instance.LogEvents.Last();
            recentEvent.Properties.Should().ContainKey(ErrorController.RequestPathLogContextName);

            var actualPath = recentEvent.Properties.GetValueOrDefault(ErrorController.RequestPathLogContextName)?.ToString();
            actualPath.Should().NotBeNullOrWhiteSpace();
            actualPath.Should().Contain(expectedPath);
        }
        finally
        {
            // Cleanup
            CleanUpInMemoryLogger(originalLogger);
        }
    }

    [Theory]
    [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
    [InlineData("C7122A38-F084-4788-9627-7D86707F52D1")]
    [InlineData("AEAFCE11-67DA-4680-8850-CC37D469D76C")]
    public void Handle_ContainsTheTraceIdentifier_InLoggedContextData(string traceGuidString)
    {
        // Arrange
        var originalLogger = SetupInMemoryLogger();

        try
        {
            // Arrange some more...
            var traceId = new Guid(traceGuidString);
            var sut = ErrorControllerSut.Create(traceIdentifier: traceId);

            // Act
            var result = sut.Controller.Handle();

            // Assert
            var recentEvent = InMemorySink.Instance.LogEvents.Last();
            recentEvent.Properties.Should().ContainKey(ErrorController.TraceIdentifierLogContextName);

            var actualTraceIdentifier = recentEvent.Properties.GetValueOrDefault(ErrorController.TraceIdentifierLogContextName)?.ToString();
            actualTraceIdentifier.Should().NotBeNullOrWhiteSpace();
            actualTraceIdentifier.Should().ContainEquivalentOf(traceId.ToString("N"));
        }
        finally
        {
            // Cleanup
            CleanUpInMemoryLogger(originalLogger);
        }
    }

    [Theory]
    [InlineData("This is an error message")]
    [InlineData("Another error message!")]
    [InlineData("Error messages for days.")]
    public void Handle_ContainsTheException_InLoggedContextData(string expectedErrorMessage)
    {
        // Arrange
        var originalLogger = SetupInMemoryLogger();

        try
        {
            // Arrange some more...
            var exception = new Exception(expectedErrorMessage);
            var sut = ErrorControllerSut.Create(testException: exception);

            // Act
            var result = sut.Controller.Handle();

            // Assert
            var recentEvent = InMemorySink.Instance.LogEvents.Last();
            recentEvent.Exception.Should().NotBeNull();
            recentEvent.Exception.Message.Should().Be(expectedErrorMessage);
        }
        finally
        {
            // Cleanup
            CleanUpInMemoryLogger(originalLogger);
        }
    }

    private static ILogger SetupInMemoryLogger()
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.InMemory()
            .CreateLogger();

        var original = Log.Logger;
        Log.Logger = logger;

        return original;
    }

    private static void CleanUpInMemoryLogger(ILogger original)
    {
        Log.Logger = original;
        InMemorySink.Instance?.Dispose();
    }
}
