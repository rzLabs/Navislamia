using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.DataAccess.Repositories.Interfaces;

public interface ICharacterRepository
{
    Task<IEnumerable<CharacterEntity>> GetCharactersByAccountNameAsync(string accountName, bool withItems = false);
    Task CreateCharacterAsync(CharacterEntity character);

    bool CharacterExists(string characterName);

    Task SaveChangesAsync();
}