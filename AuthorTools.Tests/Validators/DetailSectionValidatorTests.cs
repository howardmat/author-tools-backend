using AuthorTools.Api.Validators;
using AuthorTools.Data.Models;
using FluentAssertions;
using Xunit;
using Attribute = AuthorTools.Data.Models.Attribute;


namespace AuthorTools.Api.Validators.UnitTests;

/// <summary>
/// Unit tests for the <see cref="DetailSectionValidator"/> class.
/// </summary>
public class DetailSectionValidatorTests
{
    /// <summary>
    /// Verifies that the DetailSectionValidator constructor successfully initializes the validator.
    /// </summary>
    [Fact]
    public void Constructor_WhenCalled_InitializesValidator()
    {
        // Act
        var validator = new DetailSectionValidator();

        // Assert
        validator.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that validation passes when all required properties are valid.
    /// </summary>
    [Fact]
    public void Validate_ValidDetailSection_ValidationPasses()
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "valid-id",
            Title = "Valid Title",
            Attributes = new[]
            {
                new Attribute { Id = "attr1", Label = "Label1" }
            }
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that validation passes when Attributes collection is empty.
    /// </summary>
    [Fact]
    public void Validate_EmptyAttributesCollection_ValidationPasses()
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "valid-id",
            Title = "Valid Title",
            Attributes = []
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that validation fails when Id is null, empty, or whitespace.
    /// </summary>
    /// <param name="id">The Id value to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_InvalidId_ValidationFails(string? id)
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = id!,
            Title = "Valid Title",
            Attributes = []
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Id");
    }

    /// <summary>
    /// Verifies that validation fails when Title is null, empty, or whitespace.
    /// </summary>
    /// <param name="title">The Title value to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_InvalidTitle_ValidationFails(string? title)
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "valid-id",
            Title = title,
            Attributes = []
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title");
    }

    /// <summary>
    /// Verifies that validation fails when an Attribute in the collection has an invalid Id.
    /// </summary>
    /// <param name="attributeId">The attribute Id value to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Validate_AttributeWithInvalidId_ValidationFails(string? attributeId)
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "valid-id",
            Title = "Valid Title",
            Attributes = new[]
            {
                new Attribute { Id = attributeId!, Label = "Valid Label" }
            }
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Attributes[0].Id");
    }

    /// <summary>
    /// Verifies that validation fails when an Attribute in the collection has an invalid Label.
    /// </summary>
    /// <param name="label">The attribute Label value to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Validate_AttributeWithInvalidLabel_ValidationFails(string? label)
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "valid-id",
            Title = "Valid Title",
            Attributes = new[]
            {
                new Attribute { Id = "valid-id", Label = label }
            }
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Attributes[0].Label");
    }

    /// <summary>
    /// Verifies that validation fails when multiple attributes have validation errors.
    /// </summary>
    [Fact]
    public void Validate_MultipleInvalidAttributes_ValidationFails()
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "valid-id",
            Title = "Valid Title",
            Attributes = new[]
            {
                new Attribute { Id = "", Label = "Valid Label" },
                new Attribute { Id = "valid-id", Label = "" },
                new Attribute { Id = "", Label = "" }
            }
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(0);
        result.Errors.Should().Contain(e => e.PropertyName == "Attributes[0].Id");
        result.Errors.Should().Contain(e => e.PropertyName == "Attributes[1].Label");
        result.Errors.Should().Contain(e => e.PropertyName == "Attributes[2].Id");
        result.Errors.Should().Contain(e => e.PropertyName == "Attributes[2].Label");
    }

    /// <summary>
    /// Verifies that validation fails when both Id and Title are invalid.
    /// </summary>
    [Fact]
    public void Validate_MultiplePropertiesInvalid_ValidationFails()
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "",
            Title = "",
            Attributes = []
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    /// <summary>
    /// Verifies that validation fails when Id, Title, and Attributes are all invalid.
    /// </summary>
    [Fact]
    public void Validate_AllPropertiesInvalid_ValidationFails()
    {
        // Arrange
        var validator = new DetailSectionValidator();
        var detailSection = new DetailSection
        {
            Id = "",
            Title = null,
            Attributes = new[]
            {
                new Attribute { Id = "", Label = null }
            }
        };

        // Act
        var result = validator.Validate(detailSection);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
        result.Errors.Should().Contain(e => e.PropertyName == "Attributes[0].Id");
        result.Errors.Should().Contain(e => e.PropertyName == "Attributes[0].Label");
    }
}