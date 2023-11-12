using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories;

public interface IRepository
{
    public string Name { get; }

    public int Count { get; }

    public IEnumerable<T> GetData<T>();

    public async Task<IRepository> Load() => null;
}