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

    public async Task<UserSetting?> GetAsync()
    {
        var user = _identityProvider.GetCurrentUser();
        return (await _repository.GetAllAsync(user.Id)).FirstOrDefault();
    }

    public async Task<UserSetting> CreateAsync(UserSetting userSetting)
    {
        var user = _identityProvider.GetCurrentUser();

        userSetting.Owner = user;

        return await _repository.CreateAsync(userSetting, user.Id);
    }

    public async Task<UserSetting> UpdateAsync(string id, UserSetting userSetting)
    {
        var user = _identityProvider.GetCurrentUser();

        userSetting.Id = id;
        userSetting.Owner = user;

        return await _repository.UpdateAsync(userSetting, user.Id);
    }
}
