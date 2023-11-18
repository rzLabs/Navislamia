using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Interfaces
{
    public interface IDbContext
    {
        public IDbConnection CreateConnection();
    }
}
