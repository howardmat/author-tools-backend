using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Models;

namespace AuthorTools.Api.Routes;

public static class WorkspaceRouteExtensions
{
    public static void MapWorkspaceRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/workspace")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>()
            .WithTags($"{typeof(Workspace).Name}");

        group.MapGet("", async (IWorkspaceService service) => await service.GetAllAsync());

        group.MapPost("", async (Workspace model, IWorkspaceService service) => await service.CreateAsync(model));

        group.MapPut("{id}", async (string id, Workspace model, IWorkspaceService service) => await service.UpdateAsync(id, model));

        group.MapDelete("{id}", async (string id, IWorkspaceService service) => (await service.DeleteAsync(id)).ToHttpResult());
    }
}
