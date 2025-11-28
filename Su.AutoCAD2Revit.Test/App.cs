using Autodesk.Revit.UI;
using ricaun.Revit.UI;
using System.IO;

namespace Su.AutoCAD2Revit.Test
{
    internal class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var panel = application.CreatePanel(Path.GetFileNameWithoutExtension(GetType().Assembly.Location));
            panel.CreatePushButton<TestCommand>("Test Command");
            return Result.Succeeded;
        }
    }
}
