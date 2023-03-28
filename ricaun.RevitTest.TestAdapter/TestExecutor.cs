using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Models;
using ricaun.RevitTest.TestAdapter.Services;
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

                    await RunTests(frameworkHandle, source, tests);
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

            var task = Task.Run(async () =>
            {
                foreach (var source in sources)
                {
                    await RunTests(frameworkHandle, source);
                }
            });
            task.GetAwaiter().GetResult();
        }


        private async Task RunTests(IFrameworkHandle frameworkHandle, string source, List<TestCase> tests = null)
        {
            tests = tests ?? new List<TestCase>();

            TestLog.Info($"RunTest: {source} {tests.Count}");

            using (var revit = new RevitTestConsole())
            {
                var filters = tests.Select(e => $"{e.FullyQualifiedName}.{e.DisplayName}").ToArray();

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
                            var testCase = tests.FirstOrDefault(e => $"{e.FullyQualifiedName}.{e.DisplayName}".Equals(testModel.FullName));

                            if (testCase is null)
                            {
                                testCase = TestCaseUtils.Create(source, testModel.FullName);
                            }

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


        public void Cancel()
        {
        }
    }
}