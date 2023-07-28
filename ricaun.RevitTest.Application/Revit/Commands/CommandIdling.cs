using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ricaun.Revit.Async;
using System;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CommandIdling : IExternalCommand
    {
        private static int index;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            RevitTask.Initialize();

            var task = Task.Run(async () =>
            {
                await RevitTask.Run((uiapp) =>
                {
                    uiapp.Idling += Uiapp_Idling;
                });

                await RevitTask.Run(() => { });

                await RevitTask.Run((uiapp) =>
                {
                    uiapp.Idling -= Uiapp_Idling;
                });
            });

            return Result.Succeeded;
        }

        private void Uiapp_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            Console.WriteLine(index++);
        }
    }

}
