using AuthorTools.Api.Filters;
using AuthorTools.Api.Services.Interfaces;

namespace AuthorTools.Api.Routes;

public static class FileRoutes
{
    public static void Add(WebApplication app)
    {
        app.MapGet("/file/{id}", async (string id, IFileService fileService) => await fileService.GetFileResult(id));

        app.MapPost("/file", async (IFormFile file, IFileService fileService) => await fileService.UploadAsync(file))
            .DisableAntiforgery()
            .RequireAuthorization()
            .AddEndpointFilter<JwtUserEndpointFilter>();
    }
}
