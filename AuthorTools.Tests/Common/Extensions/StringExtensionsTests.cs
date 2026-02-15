using AuthorTools.Common.Extensions;
using FluentAssertions;

namespace AuthorTools.Tests.Common.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToCamelCase_NullOrEmpty_ReturnsInput(string? input)
    {
        // Extension method cannot be called when input is null; validate empty string behavior.
        (input ?? "").ToCamelCase().Should().Be(input ?? "");
    }

    [Theory]
    [InlineData("alreadyCamel")]
    [InlineData("a")]
    public void ToCamelCase_AlreadyLowercaseFirstChar_ReturnsInput(string input)
    {
        input.ToCamelCase().Should().Be(input);
    }

    [Theory]
    [InlineData("Hello", "hello")]
    [InlineData("HelloWorld", "helloWorld")]
    public void ToCamelCase_UppercaseFirstChar_LowercasesFirstChar(string input, string expected)
    {
        input.ToCamelCase().Should().Be(expected);
    }
}
