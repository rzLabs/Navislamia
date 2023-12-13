using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Repositories;

public class ArcadiaRepository<T> : IArcadiaRepository<T> where T : class
{
    private readonly ArcadiaContext _context;
    private readonly ILogger<ArcadiaRepository<T>> _logger;
    private readonly DbContextOptions<ArcadiaContext> _options;
    
    public ArcadiaRepository(ArcadiaContext context, ILogger<ArcadiaRepository<T>> logger, DbContextOptions<ArcadiaContext> options)
    {
        _context = context;
        _logger = logger;
        _options = options;

        // navigational properties will not be queried automatically this is to improve performance
        _context.ChangeTracker.LazyLoadingEnabled = false;
    }
    
    public async Task<T> GetAsync(int id)
    {
        await using (ArcadiaContext context = new ArcadiaContext(_options))
        {
            return await context.Set<T>().FindAsync(id);
        }
    }

    public IEnumerable<T> GetAllAsync()
    {
        using (ArcadiaContext context = new ArcadiaContext(_options))
        {
            return context.Set<T>().ToList();
        }
    }

    public async Task<T> CreateAsync(T entity)
    {
        await using (ArcadiaContext context = new ArcadiaContext(_options))
        {
            return (await context.Set<T>().AddAsync(entity)).Entity;
        }
    }

    public async Task<T> UpdateAsync(int id, T entity)
    {
        var entityToUpdate = await GetAsync(id);
        if (entityToUpdate == null)
        {
            _logger.LogWarning("Could not update entity {entityType} with ID {entityID}: Not Found", nameof(entity), id);
            return null;
        }

        await using (ArcadiaContext context = new ArcadiaContext(_options))
        {
            return context.Set<T>().Update(entity).Entity;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var entityToDelete = await GetAsync(id);
        if (entityToDelete == null)
        {
            _logger.LogWarning("Could not delete entity with ID {entityID}: Not Found", id);
            return;
        }

        await using (ArcadiaContext context = new ArcadiaContext(_options))
        {
            context.Set<T>().Remove(entityToDelete);
        }

    }
    
    public void DeleteAsync(T entity)
    {
        using (ArcadiaContext context = new ArcadiaContext(_options))
        {
            context.Set<T>().Remove(entity);
        }
    }
}