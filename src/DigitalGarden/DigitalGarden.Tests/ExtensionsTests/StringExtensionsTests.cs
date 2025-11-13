using DigitalGarden.Shared.Extensions;

namespace DigitalGarden.Tests.ExtensionsTests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("Single", "single")]
    [InlineData("word", "word")]
    [InlineData("FooBar", "foobar")]
    public void ToSlugString_WhenInputIsOneWord_ReturnsSameWordLowerCase(string input, string expected)
    {
        var result = input.ToSlugString();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Crème", "creme")]
    [InlineData("brûlée", "brulee")]
    [InlineData("Ökonomie", "okonomie")]
    public void ToSlugString_SingleWordWithDiacritics_ReturnsSameWordNoDiacritics(string input, string expected)
    {
        var result = input.ToSlugString();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("Crème brûlée à la carte", "creme-brulee-a-la-carte")]
    [InlineData("   many   spaces   ", "many-spaces")]
    public void ToSlugString_SpacesInInput_ReturnsTrimmedAndReplacedSpacesWithDashes(string input, string expected)
    {
        var result = input.ToSlugString();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello_World!", "hello-world")]
    [InlineData("some/slash/Foobar", "some-slash-foobar")]
    [InlineData("This.Is. A sentance_", "this-is-a-sentance")]
    public void ToSlugString_PeriodsSlashesUnderscoresInInput_ReturnsPunctu(string input, string expected)
    {
        var result = input.ToSlugString();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello, World!", "hello-world")]
    [InlineData("I'd like some $$", "id-like-some")]
    [InlineData("C#/.NET 9 — What's New?", "c-net-9-whats-new")]
    public void ToSlugString_OtherPunctuationInInput_ReturnsPunctu(string input, string expected)
    {
        var result = input.ToSlugString();

        result.Should().Be(expected);
    }
}
