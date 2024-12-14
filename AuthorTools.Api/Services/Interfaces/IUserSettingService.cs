using AuthorTools.Data.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface IUserSettingService
{
    Task<UserSetting> GetAsync();
    Task<UserSetting> CreateAsync(UserSetting userSetting);
    Task<UserSetting> UpdateAsync(string id, UserSetting userSetting);
}
