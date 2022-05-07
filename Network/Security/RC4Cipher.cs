using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Security
{
    public class RC4Cipher
    {
        public bool Init(string key, int keyLen = 0) => prepareKey(state, key, 0); // Can be 0 because prepareKey will automatically generate keyLen from key.Length if length == 0

        public void Code(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void Encode(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void Decode(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void SaveStateTo(out State outState) => outState = state;

        public void LoadStateFrom(State aState) => state = aState;

        static bool prepareKey(State mState, string key, int keyLen)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (keyLen == 0)
                keyLen = key.Length;

            int i, j = 0;

            for (i = 0; i < 256; i++)
                state.S[i] = Convert.ToByte(i);

            byte[] _key = new byte[256];

            j = 0;
            
            for (i = 0; i < 256; i++)
            {
                _key[i] = Convert.ToByte(key[j++]);

                if (j >= keyLen)
                    j = 0;
            }

            j = 0;
            for (i = 0; i < 256; i++)
            {
                j += state.S[j] + _key[j];
                j &= 0xff;

                var swappedBytes = swapByte(state.S[i], state.S[j]);

                state.S[i] = swappedBytes.Item1;
                state.S[j] = swappedBytes.Item2;
            }

            state.X = 0;
            state.Y = 0;
      
            skipFor(state, 1013);

            return true;
        }

        static void codeBlock(State state, byte[] source, byte[] destination, int len)
        {
            int x = state.X, y = state.Y;
            int d = 0, s = 0;

            while (len-- > 0)
            {
                ++x;
                x &= 0xff;
                int sx = state.S[x];

                y += sx;
                y &= 0xff;
                int sy = state.S[y];

                state.S[x] = (byte)sy;
                state.S[y] = (byte)sx;

                destination[d++] = Convert.ToByte(source[s++] ^ state.S[(sx + sy) & 0xff]);
            }

            state.X = x;
            state.Y = y;
        }

        static void skipFor(State state, int len)
        {
            int x = state.X, y = state.Y;

            while (len-- > 0)
            {
                ++x;
                x &= 0xff;
                int sx = state.S[x];

                y += sx;
                y &= 0xff;
                int sy = state.S[y];

                state.S[x] = (byte)sy;
                state.S[y] = (byte)sx;
            }

            state.X = x;
            state.Y = y;
        }

        static Tuple<byte, byte> swapByte(byte a, byte b)
        {
            byte t = a;
            a = b;
            b = t;

            return new Tuple<byte, byte>(a, b);
        }

        static State state = new State();

        public class State
        {
            public int X, Y;
            public byte[] S = new byte[256];
        }
    }
}
