using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Threading.Tasks;

#if DEBUG

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    [Transaction(TransactionMode.Manual)]
    public class CommandApsLog : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            var task = Task.Run(async () =>
            {
                var now = DateTime.UtcNow;
                var result = await ApsApplicationLogger.Log("Null", "This is a message test to the server");
                Console.WriteLine(result);
                Console.WriteLine($"Time: {(DateTime.UtcNow - now).TotalMilliseconds}");
            });
            task.GetAwaiter().GetResult();

            return Result.Succeeded;
        }
    }

}

#endif