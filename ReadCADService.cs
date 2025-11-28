using Autodesk.Revit.DB;
using Su.AutoCAD2Revit.Extension;
using System.IO;
using Teigha.Runtime;
using Arc = Teigha.DatabaseServices.Arc;
using Ellipse = Teigha.DatabaseServices.Ellipse;
using Entity = Teigha.DatabaseServices.Entity;
using Exception = System.Exception;
using Line = Teigha.DatabaseServices.Line;
using Path = System.IO.Path;
using Polyline = Teigha.DatabaseServices.Polyline;

namespace Su.AutoCAD2Revit
{

    /// <summary>
    /// AutoCAD2Revit图纸识别服务对象
    /// </summary>
    public class ReadCADService : IDisposable
    {
        //图纸element
        private ImportInstance importInstance;
        private string cacheDwgFile;//缓存图纸的路径
        private Transform importInstanceTransform;//图纸的transform
        private double levelHeight = 0;//图纸的绝对标高z

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="importInstance">导入实例</param>
        public ReadCADService(ImportInstance importInstance)
        {
            this.importInstance = importInstance;
            cacheDwgFile = Directory.GetParent(GetType().Assembly.Location).FullName + $"\\{Path.GetFileName(importInstance.GetCADPath())}";
            //Doc.GetCADPath(dwgElement).SmartCopyFile(cacheDwgFile);
            importInstanceTransform = importInstance.GetTransform();
            Level importanLevel = importInstance.Document.GetElement(importInstance.LevelId) as Level;
            if (importInstance.ViewSpecific)//仅当前视图可见
            {
                levelHeight = importanLevel.ProjectElevation;
            }
            else
            {
                levelHeight = importanLevel.ProjectElevation + importInstance.get_Parameter(BuiltInParameter.IMPORT_BASE_LEVEL_OFFSET).AsDouble();
            }
        }

        private ReadCADService()
        {

        }

        private void DeleteCacheFile()
        {
            try
            {
                File.Delete(cacheDwgFile);
            }
            catch
            {


            }
        }



        /// <summary>
        /// 取得该图纸中的所有文字信息
        /// </summary>
        /// <param name="fileOpenMode"></param>
        /// <param name="blockTableRecord"></param>
        /// <param name="allowCPConversion"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CADTextModel> GetAllTexts(FileOpenMode fileOpenMode = FileOpenMode.OpenForReadAndWriteNoShare, string blockTableRecord = "*MODEL_SPACE", bool allowCPConversion = true, string password = "")
        {
            List<CADTextModel> listCADModels = [];
            using (new Services())
            {
                using Database database = new(false, false);
                try
                {
                    database.ReadDwgFile(cacheDwgFile, fileOpenMode, allowCPConversion, password);
                }
                catch
                {
                    throw new Exception($"图纸读取失败！可能是您的图纸版本高于AutoCAD2013，或您的图纸进行了特殊加密，或您链接的图纸失去链接！请检查后重试");
                }
                try
                {
                    database.ExportBlocks(blockTableRecord);
                }
                catch (Exception ex)
                {


                }
                using var trans = database.TransactionManager.StartTransaction();
                using BlockTable table = (BlockTable)database.BlockTableId.GetObject(OpenMode.ForWrite);
                using BlockTableRecord record = (BlockTableRecord)table[blockTableRecord].GetObject(OpenMode.ForWrite);
                foreach (ObjectId id in record)
                {
                    using Entity entity = (Entity)id.GetObject(OpenMode.ForRead, false, false);
                    switch (entity.GetType().Name)
                    {
                        case nameof(DBText):
                            DBText text = (DBText)entity;
                            var dbLocation = text.Position.ToRevitPoint().Transform(importInstanceTransform).SetZ(levelHeight);
                            var dbCenter = text.GeometricExtents.Center().ToRevitPoint().Transform(importInstanceTransform).SetZ(levelHeight);
                            CADTextModel model = new CADTextModel(dbLocation, dbCenter, text.TextString, text.Layer, text.Rotation, importInstanceTransform, text.BlockName);
                            listCADModels.Add(model);
                            break;
                        case nameof(MText):
                            MText mText = (MText)entity;
                            var mtLocation = mText.Location.ToRevitPoint().Transform(importInstanceTransform).SetZ(levelHeight);
                            var mtCenter = mText.GeometricExtents.Center().ToRevitPoint().Transform(importInstanceTransform).SetZ(levelHeight);
                            CADTextModel acDbMTextModel = new CADTextModel(mtLocation, mtCenter, mText.Text, mText.Layer, mText.Rotation, importInstanceTransform, mText.BlockName);
                            listCADModels.Add(acDbMTextModel);
                            break;
                    }
                }
            }

            return listCADModels;
        }

