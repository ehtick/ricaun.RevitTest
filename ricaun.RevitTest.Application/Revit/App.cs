using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NamedPipeWrapper;
using Revit.Busy;
using ricaun.NUnit;
using ricaun.Revit.Async;
using ricaun.Revit.UI;
using ricaun.RevitTest.Shared;
using System;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit
{
    [AppLoader]
    public class App : IExternalApplication
    {
        public App()
        {
            CosturaUtility.Initialize();
        }

        private static RibbonPanel ribbonPanel;
        private static RibbonItem ribbonItem;
        private static PipeTestServer PipeTestServer;
        public Result OnStartup(UIControlledApplication application)
        {
            Log.WriteLine();
            Log.WriteLine($"TestEngine: {TestEngine.Initialize(out string testInitialize)} {testInitialize}");
            Log.WriteLine();

            RevitBusyControl.Initialize(application);
            RevitBusyControl.Control.PropertyChanged += RevitBusyControlPropertyChanged;
            RevitTask.Initialize(application);

            RevitParameters.AddParameter(
                application,
                application.ControlledApplication,
                application.GetUIApplication(),
                application.ControlledApplication.GetApplication());

            PipeTestServer = new PipeTestServer();
            PipeTestServer.SendResponse(response =>
            {
                response.IsBusy = RevitBusyControl.Control.IsRevitBusy;
            });
            var initializeServer = PipeTestServer.Initialize();

            Log.WriteLine();
            Log.WriteLine($"PipeTestServer: {initializeServer} {PipeTestServer.PipeName}");
            Log.WriteLine();

            ribbonPanel = application.CreatePanel("");
            ribbonItem = ribbonPanel.CreatePushButton<Commands.Command>("RevitTest");
            UpdateLargeImageBusy(ribbonItem, RevitBusyControl.Control);

#if DEBUG
            ribbonPanel.GetRibbonPanel().CustomPanelTitleBarBackground = System.Windows.Media.Brushes.Salmon;
#endif

            //var task = Task.Run(async () =>
            //{
            //    await Task.Delay(5000);
            //    var client = new PipeTestClient();

            //    client.Request = new TestRequest();

            //    var initializeClient = client.Initialize();
            //    Log.WriteLine();
            //    Log.WriteLine($"PipeTestClient: {initializeClient}");
            //    Log.WriteLine();

            //    await Task.Delay(1000);
            //    client.Dispose();
            //});

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            ribbonPanel?.Remove();
            PipeTestServer?.Dispose();
            return Result.Succeeded;
        }

        private void RevitBusyControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var control = sender as RevitBusyService;
            UpdateLargeImageBusy(ribbonItem, control);
            try
            {
                PipeTestServer.SendResponse(response =>
                {
                    response.IsBusy = control.IsRevitBusy;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void UpdateLargeImageBusy(RibbonItem ribbonItem, RevitBusyService control)
        {
            if (ribbonItem is null) return;
            const string LargeImageIsBusy = "/UIFrameworkRes;component/ribbon/images/close.ico";
            const string LargeImageNoBusy = "/UIFrameworkRes;component/ribbon/images/add.ico";
            if (control.IsRevitBusy)
                ribbonItem.SetLargeImage(LargeImageIsBusy);
            else
                ribbonItem.SetLargeImage(LargeImageNoBusy);
        }

    }

}