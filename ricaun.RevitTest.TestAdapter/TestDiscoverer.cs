using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Metadatas;
using ricaun.RevitTest.TestAdapter.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter
{
    [DefaultExecutorUri(ExecutorUriString)]
    [FileExtension(".dll")]
    internal class TestDiscoverer : TestAdapter, ITestDiscoverer
    {
        public void DiscoverTests(
            IEnumerable<string> sources, IDiscoveryContext discoveryContext,
            IMessageLogger messageLogger, ITestCaseDiscoverySink discoverySink)
        {
            Initialize(discoveryContext, messageLogger);

            var tests = GetTests(sources, discoverySink);

            if (tests.Any() == false)
                AdapterLogger.Logger.Warning($"DiscoverTests: Tests not found [{string.Join(" ", sources)}]");

        }

        /// <summary>
        /// Check if the application is disable to skip the test discovery
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        internal static bool IsApplicationDisable(string application)
        {
            if (string.IsNullOrWhiteSpace(application))
                return false;

            string[] disableNames = new string[] { "disable", "none", "null" };
            foreach (var disableName in disableNames)
            {
                return (application.Equals(disableName, StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }

        /// <summary>
        /// Get All Tests using RevitTest.Console -r
        /// </summary>
        /// <param name="TestAdapter"></param>
        /// <param name="sources"></param>
        /// <param name="discoverySink"></param>
        /// <returns></returns>
        internal static List<TestCase> GetTests(
            IEnumerable<string> sources,
            ITestCaseDiscoverySink discoverySink = null)
        {
            List<TestCase> tests = new List<TestCase>();
            var task = Task.Run(async () =>
            {
                foreach (var source in sources)
                {
                    MetadataSettings.Create(source);
                    EnvironmentSettings.Create();
                    AdapterLogger.Logger.Info($"DiscoverTests: {source}");

                    var application = AdapterSettings.Settings.NUnit.Application;
                    if (IsApplicationDisable(application))
                    {
                        AdapterLogger.Logger.Warning($"DiscoverTests: Application is {application}.");
                        continue;
                    }
                    using (var revit = new RevitTestConsole(application, source))
                    {
                        if (revit.IsTrusted(out string message) == false)
                        {
                            AdapterLogger.Logger.Error(message);
#if !DEBUG
                            break;
#endif
                        }

                        var testNames = new string[] { };

                        await revit.RunReadTests(source, (tests) => { testNames = tests; },
                            AdapterLogger.Logger.Debug,
                            AdapterLogger.Logger.Debug,
                            AdapterLogger.Logger.Warning);

                        AdapterLogger.Logger.Debug("DiscoverTests: -------------------------------");
                        AdapterLogger.Logger.Info($"DiscoverTests: {testNames.ToJson()}");
                        AdapterLogger.Logger.Debug("DiscoverTests: -------------------------------");

                        foreach (var testName in testNames)
                        {
                            var testCase = TestCaseUtils.Create(source, testName);

                            AdapterLogger.Logger.Info($"TestCase: {testCase} [{testCase.DisplayName}] \t{testCase.Id}");

                            discoverySink?.SendTestCase(testCase);
                            tests.Add(testCase);
                        }

                        AdapterLogger.Logger.Debug("DiscoverTests: -------------------------------");
                    }
                }
            });
            task.GetAwaiter().GetResult();
            return tests;
        }

    }
}