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

        public void SaveStateTo(State outState) => outState = state;

        public void LoadStateFrom(State aState) => state = aState;

        static bool prepareKey(State mState, string key, int keyLen)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (keyLen == 0)
                keyLen = key.Length;

            state.X = state.Y = 0;
            int j = 0;
            byte[] bKey = new byte[256];
            bKey = Encoding.ASCII.GetBytes(key);

            for (int i = 0; i < 256; i++)
                state.S[i] = (byte)i;

            for (int i = 0; i < 256; i++)
            {
                j = (state.S[j % keyLen] + bKey[j] + j) & 0xff;
                swapByte(state.S[i], state.S[j]);

                if (j >= keyLen)
                    j = 0;
            }
            
            skipFor(state, 1013);
            return true;
        }

        static void codeBlock(State state, byte[] source, byte[] destination, int len)
        {
            int i, j;
            i = j = 0;
            for(int n = 0; n < len; n++)
            {
                i = (i + 1) & 0xFF;
                j = (j + state.S[i]) & 0xFF;
                swapByte(state.S[i], state.S[j]);

                int rnd = state.S[(state.S[i] + state.S[j]) % 256];

                destination[n] = (byte)(rnd ^ source[n]);
            }
        }

        static void skipFor(State state, int len)
        {
            for(int i = 0; i < len; i++)
            {
                state.X = (state.X + 1) & 0xFF;
                state.Y = (state.S[state.X + state.Y]) & 0xFF;
                swapByte(state.S[state.X], state.S[state.Y]);
            }
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
