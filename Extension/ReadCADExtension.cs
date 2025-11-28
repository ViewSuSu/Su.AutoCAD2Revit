global using System;
global using System.Collections.Generic;
global using System.Linq;
global using Teigha.DatabaseServices;
global using Teigha.Geometry;

namespace Su.AutoCAD2Revit.Extension
{
    internal static class ReadCADExtension
    {
        /// <summary>
        /// 炸开所有的块（包括嵌套块）
        /// </summary>
        /// <param name="db"></param>
        internal static void ExportBlocks(this Database db, string blockTableRecord)
        {
            bool continu = false;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                using (LayerTable lt = (LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForWrite))
                {
                    foreach (ObjectId id in lt)
                    {
                        LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(id, OpenMode.ForWrite);
                        if (ltr.IsLocked)
                        {
                            ltr.IsLocked = false;
                        }
                    }
                }

                BlockTable blockTable = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForWrite);
                BlockTableRecord space = trans.GetObject(blockTable[blockTableRecord], OpenMode.ForWrite) as BlockTableRecord;
                foreach (var id in space)
                {
                    DBObject obj = trans.GetObject(id, OpenMode.ForWrite);
                    if (obj is BlockReference brf)
                    {
                        brf.ExplodeToOwnerSpace();
                        brf.Erase();
                        continu = true;
                    }
                }
                trans.Commit();
            }
            if (continu)
            {
                ExportBlocks(db, blockTableRecord);
            }
        }
    }
}