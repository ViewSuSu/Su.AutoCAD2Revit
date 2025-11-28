using Autodesk.Revit.DB;

namespace Su.AutoCAD2Revit
{
    public class CADModelBase
    {
        /// <summary>
        /// 图层
        /// </summary>
        public string Layer { get; protected set; }

        /// <summary>
        /// 图块名
        /// </summary>
        public string BlockName { get; private set; }

        internal CADModelBase(string layer,  string blockName)
        {
            Layer = layer;
            BlockName = blockName;
        }

        private CADModelBase()
        {
        }
    }
}