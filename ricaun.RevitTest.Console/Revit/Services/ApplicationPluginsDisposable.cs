using ricaun.Revit.Installation;
using ricaun.RevitTest.Command.Extensions;
using System;
using System.IO;

namespace ricaun.RevitTest.Console.Revit.Services
{
    public class ApplicationPluginsDisposable : IDisposable
    {
        private readonly string applicationPluginsPath;

        public ApplicationPluginsDisposable(string applicationPluginsPath)
        {
            this.applicationPluginsPath = applicationPluginsPath;
            Initialize();
        }
        public ApplicationPluginsDisposable(byte[] data, string fileName)
        {
            this.applicationPluginsPath = data.CopyToFile(fileName);
            Initialize();
            File.Delete(this.applicationPluginsPath);
        }
        private void Initialize()
        {
            if (string.IsNullOrEmpty(applicationPluginsPath)) return;
            var applicationPluginsFolder = RevitUtils.GetCurrentUserApplicationPluginsFolder();
            ApplicationPluginsUtils.DownloadBundle(applicationPluginsFolder, applicationPluginsPath);
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
