using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Data.Repositories;

public class UserSettingRepository(string databaseName, string connectionString, bool forcePartitionKey, string? partitionKeyBase = null)
    : MongoDbRepository<UserSetting>(ContainerName, databaseName, connectionString, forcePartitionKey, partitionKeyBase), IRepository<UserSetting>
{
    private const string ContainerName = "user-settings";
}