using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter
{
    [DefaultExecutorUri(ExecutorUriString)]
    [FileExtension(".dll")]
    public class TestDiscoverer : TestAdapter, ITestDiscoverer
    {
        public void DiscoverTests(
            IEnumerable<string> sources, IDiscoveryContext discoveryContext,
            IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            Initialize(logger);

            foreach (var source in sources)
            {
                TestLog.Info($"DiscoverTests: {source}");
            }

            GetTests(this, sources, discoverySink);
        }

        /// <summary>
        /// Get All Tests using RevitTest.Console -r
        /// </summary>
        /// <param name="TestAdapter"></param>
        /// <param name="sources"></param>
        /// <param name="discoverySink"></param>
        /// <returns></returns>
        internal static List<TestCase> GetTests(
            TestAdapter TestAdapter,
            IEnumerable<string> sources,
            ITestCaseDiscoverySink discoverySink = null)
        {
            List<TestCase> tests = new List<TestCase>();
            var task = Task.Run(async () =>
            {
                foreach (var source in sources)
                {
                    using (var revit = new RevitTestConsole())
                    {
                        var testNames = await revit.RunTestRead(source);
                        TestAdapter.TestLog.Info($"DiscoverTests: {testNames.ToJson()}");

                        foreach (var testName in testNames)
                        {
                            TestAdapter.TestLog.Info($"Test: {testName}");
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