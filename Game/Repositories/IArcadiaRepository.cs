using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.Game.Repositories;

public interface IArcadiaRepository<T>
{
    public Task<T> GetAsync(int id);
    public IEnumerable<T> GetAllAsync();
    public Task<T> CreateAsync(T entity);
    public Task<T> UpdateAsync(int id, T entity);
    public Task DeleteAsync(int id);
    public void DeleteAsync(T entity);
}