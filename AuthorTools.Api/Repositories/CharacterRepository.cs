using AuthorTools.Api.Models;
using AuthorTools.Api.Options;
using Microsoft.Extensions.Options;

namespace AuthorTools.Api.Repositories;

public class CharacterRepository : CosmosRepository<Character>, ICharacterRepository
{
    public CharacterRepository(IOptions<CharacterDbOptions> cosmosDbOptions, IOptions<ApplicationOptions> appOptions)
        : base(cosmosDbOptions, appOptions)
    { }
}
