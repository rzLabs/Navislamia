using System.Linq;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models.Enums;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Services;

public class StarterItemsRepository : IStarterItemsRepository
{
    private readonly TelecasterContext _context;

    public StarterItemsRepository(TelecasterContext context)
    {
        _context = context;
    }
    
    public IQueryable<StarterItemsEntity> GetStarterItemsByJobAsync(Job job)
    {
        var starterItems = _context.StarterItems.Where(s => s.Job == job);
        return starterItems;
    }
}