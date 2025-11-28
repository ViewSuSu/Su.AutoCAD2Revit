namespace Su.AutoCAD2Revit.Extension
{
    internal static class CurveExtension
    {
        internal static Polyline GetBoundary(this Wipeout wipeout)
        {
            List<Point3d> points = wipeout.GetVertices().OfType<Point3d>().ToList();
            Polyline polyline = new Polyline();
            for (int i = 0; i < points.Count; i++)
            {
                polyline.AddVertexAt(i, points[i].ToPoint2d(), 0, 0, 0);
            }

            polyline.Closed = true;

            return polyline;
        }

        internal static Polyline GetBoundary(this HatchLoop hatchLoop)
        {
            if (hatchLoop == null)
                return null;

            if (!hatchLoop.IsPolyline)
            {
                Curve2dCollection curves = hatchLoop.Curves;
                if (curves.Count == 0)
                    return null;

                var lines = curves.OfType<LineSegment2d>().ToList();
                if (lines.Count == 0)
                    return null;

                Polyline polyline = new Polyline();
                for (int i = 0; i < lines.Count; i++)
                    polyline.AddVertexAt(i, lines[i].StartPoint, 0, 0, 0);

                polyline.Closed = true;
                return polyline;
            }
            else
            {
                Polyline polyline = new Polyline();
                for (int i = 0; i < hatchLoop.Polyline.Count; i++)
                    polyline.AddVertexAt(i, hatchLoop.Polyline[i].Vertex, 0, 0, 0);

                polyline.Closed = true;
                return polyline;
            }
        }

        /// <summary>
        /// 通过点集合得到多段线， 点集合必须确定可以转换才可以
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        internal static Polyline GetClosePolyline(List<Point2d> points)
        {
            Polyline poly = new Polyline();
            for (int i = 0; i < points.Count; i++)
                poly.AddVertexAt(i, points[i], 0, 0, 0);

            poly.Closed = true;
            return poly;
        }

        /// <summary>
        /// 通过点集合得到多段线， 点集合必须确定可以转换才可以
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        internal static Polyline GetClosePolyline(this List<Point3d> points)
        {
            Polyline poly = new Polyline();
            for (int i = 0; i < points.Count; i++)
                poly.AddVertexAt(i, points[i].ToPoint2d(), 0, 0, 0);

            poly.Closed = true;
            return poly;
        }

        /// <summary>
        /// 获取多段线的各个边
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        internal static List<Line> GetSides(this Polyline polyline)
        {
            List<Line> lines = new List<Line>();
            for (int i = 0; i < polyline.NumberOfVertices - 1; i++)
            {
                Line line = new Line(polyline.GetPoint3dAt(i), polyline.GetPoint3dAt(i + 1));
                lines.Add(line);
            }

            if (polyline.Closed)
            {
                lines.Add(new Line(polyline.GetPoint3dAt(polyline.NumberOfVertices - 1), polyline.GetPoint3dAt(0)));
            }
            return lines;
        }

        /// <summary>
        /// 获取多段线的各个边
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        internal static List<LineSegment3d> GetSideSegments(this Polyline polyline)
        {
            List<LineSegment3d> lines = new List<LineSegment3d>();
            for (int i = 0; i < polyline.NumberOfVertices - 1; i++)
            {
                LineSegment3d line = new LineSegment3d(polyline.GetPoint3dAt(i), polyline.GetPoint3dAt(i + 1));
                lines.Add(line);
            }

            if (polyline.Closed)
            {
                lines.Add(new LineSegment3d(polyline.GetPoint3dAt(polyline.NumberOfVertices - 1), polyline.GetPoint3dAt(0)));
            }
            return lines;
        }

        /// <summary>
        /// 获取多段线的各个顶点
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        internal static List<Point2d> GetPoint2ds(this Polyline polyline)
        {
            List<Point2d> points = new List<Point2d>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
                points.Add(polyline.GetPoint2dAt(i));

            return points;
        }

        /// <summary>
        /// 获取多段线的各个顶点
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        internal static List<Point3d> GetPoint3ds(this Polyline polyline)
        {
            List<Point3d> points = new List<Point3d>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
                points.Add(polyline.GetPoint3dAt(i));

            return points;
        }

        /// <summary>
        /// 获取多段线的各个顶点
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        internal static Point3dCollection GetPoint3dCollection(this Polyline polyline)
        {
            Point3dCollection points = new Point3dCollection();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
                points.Add(polyline.GetPoint3dAt(i));

            return points;
        }

        /// <summary>
        /// 判断点在多段线内
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal static bool IsInPolyline(this Polyline polyline, Point3d point)
        {
            return IsInVertexes(polyline, point);
        }

        /// <summary>
        /// 判断点在多段线内
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal static bool IsInPolyline(this Polyline polyline, Point2d point)
        {
            return IsInVertexes(polyline, point.ToPoint3d());
        }

        /// <summary>
        /// 将多段线转化成线段与弧线
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        internal static List<Curve> ToCurves(this Polyline polyline)
        {
            List<Curve> curves = new List<Curve>();

            int n = polyline.NumberOfVertices - 1;
            if (polyline.Closed)
                n = polyline.NumberOfVertices;

            for (int i = 0; i < n; i++)
            {
                int succ = (i + 1) % polyline.NumberOfVertices;
                Point2d p1 = polyline.GetPoint2dAt(i);
                Point2d p2 = polyline.GetPoint2dAt(succ);
                double bulge = polyline.GetBulgeAt(i);

                if (bulge == 0)
                    curves.Add(new Line(p1.ToPoint3d(), p2.ToPoint3d()) { Layer = polyline.Layer });
                else
                {
                    Arc arc = GetArc(p1, p2, bulge);
                    arc.Layer = polyline.Layer;
                    curves.Add(arc);
                }
            }

            //if (polyline.Closed)
            //{
            //    Point2d p1 = polyline.GetPoint2dAt(polyline.NumberOfVertices - 1);
            //    Point2d p2 = polyline.GetPoint2dAt(0);
            //    double bulge = polyline.GetBulgeAt(polyline.NumberOfVertices - 1);
            //    if (bulge == 0)
            //        curves.Add(new Line(p1.ToPoint3d(), p2.ToPoint3d()) { Layer = polyline.Layer });
            //    else
            //    {
            //        Arc arc = GetArc(p1, p2, bulge);
            //        arc.Layer = polyline.Layer;
            //        curves.Add(arc);
            //    }
            //}

            return curves;
        }

        /// <summary>
        /// 获取Polylines的外包框
        /// </summary>
        /// <param name="polylines"></param>
        /// <returns></returns>
        internal static Extents3d GetGeometricExtents(this List<Polyline> polylines)
        {
            // 初始化外框变量
            Extents3d extents = new Extents3d();
            foreach (var polyline in polylines)
            {
                var ext = polyline.GeometricExtents;
                extents.AddExtents(ext);
            }
            return extents;
        }

        /// <summary>
        /// 获取extents边界
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="exDis">扩大或缩小距离</param>
        /// <returns></returns>
        internal static Polyline CreatePolyline(List<Point3d> pts, bool isClosed = true)
        {
            Polyline polyline = new Polyline();
            for (int i = 0; i < pts.Count; i++)
            {
                polyline.AddVertexAt(i, pts[i].ToPoint2d(), 0, 0, 0);
            }
            polyline.Closed = isClosed;

            return polyline;
        }

        /// <summary>
        /// 获取extents边界
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="exDis">扩大或缩小距离</param>
        /// <returns></returns>
        internal static Polyline CreatePolyline(List<Point2d> pts)
        {
            Polyline polyline = new Polyline();
            for (int i = 0; i < pts.Count; i++)
            {
                polyline.AddVertexAt(i, pts[i], 0, 0, 0);
            }
            polyline.Closed = true;

            return polyline;
        }

        internal static bool IsInVertexes(this Polyline pl, Point3d point)
        {
            bool oddNodes = false;

            for (int i = 0, j = pl.NumberOfVertices - 1; i < pl.NumberOfVertices; i++)
            {
                Point2d pI = pl.GetPoint2dAt(i);
                Point2d pJ = pl.GetPoint2dAt(j);

                if ((pI.Y < point.Y && pJ.Y >= point.Y ||
                     pJ.Y < point.Y && pI.Y >= point.Y) &&
                    (pI.X <= point.X || pJ.X <= point.X))
                {
                    oddNodes ^= (pI.X + (point.Y - pI.Y) / (pJ.Y - pI.Y) *
                                 (pJ.X - pI.X) < point.X);
                }

                j = i;
            }

            return oddNodes;
        }

        /// <summary>
        /// 找在多段线内部的点
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        internal static Point3d GetPointInPolyline(this Polyline polyline)
        {
            return PointExtension.GetPointInPolyline(polyline.GetPoint3ds());
        }

        /// 获取多段线
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        internal static Polyline GetPolyline(this Point3dCollection points, bool isclosed = true)
        {
            Polyline poly = new Polyline();
            for (int i = 0; i < points.Count; i++)
                poly.AddVertexAt(i, points[i].ToPoint2d(), 0, 0, 0);

            poly.Closed = isclosed;
            return poly;
        }

        internal static List<Line> GetAllLines(List<Curve> curves)
        {
            List<Line> allLines = new List<Line>();
            for (int i = 0; i < curves.Count; i++)
            {
                if (curves[i] is Line)
                {
                    Line line = curves[i] as Line;
                    allLines.Add(new Line(line.StartPoint, line.EndPoint));
                }
                else if (curves[i] is Polyline)
                {
                    Polyline polyline = curves[i] as Polyline;
                    for (int j = 0; j < polyline.NumberOfVertices - 1; j++)
                        allLines.Add(new Line(polyline.GetPoint3dAt(j), polyline.GetPoint3dAt(j + 1)));
                    if (polyline.Closed)
                        allLines.Add(new Line(polyline.GetPoint3dAt(polyline.NumberOfVertices - 1), polyline.GetPoint3dAt(0)));
                }
                else
                {
                    allLines.Add(new Line(curves[i].StartPoint, curves[i].EndPoint));
                }
            }
            return allLines;
        }

        /// <summary>
        /// 点是否在曲线内
        /// </summary>
        /// <param name="curve">曲线</param>
        /// <param name="point">点</param>
        /// <param name="certainty">误差</param>
        /// <returns></returns>
        internal static bool IsPointInCurve(Curve curve, Point3d point, double certainty)
        {
            return GetDistancePointToCurve(curve, point, false) <= certainty;
        }

        /// <summary>
        /// 点到曲线距离
        /// </summary>
        /// <param name="curve">曲线</param>
        /// <param name="point">点</param>
        /// <param name="extendLine">是否延长曲线</param>
        /// <returns>距离</returns>

        internal static double GetDistancePointToCurve(Curve curve, Point3d point, bool extendLine)
        {
            Point3d pt = curve.GetClosestPointTo(point, extendLine);
            return point.DistanceTo(pt);
        }

        /// <summary>
        /// 获取根据起点终点和凸度画圆弧
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="bulge"></param>
        /// <returns></returns>
        internal static Arc GetArc(Point2d p1, Point2d p2, double bulge)
        {
            Point2d center = PointExtension.GetArcBulgeCenter(p1, p2, bulge);
            double angle1 = (p1 - center).Angle;
            double angle2 = (p2 - center).Angle;
            Arc arc = null;
            if (bulge > 0)
                arc = new Arc(center.ToPoint3d(), p1.GetDistanceTo(center), angle1, angle2);
            else
                arc = new Arc(center.ToPoint3d(), p1.GetDistanceTo(center), angle2, angle1);
            return arc;
        }

        /// <summary>
        /// 偏移曲线
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="offsetDist"></param>
        /// <returns></returns>
        internal static Curve Offset(this Curve curve, double offsetDist)
        {
            return curve.GetOffsetCurves(offsetDist).OfType<Curve>().FirstOrDefault();
        }

        internal static List<Point3d> GetPointAtPolyLine(this Polyline polyline)
        {
            List<Point3d> points = new List<Point3d>();
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                points.Add(polyline.GetPoint3dAt(i));
            }
            return points;
        }

        /// <summary>
        /// 将curve合并成多段线，curve只支持圆弧，默认闭合
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        internal static Polyline GetPolyline(List<Curve> curves, double tol)
        {
            if (curves.Count <= 1)
                return null;
            Polyline pl = new Polyline();

            Point3d s = curves[0].StartPoint;
            if (s.DistanceTo(curves[1].StartPoint) < tol || s.DistanceTo(curves[1].EndPoint) < tol)
                s = curves[0].EndPoint;

            int curIndex = 0;
            for (int i = 0; i < curves.Count; i++)
            {
                Point3d e = PointExtension.GetAnotherPoint(s, curves[i], tol);
                if (curves[i] is Arc arc)
                {
                    double len = arc.GetDistAtPoint(arc.EndPoint);
                    Point3d midPt = arc.GetPointAtDist(len / 2);
                    double dBulge = PointExtension.GetArcBulge(s.ToPoint2d(), midPt.ToPoint2d(), e.ToPoint2d());
                    pl.AddVertexAt(curIndex, s.ToPoint2d(), dBulge, 0, 0);
                    curIndex++;
                }
                else if (curves[i] is Polyline polyline)
                {
                    if (s.DistanceTo(polyline.StartPoint) > tol)
                        polyline.ReverseCurve();

                    for (int j = 0; j < polyline.NumberOfVertices; j++)
                    {
                        pl.AddVertexAt(curIndex, polyline.GetPoint2dAt(j), polyline.GetBulgeAt(j), 0, 0);
                        curIndex++;
                    }
                }
                else if (curves[i] is Line line)
                {
                    pl.AddVertexAt(curIndex, s.ToPoint2d(), 0, 0, 0);
                    curIndex++;
                }

                s = e;
            }

            pl.Closed = true;
            return pl;
        }

        /// <summary>
        /// 得到两条多段线之间的最近距离
        /// </summary>
        /// <param name="polylineA"></param>
        /// <param name="polylineB"></param>
        /// <returns></returns>
        internal static double GetMinDistance(Polyline polylineA, Polyline polylineB)
        {
            List<Point3d> vertexAs = polylineA.GetPoint3ds();
            List<Point3d> vertexBs = polylineB.GetPoint3ds();

            double minDis = -1;
            for (int i = 0; i < vertexAs.Count; i++)
            {
                double dis = polylineB.GetClosestPointTo(vertexAs[i], false).DistanceTo(vertexAs[i]);
                if (minDis < -0.1 || minDis > dis)
                    minDis = dis;
            }

            for (int i = 0; i < vertexBs.Count; i++)
            {
                double dis = polylineA.GetClosestPointTo(vertexBs[i], false).DistanceTo(vertexBs[i]);
                if (minDis < -0.1 || minDis > dis)
                    minDis = dis;
            }

            return minDis;
        }

        /// <summary>
        /// 添加顶点，api是2d点，通常约定使用3d点
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="index"></param>
        /// <param name="pt"></param>
        /// <param name="bulge"></param>
        /// <param name="startWidth"></param>
        /// <param name="endWidth"></param>
        internal static void AddVertexAt(this Polyline polyline, int index, Point3d pt, double bulge, double startWidth, double endWidth)
        {
            polyline.AddVertexAt(index, pt.ToPoint2d(), bulge, startWidth, endWidth);
        }

        internal static bool IsClosed(this Polyline polyline)
        {
            if (polyline.Closed == true)
                return true;
            else if (polyline.StartPoint.DistanceTo(polyline.EndPoint) < 1)
                return true;
            else
                return false;
        }

        internal static bool HasLapJoinEdge(this Polyline pl1, Polyline pl2, double errorValue)
        {
            List<Line> line1s = pl1.GetSides();
            List<Line> line2s = pl2.GetSides();
            for (int i = 0; i < line1s.Count; i++)
            {
                for (int j = 0; j < line2s.Count; j++)
                {
                    if (line1s[i].IsLapJoint(line2s[j], errorValue))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool? IsLeftOfPolyline(this Polyline polyline, Point3d pt)
        {
            List<Point3d> vertexes = polyline.GetPoint3ds();
            Point3d minDisPt = vertexes.OrderBy(x => x.DistanceTo(pt)).First();
            int index = vertexes.IndexOf(minDisPt);

            if (index == 0)
            {
                Line line = new Line(vertexes[0], vertexes[1]);
                if (line.IsOnLine(line.GetClosestPointTo(pt, true), false, 1e-6))
                {
                    return line.IsLeftOfVector(pt);
                }
            }
            else if (index == vertexes.Count - 1)
            {
                Line line = new Line(vertexes[vertexes.Count - 2], vertexes[vertexes.Count - 1]);
                if (line.IsOnLine(line.GetClosestPointTo(pt, true), false, 1e-6))
                {
                    return line.IsLeftOfVector(pt);
                }
            }
            else
            {
                Line line1 = new Line(vertexes[index - 1], vertexes[index]);
                Line line2 = new Line(vertexes[index], vertexes[index + 1]);
                if (line1.IsOnLine(line1.GetClosestPointTo(pt, true), false, 1e-6))
                {
                    return line1.IsLeftOfVector(pt);
                }

                if (line1.IsOnLine(line2.GetClosestPointTo(pt, true), false, 1e-6))
                {
                    return line1.IsLeftOfVector(pt);
                }
            }

            return null;
        }
    }
}