using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Services;

public interface ICharacterService
{
    IEnumerable<CharacterEntity> GetCharacterListByAccountId(long accountId, bool withItems = false);
    IEnumerable<ItemEntity> GetCharactersItems(long accountId, long characterId);
}