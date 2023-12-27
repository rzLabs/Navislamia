using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Telecaster;
using Navislamia.Game.Repositories.Interfaces;

namespace Navislamia.Game.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly DbContextOptions<TelecasterContext> _options;

    public CharacterRepository(DbContextOptions<TelecasterContext> options)
    {
        _options = options;
    }
    
    public IEnumerable<CharacterEntity> GetCharactersByAccountName(string accountName, bool withItems = false)
    {
        using var context = new TelecasterContext(_options);

        var query = context.Characters.Where(c => c.AccountName == accountName);

        if (withItems)
        {
            query = query.Include(c => c.Items);
        }
        
        return query;
    }

    public async Task CreateCharacterAsync(CharacterEntity character)
    {
        await using var context = new TelecasterContext(_options);
        await context.Characters.AddAsync(character);
    }

    public async Task SaveChangesAsync()
    {
        await using var context = new TelecasterContext(_options);
        await context.SaveChangesAsync();
    }

}