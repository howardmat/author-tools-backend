using AuthorTools.Data.Enums;
using AuthorTools.Data.Models.Interfaces;
using AuthorTools.SharedLib.Models;

namespace AuthorTools.Data.Repositories.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(string partitionKeyValue);
    Task<IEnumerable<T>> GetAllAsync<TOrderable>(string partitionKeyValue, SortOrder sortOrder) where TOrderable : T, ISortableModel;
    Task<T> GetByIdAsync(string id, string partitionKeyValue);
    Task<T> CreateAsync(T entity, string partitionKeyValue);
    Task<T> UpdateAsync(T entity, string partitionKeyValue);
    Task PatchAsync(string id, IEnumerable<PatchRequest> operations, string partitionKeyValue);
    Task DeleteAsync(string id, string partitionKeyValue);
}