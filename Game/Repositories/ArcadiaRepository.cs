using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.Contexts;


namespace Navislamia.Game.Repositories;

public class ArcadiaRepository<T> : IArcadiaRepository<T> where T : class
{
    private readonly ILogger<ArcadiaRepository<T>> _logger;
    private readonly DbContextOptions<ArcadiaContext> _options;
    private readonly IOptions<DatabaseOptions> _dbOptions;

    public ArcadiaRepository(ILogger<ArcadiaRepository<T>> logger, DbContextOptions<ArcadiaContext> options, 
        IOptions<DatabaseOptions> dbOptions)
    {
        _logger = logger;
        _options = options;
        _dbOptions = dbOptions;

        // navigational properties will not be queried automatically this is to improve performance
    }
    
    public async Task<T> GetAsync(int id)
    {
        await using var context = new ArcadiaContext(_options, _dbOptions);
        return await context.Set<T>().FindAsync(id);
    }

    public IEnumerable<T> GetAllAsync()
    {
        using var context = new ArcadiaContext(_options, _dbOptions);
        return context.Set<T>().ToList();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await using var context = new ArcadiaContext(_options, _dbOptions);
        return (await context.Set<T>().AddAsync(entity)).Entity;
    }

    public async Task<T> UpdateAsync(int id, T entity)
    {
        var entityToUpdate = await GetAsync(id);
        if (entityToUpdate == null)
        {
            _logger.LogWarning("Could not update entity {entityType} with ID {entityID}: Not Found", nameof(entity), id);
            return null;
        }

        await using var context = new ArcadiaContext(_options, _dbOptions);
        return context.Set<T>().Update(entity).Entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entityToDelete = await GetAsync(id);
        if (entityToDelete == null)
        {
            _logger.LogWarning("Could not delete entity with ID {entityID}: Not Found", id);
            return;
        }

        await using var context = new ArcadiaContext(_options, _dbOptions);
        context.Set<T>().Remove(entityToDelete);
    }
    
    public void DeleteAsync(T entity)
    {
        using var context = new ArcadiaContext(_options, _dbOptions);
        context.Set<T>().Remove(entity);
    }
}