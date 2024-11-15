using AuthorTools.Api.Models;
using AuthorTools.Api.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AuthorTools.Api.Repositories;

public abstract class CosmosRepository<T> : IEntityRepository<T> where T : BaseCosmosModel
{
    private readonly ApplicationOptions _appOptions;
    private readonly CosmosDbOptions _cosmosDbOptions;
    private readonly Container _container;
    private readonly CosmosClient _client;

    public CosmosRepository(IOptions<CosmosDbOptions> cosmosDbOptions,
        IOptions<ApplicationOptions> appOptions)
    {
        _appOptions = appOptions.Value;
        _cosmosDbOptions = cosmosDbOptions.Value;

        _client = new CosmosClient(_cosmosDbOptions.Url, _cosmosDbOptions.PrimaryKey, new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });

        _container = _client.GetContainer(_cosmosDbOptions.DatabaseName, _cosmosDbOptions.ContainerName);
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
        entity.PartitionKey = GetPartitionKey(userId);
        return await _container.CreateItemAsync(entity, new PartitionKey(entity.PartitionKey));
    }

    public async Task<T> UpdateAsync(T entity, string userId)
    {
        entity.PartitionKey = GetPartitionKey(userId);
        return await _container.UpsertItemAsync(entity, new PartitionKey(entity.PartitionKey));
    }

    public async Task DeleteAsync(string id, string userId) =>
        await _container.DeleteItemAsync<T>(id, new PartitionKey(GetPartitionKey(userId)));

    private string GetPartitionKey(string value) => $"{_appOptions.Environment}_{value}";
}