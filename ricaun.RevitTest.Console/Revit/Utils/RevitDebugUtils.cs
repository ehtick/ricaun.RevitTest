using ricaun.Revit.Installation;
using ricaun.RevitTest.Command;
using ricaun.RevitTest.Command.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Console.Revit.Utils
{
    public static class RevitDebugUtils
    {
        public static class App
        {
            public static string FilePath = "";
            public static string DefaultFilePath = @"D:\Users\ricau\source\repos\RevitTest0\RevitTest0\bin\Debug\RevitTest0.dll";
        }

        internal static void ProcessServerSelect()
        {
            Task.Run(RevitProcessServerSelectAsync).GetAwaiter().GetResult();
        }

        static async Task RevitProcessServerSelectAsync()
        {
            await Task.Delay(0);

            Log.WriteLine();
            Log.WriteLine($"{AppUtils.GetInfo()}");

            Log.WriteLine();
            Log.WriteLine($"DebuggerUtils: {DebuggerUtils.IsDebuggerAttached}");

            if (DebuggerUtils.IsDebuggerAttached)
            {
                Log.WriteLine($"DebuggerUtils: {VisualStudioDebugUtils.GetName()}");
            }

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

                RevitTestUtils.CreateRevitServer(fileToTest, revitVersionNumber, null, null, false, true, false);

            } while (keyLoop.Key != ConsoleKey.Escape);

            Log.WriteLine("...");

            Thread.Sleep(1000);
        }
    }
}
