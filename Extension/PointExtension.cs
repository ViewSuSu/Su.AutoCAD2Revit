using System.Runtime.CompilerServices;

namespace Su.AutoCAD2Revit.Extension
{
    /// <summary>
    /// Point
    /// </summary>
    internal static class PointUtils
    {
        /// <summary>
        /// 返回不等比例变换矩阵
        /// </summary>
        /// <param name="point">基点</param>
        /// <param name="x">x方向比例</param>
        /// <param name="y">y方向比例</param>
        /// <param name="z">z方向比例</param>
        /// <returns>三维矩阵</returns>
        internal static Matrix3d GetScaleMatrix(this Point3d point, double x, double y, double z)
        {
            double[] matdata = new double[16];
            matdata[0] = x;
            matdata[3] = point.X * (1 - x);
            matdata[5] = y;
            matdata[7] = point.Y * (1 - y);
            matdata[10] = z;
            matdata[11] = point.Z * (1 - z);
            matdata[15] = 1;
            return new Matrix3d(matdata);
        }

        /// <summary>
        /// 将点转换至Z坐标为0
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal static Point3d ToZ0(this Point3d point)
        {
            return new Point3d(point.X, point.Y, 0);
        }

        /// <summary>
        /// 取两个点的中点
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        internal static Point2d MidPoint(this Point2d point1, Point2d point2)
        {
            return new Point2d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
        }

        /// <summary>
        /// 取两个点的中点
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        internal static Point3d MidPoint(this Point3d point1, Point3d point2)
        {
            return new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
        }

        internal static Point3d ToPoint3d(this Point2d point)
        {
            return new Point3d(point.X, point.Y, 0);
        }

        internal static Point2d ToPoint2d(this Point3d point)
        {
            return new Point2d(point.X, point.Y);
        }

        /// <summary>
        /// 求两点之间的距离
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        internal static double DistanceTo(this Point2d point1, Point2d point2)
        {
            double a = Math.Pow(point2.X - point1.X, 2);//求x的y次方
            double b = Math.Pow(point2.Y - point1.Y, 2);
            return Math.Sqrt(a + b);
        }

        /// <summary>
        /// 两个点坐标是否近似相等
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        internal static bool IsPoint2dEqual(this Point2d point1, Point2d point2)
        {
            if (Math.Abs(point1.DistanceTo(point2)) < 0.001)
                return true;
            return false;
        }

        internal static bool IsExsit(this Point3d point2D, Point3d minPoint, Point3d maxPoint)
        {
            if (point2D.X >= minPoint.X && point2D.X <= maxPoint.X && point2D.Y >= minPoint.Y && point2D.Y <= maxPoint.Y)
                return true;
            return false;
        }

        internal static Point3d ToPoint3d(this Point2d point2D, double z = 0)
        {
            return new Point3d(point2D.X, point2D.Y, z);
        }

        /// <summary>
        /// p1/p2在以lineStartPt与lineEndPt所组成线的两侧
        /// </summary>
        /// <param name="lineStartPt"></param>
        /// <param name="lineEndPt"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        internal static bool IsDiffSide(this Point3d lineStartPt, Point3d lineEndPt, Point3d p1, Point3d p2)
        {
            Vector3d vec = lineEndPt - lineStartPt;
            Vector3d vec1 = p1 - lineStartPt;
            Vector3d vec2 = p2 - lineStartPt;

            return vec.CrossProduct(vec1).Z * vec.CrossProduct(vec2).Z < 0;
        }

