using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Api.Services;

public class WorkspaceService(
    IWorkspaceRepository repository,
    IIdentityProvider identityProvider,
    WorkspaceValidationService workspaceValidationService) : IWorkspaceService
{
    private readonly IWorkspaceRepository _repository = repository;
    private readonly IIdentityProvider _identityProvider = identityProvider;
    private readonly WorkspaceValidationService _workspaceValidationService = workspaceValidationService;

    public async Task<IEnumerable<Workspace>> GetAllAsync()
    {
        var user = _identityProvider.GetCurrentUser();
        return await _repository.GetAllAsync(user.Id);
    }

    public async Task<Workspace> GetAsync(string id)
    {
        var user = _identityProvider.GetCurrentUser();
        return await _repository.GetByIdAsync(id, user.Id);
    }

    public async Task<Workspace> CreateAsync(Workspace workspace)
    {
        var user = _identityProvider.GetCurrentUser();

        workspace.Owner = user;

        if (workspace.IsDefault)
        {
            await ClearDefaultWorkspaceAsync(null, user.Id);
        }

        return await _repository.CreateAsync(workspace, user.Id);
    }

    public async Task<Workspace> UpdateAsync(string id, Workspace workspace)
    {
        var user = _identityProvider.GetCurrentUser();

        workspace.Id = id;
        workspace.Owner = user;

        if (workspace.IsDefault)
        {
            await ClearDefaultWorkspaceAsync(id, user.Id);
        }

        return await _repository.UpdateAsync(workspace, user.Id);
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
