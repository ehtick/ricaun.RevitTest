using CommandLine;
using NamedPipeWrapper;
using ricaun.Revit.Installation;
using ricaun.RevitTest.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Console
{
    class Options
    {
        [Option('f', "file",
            Required = true,
            HelpText = "Input file to be processed.")]
        public string File { get; set; }

        [Option('o', "output",
            HelpText = "Output file processed.")]
        public string Output { get; set; }

        [Option('v', "version",
          HelpText = "Force to run with Revit version.")]
        public int Version { get; set; }

        [Option('l', "log",
          Default = false,
          HelpText = "Prints all messages to log in standard output.")]
        public bool Log { get; set; }
    }

    internal class Program
    {
        static Program()
        {
            //CosturaUtility.Initialize();
        }
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options options)
        {
            Log.Enabled = options.Log;

            Log.WriteLine();
            ricaun.NUnit.TestEngine.Initialize(out string init);
            Log.WriteLine(init);
            Log.WriteLine();

            var file = new FileInfo(options.File).FullName;

            Log.WriteLine($"File: {file}");

            var output = "";
            if (File.Exists(file))
            {
                string directoryResolver = null;
                if (RevitUtils.TryGetRevitVersion(file, out int revitVersion))
                {
                    Log.WriteLine($"File with Revit Version: {revitVersion}");
                    if (options.Version != 0)
                    {
                        revitVersion = options.Version;
                        Log.WriteLine($"Force to use Revit Version: {revitVersion}");
                    }
                    if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersion, out RevitInstallation revitInstallation))
                    {
                        Log.WriteLine($"Revit Installation: {revitInstallation}");
                        directoryResolver = revitInstallation.InstallLocation;
                    }
                    Log.WriteLine();
                }
                var tests = ShowTestNamesAssembly(options.File, directoryResolver);

                output = tests.ToJson();


                Log.WriteLine();
            }

            if (string.IsNullOrEmpty(output) == false)
            {
                Log.WriteLine($"Output: {output}");
                Log.WriteLine();
                try
                {
                    File.WriteAllText(options.Output, output);
                }
                catch { }

            }

            //ShowTestNamesAssembly(GetFile("SampleTest.Tests/SampleTest.Tests.dll"));
            //Console.WriteLine();
            //ShowTestNamesAssembly(GetFile("RevitAddin.UnitTest/RevitAddin.UnitTest.dll"), @"C:\Program Files\Autodesk\Revit 2018");

            Log.WriteLine(output);
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var installedRevit in RevitInstallationUtils.InstalledRevit)
            {
                foreach (var process in installedRevit.GetProcesses())
                {
                    Log.WriteLine($"{installedRevit} {process.GetPipeName()} {process.PipeFileExists()}");
                    if (process.PipeFileExists())
                    {
                        var client = new PipeTestClient(process);
                        client.Request = new TestRequest();
                        client.Initialize();
                        Thread.Sleep(5000);
                        client.Dispose();
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        static void HandleParseErrorOld(IEnumerable<Error> errs)
        {
            var bundleUrl = Properties.Resources.ricaun_RevitTest_Application_bundle
                .CopyToFile("ricaun.RevitTest.Application.bundle.zip");

            var bundleLocal = @"D:\Users\ricau\source\repos\ricaun.RevitTest\ricaun.RevitTest.Application\bin\ReleaseFiles\ricaun.RevitTest.Application.bundle.zip";
            var bundleResource = @"D:\Users\ricau\source\repos\ricaun.RevitTest\ricaun.RevitTest.Console\Resources\ricaun.RevitTest.Application.bundle.zip";
            if (File.Exists(bundleLocal) == true)
            {
                try
                {
                    File.Copy(bundleLocal, bundleResource, true);
                }
                catch { }
                bundleUrl = bundleLocal;

            }

            var applicationPluginsFolder = RevitUtils.GetCurrentUserApplicationPluginsFolder();
            var bundleName = Path.GetFileNameWithoutExtension(bundleUrl);
            //Console.WriteLine(applicationPluginsFolder);
            Log.WriteLine(bundleUrl);
            Log.WriteLine($"DownloadBundle: {bundleName}");
            ApplicationPluginsUtils.DownloadBundle(applicationPluginsFolder, bundleUrl);

            //Thread.Sleep(5000);

            if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallation(2023, out RevitInstallation revitInstallation))
            {
                if (revitInstallation.TryGetProcess(out Process process) == false)
                {
                    Log.WriteLine($"{revitInstallation}: Start");
                    process = revitInstallation.Start();

                    var client = new NamedPipeClient<MessageString>(process.GetPipeName());
                    var clientConnected = false;
                    var clientIsBusy = true;
                    client.ServerMessage += (server, message) =>
                    {
                        Log.WriteLine($"> {message}");
                        clientConnected = true;
                        clientIsBusy = message.IsBusy;
                    };
                    client.Start();

                    for (int i = 0; i < 60; i++)
                    {

                        if (clientConnected)
                        {
                            if (clientIsBusy == false)
                            {
                                //Console.WriteLine("Send: Source");
                                client.PushMessage(new MessageString() { Source = "" });
                            }
                            //client.PushMessage(new MessageString());
                        }
                        else
                        {
                            Log.WriteLine($"{revitInstallation}: Wait {i}");
                        }
                        Thread.Sleep(1000);
                    }

                    client.Stop();
                    if (!process.HasExited)
                        process.Kill();
                }
            }

            ApplicationPluginsUtils.DeleteBundle(applicationPluginsFolder, bundleName);

            //ShowTestNamesAssembly(GetFile("SampleTest.Tests/SampleTest.Tests.dll"));
            //ShowTestNamesAssembly(GetFile("RevitAddin.UnitTest/RevitAddin.UnitTest.dll"), @"C:\Program Files\Autodesk\Revit 2018");
        }


        public static void ShowInstalledRevit()
        {
            var installedRevits = RevitInstallationUtils.InstalledRevit;
            foreach (var installedRevit in installedRevits)
            {
                Log.WriteLine(installedRevit);
                if (installedRevit.TryGetProcess(out Process process))
                {
                    Log.WriteLine($"\t{process}");
                }
            }
        }

        private static string[] ShowTestNamesAssembly(string file, string directoryResolver = null)
        {
            if (!File.Exists(file))
                file = GetFile(file);

            var tests = new List<string>();

            Log.WriteLine(Path.GetFileName(file) + " " + directoryResolver);
            //using (new AssemblyResolveService(directoryResolver))
            {
                var testNames = ricaun.NUnit.TestEngine.GetTestFullNames(new FileInfo(file).FullName, directoryResolver);
                foreach (var testName in testNames)
                {
                    Log.WriteLine($"\t{testName}");
                    tests.Add(testName);
                }
            }
            return tests.ToArray();
        }

        private static string GetFile(string file)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(directory, file);
        }

        class MessageString
        {
            public string Source { get; set; }
            public string Text { get; set; }
            public DateTime Date { get; set; }
            public int Id { get; set; }
            public string Revit { get; set; }
            public bool IsBusy { get; set; }
            public override string ToString()
            {
                return $"{Text} {IsBusy}";
            }
        }
    }

}
