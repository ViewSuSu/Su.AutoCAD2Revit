using Line = Teigha.DatabaseServices.Line;

namespace Su.AutoCAD2Revit.Extension
{
    internal static class LineExtension
    {
        /// <summary>
        /// 获取线的中点
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static Point3d MidPoint(this Line line)
        {
            return PointExtension.MidPoint(line.StartPoint, line.EndPoint);
        }

        /// <summary>
        /// 判断两条线是否平行
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        internal static bool IsParallel(this Line line1, Line line2, double tol = 0.01)
        {
            double angle = Math.Abs(line1.Angle - line2.Angle) % Math.PI;
            return angle < tol || Math.PI - angle < tol;
        }

        internal static bool IsOnLine(this Line line, Point3d pt, bool extend, double tol)
        {
            return line.GetClosestPointTo(pt, extend).DistanceTo(pt) < tol;
        }

        /// <summary>
        /// 获取直线的垂直向量
        /// </summary>
        /// <param name="dirLine"></param>
        /// <returns></returns>
        internal static Vector3d GetNormal(this Line dirLine)
        {
            Vector3d vec = dirLine.EndPoint - dirLine.StartPoint;
            return vec.GetNormal();
        }

        /// <summary>
        /// 过点做线的垂线
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="dirLine"></param>
        /// <returns></returns>
        internal static Line GetVerticalLine(this Line dirLine, Point3d pt)
        {
            Vector3d vec = dirLine.EndPoint - dirLine.StartPoint;
            return vec.GetVerticalLine(pt);
        }

        /// <summary>
        /// 点是否在线的两侧
        /// </summary>
        /// <param name="line"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        internal static bool IsDiffSide(this Line line, Point3d p1, Point3d p2)
        {
            return PointExtension.IsDiffSide(line.StartPoint, line.EndPoint, p1, p2);
        }

        /// <summary>
        /// 点是否在线的左边
        /// </summary>
        /// <param name="line"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal static bool IsLeftSide(this Line line, Point3d point)
        {
            Point3d startPos = line.StartPoint;
            Point3d endPos = line.EndPoint;

            double a = endPos.Y - startPos.Y;
            if (Math.Abs(a) < 1e-6)
                return startPos.Y > point.Y;

            double b = startPos.X - endPos.X;
            double c = endPos.X * startPos.Y - startPos.X * endPos.Y;
            double temp = a * point.X + b * point.Y + c;

            if (a < 0)
                temp = -temp;

            return temp < 1e-6;
        }

        internal static bool IsLeftOfVector(this Line line, Point3d point)
        {
            Vector3d vecAB = line.EndPoint - line.StartPoint;
            Vector3d vecAC = point - line.StartPoint;
            return vecAB.CrossProduct(vecAC).Z > 0;

            //Point3d startPos = line.StartPoint;
            //Point3d endPos = line.EndPoint;

            //double a = endPos.Y - startPos.Y;
            //if (Math.Abs(a) < 1e-6)
            //    return startPos.Y > point.Y;

            //double b = startPos.X - endPos.X;
            //double c = endPos.X * startPos.Y - startPos.X * endPos.Y;
            //double temp = a * point.X + b * point.Y + c;

            //if (a < 0)
            //    temp = -temp;

            //return temp < 1e-6;
        }

        /// <summary>
        /// 比较两条线，从左到右
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        internal static int CompareLine(this Line line1, Line line2)
        {
            //if (Math.Abs(a.X - b.X) > 1e-6)
            //    return a.X < b.X ? -1 : 1;
            //return a.Y < b.Y ? -1 : 1;

            if (line1 == line2)
            {
                return 0;
            }

            return IsLeftSide(line2, line1.MidPoint()) ? -1 : 1;
        }

        /// <summary>
        /// 将线从左到右排序
        /// </summary>
        /// <param name="lines"></param>
        internal static void SortLines(List<Line> lines)
        {
            lines.Sort(CompareLine);
        }

        /// <summary>
        /// 延长线
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        internal static void Extended(this Line line, double len)
        {
            Point3d s = line.StartPoint + (line.StartPoint - line.EndPoint).GetNormal() * len;
            Point3d e = line.EndPoint + (line.EndPoint - line.StartPoint).GetNormal() * len;
            line.StartPoint = s;
            line.EndPoint = e;
        }

