using AuthorTools.Api.Models;
using AuthorTools.Api.Validators;
using FluentAssertions;
using Xunit;


namespace AuthorTools.Api.Validators.UnitTests;

public class CommonEntityCreateRequestValidatorTests
{
    /// <summary>
    /// Tests that the validator is constructed successfully and ready to validate.
    /// </summary>
    [Fact]
    public void Constructor_WhenCalled_CreatesValidatorInstance()
    {
        // Arrange & Act
        var validator = new CommonEntityCreateRequestValidator();

        // Assert
        validator.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that validation fails when Name property is null, empty, or whitespace.
    /// These values should fail the NotEmpty validation rule configured in the constructor.
    /// </summary>
    /// <param name="name">The Name value to test (null, empty, or whitespace).</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(" \t \n ")]
    public void Constructor_ConfiguresNameValidation_FailsWhenNameIsNullEmptyOrWhitespace(string? name)
    {
        // Arrange
        var validator = new CommonEntityCreateRequestValidator();
        var request = new CommonEntityCreateRequest { Name = name };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.PropertyName.Should().Be("Name");
    }

    /// <summary>
    /// Tests that validation passes when Name property has a valid non-empty value.
    /// The NotEmpty validation rule should allow any non-null, non-empty, non-whitespace string.
    /// </summary>
    /// <param name="name">A valid Name value to test.</param>
    [Theory]
    [InlineData("a")]
    [InlineData("ValidName")]
    [InlineData("Name with spaces")]
    [InlineData("123")]
    [InlineData("Special!@#$%^&*()Characters")]
    [InlineData("   LeadingSpaces")]
    [InlineData("TrailingSpaces   ")]
    [InlineData("Very long name that exceeds typical length expectations to test that there is no maximum length validation")]
    public void Constructor_ConfiguresNameValidation_PassesWhenNameIsValid(string name)
    {
        // Arrange
        var validator = new CommonEntityCreateRequestValidator();
        var request = new CommonEntityCreateRequest { Name = name };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}