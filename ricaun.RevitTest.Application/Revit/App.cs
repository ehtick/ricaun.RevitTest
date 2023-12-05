using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Busy;
using ricaun.NUnit;
using ricaun.NUnit.Models;
using ricaun.Revit.Async.Services;
using ricaun.Revit.UI;
using ricaun.RevitTest.Application.Revit.ApsApplication;
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
        private static RevitTaskService RevitTask;
        private static RevitBusyService RevitBusyService;
        private static bool IsTestRunning = false;

        private const int TestThreadSleepMin = 50;
        private const int TestAfterFinishSleepTime = 100;

        public Result OnStartup(UIControlledApplication application)
        {
            Log.Initilize(application);

            RevitBusyService = new RevitBusyService(application);
            RevitBusyService.PropertyChanged += RevitBusyControlPropertyChanged;

            RevitTask = new RevitTaskService();
            RevitTask.Initialize();
            ricaun.Revit.Async.RevitTask.Initialize();

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

            Task.Run(async () =>
            {
                await ApsApplication.ApsApplication.Initialize();
            }).GetAwaiter().GetResult();

            return Result.Succeeded;
        }

        private static void PipeTestServer_Initialize()
        {
            PipeTestServer = new PipeTestServer();
            PipeTestServer.Update(response =>
            {
                response.IsBusy = RevitBusyService.IsRevitBusy;
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
                        if (string.IsNullOrEmpty(message.Info))
                            return;

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

                    IsTestRunning = true;
                    PipeTestServer.Update((response) =>
                    {
                        response.IsBusy = true;
                        response.Test = null;
                        response.Info = null;
                        response.Tests = null;
                    });

                    TestAssemblyModel ApsApplicationCheckTest(UIApplication uiapp)
                    {
                        try
                        {
                            if (Autodesk.Revit.ApplicationServices.Application.IsLoggedIn == false)
                            {
                                var exceptionNeedLoggedIn = new Exception("There is no user connected to Revit.");
                                Log.WriteLine(exceptionNeedLoggedIn.Message);
                                PipeTestServer.Update((response) =>
                                {
                                    response.Info = exceptionNeedLoggedIn.Message;
                                });

                                var isPreviewRelease = ApplicationPreviewUtils.IsPreviewRelease();
                                if (isPreviewRelease)
                                {
                                    var previewReleaseMessage = "Preview Release allow Revit user not logged.";
                                    Log.WriteLine(previewReleaseMessage);
                                    PipeTestServer.Update((response) =>
                                    {
                                        response.Info = previewReleaseMessage;
                                    });
                                }
                                else
                                {
                                    return TestEngine.Fail(message.TestPathFile, exceptionNeedLoggedIn, testFilterNames);
                                }
                            }

                            if (ApsApplication.ApsApplication.IsConnected == false)
                            {
                                PipeTestServer.Update((response) =>
                                {
                                    response.Info = "The user is not connected with 'ricaun.Auth' and Autodesk Platform Service. 'ricaun.Auth' opened in Revit and waiting for the login...";
                                });

                                ApsApplication.ApsApplicationView.OpenApsView(true);
                            }

                            if (ApsApplication.ApsApplication.IsConnected == false)
                            {
                                var exceptionNeedAuth = new Exception("The user is not connected with 'ricaun.Auth' and Autodesk Platform Service.");
                                return TestEngine.Fail(message.TestPathFile, exceptionNeedAuth, testFilterNames);
                            }

                            if (ApsApplication.ApsApplication.IsConnected == true)
                            {
                                var task = Task.Run(async () =>
                                {
                                    return await ApsApplicationCheck.Check();
                                });
                                var apsResponse = task.GetAwaiter().GetResult();
                                if (apsResponse is not null && apsResponse.isValid == false)
                                {
                                    var exceptionNotValid = new Exception($"The user is not valid, {apsResponse?.message}");
                                    return TestEngine.Fail(message.TestPathFile, exceptionNotValid, testFilterNames);
                                }
                            }

                            if (ApsApplication.ApsApplication.IsConnected == true)
                            {
                                //var task = Task.Run(async () =>
                                //{
                                //    return await ApsApplication.ApsApplication.EnsureApsUserHaveOpenId();
                                //});
                                //var userHaveOpenId = task.GetAwaiter().GetResult();
                                //if (userHaveOpenId == false)
                                //{
                                //    var exceptionUserWithoutOpenId = new Exception($"The user was disconnected, reconnect the user to 'ricaun.Auth' and Autodesk Platform Service.");
                                //    return TestEngine.Fail(message.TestPathFile, exceptionUserWithoutOpenId, testFilterNames);
                                //}

                                var revitLoginUserId = uiapp.Application.LoginUserId;
                                var apsLoginUserId = ApsApplication.ApsApplication.LoginUserId;
                                var isSameUserId = apsLoginUserId.Equals(revitLoginUserId);
                                var isRevitLoginUserIdEmpty = string.IsNullOrEmpty(revitLoginUserId);

                                if (isRevitLoginUserIdEmpty)
                                {
                                    Log.WriteLine($"Revit Login UserId is empty.");
                                }

                                if (isSameUserId == false && isRevitLoginUserIdEmpty == false)
                                {
                                    Log.WriteLine($"ApsApplication: '{revitLoginUserId}' != '{apsLoginUserId}'");
                                    var exceptionDifferentLoginUserId = new Exception($"The user connected to Revit is different from the user connected to 'ricaun.Auth'.");
                                    return TestEngine.Fail(message.TestPathFile, exceptionDifferentLoginUserId, testFilterNames);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                        return null;
                    }
                    var testAssemblyModel = await RevitTask.Run(ApsApplicationCheckTest);

                    if (testAssemblyModel is null)
                    {
                        ricaun.NUnit.TestEngine.Result = new TestModelResult((test) =>
                        {
                            //Debug.WriteLine($"\t TestModelResult: {test}");
                            PipeTestServer.Update((response) =>
                            {
                                response.IsBusy = true;
                                response.Test = test;
                                response.Info = null;
                                response.Tests = null;
                            });
                            if (test.Time < TestThreadSleepMin) System.Threading.Thread.Sleep(TestThreadSleepMin);
                        });

                        {
                            foreach (var testFilterName in testFilterNames)
                            {
                                ricaun.NUnit.TestEngineFilter.Add(testFilterName);
                            }

                            try
                            {
                                testAssemblyModel = await TestExecuteUtils.ExecuteAsync(RevitTask, message.TestPathFile, RevitParameters.Parameters);

                                await RevitTask.Run((uiapp) =>
                                {
                                    try
                                    {
                                        var task = Task.Run(async () =>
                                        {
                                            var modelTests = testAssemblyModel.Tests.SelectMany(e => e.Tests).ToArray();
                                            await ApsApplication.ApsApplicationLogger.Log("Test", $"{uiapp.Application.VersionName}", modelTests.Length);
                                        });
                                        task.GetAwaiter().GetResult();
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex);
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLine($"TestExecuteUtils: Exception {ex}");
                                var exceptionFail = new Exception("TestExecuteUtils.Execute Fails", ex);
                                testAssemblyModel = TestEngine.Fail(message.TestPathFile, exceptionFail, testFilterNames);
                            }

                            if (testAssemblyModel == null)
                            {
                                var exceptionFail = new Exception("TestExecuteUtils.Execute is null");
                                testAssemblyModel = TestEngine.Fail(message.TestPathFile, exceptionFail, testFilterNames);
                            }

                            ricaun.NUnit.TestEngineFilter.Reset();
                        }

                        ricaun.NUnit.TestEngine.Result = null;
                    }

                    PipeTestServer.Update((response) =>
                    {
                        response.IsBusy = true;
                        response.Test = null;
                        response.Info = null;
                        response.Tests = testAssemblyModel;
                    });

                    // Todo: Send back the zip files
                    await Task.Delay(TestAfterFinishSleepTime);
                    IsTestRunning = false;
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
                .SetToolTip("Open RevitTest.log File")
                .SetLargeImage(RibbonUtils.RevitTest);

            var ribbon = ribbonPanel.CreatePushButton<ApsApplication.CommandApsView>("ricaun.Auth")
                .SetToolTip("Open dialog to Login/Logout with Autodesk Platform Service.");
            ribbonPanel.SetDialogLauncher(ribbon);

            UpdateLargeImageBusy(ribbonItem, RevitBusyService);

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
            RevitBusyService?.Dispose();

            RevitTask?.Dispose();

            Log.Finish();

            return Result.Succeeded;
        }

        private void RevitBusyControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (IsTestRunning) return;

            var control = sender as RevitBusyService;
            UpdateLargeImageBusy(ribbonItem, control);
            try
            {
                PipeTestServer?.Update(response =>
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