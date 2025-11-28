namespace Su.AutoCAD2Revit.Extension
{
    internal static class ExtentsUtils
    {
        /// <summary>
        /// 包围盒有重叠
        /// </summary>
        /// <param name="extents1"></param>
        /// <param name="extents2"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        internal static bool IsOverlap(this Extents3d extents1, Extents3d extents2, double tol)
        {
            if (extents1.MinPoint.X > extents2.MaxPoint.X + tol)
                return false;

            if (extents1.MaxPoint.X + tol < extents2.MinPoint.X)
                return false;

            if (extents1.MinPoint.Y > extents2.MaxPoint.Y + tol)
                return false;

            if (extents1.MaxPoint.Y + tol < extents2.MinPoint.Y)
                return false;

            return true;
        }

        /// <summary>
        /// 获取包围盒面积
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static double Area(this Extents3d extents)
        {
            return extents.Width() * extents.Height();
        }

        /// <summary>
        /// 判断包围盒是否都是高比宽大或者高比宽小
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="otherExtents"></param>
        /// <returns></returns>
        internal static bool IsSameDir(this Extents3d extents, Extents3d otherExtents)
        {
            return (extents.Width() > extents.Height() && otherExtents.Width() > otherExtents.Height()
                || extents.Width() <= extents.Height() && otherExtents.Width() <= otherExtents.Height());
        }

        /// <summary>
        /// 获取包围盒中点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d Center(this Extents3d extents)
        {
            return PointUtils.MidPoint(extents.MinPoint, extents.MaxPoint);
        }

        /// <summary>
        /// 获取包围盒左上点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d LUPoint(this Extents3d extents)
        {
            return new Point3d(extents.MinPoint.X, extents.MaxPoint.Y, 0);
        }

        /// <summary>
        /// 获取包围盒左下点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d LDPoint(this Extents3d extents)
        {
            return extents.MinPoint;
        }

        /// <summary>
        /// 获取包围盒右下点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d RDPoint(this Extents3d extents)
        {
            return new Point3d(extents.MaxPoint.X, extents.MinPoint.Y, 0);
        }

        /// <summary>
        /// 获取包围盒右上点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d RUPoint(this Extents3d extents)
        {
            return extents.MaxPoint;
        }

        /// <summary>
        /// 获取包围盒上边
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Line USide(this Extents3d extents)
        {
            return new Line(extents.LUPoint(), extents.RUPoint());
        }

        /// <summary>
        /// 获取包围盒左边
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Line LSide(this Extents3d extents)
        {
            return new Line(extents.LDPoint(), extents.LUPoint());
        }

        /// <summary>
        /// 获取包围盒下边
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Line DSide(this Extents3d extents)
        {
            return new Line(extents.LDPoint(), extents.RDPoint());
        }

        /// <summary>
        /// 获取包围盒右边
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Line RSide(this Extents3d extents)
        {
            return new Line(extents.RDPoint(), extents.RUPoint());
        }

        /// <summary>
        /// 获取包围盒上中点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d UMidPoint(this Extents3d extents)
        {
            return new Point3d((extents.MinPoint.X + extents.MaxPoint.X) / 2, extents.MaxPoint.Y, 0);
        }

        /// <summary>
        /// 获取包围盒左中点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d LMidPoint(this Extents3d extents)
        {
            return new Point3d(extents.MinPoint.X, (extents.MinPoint.Y + extents.MaxPoint.Y) / 2, 0);
        }

        /// <summary>
        /// 获取包围盒右中点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d RMidPoint(this Extents3d extents)
        {
            return new Point3d(extents.MaxPoint.X, (extents.MinPoint.Y + extents.MaxPoint.Y) / 2, 0);
        }

        /// <summary>
        /// 获取包围盒下中点
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Point3d DMidPoint(this Extents3d extents)
        {
            return new Point3d((extents.MinPoint.X + extents.MaxPoint.X) / 2, extents.MinPoint.Y, 0);
        }

        internal static double Width(this Extents3d extents)
        {
            return extents.MaxPoint.X - extents.MinPoint.X;
        }

        internal static double Height(this Extents3d extents)
        {
            return extents.MaxPoint.Y - extents.MinPoint.Y;
        }

        /// <summary>
        /// 获取位移后的新Extents3d
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        internal static Extents3d Displacement(this Extents3d extents, Vector3d vector)
        {
            return new Extents3d(extents.MinPoint + vector, extents.MaxPoint + vector);
        }

        /// <summary>
        /// 获取顶点
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        internal static Point3dCollection GetPoint3dCollection(this Extents3d extents, double exDis = 0)
        {
            Point3dCollection points = new Point3dCollection();
            points.Add(extents.MinPoint + new Vector3d(-exDis, -exDis, 0));
            points.Add(new Point3d(extents.MaxPoint.X, extents.MinPoint.Y, 0) + new Vector3d(exDis, -exDis, 0));
            points.Add(extents.MaxPoint + new Vector3d(exDis, exDis, 0));
            points.Add(new Point3d(extents.MinPoint.X, extents.MaxPoint.Y, 0) + new Vector3d(-exDis, exDis, 0));
            return points;
        }

        /// <summary>
        /// 将包围盒的Z坐标转换成0
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Extents3d ToZ0(this Extents3d extents)
        {
            return new Extents3d(extents.MinPoint.ToZ0(), extents.MaxPoint.ToZ0());
        }

        /// <summary>
        /// 获取extents边界
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="exDis">扩大或缩小距离</param>
        /// <returns></returns>
        internal static Polyline GetBoundary(this Extents3d extents, double exDis = 0)
        {
            Polyline polyline = new Polyline();
            polyline.AddVertexAt(0, extents.MinPoint.ToPoint2d() + new Vector2d(-exDis, -exDis), 0, 0, 0);
            polyline.AddVertexAt(1, new Point2d(extents.MaxPoint.X, extents.MinPoint.Y) + new Vector2d(exDis, -exDis), 0, 0, 0);
            polyline.AddVertexAt(2, extents.MaxPoint.ToPoint2d() + new Vector2d(exDis, exDis), 0, 0, 0);
            polyline.AddVertexAt(3, new Point2d(extents.MinPoint.X, extents.MaxPoint.Y) + new Vector2d(-exDis, exDis), 0, 0, 0);
            polyline.Closed = true;
            return polyline;
        }

        /// <summary>
        /// 获取extents边界
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="exDis">扩大或缩小距离</param>
        /// <returns></returns>
        internal static Polyline GetBoundary(this Extents2d extents, double exDis = 0)
        {
            Polyline polyline = new Polyline();
            polyline.AddVertexAt(0, extents.MinPoint + new Vector2d(-exDis, -exDis), 0, 0, 0);
            polyline.AddVertexAt(1, new Point2d(extents.MaxPoint.X, extents.MinPoint.Y) + new Vector2d(exDis, -exDis), 0, 0, 0);
            polyline.AddVertexAt(2, extents.MaxPoint + new Vector2d(exDis, exDis), 0, 0, 0);
            polyline.AddVertexAt(3, new Point2d(extents.MinPoint.X, extents.MaxPoint.Y) + new Vector2d(-exDis, exDis), 0, 0, 0);
            polyline.Closed = true;
            return polyline;
        }

        /// <summary>
        /// 点在包围盒内部
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="point"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        internal static bool IsInter(this Extents3d extents, Point3d point, double tol = 0)
        {
            return point.Y < extents.MaxPoint.Y + tol && point.Y > extents.MinPoint.Y - tol
                && point.X < extents.MaxPoint.X + tol && point.X > extents.MinPoint.X - tol;
        }

        /// <summary>
        /// 包围盒在包围盒内部
        /// </summary>
        /// <param name="extents">大包围盒</param>
        /// <param name="pSmallExtents">小包围盒</param>
        /// <param name="tol"></param>
        /// <returns></returns>
        internal static bool IsInter(this Extents3d extents, Extents3d? pSmallExtents, double tol)
        {
            if (pSmallExtents == null)
                return false;

            Extents3d smallExtents = pSmallExtents.Value;

            return smallExtents.MaxPoint.Y < extents.MaxPoint.Y + tol && smallExtents.MinPoint.Y > extents.MinPoint.Y - tol
                && smallExtents.MaxPoint.X < extents.MaxPoint.X + tol && smallExtents.MinPoint.X > extents.MinPoint.X - tol;
        }

        /// <summary>
        /// 包围盒在包围盒内部
        /// </summary>
        /// <param name="extents">大包围盒</param>
        /// <param name="smallExtents">小包围盒</param>
        /// <param name="tol"></param>
        /// <returns></returns>
        internal static bool IsInter(this Extents3d extents, Extents3d smallExtents, double tol = 0)
        {
            return smallExtents.MaxPoint.Y < extents.MaxPoint.Y + tol && smallExtents.MinPoint.Y > extents.MinPoint.Y - tol
                && smallExtents.MaxPoint.X < extents.MaxPoint.X + tol && smallExtents.MinPoint.X > extents.MinPoint.X - tol;
        }

        internal static Extents3d? AddExtents(this Extents3d? ext, Extents3d tempExt)
        {
            if (ext == null)
                return tempExt;

            Extents3d extents = ext.Value;
            extents.AddExtents(tempExt);
            return extents;
        }

        internal static Extents3d? AddExtents(this Extents3d? ext, Extents3d? tempExt)
        {
            if (tempExt == null)
            {
                return ext;
            }
            else
            {
                if (ext == null)
                    return tempExt;

                if (tempExt != null)
                {
                    Extents3d extents = ext.Value;
                    extents.AddExtents(tempExt.Value);
                    return extents;
                }
                else
                    return null;
            }
        }

        internal static Extents3d? TransformBy(this Extents3d? ext, Matrix3d mat)
        {
            if (ext == null)
                return null;

            Extents3d extents = ext.Value;
            extents.TransformBy(mat);
            return extents;
        }

        /// <summary>
        /// 将包围盒扩大或缩小
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="exDis"></param>
        /// <returns></returns>
        internal static Extents3d Extended(this Extents3d extents, double exDis)
        {
            return new Extents3d(extents.MinPoint + new Vector3d(-exDis, -exDis, 0), extents.MaxPoint + new Vector3d(exDis, exDis, 0));
        }

        /// <summary>
        ///  获取下半包围盒
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Extents3d GetDownHalfExtents(this Extents3d extents)
        {
            return new Extents3d(extents.MinPoint, extents.RMidPoint());
        }

        /// <summary>
        /// 获取上半包围盒
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Extents3d GetUpHalfExtents(this Extents3d extents)
        {
            return new Extents3d(extents.LMidPoint(), extents.MaxPoint);
        }

        /// <summary>
        /// 获取左半包围盒
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Extents3d GetLeftHalfExtents(this Extents3d extents)
        {
            return new Extents3d(extents.MinPoint, extents.UMidPoint());
        }

        /// <summary>
        /// 获取右半包围盒
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        internal static Extents3d GetRightHalfExtents(this Extents3d extents)
        {
            return new Extents3d(extents.DMidPoint(), extents.MaxPoint);
        }

        internal static Extents3d ExtendLongSide(this Extents3d extents, double len)
        {
            if (extents.Height() > extents.Width())
                return new Extents3d(extents.MinPoint + new Vector3d(0, -len, 0), extents.MaxPoint + new Vector3d(0, len, 0));
            else
                return new Extents3d(extents.MinPoint + new Vector3d(-len, 0, 0), extents.MaxPoint + new Vector3d(len, 0, 0));
        }
    }
}