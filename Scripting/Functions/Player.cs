using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Scripting.Functions
{
    public class Player
    {
        public static int get_value(params object[] args)
        {
            string playerName;

            if (args.Length < 2)
                return 1;

            if (args.Length == 3)
                playerName = args[2].ToString();

            string key = args[0].ToString();

            // return pPlayer.GetValue();

            // pPlayer.SetValue()


            return 0;
        }
    }
}
