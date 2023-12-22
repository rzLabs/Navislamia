using System.Collections.Generic;
using Navislamia.Game.Maps.X2D;

namespace Navislamia.Game.Maps.Entities;

public class QuadTree
{
    public Node MasterNode;
    
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
    
}