        /// <summary>
        /// 删除点集中相同的点
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tol"></param>
        internal static void RemoveSamePts(this List<Point3d> points, double tol)
        {
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].DistanceTo(points[j]) < tol)
                    {
                        points.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        internal static void RemoveDuplicatePoints(this List<Point3d> allPoints, double tol = 1)
        {
            if (allPoints.Count == 0)
                return;

            for (int i = 0; i < allPoints.Count - 1; i++)
            {
                for (int j = i + 1; j < allPoints.Count; j++)
                {
                    if (IsSamePoint(allPoints[i], allPoints[j], tol))
                    {
                        allPoints.RemoveAt(j);
                        --j;
                    }
                }
            }
        }

        /// <summary>
        /// 两个点相同
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        internal static bool IsSamePoint(this Point3d pointA, Point3d pointB, double tol = 1e-5)
        {
            return pointA.DistanceTo(pointB) < tol;
        }

        internal static List<Point3d> GetConvexHull(List<Point3d> allPoints)
        {
            List<Point3d> tempPoints = new List<Point3d>(allPoints);
            if (allPoints.Count < 3)
                return tempPoints;

            List<Point3d> convexPoints = new List<Point3d>();
            RemoveDuplicatePoints(tempPoints);
            SortPointX(tempPoints);
            int total = 0;
            List<int> convexIndex = new List<int>();
            for (int i = 0; i < tempPoints.Count; i++)
            {
                while ((total > 1) && CrossProduct(tempPoints[convexIndex[total - 1]], tempPoints[convexIndex[total - 2]], tempPoints[i], tempPoints[convexIndex[total - 1]]) <= 0)
                    total--;

                if (total >= convexIndex.Count)
                    convexIndex.Add(i);
                else
                    convexIndex[total] = i;
                total++;
            }

            int temp = total;
            for (int i = tempPoints.Count - 2; i >= 0; --i)
            {
                while ((total > temp) && CrossProduct(tempPoints[convexIndex[total - 1]], tempPoints[convexIndex[total - 2]], tempPoints[i], tempPoints[convexIndex[total - 1]]) <= 0)
                    total--;

                if (total >= convexIndex.Count)
                    convexIndex.Add(i);
                else
                    convexIndex[total] = i;
                total++;
            }

            for (int i = 0; i < total; i++)
                convexPoints.Add(tempPoints[convexIndex[i]]);

            return convexPoints;
        }

        /// <summary>
        /// 判断两点x的坐标大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static int ComparePointX(this Point3d a, Point3d b)
        {
            if (Math.Abs(a.X - b.X) > 1e-6)
                return a.X < b.X ? -1 : 1;
            return a.Y < b.Y ? -1 : 1;
        }

        /// <summary>
        /// 根据X坐标排序点
        /// </summary>
        /// <param name="points"></param>
        internal static void SortPointX(this List<Point3d> points)
        {
            points.Sort(ComparePointX);
        }

        internal static double CrossProduct(this Point3d aa, Point3d bb, Point3d cc, Point3d dd)
        {
            Vector3d vec1 = bb - aa;
            Vector3d vec2 = dd - cc;
            return vec1.CrossProduct(vec2).Z;

            //return vec1.C
            //return (bb.X - aa.X) * (dd.Y - cc.Y) - (bb.Y - aa.Y) * (dd.X - cc.X);
        }

        //internal static double CrossProduct1(Point3d aa, Point3d bb, Point3d cc, Point3d dd)
        //{
        //    //Vector3d vec1 = bb - aa;
        //    //Vector3d vec2 = dd - cc;
        //    //return vec1.CrossProduct(vec2).Z;

        //    //return vec1.C
        //    return (bb.X - aa.X) * (dd.Y - cc.Y) - (bb.Y - aa.Y) * (dd.X - cc.X);
        //}

        /// <summary>
        /// 去掉相同点
        /// </summary>
        /// <param name="vertexes"></param>
        /// <param name="errorValue"></param>
        internal static void RemoveSamePoint(this List<Point3d> vertexes, double errorValue)
        {
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (vertexes[i].DistanceTo(vertexes[(i + 1) % vertexes.Count]) < errorValue)
                {
                    vertexes.RemoveAt(i);
                    --i;
                }
            }
        }

        /// <summary>
        /// 去掉共线点
        /// </summary>
        /// <param name="vertexes"></param>
        /// <param name="errorValue"></param>
        internal static void RemoveCollinearPoint(this List<Point3d> vertexes, double errorValue)
        {
            RemoveSamePoint(vertexes, errorValue);
            for (int i = 0; i < vertexes.Count; i++)
            {
                Vector3d vec1 = vertexes[i] - vertexes[(i - 1 + vertexes.Count) % vertexes.Count];
                Vector3d vec2 = vertexes[(i + 1) % vertexes.Count] - vertexes[i];

                if (Math.Abs(vec1.CrossProduct(vec2).Z) / vec1.Length / vec2.Length < 0.01)
                {
                    vertexes.RemoveAt(i);
                    --i;
                }
            }
        }

        /// <summary>
        /// 圆弧的腰点
        /// </summary>
        /// <param name="arc1">圆弧点1</param>
        /// <param name="arc3">圆弧点3</param>
        /// <param name="bulge">凸度</param>
        /// <returns>返回腰点</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl]
        internal static Point2d GetArcMidPoint(this Point2d arc1, Point2d arc3, double bulge)
        {
            if (bulge == 0)
                throw new ArgumentException("凸度为0,此线是平的");

            var center = GetArcBulgeCenter(arc1, arc3, bulge);
            var angle1 = center.GetVectorTo(arc1).GetAngle2XAxis();
            var angle3 = center.GetVectorTo(arc3).GetAngle2XAxis();
            // 利用边点进行旋转,就得到腰点,旋转角/2
            // 需要注意镜像的多段线
            double angle = angle3 - angle1;
            if (bulge > 0)
            {
                if (angle < 0)
                    angle += Math.PI * 2;
            }
            else
            {
                if (angle > 0)
                    angle += Math.PI * 2;
            }
            return arc1.RotateBy(angle / 2, center);
        }

