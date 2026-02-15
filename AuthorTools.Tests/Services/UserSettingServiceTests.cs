using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Api.Validators;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace AuthorTools.Api.Services.UnitTests;


/// <summary>
/// Unit tests for the <see cref="UserSettingService"/> class.
/// </summary>
public class UserSettingServiceTests
{
    /// <summary>
    /// Tests that CreateAsync returns BadRequest when validation fails with a single error.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ValidationFailsWithSingleError_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = null };
        var validationFailure = new ValidationFailure("Theme", "Theme is required");
        var validationResult = new ValidationResult(new[] { validationFailure });

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<ValidationProblemDetails>>();
        var badRequestResult = result.Result as Microsoft.AspNetCore.Http.HttpResults.BadRequest<ValidationProblemDetails>;
        badRequestResult!.Value.Should().NotBeNull();
        badRequestResult.Value!.Errors.Should().ContainKey("Theme");
        badRequestResult.Value.Errors["Theme"].Should().Contain("Theme is required");

        identityProvider.DidNotReceive().GetCurrentUser();
        await repository.DidNotReceive().CreateAsync(Arg.Any<UserSetting>(), Arg.Any<string>());
    }

    /// <summary>
    /// Tests that CreateAsync returns BadRequest when validation fails with multiple errors.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ValidationFailsWithMultipleErrors_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = null };
        var validationFailures = new[]
        {
            new ValidationFailure("Theme", "Theme is required"),
            new ValidationFailure("Theme", "Theme must be a valid value"),
            new ValidationFailure("AnotherProperty", "Another error")
        };
        var validationResult = new ValidationResult(validationFailures);

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<ValidationProblemDetails>>();
        var badRequestResult = result.Result as Microsoft.AspNetCore.Http.HttpResults.BadRequest<ValidationProblemDetails>;
        badRequestResult!.Value.Should().NotBeNull();
        badRequestResult.Value!.Errors.Should().ContainKey("Theme");
        badRequestResult.Value.Errors["Theme"].Should().HaveCount(2);
        badRequestResult.Value.Errors.Should().ContainKey("AnotherProperty");
        badRequestResult.Value.Errors["AnotherProperty"].Should().ContainSingle();

        identityProvider.DidNotReceive().GetCurrentUser();
        await repository.DidNotReceive().CreateAsync(Arg.Any<UserSetting>(), Arg.Any<string>());
    }

    /// <summary>
    /// Tests that CreateAsync successfully creates a user setting when validation passes.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ValidationSucceeds_CreatesAndReturnsOk()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "dark" };
        var validationResult = new ValidationResult();

        var user = new User("user123", "test@example.com", "John", "Doe");
        var createdEntity = new UserSetting
        {
            Id = "setting123",
            Theme = "dark",
            Owner = user
        };

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Any<UserSetting>(), user.Id).Returns(createdEntity);

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserSettingResponse>>();
        var okResult = result.Result as Microsoft.AspNetCore.Http.HttpResults.Ok<UserSettingResponse>;
        okResult!.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be("setting123");
        okResult.Value.Theme.Should().Be("dark");

        identityProvider.Received(1).GetCurrentUser();
        await repository.Received(1).CreateAsync(
            Arg.Is<UserSetting>(e => e.Theme == "dark" && e.Owner == user),
            user.Id);
    }

    /// <summary>
    /// Tests that CreateAsync sets the Owner property on the entity before creating it.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ValidationSucceeds_SetsOwnerOnEntity()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "light" };
        var validationResult = new ValidationResult();

        var user = new User("user456", "user@test.com", "Jane", "Smith");
        UserSetting? capturedEntity = null;

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Do<UserSetting>(e => capturedEntity = e), user.Id)
            .Returns(callInfo =>
            {
                var entity = callInfo.Arg<UserSetting>();
                entity.Id = "newId";
                return entity;
            });

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        await service.CreateAsync(request);

        // Assert
        capturedEntity.Should().NotBeNull();
        capturedEntity!.Owner.Should().Be(user);
        capturedEntity.Theme.Should().Be("light");
    }

    /// <summary>
    /// Tests that CreateAsync handles null theme value in the request.
    /// </summary>
    [Fact]
    public async Task CreateAsync_RequestWithNullTheme_CreatesEntityWithNullTheme()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = null };
        var validationResult = new ValidationResult();

        var user = new User("user789", "null@test.com", null, null);
        var createdEntity = new UserSetting
        {
            Id = "setting789",
            Theme = null,
            Owner = user
        };

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Any<UserSetting>(), user.Id).Returns(createdEntity);

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        await repository.Received(1).CreateAsync(
            Arg.Is<UserSetting>(e => e.Theme == null && e.Owner == user),
            user.Id);
    }

    /// <summary>
    /// Tests that CreateAsync correctly passes user ID to repository.
    /// </summary>
    [Theory]
    [InlineData("user1")]
    [InlineData("")]
    [InlineData("very-long-user-id-12345678901234567890")]
    public async Task CreateAsync_DifferentUserIds_PassesCorrectIdToRepository(string userId)
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "default" };
        var validationResult = new ValidationResult();

        var user = new User(userId, "test@example.com", "Test", "User");
        var createdEntity = new UserSetting
        {
            Id = "settingId",
            Theme = "default",
            Owner = user
        };

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Any<UserSetting>(), userId).Returns(createdEntity);

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        await service.CreateAsync(request);

        // Assert
        await repository.Received(1).CreateAsync(Arg.Any<UserSetting>(), userId);
    }

    /// <summary>
    /// Tests that CreateAsync handles empty string theme value.
    /// </summary>
    [Fact]
    public async Task CreateAsync_RequestWithEmptyTheme_CreatesEntityWithEmptyTheme()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "" };
        var validationResult = new ValidationResult();

        var user = new User("userEmpty", "empty@test.com", "Empty", "User");
        var createdEntity = new UserSetting
        {
            Id = "settingEmpty",
            Theme = "",
            Owner = user
        };

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Any<UserSetting>(), user.Id).Returns(createdEntity);

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        await repository.Received(1).CreateAsync(
            Arg.Is<UserSetting>(e => e.Theme == "" && e.Owner == user),
            user.Id);
    }

    /// <summary>
    /// Tests that CreateAsync handles whitespace-only theme value.
    /// </summary>
    [Fact]
    public async Task CreateAsync_RequestWithWhitespaceTheme_CreatesEntityWithWhitespaceTheme()
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = "   " };
        var validationResult = new ValidationResult();

        var user = new User("userSpace", "space@test.com", "Space", "User");
        var createdEntity = new UserSetting
        {
            Id = "settingSpace",
            Theme = "   ",
            Owner = user
        };

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Any<UserSetting>(), user.Id).Returns(createdEntity);

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        await repository.Received(1).CreateAsync(
            Arg.Is<UserSetting>(e => e.Theme == "   " && e.Owner == user),
            user.Id);
    }

    /// <summary>
    /// Tests that CreateAsync handles very long theme value.
    /// </summary>
    [Fact]
    public async Task CreateAsync_RequestWithVeryLongTheme_CreatesEntityWithLongTheme()
    {
        // Arrange
        var longTheme = new string('x', 10000);
        var request = new UserSettingCreateRequest { Theme = longTheme };
        var validationResult = new ValidationResult();

        var user = new User("userLong", "long@test.com", "Long", "User");
        var createdEntity = new UserSetting
        {
            Id = "settingLong",
            Theme = longTheme,
            Owner = user
        };

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Any<UserSetting>(), user.Id).Returns(createdEntity);

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        await repository.Received(1).CreateAsync(
            Arg.Is<UserSetting>(e => e.Theme == longTheme && e.Owner == user),
            user.Id);
    }

    /// <summary>
    /// Tests that CreateAsync handles special characters in theme value.
    /// </summary>
    [Theory]
    [InlineData("dark\nmode")]
    [InlineData("theme\twith\ttabs")]
    [InlineData("theme<>with\"special'chars")]
    [InlineData("emoji🎨theme")]
    public async Task CreateAsync_RequestWithSpecialCharactersInTheme_CreatesEntity(string theme)
    {
        // Arrange
        var request = new UserSettingCreateRequest { Theme = theme };
        var validationResult = new ValidationResult();

        var user = new User("userSpecial", "special@test.com", "Special", "User");
        var createdEntity = new UserSetting
        {
            Id = "settingSpecial",
            Theme = theme,
            Owner = user
        };

        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        createValidator.Validate(request).Returns(validationResult);

        var identityProvider = Substitute.For<IIdentityProvider>();
        identityProvider.GetCurrentUser().Returns(user);

        var repository = Substitute.For<IRepository<UserSetting>>();
        repository.CreateAsync(Arg.Any<UserSetting>(), user.Id).Returns(createdEntity);

        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        await repository.Received(1).CreateAsync(
            Arg.Is<UserSetting>(e => e.Theme == theme && e.Owner == user),
            user.Id);
    }

    /// <summary>
    /// Tests that GetAsync returns Ok with null when no user settings exist for the current user.
    /// </summary>
    [Fact]
    public async Task GetAsync_NoSettingsExist_ReturnsOkWithNull()
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var user = new User("user-123", "test@example.com", "John", "Doe");
        identityProvider.GetCurrentUser().Returns(user);
        repository.GetAllAsync(user.Id).Returns(Task.FromResult(Enumerable.Empty<UserSetting>()));

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetAsync returns Ok with mapped UserSettingResponse when a single user setting exists.
    /// </summary>
    [Fact]
    public async Task GetAsync_SingleSettingExists_ReturnsOkWithMappedResponse()
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var user = new User("user-456", "user@example.com", "Jane", "Smith");
        var userSetting = new UserSetting { Id = "setting-1", Theme = "dark" };

        identityProvider.GetCurrentUser().Returns(user);
        repository.GetAllAsync(user.Id).Returns(Task.FromResult<IEnumerable<UserSetting>>(new[] { userSetting }));

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be("setting-1");
        result.Value.Theme.Should().Be("dark");
    }

    /// <summary>
    /// Tests that GetAsync returns Ok with the first user setting when multiple settings exist for the current user.
    /// </summary>
    [Fact]
    public async Task GetAsync_MultipleSettingsExist_ReturnsOkWithFirstSetting()
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var user = new User("user-789", "admin@example.com", null, null);
        var firstSetting = new UserSetting { Id = "setting-first", Theme = "light" };
        var secondSetting = new UserSetting { Id = "setting-second", Theme = "dark" };

        identityProvider.GetCurrentUser().Returns(user);
        repository.GetAllAsync(user.Id).Returns(Task.FromResult<IEnumerable<UserSetting>>(new[] { firstSetting, secondSetting }));

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be("setting-first");
        result.Value.Theme.Should().Be("light");
    }

    /// <summary>
    /// Tests that GetAsync passes the correct user ID to the repository when retrieving settings.
    /// </summary>
    [Theory]
    [InlineData("user-1")]
    [InlineData("user-2")]
    [InlineData("different-user-id")]
    public async Task GetAsync_DifferentUserIds_PassesCorrectIdToRepository(string userId)
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var user = new User(userId, "test@example.com", "Test", "User");
        identityProvider.GetCurrentUser().Returns(user);
        repository.GetAllAsync(userId).Returns(Task.FromResult(Enumerable.Empty<UserSetting>()));

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        await service.GetAsync();

        // Assert
        await repository.Received(1).GetAllAsync(userId);
    }

    /// <summary>
    /// Tests that GetAsync correctly handles user settings with null optional properties.
    /// </summary>
    [Fact]
    public async Task GetAsync_SettingWithNullTheme_ReturnsOkWithMappedResponse()
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();

        var user = new User("user-null-theme", "null@example.com", "Null", "Theme");
        var userSetting = new UserSetting { Id = "setting-null", Theme = null };

        identityProvider.GetCurrentUser().Returns(user);
        repository.GetAllAsync(user.Id).Returns(Task.FromResult<IEnumerable<UserSetting>>(new[] { userSetting }));

        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        // Act
        var result = await service.GetAsync();

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be("setting-null");
    }

    /// <summary>
    /// Tests that UpdateAsync returns BadRequest when validation fails.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();
        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        var request = new UserSettingUpdateRequest { Theme = "dark" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Theme", "Invalid theme")
        };
        var validationResult = new ValidationResult(validationFailures);

        updateValidator.Validate(request).Returns(validationResult);

        // Act
        var result = await service.UpdateAsync("test-id", request);

        // Assert
        result.Result.Should().BeOfType<BadRequest<ValidationProblemDetails>>();
    }

    /// <summary>
    /// Tests that UpdateAsync does not call GetCurrentUser when validation fails.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_InvalidRequest_DoesNotCallGetCurrentUser()
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();
        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        var request = new UserSettingUpdateRequest { Theme = "dark" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Theme", "Invalid theme")
        };
        var validationResult = new ValidationResult(validationFailures);

        updateValidator.Validate(request).Returns(validationResult);

        // Act
        await service.UpdateAsync("test-id", request);

        // Assert
        identityProvider.DidNotReceive().GetCurrentUser();
    }

    /// <summary>
    /// Tests that UpdateAsync does not call repository when validation fails.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_InvalidRequest_DoesNotCallRepository()
    {
        // Arrange
        var identityProvider = Substitute.For<IIdentityProvider>();
        var repository = Substitute.For<IRepository<UserSetting>>();
        var createValidator = Substitute.For<IValidator<UserSettingCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<UserSettingUpdateRequest>>();
        var service = new UserSettingService(identityProvider, repository, createValidator, updateValidator);

        var request = new UserSettingUpdateRequest { Theme = "dark" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Theme", "Invalid theme")
        };
        var validationResult = new ValidationResult(validationFailures);

        updateValidator.Validate(request).Returns(validationResult);

        // Act
        await service.UpdateAsync("test-id", request);

        // Assert
        await repository.DidNotReceive().GetByIdAsync(Arg.Any<string>(), Arg.Any<string>());
        await repository.DidNotReceive().UpdateAsync(Arg.Any<UserSetting>(), Arg.Any<string>());
    }

}