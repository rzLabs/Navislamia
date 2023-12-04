using System.Threading.Tasks;

namespace Navislamia.Game.Entities.Data.Interfaces;

public interface IEfRepository<T>
{
    public Task GetAsync(int id);
    public Task GetAllAsync();
    public Task CreateAsync(T entity);
    public Task UpdateAsync(int id, T entity);
    public Task DeleteAsync(int id);
}