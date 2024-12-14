using AuthorTools.Data.Models;
using AuthorTools.SharedLib.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace AuthorTools.Data.Repositories;

public abstract class MongoDbRepository<T> : IRepository<T> where T : BaseMongoModel
{
    private readonly string _partitionKeyBase;
    private readonly IMongoCollection<T> _collection;

    public MongoDbRepository(
        string collectionName,
        string databaseName,
        string connectionString,
        string partitionKeyBase)
    {
        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseName);

        _collection = mongoDatabase.GetCollection<T>(collectionName);

        _partitionKeyBase = partitionKeyBase;
    }

    public async Task<IEnumerable<T>> GetAllAsync(string partitionKeyValue)
    {
        return await _collection.Find(x => x.PartitionKey == GetPartitionKey(partitionKeyValue))
            .ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync<TOrderable>(string partitionKeyValue, SortOrder sortOrder) where TOrderable : T, ISortableModel
    {
        var query = _collection.Find(x => x.PartitionKey == GetPartitionKey(partitionKeyValue));
        if (typeof(TOrderable) == typeof(T))
        {
            if (sortOrder == SortOrder.Ascending)
                query = query.SortBy(x => ((TOrderable)x).Order);
            else
                query = query.SortByDescending(x => ((TOrderable)x).Order);
        }

        return await query.ToListAsync();
    }

    public async Task<T> GetByIdAsync(string id, string partitionKeyValue)
    {
        return await _collection.Find(x => x.Id == id && x.PartitionKey == GetPartitionKey(partitionKeyValue))
               .FirstOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity, string partitionKeyValue)
    {
        var now = DateTimeOffset.UtcNow;

        entity.Id = ObjectId.GenerateNewId().ToString();
        entity.PartitionKey = GetPartitionKey(partitionKeyValue);
        entity.CreatedDateTime = now;
        entity.UpdatedDateTime = now;

        await _collection.InsertOneAsync(entity);

        return entity;
    }

    public async Task<T> UpdateAsync(T entity, string partitionKeyValue)
    {
        if (string.IsNullOrWhiteSpace(entity.Id)) throw new ArgumentNullException(nameof(entity));

        var existingEntity = await GetByIdAsync(entity.Id, partitionKeyValue) ?? throw new ArgumentNullException(nameof(entity));

        entity.PartitionKey = existingEntity.PartitionKey;
        entity.CreatedDateTime = existingEntity.CreatedDateTime;
        entity.UpdatedDateTime = DateTimeOffset.UtcNow;

        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);

        return entity;
    }

    public async Task PatchAsync(string id, IEnumerable<PatchRequest> operations, string partitionKeyValue)
    {
        var filter = Builders<T>.Filter.Eq(c => c.Id, id);
        filter &= Builders<T>.Filter.Eq(c => c.PartitionKey, GetPartitionKey(partitionKeyValue));

        UpdateDefinition<T>? update = null;
        foreach (var operation in operations)
        {
            if (int.TryParse(operation.Value?.ToString(), out var intValue))
            {
                if (update == null)
                {
                    var builder = Builders<T>.Update;
                    update = builder.Set(operation.Path, intValue);
                }
                else
                {
                    update = update.Set(operation.Path, intValue);
                }
            }
            else
            {
                throw new NotSupportedException($"Type of Value is not supported");
            }
        }

        update = update.Set("UpdatedDateTime", DateTimeOffset.UtcNow);

        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(string id, string partitionKeyValue)
    {
        await _collection.DeleteOneAsync(x => x.Id == id && x.PartitionKey == GetPartitionKey(partitionKeyValue));
    }

    private string GetPartitionKey(string value) => $"{_partitionKeyBase}_{value}";
}