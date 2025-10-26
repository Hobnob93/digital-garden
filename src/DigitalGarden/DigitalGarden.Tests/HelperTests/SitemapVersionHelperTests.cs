using DigitalGarden.Helpers;

namespace DigitalGarden.Tests.HelperTests;

public class SitemapVersionHelperTests
{
    public static readonly (string Url, DateTimeOffset? LastModified)[] OneItem =
    [
        ("https://example.com/hello-world", null)
    ];

    public static readonly (string Url, DateTimeOffset? LastModified)[] ThreeItems =
    [
        ("https://example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
        ("https://example.com/hello-world", null),
        ("https://example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero)),
    ];

    public static readonly (string Url, DateTimeOffset? LastModified)[] ThreeItemsDifferentHost =
    [
        ("https://different-example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
        ("https://different-example.com/hello-world", null),
        ("https://different-example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero)),
    ];

    [Fact]
    public void Compute_ReturnsEmptyString_WhenInputIsEmpty()
    {
        var data = Array.Empty<(string, DateTimeOffset?)>();
        var expected = string.Empty;

        var hash = SitemapVersionHelper.Compute(data);

        hash.Should().Be(expected);
    }

    [Fact]
    public void Compute_ReturnsHash_ForSingleItem()
    {
        var data = OneItem;

        var hash = SitemapVersionHelper.Compute(data);

        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Compute_IsDeterministic_WithIdenticalInput()
    {
        var data = OneItem;

        var hash1 = SitemapVersionHelper.Compute(data);
        var hash2 = SitemapVersionHelper.Compute(data);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_ReturnsSameHash_RegardlessOfCollectionOrder()
    {
        var data = ThreeItems;

        var dataReversed = data.Reverse();

        var hash1 = SitemapVersionHelper.Compute(data);
        var hash2 = SitemapVersionHelper.Compute(dataReversed);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_ReturnsDifferentHashes_ForDifferentData()
    {
        var data1 = ThreeItems;
        var data2 = ThreeItemsDifferentHost;

        var hash1 = SitemapVersionHelper.Compute(data1);
        var hash2 = SitemapVersionHelper.Compute(data2);

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Compute_ReturnsSameHash_WithTrailingOrLeadingWhitespace()
    {
        var data1 = ThreeItems;
        var data2 = new[]
        {
            ($" {ThreeItems[0].Url}", ThreeItems[0].LastModified),
            ($"{ThreeItems[1].Url}   ", ThreeItems[1].LastModified),
            ($"   {ThreeItems[2].Url} ", ThreeItems[2].LastModified)
        };

        var hash1 = SitemapVersionHelper.Compute(data1);
        var hash2 = SitemapVersionHelper.Compute(data2);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_ReturnsSameHash_WithDifferentCasing()
    {
        var data1 = ThreeItems;
        var data2 = new[]
        {
            (ThreeItems[0].Url.ToUpperInvariant(), ThreeItems[0].LastModified),
            (ThreeItems[1].Url.ToUpperInvariant(), ThreeItems[1].LastModified),
            (ThreeItems[2].Url.ToUpperInvariant(), ThreeItems[2].LastModified)
        };

        var hash1 = SitemapVersionHelper.Compute(data1);
        var hash2 = SitemapVersionHelper.Compute(data2);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_ReturnsExactHash_ForKnownDataSet()
    {
        var expected = "7F6363A642B36F3B20740FAF9CD45E6C51E196E5CE7195C09930B6FC76D098C8";
        var data = ThreeItems;

        var hash = SitemapVersionHelper.Compute(data);

        hash.Should().Be(expected);
    }

    [Fact]
    public void Compute_ReturnsDifferentHash_WhenLastModifiedDifferent()
    {
        var data1 = ThreeItems;
        var data2 = new[]
        {
            (ThreeItems[0].Url.ToUpperInvariant(), ThreeItems[0].LastModified!.Value.AddMinutes(1)),
            (ThreeItems[1].Url.ToUpperInvariant(), ThreeItems[1].LastModified),
            (ThreeItems[2].Url.ToUpperInvariant(), ThreeItems[2].LastModified)
        };

        var hash1 = SitemapVersionHelper.Compute(data1);
        var hash2 = SitemapVersionHelper.Compute(data2);

        hash1.Should().NotBe(hash2);
    }
}
