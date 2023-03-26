using CommandLine;
using ricaun.Revit.Installation;
using ricaun.RevitTest.Console.Command;
using ricaun.RevitTest.Console.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options options)
        {
            var runCommand = new RunCommand(options);
            runCommand.Run();
        }
        static void HandleParseError(IEnumerable<Error> errors)
        {
            if (errors.IsHelp()) return;
            if (errors.IsVersion()) return;

            //foreach (var error in errors)
            //{
            //    Log.WriteLine($"Error: {error}");
            //}

            RevitProcessServerSelect();
        }
        [Conditional("DEBUG")]
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
                    fileToTest = App.DefaultFilePath;
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

            //foreach (var test in RevitTestUtils.GetTestFullNames(fileToTest))
            //{
            //    Log.WriteLine(test);
            //}

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

                RevitTestUtils.CreateRevitServer(fileToTest, revitVersionNumber, null, false, true, false);

            } while (keyLoop.Key != ConsoleKey.Escape);

            Log.WriteLine("...");

            Thread.Sleep(1000);
        }
    }
}