        /// http://bbs.xdcad.net/thread-722387-1-1.html
        /// https://blog.csdn.net/jiangyb999/article/details/89366912
        /// <summary>
        /// 凸度求圆心
        /// </summary>
        /// <param name="arc1">圆弧头点</param>
        /// <param name="arc3">圆弧尾点</param>
        /// <param name="bulge">凸度</param>
        /// <returns>圆心</returns>
        [MethodImpl]
        internal static Point2d GetArcBulgeCenter(this Point2d arc1, Point2d arc3, double bulge)
        {
            if (bulge == 0)
                throw new ArgumentException("凸度为0,此线是平的");

            var x1 = arc1.X;
            var y1 = arc1.Y;
            var x2 = arc3.X;
            var y2 = arc3.Y;

            var b = (1 / bulge - bulge) / 2;
            var x = (x1 + x2 - b * (y2 - y1)) / 2;
            var y = (y1 + y2 + b * (x2 - x1)) / 2;
            return new Point2d(x, y);
        }

        internal static bool IsInVertexes(this List<Point2d> points, Point3d point)
        {
            return IsInVertexes(points, new Point2d(point.X, point.Y));
        }

        internal static bool IsInVertexes(this List<Point2d> points, Point2d point)
        {
            bool oddNodes = false;

            for (int i = 0, j = points.Count - 1; i < points.Count; i++)
            {
                if ((points[i].Y < point.Y && points[j].Y >= point.Y ||
                     points[j].Y < point.Y && points[i].Y >= point.Y) &&
                    (points[i].X <= point.X || points[j].X <= point.X))
                {
                    oddNodes ^= (points[i].X + (point.Y - points[i].Y) / (points[j].Y - points[i].Y) *
                                 (points[j].X - points[i].X) < point.X);
                }

                j = i;
            }

            return oddNodes;
        }

        internal static bool IsInVertexes(this List<Point3d> points, Point3d point)
        {
            bool oddNodes = false;

            for (int i = 0, j = points.Count - 1; i < points.Count; i++)
            {
                if ((points[i].Y < point.Y && points[j].Y >= point.Y ||
                     points[j].Y < point.Y && points[i].Y >= point.Y) &&
                    (points[i].X <= point.X || points[j].X <= point.X))
                {
                    oddNodes ^= (points[i].X + (point.Y - points[i].Y) / (points[j].Y - points[i].Y) *
                                 (points[j].X - points[i].X) < point.X);
                }

                j = i;
            }

            return oddNodes;
        }

        /// <summary>
        /// 两点计算弧度范围0到2Pi
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns>弧度值</returns>
        internal static double GetAngle(this Point2d startPoint, Point2d endPoint)
        {
            return startPoint.GetVectorTo(endPoint).Angle;
        }

        /// <summary>
        /// 得到三角形面积
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        internal static double GetArea(this Point3d p0, Point3d p1, Point3d p2)
        {
            double area = p0.X * p1.Y + p1.X * p2.Y + p2.X * p0.Y - p1.X * p0.Y - p2.X * p1.Y - p0.X * p2.Y;
            return area / 2;
        }

        /// <summary>
        /// 得到多段线重心
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        internal static Point3d GetBarycenter(this List<Point3d> points)
        {
            if (points.Count == 1)
                return points[0];

            double sumX = 0;
            double sumY = 0;
            double sumArea = 0;
            Point3d p1 = points[1];

            for (int i = 2; i < points.Count; i++)
            {
                Point3d p2 = points[i];
                double area = GetArea(points[0], p1, p2);
                sumArea += area;
                sumX += (points[0].X + p1.X + p2.X) * area;
                sumY += (points[0].Y + p1.Y + p2.Y) * area;
                p1 = p2;
            }

            double x = sumX / sumArea / 3;
            double y = sumY / sumArea / 3;
            return new Point3d(x, y, 0);
        }

