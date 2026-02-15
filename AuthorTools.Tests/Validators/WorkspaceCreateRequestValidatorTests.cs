using AuthorTools.Api.Models;
using AuthorTools.Api.Validators;
using FluentAssertions;
using FluentValidation;
using Xunit;


namespace AuthorTools.Api.Validators.UnitTests;

public class WorkspaceCreateRequestValidatorTests
{
    /// <summary>
    /// Tests that the validator correctly rejects WorkspaceCreateRequest objects with invalid Name values.
    /// Invalid values include null, empty strings, and whitespace-only strings.
    /// Expected result: Validation should fail (IsValid = false).
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("  \t  ")]
    public void Constructor_InvalidNameValues_ValidationFails(string? name)
    {
        // Arrange
        var validator = new WorkspaceCreateRequestValidator();
        var request = new WorkspaceCreateRequest { Name = name };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == "Name");
    }

    /// <summary>
    /// Tests that the validator correctly accepts WorkspaceCreateRequest objects with valid Name values.
    /// Valid values are non-empty, non-whitespace strings.
    /// Expected result: Validation should pass (IsValid = true).
    /// </summary>
    [Theory]
    [InlineData("ValidName")]
    [InlineData("Valid Name")]
    [InlineData("Valid@Name#123")]
    [InlineData("a")]
    [InlineData("123")]
    [InlineData("  ValidName  ")]
    [InlineData("Name with special chars: !@#$%^&*()")]
    public void Constructor_ValidNameValues_ValidationPasses(string name)
    {
        // Arrange
        var validator = new WorkspaceCreateRequestValidator();
        var request = new WorkspaceCreateRequest { Name = name };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the validator correctly validates very long name strings.
    /// Expected result: Validation should pass as there's no length restriction.
    /// </summary>
    [Fact]
    public void Constructor_VeryLongName_ValidationPasses()
    {
        // Arrange
        var validator = new WorkspaceCreateRequestValidator();
        var longName = new string('a', 10000);
        var request = new WorkspaceCreateRequest { Name = longName };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the validator validates other properties independently.
    /// Even with invalid Name, other properties should not be validated (no rules defined for them).
    /// Expected result: Only Name should have validation errors.
    /// </summary>
    [Fact]
    public void Constructor_InvalidNameWithOtherProperties_OnlyNameValidationFails()
    {
        // Arrange
        var validator = new WorkspaceCreateRequestValidator();
        var request = new WorkspaceCreateRequest
        {
            Name = null,
            Description = "Some description",
            Icon = "icon.png",
            IsDefault = true
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.Should().ContainSingle(error => error.PropertyName == "Name");
    }
}