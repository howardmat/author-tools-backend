using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;

namespace AuthorTools.Api.Routes;

public static class FileRouteExtensions
{
    public static void MapFileRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/file")
            .WithTags("File");

        group.MapGet("{id}", async (string id, IFileService fileService) => await fileService.GetFileResult(id));

        group.MapPost("", async (IFormFile file, IFileService fileService) => await fileService.UploadAsync(file))
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();
    }
}
