using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Enums;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using AuthorTools.Common.Models;
using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using AuthorTools.Api.Validators;

namespace AuthorTools.Api.Services;

public class CommonEntityService<T>(
    IRepository<T> entityRepository,
    IIdentityProvider identityProvider,
    IFileService fileService,
    IValidator<CommonEntityCreateRequest> createValidator,
    IValidator<CommonEntityUpdateRequest> updateValidator) : ICommonEntityService<T>
    where T : CommonEntity, new() 
{
    public async Task<Ok<IEnumerable<CommonEntityResponse>>> GetAllAsync(string workspaceId)
    {
        var user = identityProvider.GetCurrentUser();
        var entities = await entityRepository.GetAllAsync<T>(user.Id, workspaceId, SortOrder.Ascending);
        return TypedResults.Ok(entities.Select(e => e.ToResponse()));
    }

    public async Task<Results<Ok<CommonEntityResponse>, NotFound>> GetAsync(string id)
    {
        var user = identityProvider.GetCurrentUser();
        var entity = await entityRepository.GetByIdAsync(id, user.Id);
        return entity != null 
            ? TypedResults.Ok(entity.ToResponse())
            : TypedResults.NotFound();
    }

    public async Task<Results<Ok<CommonEntityResponse>, BadRequest<ValidationProblemDetails>>> CreateAsync(CommonEntityCreateRequest request)
    {
        var validationResult = createValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToBadRequest();
        }

        var user = identityProvider.GetCurrentUser();

        var entity = request.ToEntity<T>(user);
        var created = await entityRepository.CreateAsync(entity, user.Id);

        return TypedResults.Ok(created.ToResponse());
    }

    public async Task<Results<Ok<CommonEntityResponse>, NotFound, BadRequest<ValidationProblemDetails>>> UpdateAsync(string id, CommonEntityUpdateRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToBadRequest();
        }

        var user = identityProvider.GetCurrentUser();

        var existingEntity = await entityRepository.GetByIdAsync(id, user.Id);
        if (existingEntity == null)
            return TypedResults.NotFound();

        var entity = request.ToEntity<T>(id, user);
        var updated = await entityRepository.UpdateAsync(entity, user.Id);

        return TypedResults.Ok(updated.ToResponse());
    }

    public async Task<Results<Ok<CommonEntityResponse>, NotFound>> PatchAsync(string id, IEnumerable<PatchRequest> patchRequests)
    {
        var user = identityProvider.GetCurrentUser();

        var existingEntity = await entityRepository.GetByIdAsync(id, user.Id);
        if (existingEntity == null)
            return TypedResults.NotFound();

        var updated = await entityRepository.PatchAsync(id, patchRequests, user.Id);
        return TypedResults.Ok(updated.ToResponse());
    }

    public async Task<Results<Ok, NotFound>> DeleteAsync(string id)
    {
        var user = identityProvider.GetCurrentUser();

        var entity = await entityRepository.GetByIdAsync(id, user.Id);
        if (entity == null)
            return TypedResults.NotFound();

        if (!string.IsNullOrWhiteSpace(entity.ImageFileId))
        {
            await fileService.DeleteAsync(entity.ImageFileId);
        }

        await entityRepository.DeleteAsync(id, user.Id);
        return TypedResults.Ok();
    }
}
