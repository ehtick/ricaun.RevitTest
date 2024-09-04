using ricaun.NUnit;
using ricaun.RevitTest.Command.Extensions;
using ricaun.RevitTest.Command.Utils;
using System;
using System.IO;
using System.Linq;

namespace ricaun.RevitTest.Command
{
    public class RunCommand<T> where T : IRunTestService, new()
    {
        private readonly Options options;
        private readonly IRunTestService testService;

        public RunCommand(Options options)
        {
            this.testService = new T();
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
            TestEngine.Initialize(out string init);

            Log.WriteLine();
            Log.WriteLine($"{AppUtils.GetInfo()} [{init}]");
            Log.WriteLine();
        }

        public void Run()
        {
            try
            {
                RunOrReadTests();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
                throw;
            }
        }

        private bool RunOrReadTests()
        {
            WriteOutput();

            if (options.Read)
            {
                var tests = testService.GetTests(options.File);
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

            DebuggerUtils.AttachedDebugger(options.DebuggerAttach);

            testService.RunTests(
                options.File,
                options.RevitVersion,
                outputAction,
                options.RevitLanguage,
                options.ForceToOpen,
                options.ForceToWait,
                options.ForceToClose,
                options.Test.ToArray());

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