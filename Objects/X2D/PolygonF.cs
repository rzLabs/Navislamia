using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Objects.X2D.X2DUtil;

namespace Objects.X2D
{
    public class PolygonF
    {
        public PolygonF()
        {
            Clear();
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
            Set(rh.list.ToArray());
        }

        public PolygonF(PointF[] pts) => Set(pts);

        public bool Set(PointF[] list)
        {
            Clear();

            this.list = new List<PointF>(list);

            RemoveDuplicatedPoint();

            _isValid = isValid(this.list);

            if (_isValid)
            {
                _isClockWise = isClockWise();

                caculateArea(this.list, bxArea);
            }
            else
                Clear();

            return _isValid;
        }

        public bool Scale(float scale, bool alignCenter = false)
        {
            List<PointF> tmpList = new List<PointF>(list);

            for (int i = 0; i < tmpList.Count; ++i)
            {
                tmpList[i].X *= scale;
                tmpList[i].Y *= scale;
            }

            float x_offset;
            float y_offset;

            BoxF bxArea = new BoxF(0, 0, 0, 0);
            caculateArea(tmpList, bxArea);

            if (alignCenter)
            {
                x_offset = bxArea.GetCenter().X - this.bxArea.GetCenter().X;
                y_offset = bxArea.GetCenter().Y - this.bxArea.GetCenter().Y;
            }
            else
            {
                x_offset = bxArea.GetLeft() - this.bxArea.GetLeft();
                y_offset = bxArea.GetTop() - this.bxArea.GetTop();
            }

            for (int i = 0; i < tmpList.Count; ++i)
            {
                tmpList[i].X -= x_offset;
                tmpList[i].Y -= y_offset;
            }

            Set(tmpList.ToArray());

            return true;
        }

        public void Clear()
        {
            list.Clear();
            bxArea.Set(0, 0, 0, 0);
            _isClockWise = false;
            _isValid = false;
        }

        public void Reverse()
        {
            if (list.Count == 0)
                return;

            list.Reverse();

            _isClockWise = !_isClockWise;
        }

        public bool IsClockWise() => _isClockWise;

        public void MakeToClockwise()
        {
            if (!IsClockWise())
                Reverse();
        }

        public bool IsIn(RectF t)
        {
            for (int i = 0; i < list.Count; ++i)
                if (!t.IsInclude(list[i]))
                    return false;

            return true;
        }

        public bool IsLooseIn(RectF t)
        {
            for (int i = 0; i < list.Count; ++i)
                if (!t.IsLooseInclude(list[i]))
                    return false;

            return true;
        }

