using AuthorTools.Api.Models;
using AuthorTools.Api.Options;
using AuthorTools.Api.Repositories;
using Microsoft.Extensions.Options;

namespace AuthorTools.Api;

public static class RouteExtentions
{
    public static IApplicationBuilder UseRoutes(this WebApplication app)
    {
        app.MapGet("/characters", async (ICharacterRepository characterRepo) =>
        {
            return await characterRepo.GetAllAsync();
        });

        app.MapGet("/characters/{id}", async (string id, ICharacterRepository characterRepo) =>
        {
            return await characterRepo.GetByIdAsync(id);
        });

        app.MapPost("/characters", async (Character character, ICharacterRepository characterRepo, IOptions<ApplicationOptions> options) =>
        {
            character.Id = Guid.NewGuid().ToString();
            character.PartitionKey = options.Value.Environment;

            return await characterRepo.AddAsync(character);
        });

        app.MapPut("/characters/{id}", async (string id, Character character, ICharacterRepository characterRepo, IOptions<ApplicationOptions> options) =>
        {
            character.Id = id;
            character.PartitionKey = options.Value.Environment;

            return await characterRepo.UpdateAsync(character);
        });

        return app;
    }
}
