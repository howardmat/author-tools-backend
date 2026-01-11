using AuthorTools.Api.Filters;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;

namespace AuthorTools.Api.Routes;

public static class WorkspaceRouteExtensions
{
    public static void MapWorkspaceRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/workspace")
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>()
            .WithTags("Workspace");

        group.MapGet("", async (IWorkspaceService service) => await service.GetAllAsync());

        group.MapGet("{id}", async (string id, IWorkspaceService service) => await service.GetAsync(id));

        group.MapPost("", async (WorkspaceCreateRequest request, IWorkspaceService service) => await service.CreateAsync(request));

        group.MapPut("{id}", async (string id, WorkspaceUpdateRequest request, IWorkspaceService service) => await service.UpdateAsync(id, request));

        group.MapDelete("{id}", async (string id, IWorkspaceService service) => await service.DeleteAsync(id));
    }
}
