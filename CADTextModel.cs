using Autodesk.Revit.DB;

namespace Su.AutoCAD2Revit
{
    /// <summary>
    /// AutoCAD文本模型
    /// </summary>
    public class CADTextModel : CADModelBase
    {
        /// <summary>
        /// 文本的位置
        /// </summary>
        public XYZ Location { get; private set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; private set; }

        /// <summary>
        /// 文本形心
        /// </summary>
        public XYZ Centroid { get; private set; }

        internal CADTextModel(XYZ location, XYZ center, string text, string layer, double angle, Transform transform, string blockName) : base(layer, transform, blockName)
        {
            Location = location;
            Centroid = center;
            Text = text;
            Angle = angle;
        }
    }
}