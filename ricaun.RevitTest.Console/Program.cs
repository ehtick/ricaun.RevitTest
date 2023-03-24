using CommandLine;
using ricaun.Revit.Installation;
using ricaun.RevitTest.Console.Extensions;
using ricaun.RevitTest.Console.Utils;
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

    internal class Program
    {
        static void Main(string[] args)
        {

            // RevitProcessServerSelect(); return;

            CommandLine.Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

        }
        static void RunOptions(Options options)
        {
            var runCommand = new RunCommand(options);
            runCommand.Run();

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Log.WriteLine($"{a} {a.Location}");
            }

            Thread.Sleep(1000);

            return;

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
        static void HandleParseError2(IEnumerable<Error> errs)
        {
            //var task = Task.Run(async () =>
            //{
            //    var process = Process.GetProcesses()
            //        .Where(e => e.ProcessName == "Revit")
            //        .FirstOrDefault();
            //    if (process is null) return;

            //    var client = new NamedPipeClient<string>(process.GetPipeName());

            //    client.Connected += (connection) =>
            //    {
            //        Log.WriteLine($"[{connection.Id}] Connected");
            //    };

            //    client.Disconnected += (connection) =>
            //    {
            //        Log.WriteLine($"[{connection.Id}] Disconnected");
            //    };

            //    client.ServerMessage += (connection, message) =>
            //    {
            //        Log.WriteLine($"[{connection.Id}] ServerMessage: \t{message}");
            //    };


            //    Log.WriteLine(process.GetPipeName());
            //    client.Start();
            //    client.WaitForConnection(1000);

            //    await Task.Delay(10000);

            //    client.Stop();
            //    client.WaitForDisconnection(1000);
            //    await Task.Delay(1000);


            //});
            //task.GetAwaiter().GetResult();


            var t = Task.Run(async () =>
            {
                var clients = GetRevitPipeTestClient();
                foreach (var client in clients)
                {
                    //client.Request = new TestRequest() { Id = 5 };
                    client.Initialize();
                }

                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(1000);
                    //Log.WriteLine(".");
                }

                foreach (var client in clients)
                    client.Dispose();


            });
            t.GetAwaiter().GetResult();


        }

        private static IList<PipeTestClient> GetRevitPipeTestClient()
        {
            var clients = new List<PipeTestClient>();
            foreach (var installedRevit in RevitInstallationUtils.InstalledRevit)
            {
                foreach (var process in installedRevit.GetProcesses())
                {
                    Log.WriteLine($"{installedRevit} {process.GetPipeName()} {process.PipeFileExists()}");
                    if (process.PipeFileExists())
                    {
                        var client = new PipeTestClient(process);
                        clients.Add(client);
                    }
                }
            }
            return clients;
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
            if (errors.IsHelp()) return;
            if (errors.IsVersion()) return;

            foreach (var error in errors)
            {
                Log.WriteLine($"Error: {error}");
            }

            RevitProcessServerSelect();
        }

        static void RevitProcessServerSelect()
        {
            Task.Run(RevitProcessServerSelectAsync).GetAwaiter().GetResult();
        }

        static async Task RevitProcessServerSelectAsync()
        {
            await Task.Delay(0);

            var assemblyName = typeof(Program).Assembly.GetName();
            Log.WriteLine();
            Log.WriteLine($"{assemblyName.Name} {assemblyName.Version.ToString(3)}");

            var fileToTest = App.FilePath;

            if (string.IsNullOrEmpty(fileToTest))
            {
                Log.WriteLine();
                Log.Write("FileToTest: ");
                var line = System.Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    fileToTest = @"C:\Users\ricau\source\repos\TestProject.Tests\TestProject.Tests\bin\Debug\TestProject.Tests.dll";
                    Log.WriteLine(Path.GetFileName(fileToTest));
                }
                else
                {
                    fileToTest = line;
                }

                if (!File.Exists(fileToTest))
                {
                    return;
                }
            }

            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            //{
            //    Log.WriteLine($"FirstChanceException {s}");
            //    Log.WriteLine(e.Exception);
            //};

            //AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            //{
            //    Log.WriteLine($"UnhandledException {s}");
            //    Log.WriteLine(e);
            //};


            foreach (var test in RevitTestUtils.GetTestFullNames(fileToTest))
            {
                Log.WriteLine(test);
            }

            var installedRevits = RevitInstallationUtils.InstalledRevit;
            ConsoleKeyInfo keyLoop;
            do
            {
                Log.WriteLine();
                for (int i = 0; i < installedRevits.Length; i++)
                {
                    Log.WriteLine($"[NumPad{i + 1}] {installedRevits[i]}");
                }
                Log.WriteLine();
                keyLoop = System.Console.ReadKey(true);

                var revitVersionNumber = 0;

                var number = keyLoop.Key - ConsoleKey.NumPad1;
                if (keyLoop.Key == ConsoleKey.Spacebar)
                {

                    if (!RevitUtils.TryGetRevitVersion(fileToTest, out revitVersionNumber))
                    {
                        break;
                    }

                    Log.WriteLine($"TestFile {Path.GetFileName(fileToTest)} | Revit {revitVersionNumber}");
                }
                else
                {
                    if (number < 0) break;
                    if (number >= installedRevits.Length) break;

                    revitVersionNumber = installedRevits[number].Version;
                }

                CreateRevitServer(revitVersionNumber, fileToTest);

            } while (keyLoop.Key != ConsoleKey.Escape);

            Log.WriteLine("...");

            Thread.Sleep(1000);
        }

        //private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    Log.WriteLine($"{args.Name} {args.RequestingAssembly}");
        //    return null;
        //}

        private static void CreateRevitServer(int revitVersionNumber, string fileToTest)
        {
            if (revitVersionNumber == 0)
                return;

            var sendFileWhenCreatedOrUpdated = true;

            Action<string, bool> resetSendFile = (file, exists) =>
            {
                sendFileWhenCreatedOrUpdated = exists;
            };

            using (new FileWatcher().Initialize(fileToTest, resetSendFile))
            {

                using (new ApplicationPluginsDisposable(
                                    Properties.Resources.ricaun_RevitTest_Application_bundle,
                                    "ricaun.RevitTest.Application.bundle.zip"))
                {
                    if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersionNumber, out RevitInstallation revitInstallation))
                    {
                        Log.WriteLine(revitInstallation);
                        var processStarted = false;
                        if (revitInstallation.TryGetProcess(out Process process) == false)
                        {
                            Log.WriteLine($"{revitInstallation}: Start");
                            process = revitInstallation.Start();
                            processStarted = true;
                        }

                        var client = new PipeTestClient(process);
                        client.Initialize();


                        for (int i = 0; i < 10 * 60; i++)
                        {
                            //Log.WriteLine($"{revitInstallation}: Wait {i}");
                            Thread.Sleep(1000);
                            if (process.HasExited) break;

                            if (client.ServerMessage is null)
                                continue;

                            if (client.ServerMessage.IsBusy)
                                continue;

                            if (System.Console.KeyAvailable)
                            {
                                var cki = System.Console.ReadKey(true);
                                if (cki.Key == ConsoleKey.Escape) break;
                                if (cki.Key == ConsoleKey.Spacebar)
                                {
                                    Log.WriteLine($"{revitInstallation}: TestFile {Path.GetFileName(fileToTest)}");
                                    client.Update((request) =>
                                    {
                                        request.TestPathFile = fileToTest;
                                    });
                                }
                            }

                            if (sendFileWhenCreatedOrUpdated)
                            {
                                Log.WriteLine($"{revitInstallation}: TestFile {Path.GetFileName(fileToTest)}");
                                Thread.Sleep(100);
                                client.Update((request) =>
                                {
                                    request.TestPathFile = fileToTest;
                                });
                                sendFileWhenCreatedOrUpdated = false;
                            }

                        }

                        client.Dispose();

                        if (processStarted)
                        {
                            if (!process.HasExited)
                                process.Kill();

                            Log.WriteLine($"{revitInstallation}: Exited");
                        }

                    }

                }
            }
        }

        static void HandleParseError3(IEnumerable<Error> errs)
        {


            var bundleUrl = Properties.Resources.ricaun_RevitTest_Application_bundle
                .CopyToFile("ricaun.RevitTest.Application.bundle.zip");

            //var bundleLocal = @"D:\Users\ricau\source\repos\ricaun.RevitTest\ricaun.RevitTest.Application\bin\ReleaseFiles\ricaun.RevitTest.Application.bundle.zip";
            //var bundleResource = @"D:\Users\ricau\source\repos\ricaun.RevitTest\ricaun.RevitTest.Console\Resources\ricaun.RevitTest.Application.bundle.zip";
            //if (File.Exists(bundleLocal) == true)
            //{
            //    try
            //    {
            //        File.Copy(bundleLocal, bundleResource, true);
            //    }
            //    catch { }
            //    bundleUrl = bundleLocal;

            //}

            var applicationPluginsFolder = RevitUtils.GetCurrentUserApplicationPluginsFolder();
            var bundleName = Path.GetFileNameWithoutExtension(bundleUrl);
            Log.WriteLine(bundleUrl);
            Log.WriteLine($"DownloadBundle: {bundleName}");
            ApplicationPluginsUtils.DownloadBundle(applicationPluginsFolder, bundleUrl);

            if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallation(2021, out RevitInstallation revitInstallation))
            {
                if (revitInstallation.TryGetProcess(out Process process) == false)
                {
                    Log.WriteLine($"{revitInstallation}: Start");
                    process = revitInstallation.Start();

                    var client = new PipeTestClient(process);
                    client.Initialize();

                    for (int i = 0; i < 2 * 60; i++)
                    {
                        Log.WriteLine($"{revitInstallation}: Wait {i}");
                        Thread.Sleep(1000);
                    }

                    client.Dispose();
                    if (!process.HasExited)
                        process.Kill();
                }
            }

            ApplicationPluginsUtils.DeleteBundle(applicationPluginsFolder, bundleName);

            Thread.Sleep(3000);

            /*
            if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallation(2020, out RevitInstallation revitInstallation))
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
                            Log.WriteLine($"{revitInstallation}: Wait {i}");
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
            */
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
    }
}