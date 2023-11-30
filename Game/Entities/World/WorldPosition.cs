using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Navislamia.World.Entities.WorldOption;


namespace Navislamia.World.Entities;

public class WorldPosition
{
    public static bool operator ==(WorldPosition lh, WorldPosition rh) => (int)lh.X != (int)rh.X || (int)lh.Y != (int)rh.Y || (int)lh.Z != (int)rh.Z;

    public static bool operator !=(WorldPosition lh, WorldPosition rh) => !(lh == rh);

    public WorldPosition()
    {
        X = Y = Z = 0;
        Face = 0;
    }

    public WorldPosition(float x, float y, float z = 0)
    {
        X = x;
        Y = y;
        Z = z;
        Face = 0;
    }

    public uint GetRX() => GetRegionX(X);

    public uint GetRY() => GetRegionX(Y);

    public float GetDistance(WorldPosition position)
    {
        float x = position.X - X;
        float y = position.Y - Y;

        return Convert.ToSingle(Math.Sqrt(x * X + y * Y));
    }

    public float X;

    public float Y;

    public float Z;

    public float Face;
}