        ///// <summary>
        ///// 取得该图纸中的所有的图层名称
        ///// </summary>
        ///// <returns></returns>
        ///// <exception cref="Exception"></exception>
        //public List<string> GetAllLayerNames()
        //{
        //    List<string> cadLayerNames = new List<string>();
        //    using (new Services())
        //    {
        //        using (Database database = new Database(false, false))
        //        {
        //            try
        //            {
        //                database.ReadDwgFile(cacheDwgFile, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
        //            }
        //            catch
        //            {
        //                throw new Exception($"图纸读取失败！可能是您的图纸版本高于CAD2013，或您的图纸进行了特殊加密，或您链接的图纸失去链接！请检查后重试");
        //            }
        //            using (var trans = database.TransactionManager.StartTransaction())
        //            {
        //                using (LayerTable lt = (LayerTable)trans.GetObject(database.LayerTableId, OpenMode.ForWrite))
        //                {
        //                    foreach (ObjectId id in lt)
        //                    {
        //                        LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(id, OpenMode.ForWrite);
        //                        cadLayerNames.Add(ltr.Name);
        //                    }
        //                }
        //                trans.Commit();
        //            }
        //        }
        //    }
        //    DeleteCacheFile();
        //    return cadLayerNames;
        //}

        ///// <summary>
        ///// 获取CAD里的几何
        ///// </summary>
        ///// <param name="directOnly"></param>
        ///// <param name="forceValidity"></param>
        ///// <returns></returns>
        //public List<CADGeometryModel> GetCADGeometries()
        //{
        //    List<CADGeometryModel> models = new();
        //    using (new Services())
        //    {
        //        using Database database = new Database(false, false);
        //        try
        //        {
        //            database.ReadDwgFile(cacheDwgFile, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
        //        }
        //        catch
        //        {
        //            throw new Exception($"图纸读取失败！可能是您的图纸版本高于CAD2013，或您的图纸进行了特殊加密，或您链接的图纸失去链接！请检查后重试");
        //        }
        //        try
        //        {
        //            database.ExportBlocks();
        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //        using var trans = database.TransactionManager.StartTransaction();
        //        using BlockTable table = (BlockTable)database.BlockTableId.GetObject(OpenMode.ForWrite);
        //        BlockTableRecord btr = (BlockTableRecord)table[blockTableRecord].GetObject(OpenMode.ForWrite);
        //        foreach (var id in btr)
        //        {
        //            using var entity = (Entity)id.GetObject(OpenMode.ForWrite);
        //            if (entity != null && entity is Teigha.DatabaseServices.Curve curve)
        //            {
        //                List<Autodesk.Revit.DB.Curve> curves = new List<Autodesk.Revit.DB.Curve>();

        //                if (func != null && func.Invoke(entity))
        //                {
        //                    if (curve is Polyline polyline)
        //                    {
        //                        curves.AddRange(CADPolyLineToRevitCurves(polyline));
        //                    }
        //                    else
        //                    {
        //                        curves.AddRange(CADCurveToRevitCurve(curve));
        //                    }
        //                }
        //                else if (func == null)
        //                {
        //                    if (curve is Polyline polyline)
        //                    {
        //                        curves.AddRange(CADPolyLineToRevitCurves(polyline));
        //                    }
        //                    else
        //                    {
        //                        curves.AddRange(CADCurveToRevitCurve(curve));
        //                    }
        //                }
        //                if (curves.Count != 0)
        //                {
        //                    var model = new CADGeometryModel(entity.Layer, ConverCADPointToRevitPoint(ExtentsUtils.Center(curve.GeometricExtents)).TransformPoint(importInstanceTransform), importInstanceTransform, entity.BlockName, curves);
        //                    models.Add(model);
        //                }
        //            }
        //        }
        //    }
        //    DeleteCacheFile();
        //    return models;
        //}


