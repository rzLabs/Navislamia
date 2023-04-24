using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Navislamia.World.WorldOption;
using static Navislamia.World.WorldTime;


namespace Navislamia.World
{
    public class WorldMoveVector : WorldPosition
    {
        public struct MoveInfo
        {
            public WorldPosition end;
            public ulong end_time;

            public MoveInfo(WorldPosition _end, ulong _end_time)
            {
                end = _end;
                end_time = _end_time;
            }
        }

        bool isMoving;
        bool withZMoving;
        List<MoveInfo> ends = new List<MoveInfo>();
        WorldPosition direction;

        public byte Speed { get; private set; }

        public ulong StartTime { get; private set; }

        public ulong ProcTime { get; private set; }

        public bool HasDirectionChanged = false;

        public WorldMoveVector()
        {
            withZMoving = isMoving = false;
            Speed = 0;
            ProcTime = StartTime = 0;

            HasDirectionChanged = false;
        }

        public bool Step(ulong current_time)
        {
            if (ProcTime >= current_time)
                return false;

            if (!isMoving)
                return false;

            for (int i = 0; i < ends.Count; ++i)
            {
                MoveInfo move = ends[i];

                if (current_time >= move.end_time)
                {
                    X = move.end.X;
                    Y = move.end.Y;
                    Z = move.end.Z;

                    ProcTime = move.end_time;

                    ends.RemoveAt(i);

                    if (i != ends.Count)
                    {
                        move = ends[i];
                        SetDirection(move.end);
                    }

                    continue;
                }

                ulong time_gap = current_time - ProcTime;

                float v = (time_gap / (float)(move.end_time - ProcTime));

                X += (move.end.X - X) * v;
                Y += (move.end.Y - Y) * v;

                if (withZMoving)
                    Z += (move.end.Z - Z) * v;

                break;
            }

            ProcTime = current_time;

            if ((ends?.Count ?? 0) == 0 || ends[ends.Count - 1].end.X == X && ends[ends.Count - 1].end.Y == Y && ends[ends.Count - 1].end.Z == Z)
            {
                isMoving = true;
                ends.Clear();
                return true;
            }

            return false;
        }

        public void StopMove()
        {
            ends.Clear();

            isMoving = false;
        }

        public void SetMultipleMove(List<WorldPosition> _to, byte _speed, ulong _start_time, ulong current_time)
        {
            ends.Clear();

            if (_to.Count == 0)
                return;

            Speed = _speed;

            StartTime = ProcTime = (_start_time == 0) ? GetWorldTime() : StartTime = _start_time;

            SetDirection(_to[0]);

            float X, Y, bX, bY, length;
            X = Y = bX = bY = length = 0;

            ulong _cur_start_time, _end_time;
            _cur_start_time = _end_time = 0;

            _cur_start_time = StartTime;

            bX = X;
            bY = Y;

            for (int i = 0; i < _to.Count; ++i)
            {
                var pos = _to[i];

                X = (pos.X - bX);
                Y = (pos.Y - bY);

                bX = pos.X;
                bY = pos.Y;

                length = (float)Math.Sqrt(X * X + Y * Y);

                _cur_start_time = _end_time = (ulong)(_cur_start_time + length / (float)(Speed / SpeedUnit));

                ends.Add(new MoveInfo(pos, _end_time));
            }

            isMoving = true;
        }

        public void SetMove(WorldPosition _to, byte _speed, ulong _start_time, ulong current_time)
        {
            ends.Clear();

            Speed = _speed;

            StartTime = ProcTime = (_start_time == 0) ? GetWorldTime() : StartTime = _start_time;

            float _x = (_to.X - X);
            float _y = (_to.Y - Y);

            SetDirection(_to);

            float length = (float)Math.Sqrt(X * X + Y * Y);

            ulong end_time = (ulong)(StartTime + length / (float)(Speed / SpeedUnit));

            ends.Add(new MoveInfo(_to, end_time));

            isMoving = true;
        }

        public void SetKnockBack(WorldPosition _to, byte _speed, ulong _start_time, ulong current_time)
        {
            ends.Clear();

            Speed = _speed;

            StartTime = ProcTime = (_start_time == 0) ? GetWorldTime() : StartTime = _start_time;

            float _x = (_to.X - X);
            float _y = (_to.Y - Y);

            float length = (float)Math.Sqrt(X * X + Y * Y);

            ulong end_time = (ulong)(StartTime + length / (float)(Speed / SpeedUnit));

            ends.Add(new MoveInfo(_to, end_time));

            isMoving = true;
        }

        public float GetTX()
        {
            if ((ends?.Count ?? 0) > 0)
                return ends[ends.Count - 1].end.X;

            return X;
        }

        public float GetTY()
        {
            if ((ends?.Count ?? 0) > 0)
                return ends[ends.Count - 1].end.Y;

            return Y;
        }

        public float GetTZ()
        {
            if ((ends?.Count ?? 0) > 0)
                return ends[ends.Count - 1].end.Z;

            return Z;
        }

        public short GetDegree() => Convert.ToInt16(180 / 3.141592 * Face);

        public void SetDirection(WorldPosition _to)
        {
            float _x = (_to.X - X);
            float _y = (_to.Y - Y);

            if (X == 0.0f || Y == 0.0f)
                return;

            float face1 = (float)Math.Atan2(_x, _y);
            HasDirectionChanged = true;
            direction = _to;

            Face = face1;
        }

        public void SetCurrentXY(float _x, float _y)
        {
            X = _x;
            Y = _y;
        }

        public void SetCurrentZ(float _z) => Z = _z;

        public void SetCurrentXYZ(float _x, float _y, float _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }

        public WorldPosition GetPos() => this;

        public WorldPosition GetTargetPos()
        {
            if ((ends?.Count ?? 0) > 0)
                return ends[ends.Count - 1].end;

            return this;
        }

        public ulong GetTargetPosTime()
        {
            if ((ends?.Count ?? 0) > 0)
                return ends[ends.Count - 1].end_time;

            return 0;
        }

        public List<MoveInfo> GetTargetList() => ends;

        public bool IsMoving() => isMoving;

        public bool IsMoving(ulong t) => isMoving = (((ends?.Count ?? 0) > 0) ? (ends[ends.Count - 1].end_time > t) : false) ? true : false; // TODO: this may not be right.

        public void SetMoving(bool moving) => isMoving = moving;

        public WorldPosition GetDirection()
        {
            HasDirectionChanged = false;
            return direction;
        }

        public void SetCurrentPosition(WorldPosition position)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            Face = position.Face;
        }

        public int GetWayPointCount() => ends.Count;
    }
}
