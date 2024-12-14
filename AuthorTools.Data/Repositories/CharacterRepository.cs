using AuthorTools.Data.Models;

namespace AuthorTools.Data.Repositories;

public class CharacterRepository : MongoDbRepository<Character>, ICharacterRepository
{
    private const string ContainerName = "characters";

    public CharacterRepository(string databaseName, string connectionString, string partitionKeyBase)
        : base(ContainerName, databaseName, connectionString, partitionKeyBase)
    { }
}