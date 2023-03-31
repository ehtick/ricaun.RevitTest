using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Busy;
using ricaun.NUnit;
using ricaun.NUnit.Models;
using ricaun.Revit.Async;
using ricaun.Revit.UI;
using ricaun.RevitTest.Application.Revit.Utils;
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

        private const int TestThreadSleepMin = 50;

        public Result OnStartup(UIControlledApplication application)
        {
            RevitBusyControl.Initialize(application);
            RevitBusyControl.Control.PropertyChanged += RevitBusyControlPropertyChanged;
            RevitTask.Initialize(application);

            NUnitUtils.Initialize();

            Debug.WriteLine($"Debugger: {Debugger.IsAttached}");

            RevitParameters.AddParameter(
                application,
                application.ControlledApplication,
                application.GetUIApplication(),
                application.ControlledApplication.GetApplication());

            PipeTestServer_Initialize();

            ribbonPanel = application.CreatePanel("");
            ribbonItem = ribbonPanel.CreatePushButton<Commands.Command>("RevitTest");
            UpdateLargeImageBusy(ribbonItem, RevitBusyControl.Control);

#if DEBUG
            ribbonPanel.GetRibbonPanel().CustomPanelTitleBarBackground = System.Windows.Media.Brushes.Salmon;
#endif

            return Result.Succeeded;
        }

        private static void PipeTestServer_Initialize()
        {
            PipeTestServer = new PipeTestServer();
            PipeTestServer.Update(response =>
            {
                response.IsBusy = RevitBusyControl.Control.IsRevitBusy;
                response.Info = AppUtils.GetInfo() + $" [{TestUtils.GetInitialize()}]";
                response.Test = null;
                response.Tests = null;
            });

            var initializeServer = PipeTestServer.Initialize();
            PipeTestServer.ClientMessage.PropertyChanged += (s, e) =>
            {
                Debug.WriteLine($"PropertyChanged[ {e.PropertyName} ]\t {s.GetType().Name}");
            };

            if (initializeServer)
            {
                PipeTestServer.NamedPipe.ClientMessage += async (connection, message) =>
                {
                    if (string.IsNullOrEmpty(message.TestPathFile))
                    {
                        return;
                    }

                    Log.WriteLine($"Execute: {message.TestPathFile}");

                    string[] testFilterNames = new string[] { };
                    if (string.IsNullOrEmpty(message.TestFilter) == false)
                    {
                        testFilterNames = message.TestFilter.Split(',');
                        Log.WriteLine($"ExecuteFilter: {message.TestFilter}");
                    }

                    PipeTestServer.Update((response) =>
                    {
                        response.IsBusy = true;
                        response.Test = null;
                        response.Info = null;
                        response.Tests = null;
                    });

                    ricaun.NUnit.TestEngine.Result = new TestModelResult((test) =>
                    {
                        PipeTestServer.Update((response) =>
                        {
                            response.IsBusy = true;
                            response.Test = test;
                            response.Info = null;
                            response.Tests = null;
                        });
                        if (test.Time < TestThreadSleepMin) System.Threading.Thread.Sleep(TestThreadSleepMin);
                    });
                    foreach (var testFilterName in testFilterNames)
                    {
                        ricaun.NUnit.TestEngineFilter.Add(testFilterName);
                    }
                    var tests = await RevitTask.Run((uiapp) =>
                    {
                        var tests = TestExecuteUtils.Execute(message.TestPathFile, uiapp.Application.VersionNumber, RevitParameters.Parameters);
                        return tests;
                    });
                    ricaun.NUnit.TestEngine.Result = null;
                    ricaun.NUnit.TestEngineFilter.Reset();
                    PipeTestServer.Update((response) =>
                    {
                        response.IsBusy = true;
                        response.Test = null;
                        response.Info = null;
                        response.Tests = tests as TestAssemblyModel;
                    });
                    // Todo: Send back the zip files
                    await Task.Delay(50);
                    PipeTestServer.Update((response) =>
                    {
                        response.IsBusy = false;
                        response.Test = null;
                        response.Info = null;
                        response.Tests = null;
                    });
                };
            }

            Log.WriteLine();
            Log.WriteLine($"PipeTestServer: {initializeServer} {PipeTestServer.PipeName}");
            Log.WriteLine();
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            PipeTestServer.Update(response =>
            {
                response.Info = "OnShutdown";
                response.Test = null;
                response.Tests = null;
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
                    response.Test = null;
                    response.Tests = null;
                    response.Info = null;
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