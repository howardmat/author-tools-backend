using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.Common.Models;
using AuthorTools.Api.Models;

namespace AuthorTools.Api.Routes;

public static class CharacterRouteExtensions
{
    public static void MapCharacterRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/characters")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>()
            .WithTags("Character");

        group.MapGet("", async (string workspaceId, ICommonEntityService<Character> entityService)
            => await entityService.GetAllAsync(workspaceId));

        group.MapGet("{id}", async (string id, ICommonEntityService<Character> entityService)
            => await entityService.GetAsync(id));

        group.MapPost("", async (CommonEntityCreateRequest request, ICommonEntityService<Character> entityService)
            => await entityService.CreateAsync(request));

        group.MapPut("{id}", async (string id, CommonEntityUpdateRequest request, ICommonEntityService<Character> entityService)
            => await entityService.UpdateAsync(id, request));

        group.MapPatch("{id}", async (string id, PatchRequest[] patchRequests, ICommonEntityService<Character> entityService)
            => await entityService.PatchAsync(id, patchRequests));

        group.MapDelete("{id}", async (string id, ICommonEntityService<Character> entityService)
            => await entityService.DeleteAsync(id));
    }
}
