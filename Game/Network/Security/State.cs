namespace Navislamia.Game.Network.Security;

public class State
{
    public int X;
    public int Y;
    public byte[] S;
    
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

        for (var i = 0; i < state.S.Length; i++)
        {
            S[i] = state.S[i];
        }
    }


}

