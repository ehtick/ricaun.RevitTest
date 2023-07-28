using ricaun.NUnit;
using ricaun.NUnit.Models;
using ricaun.Revit.Async.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit
{
    public static class TestExecuteUtils
    {
        public static async Task<TestAssemblyModel> ExecuteAsync(IRevitTask revitTask, string filePath, params object[] parameters)
        {
            var zipFile = await revitTask.Run(() => CopyFilesUsingZipFolder(filePath));

            if (zipFile is null)
                return null;

            string zipDestination = null;
            var extractToTempSucess = await revitTask.Run(() => ZipExtension.ExtractToTempFolder(zipFile, out zipDestination));

            if (extractToTempSucess == false)
                return null;

            //TestAssemblyModel tests = await revitTask.Run(() => TestDirectory(zipDestination, parameters));
            TestAssemblyModel tests = await TestDirectoryAsync(revitTask, zipDestination, parameters);

            await revitTask.Run(() => CopyFilesBackUsingZip(filePath, zipDestination));

            return tests;
        }



        public static TestAssemblyModel Execute(string filePath, params object[] parameters)
        {
            var zipFile = CopyFilesUsingZipFolder(filePath);

            if (zipFile is null)
                return null;

            var extractToTempSucess = ZipExtension.ExtractToTempFolder(zipFile, out string zipDestination);

            if (extractToTempSucess == false)
                return null;

            TestAssemblyModel tests = TestDirectory(zipDestination, parameters);

            CopyFilesBackUsingZip(filePath, zipDestination);

            return tests;
        }

        private static string CopyFilesUsingZipFolder(string filePath)
        {
            if (filePath is null)
                return null;

            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location);

            if (Path.GetExtension(filePath).EndsWith("dll") == false)
                return null;

            var zipFile = ZipExtension.CreateFromDirectory(
                Path.GetDirectoryName(filePath),
                Path.Combine(directory, Path.GetFileName(filePath))
                );

            return zipFile;
        }

        private static void CopyFilesBackUsingZip(string filePath, string zipDestination)
        {
            try
            {
                var zipCopyBack = zipDestination + ".zip";

                if (File.Exists(zipCopyBack))
                    File.Delete(zipCopyBack);

                var zipCopyPathBack = ZipExtension.CreateFromDirectory(zipDestination, zipCopyBack);

                ZipExtension.ExtractToDirectoryIfNewer(zipCopyPathBack, Path.GetDirectoryName(filePath));

                if (File.Exists(zipCopyPathBack))
                    File.Delete(zipCopyPathBack);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
            }
        }

        private static async Task<TestAssemblyModel> TestDirectoryAsync(IRevitTask revitTask, string directory, params object[] parameters)
        {
            TestAssemblyModel modelTest = null;

            Log.WriteLine("----------------------------------");
            Log.WriteLine($"TestEngine: {ricaun.NUnit.TestEngine.Initialize(out string testInitialize)} {testInitialize}");
            Log.WriteLine("----------------------------------");

            foreach (var filePath in Directory.GetFiles(directory, "*.dll"))
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    if (TestEngine.ContainNUnit(filePath))
                    {
                        TestEngineFilter.CancellationTokenTimeOut = TimeSpan.FromMinutes(1);

                        // Custom Configuration
                        {
                            string targetSubstring = "Async";

                            if (TestEngineFilter.ExplicitEnabled == false)
                            {
                                TestEngineFilter.ExplicitEnabled = false;
                                TestEngineFilter.TestNames.AddRange(TestEngine.GetTestFullNames(filePath));
                            }

                            var originalTestNames = TestEngineFilter.TestNames.ToArray();
                            var groupedStrings = originalTestNames
                                .GroupBy(str => str.Contains(targetSubstring))
                                .ToDictionary(group => group.Key, group => group.ToList());

                            if (groupedStrings.TryGetValue(false, out var inContextTests))
                            {
                                TestEngineFilter.TestNames.Clear();
                                TestEngineFilter.TestNames.AddRange(inContextTests);
                                modelTest = await revitTask.Run(() => TestEngine.TestAssembly(filePath, parameters));
                            }

                            if (groupedStrings.TryGetValue(true, out var asyncTests))
                            {
                                TestEngineFilter.TestNames.Clear();
                                TestEngineFilter.TestNames.AddRange(asyncTests);
                                var modelTestAsync = TestEngine.TestAssembly(filePath, parameters);
                                if (modelTest is null) modelTest = modelTestAsync;
                                else
                                {
                                    modelTest.Tests.AddRange(modelTestAsync.Tests);
                                }
                            }

                            TestEngineFilter.TestNames.Clear();
                            TestEngineFilter.TestNames.AddRange(originalTestNames);
                        }

                        var passed = modelTest.Success ? "Passed" : "Failed";
                        if (modelTest.TestCount == 0) { passed = "No Tests"; }

                        Log.WriteLine($"{modelTest}\t {passed}");

                        var tests = modelTest.Tests.SelectMany(e => e.Tests);

                        foreach (var test in tests)
                        {
                            Log.WriteLine($"\t {test.Time}\t {test}");
                            //Debug.WriteLine($"Debug:\t {test}\t {test.Console.Trim()}");
                        }

                        if (tests.Any() == false)
                        {
                            Log.WriteLine($"Error: {modelTest.Message}");
                            try
                            {
                                var ex = new Exception(modelTest.Message.Split('\n').FirstOrDefault());
                                modelTest = TestEngine.Fail(filePath, ex);
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine($"Error: {fileName} {ex}");
                }
            }

            Log.WriteLine("----------------------------------");

            return modelTest;
        }

        private static TestAssemblyModel TestDirectory(string directory, params object[] parameters)
        {
            TestAssemblyModel modelTest = null;

            Log.WriteLine("----------------------------------");
            Log.WriteLine($"TestEngine: {ricaun.NUnit.TestEngine.Initialize(out string testInitialize)} {testInitialize}");
            Log.WriteLine("----------------------------------");

            foreach (var filePath in Directory.GetFiles(directory, "*.dll"))
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    if (TestEngine.ContainNUnit(filePath))
                    {
                        //Log.WriteLine($"Test File: {fileName}");
                        //foreach (var testName in TestEngine.GetTestFullNames(filePath))
                        //{
                        //    Log.WriteLine($"\t{testName}");
                        //}

                        TestEngineFilter.CancellationTokenTimeOut = TimeSpan.FromMinutes(1);

                        modelTest = TestEngine.TestAssembly(filePath, parameters);

                        var passed = modelTest.Success ? "Passed" : "Failed";
                        if (modelTest.TestCount == 0) { passed = "No Tests"; }

                        Log.WriteLine($"{modelTest}\t {passed}");

                        var tests = modelTest.Tests.SelectMany(e => e.Tests);

                        foreach (var test in tests)
                        {
                            Log.WriteLine($"\t {test.Time}\t {test}");
                            //Debug.WriteLine($"Debug:\t {test}\t {test.Console.Trim()}");
                        }

                        if (tests.Any() == false)
                        {
                            Log.WriteLine($"Error: {modelTest.Message}");
                            try
                            {
                                var ex = new Exception(modelTest.Message.Split('\n').FirstOrDefault());
                                modelTest = TestEngine.Fail(filePath, ex);
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine($"Error: {fileName} {ex}");
                }
            }

            Log.WriteLine("----------------------------------");

            return modelTest;
        }
    }
}
