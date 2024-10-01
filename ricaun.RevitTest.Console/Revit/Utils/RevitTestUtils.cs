using ricaun.NUnit;
using ricaun.Revit.Installation;
using ricaun.RevitTest.Command;
using ricaun.RevitTest.Command.Extensions;
using ricaun.RevitTest.Command.Utils;
using ricaun.RevitTest.Console.Revit.Services;
using ricaun.RevitTest.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ricaun.RevitTest.Console.Revit.Utils
{
    /// <summary>
    /// RevitTestUtils
    /// </summary>
    public static class RevitTestUtils
    {
        private const int SleepMillisecondsBeforeFinish = 1000;
        private const int SleepMillisecondsDebuggerAttached = 1000;

        private const int TimeoutMinutesDefault = 10;

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
                LoggerTest($"RevitTestUtils: {revitVersion}");

                if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersion, out RevitInstallation revitInstallation))
                {
                    LoggerTest($"GetTest Version {revitVersion}");
                    Log.WriteLine($"RevitTestUtils: {revitInstallation.InstallLocation}");
                    tests = TestEngine.GetTestFullNames(filePath, revitInstallation.InstallLocation);
                }

                foreach (var exception in TestEngine.Exceptions)
                {
                    Log.WriteLine($"RevitTestUtils.Exceptions: {exception}");
                }
                if (tests.Length == 0)
                {
                    throw new Exception($"Unable to read tests class using the Revit version {revitVersion}.");
                }
            }

#if DEBUG
            LoggerTest($"Test °C");
            LoggerTest($"Length {tests.Length}");
            LoggerTest($"TestEngine {TestEngine.Version.ToString(3)}");
            LoggerTest($"{AppUtils.GetInfo()}");
            if (LoggerTests.Any())
            {
                LoggerTests.AddRange(tests);
                return LoggerTests.ToArray();
            }
