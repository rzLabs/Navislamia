using System.Threading.Tasks;
using Navislamia.Database.Contexts;

namespace Navislamia.Game.Entities.Data.Interfaces;

public class EfRepository<T> : IEfRepository<T>
{
    private readonly ArcadiaContext _context;
    public EfRepository(ArcadiaContext context)
    {
        _context = context;
    }
    
    public Task GetAsync(int id)
    {
        _context
        throw new System.NotImplementedException();
    }

    public Task GetAllAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task CreateAsync(T entity)
    {
        throw new System.NotImplementedException();
    }

    public Task UpdateAsync(int id, T entity)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new System.NotImplementedException();
    }
}