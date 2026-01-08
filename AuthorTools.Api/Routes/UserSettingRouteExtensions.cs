using AuthorTools.Api.Filters;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;

namespace AuthorTools.Api.Routes;

public static class UserSettingRouteExtensions
{
    public static void MapUserSettingRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/user-setting")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>()
            .WithTags("UserSetting");

        group.MapGet("", async (IUserSettingService service) => await service.GetAsync());

        group.MapPost("", async (UserSettingCreateRequest request, IUserSettingService service) => await service.CreateAsync(request));

        group.MapPut("{id}", async (string id, UserSettingUpdateRequest request, IUserSettingService service) => await service.UpdateAsync(id, request));
    }
}
