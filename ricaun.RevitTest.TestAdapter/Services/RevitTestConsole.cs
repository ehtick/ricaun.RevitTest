using ricaun.NUnit.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                var environmentVariable = Environment.GetEnvironmentVariable(applicationPath);
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    AdapterLogger.Logger.Info($"Application Environment: {Path.GetFileName(environmentVariable)}");
                    return environmentVariable;
                }
            }
            catch { }
            return applicationPath;
        }

        private string ValidadeApplication(string applicationPath, bool showApplicationName = false)
        {
            if (string.IsNullOrWhiteSpace(applicationPath))
                return null;

            if (showApplicationName)
                AdapterLogger.Logger.InfoAny($"Application: {Path.GetFileName(applicationPath)}");
            else
                AdapterLogger.Logger.Info($"Application: {Path.GetFileName(applicationPath)}");

            applicationPath = GetEnvironmentVariable(applicationPath);

            AdapterLogger.Logger.DebugOnlyLocal($"Application Environment: {applicationPath}");
            if (ApplicationUtils.Download(applicationPath, out string directory))
            {
                AdapterLogger.Logger.Info($"Application Download: {Path.GetFileName(applicationPath)}");

                var applicationNewPath = Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (File.Exists(applicationNewPath))
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(applicationNewPath);
                    AdapterLogger.Logger.DebugOnlyLocal($"Application FileVersionInfo: {fileVersionInfo}");
                    var productVersion = fileVersionInfo.GetSafeProductVersion();
                    AdapterLogger.Logger.Info($"Application Process: {Path.GetFileName(applicationNewPath)} {productVersion}");
                    return applicationNewPath;
                }
            }

            AdapterLogger.Logger.Info($"Application Download: Fail");
            return null;
        }

        public RevitTestConsole(string application = null, string sourceFile = null)
        {
            if (!string.IsNullOrEmpty(sourceFile))
            {
                var directory = Path.GetDirectoryName(sourceFile);
                if (Directory.Exists(directory))
                    Directory.SetCurrentDirectory(directory);
            }

            applicationPath = ValidadeApplication(application, true);
            if (applicationPath is null)
            {
                var name = ResourceConsoleUtils.Name;
                var directory = ApplicationUtils.CreateTemporaryDirectory(name);
                var file = Path.Combine(directory, name);
                applicationPath = ResourceConsoleUtils.CopyToFile(file);
                applicationPath = ValidadeApplication(applicationPath);
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
                AdapterLogger.Logger.Debug($"Application: [{File.Exists(applicationPath)}] {applicationPath}");
            }
            else
            {
                AdapterLogger.Logger.Info($"Application: {signedMessage}");
                AdapterLogger.Logger.Debug(message);
            }
            return isTrust;
        }

        public async Task RunExecuteTests(
            string file,
            Action<TestModel> testResultAction,
            int version = 0,
            string language = null,
            bool revitOpen = false,
            bool revitClose = false,
            double timeoutMinutes = 0,
            Action<string> consoleAction = null,
            Action<string> debugAction = null,
            Action<string> errorAction = null,
            params string[] filter)
        {
            await ricaun.RevitTest.Command.Process.RevitTestProcessStartUtils.RunExecuteTests(
                applicationPath, file, testResultAction,
                version, language, revitOpen, revitClose, timeoutMinutes,
                System.Diagnostics.Debugger.IsAttached,
                consoleAction, debugAction, errorAction, filter);
        }

        public async Task RunReadTests(
            string file,
            Action<string[]> testResultAction,
            Action<string> consoleAction = null,
            Action<string> debugAction = null,
            Action<string> errorAction = null)
        {
            await ricaun.RevitTest.Command.Process.RevitTestProcessStartUtils.RunReadTests(
                applicationPath, file, testResultAction,
                consoleAction, debugAction, errorAction);
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