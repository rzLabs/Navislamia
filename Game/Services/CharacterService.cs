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

    public IEnumerable<CharacterEntity> GetCharactersByAccountName(string accountName, bool withItems = false)
    {
        return _repository.GetCharactersByAccountName(accountName, withItems);
    }
}