using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Navislamia.Game.DataAccess.Contexts;
using Navislamia.Game.DataAccess.Repositories.Interfaces;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.DataAccess.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly DbContextOptions<TelecasterContext> _options;
    private readonly TelecasterContext _context;

    public CharacterRepository(DbContextOptions<TelecasterContext> options)
    {
        _options = options;
        _context = new TelecasterContext(options);
    }
    
    public async Task<IEnumerable<CharacterEntity>> GetCharactersByAccountNameAsync(string accountName, bool withItems = false)
    {
        var query = _context.Characters.AsNoTracking().Where(c => c.AccountName == accountName);

        if (withItems)
        {
            query = query.Include(c => c.Items);
        }
        
        return query;
    }

    public async Task CreateCharacterAsync(CharacterEntity character)
    {
        await _context.Characters.AddAsync(character);
    }

    public bool CharacterExists(string characterName)
    {
        return _context.Characters.AsNoTracking().Any(c=>c.CharacterName ==  characterName);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

}