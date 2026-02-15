using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Data.Models;
using FluentAssertions;
using Xunit;

namespace AuthorTools.Api.Mappers.UnitTests;

public class WorkspaceMapperTests
{
    /// <summary>
    /// Verifies that ToEntity correctly maps all properties from a WorkspaceUpdateRequest
    /// to a Workspace entity when provided with valid inputs.
    /// </summary>
    /// <param name="name">The workspace name.</param>
    /// <param name="description">The workspace description.</param>
    /// <param name="icon">The workspace icon.</param>
    /// <param name="isDefault">Whether the workspace is default.</param>
    [Theory]
    [InlineData("Test Workspace", "Test Description", "test-icon.png", true)]
    [InlineData("Another Workspace", null, null, false)]
    [InlineData("Workspace", "", "", true)]
    [InlineData("Special!@#$%", "Description with special chars !@#$%^&*()", "icon-123", false)]
    [InlineData("   Whitespace   ", "   ", "   ", true)]
    [InlineData("VeryLongWorkspaceNameThatExceedsNormalLengthToTestBoundaryConditionsAndEnsureProperHandling", "VeryLongDescriptionThatExceedsNormalLengthToTestBoundaryConditionsAndEnsureProperHandling", "VeryLongIconPathThatExceedsNormalLength", false)]
    public void ToEntity_ValidInputs_MapsAllPropertiesCorrectly(
        string? name,
        string? description,
        string? icon,
        bool isDefault)
    {
        // Arrange
        var request = new WorkspaceUpdateRequest
        {
            Name = name,
            Description = description,
            Icon = icon,
            IsDefault = isDefault
        };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be(name);
        result.Description.Should().Be(description);
        result.Icon.Should().Be(icon);
        result.IsDefault.Should().Be(isDefault);
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Verifies that ToEntity throws a NullReferenceException when the request parameter is null.
    /// </summary>
    [Fact]
    public void ToEntity_NullRequest_ThrowsNullReferenceException()
    {
        // Arrange
        WorkspaceUpdateRequest request = null!;
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var act = () => request.ToEntity(id, owner);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    /// <summary>
    /// Verifies that ToEntity correctly handles various edge case values for the id parameter.
    /// </summary>
    /// <param name="id">The workspace identifier to test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("507f1f77bcf86cd799439011")]
    [InlineData("!@#$%^&*()")]
    [InlineData("VeryLongIdStringThatExceedsNormalLengthToTestBoundaryConditionsAndEnsureProperHandlingOfExtremeCases")]
    public void ToEntity_VariousIdValues_MapsIdCorrectly(string? id)
    {
        // Arrange
        var request = new WorkspaceUpdateRequest
        {
            Name = "Test",
            Description = "Description",
            Icon = "icon.png",
            IsDefault = false
        };
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(id!, owner);

        // Assert
        result.Id.Should().Be(id);
    }

    /// <summary>
    /// Verifies that ToEntity correctly handles a null owner parameter by assigning it to the Owner property.
    /// </summary>
    [Fact]
    public void ToEntity_NullOwner_AssignsNullToOwnerProperty()
    {
        // Arrange
        var request = new WorkspaceUpdateRequest
        {
            Name = "Test",
            Description = "Description",
            Icon = "icon.png",
            IsDefault = true
        };
        var id = "507f1f77bcf86cd799439011";
        User owner = null!;

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Owner.Should().BeNull();
    }

    /// <summary>
    /// Verifies that ToEntity correctly handles when all optional string properties in the request are null.
    /// </summary>
    [Fact]
    public void ToEntity_AllNullablePropertiesNull_MapsCorrectly()
    {
        // Arrange
        var request = new WorkspaceUpdateRequest
        {
            Name = null,
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Id.Should().Be(id);
        result.Name.Should().BeNull();
        result.Description.Should().BeNull();
        result.Icon.Should().BeNull();
        result.IsDefault.Should().BeFalse();
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Verifies that ToEntity correctly maps the IsDefault property for both true and false values.
    /// </summary>
    /// <param name="isDefault">The IsDefault value to test.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToEntity_IsDefaultProperty_MapsCorrectly(bool isDefault)
    {
        // Arrange
        var request = new WorkspaceUpdateRequest
        {
            Name = "Test",
            Description = "Description",
            Icon = "icon.png",
            IsDefault = isDefault
        };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.IsDefault.Should().Be(isDefault);
    }

    /// <summary>
    /// Verifies that ToEntity preserves the owner reference correctly.
    /// </summary>
    [Fact]
    public void ToEntity_OwnerParameter_PreservesReference()
    {
        // Arrange
        var request = new WorkspaceUpdateRequest
        {
            Name = "Test",
            Description = "Description",
            Icon = "icon.png",
            IsDefault = true
        };
        var id = "507f1f77bcf86cd799439011";
        var owner = new User("user123", "test@example.com", "Jane", "Smith");

        // Act
        var result = request.ToEntity(id, owner);

        // Assert
        result.Owner.Should().BeSameAs(owner);
        result.Owner!.Id.Should().Be("user123");
        result.Owner.Email.Should().Be("test@example.com");
        result.Owner.Firstname.Should().Be("Jane");
        result.Owner.Lastname.Should().Be("Smith");
    }

    /// <summary>
    /// Tests that ToEntity successfully maps all properties from WorkspaceCreateRequest to Workspace entity
    /// when all properties are provided with valid values.
    /// </summary>
    [Fact]
    public void ToEntity_WithAllPropertiesSet_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Test Workspace",
            Description = "Test Description",
            Icon = "test-icon",
            IsDefault = true
        };
        var owner = new User("user-id", "test@example.com", "John", "Doe");

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Workspace");
        result.Description.Should().Be("Test Description");
        result.Icon.Should().Be("test-icon");
        result.IsDefault.Should().BeTrue();
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Tests that ToEntity correctly maps Name property with various valid string values.
    /// </summary>
    /// <param name="name">The workspace name to test.</param>
    [Theory]
    [InlineData("Simple Name")]
    [InlineData("Name with Special Characters !@#$%")]
    [InlineData("Name with Unicode 日本語")]
    [InlineData("A")]
    [InlineData("   Name with leading and trailing spaces   ")]
    public void ToEntity_WithVariousValidNames_MapsNameCorrectly(string name)
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = name,
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("user-id", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Name.Should().Be(name);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles very long Name values.
    /// </summary>
    [Fact]
    public void ToEntity_WithVeryLongName_MapsNameCorrectly()
    {
        // Arrange
        var longName = new string('A', 10000);
        var request = new WorkspaceCreateRequest
        {
            Name = longName,
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("user-id", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Name.Should().Be(longName);
        result.Name.Length.Should().Be(10000);
    }

    /// <summary>
    /// Tests that ToEntity correctly maps Description property when it is null.
    /// </summary>
    [Fact]
    public void ToEntity_WithNullDescription_MapsDescriptionAsNull()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Test Workspace",
            Description = null,
            Icon = "icon",
            IsDefault = false
        };
        var owner = new User("user-id", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Description.Should().BeNull();
    }

    /// <summary>
    /// Tests that ToEntity correctly maps Description property with various string values.
    /// </summary>
    /// <param name="description">The description to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Valid Description")]
    [InlineData("Description with special chars: !@#$%^&*()")]
    [InlineData("Multi\nLine\nDescription")]
    public void ToEntity_WithVariousDescriptions_MapsDescriptionCorrectly(string description)
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Test",
            Description = description,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("user-id", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Description.Should().Be(description);
    }

    /// <summary>
    /// Tests that ToEntity correctly maps Icon property when it is null.
    /// </summary>
    [Fact]
    public void ToEntity_WithNullIcon_MapsIconAsNull()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Test Workspace",
            Description = "description",
            Icon = null,
            IsDefault = false
        };
        var owner = new User("user-id", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Icon.Should().BeNull();
    }

    /// <summary>
    /// Tests that ToEntity correctly maps Icon property with various string values.
    /// </summary>
    /// <param name="icon">The icon value to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("icon-name")]
    [InlineData("path/to/icon.png")]
    [InlineData("https://example.com/icon.svg")]
    public void ToEntity_WithVariousIcons_MapsIconCorrectly(string icon)
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Test",
            Description = null,
            Icon = icon,
            IsDefault = false
        };
        var owner = new User("user-id", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Icon.Should().Be(icon);
    }

    /// <summary>
    /// Tests that ToEntity correctly maps IsDefault property for both true and false values.
    /// </summary>
    /// <param name="isDefault">The IsDefault value to test.</param>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToEntity_WithIsDefaultValues_MapsIsDefaultCorrectly(bool isDefault)
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Test",
            Description = null,
            Icon = null,
            IsDefault = isDefault
        };
        var owner = new User("user-id", "test@example.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.IsDefault.Should().Be(isDefault);
    }

    /// <summary>
    /// Tests that ToEntity correctly maps Owner property and maintains the same reference.
    /// </summary>
    [Fact]
    public void ToEntity_WithOwner_MapsOwnerCorrectly()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Test",
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("owner-id-123", "owner@example.com", "Jane", "Smith");

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Owner.Should().BeSameAs(owner);
        result.Owner.Id.Should().Be("owner-id-123");
        result.Owner.Email.Should().Be("owner@example.com");
    }

    /// <summary>
    /// Tests that ToEntity correctly handles the minimum valid scenario with only required properties set.
    /// </summary>
    [Fact]
    public void ToEntity_WithMinimalProperties_MapsCorrectly()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "MinimalWorkspace",
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("id", "email@test.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Name.Should().Be("MinimalWorkspace");
        result.Description.Should().BeNull();
        result.Icon.Should().BeNull();
        result.IsDefault.Should().BeFalse();
        result.Owner.Should().BeSameAs(owner);
    }

    /// <summary>
    /// Tests that ToEntity correctly handles empty strings for Name property.
    /// </summary>
    [Fact]
    public void ToEntity_WithEmptyName_MapsEmptyName()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "",
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("id", "email@test.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Name.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that ToEntity correctly handles whitespace-only strings for Name property.
    /// </summary>
    [Fact]
    public void ToEntity_WithWhitespaceName_MapsWhitespaceName()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "   ",
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("id", "email@test.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Name.Should().Be("   ");
    }

    /// <summary>
    /// Tests that ToEntity correctly handles all nullable properties set to null simultaneously.
    /// </summary>
    [Fact]
    public void ToEntity_WithAllNullablePropertiesNull_MapsCorrectly()
    {
        // Arrange
        var request = new WorkspaceCreateRequest
        {
            Name = "Workspace",
            Description = null,
            Icon = null,
            IsDefault = false
        };
        var owner = new User("id", "email@test.com", null, null);

        // Act
        var result = request.ToEntity(owner);

        // Assert
        result.Name.Should().Be("Workspace");
        result.Description.Should().BeNull();
        result.Icon.Should().BeNull();
        result.IsDefault.Should().BeFalse();
        result.Owner.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that ToResponse correctly maps all properties from a Workspace entity
    /// to a WorkspaceResponse when all properties are populated.
    /// </summary>
    [Fact]
    public void ToResponse_AllPropertiesPopulated_MapsCorrectly()
    {
        // Arrange
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "Test Workspace",
            Description = "Test Description",
            Icon = "test-icon",
            IsDefault = true
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("507f1f77bcf86cd799439011");
        result.Name.Should().Be("Test Workspace");
        result.Description.Should().Be("Test Description");
        result.Icon.Should().Be("test-icon");
        result.IsDefault.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ToResponse correctly maps properties when nullable Description
    /// and Icon properties are null.
    /// </summary>
    [Fact]
    public void ToResponse_NullablePropertiesNull_MapsCorrectly()
    {
        // Arrange
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "Workspace Without Description",
            Description = null,
            Icon = null,
            IsDefault = false
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("507f1f77bcf86cd799439011");
        result.Name.Should().Be("Workspace Without Description");
        result.Description.Should().BeNull();
        result.Icon.Should().BeNull();
        result.IsDefault.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ToResponse correctly maps the IsDefault property when set to true.
    /// </summary>
    [Fact]
    public void ToResponse_IsDefaultTrue_MapsCorrectly()
    {
        // Arrange
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "Default Workspace",
            IsDefault = true
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.IsDefault.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ToResponse correctly maps the IsDefault property when set to false.
    /// </summary>
    [Fact]
    public void ToResponse_IsDefaultFalse_MapsCorrectly()
    {
        // Arrange
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "Non-Default Workspace",
            IsDefault = false
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.IsDefault.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ToResponse correctly handles empty strings for nullable properties
    /// Description and Icon.
    /// </summary>
    [Fact]
    public void ToResponse_EmptyStringsForNullableProperties_MapsCorrectly()
    {
        // Arrange
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "Workspace",
            Description = "",
            Icon = "",
            IsDefault = false
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Description.Should().Be("");
        result.Icon.Should().Be("");
    }

    /// <summary>
    /// Tests that ToResponse correctly handles special characters in string properties.
    /// </summary>
    [Fact]
    public void ToResponse_SpecialCharactersInStrings_MapsCorrectly()
    {
        // Arrange
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "Test <Workspace> & \"Special\" 'Characters'",
            Description = "Description with\nnewlines\tand\ttabs",
            Icon = "icon-with-unicode-⭐",
            IsDefault = false
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Name.Should().Be("Test <Workspace> & \"Special\" 'Characters'");
        result.Description.Should().Be("Description with\nnewlines\tand\ttabs");
        result.Icon.Should().Be("icon-with-unicode-⭐");
    }

    /// <summary>
    /// Tests that ToResponse correctly handles very long strings in properties.
    /// </summary>
    [Fact]
    public void ToResponse_VeryLongStrings_MapsCorrectly()
    {
        // Arrange
        var longString = new string('a', 10000);
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = longString,
            Description = longString,
            Icon = longString,
            IsDefault = false
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Name.Should().Be(longString);
        result.Description.Should().Be(longString);
        result.Icon.Should().Be(longString);
    }

    /// <summary>
    /// Tests that ToResponse correctly handles whitespace-only strings in nullable properties.
    /// </summary>
    [Fact]
    public void ToResponse_WhitespaceOnlyStrings_MapsCorrectly()
    {
        // Arrange
        var entity = new Workspace
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "   ",
            Description = "   ",
            Icon = "   ",
            IsDefault = false
        };

        // Act
        var result = entity.ToResponse();

        // Assert
        result.Name.Should().Be("   ");
        result.Description.Should().Be("   ");
        result.Icon.Should().Be("   ");
    }
}