using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;

namespace AuthorTools.Api.Routes;

public static class UserSettingRouteExtensions
{
    public static void MapUserSettingRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/user-setting")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();

        group.MapGet("", async (IUserSettingService service) => await service.GetAsync());

        group.MapPost("", async (UserSetting model, IUserSettingService service) => await service.CreateAsync(model));

        group.MapPut("{id}", async (string id, UserSetting model, IUserSettingService service) => await service.UpdateAsync(id, model));
    }
}
