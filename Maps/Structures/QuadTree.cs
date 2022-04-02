using System.Collections.Generic;

using Maps.X2D;
using static Maps.X2D.X2DUtil;

namespace Maps.Structures
{
    public class QuadTree
    {
        static bool LOOSE = true;
        static int THRESHOLD = 10;
        static int MAX_DEPTH = 10;

        public QuadTree(float left, float top, float width, float height) => MasterNode = new Node(1, left, top, width, height);

        public QuadTree(float width, float height) => MasterNode = new Node(1, 0, 0, width, height);

        public void ReInit(float left, float top, float width, float height) => MasterNode = new Node(1, left, top, width, height);

        public void ReInit(float width, float height) => MasterNode = new Node(1, 0, 0, width, height);

        public float GetX() => MasterNode.GetX();

        public float GetY() => MasterNode.GetY();

        public float GetWidth() => MasterNode.GetWidth();

        public float GetHeight() => MasterNode.GetHeight();

        public bool Add(MapLocationInfo m) => MasterNode.Add(m);

        public void Enum(PointF pt, List<MapLocationInfo> result) => MasterNode.Enum(pt, result);

        public bool Collision(PointF pt) => MasterNode.Collision(pt);

        public bool LooseCollision(LineF line) => MasterNode.LooseCollision(line);

        public bool Has(MapLocationInfo m) => MasterNode.Has(m);

        public void Remove(MapLocationInfo m) => MasterNode.Remove(m);

        public void Remove(PointF pt) => MasterNode.Remove(pt);

        public class Node
        {
            public Node() 
            {
                for (int i = 0; i < 4; ++i)
                    Nodes[i] = null;
            }

            public Node(short depth, float left, float top, float width, float height)
            {
                Depth = depth;

                Nodes[0] = null;
                Nodes[1] = null;
                Nodes[2] = null;
                Nodes[3] = null;

                init(depth, left, top, width, height);
            }

            ~Node()
            {
                if (hasChildNode)
                    Nodes.Clear();
            }

            internal float GetX() => Area.GetX();

            internal float GetY() => Area.GetY();

            internal float GetWidth() => Area.GetWidth();

            internal float GetHeight() => Area.GetHeight();

            bool hasChildNode => Nodes[0] != null;

            internal bool Add(MapLocationInfo m)
            {
                if (!COLLISION(Area, m))
                    return false;

                if (hasChildNode)
                {
                    Node node = getFitNode(m);

                    if (node != null)
                        if (node.Add(m))
                            return true;
                }

                add(m);

                return true;
            }

            internal bool IsAddable(MapLocationInfo m) => INCLUDE(Area, m);

            internal bool Collision(PointF pt)
            {
                if (!COLLISION(Area, pt))
                    return false;

                for (int i = 0; i < List.Count; ++i)
                    if (COLLISION(List[i], pt))
                        return true;

                if (hasChildNode)
                    for (int i = 0; i < Nodes.Count; ++i)
                        if (Nodes[i].Collision(pt))
                            return true;

                return false;
            }

            internal bool LooseCollision(LineF line)
            {
                if (!COLLISION(Area, line))
                    return false;

                for (int i = 0; i < List.Count; ++i)
                    if (LOOSE_COLLISION(List[i], line))
                        return true;

                if (hasChildNode)
                    for (int i = 0; i < Nodes.Count; ++i)
                        if (Nodes[i].LooseCollision(line))
                            return true;

                return false;
            }

            internal void Enum(PointF pt, List<MapLocationInfo> m)
            {
                if (!COLLISION(Area, pt))
                    return;

                if (hasChildNode)
                    for (int i = 0; i < Nodes.Count; ++i)
                        Nodes[i].Enum(pt, m);

                for (int i = 0; i < List.Count; ++i)
                    if (COLLISION(List[i], pt))
                        m.Add(List[i]);
            }

