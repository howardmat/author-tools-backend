using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;

namespace AuthorTools.Data.Repositories;

public class CharacterRepository(string databaseName, string connectionString, string partitionKeyBase)
    : MongoDbRepository<Character>(ContainerName, databaseName, connectionString, partitionKeyBase), ICharacterRepository
{
    private const string ContainerName = "characters";
}