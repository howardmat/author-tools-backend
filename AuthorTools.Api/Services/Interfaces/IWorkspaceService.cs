using AuthorTools.Api.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface IWorkspaceService
{
    Task<IEnumerable<WorkspaceResponse>> GetAllAsync();
    Task<WorkspaceResponse> GetAsync(string id);
    Task<WorkspaceResponse> CreateAsync(WorkspaceCreateRequest request);
    Task<WorkspaceResponse> UpdateAsync(string id, WorkspaceUpdateRequest request);
    Task<ServiceResult> DeleteAsync(string id);
}
