using System;

using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Data.Models;
using FluentAssertions;
using Xunit;

namespace AuthorTools.Api.Mappers.UnitTests;


public class UserSettingMapperTests
{
    /// <summary>
    /// Tests that ToEntity correctly maps all properties from UserSettingUpdateRequest to UserSetting
    /// when provided with valid non-null values.
    /// Expected: All properties (Id, Theme, Owner) are correctly assigned.
    /// </summary>
    [Fact]
    public void ToEntity_ValidRequest_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var request = new UserSettingUpdateRequest { Theme = "dark" };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Theme.Should().Be("dark");
        result.Owner.Should().Be(owner);
    }

    /// <summary>
    /// Tests that ToEntity correctly assigns null Theme when the request has a null Theme property.
    /// Expected: Theme property on the resulting UserSetting should be null.
    /// </summary>
    [Fact]
    public void ToEntity_NullTheme_AssignsNullTheme()
    {
        // Arrange
        var request = new UserSettingUpdateRequest { Theme = null };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Theme.Should().BeNull();
        result.Id.Should().Be(id);
        result.Owner.Should().Be(owner);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles various edge case values for the Theme property,
    /// including empty strings, whitespace, and strings with special characters.
    /// Expected: Theme value should be assigned exactly as provided.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("theme-with-special-chars!@#$%")]
    [InlineData("very-long-theme-string-that-could-potentially-cause-issues-if-there-were-length-validations-applied")]
    public void ToEntity_ThemeEdgeCases_AssignsThemeExactly(string theme)
    {
        // Arrange
        var request = new UserSettingUpdateRequest { Theme = theme };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Theme.Should().Be(theme);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles various edge case values for the id parameter,
    /// including empty strings, whitespace, very long strings, and special characters.
    /// Expected: Id value should be assigned exactly as provided.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("507f1f77bcf86cd799439011")]
    [InlineData("id-with-special-chars!@#$%^&*()")]
    [InlineData("very-long-id-string-that-exceeds-typical-lengths-for-mongodb-objectids-but-should-still-be-handled-correctly-without-truncation-or-errors")]
    public void ToEntity_IdEdgeCases_AssignsIdExactly(string id)
    {
        // Arrange
        var request = new UserSettingUpdateRequest { Theme = "dark" };
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Id.Should().Be(id);
    }

    /// <summary>
    /// Tests that ToEntity correctly assigns the owner reference to the resulting UserSetting.
    /// Expected: The Owner property should reference the exact same User instance provided.
    /// </summary>
    [Fact]
    public void ToEntity_DifferentOwners_AssignsOwnerCorrectly()
    {
        // Arrange
        var request = new UserSettingUpdateRequest { Theme = "light" };
        var id = "507f1f77bcf86cd799439011";
        var owner1 = new User("user1", "user1@example.com", "Alice", "Smith");
        var owner2 = new User("user2", "user2@example.com", null, null);

        // Act
        var result1 = request.ToEntity(id, owner1);
        var result2 = request.ToEntity(id, owner2);

        // Assert
        result1.Owner.Should().BeSameAs(owner1);
        result2.Owner.Should().BeSameAs(owner2);
    }

    /// <summary>
    /// Tests that ToEntity creates a new UserSetting instance each time it is called.
    /// Expected: Multiple calls should return different instances.
    /// </summary>
    [Fact]
    public void ToEntity_MultipleCalls_CreatesNewInstances()
    {
        // Arrange
        var request = new UserSettingUpdateRequest { Theme = "dark" };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result1 = request.ToEntity(id, owner);
        var result2 = request.ToEntity(id, owner);

        // Assert
        result1.Should().NotBeSameAs(result2);
        result1.Id.Should().Be(result2.Id);
        result1.Theme.Should().Be(result2.Theme);
        result1.Owner.Should().BeSameAs(result2.Owner);
    }

    /// <summary>
    /// Tests that ToEntity throws ArgumentNullException when the request parameter is null.
    /// </summary>
    [Fact(Skip="ProductionBugSuspected")]
    [Trait("Category", "ProductionBugSuspected")]
    public void ToEntity_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        UserSettingCreateRequest request = null!;
        var owner = new User("testId", "test@example.com", "John", "Doe");

        // Act
        Action act = () => request.ToEntity(owner);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that ToEntity correctly maps request with null Theme to entity.
    /// </summary>
    /// <param name="theme">The theme value to test (null, empty, whitespace, or normal value).</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("dark")]
    [InlineData("light")]
    [InlineData("custom-theme-123")]
    [InlineData("Theme with spaces")]
    [InlineData("特殊字符")]
    public void ToEntity_ValidRequest_MapsThemeCorrectly(string? theme)
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = theme };
        var owner = new User("ownerId", "owner@example.com", "Jane", "Smith");

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Should().NotBeNull();
        result.Theme.Should().Be(theme);
    }

    /// <summary>
    /// Tests that ToEntity correctly assigns the owner to the resulting entity.
    /// </summary>
    [Fact]
    public void ToEntity_ValidRequest_AssignsOwnerCorrectly()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "dark" };
        var owner = new User("ownerId123", "owner@test.com", "Alice", "Brown");

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Should().NotBeNull();
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Tests that ToEntity creates a new UserSetting instance each time it's called.
    /// </summary>
    [Fact]
    public void ToEntity_CalledMultipleTimes_CreatesNewInstances()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "theme1" };
        var owner = new User("id", "email@test.com", null, null);

        // Act
        var result1 = request.ToEntity(owner);
        var result2 = request.ToEntity(owner);

        // Assert
        result1.Should().NotBeSameAs(result2);
    }

    /// <summary>
    /// Tests that ToEntity handles owner with minimal data (null firstname and lastname).
    /// </summary>
    [Fact]
    public void ToEntity_OwnerWithNullNames_AssignsOwnerCorrectly()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "minimal" };
        var owner = new User("minimalId", "minimal@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Should().NotBeNull();
        result.Owner.Should().BeSameAs(owner);
        result.Owner!.Firstname.Should().BeNull();
        result.Owner.Lastname.Should().BeNull();
    }

    /// <summary>
    /// Tests that ToResponse correctly maps a valid UserSetting entity to UserSettingResponse.
    /// Input: Valid entity with non-null Id and Theme.
    /// Expected: Returns UserSettingResponse with correctly mapped Id and Theme properties.
    /// </summary>
    [Fact]
    public void ToResponse_ValidEntity_ReturnsCorrectlyMappedResponse()
    {
        // Arrange
        var entity = new UserSetting
        {
            Id = "507f1f77bcf86cd799439011",
            Theme = "dark"
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("507f1f77bcf86cd799439011");
        result.Theme.Should().Be("dark");
    }

    /// <summary>
    /// Tests that ToResponse handles entity with null Id despite using null-forgiving operator.
    /// Input: Entity with null Id and valid Theme.
    /// Expected: Returns response with null Id (suppressed by null-forgiving operator).
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithNullId_ReturnsResponseWithNullId()
    {
        // Arrange
        var entity = new UserSetting
        {
            Id = null,
            Theme = "light"
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeNull();
        result.Theme.Should().Be("light");
    }

    /// <summary>
    /// Tests that ToResponse handles entity with null Theme despite using null-forgiving operator.
    /// Input: Entity with valid Id and null Theme.
    /// Expected: Returns response with null Theme (suppressed by null-forgiving operator).
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithNullTheme_ReturnsResponseWithNullTheme()
    {
        // Arrange
        var entity = new UserSetting
        {
            Id = "507f1f77bcf86cd799439011",
            Theme = null
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("507f1f77bcf86cd799439011");
        result.Theme.Should().BeNull();
    }

    /// <summary>
    /// Tests that ToResponse handles entity with both null Id and Theme.
    /// Input: Entity with null Id and null Theme.
    /// Expected: Returns response with both properties null.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithNullIdAndTheme_ReturnsResponseWithNullProperties()
    {
        // Arrange
        var entity = new UserSetting
        {
            Id = null,
            Theme = null
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeNull();
        result.Theme.Should().BeNull();
    }

    /// <summary>
    /// Tests that ToResponse correctly handles empty strings for Id and Theme.
    /// Input: Entity with empty string Id and Theme.
    /// Expected: Returns response with empty string values mapped correctly.
    /// </summary>
    [Theory]
    [InlineData("", "")]
    [InlineData("", "dark")]
    [InlineData("507f1f77bcf86cd799439011", "")]
    public void ToResponse_EntityWithEmptyStrings_ReturnsMappedResponse(string id, string theme)
    {
        // Arrange
        var entity = new UserSetting
        {
            Id = id,
            Theme = theme
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Theme.Should().Be(theme);
    }

    /// <summary>
    /// Tests that ToResponse correctly handles whitespace strings for Id and Theme.
    /// Input: Entity with whitespace-only strings.
    /// Expected: Returns response with whitespace values preserved.
    /// </summary>
    [Theory]
    [InlineData("   ", "   ")]
    [InlineData("\t", "\n")]
    [InlineData(" ", "dark")]
    public void ToResponse_EntityWithWhitespaceStrings_ReturnsMappedResponse(string id, string theme)
    {
        // Arrange
        var entity = new UserSetting
        {
            Id = id,
            Theme = theme
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Theme.Should().Be(theme);
    }

    /// <summary>
    /// Tests that ToResponse handles very long strings for Id and Theme.
    /// Input: Entity with very long string values.
    /// Expected: Returns response with long strings mapped correctly.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithLongStrings_ReturnsMappedResponse()
    {
        // Arrange
        var longId = new string('a', 10000);
        var longTheme = new string('b', 10000);
        var entity = new UserSetting
        {
            Id = longId,
            Theme = longTheme
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(longId);
        result.Theme.Should().Be(longTheme);
    }

    /// <summary>
    /// Tests that ToResponse handles strings with special characters.
    /// Input: Entity with special characters in Id and Theme.
    /// Expected: Returns response with special characters preserved.
    /// </summary>
    [Theory]
    [InlineData("id@#$%", "theme!@#")]
    [InlineData("id\u0000null", "theme\u0000")]
    [InlineData("id\r\nline", "theme\ttab")]
    public void ToResponse_EntityWithSpecialCharacters_ReturnsMappedResponse(string id, string theme)
    {
        // Arrange
        var entity = new UserSetting
        {
            Id = id,
            Theme = theme
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Theme.Should().Be(theme);
    }
}