        public bool Has(PointF pt)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i] == pt)
                    return true;
            }

            return false;
        }

        public bool Has(LineF line)
        {
            for (int idx = 0; idx < list.Count; ++idx)
            {
                if (GetSegment(idx) == line)
                    return true;
            }

            return false;
        }

        public BoxF GetBoundingBox() => bxArea;

        public bool IsInclude(PointF pt)
        {
            // if (!IsValid())
            //  throw new Exception("PolygonF is not valid!")

            if (!bxArea.IsInclude(pt))
                return false;

            PointF farAway = new PointF(bxArea.GetRight(), pt.Y);
            farAway.X++;

            int touchCnt = 0;
            int intersectCnt = 0;

            for (int idx = 0; idx < list.Count; ++idx)
            {
                if (pt == list[idx])
                    return true;

                PointF pt1 = list[idx];
                PointF pt2 = list[getNextIndex(idx)];

                IntersectResult result = LineF.IntersectCCW(pt1, pt2, farAway, pt);

                if (result == IntersectResult.INTERSECT)
                    intersectCnt++;

                if (result == IntersectResult.TOUCH)
                    if (pt.Y == Math.Min(pt1.Y, pt2.Y) && pt1.Y != pt2.Y)
                        touchCnt++;
            }

            return ((touchCnt + intersectCnt) % 2 == 1);
        }

        public bool IsInclude(float x, float y) => IsInclude(new PointF(x, y));

        public bool IsLooseInclude(PointF pt)
        {
            // if (!IsValid)
            //  throw new Exception("PolygonF is not valid!");

            if (bxArea.IsInclude(pt))
                return false;

            PointF farAway = new PointF(bxArea.GetRight(), pt.Y);
            farAway.X++;

            int intersectCnt = 0;
            int touchCnt = 0;

            for (int idx = 0; idx < list.Count; ++idx)
            {
                if (pt == list[idx])
                    return false;

                PointF pt1 = list[idx];
                PointF pt2 = list[getNextIndex(idx)];

                IntersectResult result = LineF.IntersectCCW(pt1, pt2, farAway, pt);

                if (result == IntersectResult.INTERSECT)
                    intersectCnt++;

                if (result == IntersectResult.TOUCH)
                    if (pt.Y == Math.Min(pt1.Y, pt2.Y) && pt1.Y != pt2.Y)
                        touchCnt++;
            }

            return ((intersectCnt + touchCnt) % 2 == 1);
        }

        public bool IsLooseInclude(float x, float y) => IsLooseInclude(new PointF(x, y));

        public bool IsValid => _isValid;

        public LineF GetSegment(int idx) => new LineF(list[idx], list[getNextIndex(idx)]);

        public static bool operator ==(PolygonF lh, PolygonF rh)
        {
            // (if !lh.IsValid && !rh.IsValid)
            //      throw new Exception("Provided polygons are not valid!");

            if (lh.Size() != rh.Size())
                return false;

            int begin = 0;
            int add = 1;

            if (lh.IsClockWise() != rh.IsClockWise())
            {
                begin = lh.Size() - 1;
                add = -1;
            }

            int mod = 0;

            for (int i = 0; i < rh.list.Count; ++i, ++mod)
                if (lh.list[0] == rh.list[i])
                    break;

            if (lh.list[0] == rh.list[rh.list.Count - 1])
                return false;

            int rh_idx = 0;

            for (int idx = begin; idx >= 0 && idx < lh.Size(); idx += add, ++rh_idx)
                if (rh.list[(rh_idx + mod) % rh.Size()] != lh.list[idx])
                    return false;

            return true;
        }

        public static bool operator !=(PolygonF lh, PolygonF rh) => !(lh == rh);

        public bool IsInclude(PolygonF rh)
        {
            if (!bxArea.IsInclude(rh.bxArea))
                return false;

            for (int i = 0; i < rh.list.Count; ++i)
                if (!IsInclude(rh.list[i]))
                    return false;

            for (int my_idx = 0; my_idx < Size(); ++my_idx)
                for (int rh_idx = 0; rh_idx < rh.Size(); ++rh_idx)
                    if (GetSegment(my_idx).IntersectCCW(rh.GetSegment(rh_idx)) != IntersectResult.SEPERATE)
                        return false;

            return true;
        }

        public bool IsCollision(PointF rh) => IsInclude(rh);

        public bool IsCollision(BoxF rh)
        {
            PolygonF temp = new PolygonF(rh);
            return IsCollision(temp);
        }

        public bool IsCollision(RectF rc)
        {
            if (IsInclude(new PointF(rc.GetLeft(), rc.GetTop())))
                return true;

            for (int i = 0; i < list.Count; ++i)
                if (INCLUDE(rc, list[i]))
                    return true;

            LineF line_a = new LineF(rc.GetLeft(), rc.GetTop(), rc.GetRight(), rc.GetTop());
            LineF line_b = new LineF(rc.GetRight(), rc.GetTop(), rc.GetRight(), rc.GetBottom());
            LineF line_c = new LineF(rc.GetRight(), rc.GetBottom(), rc.GetLeft(), rc.GetBottom());
            LineF line_d = new LineF(rc.GetLeft(), rc.GetBottom(), rc.GetLeft(), rc.GetTop());

            for (int my_idx = 0; my_idx < Size(); ++my_idx)
            {
                LineF line = GetSegment(my_idx);
                IntersectResult result_a, result_b, result_c, result_d;

                if ((result_a = line.IntersectCCW(line_a)) == IntersectResult.INTERSECT)
                    return true;

                if ((result_b = line.IntersectCCW(line_b)) == IntersectResult.INTERSECT)
                    return true;

                if ((result_c = line.IntersectCCW(line_c)) == IntersectResult.INTERSECT)
                    return true;

                if ((result_d = line.IntersectCCW(line_d)) == IntersectResult.INTERSECT)
                    return true;

                if (line.Begin.X != line.End.X && line.Begin.Y != line.End.Y)
                    if (result_b == IntersectResult.TOUCH && result_c == IntersectResult.TOUCH)
                        return true;
            }


            return false;
        }

        public bool IsCollision(LineF line)
        {
            if (IsInclude(line.Begin) || IsInclude(line.End))
                return true;

            for (int my_idx = 0; my_idx < Size(); ++my_idx)
                if (GetSegment(my_idx).IntersectCCW(line) != IntersectResult.SEPERATE)
                    return true;

            return false;
        }

        public bool IsCollision(PolygonF rh)
        {
            if (!bxArea.IsCollision(rh.bxArea))
                return false;

            for (int i = 0; i < list.Count; ++i)
                if (rh.IsInclude(list[i]))
                    return true;

            for (int i = 0; i < rh.list.Count; ++i)
                if (IsInclude(rh.list[i]))
                    return true;

            for (int my_idx = 0; my_idx < Size(); ++my_idx)
                for (int rh_idx = 0; rh_idx < rh.Size(); ++rh_idx)
                    if (GetSegment(my_idx).IntersectCCW(rh.GetSegment(rh_idx)) != IntersectResult.SEPERATE)
                        return true;

            return false;
        }

        public bool IsLooseCollision(PointF rh) => IsLooseInclude(rh);

        public bool IsLooseCollision(BoxF rh)
        {
            PolygonF temp = new PolygonF(rh);
            return IsLooseCollision(temp);
        }

        public bool IsLooseCollision(RectF rc)
        {
            if (IsLooseInclude(new PointF(rc.GetLeft(), rc.GetTop())))
                return true;

            for (int i = 0; i < list.Count; ++i)
                if (LOOSE_INCLUDE(rc, list[i]))
                    return true;

            LineF line_a = new LineF(rc.GetLeft(), rc.GetTop(), rc.GetRight(), rc.GetTop());
            LineF line_b = new LineF(rc.GetRight(), rc.GetTop(), rc.GetRight(), rc.GetBottom());
            LineF line_c = new LineF(rc.GetRight(), rc.GetBottom(), rc.GetLeft(), rc.GetBottom());
            LineF line_d = new LineF(rc.GetLeft(), rc.GetBottom(), rc.GetLeft(), rc.GetTop());

            for (int my_idx = 0; my_idx < Size(); ++my_idx)
            {
                LineF line = GetSegment(my_idx);

                IntersectResult result_a, result_b, result_c, result_d;

                if ((result_a = line.IntersectCCW(line_a)) == IntersectResult.INTERSECT)
                    return true;

                if ((result_b = line.IntersectCCW(line_b)) == IntersectResult.INTERSECT)
                    return true;

                if ((result_c = line.IntersectCCW(line_c)) == IntersectResult.INTERSECT)
                    return true;

                if ((result_d = line.IntersectCCW(line_d)) == IntersectResult.INTERSECT)
                    return true;
            }

            return false;
        }

        public bool IsLooseCollision(LineF line)
        {
            if (IsLooseInclude(line.Begin) || IsLooseInclude(line.End))
                return true;

            for (int my_idx = 0; my_idx < Size(); ++my_idx)
            {
                IntersectResult result = GetSegment(my_idx).IntersectCCW(line);

                if (result != IntersectResult.SEPERATE && result != IntersectResult.TOUCH)
                    return true;
            }

            return false;
        }

        public bool IsLooseCollision(PolygonF rh)
        {
            if (!bxArea.IsLooseCollision(rh.bxArea))
                return false;

            for (int i = 0; i < list.Count; ++i)
                if (rh.IsLooseInclude(list[i]))
                    return true;

            for (int i = 0; i < rh.list.Count; ++i)
                if (IsLooseInclude(rh.list[i]))
                    return true;

            for (int my_idx = 0; my_idx < Size(); ++my_idx)
                for (int rh_idx = 0; rh_idx < rh.Size(); ++rh_idx)
                {
                    IntersectResult result = GetSegment(my_idx).IntersectCCW(rh.GetSegment(rh_idx));

                    if (result != IntersectResult.SEPERATE && result != IntersectResult.TOUCH)
                        return true;
                }

            return false;
        }

        public void RemoveDuplicatedPoint()
        {
            if (!IsValid)
                return;
            
            PointF prev_pt = list[0];
            PointF cur_pt = null;

            for (int i = 0; i < list.Count; ++i)
                if ((cur_pt = list[i]) == prev_pt)
                    list.RemoveAt(i);
                else
                    prev_pt = cur_pt;

            int cnt = list.Count;

            if (cnt < 3)
                Clear();
        }

        public int Size() => list.Count();

        public PointF GetPoint(int idx) => list[idx];

        public PointF GetRawPoint() => list[0];

        public PointF GetCenter() => bxArea.GetCenter();

        public float GetTop() => bxArea.GetTop(); 

        public float GetBottom() => bxArea.GetBottom();

        public float GetLeft() => bxArea.GetLeft();

        public float GetRight() => bxArea.GetRight();
        
        int getPrevIndex(int idx) => idx == 0 ? list.Count() - 1 : idx - 1;
        int getNextIndex(int idx) => idx == list.Count() - 1 ? 0 : idx + 1;

        bool isValid(List<PointF> list)
        {
            if (list.Count < 3)
                return false;

            for (int i = 0; i < list.Count; ++i)
                for (int j = i + 1; j < list.Count; ++j)
                    if (list[j] == list[i])
                        return false;

            for (int idx_1 = 0; idx_1 < list.Count; idx_1++)
            {
                for (int idx_2 = idx_1 + 1; idx_2 < list.Count; ++idx_2)
                {
                    int p1 = idx_1;
                    int p2 = getNextIndex(idx_1);
                    int p3 = idx_2;
                    int p4 = getNextIndex(idx_2);

                    IntersectResult result = LineF.IntersectCCW(list[p1], list[p2], list[p3], list[p4]);

                    if (result == IntersectResult.INTERSECT)
                        return false;

                    if (result == IntersectResult.TOUCH && p2 != p3 && p1 != p4)
                        return false;
                }
            }

            return true;
        }

        bool isClockWise()
        {
            int mid_idx = 0;

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[mid_idx].X > list[i].X)
                    mid_idx = i;
            }

            CCWResult ccwResult = CCWResult.ClockWise;

            int cnt = 0;

            while (true)
            {
                int prev_idx = getPrevIndex(mid_idx);
                int next_idx = getNextIndex(mid_idx);

                ccwResult = CheckCloseWise(list[prev_idx].X, list[prev_idx].Y, list[mid_idx].X, list[mid_idx].Y, list[next_idx].X, list[next_idx].Y);

                if (ccwResult == CCWResult.Parallelism)
                    break;

                if (mid_idx > list.Count)
                    throw new IndexOutOfRangeException("mid_idx is out of range!");

                ++mid_idx;
                ++cnt;

                if (cnt > list.Count)
                    break;

                if (mid_idx == list.Count)
                    mid_idx = 0;
            }

            return ccwResult == CCWResult.ClockWise;
        }

        void loop() =>
            throw new NotImplementedException();

        void caculateArea(List<PointF> list, BoxF area)
        {
            PointF p1 = new PointF(list[0].X, list[0].Y);
            PointF p2 = new PointF(list[0].X, list[0].Y);

            area.Set(p1, p2);

            for (int i = 0; i < list.Count; ++i)
            {
                PointF it = list[i];

                if (it.X < area.GetLeft())
                    area.SetLeft(it.X);

                if (it.Y < area.GetTop())
                    area.SetTop(it.Y);

                if (it.X > area.GetRight())
                    area.SetRight(it.X);

                if (it.Y > area.GetBottom())
                    area.SetBottom(it.Y);
            }
        }

        bool _isValid = false;
        bool _isClockWise = false;
        List<PointF> list = new List<PointF>();
        BoxF bxArea = new BoxF(0, 0, 0, 0);
    }
}
