using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Data.Repositories;

public class WorkspaceRepository(string databaseName, string connectionString, string partitionKeyBase)
    : MongoDbRepository<Workspace>(ContainerName, databaseName, connectionString, partitionKeyBase), IWorkspaceRepository
{
    private const string ContainerName = "workspace";
}