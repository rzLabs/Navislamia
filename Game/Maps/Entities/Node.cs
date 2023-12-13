using System.Collections.Generic;
using System.Linq;
using Navislamia.Game.Maps.X2D;
using static Navislamia.Game.Maps.X2D.X2DUtil;

namespace Navislamia.Game.Maps.Entities;

public class Node
{
    static bool LOOSE = true;
    private const int Threshold = 10;
    private const int MaxDepth = 10;

    private List<MapLocationInfo> _mapLocationInfos = new ();
    private RectF _area = new (0, 0, 0, 0);
    private Dictionary<int, Node> _nodes = new ();
    private short _depth = 0;
    
    public Node() 
    {
        for (var i = 0; i < 4; ++i)
            _nodes[i] = null;
    }

    public Node(short depth, float left, float top, float width, float height)
    {
        _depth = depth;

        _nodes[0] = null;
        _nodes[1] = null;
        _nodes[2] = null;
        _nodes[3] = null;

        Init(depth, left, top, width, height);
    }

    // TODO reseach if this destructor is nesessary. C# has a garbage collector which should take care of this
    ~Node()
    {
        if (HasChildNode)
        {
            _nodes.Clear();
        }
    }

    internal float GetX() => _area.GetX();

    internal float GetY() => _area.GetY();

    internal float GetWidth() => _area.GetWidth();

    internal float GetHeight() => _area.GetHeight();

    private bool HasChildNode => _nodes[0] != null;

    internal bool Add(MapLocationInfo m)
    {
        if (!X2DUtil.Collision(_area, m))
        {
            return false;
        }

        if (HasChildNode)
        {
            var node = GetFitNode(m);

            if (node != null)
            {
                if (node.Add(m))
                {
                    return true;
                }
                
            }
        }

        AddToList(m);

        return true;
    }

    private bool IsAddable(MapLocationInfo m) => Included(_area, m);

    internal bool Collision(PointF pt)
    {
        if (!X2DUtil.Collision(_area, pt))
        {
            return false;
        }

        if (_mapLocationInfos.Any(info => X2DUtil.Collision(info, pt)))
        {
            return true;
        }

        if (!HasChildNode)
        {
            return false;
        }
        
        for (var i = 0; i < _nodes.Count; ++i)
        {
            if (_nodes[i].Collision(pt))
            {
                return true;
            }
        }

        return false;
    }

    internal bool LooseCollision(LineF line)
    {
        if (!X2DUtil.Collision(_area, line))
        {
            return false;
        }

        if (_mapLocationInfos.Any(t => LOOSE_COLLISION(t, line)))
        {
            return true;
        }

        if (!HasChildNode)
        {
            return false;
        }
        
        for (var i = 0; i < _nodes.Count; ++i)
        {
            if (_nodes[i].LooseCollision(line))
            {
                return true;
            }
        }

        return false;
    }

    internal void Enum(PointF pt, List<MapLocationInfo> m)
    {
        if (!X2DUtil.Collision(_area, pt))
        {
            return;
        }

        if (HasChildNode)
        {
            for (var i = 0; i < _nodes.Count; ++i)
            {
                _nodes[i].Enum(pt, m);
            }
            
        }

        m.AddRange(_mapLocationInfos.Where(t => X2DUtil.Collision(t, pt)));
    }

    internal bool Has(MapLocationInfo m)
    {
        if (!IsAddable(m))
        {
            return false;
        }

        if (HasChildNode)
        {
            var node = GetFitNode(m);

            if (node.Has(m))
            {
                return true;
            }
        }

        for (var i = 0; i < _mapLocationInfos.Count; ++i)
        {
            if (_mapLocationInfos[i] != m)
            {
                continue;
            }
            
            _mapLocationInfos.RemoveAt(i);
            return true;
        }

        return false;
    }

    internal void Remove(MapLocationInfo m)
    {
        if (!IsAddable(m))
        {
            return;
        }

        if (HasChildNode)
        {
            var node = GetFitNode(m);
            node?.Remove(m);
        }

        for (var i = 0; i < _mapLocationInfos.Count; ++i)
        {
            if (_mapLocationInfos[i] != m)
            {
                continue;
            }
            _mapLocationInfos.RemoveAt(i);
        }
    }

