using ricaun.NUnit;
using ricaun.Revit.Installation;
using ricaun.RevitTest.Console.Extensions;
using ricaun.RevitTest.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ricaun.RevitTest.Console.Utils
{
    public static class RevitTestUtils
    {
        /// <summary>
        /// Get Test Full Names using RevitInstallation if needed (Revit +2021)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[] GetTestFullNames(string filePath)
        {
            var tests = TestEngine.GetTestFullNames(filePath);
            if (RevitUtils.TryGetRevitVersion(filePath, out var revitVersion))
            {
                Log.WriteLine($"RevitTestUtils: {revitVersion}");
                if (tests.Length == 0)
                {
                    // Problem with AnavRes.dll / adui22res.dll (version -2020)
                    revitVersion = Math.Max(revitVersion, 2021);
                    if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersion, out RevitInstallation revitInstallation))
                    {
                        Log.WriteLine($"RevitTestUtils: {revitInstallation.InstallLocation}");
                        tests = TestEngine.GetTestFullNames(filePath, revitInstallation.InstallLocation);
                    }
                }
            }
            return tests;
        }

        /// <summary>
        /// Create Revit Server
        /// </summary>
        /// <param name="fileToTest"></param>
        /// <param name="revitVersionNumber"></param>
        /// <param name="actionOutput"></param>
        /// <param name="forceToOpenNewRevit"></param>
        /// <param name="forceToWaitRevit"></param>
        /// <param name="forceToCloseRevit"></param>
        public static void CreateRevitServer(
            string fileToTest,
            int revitVersionNumber,
            Action<string> actionOutput = null,
            bool forceToOpenNewRevit = false,
            bool forceToWaitRevit = false,
            bool forceToCloseRevit = false,
            string testFilter = null)
        {
            int timeoutCountMax = forceToWaitRevit ? 0 : 1;

            if (revitVersionNumber == 0)
            {
                RevitUtils.TryGetRevitVersion(fileToTest, out revitVersionNumber);
            }

            int timeoutCount = 0;
            bool sendFileWhenCreatedOrUpdated = true;

            Action<string, bool> resetSendFile = (file, exists) =>
            {
                sendFileWhenCreatedOrUpdated = exists;
            };

            using (new FileWatcher().Initialize(fileToTest, resetSendFile))
            {
                using (CreateAppPlugin())
                {
                    if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersionNumber, out RevitInstallation revitInstallation))
                    {
                        Log.WriteLine(revitInstallation);
                        //var processStarted = false;
                        if (revitInstallation.TryGetProcess(out Process process) == false || forceToOpenNewRevit)
                        {
                            Log.WriteLine($"{revitInstallation}: Start");
                            process = revitInstallation.Start();
                            //processStarted = true;
                        }

                        var client = new PipeTestClient(process);
                        client.Initialize();

                        client.ServerMessage.PropertyChanged += (s, e) =>
                        {
                            var message = s as TestResponse;
                            Debug.WriteLine($"{revitInstallation}: PropertyChanged[ {e.PropertyName} ]");
                            Debug.WriteLine($"{revitInstallation}: {message}");
                            switch (e.PropertyName)
                            {
                                case nameof(TestResponse.Test):
                                    var test = message.Test;
                                    if (test is not null)
                                    {
                                        Log.WriteLine($"{revitInstallation}: {test.Time} \t {test}");
                                        actionOutput?.Invoke(test.ToJson());
                                    }
                                    break;
                                case nameof(TestResponse.Tests):
                                    var tests = message.Tests;
                                    if (tests is not null)
                                    {
                                        Log.WriteLine($"{revitInstallation}: {tests.Time} \t {tests}");
                                        actionOutput?.Invoke(null); // Force to clear file
                                        actionOutput?.Invoke(tests.ToJson());
                                    }
                                    break;
                                case nameof(TestResponse.Info):
                                    var text = message.Info;
                                    if (text is not null)
                                    {
                                        Log.WriteLine($"{revitInstallation}: {text}");
                                    }
                                    break;
                            }
                        };

                        for (int i = 0; i < 10 * 60; i++)
                        {
                            Thread.Sleep(1000);

                            if (process.HasExited) break;

                            if (client.ServerMessage is null)
                                continue;

                            if (client.ServerMessage.IsBusy)
                                timeoutCount = 0;
                            else
                                timeoutCount++;

                            if (timeoutCountMax > 0 && timeoutCount > timeoutCountMax)
                                break;

                            if (client.ServerMessage.IsBusy)
                                continue;

                            if (System.Console.KeyAvailable)
                            {
                                var cki = System.Console.ReadKey(true);
                                if (cki.Key == ConsoleKey.Escape) break;
                                if (cki.Key == ConsoleKey.Spacebar)
                                {
                                    sendFileWhenCreatedOrUpdated = true;
                                }
                            }

                            if (sendFileWhenCreatedOrUpdated)
                            {
                                i = 0;
                                Log.WriteLine($"{revitInstallation}: TestFile {Path.GetFileName(fileToTest)}");
                                process.AttachDTE();
                                client.Update((request) =>
                                {
                                    request.TestFilter = testFilter;
                                    request.TestPathFile = fileToTest;
                                    request.Info = AppUtils.GetInfo();
                                });
                                sendFileWhenCreatedOrUpdated = false;
                            }
                        }

                        client.Dispose();

                        //if (forceToCloseRevit)
                        //    processStarted = true;

                        process.DetachDTE();

                        if (forceToCloseRevit)
                        {
                            if (!process.HasExited)
                                process.Kill();

                            Log.WriteLine($"{revitInstallation}: Exited");
                        }

                        Log.WriteLine($"{revitInstallation}: Finish");
                    }

                }
            }
        }

        #region private
        private static ApplicationPluginsDisposable CreateAppPlugin()
        {
            return new ApplicationPluginsDisposable(
                        Properties.Resources.ricaun_RevitTest_Application_bundle,
                        "ricaun.RevitTest.Application.bundle.zip");
        }
        #endregion
    }
}