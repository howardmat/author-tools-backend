using AuthorTools.Api.Models;
using AuthorTools.Api.Validators;
using AuthorTools.Data.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;


namespace AuthorTools.Api.Validators.UnitTests;

public class CommonEntityUpdateRequestValidatorTests
{
    /// <summary>
    /// Tests that the validator correctly validates a request with valid Name and empty DetailSections.
    /// </summary>
    [Fact]
    public void Constructor_ValidNameAndEmptyDetailSections_PassesValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = []
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that the validator fails validation when Name is null, empty, or whitespace.
    /// </summary>
    /// <param name="name">The Name value to test (null, empty, or whitespace).</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_InvalidName_FailsValidation(string? name)
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var request = new CommonEntityUpdateRequest
        {
            Name = name,
            DetailSections = []
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }

    /// <summary>
    /// Tests that the validator correctly validates a request with valid Name and valid DetailSections.
    /// </summary>
    [Fact]
    public void Constructor_ValidNameAndValidDetailSections_PassesValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var validDetailSection = new DetailSection
        {
            Id = "section-id",
            Title = "Section Title",
            Attributes = []
        };
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = [validDetailSection]
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that the validator fails validation when a DetailSection has an empty Id.
    /// </summary>
    [Fact]
    public void Constructor_DetailSectionWithEmptyId_FailsValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var invalidDetailSection = new DetailSection
        {
            Id = "",
            Title = "Section Title",
            Attributes = []
        };
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = [invalidDetailSection]
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("DetailSections[0].Id");
    }

    /// <summary>
    /// Tests that the validator fails validation when a DetailSection has a null Id.
    /// </summary>
    [Fact]
    public void Constructor_DetailSectionWithNullId_FailsValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var invalidDetailSection = new DetailSection
        {
            Id = null,
            Title = "Section Title",
            Attributes = []
        };
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = [invalidDetailSection]
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("DetailSections[0].Id");
    }

    /// <summary>
    /// Tests that the validator fails validation when a DetailSection has an empty Title.
    /// </summary>
    [Fact]
    public void Constructor_DetailSectionWithEmptyTitle_FailsValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var invalidDetailSection = new DetailSection
        {
            Id = "section-id",
            Title = "",
            Attributes = []
        };
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = [invalidDetailSection]
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("DetailSections[0].Title");
    }

    /// <summary>
    /// Tests that the validator fails validation when a DetailSection has a null Title.
    /// </summary>
    [Fact]
    public void Constructor_DetailSectionWithNullTitle_FailsValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var invalidDetailSection = new DetailSection
        {
            Id = "section-id",
            Title = null,
            Attributes = []
        };
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = [invalidDetailSection]
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("DetailSections[0].Title");
    }

    /// <summary>
    /// Tests that the validator correctly validates multiple DetailSections with mixed valid and invalid items.
    /// </summary>
    [Fact]
    public void Constructor_MultipleDetailSectionsWithInvalidItem_FailsValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var validDetailSection = new DetailSection
        {
            Id = "section-id-1",
            Title = "Section Title 1",
            Attributes = []
        };
        var invalidDetailSection = new DetailSection
        {
            Id = "",
            Title = "Section Title 2",
            Attributes = []
        };
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = [validDetailSection, invalidDetailSection]
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("DetailSections[1].Id");
        result.ShouldNotHaveValidationErrorFor("DetailSections[0].Id");
    }

    /// <summary>
    /// Tests that the validator correctly validates multiple valid DetailSections.
    /// </summary>
    [Fact]
    public void Constructor_MultipleValidDetailSections_PassesValidation()
    {
        // Arrange
        var validator = new CommonEntityUpdateRequestValidator();
        var detailSection1 = new DetailSection
        {
            Id = "section-id-1",
            Title = "Section Title 1",
            Attributes = []
        };
        var detailSection2 = new DetailSection
        {
            Id = "section-id-2",
            Title = "Section Title 2",
            Attributes = []
        };
        var request = new CommonEntityUpdateRequest
        {
            Name = "Valid Name",
            DetailSections = [detailSection1, detailSection2]
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}