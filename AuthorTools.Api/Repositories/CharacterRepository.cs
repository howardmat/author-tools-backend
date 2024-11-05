using AuthorTools.Api.Models;
using AuthorTools.Api.Options;
using Microsoft.Extensions.Options;

namespace AuthorTools.Api.Repositories;

public class CharacterRepository : CosmosRepository<Character>, ICharacterRepository
{
    private const string CosmosContainerName = "characters";

    public CharacterRepository(IOptions<CosmosDbOptions> cosmosDbOptions)
        : base(CosmosContainerName, cosmosDbOptions)
    { }
}