        ///// <summary>
        ///// 获取CAD里的PolyLine几何
        ///// </summary>
        ///// <param name="directOnly"></param>
        ///// <param name="forceValidity"></param>
        ///// <returns></returns>
        //public List<CADPolyLineGeometryModel> GetCADPolyLines()
        //{
        //    List<CADPolyLineGeometryModel> models = new List<CADPolyLineGeometryModel>();
        //    using (new Services())
        //    {
        //        using Database database = new Database(false, false);
        //        try
        //        {
        //            database.ReadDwgFile(cacheDwgFile, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
        //        }
        //        catch
        //        {
        //            throw new Exception($"图纸读取失败！可能是您的图纸版本高于CAD2013，或您的图纸进行了特殊加密，或您链接的图纸失去链接！请检查后重试");
        //        }
        //        try
        //        {
        //            database.ExportBlocks();
        //        }
        //        catch (Exception ex)
        //        {


        //        }
        //        using var trans = database.TransactionManager.StartTransaction();
        //        using BlockTable table = (BlockTable)database.BlockTableId.GetObject(OpenMode.ForRead);
        //        BlockTableRecord btr = (BlockTableRecord)table[blockTableRecord].GetObject(OpenMode.ForRead);
        //        foreach (var id in btr)
        //        {
        //            using var entity = (Entity)id.GetObject(OpenMode.ForRead);
        //            if (entity != null && entity.Id.IsValid && entity is Polyline polyline)
        //            {
        //                List<Autodesk.Revit.DB.Curve> revitCurves = new List<Autodesk.Revit.DB.Curve>();
        //                if (func != null && func.Invoke(entity))
        //                {
        //                    revitCurves.AddRange(CADPolyLineToRevitCurves(polyline));
        //                }
        //                else if (func == null)
        //                {
        //                    revitCurves.AddRange(CADPolyLineToRevitCurves(polyline));
        //                }
        //                if (revitCurves.Count > 0)
        //                {
        //                    var model = new CADPolyLineGeometryModel(entity.Layer, ConverCADPointToRevitPoint(polyline.GeometricExtents.Center()).TransformPoint(importInstanceTransform), importInstanceTransform, entity.BlockName, revitCurves);
        //                    models.Add(model);
        //                }
        //            }
        //        }
        //    }
        //    DeleteCacheFile();
        //    return models;
        //}

        ///// <summary>
        ///// 获取图块内的几何
        ///// </summary>
        ///// <param name="directOnly"></param>
        ///// <param name="forceValidity"></param>
        ///// <returns></returns>
        //public List<CADBlockGeometryModel> GetModelSpaceBlockReferenceGeometries(bool directOnly = false, bool forceValidity = true)
        //{
        //    List<CADBlockGeometryModel> models = new List<CADBlockGeometryModel>();
        //    using (new Services())
        //    {
        //        using (Database database = new Database(false, false))
        //        {
        //            database.ReadDwgFile(cacheDwgFile, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
        //            using (var trans = database.TransactionManager.StartTransaction())
        //            {
        //                using (BlockTable table = (BlockTable)database.BlockTableId.GetObject(OpenMode.ForWrite))
        //                {
        //                    foreach (var btrId in table)
        //                    {
        //                        using (BlockTableRecord btr = (BlockTableRecord)btrId.GetObject(OpenMode.ForWrite))
        //                        {
        //                            if (!btr.Name.Contains(blockTableRecord) && !btr.IsLayout && !btr.Name.Contains(BlockTableRecord.PaperSpace))
        //                            {
        //                                var ids = btr.GetBlockReferenceIds(directOnly, forceValidity);//查看该图块是否存在块参照
        //                                if (ids.Count > 0)//如果存在块参照
        //                                {
        //                                    List<Teigha.DatabaseServices.Curve> curves = new List<Teigha.DatabaseServices.Curve>();//记录这个块里的所有图元
        //                                    Extents3d ext = new Extents3d();
        //                                    foreach (var entityId in btr)
        //                                    {
        //                                        var entity = (Entity)entityId.GetObject(OpenMode.ForWrite);
        //                                        if (entity != null && entity is Teigha.DatabaseServices.Curve curve)
        //                                        {
        //                                            curves.Add(curve);
        //                                            ext.AddExtents(entity.GeometricExtents);
        //                                        }
        //                                        else
        //                                        {
        //                                            entity.Dispose();
        //                                        }
        //                                    }
        //                                    foreach (ObjectId id in ids)//遍历所有块参照
        //                                    {
        //                                        using (var blockReference = (BlockReference)id.GetObject(OpenMode.ForWrite))
        //                                        {
        //                                            if(curves.Count>0)
        //                                            {
        //                                                List<GeometryObject> geometryObjects = new List<GeometryObject>();
        //                                                for (int i = 0; i < curves.Count; i++)
        //                                                {
        //                                                    if (func != null && func.Invoke(curves[i]))
        //                                                    {
        //                                                        //基于块参照的transform对每一个块对象进行坐标转化，最后转成revit的图元
        //                                                        var geometryObject = ConverCADGeometryToRevitGeometry(curves[i], blockReference.BlockTransform);
        //                                                        if (geometryObject != null)
        //                                                            geometryObjects.Add(geometryObject);
        //                                                    }
        //                                                    else if (func == null)
        //                                                    {
        //                                                        //基于块参照的transform对每一个块对象进行坐标转化，最后转成revit的图元
        //                                                        var geometryObject = ConverCADGeometryToRevitGeometry(curves[i], blockReference.BlockTransform);
        //                                                        if (geometryObject != null)
        //                                                            geometryObjects.Add(geometryObject);
        //                                                    }
        //                                                    trans.GetObject(curves[i].Id, OpenMode.ForWrite).Dispose();
        //                                                }
        //                                                if (geometryObjects.Count > 0)
        //                                                {
        //                                                    var model = new CADBlockGeometryModel(blockReference.Name, blockReference.Layer, geometryObjects, ConverCADPointToRevitPoint(ExtentsUtils.Center(ext).TransformBy(blockReference.BlockTransform)).Transform(importInstanceTransform), importInstanceTransform);
        //                                                    models.Add(model);
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    DeleteCacheFile();
        //    return models;
        //} 

