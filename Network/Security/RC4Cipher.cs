using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Security
{
    public class RC4Cipher
    {
        public bool Init(string key, int keyLen = 0) => prepareKey(state, key, keyLen); // Can be 0 because prepareKey will automatically generate keyLen from key.Length if length == 0

        public void Code(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void Encode(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public void Decode(byte[] source, byte[] destination, int length) => codeBlock(state, source, destination, length);

        public State GetState() => state;

        public void LoadStateFrom(State aState) => state = aState;

        bool prepareKey(State mState, string key, int keyLen)
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
      
            skipFor(ref state, 1013);

            return true;
        }

        void codeBlock(State state, byte[] source, byte[] destination, int len)
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

        void skipFor(ref State state, int len)
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

        Tuple<byte, byte> swapByte(byte a, byte b)
        {
            byte t = a;
            a = b;
            b = t;

            return new Tuple<byte, byte>(a, b);
        }

        public State state = new State(256);

        public class State
        {
            public State(int length = 256)
            {
                X = 0;
                Y = 0;
                S = new byte[length];
            }

            public State(State state)
            {
                X = state.X;
                Y = state.Y;
                S = new byte[256];

                for (int i = 0; i < state.S.Length; i++)
                    S[i] = state.S[i];
            }

            public int X, Y;
            public byte[] S;
        }
    }
}
