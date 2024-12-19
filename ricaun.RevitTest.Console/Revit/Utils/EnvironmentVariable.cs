using System;

namespace ricaun.RevitTest.Console.Revit.Utils
{
    /// <summary>
    /// EnvironmentVariable is used for testing purpose. 
    /// </summary>
    internal static class EnvironmentVariable
    {
        /// <summary>
        /// Extra arguments to be passed to the Revit process start.
        /// </summary>
        public static string ProcessArguments => Environment.GetEnvironmentVariable("RICAUN_REVITTEST_CONSOLE_PROCESS_ARGUMENTS");

        /// <summary>
        /// Timeout in seconds if application-console connection is not busy.
        /// </summary>
        public static int TimeoutNotBusyMaxSeconds => GetEnvironmentVariableInteger("RICAUN_REVITTEST_CONSOLE_TIMEOUT_NOT_BUSY_MAX_SECONDS", 10);

        private static int GetEnvironmentVariableInteger(string variable, int value = default)
        {
            try
            {
                var environmentVariable = Environment.GetEnvironmentVariable(variable);
                if (int.TryParse(environmentVariable, out int result))
                    return result;
            }
            catch { }
            return value;
        }
    }
}