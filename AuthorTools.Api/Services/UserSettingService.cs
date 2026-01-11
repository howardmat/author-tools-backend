using AuthorTools.Api.Mappers;
using AuthorTools.Api.Models;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Api.Validators;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthorTools.Api.Services;

public class UserSettingService(
    IIdentityProvider identityProvider,
    IRepository<UserSetting> repository,
    IValidator<UserSettingCreateRequest> createValidator,
    IValidator<UserSettingUpdateRequest> updateValidator) : IUserSettingService
{
    public async Task<Ok<UserSettingResponse?>> GetAsync()
    {
        var user = identityProvider.GetCurrentUser();
        var entity = (await repository.GetAllAsync(user.Id)).FirstOrDefault();
        return TypedResults.Ok<UserSettingResponse?>(entity?.ToResponse());
    }

    public async Task<Results<Ok<UserSettingResponse>, BadRequest<ValidationProblemDetails>>> CreateAsync(UserSettingCreateRequest request)
    {
        var validationResult = createValidator.Validate(request);
        if (!validationResult.IsValid)
            return validationResult.Errors.ToBadRequest();

        var user = identityProvider.GetCurrentUser();

        var entity = request.ToEntity(user);
        entity.Owner = user;

        var created = await repository.CreateAsync(entity, user.Id);
        return TypedResults.Ok(created.ToResponse());
    }

    public async Task<Results<Ok<UserSettingResponse>, NotFound, BadRequest<ValidationProblemDetails>>> UpdateAsync(string id, UserSettingUpdateRequest request)
    {
        var validationResult = updateValidator.Validate(request);
        if (!validationResult.IsValid)
            return validationResult.Errors.ToBadRequest();

        var user = identityProvider.GetCurrentUser();

        var existingEntity = await repository.GetByIdAsync(id, user.Id);
        if (existingEntity == null)
            return TypedResults.NotFound();

        var entity = request.ToEntity(id, user);
        entity.Id = id;
        entity.Owner = user;

        var updated = await repository.UpdateAsync(entity, user.Id);
        return TypedResults.Ok(updated.ToResponse());
    }
}
