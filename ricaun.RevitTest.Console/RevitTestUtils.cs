using ricaun.NUnit;
using ricaun.Revit.Installation;
using ricaun.RevitTest.Console.Extensions;
using ricaun.RevitTest.Console.Utils;
using ricaun.RevitTest.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ricaun.RevitTest.Console
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
            bool forceToCloseRevit = false)
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
                        client.NamedPipe.ServerMessage += (c, m) =>
                        {
                            if (m.Test is not null)
                            {
                                Log.WriteLine($"{revitInstallation}: {m.Test.Time} \t {m.Test}");
                                actionOutput?.Invoke(m.Test.ToJson());
                            }
                        };
                        client.ServerMessage.PropertyChanged += (s, e) =>
                        {
                            Log.WriteLine($"{revitInstallation}: PropertyChanged[ {e.PropertyName} ]");
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
                                    Log.WriteLine($"{revitInstallation}: TestFile {Path.GetFileName(fileToTest)}");
                                    client.Update((request) =>
                                    {
                                        request.TestPathFile = fileToTest;
                                    });
                                }
                            }

                            if (sendFileWhenCreatedOrUpdated)
                            {
                                Log.WriteLine($"{revitInstallation}: TestFile {Path.GetFileName(fileToTest)}");
                                Thread.Sleep(100);
                                client.Update((request) =>
                                {
                                    request.TestPathFile = fileToTest;
                                });
                                sendFileWhenCreatedOrUpdated = false;
                            }
                        }

                        client.Dispose();

                        //if (forceToCloseRevit)
                        //    processStarted = true;

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