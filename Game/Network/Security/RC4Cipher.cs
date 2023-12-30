using System;

namespace Navislamia.Game.Network.Security;

public class Rc4Cipher
{
    private State _state = new();

    public void Init(string key, int keyLen = 0) => PrepareKey(key, keyLen); 

    public void Code(byte[] source, byte[] destination, int length) => CodeBlock(source, destination, length);

    public void Encode(byte[] source, byte[] destination, int length) => CodeBlock(source, destination, length);

    public void Decode(byte[] source, byte[] destination, int length) => CodeBlock(source, destination, length);

    public State GetState() => _state;

    public void LoadStateFrom(State aState) => _state = aState;

    private void PrepareKey(string key, int keyLen)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        if (keyLen == 0)
        {
            keyLen = key.Length;
        }
        
        for (var i = 0; i < 256; i++)
        {
            _state.S[i] = Convert.ToByte(i);
        }

        var keyBuffer = new byte[256];

        var j = 0;
            
        for (var i = 0; i < 256; i++)
        {
            keyBuffer[i] = Convert.ToByte(key[j++]);

            if (j >= keyLen)
            {
                j = 0;
            }
        }

        j = 0;
        for (var i = 0; i < 256; i++)
        {
            j += _state.S[j] + keyBuffer[j];
            j &= 0xff;

            var swappedBytes = SwapByte(_state.S[i], _state.S[j]);

            _state.S[i] = swappedBytes.Item1;
            _state.S[j] = swappedBytes.Item2;
        }

        _state.X = 0;
        _state.Y = 0;
      
        SkipFor(1013);
    }

    private void CodeBlock(byte[] source, byte[] destination, int len)
    {
        int x = _state.X, y = _state.Y;
        int d = 0, s = 0;

        while (len-- > 0)
        {
            ++x;
            x &= 0xff;
            int sx = _state.S[x];

            y += sx;
            y &= 0xff;
            int sy = _state.S[y];

            _state.S[x] = (byte)sy;
            _state.S[y] = (byte)sx;

            destination[d++] = Convert.ToByte(source[s++] ^ _state.S[(sx + sy) & 0xff]);
        }

        _state.X = x;
        _state.Y = y;
    }

    private void SkipFor(int len)
    {
        int x = _state.X, y = _state.Y;

        while (len-- > 0)
        {
            ++x;
            x &= 0xff;
            int sx = _state.S[x];

            y += sx;
            y &= 0xff;
            int sy = _state.S[y];

            _state.S[x] = (byte)sy;
            _state.S[y] = (byte)sx;
        }

        _state.X = x;
        _state.Y = y;
    }

    private static Tuple<byte, byte> SwapByte(byte a, byte b)
    {
        (a, b) = (b, a);
        return new Tuple<byte, byte>(a, b);
    }
}