using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navislamia.Game.Repositories;

public interface IBaseContextRepository<U>
{
    Task<U> GetAsync(long id);
    IEnumerable<U> GetAllAsync();
    Task<U> CreateAsync(U entity);
    Task<U> UpdateAsync(long id, U entity);
    Task DeleteAsync(long id);
    void DeleteAsync(U entity);
}