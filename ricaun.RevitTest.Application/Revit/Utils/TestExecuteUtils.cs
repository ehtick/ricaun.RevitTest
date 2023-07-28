using ricaun.NUnit;
using ricaun.NUnit.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.Application.Revit
{
    public static class TestExecuteUtils
    {
        public static TestAssemblyModel Execute(string filePath, params object[] parameters)
        {
            if (filePath is null)
                return null;

            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location);

            if (Path.GetExtension(filePath).EndsWith("dll") == false)
                return null;

            var copyPathBack = true;
            var copyPath = ZipExtension.CreateFromDirectory(
                Path.GetDirectoryName(filePath),
                Path.Combine(directory, Path.GetFileName(filePath))
                );

            //var copyPathBack = false;
            //string copyPath = null;
            //if (Path.GetExtension(filePath).EndsWith("zip"))
            //{
            //    copyPath = CopyFile(filePath, directory);
            //}
            //else if (Path.GetExtension(filePath).EndsWith("dll"))
            //{
            //    copyPathBack = true;
            //    copyPath = ZipExtension.CreateFromDirectory(
            //        Path.GetDirectoryName(filePath),
            //        Path.Combine(directory, Path.GetFileName(filePath))
            //        );
            //}
            //else return null;

            //var tests = UnZipAndTestFiles(directory, versionNumber, parameters);

            TestAssemblyModel tests = null;

            if (ZipExtension.ExtractToTempFolder(copyPath, out string zipDestination))
            {
                tests = TestDirectory(zipDestination, parameters);
            }

            if (copyPath is not null)
                File.Delete(copyPath);

            if (copyPathBack)
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

            return tests;
        }

        //private static string CopyFile(string filePath, string directory)
        //{
        //    var copy = Path.Combine(directory, Path.GetFileName(filePath));
        //    File.Copy(filePath, copy, true);
        //    return copy;
        //}

        //private static object UnZipAndTestFiles(string directory, string versionNumber, params object[] parameters)
        //{
        //    if (Directory.GetFiles(directory, "*.zip").FirstOrDefault() is string zipFile)
        //    {
        //        if (ZipExtension.ExtractToTempFolder(zipFile, out string zipDestination))
        //        {
        //            if (string.IsNullOrEmpty(versionNumber) == false)
        //            {
        //                foreach (var versionDirectory in Directory.GetDirectories(zipDestination))
        //                {
        //                    if (Path.GetFileName(versionDirectory).Equals(versionNumber))
        //                    {
        //                        Log.WriteLine($"Test VersionNumber: {versionNumber}");
        //                        return TestDirectory(versionDirectory, parameters);
        //                    }
        //                }
        //            }

        //            return TestDirectory(zipDestination, parameters);
        //        }
        //    }

        //    return null;
        //}

        private static TestAssemblyModel TestDirectory(string directory, params object[] parameters)
        {
            NUnit.Models.TestAssemblyModel modelTest = null;

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

                        modelTest = TestEngine.TestAssembly(
                            filePath, parameters);

                        //System.Windows.Clipboard.SetText(Newtonsoft.Json.JsonConvert.SerializeObject(modelTest));
                        //System.Windows.Clipboard.SetText(modelTest.AsString());

                        var passed = modelTest.Success ? "Passed" : "Failed";
                        if (modelTest.TestCount == 0) { passed = "No Tests"; }

                        Log.WriteLine($"{modelTest}\t {passed}");

                        var tests = modelTest.Tests.SelectMany(e => e.Tests);

                        foreach (var test in tests)
                        {
                            Log.WriteLine($"\t {test.Time}\t {test}");
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
