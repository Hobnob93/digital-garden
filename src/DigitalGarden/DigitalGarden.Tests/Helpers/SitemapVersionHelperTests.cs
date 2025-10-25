using DigitalGarden.Helpers;

namespace DigitalGarden.Tests.Helpers;

public class SitemapVersionHelperTests
{
    [Fact]
    public void Compute_CanHash()
    {
        var data = new[]
        {
            ("https://example.com/hello-world", (DateTimeOffset?)null),
        };

        var hash = SitemapVersionHelper.Compute(data);

        hash.Should().NotBeNull();
        hash.Should().NotBeEmpty();
    }

    [Fact]
    public void Compute_HashResultIsDeterministic()
    {
        var data = new[]
        {
            ("https://example.com/hello-world", (DateTimeOffset?)null),
        };

        var hash1 = SitemapVersionHelper.Compute(data);
        var hash2 = SitemapVersionHelper.Compute(data);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_SameItemsDifferentOrder_SameResult()
    {
        var data = new[]
        {
            ("https://example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://example.com/hello-world", (DateTimeOffset?)null),
            ("https://example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var dataReversed = data.Reverse();

        var hash1 = SitemapVersionHelper.Compute(data);
        var hash2 = SitemapVersionHelper.Compute(dataReversed);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_DifferentData_DifferentResults()
    {
        var data1 = new[]
        {
            ("https://example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://example.com/hello-world", (DateTimeOffset?)null),
            ("https://example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var data2 = new[]
        {
            ("https://different-example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://different-example.com/hello-world", (DateTimeOffset?)null),
            ("https://different-example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var hash1 = SitemapVersionHelper.Compute(data1);
        var hash2 = SitemapVersionHelper.Compute(data2);

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Compute_TrailingOrLeadingWhitespace_SameResults()
    {
        var data1 = new[]
        {
            ("https://example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://example.com/hello-world", (DateTimeOffset?)null),
            ("https://example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var data2 = new[]
        {
            (" https://example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://example.com/hello-world   ", (DateTimeOffset?)null),
            ("   https://example.com/foo-bar ", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var hash1 = SitemapVersionHelper.Compute(data1);
        var hash2 = SitemapVersionHelper.Compute(data2);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_DifferentCasing_SameResults()
    {
        var data1 = new[]
        {
            ("https://example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://example.com/hello-world", (DateTimeOffset?)null),
            ("https://example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var data2 = new[]
        {
            ("https://EXAMPLE.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://example.com/HELLO-world", (DateTimeOffset?)null),
            ("HTTPS://EXAMPLE.COM/FOO-BAR", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var hash1 = SitemapVersionHelper.Compute(data1);
        var hash2 = SitemapVersionHelper.Compute(data2);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Compute_KnownDataSet_ProducesExactHash()
    {
        var expected = "7F6363A642B36F3B20740FAF9CD45E6C51E196E5CE7195C09930B6FC76D098C8";
        var data = new[]
        {
            ("https://example.com/bar-foo", new DateTimeOffset(2025, 1, 13, 11, 45, 5, TimeSpan.Zero)),
            ("https://example.com/hello-world", (DateTimeOffset?)null),
            ("https://example.com/foo-bar", new DateTimeOffset(2023, 12, 31, 15, 30, 0, TimeSpan.Zero))
        };

        var hash = SitemapVersionHelper.Compute(data);

        hash.Should().Be(expected);
    }
}
