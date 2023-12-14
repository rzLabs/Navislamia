using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly TelecasterContext _context;
    
    public CharacterRepository(TelecasterContext context)
    {
        _context = context;
    }
    
    public IEnumerable<CharacterEntity> GetCharacterListByAccountId(long accountId, bool withItems = false)
    {
        var query = _context.Characters.Where(c => c.AccountId == accountId);

        if (withItems)
        {
            query.Include(c => c.Items);
        }
        
        return query;
    }

    public IEnumerable<ItemEntity> GetCharactersItems(long accountId, long characterId)
    {
        return _context.Items.Where(c => c.AccountId == accountId && c.CharacterId == characterId);
    }


}