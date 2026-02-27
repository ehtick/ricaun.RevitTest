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

        public ApplicationPluginsDisposable UpdateRevitAddinFile(string vendorId = null, string addInId = null)
        {
            if (!Initialized)
                throw new InvalidOperationException("ApplicationPluginsDisposable must be initialized before updating Revit VendorId and AddInId.");

            if (string.IsNullOrEmpty(applicationPluginsPath)) return this;
            if (string.IsNullOrWhiteSpace(vendorId) && string.IsNullOrWhiteSpace(addInId)) return this;

            vendorId = vendorId?.Trim();
            addInId = addInId?.Trim();

            if (!Guid.TryParse(addInId, out var _))
                addInId = null;

            var applicationPluginsFolder = RevitUtils.GetCurrentUserApplicationPluginsFolder();
            var applicationPluginsName = Path.GetFileNameWithoutExtension(applicationPluginsPath);
            var applicationPluginsBundleFolder = Path.Combine(applicationPluginsFolder, applicationPluginsName);

            if (!Directory.Exists(applicationPluginsBundleFolder))
                return this;

            var addins = Directory.GetFiles(applicationPluginsBundleFolder, "*.addin", SearchOption.AllDirectories);
            foreach (var addin in addins)
            {
                if (!string.IsNullOrWhiteSpace(vendorId))
                    XmlReplaceAllElementValue(addin, "VendorId", vendorId);
                if (!string.IsNullOrWhiteSpace(addInId))
                    XmlReplaceAllElementValue(addin, "AddInId", addInId);
            }
            return this;
        }

        /// <summary>
        /// Replaces the value of all occurrences of a specified XML element in a file with a new value.
        /// </summary>
        /// <remarks>
        /// <code>
        /// &lt;ElementName&gt;OldValue&lt;/ElementName&gt; will be replaced with &lt;ElementName&gt;NewValue&lt;/ElementName&gt;
        /// </code>
        /// </remarks>
        private void XmlReplaceAllElementValue(string filePath, string elementName, string value)
        {
            try
            {
                var addinXml = File.ReadAllText(filePath);
                elementName = elementName.Trim('<', '>');
                if (addinXml.Contains($"<{elementName}>"))
                {
                    var newAddinXml = System.Text.RegularExpressions.Regex.Replace(addinXml, $@"<{elementName}>.*?</{elementName}>", $"<{elementName}>{value}</{elementName}>");
                    File.WriteAllText(filePath, newAddinXml);
                }
            }
            catch { }
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
