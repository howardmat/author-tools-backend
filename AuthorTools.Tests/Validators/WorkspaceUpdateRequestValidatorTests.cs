using AuthorTools.Api.Models;
using AuthorTools.Api.Validators;
using FluentAssertions;
using FluentValidation;
using Xunit;


namespace AuthorTools.Api.Validators.UnitTests;

public class WorkspaceUpdateRequestValidatorTests
{
    /// <summary>
    /// Tests that the validator is successfully instantiated when the constructor is called.
    /// </summary>
    [Fact]
    public void Constructor_WhenCalled_CreatesValidatorInstance()
    {
        // Arrange & Act
        var validator = new WorkspaceUpdateRequestValidator();

        // Assert
        validator.Should().NotBeNull();
        validator.Should().BeAssignableTo<AbstractValidator<WorkspaceUpdateRequest>>();
    }

    /// <summary>
    /// Tests that validation fails when Name is null, empty, or contains only whitespace.
    /// These inputs should be rejected by the NotEmpty() validator rule.
    /// </summary>
    /// <param name="name">The invalid name value to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData(" \t\n ")]
    public void Constructor_ConfiguresNameRule_FailsValidationForNullOrEmptyOrWhitespace(string? name)
    {
        // Arrange
        var validator = new WorkspaceUpdateRequestValidator();
        var request = new WorkspaceUpdateRequest { Name = name };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].PropertyName.Should().Be(nameof(WorkspaceUpdateRequest.Name));
    }

    /// <summary>
    /// Tests that validation passes when Name contains a valid non-empty value.
    /// These inputs should be accepted by the NotEmpty() validator rule.
    /// </summary>
    /// <param name="name">The valid name value to test.</param>
    [Theory]
    [InlineData("ValidName")]
    [InlineData("A")]
    [InlineData("WorkspaceName123")]
    [InlineData("Name with spaces")]
    [InlineData("Special!@#$%Characters")]
    [InlineData("名前")]
    [InlineData("Very Long Name That Contains Many Characters To Test Edge Cases With Lengthy Input Strings")]
    [InlineData(" LeadingSpace")]
    [InlineData("TrailingSpace ")]
    [InlineData("123")]
    [InlineData("!@#$%^&*()")]
    public void Constructor_ConfiguresNameRule_PassesValidationForNonEmptyStrings(string name)
    {
        // Arrange
        var validator = new WorkspaceUpdateRequestValidator();
        var request = new WorkspaceUpdateRequest { Name = name };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that validation only checks the Name property and ignores other properties.
    /// This ensures the validator rule is specifically configured for Name property.
    /// </summary>
    [Fact]
    public void Constructor_ConfiguresNameRule_IgnoresOtherProperties()
    {
        // Arrange
        var validator = new WorkspaceUpdateRequestValidator();
        var request = new WorkspaceUpdateRequest
        {
            Name = "ValidName",
            Description = null,
            Icon = null,
            IsDefault = false
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}