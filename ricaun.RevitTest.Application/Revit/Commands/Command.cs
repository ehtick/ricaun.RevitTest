using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ricaun.RevitTest.Shared;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand, IExternalCommandAvailability
    {
        //private static PipeTestClient client { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            //if (client is null)
            //{
            //    var process = Process.GetCurrentProcess();

            //    process = Process.GetProcesses()
            //        .Where(e => e.ProcessName == process.ProcessName)
            //        .FirstOrDefault(e => e.Id != process.Id) ?? process;

            //    Console.WriteLine(process.GetPipeName());

            //    client = new PipeTestClient(process);
            //    //client.Request = new TestRequest() { Id = process.Id };
            //    client.Update(request => request.Id = process.Id);
            //    var initializeClient = client.Initialize();
            //    var task = Task.Run(async () =>
            //    {
            //        await Task.Delay(10000);
            //        client.Dispose();
            //        client = null;
            //    });
            //}


            System.Windows.MessageBox.Show(uiapp.Application.VersionName);

            return Result.Succeeded;
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return true;
        }
    }
}
