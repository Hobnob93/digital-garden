using DigitalGarden.Controllers;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;
using DigitalGarden.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalGarden.Tests.Suts;

public static class ConfigControllerSut
{
    /// <summary>
    /// A class to represent the SUT when testing the <see cref="ConfigController"/> instance
    /// </summary>
    /// <param name="Controller">The actual controller instance being tested</param>
    /// <param name="ConfigProvider">A dependency providing site's configuration options</param>
    public record Sut
    (
        ConfigController Controller,
        ISiteConfigurationProvider ConfigProvider
    );

    /// <summary>
    /// Creates a working "out-of-the-box" instance of the controller along with default-mocked dependency instances
    /// </summary>
    /// <param name="configProvider">A dependency providing site's configuration options</param>
    /// <param name="flagOptions">Options for config-controlled flags</param>
    /// <returns>The SUT instance</returns>
    public static Sut Create(
        ISiteConfigurationProvider? configProvider = null,
        GeneralFlagOptions? flagOptions = null)
    {
        flagOptions ??= new GeneralFlagOptions();

        configProvider ??= Substitute.For<ISiteConfigurationProvider>();
        configProvider
            .GetSiteFlagOptionsAsync()
            .Returns(flagOptions);

        var controller = ControllerTestHelper.CreateController<ConfigController>(
            services =>
            {
                services.AddSingleton(configProvider);
            });

        return new Sut(controller, configProvider);
    }
}
