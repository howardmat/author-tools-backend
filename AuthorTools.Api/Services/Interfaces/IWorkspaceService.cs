using AuthorTools.Data.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface IWorkspaceService
{
    Task<IEnumerable<Workspace>> GetAllAsync();
    Task<Workspace> CreateAsync(Workspace workspace);
    Task<Workspace> UpdateAsync(string id, Workspace workspace);
}
