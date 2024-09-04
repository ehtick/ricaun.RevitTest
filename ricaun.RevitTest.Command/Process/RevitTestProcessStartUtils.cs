using ricaun.NUnit.Models;
using System;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Command.Process
{
    public static class RevitTestProcessStartUtils
    {
        public static async Task RunTestReadWithLog(
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

        public static async Task RunExecuteTests(
            string applicationPath,
            string file,
            Action<TestModel> testResultAction,
            int version = 0,
            string language = null,
            bool revitOpen = false,
            bool revitClose = false,
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
                .SetLog()
                .SetTestFilter(filter)
                .SetDebugger(debugger)
                .RunExecuteTests(testResultAction, consoleAction, debugAction, errorAction);
        }
    }
}