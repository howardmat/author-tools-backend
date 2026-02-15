using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Api.Validators;
using AuthorTools.Common;
using AuthorTools.Common.Models;
using AuthorTools.Data;
using AuthorTools.Data.Enums;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories;
using AuthorTools.Data.Repositories.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace AuthorTools.Api.Services.UnitTests;


/// <summary>
/// Unit tests for CommonEntityService class.
/// </summary>
public class CommonEntityServiceTests
{
    /// <summary>
    /// Tests that GetAsync handles id with special characters correctly.
    /// Input: Entity id containing special characters.
    /// Expected: Passes special characters to repository and handles result correctly.
    /// </summary>
    [Theory]
    [InlineData("@#$%^&*()")]
    [InlineData("id-with-dashes")]
    [InlineData("id_with_underscores")]
    [InlineData("id.with.dots")]
    [InlineData("123456789")]
    public async Task GetAsync_IdWithSpecialCharacters_PassesToRepository(string entityId)
    {
        // Arrange
        var entityRepository = Substitute.For<IRepository<TestEntity>>();
        var identityProvider = Substitute.For<IIdentityProvider>();
        var fileService = Substitute.For<IFileService>();
        var createValidator = Substitute.For<IValidator<CommonEntityCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<CommonEntityUpdateRequest>>();

        var userId = "user123";
        var user = new User(userId, "test@example.com", "Test", "User");
        var entity = new TestEntity
        {
            Id = entityId,
            Name = "Test",
            DetailSections = new List<DetailSection>()
        };

        identityProvider.GetCurrentUser().Returns(user);
        entityRepository.GetByIdAsync(entityId, userId).Returns(entity);

        var service = new CommonEntityService<TestEntity>(
            entityRepository,
            identityProvider,
            fileService,
            createValidator,
            updateValidator);

        // Act
        var result = await service.GetAsync(entityId);

        // Assert
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<CommonEntityResponse>>();
        await entityRepository.Received(1).GetByIdAsync(entityId, userId);
    }

    /// <summary>
    /// Tests that GetAsync uses the correct user id from identity provider.
    /// Input: Different user ids.
    /// Expected: Repository is called with the user id from identity provider.
    /// </summary>
    [Theory]
    [InlineData("user1")]
    [InlineData("user2")]
    [InlineData("admin")]
    public async Task GetAsync_DifferentUsers_UsesCorrectUserId(string userId)
    {
        // Arrange
        var entityRepository = Substitute.For<IRepository<TestEntity>>();
        var identityProvider = Substitute.For<IIdentityProvider>();
        var fileService = Substitute.For<IFileService>();
        var createValidator = Substitute.For<IValidator<CommonEntityCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<CommonEntityUpdateRequest>>();

        var entityId = "entity123";
        var user = new User(userId, "test@example.com", "Test", "User");

        identityProvider.GetCurrentUser().Returns(user);
        entityRepository.GetByIdAsync(entityId, userId).Returns((TestEntity?)null);

        var service = new CommonEntityService<TestEntity>(
            entityRepository,
            identityProvider,
            fileService,
            createValidator,
            updateValidator);

        // Act
        var result = await service.GetAsync(entityId);

        // Assert
        await entityRepository.Received(1).GetByIdAsync(entityId, userId);
    }

    /// <summary>
    /// Helper test entity class for testing generic CommonEntityService.
    /// </summary>
    public class TestEntity : CommonEntity
    {
    }

    /// <summary>
    /// Test entity class that inherits from CommonEntity for testing purposes.
    /// </summary>
    public class TestCommonEntity : CommonEntity
    {
    }

