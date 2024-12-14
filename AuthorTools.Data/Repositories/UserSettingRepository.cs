using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Data.Repositories;

public class UserSettingRepository(string databaseName, string connectionString, string partitionKeyBase)
    : MongoDbRepository<UserSetting>(ContainerName, databaseName, connectionString, partitionKeyBase), IUserSettingRepository
{
    private const string ContainerName = "user-settings";
}