        /// <summary>
        /// 延长线
        /// </summary>
        /// <param name="line"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        internal static Line GetExtendedLine(this Line line, double len)
        {
            Point3d s = line.StartPoint + (line.StartPoint - line.EndPoint).GetNormal() * len;
            Point3d e = line.EndPoint + (line.EndPoint - line.StartPoint).GetNormal() * len;
            return new Line(s, e);
        }

        /// <summary>
        /// 偏移曲线
        /// </summary>
        /// <param name="line"></param>
        /// <param name="offsetDist"></param>
        /// <returns></returns>
        internal static Line OffsetLine(this Line line, double offsetDist, Point3d sameSidePt)
        {
            Line tempLine = line.GetOffsetCurves(offsetDist).OfType<Line>().FirstOrDefault();
            if (IsDiffSide(line, tempLine.StartPoint, sameSidePt))
            {
                return line.GetOffsetCurves(-offsetDist).OfType<Line>().FirstOrDefault();
            }
            else
            {
                return tempLine;
            }
        }

        /// <summary>
        /// 偏移线段
        /// </summary>
        /// <param name="line"></param>
        /// <param name="offsetDist">偏移距离</param>
        /// <param name="sidePt">基准点</param>
        /// <param name="isDiffSide">同侧或不同侧</param>
        /// <returns></returns>
        internal static Line Offset(this Line line, double offsetDist, Point3d sidePt, bool isDiffSide)
        {
            Line line1 = line.GetOffsetCurves(offsetDist).OfType<Line>().FirstOrDefault();
            if (line1 == null)
                return null;

            Line line2 = line.GetOffsetCurves(-offsetDist).OfType<Line>().FirstOrDefault();
            if (line2 == null)
                return null;

            bool isDiff = IsDiffSide(line, line1.StartPoint, sidePt);

            if (isDiff && isDiffSide || !isDiff && !isDiffSide)
                return line1;
            else
                return line2;
        }

        internal static Point3d? GetSamePoint(this Line line1, Line line2)
        {
            if (line1.StartPoint.DistanceTo(line2.StartPoint) < 1e-6 || line1.StartPoint.DistanceTo(line2.EndPoint) < 1e-6)
                return line1.StartPoint;
            else if (line1.EndPoint.DistanceTo(line2.StartPoint) < 1e-6 || line1.EndPoint.DistanceTo(line2.EndPoint) < 1e-6)
                return line1.EndPoint;

            return null;
        }

        /// <summary>
        /// 将有连接的线合并成一条线
        /// </summary>
        /// <param name="allLines"></param>
        /// <param name="errorValue"></param>
        internal static void MergeIsLinkAndCollinearLines(List<Line> allLines, double errorValue)
        {
            for (int i = 0; i < allLines.Count; i++)
            {
                List<Line> tempLines = new List<Line>() { allLines[i] };
                int s = 0;

                while (s < tempLines.Count)
                {
                    for (int j = 0; j < allLines.Count; j++)
                    {
                        if (tempLines.Contains(allLines[j]))
                            continue;

                        if (IsParallel(tempLines[s], allLines[j]) &&
                            !IsNonConnect(tempLines[s], allLines[j], errorValue))
                            tempLines.Add(allLines[j]);
                    }
                    s++;
                }

                if (tempLines.Count == 1)
                    continue;

                for (int j = 1; j < tempLines.Count; j++)
                    allLines.Remove(tempLines[j]);

                allLines[i] = GetMergeLongestLine(tempLines);
            }
        }

        /// <summary>
        /// 两条线之间没有连接关系
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        internal static bool IsNonConnect(this Line lineA, Line lineB, double errorValue)
        {
            return GetMinDistance(lineA, lineB) > errorValue;
        }

        /// <summary>
        /// 两条线是否连接
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        internal static bool IsConnect(this Line lineA, Line lineB, double errorValue)
        {
            return GetMinDistance(lineA, lineB) < errorValue;
        }

