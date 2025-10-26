using DigitalGarden.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Reflection;

namespace DigitalGarden.Tests.Controllers;

public class SeoControllerTests
{
    [Fact]
    public void Sitemap_ResponseCacheMetaData_ShouldBeAsExpected()
    {
        var method = typeof(SeoController).GetMethod(nameof(SeoController.Sitemap), BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(method);         // Not the test itself; just ensure the method is fetched as expected

        var attribute = method
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
        var sut = CreateSut();

        var result = sut.Sitemap();

        Assert.IsType<ContentResult>(result);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://foobar.io")]
    [InlineData("https://www.foo.co.uk")]
    public void Sitemap_ReturnsSitemapXmlContent_WhenBasicRequest(string siteAddress)
    {
        var sut = CreateSut(siteAddress);

        var result = (ContentResult)sut.Sitemap();

        result.ContentType.Should().Be("application/xml");
        result.Content.Should().Contain("<urlset");
        result.Content.Should().Contain("<loc>");
        result.Content.Should().Contain(siteAddress);
    }

    [Fact]
    public void Sitemap_ReturnsEntityTag_WhenBasicRequest()
    {
        var sut = CreateSut();

        sut.Sitemap();
        var entityTag = sut.GetResponseEntityTag();

        entityTag.Should().NotBeNullOrWhiteSpace();
        entityTag.Should().StartWith("W/\"");
        entityTag.Should().EndWith("\"");
    }

    [Fact]
    public void Sitemap_ReturnsStatusCodeResult_WhenRequestHasIfNoneMatchWildcard()
    {
        var sut = CreateSut();
        sut.Request.Headers.IfNoneMatch = "*";

        var result = sut.Sitemap();

        Assert.IsType<StatusCodeResult>(result);
    }

    [Fact]
    public void Sitemap_Returns304_WhenRequestHasIfNoneMatchWildcard()
    {
        var sut = CreateSut();
        sut.Request.Headers.IfNoneMatch = "*";

        var result = (StatusCodeResult)sut.Sitemap();

        result.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public void Sitemap_ReturnsEchoedEntityTag_WhenRequestHasIfNoneMatchWildcard()
    {
        var sut = CreateSut();
        sut.Request.Headers.IfNoneMatch = "*";

        sut.Sitemap();
        var entityTag = sut.GetResponseEntityTag();

        entityTag.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Sitemap_Returns304_WhenRequestHasMatchingIfNoneMatch(bool isStrongClient)
    {
        var clientProvidedTag = GetClientProvidedTag(isStrongClient);
        var sut = CreateSut();
        sut.Request.Headers.IfNoneMatch = clientProvidedTag;

        var result = sut.Sitemap();

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        statusResult.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Sitemap_Returns304_WhenRequestHasAnyMatchingIfNoneMatch(bool isStrongClient)
    {
        var clientProvidedTag = GetClientProvidedTag(isStrongClient);
        var sut = CreateSut();
        sut.Request.Headers.Append(HeaderNames.IfNoneMatch, new[] { "\"FOO\"", clientProvidedTag, "\"BAR\"" });

        var result = sut.Sitemap();

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        statusResult.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Sitemap_ReturnsEchoedEntityTag_WhenRequestHasMatchingIfNoneMatch(bool isStrongClient)
    {
        var clientProvidedTag = GetClientProvidedTag(isStrongClient);
        var sut = CreateSut();
        sut.Request.Headers.IfNoneMatch = clientProvidedTag;

        sut.Sitemap();
        var entityTag = sut.GetResponseEntityTag();

        entityTag.Should().Contain(clientProvidedTag);
    }

    [Fact]
    public void Sitemap_ReturnsContentResult_WhenNonMatchingIfNoneMatch()
    {
        var sut = CreateSut();
        sut.Request.Headers.IfNoneMatch = "\"WrongHash\"";

        var result = sut.Sitemap();

        Assert.IsType<ContentResult>(result);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://foobar.io")]
    [InlineData("https://www.foo.co.uk")]
    public void Sitemap_ReturnsSitemapXmlContent_WhenNonMatchingIfNoneMatch(string siteAddress)
    {
        var sut = CreateSut(siteAddress);
        sut.Request.Headers.IfNoneMatch = "\"WrongHash\"";

        var result = (ContentResult)sut.Sitemap();

        result.ContentType.Should().Be("application/xml");
        result.Content.Should().Contain("<urlset");
        result.Content.Should().Contain("<loc>");
        result.Content.Should().Contain(siteAddress);
    }

    [Fact]
    public void Sitemap_ReturnsEntityTag_WhenNonMatchingIfNoneMatch()
    {
        var sut = CreateSut();
        sut.Request.Headers.IfNoneMatch = "\"WrongHash\"";

        sut.Sitemap();
        var entityTag = sut.GetResponseEntityTag();

        entityTag.Should().NotBeNullOrWhiteSpace();
        entityTag.Should().StartWith("W/\"");
        entityTag.Should().EndWith("\"");
    }

    private SeoController CreateSut()
    {
        return ControllerTestHelper.CreateController<SeoController>();
    }

    private SeoController CreateSut(string siteAddress)
    {
        return ControllerTestHelper.CreateController<SeoController>(siteAddress);
    }

    private string GetClientProvidedTag(bool isStrongClient)
    {
        var tempController = CreateSut();
        var initialResult = tempController.Sitemap();
        var entityTag = tempController.GetResponseEntityTag();
        Assert.NotNull(entityTag);     // Not part of test, but test case needs to ensure valid entity tag

        var tagHash = entityTag.Trim().TrimStart('W', '/');
        return isStrongClient
            ? tagHash
            : $"W/{tagHash}";
    }
}
