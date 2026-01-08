using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using System.Text.Json;

namespace AuthorTools.Api.Services;

public class WorkspaceService(
    IRepository<Workspace> repository,
    IIdentityProvider identityProvider,
    WorkspaceValidationService workspaceValidationService,
    JsonSerializerOptions jsonSerializerOptions) : IWorkspaceService
{
    private readonly IRepository<Workspace> _repository = repository;
    private readonly IIdentityProvider _identityProvider = identityProvider;
    private readonly WorkspaceValidationService _workspaceValidationService = workspaceValidationService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

    public async Task<IEnumerable<WorkspaceResponse>> GetAllAsync()
    {
        var user = _identityProvider.GetCurrentUser();
        var workspaces = await _repository.GetAllAsync(user.Id);

        if (workspaces.Count() == 0)
        {
            var defaultWorkspace = await LoadDefaultWorkspaceTemplateAsync();
            defaultWorkspace.Owner = user;
            
            var createdWorkspace = await _repository.CreateAsync(defaultWorkspace, user.Id);
            workspaces = [createdWorkspace];
        }

        return workspaces.Select(w => w.ToResponse());
    }

    public async Task<WorkspaceResponse> GetAsync(string id)
    {
        var user = _identityProvider.GetCurrentUser();
        var entity = await _repository.GetByIdAsync(id, user.Id);
        return entity.ToResponse();
    }

    public async Task<WorkspaceResponse> CreateAsync(WorkspaceCreateRequest request)
    {
        var user = _identityProvider.GetCurrentUser();

        var entity = request.ToEntity(user);
        entity.Owner = user;

        if (entity.IsDefault)
        {
            await ClearDefaultWorkspaceAsync(null, user.Id);
        }

        var created = await _repository.CreateAsync(entity, user.Id);
        return created.ToResponse();
    }

    public async Task<WorkspaceResponse> UpdateAsync(string id, WorkspaceUpdateRequest request)
    {
        var user = _identityProvider.GetCurrentUser();

        var entity = request.ToEntity(id, user);
        entity.Id = id;
        entity.Owner = user;

        if (entity.IsDefault)
        {
            await ClearDefaultWorkspaceAsync(id, user.Id);
        }

        var updated = await _repository.UpdateAsync(entity, user.Id);
        return updated.ToResponse();
    }

    public async Task<ServiceResult> DeleteAsync(string id)
    {
        var result = new ServiceResult();

        var user = _identityProvider.GetCurrentUser();

        if (_workspaceValidationService.AnyData(id, user.Id))
        {
            result.Error = "WORKSPACE_ASSOCIATED_DATA_EXISTS";
            return result;
        }

        if (await _workspaceValidationService.IsLastAsync(user.Id))
        {
            result.Error = "WORKSPACE_IS_LAST";
            return result;
        }

        await _repository.DeleteAsync(id, user.Id);
        result.Success = true;

        return result;
    }

    private async Task<Workspace> LoadDefaultWorkspaceTemplateAsync()
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Workspace", "default.json");
        var jsonContent = await File.ReadAllTextAsync(templatePath);
        
        var workspace = JsonSerializer.Deserialize<Workspace>(jsonContent, _jsonSerializerOptions);
        return workspace ?? new Workspace
        {
            Name = "My Workspace",
            Description = "My first workspace",
            IsDefault = true
        };
    }

    private async Task ClearDefaultWorkspaceAsync(string? id, string userId)
    {
        var workspaces = await _repository.GetAllAsync(userId);
        foreach (var workspace in workspaces)
        {
            if (workspace.Id != id && workspace.IsDefault)
            {
                workspace.IsDefault = false;
                await _repository.UpdateAsync(workspace, userId);
            }
        }
    }
}
