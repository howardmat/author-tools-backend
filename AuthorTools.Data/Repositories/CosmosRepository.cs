using AuthorTools.Data.Models;
using Microsoft.Azure.Cosmos;

namespace AuthorTools.Data.Repositories;

public abstract class CosmosRepository<T> : IEntityRepository<T> where T : BaseCosmosModel
{
    private readonly Container _container;
    private readonly CosmosClient _client;
    private readonly string _partitionKeyBase;

    public CosmosRepository(string cosmosUrl, string key, string databaseName, string containerName, string partitionKeyBase)
    {
        _client = new CosmosClient(cosmosUrl, key, new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });

        _container = _client.GetContainer(databaseName, containerName);

        _partitionKeyBase = partitionKeyBase;
    }

    public async Task<IEnumerable<T>> GetAllAsync(string userId)
    {
        var sqlCosmosQuery = "SELECT * FROM c";
        var query = _container.GetItemQueryIterator<T>(
            new QueryDefinition(sqlCosmosQuery),
            continuationToken: null,
            new QueryRequestOptions { PartitionKey = new PartitionKey(GetPartitionKey(userId)) });

        var result = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            result.AddRange(response);
        }

        return result;
    }

    public async Task<T> GetByIdAsync(string id, string userId)
    {
        var sqlCosmosQuery = "SELECT * FROM c WHERE c.id = @id";
        var query = _container.GetItemQueryIterator<T>(
            new QueryDefinition(sqlCosmosQuery)
                .WithParameter("@id", id),
            continuationToken: null,
            new QueryRequestOptions { PartitionKey = new PartitionKey(GetPartitionKey(userId)) });

        var result = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            result.AddRange(response);
        }
        return result.Single();
    }

    public async Task<T> AddAsync(T entity, string userId)
    {
        var now = DateTimeOffset.UtcNow;

        entity.PartitionKey = GetPartitionKey(userId);
        entity.CreatedDateTime = now;
        entity.UpdatedDateTime = now;

        return await _container.CreateItemAsync(entity, new PartitionKey(entity.PartitionKey));
    }

    public async Task<T> UpdateAsync(T entity, string userId)
    {
        if (string.IsNullOrWhiteSpace(entity.Id)) throw new ArgumentNullException(nameof(entity));

        var existingEntity = await GetByIdAsync(entity.Id, userId) ?? throw new ArgumentNullException(nameof(entity));

        entity.PartitionKey = existingEntity.PartitionKey;
        entity.CreatedDateTime = existingEntity.CreatedDateTime;
        entity.UpdatedDateTime = DateTimeOffset.UtcNow;

        return await _container.UpsertItemAsync(entity, new PartitionKey(entity.PartitionKey));
    }

    public async Task DeleteAsync(string id, string userId) =>
        await _container.DeleteItemAsync<T>(id, new PartitionKey(GetPartitionKey(userId)));

    private string GetPartitionKey(string value) => $"{_partitionKeyBase}_{value}";
}