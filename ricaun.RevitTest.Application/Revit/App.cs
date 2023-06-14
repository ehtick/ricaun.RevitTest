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
using System.Reflection;
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
            Log.Initilize(application);

            RevitBusyControl.Initialize(application);
            RevitBusyControl.Control.PropertyChanged += RevitBusyControlPropertyChanged;
            RevitTask.Initialize(application);

            Log.WriteLine();
            Log.WriteLine($"{AppUtils.GetInfo()}");

            NUnitUtils.Initialize();

            if (Debugger.IsAttached) Log.WriteLine($"Debugger: {Debugger.IsAttached}");

            RevitParameters.AddParameter(
                application,
                application.ControlledApplication,
                application.GetUIApplication(),
                application.ControlledApplication.GetApplication());

            PipeTestServer_Initialize();

            CreateRibbonPanel(application);

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

            if (initializeServer)
            {
                PipeTestServer.ClientMessage.PropertyChanged += async (s, e) =>
                {
                    Debug.WriteLine($"PropertyChanged[ {e.PropertyName} ]\t {s.GetType().Name}");

                    var message = s as TestRequest;
                    if (message is null) return;

                    if (e.PropertyName == nameof(TestRequest.Info))
                    {
                        Log.WriteLine($"Info: {message.Info}");
                    }

                    if (e.PropertyName != nameof(TestRequest.TestPathFile)) return;

                    if (string.IsNullOrEmpty(message.TestPathFile))
                    {
                        return;
                    }

                    ribbonItem.SetLargeImage(LargeImageRun);

                    Log.WriteLine($"Execute: {message.TestPathFile}");

                    string[] testFilterNames = new string[] { };
                    if (message.TestFilters is not null)
                    {
                        testFilterNames = message.TestFilters;
                        Log.WriteLine($"ExecuteFilter: {message.TestFilters.Length}");
                        foreach (var testFilterName in testFilterNames)
                        {
                            Log.WriteLine($"FilterName: {testFilterName}");
                        }
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
                        try
                        {
                            if (UserUtils.IsNotValid(uiapp)) throw new Exception("UserUtils.IsNotValid");

                            var tests = TestExecuteUtils.Execute(message.TestPathFile, uiapp.Application.VersionNumber, RevitParameters.Parameters);
                            return tests;
                        }
                        catch { Log.WriteLine("TestExecuteUtils: Fail"); }
                        return null;
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

                    ribbonItem.SetLargeImage(LargeImageNoBusy);
                };
            }

            Log.WriteLine();
            Log.WriteLine($"PipeTestServer: {initializeServer} {PipeTestServer.PipeName}");
            Log.WriteLine();
        }

        private void CreateRibbonPanel(UIControlledApplication application)
        {
            ribbonPanel = application.CreatePanel(this.GetType().Name);
            ribbonPanel.Title = "ricaun";
            ribbonItem = ribbonPanel.CreatePushButton<Commands.Command>("RevitTest");
            ribbonItem.SetContextualHelp("https://ricaun.com")
                .SetToolTip("Open RevitTest.log File");

            UpdateLargeImageBusy(ribbonItem, RevitBusyControl.Control);

#if DEBUG
            ribbonPanel.GetRibbonPanel().CustomPanelTitleBarBackground = System.Windows.Media.Brushes.Salmon;
#endif
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

            Log.Finish();

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
                Log.WriteLine(ex);
            }
        }

        #region ribbons
        static string LargeImageNoBusy = RibbonUtils.TestPass;
        static string LargeImageIsBusy = RibbonUtils.TestFail;
        static string LargeImageRun = RibbonUtils.TestWait;
        private static void UpdateLargeImageBusy(RibbonItem ribbonItem, RevitBusyService control)
        {
            if (ribbonItem is null) return;
            if (control.IsRevitBusy)
                ribbonItem.SetLargeImage(LargeImageIsBusy);
            else
                ribbonItem.SetLargeImage(LargeImageNoBusy);
        }
        #endregion
    }
}