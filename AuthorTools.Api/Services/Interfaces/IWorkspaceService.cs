using AuthorTools.Api.Models;
using AuthorTools.Data.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface IWorkspaceService
{
    Task<IEnumerable<Workspace>> GetAllAsync();
    Task<Workspace> GetAsync(string id);
    Task<Workspace> CreateAsync(Workspace workspace);
    Task<Workspace> UpdateAsync(string id, Workspace workspace);
    Task<ServiceResult> DeleteAsync(string id);
}
