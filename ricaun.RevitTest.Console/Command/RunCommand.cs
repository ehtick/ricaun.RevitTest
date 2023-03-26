using NUnit.Framework.Internal;
using ricaun.RevitTest.Console.Extensions;
using ricaun.RevitTest.Console.Utils;
using System;
using System.IO;
using System.Linq;

namespace ricaun.RevitTest.Console.Command
{
    public class RunCommand
    {
        private readonly Options options;

        public RunCommand(Options options)
        {
            this.options = options;
            ValidadeOptions();
            Initialize();
        }

        private void ValidadeOptions()
        {
            options.File = new FileInfo(options.File).FullName;
            if (!File.Exists(options.File))
                throw new FileNotFoundException();

            Log.Enabled = options.Log;
        }

        private void Initialize()
        {
            var assemblyName = typeof(RunCommand).Assembly.GetName();
            NUnit.TestEngine.Initialize(out string init);

            Log.WriteLine();
            Log.WriteLine($"{assemblyName.Name} {assemblyName.Version.ToString(3)} [{init}]");
            Log.WriteLine();
        }

        public void Run()
        {
            if (ReadTests())
                return;
        }

        private bool ReadTests()
        {
            WriteOutput();

            if (options.Read)
            {
                var tests = RevitTestUtils.GetTestFullNames(options.File);
                Log.WriteLine();
                foreach (var test in tests)
                {
                    Log.WriteLine(test);
                }
                Log.WriteLine();
                WriteOutput(tests.ToJson());
                return true;
            }

            Action<string> outputAction = (e) => { WriteOutput(e); };

            RevitTestUtils.CreateRevitServer(
                options.File,
                options.RevitVersion,
                outputAction,
                options.ForceToOpen,
                options.ForceToWait,
                options.ForceToClose,
                options.Test);

            return false;
        }

        private bool WriteOutput(string content = null)
        {
            if (string.IsNullOrEmpty(options.Output))
                return false;

            var isConsoleOutput = options.Output.Equals(nameof(System.Console), StringComparison.InvariantCultureIgnoreCase);
            if (isConsoleOutput)
            {
                if (string.IsNullOrEmpty(content)) return true;
                System.Console.WriteLine(content);
                return true;
            }

            options.Output = new FileInfo(options.Output).FullName;

            if (File.Exists(options.Output) && string.IsNullOrEmpty(content))
                File.Delete(options.Output);

            File.AppendAllText(options.Output, content);

            return true;
        }
    }
}