        /// <summary>
        /// 将直线组合并成一条线
        /// </summary>
        /// <param name="collinearLines"></param>
        /// <returns></returns>
        internal static Line GetMergeLongestLine(this List<Line> collinearLines)
        {
            if (collinearLines.Count == 0)
                return null;

            List<Point3d> points = new List<Point3d>();
            for (int i = 0; i < collinearLines.Count; i++)
            {
                points.Add(collinearLines[i].StartPoint);
                points.Add(collinearLines[i].EndPoint);
            }

            if (IsVerticalLineGroup(collinearLines))
            {
                List<Point3d> temp = points.OrderByDescending(x => x.Y).ToList();
                return new Line(temp.First(), temp.Last()) { };
            }
            else
            {
                List<Point3d> temp = points.OrderByDescending(x => x.X).ToList();
                return new Line(temp.First(), temp.Last()) { };
            }
        }

        /// <summary>
        /// 判断是否是垂直线组
        /// </summary>
        /// <param name="collinearLines"></param>
        /// <returns></returns>
        internal static bool IsVerticalLineGroup(this List<Line> collinearLines)
        {
            double maxLen = collinearLines[0].Length;
            int index = 0;
            for (int i = 1; i < collinearLines.Count; i++)
            {
                if (maxLen < collinearLines[i].Length)
                {
                    maxLen = collinearLines[i].Length;
                    index = i;
                }
            }

            return Math.Abs(collinearLines[index].Angle % Math.PI - Math.PI / 2) < 0.01;
        }

