using System.Diagnostics;

namespace ricaun.RevitTest.Shared
{
    public static class ProcessPipeNameUtils
    {
        public static string GetPipeName()
        {
            var process = Process.GetCurrentProcess();
            return process.GetPipeName();
        }
        public static string GetPipeName(this Process process)
        {
            var name = $"ricaun.{process.ProcessName}.{process.Id}";
            return name;
        }
    }

}