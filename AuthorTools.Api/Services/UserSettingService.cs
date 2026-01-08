using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Api.Services;

public class UserSettingService(
    IIdentityProvider identityProvider,
    IRepository<UserSetting> repository) : IUserSettingService
{
    private readonly IRepository<UserSetting> _repository = repository;
    private readonly IIdentityProvider _identityProvider = identityProvider;

    public async Task<UserSettingResponse?> GetAsync()
    {
        var user = _identityProvider.GetCurrentUser();
        var entity = (await _repository.GetAllAsync(user.Id)).FirstOrDefault();
        return entity?.ToResponse();
    }

    public async Task<UserSettingResponse> CreateAsync(UserSettingCreateRequest request)
    {
        var user = _identityProvider.GetCurrentUser();

        var entity = request.ToEntity(user);
        entity.Owner = user;

        var created = await _repository.CreateAsync(entity, user.Id);
        return created.ToResponse();
    }

    public async Task<UserSettingResponse> UpdateAsync(string id, UserSettingUpdateRequest request)
    {
        var user = _identityProvider.GetCurrentUser();

        var entity = request.ToEntity(id, user);
        entity.Id = id;
        entity.Owner = user;

        var updated = await _repository.UpdateAsync(entity, user.Id);
        return updated.ToResponse();
    }
}
