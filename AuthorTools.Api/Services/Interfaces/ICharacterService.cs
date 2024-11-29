using AuthorTools.Data.Models;
using AuthorTools.SharedLib.Models;

namespace AuthorTools.Api.Services.Interfaces;

public interface ICharacterService
{
    Task<IEnumerable<Character>> GetAllAsync();
    Task<Character> GetAsync(string id);
    Task<Character> CreateAsync(Character character);
    Task<Character> UpdateAsync(string id, Character character);
    Task<Character> PatchAsync(string id, IEnumerable<PatchRequest> patchRequests);
    Task DeleteAsync(string id);
}
