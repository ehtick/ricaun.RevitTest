//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using System;
//using System.Threading.Tasks;

//#if DEBUG

//namespace ricaun.RevitTest.Application.Revit.ApsApplication
//{
//    [Transaction(TransactionMode.Manual)]
//    public class CommandApsCheck : IExternalCommand
//    {
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
//        {
//            UIApplication uiapp = commandData.Application;

//            var task = Task.Run(async () =>
//            {
//                var now = DateTime.UtcNow;
//                var result = await ApsApplicationCheck.Check();
//                Console.WriteLine($"Result: {result}");
//                Console.WriteLine($"Time: {(DateTime.UtcNow - now).TotalMilliseconds}");
//            });
//            task.GetAwaiter().GetResult();

//            return Result.Succeeded;
//        }
//    }

//}

//#endif