            internal bool Has(MapLocationInfo m)
            {
                if (!IsAddable(m))
                    return false;

                if (hasChildNode)
                {
                    Node node = getFitNode(m);

                    if (node.Has(m))
                        return true;
                }

                for (int i = 0; i < List.Count; ++i)
                {
                    if (List[i] == m)
                    {
                        List.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            internal void Remove(MapLocationInfo m)
            {
                if (!IsAddable(m))
                    return;

                if (hasChildNode)
                {
                    Node node = getFitNode(m);

                    if (node != null)
                        node.Remove(m);
                }

                for (int i = 0; i < List.Count; ++i)
                {
                    if (List[i] == m)
                    {
                        List.RemoveAt(i);
                        continue;
                    }
                }
            }

            internal void Remove(PointF pt)
            {
                if (hasChildNode)
                {
                    Nodes[0].Remove(pt);
                    Nodes[1].Remove(pt);
                    Nodes[2].Remove(pt);
                    Nodes[3].Remove(pt);
                }

                for (int i = 0; i < List.Count; ++i)
                {
                    if (COLLISION(List[i], pt))
                    {
                        List.RemoveAt(i);
                        continue;
                    }
                }
            }

            internal RectF GetEffectiveArea() => Area;

            internal void SetDepth(short depth)
            {
                Depth = depth;

                if (hasChildNode)
                {
                    Nodes[0].SetDepth(depth);
                    Nodes[1].SetDepth(depth);
                    Nodes[2].SetDepth(depth);
                    Nodes[3].SetDepth(depth);
                }
            }

            Node getFitNode(MapLocationInfo m)
            {
                if (LOOSE)
                {
                    if (Nodes[0].IsAddable(m))
                        return Nodes[0];
                    else if (Nodes[1].IsAddable(m))
                        return Nodes[1];
                    else if (Nodes[2].IsAddable(m))
                        return Nodes[2];
                    else if (Nodes[3].IsAddable(m))
                        return Nodes[3];
                    else
                        return null;
                }

                bool bA0 = Nodes[0].IsAddable(m);
                bool bA1 = Nodes[1].IsAddable(m);
                bool bA2 = Nodes[2].IsAddable(m);
                bool bA3 = Nodes[3].IsAddable(m);

                if (bA1 && bA2 && !bA0 && !bA3)
                    return Nodes[1];

                if (bA2 && bA3 && !bA0 && !bA1)
                    return Nodes[2];

                if (bA3 && bA0 && !bA1 && !bA2)
                    return Nodes[3];

                if (bA0)
                    return Nodes[0];

                if (bA1)
                    return Nodes[1];

                if (bA2)
                    return Nodes[2];

                if (bA3)
                    return Nodes[3];

                return null;
            }

            void add(MapLocationInfo m) 
            {
                List.Add(m);

                if (List.Count >= THRESHOLD && Depth < MAX_DEPTH)
                    divide();
            }

            void divide()
            {
                if (hasChildNode)
                    return;

                Nodes[0] = new Node();
                Nodes[1] = new Node();
                Nodes[2] = new Node();
                Nodes[3] = new Node();

                if (LOOSE)
                {
                    float quad_width = Area.GetWidth() / 4;
                    float quad_height = Area.GetHeight() / 4;
                    float width = Area.GetWidth() - quad_width;
                    float height = Area.GetHeight() - quad_height;

                    Nodes[0].init((short)(Depth + 1), Area.GetLeft(), Area.GetTop(), width, height);
                    Nodes[1].init((short)(Depth + 1), Area.GetLeft() + quad_width, Area.GetTop(), width, height);
                    Nodes[2].init((short)(Depth + 1), Area.GetLeft(), Area.GetTop() + quad_height, width, height);
                    Nodes[3].init((short)(Depth + 1), Area.GetLeft() + quad_width, Area.GetTop() + quad_height, width, height);
                }
                else
                {
                    float half_width = Area.GetWidth() / 2;
                    float half_height = Area.GetHeight() / 2;

                    Nodes[0].init((short)(Depth + 1), Area.GetLeft(), Area.GetTop(), half_width, half_height);
                    Nodes[1].init((short)(Depth + 1), Area.GetLeft() + half_width, Area.GetTop(), half_width, half_height);
                    Nodes[2].init((short)(Depth + 1), Area.GetLeft(), Area.GetTop() + half_height, Area.GetWidth() - half_width, Area.GetHeight() - half_height);
                    Nodes[3].init((short)(Depth + 1), Area.GetLeft() + half_width, Area.GetTop() + half_height, Area.GetWidth() - half_width, Area.GetHeight() - half_height);
                }

                List<MapLocationInfo> tmpList = new List<MapLocationInfo>();

                for (int i = 0; i < List.Count; ++i)
                {
                    Node node = getFitNode(List[i]);

                    if (node != null)
                    {
                        node.Add(List[i]);
                        continue;
                    }

                    tmpList.Add(List[i]);
                }

                List.Clear();
                List = new List<MapLocationInfo>(tmpList);
            }

            void init(short depth, float left, float top, float width, float height)
            {
                Area.Set(left, top, width, height);
                Depth = depth;
            }

            public List<MapLocationInfo> List = new List<MapLocationInfo>();
            public RectF Area = new RectF(0, 0, 0, 0);
            public Dictionary<int, Node> Nodes = new Dictionary<int, Node>();
            public short Depth = 0;
        }

        public Node MasterNode;
    }
}