    internal void Remove(PointF pt)
    {
        if (HasChildNode)
        {
            _nodes[0].Remove(pt);
            _nodes[1].Remove(pt);
            _nodes[2].Remove(pt);
            _nodes[3].Remove(pt);
        }

        for (var i = 0; i < _mapLocationInfos.Count; ++i)
        {
            if (!X2DUtil.Collision(_mapLocationInfos[i], pt))
            {
                continue;
            }
            _mapLocationInfos.RemoveAt(i);
        }
    }

    internal RectF GetEffectiveArea() => _area;

    internal void SetDepth(short depth)
    {
        _depth = depth;

        if (!HasChildNode)
        {
            return;
        }
        
        _nodes[0].SetDepth(depth);
        _nodes[1].SetDepth(depth);
        _nodes[2].SetDepth(depth);
        _nodes[3].SetDepth(depth);
    }

    private Node GetFitNode(MapLocationInfo m)
    {
        if (LOOSE)
        {
            if (_nodes[0].IsAddable(m))
            {
                return _nodes[0];
            }

            if (_nodes[1].IsAddable(m))
            {
                return _nodes[1];
            }

            if (_nodes[2].IsAddable(m))
            {
                return _nodes[2];
            }

            if (_nodes[3].IsAddable(m))
            {
                return _nodes[3];
            }
            
            return null;
        }

        var bA0 = _nodes[0].IsAddable(m);
        var bA1 = _nodes[1].IsAddable(m);
        var bA2 = _nodes[2].IsAddable(m);
        var bA3 = _nodes[3].IsAddable(m);

        if (bA1 && bA2 && !bA0 && !bA3)
        {
            return _nodes[1];
        }

        if (bA2 && bA3 && !bA0 && !bA1)
        {
            return _nodes[2];
        }

        if (bA3 && bA0 && !bA1 && !bA2)
        {
            return _nodes[3];
        }

        if (bA0)
        {
            return _nodes[0];
        }

        if (bA1)
        {
            return _nodes[1];
        }

        if (bA2)
        {
            return _nodes[2];
        }

        if (bA3)
        {
            return _nodes[3];
        }

        return null;
    }

    private void AddToList(MapLocationInfo m) 
    {
        _mapLocationInfos.Add(m);

        if (_mapLocationInfos.Count >= Threshold && _depth < MaxDepth)
        {
            Divide();
        }
    }

    private void Divide()
    {
        if (HasChildNode)
        {
            return;
        }

        _nodes[0] = new Node();
        _nodes[1] = new Node();
        _nodes[2] = new Node();
        _nodes[3] = new Node();

        if (LOOSE)
        {
            var quadWidth = _area.GetWidth() / 4;
            var quadHeight = _area.GetHeight() / 4;
            var width = _area.GetWidth() - quadWidth;
            var height = _area.GetHeight() - quadHeight;

            _nodes[0].Init((short)(_depth + 1), _area.GetLeft(), _area.GetTop(), width, height);
            _nodes[1].Init((short)(_depth + 1), _area.GetLeft() + quadWidth, _area.GetTop(), width, height);
            _nodes[2].Init((short)(_depth + 1), _area.GetLeft(), _area.GetTop() + quadHeight, width, height);
            _nodes[3].Init((short)(_depth + 1), _area.GetLeft() + quadWidth, _area.GetTop() + quadHeight, width, height);
        }
        else
        {
            var halfWidth = _area.GetWidth() / 2;
            var halfHeight = _area.GetHeight() / 2;

            _nodes[0].Init((short)(_depth + 1), _area.GetLeft(), _area.GetTop(), halfWidth, halfHeight);
            _nodes[1].Init((short)(_depth + 1), _area.GetLeft() + halfWidth, _area.GetTop(), halfWidth, halfHeight);
            _nodes[2].Init((short)(_depth + 1), _area.GetLeft(), _area.GetTop() + halfHeight, _area.GetWidth() - halfWidth, _area.GetHeight() - halfHeight);
            _nodes[3].Init((short)(_depth + 1), _area.GetLeft() + halfWidth, _area.GetTop() + halfHeight, _area.GetWidth() - halfWidth, _area.GetHeight() - halfHeight);
        }

        var tmpList = new List<MapLocationInfo>();

        foreach (var info in _mapLocationInfos)
        {
            var node = GetFitNode(info);

            if (node != null)
            {
                node.Add(info);
                continue;
            }

            tmpList.Add(info);
        }

        _mapLocationInfos.Clear();
        _mapLocationInfos = new List<MapLocationInfo>(tmpList);
    }

    private void Init(short depth, float left, float top, float width, float height)
    {
        _area.Set(left, top, width, height);
        _depth = depth;
    }
}