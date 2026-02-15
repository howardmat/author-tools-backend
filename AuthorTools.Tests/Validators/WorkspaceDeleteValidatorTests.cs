using AuthorTools.Api.Validators;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using FluentAssertions;
using FluentValidation.TestHelper;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace AuthorTools.Api.Validators.UnitTests;

public class WorkspaceDeleteValidatorTests
{
    /// <summary>
    /// Tests that the constructor successfully initializes the validator with a valid repository.
    /// Expected: Validator is created without errors.
    /// </summary>
    [Fact]
    public void Constructor_ValidRepository_CreatesValidator()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();

        // Act
        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Assert
        validator.Should().NotBeNull();
    }

    /// <summary>
    /// Tests validation when workspace is null.
    /// Expected: Validation passes (returns true in the predicate).
    /// </summary>
    [Fact]
    public async Task Constructor_NullWorkspace_ValidationPasses()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var validator = new WorkspaceDeleteValidator(workspaceRepo);
        var workspace = new Workspace { Name = "Test", Owner = null };

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Tests validation when workspace has null owner.
    /// Expected: Validation passes (returns true in the predicate).
    /// </summary>
    [Fact]
    public async Task Constructor_WorkspaceWithNullOwner_ValidationPasses()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var validator = new WorkspaceDeleteValidator(workspaceRepo);
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = null
        };

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Tests validation when workspace owner has null Id.
    /// Expected: Validation passes (returns true in the predicate).
    /// </summary>
    [Fact]
    public async Task Constructor_WorkspaceWithOwnerHavingNullId_ValidationPasses()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var validator = new WorkspaceDeleteValidator(workspaceRepo);
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = new User(null!, "test@example.com", "First", "Last")
            {
                Id = null!
            }
        };

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Tests validation when user has only one workspace (count = 1).
    /// Expected: Validation fails with "WORKSPACE_IS_LAST" error message.
    /// </summary>
    [Fact]
    public async Task Constructor_UserHasOnlyOneWorkspace_ValidationFails()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = "user123";
        var workspace = new Workspace
        {
            Name = "Only Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var existingWorkspaces = new List<Workspace> { workspace };
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult<IEnumerable<Workspace>>(existingWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("WORKSPACE_IS_LAST");
    }

    /// <summary>
    /// Tests validation when user has exactly two workspaces (count = 2).
    /// Expected: Validation passes (count > 1).
    /// </summary>
    [Fact]
    public async Task Constructor_UserHasTwoWorkspaces_ValidationPasses()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = "user123";
        var workspace1 = new Workspace
        {
            Name = "First Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };
        var workspace2 = new Workspace
        {
            Name = "Second Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var existingWorkspaces = new List<Workspace> { workspace1, workspace2 };
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult<IEnumerable<Workspace>>(existingWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace1);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Tests validation when user has multiple workspaces (count > 2).
    /// Expected: Validation passes (count > 1).
    /// </summary>
    [Fact]
    public async Task Constructor_UserHasMultipleWorkspaces_ValidationPasses()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = "user123";
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var existingWorkspaces = new List<Workspace>
        {
            workspace,
            new Workspace { Name = "Workspace 2", Owner = workspace.Owner },
            new Workspace { Name = "Workspace 3", Owner = workspace.Owner }
        };
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult<IEnumerable<Workspace>>(existingWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Tests validation when repository returns empty collection for owner.
    /// Expected: Validation fails (count = 0, not > 1).
    /// </summary>
    [Fact]
    public async Task Constructor_RepositoryReturnsEmptyCollection_ValidationFails()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = "user123";
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var emptyWorkspaces = Enumerable.Empty<Workspace>();
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult(emptyWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("WORKSPACE_IS_LAST");
    }

    /// <summary>
    /// Tests validation with workspace having empty string owner Id.
    /// Expected: Validation queries repository and result depends on count.
    /// </summary>
    [Fact]
    public async Task Constructor_WorkspaceWithEmptyStringOwnerId_QueriesRepository()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = "";
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var existingWorkspaces = new List<Workspace> { workspace };
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult<IEnumerable<Workspace>>(existingWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        await workspaceRepo.Received(1).GetAllAsync(ownerId);
        result.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Tests validation with workspace having whitespace-only owner Id.
    /// Expected: Validation queries repository with the whitespace Id.
    /// </summary>
    [Fact]
    public async Task Constructor_WorkspaceWithWhitespaceOwnerId_QueriesRepository()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = "   ";
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var existingWorkspaces = new List<Workspace> { workspace, new Workspace { Name = "Another", Owner = workspace.Owner } };
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult<IEnumerable<Workspace>>(existingWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        await workspaceRepo.Received(1).GetAllAsync(ownerId);
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Tests validation with workspace having very long owner Id.
    /// Expected: Validation queries repository with the long Id.
    /// </summary>
    [Fact]
    public async Task Constructor_WorkspaceWithVeryLongOwnerId_QueriesRepository()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = new string('a', 10000);
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var existingWorkspaces = new List<Workspace> { workspace, new Workspace { Name = "Another", Owner = workspace.Owner } };
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult<IEnumerable<Workspace>>(existingWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        await workspaceRepo.Received(1).GetAllAsync(ownerId);
        result.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Tests validation with special characters in owner Id.
    /// Expected: Validation queries repository with the special character Id.
    /// </summary>
    [Fact]
    public async Task Constructor_WorkspaceWithSpecialCharactersInOwnerId_QueriesRepository()
    {
        // Arrange
        var workspaceRepo = Substitute.For<IRepository<Workspace>>();
        var ownerId = "!@#$%^&*()_+-={}[]|:;<>?,./~`";
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Owner = new User(ownerId, "test@example.com", "First", "Last")
        };

        var existingWorkspaces = new List<Workspace> { workspace, new Workspace { Name = "Another", Owner = workspace.Owner } };
        workspaceRepo.GetAllAsync(ownerId).Returns(Task.FromResult<IEnumerable<Workspace>>(existingWorkspaces));

        var validator = new WorkspaceDeleteValidator(workspaceRepo);

        // Act
        var result = await validator.TestValidateAsync(workspace);

        // Assert
        await workspaceRepo.Received(1).GetAllAsync(ownerId);
        result.IsValid.Should().BeTrue();
    }
}