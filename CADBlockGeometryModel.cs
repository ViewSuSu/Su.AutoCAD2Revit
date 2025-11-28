//using Autodesk.Revit.DB;
//using Curve = Autodesk.Revit.DB.Curve;

//namespace HYBIM.Revit.TransformerBase.Models
//{
//    /// <summary>
//    /// 图块内的几何
//    /// </summary>
//    public class CADBlockGeometryModel : CADGeometryModelBase
//    {
//        /// <summary>
//        /// 图块里的所有curves（多段线被解析成List<Line>添加到集合中）
//        /// </summary>
//        public List<Curve> Curves
//        {
//            get
//            {
//                List<Curve> curves = new List<Curve>();
//                foreach (var geometryObject in GeometryObjects)
//                {
//                    if (geometryObject is PolyLine polyLine)
//                    {
//                        polyLine.ToLines().ForEach(curves.Add);
//                        polyLine.Dispose();
//                    }
//                    else if (geometryObject is Curve curve)
//                    {
//                        curves.Add(curve);
//                    }
//                }
//                return curves;
//            }
//        }

//        /// <summary>
//        /// 所有的GeometryObject
//        /// </summary>
//        public List<GeometryObject> GeometryObjects { get; private set; }

//        ///// <summary>
//        ///// 最短边(存在多个则取第一个)
//        ///// </summary>
//        //public Curve MinLengthCurve => Curves.MinBy(x => x.Length).FirstOrDefault();

//        ///// <summary>
//        ///// 最长边(存在多个则取第一个)
//        ///// </summary>
//        //public Curve MaxLengthCurve => Curves.MaxBy(x => x.Length).FirstOrDefault();


//        ///// <summary>
//        ///// 是否是直角多边形
//        ///// </summary>
//        //public bool IsAngledPolygon => IsCurveRightAngledPolygon();

//        public CADBlockGeometryModel(string blockName, string layer, IEnumerable<GeometryObject> geometryObjects, XYZ centroid, Transform transform) : base(layer, centroid, transform, blockName)
//        {
//            GeometryObjects = geometryObjects.ToList();
//        }
//    }
//}
