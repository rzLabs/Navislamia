namespace Navislamia.Game.Scripting.Functions;

public class Player
{
    public static int get_value(params object[] args)
    {
        switch (args.Length)
        {
            case < 2:
                return 1;
            case 3:
                break;
        }

        var key = args[0].ToString();

        // return pPlayer.GetValue();

        // pPlayer.SetValue()


        return 0;
    }
}