        //private List<Autodesk.Revit.DB.Curve> CADCurveToRevitCurve(Teigha.DatabaseServices.Curve cadCurve, Matrix3d matrix3D = default)
        //{
        //    List<Autodesk.Revit.DB.Curve> curves = new List<Autodesk.Revit.DB.Curve>();
        //    if (cadCurve is Line line)
        //    {
        //        Autodesk.Revit.DB.Line revitLine = null;
        //        if (matrix3D != default)
        //        {
        //            revitLine = Autodesk.Revit.DB.Line.CreateBound(ConverCADPointToRevitPoint(line.StartPoint.TransformBy(matrix3D)).SetZ(levelHeight), ConverCADPointToRevitPoint(line.EndPoint.TransformBy(matrix3D)).SetZ(levelHeight));
        //        }
        //        else
        //        {
        //            revitLine = Autodesk.Revit.DB.Line.CreateBound(ConverCADPointToRevitPoint(line.StartPoint).SetZ(levelHeight), ConverCADPointToRevitPoint(line.EndPoint).SetZ(levelHeight));
        //        }
        //        curves.Add(revitLine.CreateTransformed(importInstanceTransform));
        //    }
        //    else if (cadCurve is Arc arc)
        //    {
        //        double enda, stara;
        //        if (arc.StartAngle > arc.EndAngle)
        //        {
        //            enda = arc.StartAngle;
        //            stara = arc.EndAngle;
        //        }
        //        else
        //        {
        //            enda = arc.EndAngle;
        //            stara = arc.StartAngle;
        //        }
        //        XYZ centerPoint = matrix3D != default ? ConverCADPointToRevitPoint(arc.Center.TransformBy(matrix3D)) : ConverCADPointToRevitPoint(arc.Center);
        //        Autodesk.Revit.DB.Arc revitArc = Autodesk.Revit.DB.Arc.Create(Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(new XYZ(arc.Normal.X, arc.Normal.Y, arc.Normal.Z), centerPoint.SetZ(levelHeight)), arc.Radius.MMToFT(), stara, enda);
        //        curves.Add(revitArc.CreateTransformed(importInstanceTransform));
        //    }
        //    else if (cadCurve is Circle circle)
        //    {
        //        XYZ centerPoint = matrix3D != default ? ConverCADPointToRevitPoint(circle.Center.TransformBy(matrix3D)) : ConverCADPointToRevitPoint(circle.Center);
        //        Autodesk.Revit.DB.Arc revitArc = Autodesk.Revit.DB.Arc.Create(Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(new XYZ(circle.Normal.X, circle.Normal.Y, circle.Normal.Z), centerPoint.SetZ(levelHeight)), circle.Radius.MMToFT(), 0, Math.PI * 2);
        //        curves.Add(revitArc.CreateTransformed(importInstanceTransform));
        //    }
        //    else if (cadCurve is Teigha.DatabaseServices.Leader leader)
        //    {
        //        if (leader.NumVertices > 2)
        //        {
        //            IList<XYZ> listPoints = new List<XYZ>();
        //            for (int i = 0; i < leader.NumVertices; i++)
        //            {
        //                XYZ xYZ = matrix3D != default
        //                    ? new XYZ(leader.VertexAt(i).TransformBy(matrix3D).X.MMToFT(), leader.VertexAt(i).TransformBy(matrix3D).Y.MMToFT(), levelHeight)
        //                    : new XYZ(leader.VertexAt(i).X.MMToFT(), leader.VertexAt(i).Y.MMToFT(), levelHeight);
        //                listPoints.Add(xYZ);
        //            }
        //            listPoints = listPoints.Reverse().ToList();
        //            for (int i = 0; i < listPoints.Count - 1; i++)
        //            {
        //                curves.Add(Autodesk.Revit.DB.Line.CreateBound(listPoints[i], listPoints[i + 1]).CreateTransformed(importInstanceTransform));
        //            }
        //        }
        //        else
        //        {
        //            Autodesk.Revit.DB.Line revitLine = null;
        //            if (matrix3D != default)
        //            {
        //                revitLine = Autodesk.Revit.DB.Line.CreateBound(ConverCADPointToRevitPoint(leader.EndPoint.TransformBy(matrix3D)).SetZ(levelHeight), ConverCADPointToRevitPoint(leader.StartPoint.TransformBy(matrix3D))).SetZ(levelHeight);
        //            }
        //            else
        //            {
        //                revitLine = Autodesk.Revit.DB.Line.CreateBound(ConverCADPointToRevitPoint(leader.EndPoint).SetZ(levelHeight), ConverCADPointToRevitPoint(leader.StartPoint)).SetZ(levelHeight);
        //            }
        //            curves.Add(revitLine.CreateTransformed(importInstanceTransform));
        //        }
        //    }
        //    return curves;
        //}

