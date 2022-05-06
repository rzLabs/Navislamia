using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets
{
    public static class Actions
    {
        static Dictionary<ushort, Func<ISerializablePacket, int>> actions = new Dictionary<ushort, Func<ISerializablePacket, int>>()
        {
            new KeyValuePair<ushort, Func<ISerializablePacket, int>((ushort)AuthPackets.TS_AG_LOGIN_RESULT, Foo) // TODO: just example, remove me!
        };

        public static int OnReceive(this ISerializablePacket msg)
        {
            if (!actions.ContainsKey(msg.ID))
                return 1;

            return actions[msg.ID]?.Invoke(msg) ?? 2;
        }

        public static int Foo(ISerializablePacket msg) => 0; // TODO: just example, remove me!
    }
}