#endif

            return tests;
        }

        #region Debug
        private static List<string> LoggerTests = new List<string>();
        [Conditional("DEBUG")]
        private static void LoggerTest(object logger)
        {
            var loggerTest = $"{typeof(RevitTestUtils).FullName}(\"{logger}\")";
            Debug.WriteLine(loggerTest);
            LoggerTests.Add(loggerTest);
        }
        #endregion

        /// <summary>
        /// Create Revit Server
        /// </summary>
        /// <param name="fileToTest"></param>
        /// <param name="revitVersionNumber"></param>
        /// <param name="actionOutput"></param>
        /// <param name="forceLanguageToRevit"></param>
        /// <param name="forceToOpenNewRevit"></param>
        /// <param name="forceToCloseRevit"></param>
        /// <param name="timeoutMinutes"></param>
        /// <param name="testFilters"></param>
        public static void CreateRevitServer(
            string fileToTest,
            int revitVersionNumber,
            Action<string> actionOutput = null,
            string forceLanguageToRevit = null,
            bool forceToOpenNewRevit = false,
            bool forceToCloseRevit = false,
            int timeoutMinutes = 0,
            params string[] testFilters)
        {
            int timeoutNotBusyCountMax = 10;

            if (timeoutMinutes <= 0)
                timeoutMinutes = TimeoutMinutesDefault;

            if (revitVersionNumber == 0)
            {
                RevitUtils.TryGetRevitVersion(fileToTest, out revitVersionNumber);
            }

            if (!RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersionNumber, out RevitInstallation revitInstallationNotExist))
            {
                var exceptionRevitNotExist = new Exception($"Installed Revit with version {revitVersionNumber} or greater not found.");

                var failTests = TestEngine.Fail(fileToTest, exceptionRevitNotExist, testFilters);
                actionOutput.Invoke(failTests.ToJson());

                Thread.Sleep(SleepMillisecondsBeforeFinish);
                return;
            }

            int timeoutNotBusyCount = 0;
            bool sendFileWhenCreatedOrUpdated = true;
            bool testsFinishedForceToEnd = false;
            bool timeoutForceToEnd = false;

            Action<string, bool> resetSendFile = (file, exists) =>
            {
                sendFileWhenCreatedOrUpdated = exists;
            };

            using (new FileWatcher().Initialize(fileToTest, resetSendFile))
            {
                using (var appPlugin = CreateApplicationPlugin())
                {
                    Debug.WriteLine($"Application Install: {appPlugin.Initialized}");

                    if (appPlugin.Initialized == false)
                    {
                        var exceptionAppPlugin = new Exception("Application not initialized.");
                        var failTests = TestEngine.Fail(fileToTest, exceptionAppPlugin, testFilters);
                        actionOutput.Invoke(failTests.ToJson());
                        Thread.Sleep(SleepMillisecondsBeforeFinish);
                        return;
                    }

                    if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersionNumber, out RevitInstallation revitInstallation))
                    {
                        Log.WriteLine(revitInstallation);

                        if (revitInstallation.TryGetProcess(out Process process) == false || forceToOpenNewRevit)
                        {
                            var startRevitLanguageArgument = RevitLanguageUtils.GetArgument(forceLanguageToRevit);
                            startRevitLanguageArgument = string.Join(" ", startRevitLanguageArgument, Environment.GetEnvironmentVariable("RICAUN_REVITTEST_CONSOLE_PROCESS_ARGUMENTS"));
                            Log.WriteLine($"{revitInstallation}: Start {startRevitLanguageArgument}");
                            process = revitInstallation.Start(startRevitLanguageArgument);
                        }

                        var client = new PipeTestClient(process);
                        client.Initialize();

                        client.ServerMessage.PropertyChanged += (s, e) =>
                        {
                            var message = s as TestResponse;
                            LogDebug.WriteLine($"{revitInstallation}: PropertyChanged[ {e.PropertyName} ]");
                            LogDebug.WriteLine($"{revitInstallation}: {message}");
                            switch (e.PropertyName)
                            {
                                case nameof(TestResponse.Test):
                                    var test = message.Test;
                                    if (test is not null)
                                    {
                                        Log.WriteLine($"{revitInstallation}: {test.Time} \t {test}");
                                        actionOutput?.Invoke(test.ToJson());
                                        testsFinishedForceToEnd = false;
                                    }
                                    break;
                                case nameof(TestResponse.Tests):
                                    var tests = message.Tests;
                                    if (tests is not null)
                                    {
                                        Log.WriteLine($"{revitInstallation}: {tests.Time} \t {tests}");
                                        actionOutput?.Invoke(null); // Force to clear file
                                        actionOutput?.Invoke(tests.ToJson());
                                        testsFinishedForceToEnd = true;
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

                        var timeoutSeconds = timeoutMinutes * 60;
                        for (int i = 0; i <= timeoutSeconds; i++)
                        {
                            Thread.Sleep(1000);

                            if (i % 30 == 0 && i > 0)
                            {
                                Log.WriteLine($"{revitInstallation}: Execution time is {i / 60.0} minutes, maximum {timeoutMinutes} minutes.");
                            }

                            if (process.HasExited) break;

                            if (client.ServerMessage is null)
                                continue;

                            if (client.ServerMessage.IsBusy)
                                timeoutNotBusyCount = 0;
                            else
                                timeoutNotBusyCount++;

                            if (timeoutNotBusyCountMax > 0 && timeoutNotBusyCount > timeoutNotBusyCountMax)
                            {
                                var timeoutMessage = $"RevitTest: Timeout not busy for too long {timeoutNotBusyCountMax} seconds.";
                                Log.WriteLine($"{revitInstallation}: {timeoutMessage}");
                                var exceptionTimeoutTests = new Exception(timeoutMessage);
                                var timeoutTests = TestEngine.Fail(fileToTest, exceptionTimeoutTests, testFilters);
                                foreach (var testModel in timeoutTests.Tests.SelectMany(e => e.Tests))
                                {
                                    actionOutput.Invoke(testModel.ToJson());
                                }
                                Thread.Sleep(SleepMillisecondsBeforeFinish);
                                break;
                            }

                            if (i == timeoutSeconds)
                                timeoutForceToEnd = true;

                            if (timeoutForceToEnd)
                            {
                                var timeoutMessage = $"RevitTest: Timeout {timeoutMinutes} minutes.";
                                Log.WriteLine($"{revitInstallation}: {timeoutMessage}");
                                var exceptionTimeoutTests = new Exception(timeoutMessage);
                                var timeoutTests = TestEngine.Fail(fileToTest, exceptionTimeoutTests, testFilters);
                                foreach (var testModel in timeoutTests.Tests.SelectMany(e => e.Tests))
                                {
                                    actionOutput?.Invoke(testModel.ToJson());
                                }
                                Thread.Sleep(SleepMillisecondsBeforeFinish);
                                break;
                            }

                            if (client.ServerMessage.IsBusy)
                                continue;

                            if (testsFinishedForceToEnd)
                            {
                                Log.WriteLine($"{revitInstallation}: Tests Finished");
                                break;
                            }

                            if (sendFileWhenCreatedOrUpdated)
                            {
                                i = 0;
                                Log.WriteLine($"{revitInstallation}: TestFile: {Path.GetFileName(fileToTest)} TestFilters:{testFilters.ToJson()}");
                                process.AttachDTE();
                                if (DebuggerUtils.IsDebuggerAttached) Thread.Sleep(SleepMillisecondsDebuggerAttached);

                                client.Update((request) =>
                                {
                                    request.TestFilters = testFilters;
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
                        if (DebuggerUtils.IsDebuggerAttached) Thread.Sleep(SleepMillisecondsDebuggerAttached);

                        if (forceToCloseRevit)
                        {
                            if (!process.HasExited)
                            {
                                Thread.Sleep(SleepMillisecondsBeforeFinish);
                                process.Kill();
                            }

                            Log.WriteLine($"{revitInstallation}: Exited");
                        }

                        Log.WriteLine($"{revitInstallation}: Finish");
                    }

                }
            }
        }

        #region private
        private static ApplicationPluginsDisposable CreateApplicationPlugin()
        {
            var application = new ApplicationPluginsDisposable(
                                Properties.Resources.ricaun_RevitTest_Application_bundle,
                                "ricaun.RevitTest.Application.bundle.zip");

            application.SetException(ApplicationException);
            application.SetLog(ApplicationLog);

            return application.Initialize();
        }

        private static void ApplicationException(Exception exception)
        {
            LogDebug.WriteLine($"Application Error: {exception.Message}");
        }

        private static void ApplicationLog(string log)
        {
            LogDebug.WriteLine($"Application: {log}");
        }
        #endregion
    }

    public static class LogDebug
    {
        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLine(string value)
        {
            Debug.WriteLine(value);
            if (DebuggerUtils.IsDebuggerAttached)
            {
                Log.WriteLine($"Debug: {value}");
            }
        }
    }
}