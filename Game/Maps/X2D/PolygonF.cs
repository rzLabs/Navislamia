using System;
using System.Collections.Generic;
using System.Linq;
using Navislamia.Maps.Enums;
using static Navislamia.Maps.X2D.X2DUtil;

namespace Navislamia.Maps.X2D;

public class PolygonF
{
    private bool _isValid;
    private bool _isClockWise;
    private List<PointF> _points = new ();
    private readonly BoxF _bxArea = new (0, 0, 0, 0);
    
    public PolygonF()
    {
        ClearPolygon();
    }

    public PolygonF(BoxF rh)
    {
        PointF[] pt = new PointF[4];
        pt[0].Set(rh.GetLeft(), rh.GetTop());
        pt[1].Set(rh.GetRight(), rh.GetTop());
        pt[2].Set(rh.GetRight(), rh.GetBottom());
        pt[3].Set(rh.GetLeft(), rh.GetBottom());

        Set(pt);
    }

    public PolygonF(PolygonF rh)
    {
        Set(rh._points.ToArray());
    }

    public PolygonF(IEnumerable<PointF> pts) => Set(pts);

    public bool Set(IEnumerable<PointF> list)
    {
        ClearPolygon();

        _points = new List<PointF>(list);

        RemoveDuplicatedPoint();

        _isValid = IsValid(_points);

        if (_isValid)
        {
            _isClockWise = IsClockWise();

            CalculateArea(_points, _bxArea);
        }
        else
        {
            ClearPolygon();
        }

        return _isValid;
    }

    public bool Scale(float scale, bool alignCenter = false)
    {
        List<PointF> points = new (_points);

        foreach (var point in points)
        {
            point.X *= scale;
            point.Y *= scale;
        }

        float xOffset;
        float yOffset;

        BoxF bxArea = new (0, 0, 0, 0);
        CalculateArea(points, bxArea);

        if (alignCenter)
        {
            xOffset = bxArea.GetCenter().X - _bxArea.GetCenter().X;
            yOffset = bxArea.GetCenter().Y - _bxArea.GetCenter().Y;
        }
        else
        {
            xOffset = bxArea.GetLeft() - _bxArea.GetLeft();
            yOffset = bxArea.GetTop() - _bxArea.GetTop();
        }

        foreach (var point in points)
        {
            point.X -= xOffset;
            point.Y -= yOffset;
        }

        Set(points.ToArray());

        return true;
    }

    public void ClearPolygon()
    {
        _points.Clear();
        _bxArea.Set(0, 0, 0, 0);
        _isClockWise = false;
        _isValid = false;
    }

    private void ReverseCurrentList()
    {
        if (_points.Count == 0)
        {
            return;
        }

        _points.Reverse();
        _isClockWise = !_isClockWise;
    }

    public void MakeToClockwise()
    {
        if (!_isClockWise)
            ReverseCurrentList();
    }

    public bool IsIn(RectF t)
    {
        return _points.All(t.IsIncluded);
    }

    public bool IsLooseIn(RectF t)
    {
        return _points.All(t.IsLooseIncluded);
    }

    public bool Contains(PointF pt)
    {
        return _points.Any(t => t == pt);
    }

    public bool Contains(LineF line)
    {
        for (var index = 0; index < _points.Count; ++index)
        {
            if (GetSegment(index) == line)
            {
                return true;
            }
        }

        return false;
    }

    public BoxF GetBoundingBox() => _bxArea;

    public bool IsIncluded(PointF point)
    {
        // if (!IsValid())
        //  throw new Exception("PolygonF is not valid!")

        if (!_bxArea.IsIncluded(point))
        {
            return false;
        }

        PointF farAway = new (_bxArea.GetRight(), point.Y);
        farAway.X++;

        var touchCnt = 0;
        var intersectCnt = 0;

        for (var index = 0; index < _points.Count; ++index)
        {
            if (point == _points[index])
            {
                return true;
            }

            var point1 = _points[index];
            var point2 = _points[GetNextIndex(index)];
            var result = LineF.IntersectCcw(point1, point2, farAway, point);

            switch (result)
            {
                case IntersectResult.INTERSECT:
                    intersectCnt++;
                    break;
                case IntersectResult.TOUCH:
                {
                    if (point.Y == Math.Min(point1.Y, point2.Y) && point1.Y != point2.Y)
                    {
                        touchCnt++;
                    }
                    break;
                }
            }
        }

        return (touchCnt + intersectCnt) % 2 == 1;
    }

