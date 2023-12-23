using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Navislamia.Game.Models.Enums;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Services;

public interface IStarterItemsRepository
{
    IQueryable<StarterItemsEntity> GetStarterItemsByJobAsync(Job job);
}