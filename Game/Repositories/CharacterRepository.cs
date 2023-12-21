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
    
    public IEnumerable<CharacterEntity> GetCharactersByAccountName(string accountName, bool withItems = false)
    {
        var query = _context.Characters.Where(c => c.AccountName == accountName);

        if (withItems)
        {
            query = query.Include(c => c.Items);
        }
        
        return query;
    }


}