        /// <summary>
        /// 找在多段线内部的点
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        internal static Point3d GetPointInPolyline(this List<Point3d> polygon)
        {
            // 如果重心在多边形内，直接取重心
            Point3d barycenter = GetBarycenter(polygon);
            if (IsInVertexes(polygon, barycenter))
                return barycenter;

            Point3d v = polygon[0];
            int index = 0;
            int vCount = polygon.Count;
            for (int i = 0; i < vCount; i++)
            {
                if (polygon[i].Y < v.Y)
                {
                    v = polygon[i];
                    index = i;
                }
            }

            Point3d a = polygon[(index - 1 + vCount) % vCount];               //得到v的前一个顶点
            Point3d b = polygon[(index + 1) % vCount];                             //得到v的后一个顶点

            List<Point3d> tri = new List<Point3d> { a, v, b };
            bool isFind = false;
            double minDis = -1;
            Point3d q = Point3d.Origin;
            for (int i = 0; i < vCount; i++)        //寻找在三角形avb内且离顶点v最近的顶点q
            {
                if (i == index || i == (index - 1 + vCount) % vCount || i == (index + 1) % vCount)
                    continue;

                if (!IsInVertexes(tri, polygon[i]))
                    continue;

                isFind = true;
                double dis = polygon[i].DistanceTo(v);
                if (minDis == -1 || dis < minDis)
                {
                    q = polygon[i];
                    minDis = dis;
                }
            }

            Point3d result;
            if (!isFind)       //没有顶点在三角形avb内，返回线段ab中点
            {
                result = MidPoint(a, b);
            }
            else
            {
                result = MidPoint(v, q);         //返回线段vq的中点
            }
            return result;
        }

        /// http://www.lee-mac.com/bulgeconversion.html
        /// <summary>
        /// 求凸度,判断三点是否一条直线上
        /// </summary>
        /// <param name="arc1">圆弧起点</param>
        /// <param name="arc2">圆弧腰点</param>
        /// <param name="arc3">圆弧尾点</param>
        /// <returns>逆时针为正,顺时针为负</returns>
        internal static double GetArcBulge(this Point2d arc1, Point2d arc2, Point2d arc3, double tol = 1e-10)
        {
            double dStartAngle = arc2.GetAngle(arc1);
            double dEndAngle = arc2.GetAngle(arc3);
            // 求的P1P2与P1P3夹角
            var talAngle = (Math.PI - dStartAngle + dEndAngle) / 2;
            // 凸度==拱高/半弦长==拱高比值/半弦长比值
            // 有了比值就不需要拿到拱高值和半弦长值了,因为接下来是相除得凸度
            double bulge = Math.Sin(talAngle) / Math.Cos(talAngle);

            // 处理精度
            if (bulge > 0.9999 && bulge < 1.0001)
                bulge = 1;
            else if (bulge < -0.9999 && bulge > -1.0001)
                bulge = -1;
            else if (Math.Abs(bulge) < tol)
                bulge = 0;
            return bulge;
        }

        internal static Point3d GetAnotherPoint(this Point3d point, Teigha.DatabaseServices.Curve curve, double tol)
        {
            if (point.DistanceTo(curve.StartPoint) < tol)
                return curve.EndPoint;
            else
                return curve.StartPoint;
        }

        /// <summary>
        /// 求点到面的正交投影（投影法向量为平面法向量）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planeOrigin"></param>
        /// <param name="planeNormal"></param>
        /// <returns></returns>
        internal static Point3d OrthoProject(this Point3d point, Point3d planeOrigin, Vector3d planeNormal)
            => point.Project(planeOrigin, planeNormal, planeNormal);

        /// <summary>
        /// 求点到面的指定方向的投影
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planePoint"></param>
        /// <param name="planeNormal"></param>
        /// <param name="projectNormal">投影法向量</param>
        /// <returns></returns>
        internal static Point3d Project(this Point3d point, Point3d planePoint, Vector3d planeNormal, Vector3d projectNormal)
        {
            /*********************************************************/
            // 直线:(x-x1)/i=(y-y1)/j=(z-z1)/k      （1）
            // 平面:(x-x2)*o+(y-y2)*p+(z-z2)*q=0    （2）
            // 求它们的交点，就是相当于联立这两个方程，解出x,y,z的值。
            // 令:(x-x1)/i=(y-y1)/j=(z-z1)/k=a
            // 则:x=i* a+x1;
            //    y=j* a+y1;                        （*）
            //    z=k* a+z1;
            // 再代入到（2）中得：
            // (i* a+x1-x2)*o+(j* a+y1-y2)*p+(k* a+z1-z2)*q=0
            // 可以解得a,再把a带回到(*)中，得到的x,y,z就是要求的交点值。
            /**********************************************************/
            // 0.00175 0.1度
            if (projectNormal.IsParallelTo(planeNormal))
                return Point3d.Origin;
            double x1 = point.X, y1 = point.Y, z1 = point.Z, x2 = planePoint.X, y2 = planePoint.Y, z2 = planePoint.Z;
            double i = projectNormal.X, j = projectNormal.Y, k = projectNormal.Z, o = planeNormal.X, p = planeNormal.Y, q = planeNormal.Z;
            double a = (o * (x2 - x1) + p * (y2 - y1) + q * (z2 - z1)) / (o * i + p * j + q * k);
            return new Point3d(i * a + x1, j * a + y1, k * a + z1);
        }
    }
}