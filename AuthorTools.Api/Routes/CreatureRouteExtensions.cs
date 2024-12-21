using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.SharedLib.Models;

namespace AuthorTools.Api.Routes;

public static class CreatureRouteExtensions
{
    public static void MapCreatureRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/creatures")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>()
            .WithTags($"{typeof(Creature).Name}");

        group.MapGet("", async (ICommonEntityService<Creature> entityService)
            => await entityService.GetAllAsync());

        group.MapGet("{id}", async (string id, ICommonEntityService<Creature> entityService)
            => await entityService.GetAsync(id));

        group.MapPost("", async (Creature entity, ICommonEntityService<Creature> entityService)
            => await entityService.CreateAsync(entity));

        group.MapPut("{id}", async (string id, Creature entity, ICommonEntityService<Creature> entityService)
            => await entityService.UpdateAsync(id, entity));

        group.MapPatch("{id}", async (string id, PatchRequest[] patchRequests, ICommonEntityService<Creature> entityService)
            => await entityService.PatchAsync(id, patchRequests));

        group.MapDelete("{id}", async (string id, ICommonEntityService<Creature> entityService)
            => await entityService.DeleteAsync(id));
    }
}