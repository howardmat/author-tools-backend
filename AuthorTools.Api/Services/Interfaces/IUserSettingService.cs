using AuthorTools.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthorTools.Api.Services.Interfaces;

public interface IUserSettingService
{
    Task<Ok<UserSettingResponse?>> GetAsync();
    Task<Results<Ok<UserSettingResponse>, BadRequest<ValidationProblemDetails>>> CreateAsync(UserSettingCreateRequest request);
    Task<Results<Ok<UserSettingResponse>, NotFound, BadRequest<ValidationProblemDetails>>> UpdateAsync(string id, UserSettingUpdateRequest request);
}
