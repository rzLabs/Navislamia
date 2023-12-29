using Navislamia.Game.Models.Navislamia;

namespace Navislamia.Game.DataAccess.Repositories.Interfaces;

public interface IWorldRepository
{
    public WorldEntity LoadWorldIntoMemory();
}