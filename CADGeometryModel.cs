//using Autodesk.Revit.DB;

//namespace HYBIM.Revit.TransformerBase.Models
//{

//    /// <summary>
//    /// CAD的单一几何对象的Model
//    /// </summary>
//    public class CADGeometryModel : CADGeometryModelBase
//    {
//        /// <summary>
//        /// 识别出的Revit几何，这里用List储存是因为：当cad底图的PolyLine时，需要将polyline解析后得到的所有线转成revit里的Curve
//        /// </summary>
//        public List<Autodesk.Revit.DB.Curve> Curves { get; private set; }

//        public CADGeometryModel(string layer, XYZ center, Transform transform, string blockName, List<Autodesk.Revit.DB.Curve> geometryObjects) : base(layer, center, transform, blockName)
//        {
//            Curves = geometryObjects;
//        }

//        /// <summary>
//        /// 获取Curves中最长的Curve
//        /// </summary>
//        /// <returns></returns>
//        public Autodesk.Revit.DB.Curve MaxLengthCurve()
//        {
//            return Curves.OrderByDescending(x => x.Length).FirstOrDefault();
//        }

//        /// <summary>
//        /// 获取Curves中最短的Curve
//        /// </summary>
//        /// <returns></returns>
//        public Autodesk.Revit.DB.Curve MinLengthCurve()
//        {
//            return Curves.OrderBy(x => x.Length).FirstOrDefault();
//        }

//        ///// <summary>
//        ///// 是否是能成圆
//        ///// </summary>
//        ///// <returns></returns>
//        //public bool IsCircel()
//        //{
//        //    if (!Curves.Any(x => x is not Autodesk.Revit.DB.Arc arc) && Curves.Count == 2)
//        //    {
//        //        var first = Curves[0] as Autodesk.Revit.DB.Arc;
//        //        var second = Curves[1] as Autodesk.Revit.DB.Arc;
//        //        if (first.Center.IsAlmostEqualTo(second.Center) && !first.Evaluate(0.5, true).IsAlmostEqualTo(second.Evaluate(0.5, true)))
//        //        {
//        //            return true;
//        //        }
//        //    }
//        //    else if (Curves.Count == 1 && Curves.FirstOrDefault() is Autodesk.Revit.DB.Arc arc && arc.IsCircel())
//        //    {
//        //        return true;
//        //    }
//        //    return false;
//        //}

//        ///// <summary>
//        ///// 尝试构成一个圆
//        ///// </summary>
//        ///// <returns></returns>
//        //public Autodesk.Revit.DB.Arc TryToCircel()
//        //{
//        //    try
//        //    {
//        //        if (!Curves.Any(x => x is not Autodesk.Revit.DB.Arc arc) && Curves.Count == 2)
//        //        {
//        //            var first = Curves[0] as Autodesk.Revit.DB.Arc;
//        //            var second = Curves[1] as Autodesk.Revit.DB.Arc;
//        //            if (first.Center.IsEqual(second.Center) && !first.Evaluate(0.5, true).IsEqual(second.Evaluate(0.5, true)))
//        //            {
//        //                return Autodesk.Revit.DB.Arc.Create(first.Center, first.Radius, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);
//        //            }
//        //        }
//        //        else if (Curves.Count == 1 && Curves.FirstOrDefault() is Autodesk.Revit.DB.Arc arc && arc.IsCircel())
//        //        {
//        //            return Autodesk.Revit.DB.Arc.Create(arc.Center, arc.Radius, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY);
//        //        }
//        //    }
//        //    catch
//        //    {


//        //    }
//        //    return null;
//        //}
//    }
//}
