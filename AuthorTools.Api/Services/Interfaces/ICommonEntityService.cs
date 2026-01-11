using AuthorTools.Api.Models;
using AuthorTools.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthorTools.Api.Services.Interfaces;

public interface ICommonEntityService<T>
{
    Task<Results<Ok<IEnumerable<CommonEntityResponse>>, BadRequest>> GetAllAsync(string workspaceId);
    Task<Results<Ok<CommonEntityResponse>, NotFound>> GetAsync(string id);
    Task<Results<Ok<CommonEntityResponse>, BadRequest<ValidationProblemDetails>>> CreateAsync(CommonEntityCreateRequest request);
    Task<Results<Ok<CommonEntityResponse>, NotFound, BadRequest<ValidationProblemDetails>>> UpdateAsync(string id, CommonEntityUpdateRequest request);
    Task<Results<Ok<CommonEntityResponse>, NotFound>> PatchAsync(string id, IEnumerable<PatchRequest> patchRequests);
    Task<Results<Ok, NotFound>> DeleteAsync(string id);
}
