//namespace Su.AutoCAD2Revit.Extension
//{
//    internal static class TeighaExtensions
//    {
//        internal static List<Curve> GetMiddleAxisCurves(IEnumerable<Curve> curves)
//        {
//            List<Curve> middleAxisCurves = new List<Curve>();

//            try
//            {
//                // 检查曲线是否形成闭合环
//                if (!IsClosedCurveLoop(curves))
//                    return middleAxisCurves;

//                // 获取曲线环所在的平面
//                Plane curvePlane = GetCurveLoopPlane(curves);
//                if (curvePlane == null)
//                    return middleAxisCurves;

//                Vector3d normal = curvePlane.Normal;
//                List<Solid3d> solids = new List<Solid3d>();
//                List<Point3d> boundaryEndPoints = GetBoundaryEndPoints(curves);

//                // 为每条曲线创建扫描实体
//                foreach (Curve curve in curves)
//                {
//                    // 创建三角形剖面
//                    using DBObjectCollection triangleProfile = CreateTriangleProfile(curve, normal);
//                    if (triangleProfile != null && triangleProfile.Count > 0)
//                    {
//                        // 使用当前曲线作为扫描路径
//                        DBObjectCollection pathCurves = [curve.Clone() as Curve];

//                        Solid3d solid = CreateSweepSolid(triangleProfile, pathCurves);
//                        if (solid != null)
//                        {
//                            solids.Add(solid);
//                        }

//                        // 清理路径曲线集合
//                        foreach (DBObject obj in pathCurves)
//                        {
//                            obj.Dispose();
//                        }
//                    }
//                }

//                // 合并所有实体
//                if (solids.Count > 0)
//                {
//                    using Solid3d unionSolid = UnionSolids(solids);
//                    if (unionSolid != null)
//                    {
//                        // 从合并实体的边中提取中轴线
//                        middleAxisCurves = ExtractProjectedCurvesFromSolid(unionSolid, curvePlane, curves, boundaryEndPoints);
//                    }
//                }

//                // 清理实体
//                foreach (var solid in solids)
//                {
//                    solid?.Dispose();
//                }
//            }
//            catch (System.Exception)
//            {
//                // 处理异常
//            }

//            return middleAxisCurves;
//        }

//        private static bool IsClosedCurveLoop(IEnumerable<Curve> curves)
//        {
//            if (curves == null || !curves.Any())
//                return false;

//            List<Point3d> points = new List<Point3d>();

//            foreach (Curve curve in curves)
//            {
//                points.Add(curve.StartPoint);
//                points.Add(curve.EndPoint);
//            }

//            // 检查首尾点是否重合（形成闭合环）
//            if (points.Count >= 2)
//            {
//                Point3d firstPoint = points.First();
//                Point3d lastPoint = points.Last();
//                return firstPoint.DistanceTo(lastPoint) < Tolerance.Global.EqualPoint;
//            }

//            return false;
//        }

//        private static Plane GetCurveLoopPlane(IEnumerable<Curve> curves)
//        {
//            try
//            {
//                // 收集所有点
//                List<Point3d> points = new List<Point3d>();
//                foreach (Curve curve in curves)
//                {
//                    points.Add(curve.StartPoint);
//                    points.Add(curve.EndPoint);
//                }

//                if (points.Count < 3)
//                    return null;

//                // 使用前三个不共线的点创建平面
//                Point3d p1 = points[0];
//                Point3d p2 = points[1];
//                Point3d p3 = points[2];

//                Vector3d v1 = p2 - p1;
//                Vector3d v2 = p3 - p1;
//                Vector3d normal = v1.CrossProduct(v2).GetNormal();

//                if (normal.Length > 1e-9)
//                {
//                    return new Plane(p1, normal);
//                }
//            }
//            catch
//            {
//                // 处理异常
//            }

//            // 默认返回XY平面
//            return new Plane(Point3d.Origin, Vector3d.ZAxis);
//        }

//        private static List<Point3d> GetBoundaryEndPoints(IEnumerable<Curve> curves)
//        {
//            List<Point3d> endPoints = new List<Point3d>();

//            foreach (Curve curve in curves)
//            {
//                endPoints.Add(curve.StartPoint);
//                endPoints.Add(curve.EndPoint);
//            }

//            return endPoints;
//        }

//        private static DBObjectCollection CreateTriangleProfile(Curve curve, Vector3d normal)
//        {
//            DBObjectCollection profileCurves = new DBObjectCollection();

//            try
//            {
//                Point3d curveStart = curve.StartPoint;
//                double curveLength = GetCurveLength(curve);

//                // 计算三角形尺寸
//                double triangleHeight = curveLength / 5;
//                double triangleBase = curveLength * 2.0;

