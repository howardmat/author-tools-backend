using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;
using AuthorTools.Common.Models;
using AuthorTools.Api.Models;

namespace AuthorTools.Api.Routes;

public static class LocationRouteExtensions
{
    public static void MapLocationRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/locations")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>()
            .WithTags("Location");

        group.MapGet("", async (string workspaceId, ICommonEntityService<Location> entityService)
            => await entityService.GetAllAsync(workspaceId));

        group.MapGet("{id}", async (string id, ICommonEntityService<Location> entityService)
            => await entityService.GetAsync(id));

        group.MapPost("", async (CommonEntityCreateRequest request, ICommonEntityService<Location> entityService)
            => await entityService.CreateAsync(request));

        group.MapPut("{id}", async (string id, CommonEntityUpdateRequest request, ICommonEntityService<Location> entityService)
            => await entityService.UpdateAsync(id, request));

        group.MapPatch("{id}", async (string id, PatchRequest[] patchRequests, ICommonEntityService<Location> entityService)
            => await entityService.PatchAsync(id, patchRequests));

        group.MapDelete("{id}", async (string id, ICommonEntityService<Location> entityService)
            => await entityService.DeleteAsync(id));
    }
}