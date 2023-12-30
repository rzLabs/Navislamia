using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.DataAccess.Entities.Telecaster;

namespace Navislamia.Game.Services;

public interface ICharacterService
{
    Task<IEnumerable<CharacterEntity>> GetCharactersByAccountNameAsync(string accountName, bool withItems = false);

    Task<CharacterEntity> CreateCharacterAsync(CharacterEntity character, bool withStarterItems = false);

    bool CharacterExists(string characterName);

    int CharacterCount(int accountId);

    CharacterEntity GetCharacterByName(string characterName);

    Task DeleteByCharacterNameAsync(string characterName);

}