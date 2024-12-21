using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Enums;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using AuthorTools.SharedLib.Models;

namespace AuthorTools.Api.Services;

public class CommonEntityService<T> : ICommonEntityService<T> where T : CommonEntity
{
    private readonly ICommonEntityRepository<T> _entityRepo;
    private readonly IIdentityProvider _identityProvider;
    private readonly IFileService _fileService;

    public CommonEntityService(
        ICommonEntityRepository<T> entityRepository,
        IIdentityProvider identityProvider,
        IFileService fileService)
    {
        _entityRepo = entityRepository;
        _identityProvider = identityProvider;
        _fileService = fileService;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var user = _identityProvider.GetCurrentUser();
        return await _entityRepo.GetAllAsync<T>(user.Id, SortOrder.Ascending);
    }

    public async Task<T> GetAsync(string id)
    {
        var user = _identityProvider.GetCurrentUser();
        return await _entityRepo.GetByIdAsync(id, user.Id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        var user = _identityProvider.GetCurrentUser();

        entity.Owner = user;

        return await _entityRepo.CreateAsync(entity, user.Id);
    }

    public async Task<T> UpdateAsync(string id, T entity)
    {
        var user = _identityProvider.GetCurrentUser();

        entity.Id = id;
        entity.Owner = user;

        return await _entityRepo.UpdateAsync(entity, user.Id);
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
