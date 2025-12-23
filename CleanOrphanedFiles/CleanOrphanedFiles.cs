using AuthorTools.Api.Services;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CleanOrphanedFiles;

public class CleanOrphanedFiles(
    ILoggerFactory loggerFactory,
    AzureBlobService azureBlobService,
    ICommonEntityRepository<Character> characterRepository,
    ICommonEntityRepository<Location> locationRepository,
    ICommonEntityRepository<Creature> creatureRepository)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<CleanOrphanedFiles>();
    private readonly AzureBlobService _azureBlobService = azureBlobService;
    private readonly ICommonEntityRepository<Character> _characterRepository = characterRepository;
    private readonly ICommonEntityRepository<Location> _locationRepository = locationRepository;
    private readonly ICommonEntityRepository<Creature> _creatureRepository = creatureRepository;

    [Function("CleanOrphanedFiles")]
    public async Task RunAsync([TimerTrigger("0 0 2 * * *", RunOnStartup = true)] TimerInfo myTimer)
    {
        _logger.LogInformation("CleanOrphanedFiles Timer trigger function executed at: {executionTime}", DateTime.Now);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }

        var characters = await _characterRepository.GetAllAsync();
        var locations = await _locationRepository.GetAllAsync();
        var creatures = await _creatureRepository.GetAllAsync();

        var imageFileIds = characters.Select(c => c.ImageFileId).ToHashSet()
            .Union(locations.Select(l => l.ImageFileId).ToHashSet())
            .Union(creatures.Select(cr => cr.ImageFileId).ToHashSet());

        var blobFileIds = await _azureBlobService.GetAllFileIdsAsync();
        foreach (var fileId in blobFileIds)
        {
            if (!imageFileIds.Contains(fileId))
            {
                _logger.LogInformation("Deleting orphaned file with ID: {fileId}", fileId);
                await _azureBlobService.DeleteBlobAsync(fileId);
            }
        }

        _logger.LogInformation("CleanOrphanedFiles Timer trigger function completed at: {completedTime}", DateTime.Now);
    }
}