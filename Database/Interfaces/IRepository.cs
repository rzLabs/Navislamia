using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Interfaces
{
    public interface IRepository<T>
    {
        public string Name { get; }

        public List<T> Fetch() => new List<T>();
    }
}
