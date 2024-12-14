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
            .RequireCors();

        group.MapGet("", async (ICharacterService characterService) => await characterService.GetAllAsync());

        group.MapGet("{id}", async (string id, ICharacterService characterService) => await characterService.GetAsync(id));

        group.MapPost("", async (Character character, ICharacterService characterService) => await characterService.CreateAsync(character));

        group.MapPut("{id}", async (string id, Character character, ICharacterService characterService) => await characterService.UpdateAsync(id, character));

        group.MapPatch("{id}", async (string id, PatchRequest[] patchRequests, ICharacterService characterService) =>
            await characterService.PatchAsync(id, patchRequests));

        group.MapDelete("{id}", async (string id, ICharacterService characterService) => await characterService.DeleteAsync(id));
    }
}