    public bool IsIncluded(float x, float y) => IsIncluded(new PointF(x, y));

    public bool IsLooseIncluded(PointF pt)
    {
        // if (!IsValid)
        //  throw new Exception("PolygonF is not valid!");

        if (_bxArea.IsIncluded(pt))
            return false;

        PointF farAway = new (_bxArea.GetRight(), pt.Y);
        farAway.X++;

        var intersectCnt = 0;
        var touchCnt = 0;

        for (var index = 0; index < _points.Count; ++index)
        {
            if (pt == _points[index])
                return false;

            var point1 = _points[index];
            var point2 = _points[GetNextIndex(index)];

            var result = LineF.IntersectCcw(point1, point2, farAway, pt);

            switch (result)
            {
                case IntersectResult.INTERSECT:
                    intersectCnt++;
                    break;
                case IntersectResult.TOUCH:
                {
                    if (pt.Y == Math.Min(point1.Y, point2.Y) && point1.Y != point2.Y)
                    {
                        touchCnt++;
                    }
                    break;
                }
            }
        }

        return (intersectCnt + touchCnt) % 2 == 1;
    }

    public bool IsLooseInclude(float x, float y) => IsLooseIncluded(new PointF(x, y));

    public LineF GetSegment(int idx) => new LineF(_points[idx], _points[GetNextIndex(idx)]);

    public static bool operator ==(PolygonF lh, PolygonF rh)
    {
        // (if !lh.IsValid && !rh.IsValid)
        //      throw new Exception("Provided polygons are not valid!");

        if (lh == null || rh == null)
        {
            // TODO is true correct here?
            return true;
        }
        
        if (lh.Size() != rh.Size())
        {
            return false;
        }

        var begin = 0;
        var add = 1;

        if (lh._isClockWise != rh._isClockWise)
        {
            begin = lh.Size() - 1;
            add = -1;
        }

        var mod = 0;

        for (var i = 0; i < rh._points.Count; ++i, ++mod)
        {
            if (lh._points[0] == rh._points[i])
            {
                break;
            }
        }

        if (lh._points[0] == rh._points[rh._points.Count - 1])
        {
            return false;
        }

        var rhIndex = 0;

        for (var index = begin; index >= 0 && index < lh.Size(); index += add, ++rhIndex)
        {
            if (rh._points[(rhIndex + mod) % rh.Size()] != lh._points[index])
            {
                return false;
            }
            
        }

        return true;
    }

    public static bool operator !=(PolygonF lh, PolygonF rh) => !(lh == rh);

    public bool IsIncluded(PolygonF rh)
    {
        if (!_bxArea.IsIncluded(rh._bxArea))
        {
            return false;
        }

        if (rh._points.Any(point => !IsIncluded(point)))
        {
            return false;
        }

        for (var myIndex = 0; myIndex < Size(); ++myIndex)
        {
            for (var rhIndex = 0; rhIndex < rh.Size(); ++rhIndex)
            {
                if (GetSegment(myIndex).IntersectCcw(rh.GetSegment(rhIndex)) != IntersectResult.SEPERATE)
                {
                    return false;
                }
            }
        }
            

        return true;
    }

    public bool IsCollision(PointF rh) => IsIncluded(rh);

    public bool IsCollision(BoxF rh)
    {
        PolygonF temp = new (rh);
        return IsCollision(temp);
    }

