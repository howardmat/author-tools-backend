using AuthorTools.Data.Models;

namespace AuthorTools.Data.Repositories;

public class CharacterRepository : CosmosRepository<Character>, ICharacterRepository
{
    public CharacterRepository(string cosmosUrl, string key, string databaseName, string containerName, string environment)
        : base(cosmosUrl, key, databaseName, containerName, environment)
    { }
}
