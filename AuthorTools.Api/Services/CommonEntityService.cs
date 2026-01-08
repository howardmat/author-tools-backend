using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Enums;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using AuthorTools.Common.Models;
using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;

namespace AuthorTools.Api.Services;

public class CommonEntityService<T> : ICommonEntityService<T>
    where T : CommonEntity, new() 
{
    private readonly IRepository<T> _entityRepo;
    private readonly IIdentityProvider _identityProvider;
    private readonly IFileService _fileService;

    public CommonEntityService(
        IRepository<T> entityRepository,
        IIdentityProvider identityProvider,
        IFileService fileService)
    {
        _entityRepo = entityRepository;
        _identityProvider = identityProvider;
        _fileService = fileService;
    }

    public async Task<IEnumerable<CommonEntityResponse>> GetAllAsync(string workspaceId)
    {
        var user = _identityProvider.GetCurrentUser();
        var entities = await _entityRepo.GetAllAsync<T>(user.Id, workspaceId, SortOrder.Ascending);
        return entities.Select(e => e.ToResponse());
    }

    public async Task<CommonEntityResponse> GetAsync(string id)
    {
        var user = _identityProvider.GetCurrentUser();
        var entity = await _entityRepo.GetByIdAsync(id, user.Id);
        return entity.ToResponse();
    }

    public async Task<CommonEntityResponse> CreateAsync(CommonEntityCreateRequest request)
    {
        var user = _identityProvider.GetCurrentUser();

        var entity = request.ToEntity<T>(user);
        var created = await _entityRepo.CreateAsync(entity, user.Id);

        return created.ToResponse();
    }

    public async Task<CommonEntityResponse> UpdateAsync(string id, CommonEntityUpdateRequest request)
    {
        var user = _identityProvider.GetCurrentUser();

        var entity = request.ToEntity<T>(id, user);
        var updated = await _entityRepo.UpdateAsync(entity, user.Id);

        return updated.ToResponse();
    }

    public async Task PatchAsync(string id, IEnumerable<PatchRequest> patchRequests)
    {
        var user = _identityProvider.GetCurrentUser();

        await _entityRepo.PatchAsync(id, patchRequests, user.Id);
    }

    public async Task DeleteAsync(string id)
    {
        var user = _identityProvider.GetCurrentUser();

        var entity = await GetAsync(id);
        if (!string.IsNullOrWhiteSpace(entity.ImageFileId))
        {
            await _fileService.DeleteAsync(entity.ImageFileId);
        }

        await _entityRepo.DeleteAsync(id, user.Id);
    }
}
