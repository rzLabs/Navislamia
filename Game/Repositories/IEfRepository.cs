using System.Threading.Tasks;

namespace Navislamia.Game.Entities.Data.Interfaces;
/// <summary>
/// Base repository for EF Core interactions
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IEfRepository<in T>
{
    public Task GetAsync(int id);
    public Task GetAllAsync();
    public Task CreateAsync(T entity);
    public Task UpdateAsync(int id, T entity);
    public Task DeleteAsync(int id);
}