    /// <summary>
    /// Tests that DeleteAsync deletes only the entity when ImageFileId is null, empty, or whitespace.
    /// </summary>
    /// <param name="imageFileId">The ImageFileId value (null, empty, or whitespace).</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task DeleteAsync_EntityFoundWithNullOrWhitespaceImageFileId_DeletesEntityOnlyReturnsOk(string? imageFileId)
    {
        // Arrange
        var entityRepository = Substitute.For<IRepository<TestEntity>>();
        var identityProvider = Substitute.For<IIdentityProvider>();
        var fileService = Substitute.For<IFileService>();
        var createValidator = Substitute.For<IValidator<CommonEntityCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<CommonEntityUpdateRequest>>();

        var userId = "user123";
        var entityId = "entity456";
        var user = new User(userId, "test@example.com", "John", "Doe");
        var entity = new TestEntity { ImageFileId = imageFileId };

        identityProvider.GetCurrentUser().Returns(user);
        entityRepository.GetByIdAsync(entityId, userId).Returns(entity);

        var service = new CommonEntityService<TestEntity>(
            entityRepository,
            identityProvider,
            fileService,
            createValidator,
            updateValidator);

        // Act
        var result = await service.DeleteAsync(entityId);

        // Assert
        await fileService.DidNotReceive().DeleteAsync(Arg.Any<string>());
        await entityRepository.Received(1).DeleteAsync(entityId, userId);
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    /// <summary>
    /// Tests that DeleteAsync handles various valid entity ID formats correctly.
    /// </summary>
    /// <param name="entityId">The entity ID to test.</param>
    [Theory]
    [InlineData("simple-id")]
    [InlineData("123456")]
    [InlineData("guid-like-507f1f77bcf86cd799439011")]
    [InlineData("id_with_underscores")]
    [InlineData("id-with-dashes")]
    public async Task DeleteAsync_VariousValidIds_ProcessesCorrectly(string entityId)
    {
        // Arrange
        var entityRepository = Substitute.For<IRepository<TestEntity>>();
        var identityProvider = Substitute.For<IIdentityProvider>();
        var fileService = Substitute.For<IFileService>();
        var createValidator = Substitute.For<IValidator<CommonEntityCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<CommonEntityUpdateRequest>>();

        var userId = "user123";
        var user = new User(userId, "test@example.com", "John", "Doe");
        var entity = new TestEntity { ImageFileId = "file123" };

        identityProvider.GetCurrentUser().Returns(user);
        entityRepository.GetByIdAsync(entityId, userId).Returns(entity);

        var service = new CommonEntityService<TestEntity>(
            entityRepository,
            identityProvider,
            fileService,
            createValidator,
            updateValidator);

        // Act
        var result = await service.DeleteAsync(entityId);

        // Assert
        await entityRepository.Received(1).GetByIdAsync(entityId, userId);
        await entityRepository.Received(1).DeleteAsync(entityId, userId);
        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    /// <summary>
    /// Tests GetAllAsync with empty string workspaceId.
    /// Input: Empty string workspaceId.
    /// Expected: Ok result (empty string is passed to repository as-is).
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAllAsync_EmptyOrWhitespaceWorkspaceId_PassesToRepository(string workspaceId)
    {
        // Arrange
        var userId = "user-456";
        var user = new User(userId, "test@example.com", "John", "Doe");
        var entities = new List<TestCommonEntity>();

        var identityProvider = Substitute.For<IIdentityProvider>();
        var entityRepository = Substitute.For<IRepository<TestCommonEntity>>();
        var fileService = Substitute.For<IFileService>();
        var createValidator = Substitute.For<IValidator<CommonEntityCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<CommonEntityUpdateRequest>>();

        identityProvider.GetCurrentUser().Returns(user);
        entityRepository.GetAllAsync<TestCommonEntity>(userId, workspaceId, SortOrder.Ascending)
            .Returns(entities);

        var service = new CommonEntityService<TestCommonEntity>(
            entityRepository,
            identityProvider,
            fileService,
            createValidator,
            updateValidator);

        // Act
        var result = await service.GetAllAsync(workspaceId);

        // Assert
        await entityRepository.Received(1).GetAllAsync<TestCommonEntity>(
            userId,
            workspaceId,
            SortOrder.Ascending);
        result.Value.Should().BeEmpty();
    }

    /// <summary>
    /// Tests GetAllAsync with special characters in workspaceId.
    /// Input: WorkspaceId containing special characters.
    /// Expected: Ok result (special characters are passed to repository as-is).
    /// </summary>
    [Theory]
    [InlineData("workspace-!@#$%^&*()")]
    [InlineData("workspace\nwith\nnewlines")]
    [InlineData("workspace\twith\ttabs")]
    public async Task GetAllAsync_SpecialCharactersInWorkspaceId_PassesToRepository(string workspaceId)
    {
        // Arrange
        var userId = "user-456";
        var user = new User(userId, "test@example.com", "John", "Doe");
        var entities = new List<TestCommonEntity>();

        var identityProvider = Substitute.For<IIdentityProvider>();
        var entityRepository = Substitute.For<IRepository<TestCommonEntity>>();
        var fileService = Substitute.For<IFileService>();
        var createValidator = Substitute.For<IValidator<CommonEntityCreateRequest>>();
        var updateValidator = Substitute.For<IValidator<CommonEntityUpdateRequest>>();

        identityProvider.GetCurrentUser().Returns(user);
        entityRepository.GetAllAsync<TestCommonEntity>(userId, workspaceId, SortOrder.Ascending)
            .Returns(entities);

        var service = new CommonEntityService<TestCommonEntity>(
            entityRepository,
            identityProvider,
            fileService,
            createValidator,
            updateValidator);

        // Act
        var result = await service.GetAllAsync(workspaceId);

        // Assert
        await entityRepository.Received(1).GetAllAsync<TestCommonEntity>(
            userId,
            workspaceId,
            SortOrder.Ascending);
        result.Value.Should().BeEmpty();
    }

    //private readonly IRepository<TestCommonEntity> _entityRepository;
    //private readonly IIdentityProvider _identityProvider;
    //private readonly IFileService _fileService;
    //private readonly IValidator<CommonEntityCreateRequest> _createValidator;
    //private readonly IValidator<CommonEntityUpdateRequest> _updateValidator;
    //private readonly CommonEntityService<TestCommonEntity> _sut;

    //public CommonEntityServiceTests()
    //{
    //    _entityRepository = Substitute.For<IRepository<TestCommonEntity>>();
    //    _identityProvider = Substitute.For<IIdentityProvider>();
    //    _fileService = Substitute.For<IFileService>();
    //    _createValidator = Substitute.For<IValidator<CommonEntityCreateRequest>>();
    //    _updateValidator = Substitute.For<IValidator<CommonEntityUpdateRequest>>();

    //    _sut = new CommonEntityService<TestCommonEntity>(
    //        _entityRepository,
    //        _identityProvider,
    //        _fileService,
    //        _createValidator,
    //        _updateValidator);
    //}

}