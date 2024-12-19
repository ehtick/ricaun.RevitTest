using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ricaun.RevitTest.Command.Extensions;

namespace ricaun.RevitTest.Command.Process
{
    public class ProcessStart
    {
        private static List<System.Diagnostics.Process> Processes = new List<System.Diagnostics.Process>();
        public static System.Diagnostics.Process[] GetProcesses()
        {
            return Processes.OfType<System.Diagnostics.Process>().Where(e => !e.HasExited).ToArray();
        }

        protected virtual void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }

        private const int DelayAfterExit = 100;
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
                    return valueString.EncodeParameterArgument();
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
            WriteLine($"\tCreateArguments: {arguments}");
            return arguments;
        }

        protected ProcessStart SetArgument(string name, object value = null)
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
            };
        }

        public async Task Run(Action<string> consoleAction, Action<string> errorAction = null)
        {
            var arguments = CreateArguments();
            WriteLine($"ProcessStart[{processPath.Length}]: {processPath}");
            WriteLine($"ProcessStart.Run[{arguments.Length}]: {arguments}");
            await Run(arguments, consoleAction, errorAction);
        }
        private async Task Run(string arguments,
            Action<string> consoleAction = null,
            Action<string> errorAction = null)
        {
            if (string.IsNullOrEmpty(processPath)) return;
            var psi = NewProcessStartInfo(arguments);
            var process = System.Diagnostics.Process.Start(psi);
            Processes.Add(process);
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
            await Task.Delay(DelayAfterExit);
        }
    }
}