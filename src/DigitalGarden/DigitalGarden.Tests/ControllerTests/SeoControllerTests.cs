using DigitalGarden.Controllers;
using DigitalGarden.Tests.Helpers;
using DigitalGarden.Tests.Suts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DigitalGarden.Tests.ControllerTests;

public class SeoControllerTests
{
    [Fact]
    public void Sitemap_ResponseCacheMetaData_ShouldBeAsExpected()
    {
        var methodInfo = ControllerTestHelper.GetControllerMethod<SeoController>(nameof(SeoController.Sitemap));

        var attribute = methodInfo
            .GetCustomAttributes(typeof(ResponseCacheAttribute), inherit: false)
            .Cast<ResponseCacheAttribute>()
            .Single();

        attribute.Duration.Should().Be(86400);
        attribute.Location.Should().Be(ResponseCacheLocation.Any);
        attribute.NoStore.Should().BeFalse();
    }

    [Fact]
    public void Sitemap_ReturnsContentResult_WhenBasicRequest()
    {
        var sut = SeoControllerSut.Create();

        var result = sut.Controller.Sitemap();

        Assert.IsType<ContentResult>(result);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://foobar.io")]
    [InlineData("https://www.foo.co.uk")]
    public void Sitemap_ReturnsSitemapXmlContent_WhenBasicRequest(string siteAddress)
    {
        var sut = SeoControllerSut.Create(siteAddress: siteAddress);

        var result = (ContentResult)sut.Controller.Sitemap();

        result.ContentType.Should().Be("application/xml");
        result.Content.Should().Contain("<urlset");
        result.Content.Should().Contain("<loc>");
        result.Content.Should().Contain(siteAddress);
    }

    [Fact]
    public void Sitemap_ReturnsEntityTag_WhenBasicRequest()
    {
        var sut = SeoControllerSut.Create();

        _ = sut.Controller.Sitemap();
        var entityTag = sut.Controller.GetResponseEntityTag();

        entityTag.Should().NotBeNullOrWhiteSpace();
        entityTag.Should().StartWith("W/\"");
        entityTag.Should().EndWith("\"");
    }

    [Fact]
    public void Sitemap_ReturnsStatusCodeResult_WhenRequestHasIfNoneMatchWildcard()
    {
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.IfNoneMatch = "*";

        var result = sut.Controller.Sitemap();

        Assert.IsType<StatusCodeResult>(result);
    }

    [Fact]
    public void Sitemap_Returns304_WhenRequestHasIfNoneMatchWildcard()
    {
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.IfNoneMatch = "*";

        var result = (StatusCodeResult)sut.Controller.Sitemap();

        result.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public void Sitemap_ReturnsEchoedEntityTag_WhenRequestHasIfNoneMatchWildcard()
    {
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.IfNoneMatch = "*";

        _ = sut.Controller.Sitemap();
        var entityTag = sut.Controller.GetResponseEntityTag();

        entityTag.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Sitemap_Returns304_WhenRequestHasMatchingIfNoneMatch(bool isStrongClient)
    {
        var clientProvidedTag = GetClientProvidedTag(isStrongClient);
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.IfNoneMatch = clientProvidedTag;

        var result = sut.Controller.Sitemap();

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        statusResult.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Sitemap_Returns304_WhenRequestHasAnyMatchingIfNoneMatch(bool isStrongClient)
    {
        var clientProvidedTag = GetClientProvidedTag(isStrongClient);
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.Append(HeaderNames.IfNoneMatch, new[] { "\"FOO\"", clientProvidedTag, "\"BAR\"" });

        var result = sut.Controller.Sitemap();

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        statusResult.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Sitemap_ReturnsEchoedEntityTag_WhenRequestHasMatchingIfNoneMatch(bool isStrongClient)
    {
        var clientProvidedTag = GetClientProvidedTag(isStrongClient);
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.IfNoneMatch = clientProvidedTag;

        _ = sut.Controller.Sitemap();
        var entityTag = sut.Controller.GetResponseEntityTag();

        entityTag.Should().Contain(clientProvidedTag);
    }

    [Fact]
    public void Sitemap_ReturnsContentResult_WhenNonMatchingIfNoneMatch()
    {
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.IfNoneMatch = "\"WrongHash\"";

        var result = sut.Controller.Sitemap();

        Assert.IsType<ContentResult>(result);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://foobar.io")]
    [InlineData("https://www.foo.co.uk")]
    public void Sitemap_ReturnsSitemapXmlContent_WhenNonMatchingIfNoneMatch(string siteAddress)
    {
        var sut = SeoControllerSut.Create(siteAddress: siteAddress);
        sut.Controller.Request.Headers.IfNoneMatch = "\"WrongHash\"";

        var result = (ContentResult)sut.Controller.Sitemap();

        result.ContentType.Should().Be("application/xml");
        result.Content.Should().Contain("<urlset");
        result.Content.Should().Contain("<loc>");
        result.Content.Should().Contain(siteAddress);
    }

    [Fact]
    public void Sitemap_ReturnsEntityTag_WhenNonMatchingIfNoneMatch()
    {
        var sut = SeoControllerSut.Create();
        sut.Controller.Request.Headers.IfNoneMatch = "\"WrongHash\"";

        _ = sut.Controller.Sitemap();
        var entityTag = sut.Controller.GetResponseEntityTag();

        entityTag.Should().NotBeNullOrWhiteSpace();
        entityTag.Should().StartWith("W/\"");
        entityTag.Should().EndWith("\"");
    }

    [Theory]
    [InlineData("https://example.com", new string[] { "/", "/foo" })]
    [InlineData("https://different-example.com", new string[] { "/", "/foo", "/bar?foo=yes" })]
    [InlineData("https://different-example.com", new string[] { "/", "/some-address", "/some-other-address", "/some/deep/address" })]
    public void Sitemap_ReturnsEveryPublicUrl_GivenByUrlsProvider(string baseAddress, string[] relativeUrls)
    {
        var sut = SeoControllerSut.Create(siteAddress: baseAddress);
        sut.UrlsProvider.GetPublicRelativeUrls().Returns(relativeUrls);

        var result = (ContentResult)sut.Controller.Sitemap();

        foreach (var relativeUrl in relativeUrls)
        {
            var fullUrl = $"{baseAddress}{relativeUrl}";
            result.Content.Should().Contain($"<loc>{fullUrl}</loc>");
        }
    }

    private string GetClientProvidedTag(bool isStrongClient)
    {
        var tempSut = SeoControllerSut.Create();
        _ = tempSut.Controller.Sitemap();
        var entityTag = tempSut.Controller.GetResponseEntityTag();
        Assert.NotNull(entityTag);     // Not part of test, but test case needs to ensure valid entity tag

        var tagHash = entityTag.Trim().TrimStart('W', '/');
        return isStrongClient
            ? tagHash
            : $"W/{tagHash}";
    }
}
