using AuthorTools.Common.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface ICommonEntityService<T>
{
    Task<IEnumerable<T>> GetAllAsync(string workspaceId);
    Task<T> GetAsync(string id);
    Task<T> CreateAsync(T character);
    Task<T> UpdateAsync(string id, T character);
    Task PatchAsync(string id, IEnumerable<PatchRequest> patchRequests);
    Task DeleteAsync(string id);
}
