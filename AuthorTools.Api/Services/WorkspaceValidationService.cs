using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using MongoDB.Driver;

namespace AuthorTools.Api.Services;

public class WorkspaceValidationService(
    IWorkspaceRepository workspaceRepo,
    ICommonEntityRepository<Character> characterRepo,
    ICommonEntityRepository<Creature> creatureRepo,
    ICommonEntityRepository<Location> locationRepo)
{
    private readonly ICommonEntityRepository<Character> _characterRepo = characterRepo;
    private readonly ICommonEntityRepository<Creature> _creatureRepo = creatureRepo;
    private readonly ICommonEntityRepository<Location> _locationRepo = locationRepo;
    private readonly IWorkspaceRepository _workspaceRepo = workspaceRepo;

    public bool AnyData(string workspaceId, string userId)
    {
        return AnyCharacters(workspaceId, userId) ||
            AnyCreatures(workspaceId, userId) ||
            AnyLocations(workspaceId, userId);
    }

    public async Task<bool> IsLastAsync(string userId)
    {
        return (await _workspaceRepo.GetAllAsync(userId)).Count() == 1;
    }

    private bool AnyCharacters(string workspaceId, string userId)
    {
        return _characterRepo.Any(Builders<Character>.Filter.Where(x => workspaceId.Equals(x.WorkspaceId)), userId);
    }

    private bool AnyCreatures(string workspaceId, string userId)
    {
        return _creatureRepo.Any(Builders<Creature>.Filter.Where(x => workspaceId.Equals(x.WorkspaceId)), userId);
    }

    private bool AnyLocations(string workspaceId, string userId)
    {
        return _locationRepo.Any(Builders<Location>.Filter.Where(x => workspaceId.Equals(x.WorkspaceId)), userId);
    }
}
