using ricaun.NUnit;
using ricaun.NUnit.Models;
using ricaun.Revit.UI.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit
{
    public static class TestExecuteUtils
    {
        private const string FOLDER_NAME_TEMP = "RevitTest";

        public static async Task<TestAssemblyModel> ExecuteAsync(IRevitTask revitTask, string filePath, params object[] parameters)
        {
            Log.WriteLine($"TestExecuteUtils: {filePath}");
            var zipFile = await revitTask.Run(() => CopyFilesUsingZipFolder(filePath));

            if (zipFile is null)
            {
                Log.WriteLine($"TestExecuteUtils: Copy Zip Fail");
                throw new Exception("Copy Zip Fail");
            }

            string zipDestination = null;
            var extractToTempSucess = await revitTask.Run(() => ZipExtension.ExtractToTempFolder(zipFile, out zipDestination));

            if (extractToTempSucess == false)
            {
                Log.WriteLine($"TestExecuteUtils: Extract Zip Fail");
                throw new Exception("Extract Zip Fail");
            }

            //TestAssemblyModel tests = await revitTask.Run(() => TestDirectory(zipDestination, parameters));
            TestAssemblyModel tests = await TestDirectoryAsync(revitTask, zipDestination, Path.GetFileName(filePath), parameters);

            await revitTask.Run(() => CopyFilesBackUsingZip(filePath, zipDestination));

            return tests;
        }
        private static string CopyFilesUsingZipFolder(string filePath)
        {
            if (filePath is null)
                return null;

            var directory = Path.Combine(Path.GetTempPath(), FOLDER_NAME_TEMP);

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

        private static async Task<TestAssemblyModel> TestDirectoryAsync(IRevitTask revitTask, string directory, string filePattern, params object[] parameters)
        {
            TestAssemblyModel modelTest = null;

            Log.WriteLine("----------------------------------");
            Log.WriteLine($"TestEngine: {ricaun.NUnit.TestEngine.Initialize(out string testInitialize)} {testInitialize}");
            Log.WriteLine("----------------------------------");

            foreach (var filePath in Directory.GetFiles(directory, filePattern)) // "*.dll"
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    if (TestEngine.ContainNUnit(filePath))
                    {
                        TestEngineFilter.CancellationTokenTimeOut = TimeSpan.FromMinutes(1);
#if DEBUG
                        TestEngineFilter.CancellationTokenTimeOut = TimeSpan.FromSeconds(3);
#endif

                        var configurationMetadata = ConfigurationMetadata.GetConfigurationMetadata(filePath);

                        if (configurationMetadata.TimeOut > 0)
                        {
                            TestEngineFilter.CancellationTokenTimeOut = TimeSpan.FromSeconds(configurationMetadata.TimeOut);
                            Log.WriteLine($"TimeOut: {configurationMetadata.TimeOut}");
                        }

                        string containTestNameForNoRevitContext = configurationMetadata.Name;
                        if (string.IsNullOrEmpty(containTestNameForNoRevitContext) == false)
                        {
                            if (TestEngineFilter.ExplicitEnabled == false)
                            {
                                TestEngineFilter.ExplicitEnabled = false;
                                TestEngineFilter.TestNames.AddRange(TestEngine.GetTestFullNames(filePath));
                            }

                            var originalTestNames = TestEngineFilter.TestNames.ToArray();
                            var testGroupNoContext = originalTestNames
                                .GroupBy(str => str.Contains(containTestNameForNoRevitContext))
                                .ToDictionary(group => group.Key, group => group.ToList());

                            if (testGroupNoContext.TryGetValue(false, out var inContextTestNames))
                            {
                                TestEngineFilter.TestNames.Clear();
                                TestEngineFilter.TestNames.AddRange(inContextTestNames);
                                modelTest = await revitTask.Run(() => TestEngine.TestAssembly(filePath, parameters));
                            }

                            if (testGroupNoContext.TryGetValue(true, out var testAsyncNames))
                            {
                                Log.WriteLine($"TestAsync: [{string.Join(",", testAsyncNames)}]");
                                TestEngineFilter.TestNames.Clear();
                                TestEngineFilter.TestNames.AddRange(testAsyncNames);
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

                        if (modelTest is null)
                        {
                            modelTest = await revitTask.Run(() => TestEngine.TestAssembly(filePath, parameters));
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
                //catch (FileNotFoundException ex)
                //{
                //    Debug.WriteLine($"Debug: {fileName} {ex}");
                //}
                catch (Exception ex)
                {
                    Log.WriteLine($"Error: {fileName} {ex}");
                }
            }

            Log.WriteLine("----------------------------------");

            return modelTest;
        }

        private class ConfigurationComments
        {
            public string TestAsync { get; set; }
            public double TimeOut { get; set; }
        }

        private class ConfigurationMetadata
        {
            public const string TasksTimeout = "ricaun.RevitTest.Application.Tasks.Timeout";
            public const string TasksName = "ricaun.RevitTest.Application.Tasks.Name";
            public string Name { get; set; }
            public double TimeOut { get; set; }

            public static ConfigurationMetadata GetConfigurationMetadata(string filePath)
            {
                var configurationMetadata = new ConfigurationMetadata();
                try
                {
                    //return GetConfigurationMetadata(Assembly.Load(File.ReadAllBytes(filePath)));
                    var attributes = TestEngine.GetAssemblyMetadataAttributes(filePath);
                    configurationMetadata.Name = attributes.Get(TasksName);
                    configurationMetadata.TimeOut = attributes.GetDouble(TasksTimeout);
                }
                catch { }
                return configurationMetadata;
            }

            //public static ConfigurationMetadata GetConfigurationMetadata(Assembly assembly)
            //{
            //    return new ConfigurationMetadata
            //    {
            //        Name = assembly.Get(TasksName),
            //        TimeOut = assembly.GetDouble(TasksTimeout)
            //    };
            //}
        }
    }
}
