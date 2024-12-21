using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.SharedLib.Models;

namespace AuthorTools.Api.Routes;

public static class CharacterRouteExtensions
{
    public static void MapCharacterRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/characters")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>()
            .WithTags($"{typeof(Character).Name}");

        group.MapGet("", async (ICommonEntityService<Character> entityService)
            => await entityService.GetAllAsync());

        group.MapGet("{id}", async (string id, ICommonEntityService<Character> entityService)
            => await entityService.GetAsync(id));

        group.MapPost("", async (Character entity, ICommonEntityService<Character> entityService)
            => await entityService.CreateAsync(entity));

        group.MapPut("{id}", async (string id, Character entity, ICommonEntityService<Character> entityService)
            => await entityService.UpdateAsync(id, entity));

        group.MapPatch("{id}", async (string id, PatchRequest[] patchRequests, ICommonEntityService<Character> entityService)
            => await entityService.PatchAsync(id, patchRequests));

        group.MapDelete("{id}", async (string id, ICommonEntityService<Character> entityService)
            => await entityService.DeleteAsync(id));
    }
}
