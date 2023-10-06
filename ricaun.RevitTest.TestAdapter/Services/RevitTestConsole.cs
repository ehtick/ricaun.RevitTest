using ricaun.RevitTest.TestAdapter.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Services
{
    internal class RevitTestConsole : IDisposable
    {
        private readonly string applicationPath;

        private string GetEnvironmentVariable(string applicationPath)
        {
            try
            {
                var enviromentVariable = Environment.GetEnvironmentVariable(applicationPath);
                if (!string.IsNullOrEmpty(enviromentVariable))
                {
                    AdapterLogger.Logger.Info($"Application Environment: {Path.GetFileName(enviromentVariable)}");
                    return enviromentVariable;
                }
            }
            catch { }
            return applicationPath;
        }

        private string ValidadeApplication(string applicationPath)
        {
            if (string.IsNullOrWhiteSpace(applicationPath))
                return null;

            AdapterLogger.Logger.Info($"Application: {Path.GetFileName(applicationPath)}", 0);

            applicationPath = GetEnvironmentVariable(applicationPath);

            if (ApplicationUtils.Download(applicationPath, out string directory))
            {
                AdapterLogger.Logger.Info($"Application Download: {Path.GetFileName(applicationPath)}");

                var applicationNewPath = Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (File.Exists(applicationNewPath))
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(applicationNewPath);
                    var productVersion = $"{fileVersionInfo.ProductVersion}";
                    AdapterLogger.Logger.Warning($"Application Process: {Path.GetFileName(applicationNewPath)} {productVersion}");
                    return applicationNewPath;
                }
            }

            return null;
        }

        public RevitTestConsole(string application = null)
        {
            applicationPath = ValidadeApplication(application);
            if (applicationPath is null)
            {
                var directory = ApplicationUtils.CreateTemporaryDirectory(Properties.Resources.ricaun_RevitTest_Console_Name);
                var file = Path.Combine(directory, Properties.Resources.ricaun_RevitTest_Console_Name);
                applicationPath = Properties.Resources.ricaun_RevitTest_Console.CopyToFile(file);
            }
            AdapterLogger.Logger.Info($"Application: {Path.GetFileName(applicationPath)}");
        }

        public bool IsTrusted(out string message)
        {
            var isTrust = TrustApplicationUtils.IsTrust(applicationPath, out string signedMessage);

            message = $"Application: {Path.GetFileName(applicationPath)} is {(isTrust ? "" : "not ")}trusted.";

            AdapterLogger.Logger.DebugOnlyLocal($"Application {signedMessage}");

            if (isTrust == false)
            {
                AdapterLogger.Logger.Error($"Application: {signedMessage}");
            }
            else
            {
                AdapterLogger.Logger.Info($"Application: {signedMessage}");
                AdapterLogger.Logger.Debug(message);
            }
            return isTrust;
        }

        public async Task RunTestAction(
            string file,
            int version = 0,
            string language = null,
            bool revitOpen = false,
            bool revitClose = false,
            Action<string> consoleAction = null,
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
                .SetDebugger(System.Diagnostics.Debugger.IsAttached)
                .Run(consoleAction);
        }

        public async Task<string[]> RunTestRead(string file)
        {
            var read = await new RevitTestProcessStart(applicationPath)
                .SetFile(file)
                .SetOutputConsole()
                .SetRead()
                .Run();

            var testNames = read.Deserialize<string[]>();
            return testNames;
        }

        public async Task RunTestReadWithLog(
            string file,
            Action<string> consoleAction)
        {
            await new RevitTestProcessStart(applicationPath)
                .SetFile(file)
                .SetOutputConsole()
                .SetRead()
                .SetLog()
                .Run(consoleAction);
        }

        public void Dispose()
        {
            try
            {
                AdapterLogger.Logger.Debug($"Dispose: {Path.GetFileName(applicationPath)}");
                if (File.Exists(applicationPath))
                {
                    File.Delete(applicationPath);
                    Directory.Delete(Path.GetDirectoryName(applicationPath), true);
                }
            }
            catch { }
        }
    }
}