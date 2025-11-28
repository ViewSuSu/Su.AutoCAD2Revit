using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Su.AutoCAD2Revit
{
    public class CADModelBase
    {
        /// <summary>
        /// 图纸的Transform
        /// </summary>
        internal Transform ImportInstanceTransform { get; private set; }
        /// <summary>
        /// 图层
        /// </summary>
        public string Layer { get; protected set; }

        /// <summary>
        /// 图块名
        /// </summary>
        public string BlockName { get; private set; }

        internal CADModelBase(string layer, Transform importInstanceTransform, string blockName)
        {
            Layer = layer;
            ImportInstanceTransform = importInstanceTransform;
            BlockName = blockName;
        }
        private CADModelBase()
        {

        }
    }
}
