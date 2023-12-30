using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Navislamia.Game.DataAccess.Entities.Enums;
using Navislamia.Game.DataAccess.Entities.Telecaster;
using Navislamia.Game.DataAccess.Repositories.Interfaces;

using Serilog;

namespace Navislamia.Game.Services;

public class CharacterService : ICharacterService
{
    private readonly ILogger<CharacterService> _logger;
    private readonly ICharacterRepository _characterRepository;
    private readonly IStarterItemsRepository _starterItemsRepository;

    public CharacterService(IStarterItemsRepository starterItemsRepository, ICharacterRepository characterRepository, ILogger<CharacterService> logger)
    {
        _starterItemsRepository = starterItemsRepository;
        _characterRepository = characterRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CharacterEntity>> GetCharactersByAccountNameAsync(string accountName, bool withItems = false)
    {
        return await _characterRepository.GetCharactersByAccountNameAsync(accountName, withItems);
    }

    public async Task<CharacterEntity> CreateCharacterAsync(CharacterEntity character, bool withStarterItems = false)
    {
        if (withStarterItems)
        {
            character.Items ??= new List<ItemEntity>();
            
            var starterItems = await _starterItemsRepository.GetStarterItemsByJobAsync((Race)character.Race);
            foreach (var starterItem in starterItems)
            {
                character.Items.Add(new ItemEntity
                {
                    ItemResourceId = starterItem.ItemId,
                    Level = starterItem.Level,
                    Enhance = starterItem.Enhancement,
                    Amount = starterItem.Amount,
                    RemainingTime = starterItem.ValidForSeconds
                });
            }
        }
        
        var result = await _characterRepository.CreateCharacterAsync(character);
        await _characterRepository.SaveChangesAsync();
        
        return result;
    }

    public bool CharacterExists(string characterName)
    {
        return _characterRepository.CharacterExists(characterName);
    }

    public int CharacterCount(int accountId)
    {
        return _characterRepository.CharacterCount(accountId);
    }

    public CharacterEntity GetCharacterByName(string characterName)
    {
        return _characterRepository.GetCharacterByName(characterName);
    }

    public async Task DeleteCharacterByNameAsync(string characterName)
    {
        var entity = _characterRepository.GetCharacterByName(characterName);
        if (entity is null)
        {
            _logger.LogWarning("Character Delete Failed! Character {name} not found!", characterName);
            return;
        }

        _characterRepository.Delete(entity);
    }

    public async void SaveChanges()
    {
        await _characterRepository.SaveChangesAsync();
    }
}