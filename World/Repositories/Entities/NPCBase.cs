using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories.Entities
{
    public class NPCBase
    {
        public int ID;
        public int TextID;
        public int NameTextID;
        public int X;
        public int Y;
        public int Face;
        public int LocalFlag;

        public bool IsPeriodic;
        public DateTime BeginOfPeriod;
        public DateTime EndOfPeriod;

        public int RoamingID;
        public int StandardWalkSpeed;
        public int StandardRunSpeed;
        public int WalkSpeed;
        public int RunSpeed;

        public int Attackable;

        public int OffensiveType;
        public int SpawnType;
        public int ChaseRange;
        public int RegenTime;
        public int Level;

        public int StatID;

        public int AttackRange;
        public int AttackSpeedType;
        public int HP;
        public int MP;
        public int AttackPoint;
        public int MagicPoint;
        public int Defence;
        public int MagicDefence;
        public int AttackSpeed;
        public int MagicSpeed;
        public int Accuracy;
        public int Avoid;
        public int MagicAccuracy;
        public int MagicAvoid;

        public string AIScript;
        public string ContactScript;

    }
}
