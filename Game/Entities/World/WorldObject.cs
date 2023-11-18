using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Navislamia.World.Entities.WorldOption;
using static Navislamia.World.Entities.WorldTime;

namespace Navislamia.World.Entities;

public enum ObjectType
{
    Static,
    Movable,
    Client
}

public class WorldObject
{

    public int RegionIndex;

    public WorldObject(ObjectType t = ObjectType.Static)
    {
        Type = t;
        tag = null;
        RegionIndex = -1;
        IsNearClient = false;
        isDeleted = false;
        layer = 0;
        IsInWorld = false;

        prevRX = prevRY = -1;
        isRegionChanging = false;

        listIndex = activeIndex = 0;

        Region = null;

        lastStepTime = 0;
    }

    public float X => mv.X;

    public float Y => mv.Y;

    public float Z => mv.Z;

    public byte Layer => layer;

    public uint RX => mv.GetRX();

    public uint RY => mv.GetRY();

    public void AddNoise(int r1, int r2, int v)
    {
        float prev_x = mv.X;
        float prev_y = mv.Y;

        uint prev_rx = RX;
        uint prev_ry = RY;

        mv.X += r1 % v - v / 2;
        mv.Y += r2 % v - v / 2;

        if (mv.GetRX() != prev_rx)
            mv.X = prev_x;

        if (mv.GetRY() != prev_ry)
            mv.Y = prev_y;
    }

    public WorldPosition Position => mv.GetPos();

    public WorldMoveVector MoveVector => mv;

    public WorldPosition GetCurrentPosition(ulong t)
    {
        if (!IsMoving())
            return Position;

        WorldMoveVector _mv = mv;

        _mv.Step(t);

        return _mv.GetPos();
    }

    public bool Step(ulong t)
    {
        bool rtn;

        int rx = (int)mv.GetRX();
        int ry = (int)mv.GetRY();

        rtn = mv.Step(t);

        if (rtn)
            mv.SetCurrentPosition(mv.GetTargetPos());

        return rtn;
    }

    public void SetMove(WorldPosition _to, byte _speed, ulong _start_time = 0)
    {
        mv.SetMove(_to, _speed, _start_time, GetWorldTime());
        lastStepTime = mv.StartTime;
    }

    public void SetMultipleMove(List<WorldPosition> _to, byte _speed, ulong _start_time = 0)
    {
        mv.SetMultipleMove(_to, _speed, _start_time, GetWorldTime());
        lastStepTime = mv.StartTime;
    }

    public void SetMove(float _x, float _y, byte _speed, ulong _start_time = 0)
    {
        mv.SetMove(new WorldPosition(_x, _y), _speed, _start_time, GetWorldTime()); ;
        lastStepTime = mv.StartTime;
    }

    public void StopMove() => mv.StopMove();

    public float TX => mv.GetTargetPos().X;

    public float TY => mv.GetTargetPos().Y;

    public float TZ => mv.GetTargetPos().Z;

    public List<WorldMoveVector.MoveInfo> TargetPosList => mv.GetTargetList();

    public WorldPosition TargetPos => mv.GetTargetPos();

    public byte Speed => mv.Speed;

    public ulong StartTime => mv.StartTime;

    public void SetTag(dynamic _tag) => tag = _tag;

    public dynamic Tag => tag;

    public bool IsMoving() => mv.IsMoving() && IsInWorld;

    public bool IsMoving(ulong t) => mv.IsMoving(t);

    public void SetDirection(WorldPosition _to) => mv.SetDirection(_to);

    public bool IsLookingRegion(uint rx, uint ry)
    {
        if (IsVisibleRegion(RX, RY, rx, ry) || IsVisibleRegion((uint)prevRX, (uint)prevRY, rx, ry))
            return true;

        return false;
    }

    public void SetCurrentXY(float x, float y)
    {
        if (prevRX > -1)
        {
            prevRX = (int)mv.GetRX();
            prevRY = (int)mv.GetRY();
        }

        mv.SetCurrentXY(x, y);
    }

    public void SetCurrentZ(float z) => mv.Z = z;

    public volatile bool IsInWorld;
    public volatile bool IsNearClient;

    int prevRX, prevRY;
    bool isRegionChanging;
    ulong lastStepTime;

    public WorldRegion Region;

    public ObjectType Type;

    ulong listIndex;
    ulong activeIndex;

    protected dynamic tag;
    protected WorldMoveVector mv;
    protected byte layer;
    protected bool isDeleted;
}
