using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;
using Navislamia.Game.Repositories;

namespace Navislamia.Game.Services;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IStarterItemsRepository _starterItemsRepository;

    public CharacterService(IStarterItemsRepository starterItemsRepository, ICharacterRepository characterRepository)
    {
        _starterItemsRepository = starterItemsRepository;
        _characterRepository = characterRepository;
    }

    public IEnumerable<CharacterEntity> GetCharactersByAccountName(string accountName, bool withItems = false)
    {
        return _characterRepository.GetCharactersByAccountName(accountName, withItems);
    }

    public async Task CreateCharacter(CharacterEntity character, bool withStarterItems = false)
    {
        if (withStarterItems)
        {
            var starterItems = _starterItemsRepository.GetStarterItemsByJobAsync(character.CurrentJob);
            foreach (var starterItem in starterItems)
            {
                character.Items.Add(new ItemEntity
                {
                    ItemResourceId = starterItem.ResourceId,
                    Level = starterItem.Level,
                    Enhance = starterItem.Enhancement,
                    Amount = starterItem.Amount,
                    RemainingTime = starterItem.ValidForSeconds
                });
            }
            character.Items = new List<ItemEntity>();
        }
        await _characterRepository.CreateCharacterAsync(character);
    }
}