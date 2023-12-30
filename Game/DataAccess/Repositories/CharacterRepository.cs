using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Navislamia.Game.DataAccess.Contexts;
using Navislamia.Game.DataAccess.Entities.Telecaster;
using Navislamia.Game.DataAccess.Repositories.Interfaces;

namespace Navislamia.Game.DataAccess.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly TelecasterContext _context;
    public CharacterRepository(DbContextOptions<TelecasterContext> options)
    {
        _context = new TelecasterContext(options);
    }
    
    public async Task<IEnumerable<CharacterEntity>> GetCharactersByAccountNameAsync(string accountName)
    {
        return _context.Characters.Where(c => c.AccountName == accountName);
    }

    public async Task<CharacterEntity> CreateCharacterAsync(CharacterEntity character)
    {
        return (await _context.Characters.AddAsync(character)).Entity;
    }

    public CharacterEntity GetCharacterByName(string characterName)
    {
        return _context.Characters.FirstOrDefault(c => c.CharacterName == characterName);
    }

    public bool CharacterExists(string characterName)
    {
        return _context.Characters.Any(c => c.CharacterName == characterName);
    }
    
    public void Delete(CharacterEntity entity)
    {
        _context.Characters.Remove(entity);
    }

    public int CharacterCount(int accountId)
    {
        return _context.Characters.Count(c => c.AccountId == accountId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

}