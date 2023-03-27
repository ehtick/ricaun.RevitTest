using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using ricaun.RevitTest.TestAdapter.Extensions;
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

        private static System.Guid GetGuid(string name)
        {
            return new System.Guid(name.GetHashCode(), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        public static List<TestCase> GetTests(TestAdapter TestAdapter, IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink = null)
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
                            var fullyQualifiedName = testName.Substring(0, testName.LastIndexOf('.'));
                            var displayName = testName.Substring(testName.LastIndexOf('.') + 1);
                            var testcase = new TestCase(fullyQualifiedName, ExecutorUri, source)
                            {
                                DisplayName = displayName,
                                Id = GetGuid($"{testName}"),
                            };
                            TestAdapter.TestLog.Info($"Test: {testName} - {testName}");

                            discoverySink?.SendTestCase(testcase);
                            tests.Add(testcase);
                        }
                    }
                }
            });
            task.GetAwaiter().GetResult();
            return tests;
        }

    }
}