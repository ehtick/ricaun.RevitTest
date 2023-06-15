using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Services
{
    internal class ProcessStart
    {
        private string processPath;
        private Dictionary<string, object> argumentsPair = new Dictionary<string, object>();
        private string CreateArguments()
        {
            string ConvertKey(string key)
            {
                if (key.StartsWith("-"))
                    return key;
                return $"--{key}";
            }
            string ConvertValue(object value)
            {
                if (value is string valueString)
                {
                    valueString = valueString.Replace("\\", "\\\\");
                    value = valueString.Replace("\"", "\\\"");
                    return $"\"{value}\"";
                }
                else if (value is IEnumerable enumerable)
                {
                    var strValues = enumerable.Cast<object>()
                        .Select(v => ConvertValue(v))
                        .ToArray();
                    return string.Join(" ", strValues);
                }
                return $"{value}";
            }
            var arguments = "";
            foreach (var item in argumentsPair)
            {
                if (item.Value is null)
                {
                    arguments += $"{ConvertKey(item.Key)} ";
                    continue;
                }
                arguments += $"{ConvertKey(item.Key)} ";
                arguments += $"{ConvertValue(item.Value)} ";
            }
            AdapterLogger.Logger.Debug($"\tCreateArguments: {arguments}");
            return arguments;
        }

        public ProcessStart SetArgument(string name, object value = null)
        {
            argumentsPair[name] = value;
            return this;
        }

        public ProcessStart(string processPath)
        {
            this.processPath = processPath;
        }

        private ProcessStartInfo NewProcessStartInfo(string arguments)
        {
            if (string.IsNullOrEmpty(arguments)) arguments = "";
            arguments = arguments.Trim().Replace("\n", " & ");
            return new ProcessStartInfo
            {
                Verb = "runas",
                FileName = processPath,
                WorkingDirectory = System.IO.Path.GetDirectoryName(processPath),
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                //StandardOutputEncoding = System.Text.Encoding.UTF8,
                //StandardErrorEncoding = System.Text.Encoding.UTF8,
            };
        }


        public async Task<string> Run()
        {
            var arguments = CreateArguments();
            Debug.WriteLine($"Run: {arguments}");
            return await Run(arguments);
        }

        public async Task Run(Action<string> consoleAction, Action<string> errorAction = null)
        {
            var arguments = CreateArguments();
            Debug.WriteLine($"Run: {arguments}");
            await Run(arguments, consoleAction, errorAction);
        }

        private async Task<string> Run(string arguments)
        {
            if (string.IsNullOrEmpty(processPath)) return string.Empty;
            var psi = NewProcessStartInfo(arguments);
            var process = Process.Start(psi);
            var output = await process.StandardOutput.ReadToEndAsync();
            //var error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit(int.MaxValue);
            return output;
        }

        private async Task Run(string arguments,
            Action<string> consoleAction = null,
            Action<string> errorAction = null)
        {
            if (string.IsNullOrEmpty(processPath)) return;
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