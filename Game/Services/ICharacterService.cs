using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Services;

public interface ICharacterService
{
    Task<IEnumerable<CharacterEntity>> GetCharactersByAccountNameAsync(string accountName, bool withItems = false);

    Task CreateCharacterAsync(CharacterEntity character, bool withStarterItems = false);

    bool CharacterExists(string characterName);

}