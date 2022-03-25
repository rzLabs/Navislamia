using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Security
{
    public interface ICipher
    {
        void Encode(byte[] source, byte[] destination, int length, bool isPeek = false);

        void Decode(byte[] source, byte[] destination, int length, bool isPeek = false);

        void Clear();
    }
}