        /// <summary>
        /// 删除相同线
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        internal static List<Line> RemoveSameLines(this List<Line> lines, double errorValue)
        {
            List<List<Line>> lineGroups = new List<List<Line>>();
            for (int i = 0; i < lines.Count; i++)
            {
                bool isFind = false;
                for (int j = 0; j < lineGroups.Count; j++)
                {
                    if (Math.Abs(lines[i].Length - lineGroups[j].First().Length) < errorValue && IsParallel(lines[i], lineGroups[j].First(), errorValue))
                    {
                        lineGroups[j].Add(lines[i]);
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    lineGroups.Add(new List<Line>() { lines[i] });
                }
            }

            List<Line> result = new List<Line>();
            for (int i = 0; i < lineGroups.Count; i++)
            {
                RemoveSubSameLines(lineGroups[i], errorValue);
                result.AddRange(lineGroups[i]);
            }

            return result;
        }

        internal static void RemoveSubSameLines(this List<Line> lines, double errorValue)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    //if (Math.Abs( lines[i].Length - lines[j].Length) > errorValue)
                    //    continue;

                    if (IsSameLine(lines[i], lines[j], errorValue))
                    {
                        lines.RemoveAt(j);
                        --j;
                    }
                }
            }
        }

        /// <summary>
        /// 两条线相同
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="certainty"></param>
        /// <returns></returns>
        internal static bool IsSameLine(this Line line1, Line line2, double certainty)
        {
            if (line1 == null || line2 == null)
                return false;

            return line1.StartPoint.DistanceTo(line2.StartPoint) < certainty &&
                   line1.EndPoint.DistanceTo(line2.EndPoint) < certainty
                   || line1.StartPoint.DistanceTo(line2.EndPoint) < certainty &&
                   line1.EndPoint.DistanceTo(line2.StartPoint) < certainty;
        }

        /// <summary>
        /// 两条共线直线搭接
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <param name="errorValue"></param>
        internal static bool IsLapJoint(this Line lineA, Line lineB, double errorValue)
        {
            if (!IsParallel(lineA, lineB) || lineA.Length < errorValue || lineB.Length < errorValue)
                return false;

            return (CurveExtension.IsPointInCurve(lineB, lineA.StartPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineA.StartPoint, lineB.StartPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineA.StartPoint, lineB.EndPoint, errorValue)
                    || CurveExtension.IsPointInCurve(lineB, lineA.EndPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineA.EndPoint, lineB.StartPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineA.EndPoint, lineB.EndPoint, errorValue)
                    || CurveExtension.IsPointInCurve(lineA, lineB.StartPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineB.StartPoint, lineA.StartPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineB.StartPoint, lineA.EndPoint, errorValue)
                    || CurveExtension.IsPointInCurve(lineA, lineB.EndPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineB.EndPoint, lineA.StartPoint, errorValue) &&
                    !PointExtension.IsSamePoint(lineB.EndPoint, lineA.EndPoint, errorValue)
                    || IsSameLine(lineA, lineB, errorValue));
        }

        /// <summary>
        /// 判断线是否是垂直线
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static bool IsVertical(this Line line)
        {
            return Math.Abs(line.Angle % Math.PI - Math.PI / 2) < 0.01;
        }

        /// <summary>
        /// 相交线段求交点
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <param name="certainty"></param>
        /// <returns></returns>
        internal static List<Point3d> GetIntersectPoints(this Line lineA, Line lineB, double certainty)
        {
            List<Point3d> pts = new List<Point3d>();
            if (IsParallel(lineA, lineB, 0.01))
                return pts;

            double A1 = lineA.StartPoint.Y - lineA.EndPoint.Y;
            double B1 = lineA.EndPoint.X - lineA.StartPoint.X;
            double C1 = lineA.StartPoint.X * lineA.EndPoint.Y - lineA.EndPoint.X * lineA.StartPoint.Y;
            double A2 = lineB.StartPoint.Y - lineB.EndPoint.Y;
            double B2 = lineB.EndPoint.X - lineB.StartPoint.X;
            double C2 = lineB.StartPoint.X * lineB.EndPoint.Y - lineB.EndPoint.X * lineB.StartPoint.Y;
            double D = A1 * B2 - A2 * B1;

            if (Math.Abs(D) < 1e-6)
                return pts;

            Point3d intersectPoint = new Point3d((B1 * C2 - B2 * C1) / D, (A2 * C1 - A1 * C2) / D, 0);

            if (lineA.GetClosestPointTo(intersectPoint, false).DistanceTo(intersectPoint) < certainty
                && lineB.GetClosestPointTo(intersectPoint, false).DistanceTo(intersectPoint) < certainty)
                pts.Add(intersectPoint);

            return pts;
        }

        /// <summary>
        /// 得到两条直线之间的距离
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <returns></returns>
        internal static double GetMinDistance(this Line lineA, Line lineB)
        {
            double dis1 = lineA.GetClosestPointTo(lineB.StartPoint, false).DistanceTo(lineB.StartPoint);
            double dis2 = lineA.GetClosestPointTo(lineB.EndPoint, false).DistanceTo(lineB.EndPoint);
            double dis3 = lineB.GetClosestPointTo(lineA.StartPoint, false).DistanceTo(lineA.StartPoint);
            double dis4 = lineB.GetClosestPointTo(lineA.EndPoint, false).DistanceTo(lineA.EndPoint);

            return Math.Min(Math.Min(dis1, dis2), Math.Min(dis3, dis4));
        }

        /// <summary>
        /// 判断两条线是相交
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <returns></returns>
        internal static bool IsIntersect(this Line lineA, Line lineB)
        {
            /*
               快速排斥：
               两个线段为对角线组成的矩形，如果这两个矩形没有重叠的部分，那么两条线段是不可能出现重叠的
               */
            Point3d a = lineA.StartPoint;
            Point3d b = lineA.EndPoint;
            Point3d c = lineB.StartPoint;
            Point3d d = lineB.EndPoint;
            if (!(Math.Min(a.X, b.X) <= Math.Max(c.X, d.X) && Math.Min(c.Y, d.Y) <= Math.Max(a.Y, b.Y) && Math.Min(c.X, d.X) <= Math.Max(a.X, b.X) && Math.Min(a.Y, b.Y) <= Math.Max(c.Y, d.Y)))
                //这里的确如此，这一步是判定两矩形是否相交
                //1.线段ab的低点低于cd的最高点（可能重合） 2.cd的最左端小于ab的最右端（可能重合）
                //3.cd的最低点低于ab的最高点（加上条件1，两线段在竖直方向上重合） 4.ab的最左端小于cd的最右端（加上条件2，两直线在水平方向上重合）
                //综上4个条件，两条线段组成的矩形是重合的
                /*特别要注意一个矩形含于另一个矩形之内的情况*/
                return false;

            /*
             跨立实验：
             如果两条线段相交，那么必须跨立，就是以一条线段为标准，另一条线段的两端点一定在这条线段的两段
             也就是说a b两点在线段cd的两端，c d两点在线段ab的两端
             */
            double u, v, w, z;//分别记录两个向量
            u = (c.X - a.X) * (b.Y - a.Y) - (b.X - a.X) * (c.Y - a.Y);
            v = (d.X - a.X) * (b.Y - a.Y) - (b.X - a.X) * (d.Y - a.Y);
            w = (a.X - c.X) * (d.Y - c.Y) - (d.X - c.X) * (a.Y - c.Y);
            z = (b.X - c.X) * (d.Y - c.Y) - (d.X - c.X) * (b.Y - c.Y);

            return u * v <= 1e-6 && w * z <= 1e-6;
        }

        //internal static bool IsInsert(this Line l1, Line l2)
        //{
        //    //快速排斥实验
        //    if ((l1.StartPoint.X > l1.EndPoint.X ? l1.StartPoint.X : l1.EndPoint.X) < (l2.StartPoint.X < l2.EndPoint.X ? l2.StartPoint.X : l2.EndPoint.X) ||
        //        (l1.StartPoint.Y > l1.EndPoint.Y ? l1.StartPoint.Y : l1.EndPoint.Y) < (l2.StartPoint.Y < l2.EndPoint.Y ? l2.StartPoint.Y : l2.EndPoint.Y) ||
        //        (l2.StartPoint.X > l2.EndPoint.X ? l2.StartPoint.X : l2.EndPoint.X) < (l1.StartPoint.X < l1.EndPoint.X ? l1.StartPoint.X : l1.EndPoint.X) ||
        //        (l2.StartPoint.Y > l2.EndPoint.Y ? l2.StartPoint.Y : l2.EndPoint.Y) < (l1.StartPoint.Y < l1.EndPoint.Y ? l1.StartPoint.Y : l1.EndPoint.Y))
        //    {
        //        return false;
        //    }
        //    //跨立实验
        //    if ((((l1.StartPoint.X - l2.StartPoint.X) * (l2.EndPoint.Y - l2.StartPoint.Y) - (l1.StartPoint.Y - l2.StartPoint.Y) * (l2.EndPoint.X - l2.StartPoint.X)) *
        //        ((l1.EndPoint.X - l2.StartPoint.X) * (l2.EndPoint.Y - l2.StartPoint.Y) - (l1.EndPoint.Y - l2.StartPoint.Y) * (l2.EndPoint.X - l2.StartPoint.X))) > 0 ||
        //        (((l2.StartPoint.X - l1.StartPoint.X) * (l1.EndPoint.Y - l1.StartPoint.Y) - (l2.StartPoint.Y - l1.StartPoint.Y) * (l1.EndPoint.X - l1.StartPoint.X)) *
        //        ((l2.EndPoint.X - l1.StartPoint.X) * (l1.EndPoint.Y - l1.StartPoint.Y) - (l2.EndPoint.Y - l1.StartPoint.Y) * (l1.EndPoint.X - l1.StartPoint.X))) > 0)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 判断点是否在线上, 或者线段的延长线上
        /// </summary>
        /// <param name="p3d"></param>
        /// <param name="tagerLine"></param>
        /// <returns></returns>
        internal static bool IsOnLineExtend(this Line line, Point3d p3d, double tolerance = 1e-6)
        {
            Tolerance tolerance1 = new Tolerance(tolerance, tolerance);
            return (p3d - line.StartPoint).IsParallelTo(line.Normal, tolerance1);
        }

        /// <summary>
        /// 判断点是否是线段的端点
        /// </summary>
        /// <param name="p"></param>
        /// <param name="line"></param>
        /// <param name="certainty"></param>
        /// <returns></returns>
        internal static bool IsPointOnLineEndPoint(this Line line, Point3d p, double certainty)
        {
            return (p.DistanceTo(line.StartPoint) < certainty || p.DistanceTo(line.EndPoint) < certainty);
        }

        /// <summary>
        /// 将直线转化成多段线
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static Polyline ToPolyline(this Line line)
        {
            Polyline polyline = new Polyline();
            polyline.AddVertexAt(0, line.StartPoint.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(1, line.EndPoint.ToPoint2d(), 0, 0, 0);
            return polyline;
        }

        /// <summary>
        /// 得到多段线重心
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        internal static Point3d GetBarycenter(this Polyline polyline)
        {
            return PointExtension.GetBarycenter(polyline.GetPoint3ds());
        }

        /// <summary>
        /// 获取两条线段重合部分
        /// </summary>
        /// <param name="line1">线段1</param>
        /// <param name="line2">线段2</param>
        /// <returns>重合部分</returns>
        internal static Line Coincide(this Line line1, Line line2)
        {
            LineSegment3d l1 = new LineSegment3d(line1.StartPoint, line1.EndPoint);
            LineSegment3d l2 = new LineSegment3d(line2.StartPoint, line2.EndPoint);
            LinearEntity3d l3 = l1.Overlap(l2);
            return new Line(l3.StartPoint, l3.EndPoint);
        }

        /// <summary>
        /// 判断线段是否重合
        /// </summary>
        /// <param name="line1">线段1</param>
        /// <param name="line2">线段2</param>
        /// <param name="Allowance">容差</param>
        /// <returns>线段是否重合</returns>
        internal static bool IsCoincide(this Line line1, Line line2, double allowance = 1e-6)
        {
            LineSegment3d l1 = new LineSegment3d(line1.StartPoint, line1.EndPoint);
            LineSegment3d l2 = new LineSegment3d(line2.StartPoint, line2.EndPoint);
            Tolerance tol = new Tolerance(allowance, allowance);
            return l1.IsColinearTo(l2, tol);
        }

        internal static LineSegment3d ToLineSegment3d(this Line line)
        {
            return new LineSegment3d(line.StartPoint, line.EndPoint);
        }

        internal static List<Point3d> GetSpiltPoints(this Line line, int segmentNum)
        {
            double len = line.Length / segmentNum;
            List<Point3d> pts = new List<Point3d>();
            Vector3d vector = (line.EndPoint - line.StartPoint).GetNormal();
            for (int i = 0; i < segmentNum - 1; i++)
            {
                pts.Add(line.StartPoint + len * (i + 1) * vector);
            }
            return pts;
        }

        /// <summary>
        /// 两条线面对面
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="lineB"></param>
        /// <param name="certainty"></param>
        /// <returns></returns>
        internal static bool IsRelative(Line lineA, Line lineB, double certainty)
        {
            Point3d aToBPointS = lineB.GetClosestPointTo(lineA.StartPoint, true);
            Point3d aToBPointE = lineB.GetClosestPointTo(lineA.EndPoint, true);
            Point3d bToAPointS = lineA.GetClosestPointTo(lineB.StartPoint, true);
            Point3d bToAPointE = lineA.GetClosestPointTo(lineB.EndPoint, true);

            return (CurveExtension.IsPointInCurve(lineB, aToBPointS, certainty) &&
                    !PointExtension.IsSamePoint(aToBPointS, lineB.StartPoint, certainty) &&
                    !PointExtension.IsSamePoint(aToBPointS, lineB.EndPoint, certainty)
                    || CurveExtension.IsPointInCurve(lineB, aToBPointE, certainty) &&
                    !PointExtension.IsSamePoint(aToBPointE, lineB.StartPoint, certainty) &&
                    !PointExtension.IsSamePoint(aToBPointE, lineB.EndPoint, certainty)
                    || CurveExtension.IsPointInCurve(lineA, bToAPointS, certainty) &&
                    !PointExtension.IsSamePoint(bToAPointS, lineA.StartPoint, certainty) &&
                    !PointExtension.IsSamePoint(bToAPointS, lineA.EndPoint, certainty)
                    || CurveExtension.IsPointInCurve(lineA, bToAPointE, certainty) &&
                    !PointExtension.IsSamePoint(bToAPointE, lineA.StartPoint, certainty) &&
                    !PointExtension.IsSamePoint(bToAPointE, lineA.EndPoint, certainty)
                    || IsSameLine(new Line(aToBPointS, aToBPointE), lineB, certainty));
        }
    }
}