using AuthorTools.Api.Filters;
using AuthorTools.Api.Models;
using AuthorTools.Api.Options;
using AuthorTools.Api.Repositories;
using AuthorTools.Api.Services;
using AuthorTools.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AuthorTools.Api;

public static class RouteExtensions
{
    public static IApplicationBuilder UseRoutes(this WebApplication app)
    {
        app.MapGet("/characters",
            async (ICharacterRepository characterRepo, IIdentityProvider userProvider) =>
        {
            var user = userProvider.GetCurrentUser();

            return await characterRepo.GetAllAsync(user.Id);
        }).RequireAuthorization()
        .AddEndpointFilter<JwtUserEndpointFilter>();

        app.MapGet("/characters/{id}",
            async (string id, ICharacterRepository characterRepo,
                IIdentityProvider userProvider) =>
        {
            var user = userProvider.GetCurrentUser();

            return await characterRepo.GetByIdAsync(id, user.Id);
        }).RequireAuthorization();

        app.MapPost("/characters",
            async (Character character, ICharacterRepository characterRepo,
                IOptions<ApplicationOptions> options, IIdentityProvider userProvider) =>
        {
            character.Id = Guid.NewGuid().ToString();

            var user = userProvider.GetCurrentUser();

            character.Owner = user;
            return await characterRepo.AddAsync(character, user.Id);
        }).RequireAuthorization();

        app.MapPut("/characters/{id}",
            async (string id, Character character, ICharacterRepository characterRepo,
                IOptions<ApplicationOptions> options, IIdentityProvider userProvider) =>
        {
            character.Id = id;
            character.PartitionKey = options.Value.Environment;

            var user = userProvider.GetCurrentUser();

            character.Owner = user;
            return await characterRepo.UpdateAsync(character, user.Id);
        }).RequireAuthorization();

        app.MapGet("/file/{id}",
            async (string id, FileStorageService fileStorageService) =>
        {
            var fileResult = await fileStorageService.GetBlobAsync(id);
            return Results.File(fileResult.FileContent, fileResult.ContentType, fileResult.FileName);
        });

        app.MapPost("/file",
            async (IFormFile file, FileStorageService fileStorageService) =>
        {
            var fileId = Guid.NewGuid().ToString();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            await fileStorageService.UploadBlobAsync(file.FileName, fileId, file.ContentType, memoryStream.ToArray());

            return fileId;
        }).DisableAntiforgery()
        .RequireAuthorization();

        return app;
    }
}
