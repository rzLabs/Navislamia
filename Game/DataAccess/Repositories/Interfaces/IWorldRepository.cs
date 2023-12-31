using Navislamia.Game.DataAccess.Entities.Navislamia;

namespace Navislamia.Game.DataAccess.Repositories.Interfaces;

public interface IWorldRepository
{
    public WorldEntity LoadWorldIntoMemory();
}