using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Security
{
    public class TImpl : RC4Cipher { }

    public class XRC4Cipher : ICipher
    {
        public XRC4Cipher()
        {
            Impl = new TImpl();
            Clear();
        }

        public void SetKey(string key) => Impl.Init(key, key.Length);

        public void Clear() => Impl.Init("Neat & Simple", 0);

        public void Decode(byte[] source, byte[] destination, int length, bool isPeek = false)
        {
            if (isPeek)
                tryCipher(source, destination, length);
            else
                doCipher(source, destination, length);
        }

        public void Encode(byte[] source, byte[] destination, int length, bool isPeek = false)
        {
            if (isPeek)
                tryCipher(source, destination, length);
            else
                doCipher(source, destination, length);
        }

        void tryCipher(byte[] source, byte[] destination, int length) 
        {
            RC4Cipher.State backup = null;
            Impl.SaveStateTo(backup);

            doCipher(source, destination, length);

            Impl.LoadStateFrom(backup);
        }

        void doCipher(byte[] source, byte[] destination, int length) => Impl.Code(source, destination, length);

        TImpl Impl;
    }
}
