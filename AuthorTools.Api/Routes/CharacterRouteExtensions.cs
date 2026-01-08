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

        //todo 
        //1. Create DTO models in Api, eg. /models/requests/CreateCharacterRequest
        //2. Create DTO validators in Api, eg. /validators/CreateCharacterValidator
        //3. Update Service layer to map DTO models to Data models
        //4. Can this RouteExtension be made into a single class that supports Generics for common entities?

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
