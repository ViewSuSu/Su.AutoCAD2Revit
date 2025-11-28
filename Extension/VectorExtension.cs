namespace Su.AutoCAD2Revit.Extension
{
    internal static class VectorUtils
    {
        /// <summary>
        /// 判断两向量是否平行(待测试)
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        internal static bool IsParallel(this Vector3d line1, Vector3d line2, Tolerance tol)
        {
            return line1.CrossProduct(line2).IsEqualTo(new Vector3d(0, 0, 0), tol);
        }

        /// <summary>
        /// 获取直线的垂直向量
        /// </summary>
        /// <param name="dirLine"></param>
        /// <returns></returns>
        internal static Vector3d GetNormal(this Vector3d vec)
        {
            Vector3d normalVec = new Vector3d(-vec.Y, vec.X, 0);
            return normalVec.GetNormal();
        }

        /// <summary>
        /// 过点做向量的垂线
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        internal static Line GetVerticalLine(this Vector3d vec, Point3d pt)
        {
            Vector3d normalVec = new Vector3d(-vec.Y, vec.X, 0);
            return new Line(pt, pt + normalVec.GetNormal());
        }

        internal static double GetAngle(this Vector3d vec)
        {
            double angle = vec.GetAngleTo(Vector3d.XAxis);
            if (vec.Y < 0)
                return Math.PI - angle + Math.PI;
            else
                return angle;
        }

        /// <summary>
        /// X轴到向量的弧度,cad的获取的弧度是1PI,所以转换为2PI(上小,下大)
        /// </summary>
        /// <param name="ve">向量</param>
        /// <returns>X轴到向量的弧度</returns>
        internal static double GetAngle2XAxis(this Vector2d ve, double tolerance = 1e-6)
        {
            const double Tau = Math.PI + Math.PI;
            // 世界重合到用户 Vector3d.XAxis->两点向量
            double al = Vector2d.XAxis.GetAngleTo(ve);
            al = ve.Y > 0 ? al : Tau - al; // 逆时针为正,大于0是上半圆,小于则是下半圆,如果-负值控制正反
            al = Math.Abs(Tau - al) <= tolerance ? 0 : al;
            return al;
        }

        /// <summary>
        ///  判断点在向量的左边还是右边，大于0是左边，小于0是右边
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        internal static double JudgeDirection(this Vector3d startPoint, Vector3d endPoint)
        {
            return startPoint.X * startPoint.X + endPoint.Y * endPoint.Y + startPoint.Z * endPoint.Z;
        }

        /// <summary>
        /// 获取向量角度
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <returns></returns>

        internal static double GetAngle(Vector3d vecA, Vector3d vecB)
        {
            double angle = vecA.GetAngleTo(vecB);

            double c = vecA.CrossProduct(vecB).Z / vecA.Length / vecB.Length;
            if (Math.Abs(c) < 1e-6)
                c = 0;

            bool flag = c >= 0 ? true : false;
            if (!flag)
                angle = 2 * Math.PI - angle;
            return angle;
        }
    }
}