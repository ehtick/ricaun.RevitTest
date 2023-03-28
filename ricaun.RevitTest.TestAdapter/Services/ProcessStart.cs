using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Services
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
                FileName = fileName,
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
            process.WaitForExit(int.MaxValue);
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
            process.WaitForExit(int.MaxValue);
            await Task.Delay(0);
        }
    }
}