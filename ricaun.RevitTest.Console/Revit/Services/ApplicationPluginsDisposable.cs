using ricaun.Revit.Installation;
using ricaun.RevitTest.Command.Extensions;
using System;
using System.IO;

namespace ricaun.RevitTest.Console.Revit.Services
{
    public class ApplicationPluginsDisposable : IDisposable
    {
        private readonly string applicationPluginsPath;
        private readonly bool applicationPluginsPathDelete;
        private Action<Exception> downloadException;
        private Action<string> logConsole;

        public bool Initialized { get; private set; }
        public ApplicationPluginsDisposable(string applicationPluginsPath)
        {
            this.applicationPluginsPath = applicationPluginsPath;
        }
        public ApplicationPluginsDisposable(byte[] data, string fileName)
        {
            this.applicationPluginsPath = data.CopyToFile(fileName);
            this.applicationPluginsPathDelete = true;
        }

        public ApplicationPluginsDisposable SetException(Action<Exception> downloadException)
        {
            this.downloadException = downloadException;
            return this;
        }

        public ApplicationPluginsDisposable SetLog(Action<string> logConsole)
        {
            this.logConsole = logConsole;
            return this;
        }

        public ApplicationPluginsDisposable Initialize()
        {
            if (string.IsNullOrEmpty(applicationPluginsPath)) return this;
            var applicationPluginsFolder = RevitUtils.GetCurrentUserApplicationPluginsFolder();
            Initialized = ApplicationPluginsUtils.DownloadBundle(applicationPluginsFolder, applicationPluginsPath, downloadException, logConsole);

            if (applicationPluginsPathDelete)
            {
                File.Delete(this.applicationPluginsPath);
            }

            return this;
        }

        public void Dispose()
        {
            if (string.IsNullOrEmpty(applicationPluginsPath)) return;
            var applicationPluginsFolder = RevitUtils.GetCurrentUserApplicationPluginsFolder();
            var applicationPluginsName = Path.GetFileNameWithoutExtension(applicationPluginsPath);
            ApplicationPluginsUtils.DeleteBundle(applicationPluginsFolder, applicationPluginsName);
        }
    }

}
