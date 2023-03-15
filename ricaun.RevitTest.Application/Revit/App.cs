using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Macros;
using Autodesk.Revit.UI;
using NamedPipeWrapper;
using Revit.Busy;
using ricaun.Revit.Async;
using ricaun.Revit.UI;
using ricaun.RevitTest.Shared;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit
{
    [AppLoader]
    public class App : IExternalApplication
    {
        private static RibbonPanel ribbonPanel;
        private static RibbonItem ribbonItem;
        private static PipeTestServer PipeTestServer;
        public Result OnStartup(UIControlledApplication application)
        {
            RevitBusyControl.Initialize(application);
            RevitBusyControl.Control.PropertyChanged += RevitBusyControlPropertyChanged;
            RevitTask.Initialize(application);

            RevitParameters.AddParameter(
                application,
                application.ControlledApplication,
                application.GetUIApplication(),
                application.ControlledApplication.GetApplication());

            PipeTestServer = new PipeTestServer();
            PipeTestServer.Update(response =>
            {
                response.IsBusy = RevitBusyControl.Control.IsRevitBusy;
                response.Text = TestUtils.GetInitialize() + " " + this.GetType().Assembly.GetName().Name;
            });
            var initializeServer = PipeTestServer.Initialize();

            if (initializeServer)
            {
                PipeTestServer.NamedPipe.ClientMessage += async (connection, message) =>
                {
                    Log.WriteLine($"Execute: {message.TestPathFile}");
                    PipeTestServer.Update((response) =>
                    {
                        response.IsBusy = true;
                        response.Text = null;
                    });
                    ricaun.NUnit.TestEngine.Result = new TestModelResultAction((test) =>
                    {
                        PipeTestServer.Update((response) =>
                        {
                            response.IsBusy = true;
                            response.Text = test.ToString();
                        });
                        System.Threading.Thread.Sleep(10);
                    });
                    var tests = await RevitTask.Run((uiapp) =>
                    {
                        var tests = TestExecuteUtils.Execute(message.TestPathFile, uiapp.Application.VersionNumber, RevitParameters.Parameters);
                        return tests;
                    });
                    ricaun.NUnit.TestEngine.Result = null;
                    PipeTestServer.Update((response) =>
                    {
                        response.IsBusy = false;
                        response.Text = tests.ToString();
                    });
                    await Task.Delay(10);

                    //if (tests is ricaun.NUnit.Models.TestAssemblyModel modelTest)
                    //{
                    //    var modelTests = modelTest.Tests.SelectMany(e => e.Tests);
                    //    foreach (var test in modelTests)
                    //    {
                    //        PipeTestServer.Update((response) =>
                    //        {
                    //            response.IsBusy = false;
                    //            response.Text = test.ToString();
                    //        });
                    //        await Task.Delay(10);
                    //    }
                    //}

                };
            }

            Log.WriteLine();
            Log.WriteLine($"PipeTestServer: {initializeServer} {PipeTestServer.PipeName}");
            Log.WriteLine();

            ribbonPanel = application.CreatePanel("");
            ribbonItem = ribbonPanel.CreatePushButton<Commands.Command>("RevitTest");
            UpdateLargeImageBusy(ribbonItem, RevitBusyControl.Control);

#if DEBUG
            ribbonPanel.GetRibbonPanel().CustomPanelTitleBarBackground = System.Windows.Media.Brushes.Salmon;
#endif

            return Result.Succeeded;
        }



        public Result OnShutdown(UIControlledApplication application)
        {
            PipeTestServer.Update(response =>
            {
                response.Text = "OnShutdown";
            });

            ribbonPanel?.Remove();
            PipeTestServer?.Dispose();
            RevitBusyControl.Control.PropertyChanged -= RevitBusyControlPropertyChanged;
            return Result.Succeeded;
        }

        private void RevitBusyControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var control = sender as RevitBusyService;
            UpdateLargeImageBusy(ribbonItem, control);
            try
            {
                PipeTestServer.Update(response =>
                {
                    response.IsBusy = control.IsRevitBusy;
                    response.Text = null;
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