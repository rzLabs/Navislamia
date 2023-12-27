using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Services;

public interface ICharacterService
{
    IEnumerable<CharacterEntity> GetCharactersByAccountName(string accountName, bool withItems = false);
    Task CreateCharacterAsync(CharacterEntity character, bool withStarterItems = false);

}