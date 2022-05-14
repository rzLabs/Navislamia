using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game
{
    public interface IGameService
    {
        public async Task<int> Start(string ip, int port, int backlog) => 0;
    }
}
