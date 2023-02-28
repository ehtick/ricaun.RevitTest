using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ricaun.RevitTest.Shared;
using System;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand, IExternalCommandAvailability
    {
        private static PipeTestClient client { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            if (client is null)
            {
                var client = new PipeTestClient();
                //client.Request = new TestRequest();
                var initializeClient = client.Initialize();
                var task = Task.Run(async () =>
                {
                    await Task.Delay(10000);
                    client.Dispose();
                    client = null;
                });
            }


            //System.Windows.MessageBox.Show(uiapp.Application.VersionName);

            return Result.Succeeded;
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return true;
        }
    }
}
