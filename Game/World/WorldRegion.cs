using Navislamia.Game.World.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.World
{
    public class WorldRegion
    {
        List<WorldObject> _staticObjects = new List<WorldObject>();
        List<WorldObject> _movableObjects = new List<WorldObject>();
        List<WorldObject> _clientObjects = new List<WorldObject>();

        public uint X, Y;
        public ushort Layer;

        public int ClientCount => _clientObjects?.Count ?? 0;

        public void AddObject(WorldObject obj)
        {
            switch (obj.Type)
            {
                case WorldObjectType.Static:
                    _staticObjects.Add(obj);
                    break;

                case WorldObjectType.Moveable:
                    _movableObjects.Add(obj);
                    break;

                case WorldObjectType.Client:
                    _clientObjects.Add(obj);
                    break;
            }
        }

        public void RemoveObject(WorldObject obj)
        {
            switch (obj.Type)
            {
                case WorldObjectType.Static:
                    _staticObjects.Remove(obj);
                    break;

                case WorldObjectType.Moveable:
                    _movableObjects.Remove(obj);
                    break;

                case WorldObjectType.Client:
                    _clientObjects.Remove(obj);
                    break;
            }
        }

        public void DoEachObject(WorldObjectType objType, Func<WorldObject, int> objFunc)
        {
            switch (objType)
            {
                case WorldObjectType.Static:
                    foreach (var obj in _staticObjects)
                    {
                        objFunc(obj);
                    }
                    break;

                case WorldObjectType.Moveable:
                    foreach (var obj in _movableObjects)
                    {
                        objFunc(obj);
                    }
                    break;

                case WorldObjectType.Client:
                    foreach (var obj in _clientObjects)
                    {
                        objFunc(obj);
                    }
                    break;
            }
        }
    }
}
