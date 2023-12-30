namespace Navislamia.Game.Network.Security;

public interface ICipher
{
    void Encode(byte[] source, byte[] destination, int length, bool isPeek = false);

    void Decode(byte[] source, byte[] destination, int length, bool isPeek = false);

    void Clear();
}