//                // 获取曲线起点的切向量
//                Vector3d tangent = GetCurveTangentAtPoint(curve, curve.StartParam);

//                // 构建坐标系
//                Vector3d zAxis = tangent;
//                Vector3d xAxis = normal.CrossProduct(zAxis).GetNormal();

//                if (xAxis.Length < 0.001)
//                {
//                    xAxis = (Math.Abs(zAxis.Z) < 0.9)
//                        ? zAxis.CrossProduct(Vector3d.ZAxis).GetNormal()
//                        : zAxis.CrossProduct(Vector3d.XAxis).GetNormal();
//                }

//                Vector3d yAxis = zAxis.CrossProduct(xAxis).GetNormal();

//                // 创建等腰三角形的三个顶点
//                Point3d vertexA = curveStart;
//                Point3d vertexB = curveStart + xAxis * (triangleBase / 2) - yAxis * triangleHeight;
//                Point3d vertexC = curveStart - xAxis * (triangleBase / 2) - yAxis * triangleHeight;

//                // 创建三角形轮廓的三条边
//                Line line1 = new Line(vertexA, vertexB);
//                Line line2 = new Line(vertexB, vertexC);
//                Line line3 = new Line(vertexC, vertexA);

//                profileCurves.Add(line1);
//                profileCurves.Add(line2);
//                profileCurves.Add(line3);

//                return profileCurves;
//            }
//            catch
//            {
//                // 清理已创建的曲线
//                foreach (DBObject obj in profileCurves)
//                {
//                    obj.Dispose();
//                }
//                return null;
//            }
//        }

//        private static Vector3d GetCurveTangentAtPoint(Curve curve, double parameter)
//        {
//            if (curve is Line line)
//            {
//                return (line.EndPoint - line.StartPoint).GetNormal();
//            }
//            else if (curve is Arc arc)
//            {
//                // 计算圆弧在起点的切线方向
//                double angle = arc.StartAngle;
//                Vector3d radialVector = new Vector3d(Math.Cos(angle), Math.Sin(angle), 0);
//                Vector3d tangent = radialVector.CrossProduct(arc.Normal).GetNormal();
//                return tangent;
//            }

//            // 默认返回X轴方向
//            return Vector3d.XAxis;
//        }

//        private static Solid3d CreateSweepSolid(DBObjectCollection profileCurves, DBObjectCollection pathCurves)
//        {
//            try
//            {
//                Solid3d solid = new Solid3d();

//                // 创建剖面区域
//                using (DBObjectCollection regions = Region.CreateFromCurves(profileCurves))
//                {
//                    if (regions.Count > 0 && regions[0] is Region profileRegion)
//                    {
//                        // 使用拉伸创建实体（沿路径方向）
//                        double height = CalculatePathsLength(pathCurves);
//                        solid.Extrude(profileRegion, height, 0);

//                        // 清理剖面区域
//                        foreach (DBObject obj in regions)
//                        {
//                            obj.Dispose();
//                        }
//                    }
//                }

//                return solid;
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        private static double CalculatePathsLength(DBObjectCollection pathCurves)
//        {
//            double totalLength = 0;
//            foreach (DBObject obj in pathCurves)
//            {
//                if (obj is Curve curve)
//                {
//                    totalLength += GetCurveLength(curve);
//                }
//            }
//            return totalLength / 10; // 返回较小的长度值
//        }

//        private static Solid3d UnionSolids(List<Solid3d> solids)
//        {
//            if (solids == null || solids.Count == 0)
//                return null;

//            if (solids.Count == 1)
//                return solids[0].Clone() as Solid3d;

//            Solid3d unionSolid = solids[0].Clone() as Solid3d;

//            try
//            {
//                for (int i = 1; i < solids.Count; i++)
//                {
//                    unionSolid.BooleanOperation(BooleanOperationType.BoolUnite, solids[i]);
//                }
//                return unionSolid;
//            }
//            catch
//            {
//                unionSolid.Dispose();
//                return null;
//            }
//        }

//        private static List<Curve> ExtractProjectedCurvesFromSolid(Solid3d solid, Plane targetPlane, IEnumerable<Curve> boundaryCurves, List<Point3d> boundaryEndPoints)
//        {
//            List<Curve> middleAxisCurves = [];
//            try
//            {
//                // 获取实体的所有边
//                using DBObjectCollection explodedObjects = new DBObjectCollection();
//                solid.Explode(explodedObjects);
//                foreach (DBObject obj in explodedObjects)
//                {
//                    if (obj is Curve originalCurve)
//                    {
//                        // 投影曲线到目标平面
//                        Curve projectedCurve = originalCurve.GetOrthoProjectedCurve(targetPlane);
//                        if (projectedCurve != null)
//                        {
//                            // 检查投影后的曲线是否满足条件
//                            if (IsValidMiddleAxisCurve(projectedCurve, boundaryCurves, boundaryEndPoints))
//                            {
//                                middleAxisCurves.Add(projectedCurve);
//                            }
//                            else
//                            {
//                                projectedCurve.Dispose();
//                            }
//                        }
//                    }
//                    obj.Dispose();
//                }
//            }
//            catch
//            {
//                // 处理异常
//            }
//            return middleAxisCurves;
//        }

