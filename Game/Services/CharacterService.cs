using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;
using Navislamia.Game.Repositories;

namespace Navislamia.Game.Services;

public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _repository;

    public CharacterService(ICharacterRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<CharacterEntity> GetCharacterListByAccountId(long accountId, bool withItems = false)
    {
        return _repository.GetCharacterListByAccountId(accountId, withItems);
    }
    
    public IEnumerable<ItemEntity> GetCharactersItems(long accountId, long characterId)
    {
        return _repository.GetCharactersItems(accountId, characterId);
    }
}