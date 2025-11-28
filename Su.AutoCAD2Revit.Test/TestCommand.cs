using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Su.AutoCAD2Revit.Test
{
    [Transaction(TransactionMode.Manual)]
    internal class TestCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ImportInstance importInstance = null;
            using var autoCADReader = new ReadCADService(importInstance, 0);
            var texts = autoCADReader.GetAllTexts();
            string messageText = "";
            for (int i = 0; i < texts.Count; i++)
            {
                messageText += texts[i] + "\n";
            }
            TaskDialog.Show("Test Command", messageText);
            return Result.Succeeded;
        }
    }
}
