using ricaun.NUnit.Models;
using System;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Command.Process
{
    public static class RevitTestProcessStartUtils
    {
        /// <summary>
        /// Execute read Tests with output console and log.
        /// </summary>
        /// <param name="applicationPath"></param>
        /// <param name="file"></param>
        /// <param name="testResultAction"></param>
        /// <param name="consoleAction"></param>
        /// <param name="debugAction"></param>
        /// <param name="errorAction"></param>
        /// <returns></returns>
        public static async Task RunReadTests(
            string applicationPath,
            string file,
            Action<string[]> testResultAction,
            Action<string> consoleAction = null,
            Action<string> debugAction = null,
            Action<string> errorAction = null)
        {
            await new RevitTestProcessStart(applicationPath)
                .SetFile(file)
                .SetOutputConsole()
                .SetRead()
                .SetLog()
                .RunReadTests(testResultAction, consoleAction, debugAction, errorAction);
        }

        /// <summary>
        /// Execute Tests with output console and log.
        /// </summary>
        /// <param name="applicationPath"></param>
        /// <param name="file"></param>
        /// <param name="testResultAction"></param>
        /// <param name="version"></param>
        /// <param name="language"></param>
        /// <param name="revitOpen"></param>
        /// <param name="revitClose"></param>
        /// <param name="debugger"></param>
        /// <param name="consoleAction"></param>
        /// <param name="debugAction"></param>
        /// <param name="errorAction"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static async Task RunExecuteTests(
            string applicationPath,
            string file,
            Action<TestModel> testResultAction,
            string version = null,
            string language = null,
            bool revitOpen = false,
            bool revitClose = false,
            double timeoutMinutes = 0,
            bool debugger = false,
            Action<string> consoleAction = null,
            Action<string> debugAction = null,
            Action<string> errorAction = null,
            params string[] filter)
        {
            await new RevitTestProcessStart(applicationPath)
                .SetFile(file)
                .SetRevitVersion(version)
                .SetRevitLanguage(language)
                .SetOutputConsole()
                .SetOpen(revitOpen)
                .SetClose(revitClose)
                .SetTimeout(timeoutMinutes)
                .SetLog()
                .SetTestFilter(filter)
                .SetDebugger(debugger)
                .RunExecuteTests(testResultAction, consoleAction, debugAction, errorAction);
        }
    }
}