//        private static Point3d ProjectPointToPlane(Point3d point, Plane plane)
//        {
//            Vector3d toPoint = point - plane.PointOnPlane;
//            double distance = toPoint.DotProduct(plane.Normal);
//            return point - plane.Normal * distance;
//        }

//        private static bool IsValidMiddleAxisCurve(Curve curve, IEnumerable<Curve> boundaryCurves, List<Point3d> boundaryEndPoints)
//        {
//            try
//            {
//                // 检查曲线的两个端点是否在边界端点上
//                Point3d start = curve.StartPoint;
//                Point3d end = curve.EndPoint;

//                bool isStartOnBoundary = boundaryEndPoints.Any(bp =>
//                    bp.DistanceTo(start) < Tolerance.Global.EqualPoint);
//                bool isEndOnBoundary = boundaryEndPoints.Any(bp =>
//                    bp.DistanceTo(end) < Tolerance.Global.EqualPoint);

//                // 端点不能在边界上
//                if (isStartOnBoundary || isEndOnBoundary)
//                    return false;

//                // 检查曲线的两个端点是否都在边界曲线环内
//                if (!IsPointInsideCurveLoop(start, boundaryCurves) ||
//                    !IsPointInsideCurveLoop(end, boundaryCurves))
//                    return false;

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private static bool IsPointInsideCurveLoop(Point3d point, IEnumerable<Curve> curves)
//        {
//            // 使用射线法判断点是否在曲线环内
//            int crossings = 0;
//            Point2d testPoint = new Point2d(point.X, point.Y);

//            // 将曲线环转换为线段集合进行检测
//            List<Tuple<Point2d, Point2d>> segments = new List<Tuple<Point2d, Point2d>>();

//            foreach (Curve curve in curves)
//            {
//                // 只使用曲线的起点和终点创建线段
//                Point2d start = new Point2d(curve.StartPoint.X, curve.StartPoint.Y);
//                Point2d end = new Point2d(curve.EndPoint.X, curve.EndPoint.Y);
//                segments.Add(new Tuple<Point2d, Point2d>(start, end));
//            }

//            // 检查射线与所有线段的交点
//            foreach (var segment in segments)
//            {
//                Point2d p1 = segment.Item1;
//                Point2d p2 = segment.Item2;

//                // 检查射线与线段的交点
//                if (((p1.Y <= testPoint.Y) && (p2.Y > testPoint.Y)) ||
//                    ((p1.Y > testPoint.Y) && (p2.Y <= testPoint.Y)))
//                {
//                    double vt = (testPoint.Y - p1.Y) / (p2.Y - p1.Y);
//                    if (!double.IsNaN(vt) && !double.IsInfinity(vt))
//                    {
//                        double intersectX = p1.X + vt * (p2.X - p1.X);

//                        if (testPoint.X < intersectX)
//                        {
//                            crossings++;
//                        }
//                    }
//                }
//            }

//            return (crossings % 2) == 1; // 奇数次交叉表示在内部
//        }

//        // 使用反射获取Curve的Length属性
//        private static double GetCurveLength(Curve curve)
//        {
//            try
//            {
//                // 使用反射获取Length属性
//                var lengthProperty = curve.GetType().GetProperty("Length");
//                if (lengthProperty != null && lengthProperty.CanRead)
//                {
//                    return (double)lengthProperty.GetValue(curve);
//                }
//                return curve.StartPoint.DistanceTo(curve.EndPoint);
//            }
//            catch
//            {
//                // 如果都失败，使用起点和终点的距离作为近似值
//                return curve.StartPoint.DistanceTo(curve.EndPoint);
//            }
//        }

//        /// <summary>
//        /// CAD点转Revit点
//        /// </summary>
//        /// <param name="point"></param>
//        /// <returns></returns>
//        private static Autodesk.Revit.DB.XYZ ConverCADPointToRevitPoint(Point3d point)
//        {
//            return new Autodesk.Revit.DB.XYZ(point.X / MillimetersPerFoot, point.Y / MillimetersPerFoot, point.Z / MillimetersPerFoot);
//        }

//        private const double MillimetersPerFoot = 304.800609601;
//    }
//}