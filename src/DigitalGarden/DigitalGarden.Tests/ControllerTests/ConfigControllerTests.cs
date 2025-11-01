using DigitalGarden.Controllers;
using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Tests.Helpers;
using DigitalGarden.Tests.Suts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DigitalGarden.Tests.ControllerTests;

public class ConfigControllerTests
{
    [Fact]
    public void GetGeneralFlagOptions_HttpVerbMetaData_IsGetOnly()
    {
        var methodInfo = ControllerTestHelper.GetControllerMethod<ConfigController>(nameof(ConfigController.GetGeneralFlagOptions));

        var httpAttributes = methodInfo
            .GetCustomAttributes(typeof(HttpMethodAttribute), inherit: false)
            .Cast<HttpMethodAttribute>();

        httpAttributes.Should().HaveCount(1);
        httpAttributes.Single().Should().BeOfType<HttpGetAttribute>();
    }

    [Fact]
    public void GetGeneralFlagOptions_HttpVerbMetaData_UsesActionName()
    {
        var methodInfo = ControllerTestHelper.GetControllerMethod<ConfigController>(nameof(ConfigController.GetGeneralFlagOptions));

        var attribute = methodInfo
            .GetCustomAttributes(typeof(HttpGetAttribute), inherit: false)
            .Cast<HttpGetAttribute>()
            .Single();

        attribute.Name.Should().Be(nameof(ConfigController.GetGeneralFlagOptions));
    }

    [Fact]
    public void GetGeneralFlagOptions_ProducesMetaData_200Ok()
    {
        var methodInfo = ControllerTestHelper.GetControllerMethod<ConfigController>(nameof(ConfigController.GetGeneralFlagOptions));

        var attribute = methodInfo
            .GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: false)
            .Cast<ProducesResponseTypeAttribute>()
            .Single();

        attribute.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public void GetGeneralFlagOptions_ProducesMetaData_CorrectDataType()
    {
        var methodInfo = ControllerTestHelper.GetControllerMethod<ConfigController>(nameof(ConfigController.GetGeneralFlagOptions));

        var attribute = methodInfo
            .GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: false)
            .Cast<ProducesResponseTypeAttribute>()
            .Single();

        attribute.Type.Should().Be(typeof(GeneralFlagOptions));
    }

    [Fact]
    public async Task GetGeneralFlagOptions_ReturnsOkObjectResult()
    {
        var sut = ConfigControllerSut.Create();

        var result = await sut.Controller.GetGeneralFlagOptions();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetGeneralFlagOptions_Returns200Ok()
    {
        var sut = ConfigControllerSut.Create();

        var result = await sut.Controller.GetGeneralFlagOptions();
        var objectResult = (OkObjectResult)result;

        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task GetGeneralFlagOptions_ReturnsExpectedOptionsInstance()
    {
        var expectedOptionsInstance = new GeneralFlagOptions();
        var sut = ConfigControllerSut.Create(flagOptions: expectedOptionsInstance);

        var result = await sut.Controller.GetGeneralFlagOptions();
        var objectResult = (OkObjectResult)result;

        objectResult.Value.Should().Be(expectedOptionsInstance);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetGeneralFlagOptions_ReturnedValue_HasCorrectWipState(bool expectedWipState)
    {
        var expectedOptionsInstance = new GeneralFlagOptions
        {
            IsWip = expectedWipState
        };
        var sut = ConfigControllerSut.Create(flagOptions: expectedOptionsInstance);

        var result = await sut.Controller.GetGeneralFlagOptions();
        var objectResult = (OkObjectResult)result;
        var resultValue = objectResult.Value as GeneralFlagOptions;

        resultValue.Should().NotBeNull();
        resultValue.IsWip.Should().Be(expectedWipState);
    }
}
