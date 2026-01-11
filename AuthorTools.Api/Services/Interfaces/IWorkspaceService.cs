using AuthorTools.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthorTools.Api.Services.Interfaces;

public interface IWorkspaceService
{
    Task<Ok<IEnumerable<WorkspaceResponse>>> GetAllAsync();
    Task<Results<Ok<WorkspaceResponse>, NotFound>> GetAsync(string id);
    Task<Results<Ok<WorkspaceResponse>, BadRequest<ValidationProblemDetails>>> CreateAsync(WorkspaceCreateRequest request);
    Task<Results<Ok<WorkspaceResponse>, NotFound, BadRequest<ValidationProblemDetails>>> UpdateAsync(string id, WorkspaceUpdateRequest request);
    Task<Results<Ok, NotFound, Conflict<ProblemDetails>>> DeleteAsync(string id);
}
