using Autodesk.Revit.DB;

namespace Su.AutoCAD2Revit.Extension
{
    internal static class RevitExtension
    {
        internal static XYZ Transform(this XYZ point, Transform transform)
        {
            return transform.OfPoint(point);
        }

        internal static XYZ SetZ(this XYZ point, double z)
        {
            return new XYZ(point.X, point.Y, z);
        }

        internal static string GetCADPath(this ImportInstance importInstance)
        {
            CADLinkType type = importInstance.Document.GetElement(importInstance.GetTypeId()) as CADLinkType;
            string filePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(type.GetExternalFileReference().GetAbsolutePath());
            return filePath;
        }

        internal static XYZ ToRevitPoint(this Point3d point)
        {
            return new XYZ(point.X / 304.8, point.Y / 304.8, point.Z / 304.8);
        }
    }
}