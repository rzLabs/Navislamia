using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Enums;
using Navislamia.Game.Models.Telecaster;
using Navislamia.Game.Repositories.Interfaces;

namespace Navislamia.Game.Repositories;

public class StarterItemsRepository : IStarterItemsRepository
{
    private readonly ILogger<StarterItemsRepository> _logger;
    private readonly DbContextOptions<TelecasterContext> _options;

    public StarterItemsRepository(ILogger<StarterItemsRepository> logger, DbContextOptions<TelecasterContext> options)
    {
        _logger = logger;
        _options = options;
    }
    
    public async Task<IEnumerable<StarterItemsEntity>> GetStarterItemsByJobAsync(Job job)
    {
        await using var context = new TelecasterContext(_options);

        var starterItems = context.StarterItems.Where(s => s.Job == job);
        return starterItems;
    }
}