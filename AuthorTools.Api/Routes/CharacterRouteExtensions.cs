using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;

namespace AuthorTools.Api.Routes;

public static class CharacterRouteExtensions
{
    public static void MapCharacterRoutes(this WebApplication app)
    {
        app.MapGet("/characters", async (ICharacterService characterService) => await characterService.GetAllAsync())
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();

        app.MapGet("/characters/{id}", async (string id, ICharacterService characterService) => await characterService.GetAsync(id))
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();

        app.MapPost("/characters", async (Character character, ICharacterService characterService) => await characterService.CreateAsync(character))
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();

        app.MapPut("/characters/{id}", async (string id, Character character, ICharacterService characterService) => await characterService.UpdateAsync(id, character))
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();

        app.MapDelete("/characters/{id}", async (string id, ICharacterService characterService) => await characterService.DeleteAsync(id))
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();
    }
}
