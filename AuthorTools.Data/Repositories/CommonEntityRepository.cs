using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Data.Repositories;

public class CommonEntityRepository<T>(string containerName, string databaseName, string connectionString, string partitionKeyBase)
    : MongoDbRepository<T>(containerName, databaseName, connectionString, partitionKeyBase), ICommonEntityRepository<T> where T : CommonEntity
{ }