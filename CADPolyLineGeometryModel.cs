//using Autodesk.Revit.DB;

//namespace Su.AutoCAD2Revit
//{
//    /// <summary>
//    /// CAD的Polyline数据模型
//    /// </summary>
//    public class CADPolyLineGeometryModel : CADGeometryModelBase
//    {
//        /// <summary>
//        /// CADPolyline多段线里的对象在Revit中的实际所有Curve(以CAD中的多段线顺序进行排序)
//        /// </summary>
//        public List<Autodesk.Revit.DB.Curve> Curves { get; private set; }
//        public CADPolyLineGeometryModel(string layer, XYZ center, Transform transform, string blockName, List<Autodesk.Revit.DB.Curve> curves) : base(layer, center, transform, blockName)
//        {
//            Curves = curves;
//        }
//        /// <summary>
//        /// 是否闭合
//        /// </summary>
//        public bool IsClose => Curves.Count > 0 && Curves.IsCurveloopIsConnection();
//    }
//}
