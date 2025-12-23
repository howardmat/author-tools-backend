using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Data.Repositories;

public class CommonEntityRepository<T>(
    string containerName, 
    string databaseName, 
    string connectionString, 
    bool forcePartitionKey, 
    string? partitionKeyBase = null) : MongoDbRepository<T>(
        containerName, 
        databaseName, 
        connectionString, 
        forcePartitionKey, 
        partitionKeyBase), ICommonEntityRepository<T> where T : CommonEntity
{ }