    public bool IsCollision(RectF rectangle)
    {
        if (IsIncluded(new PointF(rectangle.GetLeft(), rectangle.GetTop())))
        {
            return true;
        }

        if (_points.Any(point => Included(rectangle, point)))
        {
            return true;
        }

        LineF lineA = new (rectangle.GetLeft(), rectangle.GetTop(), rectangle.GetRight(), rectangle.GetTop());
        LineF lineB = new (rectangle.GetRight(), rectangle.GetTop(), rectangle.GetRight(), rectangle.GetBottom());
        LineF lineC = new (rectangle.GetRight(), rectangle.GetBottom(), rectangle.GetLeft(), rectangle.GetBottom());
        LineF lineD = new (rectangle.GetLeft(), rectangle.GetBottom(), rectangle.GetLeft(), rectangle.GetTop());

        for (var myIndex = 0; myIndex < Size(); ++myIndex)
        {
            var line = GetSegment(myIndex);

            if (line.IntersectCcw(lineA) == IntersectResult.INTERSECT)
            {
                return true;
            }

            var resultB = line.IntersectCcw(lineB);
            if (resultB == IntersectResult.INTERSECT)
            {
                return true;
            }

            var resultC = line.IntersectCcw(lineC);
            if (resultC == IntersectResult.INTERSECT)
            {
                return true;
            }

            if (line.IntersectCcw(lineD) == IntersectResult.INTERSECT)
            {
                return true;
            }

            if (line.Begin.X == line.End.X || line.Begin.Y == line.End.Y)
            {
                continue;
            }
            
            if (resultB == IntersectResult.TOUCH && resultC == IntersectResult.TOUCH)
            {
                return true;
            }
        }


        return false;
    }

