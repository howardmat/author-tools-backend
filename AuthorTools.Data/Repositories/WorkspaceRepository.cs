using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Data.Repositories;

public class WorkspaceRepository(string databaseName, string connectionString, bool forcePartitionKey, string? partitionKeyBase = null)
    : MongoDbRepository<Workspace>(ContainerName, databaseName, connectionString, forcePartitionKey, partitionKeyBase), IWorkspaceRepository
{
    private const string ContainerName = "workspace";
}