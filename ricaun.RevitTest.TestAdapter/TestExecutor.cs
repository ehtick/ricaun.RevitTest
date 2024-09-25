using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using ricaun.NUnit.Models;
using ricaun.RevitTest.TestAdapter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter
{
    [ExtensionUri(ExecutorUriString)]
    internal class TestExecutor : TestAdapter, ITestExecutor
    {
        /// <summary>
        /// RunTests
        /// </summary>
        /// <param name="testCases"></param>
        /// <param name="runContext"></param>
        /// <param name="frameworkHandle"></param>
        public void RunTests(IEnumerable<TestCase> testCases, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Initialize(runContext, frameworkHandle);

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
            Initialize(runContext, frameworkHandle);

            var task = Task.Run(async () =>
            {
                foreach (var source in sources)
                {
                    await RunTests(frameworkHandle, source);
                }
            });
            task.GetAwaiter().GetResult();
        }

        public void Cancel()
        {
        }

        private static bool IsLogDebug => System.Diagnostics.Debugger.IsAttached || AdapterSettings.Settings.NUnit.Verbosity > 2;

        /// <summary>
        /// Run Tests using RevitTest.Console -t
        /// </summary>
        /// <param name="frameworkHandle"></param>
        /// <param name="source"></param>
        /// <param name="tests"></param>
        /// <returns></returns>
        private async Task RunTests(IFrameworkHandle frameworkHandle, string source, List<TestCase> tests = null)
        {
            tests = tests ?? new List<TestCase>();

            AdapterLogger.Logger.Info($"RunTest: {source} [TestCase: {tests.Count}]");
            foreach (var test in tests)
            {
                AdapterLogger.Logger.Info($"Test: {test.FullyQualifiedName}.{test.DisplayName}");
            }
            Metadatas.MetadataSettings.Create(source);

            AdapterLogger.Logger.Info("---------");
            AdapterLogger.Logger.Info($"RevitTestConsole: {AdapterSettings.Settings.NUnit.Application}");
            AdapterLogger.Logger.Info("---------");

            using (var revit = new RevitTestConsole(AdapterSettings.Settings.NUnit.Application, source))
            {
                if (revit.IsTrusted(out string message) == false)
                {
                    AdapterLogger.Logger.Error(message);
#if !DEBUG
                    return;
#endif
                }

                var filters = tests.Select(TestCaseUtils.GetFullName).ToArray();
                foreach (var filter in filters)
                {
                    AdapterLogger.Logger.Debug($"\tTestFilter: {filter}");
                }

                AdapterLogger.Logger.Info($"RunRevitTest: {source} [Version: {AdapterSettings.Settings.NUnit.Version}] [TestFilter: {filters.Length}]");

                await revit.RunExecuteTests(source, (testModel) => { RecordResultTestModel(frameworkHandle, source, tests, testModel); },
                    AdapterSettings.Settings.NUnit.Version,
                    AdapterSettings.Settings.NUnit.Language,
                    AdapterSettings.Settings.NUnit.Open,
                    AdapterSettings.Settings.NUnit.Close,
                    AdapterSettings.Settings.NUnit.Timeout,
                    AdapterLogger.Logger.Debug, (message) => { if (IsLogDebug) AdapterLogger.Logger.Debug(message); }, AdapterLogger.Logger.Warning, 
                    filters);

            }

            AdapterLogger.Logger.Info("---------");
        }

        private static void RecordResultTestModel(IFrameworkHandle frameworkHandle, string source, List<TestCase> tests, TestModel testModel)
        {
            TestCase testCase = TryFindSimilarTestCaseUsingTestModel(tests, testModel);
            var needToCreateTestCase = testCase is null;
            if (needToCreateTestCase)
            {
                testCase = TestCaseUtils.Create(source, testModel.FullName);
            }

            AdapterLogger.Logger.Info($"\tTestCase: {testCase} [{testCase.DisplayName}] \t{testCase.Id}");

            testCase.LocalExtensionData = testModel;
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

        /// <summary>
        /// Try find SimilarTestName
        /// </summary>
        /// <param name="tests"></param>
        /// <param name="testModel"></param>
        /// <returns></returns>
        private static TestCase TryFindSimilarTestCaseUsingTestModel(List<TestCase> tests, TestModel testModel)
        {
            // if test not found try to find similar test with only FullyQualifiedName
            return
                tests.FirstOrDefault(e => TestCaseUtils.IsSimilarTestName(e, testModel.FullName)) ??
                tests.FirstOrDefault(e => TestCaseUtils.IsSimilarTestName(e, testModel.FullName, true));
        }
    }
}