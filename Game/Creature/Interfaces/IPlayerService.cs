using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navislamia.Game.Creature.Entities;

namespace Navislamia.Game.Creature.Interfaces
{
    public interface IPlayerService : ICreatureService
    {
        public Player CreatePlayer();

        public int GetPlayerHandle(string characterName);
    }
}
