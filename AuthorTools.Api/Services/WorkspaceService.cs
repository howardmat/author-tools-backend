using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Api.Validators;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthorTools.Api.Services;

public class WorkspaceService(
    IRepository<Workspace> repository,
    IIdentityProvider identityProvider,
    IValidator<WorkspaceCreateRequest> createValidator,
    IValidator<WorkspaceUpdateRequest> updateValidator,
    IValidator<Workspace> deleteValidator,
    JsonSerializerOptions jsonSerializerOptions) : IWorkspaceService
{
    public async Task<Ok<IEnumerable<WorkspaceResponse>>> GetAllAsync()
    {
        var user = identityProvider.GetCurrentUser();
        var workspaces = await repository.GetAllAsync(user.Id);

        if (workspaces.Count() == 0)
        {
            var defaultWorkspace = await LoadDefaultWorkspaceTemplateAsync();
            defaultWorkspace.Owner = user;
            
            var createdWorkspace = await repository.CreateAsync(defaultWorkspace, user.Id);
            workspaces = [createdWorkspace];
        }

        return TypedResults.Ok(workspaces.Select(w => w.ToResponse()));
    }

    public async Task<Results<Ok<WorkspaceResponse>, NotFound>> GetAsync(string id)
    {
        var user = identityProvider.GetCurrentUser();
        var entity = await repository.GetByIdAsync(id, user.Id);
        return entity != null 
            ? TypedResults.Ok(entity.ToResponse())
            : TypedResults.NotFound();
    }

    public async Task<Results<Ok<WorkspaceResponse>, BadRequest<ValidationProblemDetails>>> CreateAsync(WorkspaceCreateRequest request)
    {
        var validationResult = createValidator.Validate(request);
        if (!validationResult.IsValid)
            return validationResult.Errors.ToBadRequest();

        var user = identityProvider.GetCurrentUser();

        var entity = request.ToEntity(user);
        entity.Owner = user;

        if (entity.IsDefault)
        {
            await ClearDefaultWorkspaceAsync(null, user.Id);
        }

        var created = await repository.CreateAsync(entity, user.Id);
        return TypedResults.Ok(created.ToResponse());
    }

    public async Task<Results<Ok<WorkspaceResponse>, NotFound, BadRequest<ValidationProblemDetails>>> UpdateAsync(string id, WorkspaceUpdateRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
            return validationResult.Errors.ToBadRequest();

        var user = identityProvider.GetCurrentUser();

        var existingEntity = await repository.GetByIdAsync(id, user.Id);
        if (existingEntity == null)
            return TypedResults.NotFound();

        var entity = request.ToEntity(id, user);
        entity.Id = id;
        entity.Owner = user;

        if (entity.IsDefault)
        {
            await ClearDefaultWorkspaceAsync(id, user.Id);
        }

        var updated = await repository.UpdateAsync(entity, user.Id);
        return TypedResults.Ok(updated.ToResponse());
    }

    public async Task<Results<Ok, NotFound, Conflict<ProblemDetails>>> DeleteAsync(string id)
    {
        var user = identityProvider.GetCurrentUser();

        var existingEntity = await repository.GetByIdAsync(id, user.Id);
        if (existingEntity == null)
            return TypedResults.NotFound();

        var validationResult = await deleteValidator.ValidateAsync(existingEntity);
        if (!validationResult.IsValid)
            return validationResult.Errors.ToConflict("Cannot delete Workspace");

        await repository.DeleteAsync(id, user.Id);

        return TypedResults.Ok();
    }

    private async Task<Workspace> LoadDefaultWorkspaceTemplateAsync()
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Workspace", "default.json");
        var jsonContent = await File.ReadAllTextAsync(templatePath);
        
        var workspace = JsonSerializer.Deserialize<Workspace>(jsonContent, jsonSerializerOptions);
        return workspace ?? new Workspace
        {
            Name = "My Workspace",
            Description = "My first workspace",
            IsDefault = true
        };
    }

    private async Task ClearDefaultWorkspaceAsync(string? id, string userId)
    {
        var workspaces = await repository.GetAllAsync(userId);
        foreach (var workspace in workspaces)
        {
            if (workspace.Id != id && workspace.IsDefault)
            {
                workspace.IsDefault = false;
                await repository.UpdateAsync(workspace, userId);
            }
        }
    }
}
