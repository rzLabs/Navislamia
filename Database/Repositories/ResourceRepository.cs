using Navislamia.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Repositories
{
    public class ResourceRepository<T>
    {
        public readonly string Name;

        public IDbContext DbContext;

        public ResourceRepository(string name, IDbContext dbContext) 
        {
            Name = name;
            DbContext = dbContext;
        }

        public virtual T Get() => throw new NotImplementedException();
    }
}
