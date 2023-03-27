using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter
{
    [ExtensionUri(ExecutorUriString)]
    public class TestExecutor : TestAdapter, ITestExecutor
    {
        public void RunTests(IEnumerable<TestCase> testCases, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Initialize(frameworkHandle);

            var testCasesBySources = testCases
                .GroupBy(e => e.Source)
                .ToDictionary(e => e.Key, e => e.ToList());

            var task = Task.Run(async () =>
            {
                foreach (var testCasesBySource in testCasesBySources)
                {
                    var source = testCasesBySource.Key;
                    var tests = testCasesBySource.Value;

                    TestLog.Info($"RunTest: {source} {tests.Count}");

                    using (var revit = new RevitTestConsole())
                    {
                        var filters = testCases.Select(e => $"{e.FullyQualifiedName}.{e.DisplayName}").ToArray();

                        Action<string> outputConsole = (item) =>
                        {
                            if (string.IsNullOrEmpty(item)) return;
                            TestLog.Info($"RunTest: {item.Trim()}");
                            if (item.StartsWith("{\"FileName"))
                            {
                                var testAssembly = item.Deserialize<TestAssemblyModel>();
                            }
                            if (item.StartsWith("{\"Name"))
                            {
                                if (item.Deserialize<TestModel>() is TestModel testModel)
                                {
                                    var testCase = testCases.FirstOrDefault(e => $"{e.FullyQualifiedName}.{e.DisplayName}".Equals(testModel.FullName));
                                    TestLog.Info($"TestModel: {testModel} {testCase}");
                                    var testResult = new TestResult(testCase);

                                    testResult.Outcome = TestOutcome.Failed;
                                    if (testModel.Success)
                                        testResult.Outcome = TestOutcome.Passed;
                                    if (testModel.Skipped)
                                        testResult.Outcome = TestOutcome.Skipped;

                                    testResult.Duration = new TimeSpan((long)(TimeSpan.TicksPerMillisecond * testModel.Time));

                                    testResult.ErrorStackTrace = testModel.Message;
                                    testResult.Messages.Clear();

                                    //if (testModel.Skipped)
                                    //    testResult.Traits.Add("Name", testModel.Message);

                                    if (testModel.Skipped)
                                        testResult.ErrorMessage = testModel.Message;

                                    testResult.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, testModel.Console));

                                    frameworkHandle.RecordResult(testResult);
                                }
                            }
                        };

                        await revit.RunTestAction(source, 0, outputConsole, filters);

                    }
                }
            });
            task.GetAwaiter().GetResult();

        }

        /// <summary>
        /// Run with using 'dotnet'
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="runContext"></param>
        /// <param name="frameworkHandle"></param>
        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Initialize(frameworkHandle);

            foreach (var source in sources)
                TestLog.Info($"RunTests: {source}");

            TestLog.Debug("RunTests by IEnumerable<TestCase>. RunType = Ide");
            IEnumerable<TestCase> tests = TestDiscoverer.GetTests(this, sources, null);
            RunTests(tests, runContext, frameworkHandle);
        }

        public void Cancel()
        {
        }
    }
}