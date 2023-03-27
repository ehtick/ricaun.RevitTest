using ricaun.RevitTest.TestAdapter.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter
{
    public class ProcessStart
    {
        private string fileName;

        public ProcessStart(string fileName)
        {
            this.fileName = fileName;
        }

        private ProcessStartInfo NewProcessStartInfo(string arguments)
        {
            if (string.IsNullOrEmpty(arguments)) arguments = "";
            arguments = arguments.Trim().Replace("\n", " & ");
            return new ProcessStartInfo
            {
                Verb = "runas",
                FileName = this.fileName,
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
            };
        }

        public async Task<string> Run(string arguments = null)
        {
            var psi = NewProcessStartInfo(arguments);
            var process = Process.Start(psi);
            var output = await process.StandardOutput.ReadToEndAsync();
            //var error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            return output;
        }

        public async Task Run(string arguments = null,
            Action<string> consoleAction = null,
            Action<string> errorAction = null)
        {
            var psi = NewProcessStartInfo(arguments);
            var process = Process.Start(psi);
            process.OutputDataReceived += (sender, e) =>
            {
                consoleAction?.Invoke(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                errorAction?.Invoke(e.Data);
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            await Task.Delay(0);
        }
    }

    public class RevitTestConsole : IDisposable
    {
        private readonly string path;

        public RevitTestConsole()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Combine(directory, Properties.Resources.ricaun_RevitTest_Console_Name);
            this.path = Properties.Resources.ricaun_RevitTest_Console.CopyToFile(file);
        }

        public async Task<string> Run(params string[] arguments)
        {
            return await new ProcessStart(this.path).Run(string.Join(" ", arguments));
        }

        public async Task<string> RunTest(string file, int version = 0, params string[] filter)
        {
            var read = await Run($"-f \"{file}\" -v {version} -o console -t \"{string.Join(",", filter)}\"");
            return read;
        }

        public async Task RunTestAction(
            string file,
            int version = 0,
            Action<string> consoleAction = null,
            params string[] filter)
        {
            var arguments = $"-f \"{file}\" -v {version} -o console -t \"{string.Join(",", filter)}\"";
            await new ProcessStart(this.path).Run(arguments, consoleAction);
        }

        public async Task<string[]> RunTestRead(string file)
        {
            var read = await Run($"-f \"{file}\" -r -o console");
            var testNames = read.Deserialize<string[]>();
            return testNames;
        }

        public void Dispose()
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}