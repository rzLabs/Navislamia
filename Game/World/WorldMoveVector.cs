using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.World
{
    public class WorldMoveVector : WorldPosition
    {
        public struct MoveData
        {
            public WorldPosition EndPosition;
            public uint EndTime;
        }

        protected const int _speedUnit = 30; // TODO: should come from config

        public bool Moving, ZMoving, DirectionChanged = false;

        public ushort Speed = 0;

        public uint StartTime, ProcessTime = 0;

        List<MoveData> _moveData = new List<MoveData>();

        public WorldPosition Direction;

        public float TargetX
        {
            get
            {
                return (_moveData?.Count > 0) ? _moveData.Last().EndPosition.X : X;
            }
        }

        public float TargetY
        {
            get
            {
                return (_moveData?.Count > 0) ? _moveData.Last().EndPosition.Y : Y;
            }
        }

        public float TargetZ
        {
            get
            {
                return (_moveData?.Count > 0) ? _moveData.Last().EndPosition.Z : Z;
            }
        }

        public WorldPosition TargetPosition
        {
            get
            {
                return (_moveData?.Count > 0) ? _moveData.Last().EndPosition : this;
            }
        }

        public uint TargetPositionTime
        {
            get
            {
                return (_moveData?.Count > 0) ? _moveData.Last().EndTime : 0;
            }
        }

        public List<MoveData> TargetPositionList => _moveData;

        public short Degree
        {
            get
            {
                return (short)(180 / 3.141592 * Face);
            }
        }

        public bool Step(uint currentTime)
        {
            if (ProcessTime >= currentTime)
                return false;

            if (!Moving)
                return false;

            for (int i = 0; i < _moveData.Count; i++)
            {
                var move = _moveData[i];

                if (currentTime >= move.EndTime)
                {
                    X = move.EndPosition.X;
                    Y = move.EndPosition.Y;
                    Z = move.EndPosition.Z;

                    ProcessTime = move.EndTime;

                    SetDirection(move.EndPosition);

                    _moveData.Remove(move);

                    continue;
                }

                uint timeDiff = currentTime - ProcessTime;
                float variation = (timeDiff / (move.EndTime - ProcessTime));

                Y += (move.EndPosition.X - X) * variation;
                Y += (move.EndPosition.Y - Y) * variation;
                Z += (ZMoving) ? (move.EndPosition.Z - Z) * variation : 0;

                break;
            }

            ProcessTime = currentTime;

            if (_moveData.Count == 0)
            {
                Moving = false;
                return true;
            }

            return false;
        }

        public void Stop()
        {
            _moveData.Clear();
            Moving = false;
        }

        public void MultipleMove(List<WorldPosition> positions, ushort speed, uint startTime = 0)
        {
            _moveData.Clear();

            if (positions.Count == 0)
                return;

            Speed = speed;

            StartTime = (startTime > 0) ? startTime : WorldTime.CurrentTime;
            ProcessTime = StartTime;

            SetDirection(positions.First());

            float _x, _y, _previousX, _previousY, _length = 0.0f;
            uint _currentStartTime, _endTime = 0;

            _currentStartTime = StartTime;

            _previousX = X;
            _previousY = Y;

            for (int i = 0; i < positions.Count; i++)
            {
                var position = positions[i];

                _x = position.X - _previousX;
                _y = position.Y - _previousY;

                _previousX = position.X;
                _previousY = position.Y;

                _length = (float)Math.Sqrt((double)_x * _x + _y * _y); // TODO: should use double precision?

                _currentStartTime = _endTime = (uint)(_currentStartTime + _length / ((speed / _speedUnit)));

                _moveData.Add(new MoveData() { EndPosition = position, EndTime = _endTime });
            }

            Moving = true;
        }

        public void Move(WorldPosition position, ushort speed, uint startTime)
        {
            _moveData.Clear();

            Speed = speed;

            StartTime = (startTime > 0) ? startTime : WorldTime.CurrentTime;
            ProcessTime = StartTime;

            float _x, _y, _length;

            _x = position.X - X;
            _y = position.Y - Y;

            SetDirection(position);

            _length = (float)Math.Sqrt(X * X + Y * Y);

            uint _endTime = (uint)(startTime + _length / (speed / _speedUnit));

            _moveData.Add(new MoveData() { EndPosition = position, EndTime = _endTime });

            Moving = true;
        }

        public void SetKnockBack(WorldPosition position, ushort speed, uint startTime, uint currentTime)
        {
            _moveData.Clear();

            Speed = speed;
            StartTime = (startTime > 0) ? startTime : WorldTime.CurrentTime;
            ProcessTime = StartTime;

            float _x, _y, _length;

            _x = position.X - X;
            _y = position.Y - Y;
            _length = (float)Math.Sqrt(X * X + Y * Y);

            uint _endTime = (uint)(startTime + _length / (speed / _speedUnit));

            _moveData.Add(new MoveData() { EndPosition = position, EndTime = _endTime });

            Moving = true;
        }

        public void SetCurrentPosition(WorldPosition position)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            Face = position.Face;
        }

        private void SetDirection(WorldPosition position)
        {
            X = position.X;
            Y = position.Y;

            DirectionChanged = true;
            Direction = position;

            Face = (float)Math.Atan2(X, Y); // TODO: should be double precision?
        }
    }
}
