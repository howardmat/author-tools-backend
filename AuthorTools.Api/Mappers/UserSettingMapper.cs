using AuthorTools.Api.Models;
using AuthorTools.Data.Models;

namespace AuthorTools.Api.Mappers;

public static class UserSettingMapper
{
    public static UserSetting ToEntity(this UserSettingCreateRequest request, User owner)
    {
        return new UserSetting
        {
            Theme = request.Theme,
            Owner = owner
        };
    }

    public static UserSetting ToEntity(this UserSettingUpdateRequest request, string id, User owner)
    {
        return new UserSetting
        {
            Id = id,
            Theme = request.Theme,
            Owner = owner
        };
    }

    public static UserSettingResponse ToResponse(this UserSetting entity)
    {
        return new UserSettingResponse
        {
            Id = entity.Id!,
            Theme = entity.Theme!
        };
    }
}
