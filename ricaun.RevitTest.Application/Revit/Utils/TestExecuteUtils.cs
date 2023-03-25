using Autodesk.Revit.DB;
using ricaun.NUnit;
using ricaun.NUnit.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.Application.Revit
{
    public static class TestExecuteUtils
    {
        public static object Execute(string filePath, string versionNumber, params object[] parameters)
        {
            if (filePath is null)
                return null;

            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location);

            string copyPath = null;
            if (Path.GetExtension(filePath).EndsWith("zip"))
            {
                copyPath = CopyFile(filePath, directory);
            }
            else if (Path.GetExtension(filePath).EndsWith("dll"))
            {
                copyPath = ZipExtension.CreateFromDirectory(
                    Path.GetDirectoryName(filePath),
                    Path.Combine(directory, Path.GetFileName(filePath))
                    );
            }
            else return null;

            var tests = UnZipAndTestFiles(directory, versionNumber, parameters);

            if (copyPath is not null)
                File.Delete(copyPath);

            return tests;
        }

        private static string CopyFile(string filePath, string directory)
        {
            var copy = Path.Combine(directory, Path.GetFileName(filePath));
            File.Copy(filePath, copy, true);
            return copy;
        }

        private static object UnZipAndTestFiles(string directory, string versionNumber, params object[] parameters)
        {
            if (Directory.GetFiles(directory, "*.zip").FirstOrDefault() is string zipFile)
            {
                if (ZipExtension.ExtractToFolder(zipFile, out string zipDestination))
                {
                    if (string.IsNullOrEmpty(versionNumber) == false)
                    {
                        foreach (var versionDirectory in Directory.GetDirectories(zipDestination))
                        {
                            if (Path.GetFileName(versionDirectory).Equals(versionNumber))
                            {
                                Console.WriteLine($"Test VersionNumber: {versionNumber}");
                                return TestDirectory(versionDirectory, parameters);
                            }
                        }
                    }

                    return TestDirectory(zipDestination, parameters);
                }
            }

            return null;
        }

        private static object TestDirectory(string directory, params object[] parameters)
        {
            NUnit.Models.TestAssemblyModel modelTest = null;

            Console.WriteLine("----------------------------------");
            Console.WriteLine($"TestEngine: {ricaun.NUnit.TestEngine.Initialize(out string testInitialize)} {testInitialize}");
            Console.WriteLine("----------------------------------");

            foreach (var filePath in Directory.GetFiles(directory, "*.dll"))
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    if (TestEngine.ContainNUnit(filePath))
                    {
                        //Console.WriteLine($"Test File: {fileName}");
                        //foreach (var testName in TestEngine.GetTestFullNames(filePath))
                        //{
                        //    Console.WriteLine($"\t{testName}");
                        //}

                        modelTest = TestEngine.TestAssembly(
                            filePath, parameters);

                        //System.Windows.Clipboard.SetText(Newtonsoft.Json.JsonConvert.SerializeObject(modelTest));
                        //System.Windows.Clipboard.SetText(modelTest.AsString());

                        var passed = modelTest.Success ? "PASSED" : "FAILED";
                        if (modelTest.TestCount == 0) { passed = "NO TESTS"; }
                        Console.WriteLine($"{modelTest}\t {passed}");

                        var tests = modelTest.Tests.SelectMany(e => e.Tests);

                        foreach (var test in tests)
                        {
                            Console.WriteLine($"\t {test.Time}\t {test}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {fileName} {ex}");
                }
            }

            Console.WriteLine("----------------------------------");

            return modelTest;
        }
    }
}
