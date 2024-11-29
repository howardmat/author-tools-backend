using Microsoft.Azure.Cosmos;

namespace AuthorTools.Data.Repositories;

public interface IEntityRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(string partitionKeyValue, string? orderBy = null);
    Task<T> GetByIdAsync(string id, string partitionKeyValue);
    Task<T> AddAsync(T entity, string partitionKeyValue);
    Task<T> UpdateAsync(T entity, string partitionKeyValue);
    Task<T> PatchAsync(string id, List<PatchOperation> patchOperations, string partitionKeyValue);
    Task DeleteAsync(string id, string partitionKeyValue);
}