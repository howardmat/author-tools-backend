using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Api.Services;

public class WorkspaceService(IWorkspaceRepository repository, IIdentityProvider identityProvider) : IWorkspaceService
{
    private readonly IWorkspaceRepository _repository = repository;
    private readonly IIdentityProvider _identityProvider = identityProvider;

    public async Task<IEnumerable<Workspace>> GetAllAsync()
    {
        var user = _identityProvider.GetCurrentUser();
        return await _repository.GetAllAsync(user.Id);
    }

    public async Task<Workspace> CreateAsync(Workspace workspace)
    {
        var user = _identityProvider.GetCurrentUser();

        workspace.Owner = user;

        return await _repository.CreateAsync(workspace, user.Id);
    }

    public async Task<Workspace> UpdateAsync(string id, Workspace workspace)
    {
        var user = _identityProvider.GetCurrentUser();

        workspace.Id = id;
        workspace.Owner = user;

        return await _repository.UpdateAsync(workspace, user.Id);
    }
}
