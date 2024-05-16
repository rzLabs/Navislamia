using Microsoft.Extensions.Logging;
using Navislamia.Game.Creature.Entities;
using Navislamia.Game.Creature.Interfaces;
using Navislamia.Game.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Creature.Services
{
    public class PlayerService : CreatureService, IPlayerService
    {
        private readonly ILogger<PlayerService> _logger;

        private readonly HandleUtility _handleUtility = new HandleUtility();

        private Dictionary<string, int> _playerList = new Dictionary<string, int>();

        public Player CreatePlayer()
        {
            var player = new Player();
            player.Handle = _handleUtility.GenerateHandle();

            return player;
        }

        public int GetPlayerHandle(string characterName)
        {
            if (_playerList.ContainsKey(characterName))
            {
                return _playerList[characterName];
            }

            return -1;
        }
    }
}
