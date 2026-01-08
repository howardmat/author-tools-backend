using AuthorTools.Api.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface IUserSettingService
{
    Task<UserSettingResponse?> GetAsync();
    Task<UserSettingResponse> CreateAsync(UserSettingCreateRequest request);
    Task<UserSettingResponse> UpdateAsync(string id, UserSettingUpdateRequest request);
}
