using System.Collections.Generic;
using System.Threading.Tasks;
using Navislamia.Game.Models.Enums;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Repositories.Interfaces;

public interface IStarterItemsRepository
{
    Task<IEnumerable<StarterItemsEntity>> GetStarterItemsByJobAsync(Job job);
}