using System.Threading.Tasks;
using Navislamia.Game.Models.Navislamia;

namespace Navislamia.Game.Repositories;

public interface IWorldRepository
{
    public WorldEntity LoadWorldIntoMemory();
}