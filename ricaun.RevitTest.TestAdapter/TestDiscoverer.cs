using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter
{
    [DefaultExecutorUri(ExecutorUriString)]
    [FileExtension(".dll")]
    public class TestDiscoverer : TestAdapter, ITestDiscoverer
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
                    Metadatas.MetadataSettings.Create(source);
                    AdapterLogger.Logger.Info($"DiscoverTests: {source}");
                    using (var revit = new RevitTestConsole(AdapterSettings.Settings.NUnit.Application))
                    {
                        var testNames = await revit.RunTestRead(source);
                        AdapterLogger.Logger.Info($"DiscoverTests: {testNames.ToJson()}");

                        foreach (var testName in testNames)
                        {
                            AdapterLogger.Logger.Info($"Test: {testName}");
                            var testCase = TestCaseUtils.Create(source, testName);

                            discoverySink?.SendTestCase(testCase);
                            tests.Add(testCase);
                        }
                    }
                }
            });
            task.GetAwaiter().GetResult();
            return tests;
        }

    }
}