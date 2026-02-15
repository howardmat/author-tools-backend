using AuthorTools.Api.Models;
using AuthorTools.Api.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;


namespace AuthorTools.Api.Validators.UnitTests;

public class UserSettingCreateRequestValidatorTests
{
    /// <summary>
    /// Verifies that the validator constructor successfully creates an instance.
    /// </summary>
    [Fact]
    public void Constructor_WhenCalled_CreatesValidatorInstance()
    {
        // Arrange & Act
        var validator = new UserSettingCreateRequestValidator();

        // Assert
        validator.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that validation fails when Theme is null.
    /// The NotEmpty() rule should reject null values.
    /// </summary>
    [Fact]
    public void Validate_ThemeIsNull_FailsValidation()
    {
        // Arrange
        var validator = new UserSettingCreateRequestValidator();
        var request = new UserSettingCreateRequest { Theme = null };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Theme);
    }

    /// <summary>
    /// Verifies that validation fails when Theme is an empty string.
    /// The NotEmpty() rule should reject empty strings.
    /// </summary>
    [Fact]
    public void Validate_ThemeIsEmpty_FailsValidation()
    {
        // Arrange
        var validator = new UserSettingCreateRequestValidator();
        var request = new UserSettingCreateRequest { Theme = "" };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Theme);
    }

    /// <summary>
    /// Verifies that validation fails when Theme contains only whitespace.
    /// The NotEmpty() rule should reject whitespace-only strings.
    /// </summary>
    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("   \t  ")]
    public void Validate_ThemeIsWhitespace_FailsValidation(string whitespace)
    {
        // Arrange
        var validator = new UserSettingCreateRequestValidator();
        var request = new UserSettingCreateRequest { Theme = whitespace };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Theme);
    }

    /// <summary>
    /// Verifies that validation passes when Theme has a valid non-empty value.
    /// Various valid theme values should pass the NotEmpty() validation.
    /// </summary>
    [Theory]
    [InlineData("dark")]
    [InlineData("light")]
    [InlineData("a")]
    [InlineData("Theme123")]
    [InlineData("theme-with-dashes")]
    [InlineData("theme_with_underscores")]
    [InlineData("UPPERCASE")]
    [InlineData("MixedCase")]
    public void Validate_ThemeIsValid_PassesValidation(string theme)
    {
        // Arrange
        var validator = new UserSettingCreateRequestValidator();
        var request = new UserSettingCreateRequest { Theme = theme };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Theme);
    }

    /// <summary>
    /// Verifies that validation passes when Theme contains special characters.
    /// Special characters should be allowed as long as the string is not empty.
    /// </summary>
    [Theory]
    [InlineData("theme@123")]
    [InlineData("theme#special")]
    [InlineData("theme$value")]
    [InlineData("theme!@#$%")]
    [InlineData("café")]
    [InlineData("テーマ")]
    public void Validate_ThemeWithSpecialCharacters_PassesValidation(string theme)
    {
        // Arrange
        var validator = new UserSettingCreateRequestValidator();
        var request = new UserSettingCreateRequest { Theme = theme };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Theme);
    }

    /// <summary>
    /// Verifies that validation passes when Theme is a very long string.
    /// The NotEmpty() rule does not impose length restrictions.
    /// </summary>
    [Fact]
    public void Validate_ThemeIsVeryLong_PassesValidation()
    {
        // Arrange
        var validator = new UserSettingCreateRequestValidator();
        var longTheme = new string('a', 10000);
        var request = new UserSettingCreateRequest { Theme = longTheme };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Theme);
    }
}