        //private List<Autodesk.Revit.DB.Curve> CADPolyLineToRevitCurves(Polyline polyline)
        //{
        //    List<Autodesk.Revit.DB.Curve> curves = new List<Autodesk.Revit.DB.Curve>();
        //    DBObjectCollection dBObjectCollection = new DBObjectCollection();
        //    polyline.Explode(dBObjectCollection);
        //    foreach (Entity item in dBObjectCollection)
        //    {
        //        using (item)
        //        {
        //            if (item is Line line)
        //            {
        //                var lineStart = ConverCADPointToRevitPoint(line.StartPoint).SetZ(levelHeight);
        //                var lineEnd = ConverCADPointToRevitPoint(line.EndPoint).SetZ(levelHeight);
        //                if (lineStart.DistanceTo(lineEnd) > Doc.Application.ShortCurveTolerance)
        //                {
        //                    curves.Add(Autodesk.Revit.DB.Line.CreateBound(lineStart, lineEnd).CreateTransformed(importInstanceTransform));
        //                }
        //            }
        //            else if (item is Arc arc)
        //            {
        //                var start = ConverCADPointToRevitPoint(arc.StartPoint).SetZ(levelHeight);
        //                var end = ConverCADPointToRevitPoint(arc.EndPoint).SetZ(levelHeight);
        //                var pointOnArc = ConverCADPointToRevitPoint(arc.GetPointAtDist(arc.Length / 2)).SetZ(levelHeight);
        //                curves.Add(Autodesk.Revit.DB.Arc.Create(start, end, pointOnArc).CreateTransformed(importInstanceTransform));
        //            }
        //            else if (item is Ellipse ellipse)
        //            {

        //            }
        //        }
        //    }
        //    return curves;
        //}


        /// <summary>
        /// AutoCAD点转Revit点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>


        public void Dispose()
        {
            DeleteCacheFile();
        }
    }
}