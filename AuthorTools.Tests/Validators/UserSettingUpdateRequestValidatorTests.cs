using AuthorTools.Api.Models;
using AuthorTools.Api.Validators;
using FluentAssertions;
using Xunit;


namespace AuthorTools.Api.Validators.UnitTests;

public class UserSettingUpdateRequestValidatorTests
{
    /// <summary>
    /// Verifies that the constructor successfully creates a validator instance.
    /// </summary>
    [Fact]
    public void Constructor_WhenCalled_CreatesValidatorInstance()
    {
        // Arrange & Act
        var validator = new UserSettingUpdateRequestValidator();

        // Assert
        validator.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that validation fails when Theme is null, empty, or whitespace.
    /// Null, empty, and whitespace values should violate the NotEmpty rule.
    /// </summary>
    /// <param name="theme">The invalid theme value to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void Validate_ThemeIsNullEmptyOrWhitespace_ValidationFails(string? theme)
    {
        // Arrange
        var validator = new UserSettingUpdateRequestValidator();
        var request = new UserSettingUpdateRequest { Theme = theme };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.PropertyName.Should().Be(nameof(UserSettingUpdateRequest.Theme));
    }

    /// <summary>
    /// Verifies that validation succeeds when Theme contains a valid non-empty string.
    /// Valid themes should satisfy the NotEmpty rule.
    /// </summary>
    /// <param name="theme">The valid theme value to test.</param>
    [Theory]
    [InlineData("dark")]
    [InlineData("light")]
    [InlineData("a")]
    [InlineData("theme with spaces")]
    [InlineData("theme-with-dashes")]
    [InlineData("theme_with_underscores")]
    [InlineData("ThemeWithCapitals")]
    [InlineData("123456")]
    [InlineData("!@#$%^&*()")]
    public void Validate_ThemeIsNonEmptyString_ValidationSucceeds(string theme)
    {
        // Arrange
        var validator = new UserSettingUpdateRequestValidator();
        var request = new UserSettingUpdateRequest { Theme = theme };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that validation succeeds for very long theme strings.
    /// Ensures no length restriction is enforced by the NotEmpty rule.
    /// </summary>
    [Fact]
    public void Validate_ThemeIsVeryLongString_ValidationSucceeds()
    {
        // Arrange
        var validator = new UserSettingUpdateRequestValidator();
        var veryLongTheme = new string('a', 10000);
        var request = new UserSettingUpdateRequest { Theme = veryLongTheme };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}