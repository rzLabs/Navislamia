using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.DataAccess.Entities.Enums;
using Navislamia.Game.DataAccess.Entities.Telecaster;

namespace Navislamia.Game.DataAccess.Repositories.Interfaces;

public interface IStarterItemsRepository
{
    Task<IEnumerable<StarterItemsEntity>> GetStarterItemsByJobAsync(Race race);
}