    public bool IsCollision(LineF line)
    {
        if (IsIncluded(line.Begin) || IsIncluded(line.End))
        {
            return true;
        }

        for (int myIndex = 0; myIndex < Size(); ++myIndex)
        {
            if (GetSegment(myIndex).IntersectCcw(line) != IntersectResult.SEPERATE)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCollision(PolygonF rh)
    {
        if (!_bxArea.IsCollision(rh._bxArea))
        {
            return false;
        }

        if (_points.Any(rh.IsIncluded))
        {
            return true;
        }

        if (rh._points.Any(IsIncluded))
        {
            return true;
        }

        for (var myIndex = 0; myIndex < Size(); ++myIndex)
        {
            for (var rhIndex = 0; rhIndex < rh.Size(); ++rhIndex)
            {
                if (GetSegment(myIndex).IntersectCcw(rh.GetSegment(rhIndex)) != IntersectResult.SEPERATE)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public bool IsLooseCollision(PointF rh) => IsLooseIncluded(rh);

    public bool IsLooseCollision(BoxF rh)
    {
        var temp = new PolygonF(rh);
        return IsLooseCollision(temp);
    }

    public bool IsLooseCollision(RectF rectangle)
    {
        if (IsLooseIncluded(new PointF(rectangle.GetLeft(), rectangle.GetTop())))
        {
            return true;
        }

        if (_points.Any(point => LOOSE_INCLUDE(rectangle, point)))
        {
            return true;
        }

        LineF lineA = new (rectangle.GetLeft(), rectangle.GetTop(), rectangle.GetRight(), rectangle.GetTop());
        LineF lineB = new (rectangle.GetRight(), rectangle.GetTop(), rectangle.GetRight(), rectangle.GetBottom());
        LineF lineC = new (rectangle.GetRight(), rectangle.GetBottom(), rectangle.GetLeft(), rectangle.GetBottom());
        LineF lineD = new (rectangle.GetLeft(), rectangle.GetBottom(), rectangle.GetLeft(), rectangle.GetTop());

        for (var myIndex = 0; myIndex < Size(); ++myIndex)
        {
            var line = GetSegment(myIndex);

            if (line.IntersectCcw(lineA) == IntersectResult.INTERSECT)
            {
                return true;
            }

            if (line.IntersectCcw(lineB) == IntersectResult.INTERSECT)
            {
                return true;
            }

            if (line.IntersectCcw(lineC) == IntersectResult.INTERSECT)
            {
                return true;
            }

            if (line.IntersectCcw(lineD) == IntersectResult.INTERSECT)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsLooseCollision(LineF line)
    {
        if (IsLooseIncluded(line.Begin) || IsLooseIncluded(line.End))
        {
            return true;
        }

        for (var myIndex = 0; myIndex < Size(); ++myIndex)
        {
            var result = GetSegment(myIndex).IntersectCcw(line);

            if (result != IntersectResult.SEPERATE && result != IntersectResult.TOUCH)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsLooseCollision(PolygonF rh)
    {
        if (!_bxArea.IsLooseCollision(rh._bxArea))
        {
            return false;
        }

        if (_points.Any(rh.IsLooseIncluded))
        {
            return true;
        }

        if (rh._points.Any(IsLooseIncluded))
        {
            return true;
        }

        for (var myIndex = 0; myIndex < Size(); ++myIndex)
            for (var rhIndex = 0; rhIndex < rh.Size(); ++rhIndex)
            {
                var result = GetSegment(myIndex).IntersectCcw(rh.GetSegment(rhIndex));

                if (result != IntersectResult.SEPERATE && result != IntersectResult.TOUCH)
                {
                    return true;
                }
            }

        return false;
    }

    public void RemoveDuplicatedPoint()
    {
        if (!_isValid)
        {
            return;
        }
        
        PointF prevPoint = _points[0];

        for (var i = 0; i < _points.Count; ++i)
        {
            PointF curPoint;
            if ((curPoint = _points[i]) == prevPoint)
            {
                _points.RemoveAt(i);
            }
            else
            {
                prevPoint = curPoint;
            }
        }

        var pointCount = _points.Count;

        if (pointCount < 3)
        {
            ClearPolygon();
        }
    }

    public int Size() => _points.Count;

    public PointF GetPoint(int idx) => _points[idx];

    public PointF GetRawPoint() => _points[0];

    public PointF GetCenter() => _bxArea.GetCenter();

    public float GetTop() => _bxArea.GetTop(); 

    public float GetBottom() => _bxArea.GetBottom();

    public float GetLeft() => _bxArea.GetLeft();

    public float GetRight() => _bxArea.GetRight();
    
    private int GetPrevIndex(int idx) => idx == 0 ? _points.Count() - 1 : idx - 1;
    
    private int GetNextIndex(int idx) => idx == _points.Count() - 1 ? 0 : idx + 1;

    private bool IsValid(List<PointF> list)
    {
        if (list.Count < 3)
        {
            return false;
        }

        for (var i = 0; i < list.Count; ++i)
        {
            for (var j = i + 1; j < list.Count; ++j)
            {
                if (list[j] == list[i])
                {
                    return false;
                }
            }
        }

        for (var index1 = 0; index1 < list.Count; index1++)
        {
            for (var index2 = index1 + 1; index2 < list.Count; ++index2)
            {
                var p2 = GetNextIndex(index1);
                var p4 = GetNextIndex(index2);
                var result = LineF.IntersectCcw(list[index1], list[p2], list[index2], list[p4]);
                
                // false on intersect or touch
                switch (result)
                {
                    case IntersectResult.INTERSECT:
                    case IntersectResult.TOUCH when p2 != index2 && index1 != p4:
                        return false;
                }
            }
        }

        return true;
    }

    private bool IsClockWise()
    {
        var midIndex = 0;

        for (var i = 0; i < _points.Count; ++i)
        {
            if (_points[midIndex].X > _points[i].X)
                midIndex = i;
        }

        CcwResult ccwResult;

        var count = 0;

        while (true)
        {
            var prevIndex = GetPrevIndex(midIndex);
            var nextIndex = GetNextIndex(midIndex);

            ccwResult = CheckCloseWise(_points[prevIndex].X, _points[prevIndex].Y, _points[midIndex].X, _points[midIndex].Y, _points[nextIndex].X, _points[nextIndex].Y);

            if (ccwResult == CcwResult.Parallelism)
            {
                break;
            }

            if (midIndex > _points.Count)
            {
                throw new IndexOutOfRangeException("midIndex is out of range!");
            }

            ++midIndex;
            ++count;

            if (count > _points.Count)
            {
                break;
            }

            if (midIndex == _points.Count)
            {
                midIndex = 0;
            }
        }

        return ccwResult == CcwResult.ClockWise;
    }

    private void Loop() => throw new NotImplementedException();

    private void CalculateArea(List<PointF> points, BoxF area)
    {
        PointF p1 = new (points[0].X, points[0].Y);
        PointF p2 = new (points[0].X, points[0].Y);

        area.Set(p1, p2);

        foreach (var point in points)
        {
            if (point.X < area.GetLeft())
            {
                area.SetLeft(point.X);
            }

            if (point.Y < area.GetTop())
            {
                area.SetTop(point.Y);
            }

            if (point.X > area.GetRight())
            {
                area.SetRight(point.X);
            }

            if (point.Y > area.GetBottom())
            {
                area.SetBottom(point.Y);
            }
        }
    }
}
