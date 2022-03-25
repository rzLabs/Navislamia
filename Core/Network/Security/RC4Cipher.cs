using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Security
{
    public class RC4Cipher
    {
        public bool Init(string key, int keyLen = 0) => prepareKey(state, key, keyLen);

        public void Code(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void Encode(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void Decode(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void SaveStateTo(State outState) => outState = state;

        public void LoadStateFrom(State aState) => state = aState;

        static bool prepareKey(State mState, string key, int keyLen)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (keyLen == 0)
                keyLen = key.Length;

            int i, j;

            for (i = 0; i < 256; i++)
                state.S[i] = (byte)i;

            byte[] bkey = new byte[256];

            j = 0;

            for (i = 0; i < 256; i++)
            {
                bkey[i] = (byte)key[j++];

                if (j >= keyLen)
                    j = 0;
            }

            j = 0;

            for (i = 0; i < 256; i++)
            {
                j += state.S[j] + bkey[j];
                j &= 0xff;

                swapByte(state.S[i], state.S[j]);
            }

            state.X = state.Y = 0;
            skipFor(state, 1013);

            return true;
        }

        static void codeBlock(State state, byte[] source, byte[] destination, int len)
        {
            int x = state.X, y = state.Y;
            int sIdx = 0;
            int dIdx = 0;

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

                destination[dIdx++] = (byte)(source[sIdx++] ^ state.S[sx + sy & 0xff]);
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

                state.S[x] = state.S[y];
                state.S[y] = (byte)sx;
            }

            state.X = x;
            state.Y = y;
        }

        static void swapByte(byte a, byte b)
        {
            byte t = a;
            a = b;
            b = t;
        }

        static State state = new State();

        public class State
        {
            public int X, Y;
            public byte[] S = new byte[256];
        }
    }
}
