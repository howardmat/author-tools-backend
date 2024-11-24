namespace AuthorTools.Data.Repositories;

public interface IEntityRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(string userId);
    Task<T> GetByIdAsync(string id, string userId);
    Task<T> AddAsync(T entity, string userId);
    Task<T> UpdateAsync(T entity, string userId);
    Task DeleteAsync(string id, string userId);
}