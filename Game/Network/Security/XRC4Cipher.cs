namespace Navislamia.Game.Network.Security;

public class TImpl : Rc4Cipher { }

public class Xrc4Cipher : ICipher
{
    public Xrc4Cipher()
    {
        Impl = new TImpl();
        Clear();
    }

    public void SetKey(string key) => Impl.Init(key, key.Length);

    public void Clear() => Impl.Init("Neat & Simple", 0);

    public void Decode(byte[] source, byte[] destination, int length, bool isPeek = false)
    {
        if (isPeek)
            TryCipher(source, destination, length);
        else
            DoCipher(source, destination, length);
    }

    public void Encode(byte[] source, byte[] destination, int length, bool isPeek = false)
    {
        if (isPeek)
            TryCipher(source, destination, length);
        else
            DoCipher(source, destination, length);
    }

    private void TryCipher(byte[] source, byte[] destination, int length)
    {
        var backup = new Rc4Cipher.State(Impl.GetState());

        DoCipher(source, destination, length);

        Impl.LoadStateFrom(backup);
    }

    private void DoCipher(byte[] source, byte[] destination, int length) => Impl.Code(source, destination, length);

    TImpl Impl;
}