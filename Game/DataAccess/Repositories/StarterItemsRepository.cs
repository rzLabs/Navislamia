using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Navislamia.Game.DataAccess.Contexts;
using Navislamia.Game.DataAccess.Entities.Enums;
using Navislamia.Game.DataAccess.Entities.Telecaster;
using Navislamia.Game.DataAccess.Repositories.Interfaces;

namespace Navislamia.Game.DataAccess.Repositories;

public class StarterItemsRepository : IStarterItemsRepository
{
    private readonly ILogger<StarterItemsRepository> _logger;
    private readonly DbContextOptions<TelecasterContext> _options;
    private readonly TelecasterContext _context;

    public StarterItemsRepository(ILogger<StarterItemsRepository> logger, DbContextOptions<TelecasterContext> options)
    {
        _logger = logger;
        _options = options;
        _context = new TelecasterContext(options);

    }
    
    public async Task<IEnumerable<StarterItemsEntity>> GetStarterItemsByJobAsync(Job job)
    {
        return _context.StarterItems.Where(s => s.Job == job);
    }
}