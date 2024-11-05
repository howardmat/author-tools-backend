using AuthorTools.Api.Models;
using AuthorTools.Api.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AuthorTools.Api.Repositories;

public abstract class CosmosRepository<T> : IEntityRepository<T> where T : BaseCosmosModel
{
    private readonly CosmosDbOptions _cosmosDbOptions;
    private readonly Container _container;
    private readonly CosmosClient _client;

    public CosmosRepository(IOptions<CosmosDbOptions> cosmosDbOptions)
    {
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

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var sqlCosmosQuery = "Select * from c";
        var query = _container.GetItemQueryIterator<T>(new QueryDefinition(sqlCosmosQuery));

        var result = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            result.AddRange(response);
        }

        return result;
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var sqlCosmosQuery = "Select * from c where c.id = @id";
        var query = _container.GetItemQueryIterator<T>(
            new QueryDefinition(sqlCosmosQuery)
                .WithParameter("@id", id));

        var result = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            result.AddRange(response);
        }
        return result.Single();
    }

    public async Task<T> AddAsync(T entity) => await _container.CreateItemAsync(entity, new PartitionKey(entity.PartitionKey));

    public async Task<T> UpdateAsync(T entity) => await _container.UpsertItemAsync(entity, new PartitionKey(entity.PartitionKey));

    public async Task DeleteAsync(string id, string partitionKey) => await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
}