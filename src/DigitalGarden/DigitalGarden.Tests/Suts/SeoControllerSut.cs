using DigitalGarden.Controllers;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalGarden.Tests.Suts;

public static class SeoControllerSut
{
    /// <summary>
    /// A class to represent the SUT when testing the <see cref="SeoController"/> instance
    /// </summary>
    /// <param name="Controller">The actual controller instance being tested</param>
    /// <param name="UrlsProvider">A dependency providing relative publicly available URLs</param>
    public record Sut
    (
        SeoController Controller,
        ISitemapRelativeUrlsProvider UrlsProvider
    );

    /// <summary>
    /// Creates a working "out-of-the-box" instance of the controller along with default-mocked dependency instances
    /// </summary>
    /// <param name="urlsProvider">A dependency providing relative URLs to be shown in the sitemap output</param>
    /// <param name="siteAddress">Determine the controller's scheme and host values</param>
    /// <returns></returns>
    public static Sut Create(
        ISitemapRelativeUrlsProvider? urlsProvider = null,
        string siteAddress = "https://example.com")
    {
        urlsProvider ??= Substitute.For<ISitemapRelativeUrlsProvider>();
        urlsProvider.GetPublicRelativeUrls().Returns([ "/" ]);

        var controller = ControllerTestHelper.CreateController<SeoController>(
            services =>
            {
                services.AddSingleton(urlsProvider);
            },
            siteAddress: siteAddress);

        return new Sut(controller, urlsProvider);
    }
}
