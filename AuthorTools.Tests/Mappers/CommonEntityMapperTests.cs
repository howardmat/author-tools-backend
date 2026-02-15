using System;
using System.Collections.Generic;
using System.Linq;

using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Data.Models;
using FluentAssertions;
using Xunit;

namespace AuthorTools.Api.Mappers.UnitTests;


public class CommonEntityMapperTests
{
    /// <summary>
    /// Tests that ToEntity correctly handles nullable properties set to null.
    /// Input: Request with null Name, ImageFileId, WorkspaceId, and Order.
    /// Expected: Returns entity with nullable properties set to null.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithNullProperties_MapsNullPropertiesCorrectly()
    {
        // Arrange
        var request = new CommonEntityUpdateRequest
        {
            Name = null,
            ImageFileId = null,
            WorkspaceId = null,
            Order = null,
            DetailSections = []
        };
        var id = "entity123";
        var owner = new User("user1", "test@example.com", null, null);

        // Act
        var result = request.ToEntity<TestCommonEntity>(id, owner);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().BeNull();
        result.ImageFileId.Should().BeNull();
        result.WorkspaceId.Should().BeNull();
        result.Order.Should().BeNull();
        result.DetailSections.Should().NotBeNull().And.BeEmpty();
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles empty DetailSections collection.
    /// Input: Request with empty DetailSections collection.
    /// Expected: Returns entity with empty DetailSections collection.
    /// </summary>
    [Fact]
    public void ToEntity_EmptyDetailSections_MapsEmptyCollection()
    {
        // Arrange
        var request = new CommonEntityUpdateRequest
        {
            Name = "Test",
            DetailSections = []
        };
        var id = "entity123";
        var owner = new User("user1", "test@example.com", "Jane", "Smith");

        // Act
        var result = request.ToEntity<TestCommonEntity>(id, owner);

        // Assert
        result.DetailSections.Should().NotBeNull().And.BeEmpty();
    }

    /// <summary>
    /// Tests that ToEntity correctly handles various id string edge cases.
    /// Input: Different id values including null, empty, whitespace, and special characters.
    /// Expected: Returns entity with id property set to the provided value exactly.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("normal-id-123")]
    [InlineData("!@#$%^&*()")]
    [InlineData("very-long-id-" + "abcdefghijklmnopqrstuvwxyz0123456789")]
    public void ToEntity_VariousIdValues_AssignsIdCorrectly(string? id)
    {
        // Arrange
        var request = new CommonEntityUpdateRequest { Name = "Test" };
        var owner = new User("user1", "test@example.com", "Test", "User");

        // Act
        var result = request.ToEntity<TestCommonEntity>(id!, owner);

        // Assert
        result.Id.Should().Be(id);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles int? Order property with extreme values.
    /// Input: Order values including null, int.MinValue, int.MaxValue, 0, negative and positive values.
    /// Expected: Returns entity with Order property correctly mapped.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(100)]
    public void ToEntity_VariousOrderValues_MapsOrderCorrectly(int? order)
    {
        // Arrange
        var request = new CommonEntityUpdateRequest
        {
            Name = "Test",
            Order = order
        };
        var id = "entity123";
        var owner = new User("user1", "test@example.com", "Test", "User");

        // Act
        var result = request.ToEntity<TestCommonEntity>(id, owner);

        // Assert
        result.Order.Should().Be(order);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles various string property edge cases.
    /// Input: Different Name values including null, empty, whitespace, special characters, and long strings.
    /// Expected: Returns entity with Name property set to the exact value from request.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Normal Name")]
    [InlineData("Name with special chars: !@#$%^&*()")]
    [InlineData("Very long name that exceeds typical length expectations and contains many characters to test boundary conditions")]
    public void ToEntity_VariousNameValues_MapsNameCorrectly(string? name)
    {
        // Arrange
        var request = new CommonEntityUpdateRequest { Name = name };
        var id = "entity123";
        var owner = new User("user1", "test@example.com", "Test", "User");

        // Act
        var result = request.ToEntity<TestCommonEntity>(id, owner);

        // Assert
        result.Name.Should().Be(name);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles various ImageFileId edge cases.
    /// Input: Different ImageFileId values including null, empty, and valid identifiers.
    /// Expected: Returns entity with ImageFileId property correctly mapped.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("imageFile123")]
    public void ToEntity_VariousImageFileIdValues_MapsImageFileIdCorrectly(string? imageFileId)
    {
        // Arrange
        var request = new CommonEntityUpdateRequest { ImageFileId = imageFileId };
        var id = "entity123";
        var owner = new User("user1", "test@example.com", "Test", "User");

        // Act
        var result = request.ToEntity<TestCommonEntity>(id, owner);

        // Assert
        result.ImageFileId.Should().Be(imageFileId);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles various WorkspaceId edge cases.
    /// Input: Different WorkspaceId values including null, empty, and valid identifiers.
    /// Expected: Returns entity with WorkspaceId property correctly mapped.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("workspace123")]
    public void ToEntity_VariousWorkspaceIdValues_MapsWorkspaceIdCorrectly(string? workspaceId)
    {
        // Arrange
        var request = new CommonEntityUpdateRequest { WorkspaceId = workspaceId };
        var id = "entity123";
        var owner = new User("user1", "test@example.com", "Test", "User");

        // Act
        var result = request.ToEntity<TestCommonEntity>(id, owner);

        // Assert
        result.WorkspaceId.Should().Be(workspaceId);
    }

    /// <summary>
    /// Helper class for testing CommonEntity mapper.
    /// Provides a concrete implementation of the abstract CommonEntity class.
    /// </summary>
    private class TestCommonEntity : CommonEntity
    {
    }

    /// <summary>
    /// Verifies that ToResponse correctly handles an entity with null Id property.
    /// The method uses null-forgiving operator on Id, so this tests the actual behavior.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithNullId_AssignsNullToResponseId()
    {
        // Arrange
        var entity = new TestCommonEntity
        {
            Id = null,
            Name = "Test"
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Id.Should().BeNull();
    }

    /// <summary>
    /// Verifies that ToResponse correctly handles nullable string properties being null.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithNullablePropertiesNull_MapsNullValuesCorrectly()
    {
        // Arrange
        var entity = new TestCommonEntity
        {
            Id = "507f1f77bcf86cd799439011",
            Name = null,
            ImageFileId = null,
            WorkspaceId = null,
            Order = null,
            DetailSections = new List<DetailSection>()
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Id.Should().Be("507f1f77bcf86cd799439011");
        result.Name.Should().BeNull();
        result.ImageFileId.Should().BeNull();
        result.WorkspaceId.Should().BeNull();
        result.Order.Should().BeNull();
        result.DetailSections.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that ToResponse handles empty DetailSections collection correctly.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithEmptyDetailSections_MapsEmptyCollection()
    {
        // Arrange
        var entity = new TestCommonEntity
        {
            Id = "507f1f77bcf86cd799439011",
            DetailSections = new List<DetailSection>()
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.DetailSections.Should().NotBeNull();
        result.DetailSections.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that ToResponse correctly maps various edge case values for Order property.
    /// Tests boundary conditions including int.MinValue, int.MaxValue, zero, and negative values.
    /// </summary>
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1)]
    public void ToResponse_EntityWithEdgeCaseOrderValues_MapsCorrectly(int orderValue)
    {
        // Arrange
        var entity = new TestCommonEntity
        {
            Id = "507f1f77bcf86cd799439011",
            Order = orderValue
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Order.Should().Be(orderValue);
    }

    /// <summary>
    /// Verifies that ToResponse correctly handles empty strings for string properties.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithEmptyStrings_MapsEmptyStringsCorrectly()
    {
        // Arrange
        var entity = new TestCommonEntity
        {
            Id = string.Empty,
            Name = string.Empty,
            ImageFileId = string.Empty,
            WorkspaceId = string.Empty
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Id.Should().Be(string.Empty);
        result.Name.Should().Be(string.Empty);
        result.ImageFileId.Should().Be(string.Empty);
        result.WorkspaceId.Should().Be(string.Empty);
    }

    /// <summary>
    /// Verifies that ToResponse correctly handles whitespace-only strings for string properties.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithWhitespaceStrings_MapsWhitespaceStringsCorrectly()
    {
        // Arrange
        var entity = new TestCommonEntity
        {
            Id = "   ",
            Name = "   ",
            ImageFileId = "\t",
            WorkspaceId = "\n"
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Id.Should().Be("   ");
        result.Name.Should().Be("   ");
        result.ImageFileId.Should().Be("\t");
        result.WorkspaceId.Should().Be("\n");
    }

    /// <summary>
    /// Verifies that ToResponse correctly handles very long strings.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithVeryLongStrings_MapsLongStringsCorrectly()
    {
        // Arrange
        var longString = new string('a', 10000);
        var entity = new TestCommonEntity
        {
            Id = longString,
            Name = longString,
            ImageFileId = longString,
            WorkspaceId = longString
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Id.Should().Be(longString);
        result.Name.Should().Be(longString);
        result.ImageFileId.Should().Be(longString);
        result.WorkspaceId.Should().Be(longString);
    }

    /// <summary>
    /// Verifies that ToResponse correctly handles strings with special characters.
    /// </summary>
    [Fact]
    public void ToResponse_EntityWithSpecialCharactersInStrings_MapsSpecialCharactersCorrectly()
    {
        // Arrange
        var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?`~";
        var entity = new TestCommonEntity
        {
            Id = specialChars,
            Name = specialChars,
            ImageFileId = specialChars,
            WorkspaceId = specialChars
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Id.Should().Be(specialChars);
        result.Name.Should().Be(specialChars);
        result.ImageFileId.Should().Be(specialChars);
        result.WorkspaceId.Should().Be(specialChars);
    }

    /// <summary>
    /// Tests that ToEntity creates a new entity with all properties correctly mapped from the request.
    /// Input: Valid request with all properties populated and valid owner.
    /// Expected: Returns entity with Name, ImageFileId, WorkspaceId, and Owner correctly set.
    /// </summary>
    [Fact]
    public void ToEntity_ValidRequestAndOwner_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "Test Entity",
            WorkspaceId = "workspace123",
            ImageFileId = "image456"
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Entity");
        result.WorkspaceId.Should().Be("workspace123");
        result.ImageFileId.Should().Be("image456");
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles null Name property in the request.
    /// Input: Request with null Name.
    /// Expected: Returns entity with Name set to null.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithNullName_SetsNameToNull()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = null,
            WorkspaceId = "workspace123",
            ImageFileId = "image456"
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().BeNull();
        result.WorkspaceId.Should().Be("workspace123");
        result.ImageFileId.Should().Be("image456");
    }

    /// <summary>
    /// Tests that ToEntity correctly handles null ImageFileId property in the request.
    /// Input: Request with null ImageFileId.
    /// Expected: Returns entity with ImageFileId set to null.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithNullImageFileId_SetsImageFileIdToNull()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "Test Entity",
            WorkspaceId = "workspace123",
            ImageFileId = null
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().Be("Test Entity");
        result.ImageFileId.Should().BeNull();
        result.WorkspaceId.Should().Be("workspace123");
    }

    /// <summary>
    /// Tests that ToEntity correctly handles null WorkspaceId property in the request.
    /// Input: Request with null WorkspaceId.
    /// Expected: Returns entity with WorkspaceId set to null.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithNullWorkspaceId_SetsWorkspaceIdToNull()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "Test Entity",
            WorkspaceId = null,
            ImageFileId = "image456"
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().Be("Test Entity");
        result.WorkspaceId.Should().BeNull();
        result.ImageFileId.Should().Be("image456");
    }

    /// <summary>
    /// Tests that ToEntity correctly handles request with all nullable properties set to null.
    /// Input: Request with all nullable properties (Name, WorkspaceId, ImageFileId) set to null.
    /// Expected: Returns entity with all nullable properties set to null, and Owner set correctly.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithAllNullablePropertiesNull_SetsAllPropertiesToNull()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = null,
            WorkspaceId = null,
            ImageFileId = null
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().BeNull();
        result.WorkspaceId.Should().BeNull();
        result.ImageFileId.Should().BeNull();
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles empty string values in request properties.
    /// Input: Request with empty strings for Name, WorkspaceId, and ImageFileId.
    /// Expected: Returns entity with empty strings correctly mapped.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithEmptyStrings_MapsEmptyStringsCorrectly()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "",
            WorkspaceId = "",
            ImageFileId = ""
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().Be("");
        result.WorkspaceId.Should().Be("");
        result.ImageFileId.Should().Be("");
    }

    /// <summary>
    /// Tests that ToEntity correctly handles whitespace-only string values in request properties.
    /// Input: Request with whitespace-only strings for Name, WorkspaceId, and ImageFileId.
    /// Expected: Returns entity with whitespace strings correctly mapped without trimming.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithWhitespaceStrings_MapsWhitespaceStringsCorrectly()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "   ",
            WorkspaceId = "\t\n",
            ImageFileId = "  \r\n  "
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().Be("   ");
        result.WorkspaceId.Should().Be("\t\n");
        result.ImageFileId.Should().Be("  \r\n  ");
    }

    /// <summary>
    /// Tests that ToEntity correctly handles very long string values in request properties.
    /// Input: Request with very long strings.
    /// Expected: Returns entity with long strings correctly mapped without truncation.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithVeryLongStrings_MapsLongStringsCorrectly()
    {
        // Arrange
        var longString = new string('a', 10000);
        var request = new CommonEntityCreateRequest
        {
            Name = longString,
            WorkspaceId = longString,
            ImageFileId = longString
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().Be(longString);
        result.WorkspaceId.Should().Be(longString);
        result.ImageFileId.Should().Be(longString);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles special characters in request properties.
    /// Input: Request with special characters in string properties.
    /// Expected: Returns entity with special characters correctly mapped.
    /// </summary>
    [Fact]
    public void ToEntity_RequestWithSpecialCharacters_MapsSpecialCharactersCorrectly()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "Test<>&\"'`Entity",
            WorkspaceId = "workspace!@#$%^&*()",
            ImageFileId = "image\u0000\u0001\u001F"
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Name.Should().Be("Test<>&\"'`Entity");
        result.WorkspaceId.Should().Be("workspace!@#$%^&*()");
        result.ImageFileId.Should().Be("image\u0000\u0001\u001F");
    }

    /// <summary>
    /// Tests that ToEntity creates a new instance of the entity for each call.
    /// Input: Same request and owner used multiple times.
    /// Expected: Returns different entity instances for each call.
    /// </summary>
    [Fact]
    public void ToEntity_CalledMultipleTimes_CreatesNewInstanceEachTime()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "Test Entity",
            WorkspaceId = "workspace123",
            ImageFileId = "image456"
        };
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        var result1 = request.ToEntity<TestCommonEntity>(owner);
        var result2 = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result1.Should().NotBeSameAs(result2);
        result1.Name.Should().Be(result2.Name);
    }

    /// <summary>
    /// Tests that ToEntity throws NullReferenceException when request is null.
    /// Input: Null request parameter.
    /// Expected: Throws NullReferenceException when attempting to access request properties.
    /// </summary>
    [Fact]
    public void ToEntity_NullRequest_ThrowsNullReferenceException()
    {
        // Arrange
        CommonEntityCreateRequest request = null!;
        var owner = new User("user1", "test@example.com", "John", "Doe");

        // Act
        Action act = () => request.ToEntity<TestCommonEntity>(owner);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    /// <summary>
    /// Tests that ToEntity correctly handles owner with null optional properties.
    /// Input: Owner with null Firstname and Lastname.
    /// Expected: Returns entity with Owner correctly set.
    /// </summary>
    [Fact]
    public void ToEntity_OwnerWithNullOptionalProperties_SetsOwnerCorrectly()
    {
        // Arrange
        var request = new CommonEntityCreateRequest
        {
            Name = "Test Entity",
            WorkspaceId = "workspace123",
            ImageFileId = "image456"
        };
        var owner = new User("user1", "test@example.com", null, null);

        // Act
        var result = request.ToEntity<TestCommonEntity>(owner);

        // Assert
        result.Owner.Should().BeSameAs(owner);
        result.Owner.Firstname.Should().BeNull();
        result.Owner.Lastname.Should().BeNull();
    }

}