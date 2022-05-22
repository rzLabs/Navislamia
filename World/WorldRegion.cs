using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World
{
    public class WorldRegion
    {
        public void AddStaticObject(WorldObject obj) => addObject(obj, StaticObjects);

        public void AddMovableObject(WorldObject obj) => addObject(obj, MoveableObjects);

        public void AddClientObject(WorldObject obj) => addObject(obj, ClientObjects);

        public void AddObject(WorldObject obj)
        {
            switch (obj.Type)
            {
                case ObjectType.Static: AddStaticObject(obj); break;
                case ObjectType.Movable: AddMovableObject(obj); break;
                case ObjectType.Client: AddClientObject(obj); break;
            }
        }

        public void RemoveStaticObject(WorldObject obj) => removeObject(obj, StaticObjects);

        public void RemoveMoveableObject(WorldObject obj) => removeObject(obj, MoveableObjects);

        public void RemoveClientObject(WorldObject obj) => removeObject(obj, StaticObjects);

        public void RemoveObject(WorldObject obj)
        {
            switch (obj.Type)
            {
                case ObjectType.Static: RemoveStaticObject(obj); break;
                case ObjectType.Movable: RemoveMoveableObject(obj); break;
                case ObjectType.Client: RemoveClientObject(obj); break;

            }
        }

        public uint ClientCount => (uint)ClientObjects.Count;

        public uint DoEachClient(Func<object, int> _fu)
        {
            for (int i = 0; i < ClientObjects.Count; ++i)
                _fu(ClientObjects[i]);

            return (uint)ClientObjects.Count;
        }

        public uint DoEachStaticObject(Func<object, int> _fu)
        {
            for (int i = 0; i < StaticObjects.Count; ++i)
                _fu(StaticObjects[i]);

            return (uint)StaticObjects.Count;
        }

        public uint DoEachMovableObject(Func<object, int> _fu)
        {
            for (int i = 0; i < MoveableObjects.Count; ++i)
                _fu(MoveableObjects[i]);

            return (uint)MoveableObjects.Count;
        }

        public float X, Y;
        public byte Layer;

        void addObject(WorldObject obj, List<WorldObject> l)
        {
            l.Add(obj);

            obj.RegionIndex = l.Count - 1;

            if (obj.RegionIndex < 0)
            {
                // TODO: log invalid region

                return;
            }

            obj.IsInWorld = true;
            obj.Region = this;
        }

        void removeObject(WorldObject obj, List<WorldObject> l)
        {
            // TODO: DebugRegionList

            if (l.Count == 0)
            {
                // TODO: log remove object exception
                return;
            }

           if (l[obj.RegionIndex] != obj)
            {
                // TODO: log mismatch exception
                return;
            }

           if (obj != l[l.Count - 1])
            {
                l[obj.RegionIndex] = l[l.Count - 1];
                l[obj.RegionIndex].RegionIndex = obj.RegionIndex;
            }

            l.RemoveAt(l.Count - 1);

            obj.RegionIndex = -1;
            obj.IsInWorld = false;
            obj.Region = null;
        }

        List<WorldObject> StaticObjects = new List<WorldObject>();
        List<WorldObject> MoveableObjects = new List<WorldObject>();
        List<WorldObject> ClientObjects = new List<WorldObject>();

    }
}
