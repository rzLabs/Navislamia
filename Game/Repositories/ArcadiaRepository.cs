using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Navislamia.Game.Contexts;

namespace Navislamia.Game.Repositories;

public class ArcadiaRepository<T> : IArcadiaRepository<T> where T : class
{
    private readonly ArcadiaContext _context;
    private readonly ILogger<ArcadiaRepository<T>> _logger;
    
    public ArcadiaRepository(ArcadiaContext context, ILogger<ArcadiaRepository<T>> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<T> GetAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public IEnumerable<T> GetAllAsync()
    {
        return _context.Set<T>().ToList();
    }

    public async Task<T> CreateAsync(T entity)
    {
        return (await _context.Set<T>().AddAsync(entity)).Entity;
    }

    public async Task<T> UpdateAsync(int id, T entity)
    {
        var entityToUpdate = await GetAsync(id);
        if (entityToUpdate == null)
        {
            _logger.LogWarning("Could not update entity {entityType} with ID {entityID}: Not Found", nameof(entity), id);
            return null;
        }
        
        return _context.Set<T>().Update(entity).Entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entityToDelete = await GetAsync(id);
        if (entityToDelete == null)
        {
            _logger.LogWarning("Could not delete entity with ID {entityID}: Not Found", id);
            return;
        }

        _context.Set<T>().Remove(entityToDelete);
    }
    
    public void DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}