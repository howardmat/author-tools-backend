using AuthorTools.Data.Enums;
using AuthorTools.Data.Models.Interfaces;
using AuthorTools.Common.Models;
using MongoDB.Driver;

namespace AuthorTools.Data.Repositories.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(string partitionKeyValue);
    Task<IEnumerable<T>> GetAllAsync<TOrderable>(string partitionKeyValue, string workspaceId, SortOrder sortOrder) where TOrderable : T, ISortableModel, IWorkspaceModel;
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(string id, string partitionKeyValue);
    Task<T> CreateAsync(T entity, string partitionKeyValue);
    Task<T> UpdateAsync(T entity, string partitionKeyValue);
    Task PatchAsync(string id, IEnumerable<PatchRequest> operations, string partitionKeyValue);
    Task DeleteAsync(string id, string partitionKeyValue);
    bool Any(FilterDefinition<T> filterDefinition, string partitionKeyValue);
}