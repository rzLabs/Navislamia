using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Repositories;

public interface ICharacterRepository
{
    IEnumerable<CharacterEntity> GetCharacterListByAccountId(long accountId, bool withItems = false);
    IEnumerable<ItemEntity> GetCharactersItems(long accountId, long characterId);
}