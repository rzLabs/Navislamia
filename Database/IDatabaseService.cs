using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;

using Configuration;

namespace Database
{
    public interface IDatabaseService
    {
        public async Task<int> LoadRepositories() => 0;

    }
}
