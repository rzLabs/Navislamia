using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Repositories;

public interface ICharacterRepository
{
    IEnumerable<CharacterEntity> GetCharactersByAccountName(string accountName, bool withItems = false);
    Task CreateCharacterAsync(CharacterEntity character);
    Task SaveChangesAsync();
}