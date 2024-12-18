﻿using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Enums;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using AuthorTools.SharedLib.Models;

namespace AuthorTools.Api.Services;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepo;
    private readonly IIdentityProvider _identityProvider;
    private readonly IFileService _fileService;

    public CharacterService(
        ICharacterRepository characterRepository,
        IIdentityProvider identityProvider,
        IFileService fileService)
    {
        _characterRepo = characterRepository;
        _identityProvider = identityProvider;
        _fileService = fileService;
    }

    public async Task<IEnumerable<Character>> GetAllAsync()
    {
        var user = _identityProvider.GetCurrentUser();
        return await _characterRepo.GetAllAsync<Character>(user.Id, SortOrder.Ascending);
    }

    public async Task<Character> GetAsync(string id)
    {
        var user = _identityProvider.GetCurrentUser();
        return await _characterRepo.GetByIdAsync(id, user.Id);
    }

    public async Task<Character> CreateAsync(Character character)
    {
        var user = _identityProvider.GetCurrentUser();

        character.Owner = user;

        return await _characterRepo.CreateAsync(character, user.Id);
    }

    public async Task<Character> UpdateAsync(string id, Character character)
    {
        var user = _identityProvider.GetCurrentUser();

        character.Id = id;
        character.Owner = user;

        return await _characterRepo.UpdateAsync(character, user.Id);
    }

    public async Task PatchAsync(string id, IEnumerable<PatchRequest> patchRequests)
    {
        var user = _identityProvider.GetCurrentUser();

        await _characterRepo.PatchAsync(id, patchRequests, user.Id);
    }

    public async Task DeleteAsync(string id)
    {
        var user = _identityProvider.GetCurrentUser();

        var character = await GetAsync(id);
        if (!string.IsNullOrWhiteSpace(character.ImageFileId))
        {
            await _fileService.DeleteAsync(character.ImageFileId);
        }

        await _characterRepo.DeleteAsync(id, user.Id);
    }
}
