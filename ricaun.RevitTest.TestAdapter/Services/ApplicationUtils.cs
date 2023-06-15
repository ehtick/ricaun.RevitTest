using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Services
{
    /// <summary>
    /// ApplicationUtils
    /// </summary>
    internal static class ApplicationUtils
    {
        private const string ZIP_FILE_EXTENSION = ".zip";

        /// <summary>
        /// Create Temporary Directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string CreateTemporaryDirectory(string file = null)
        {
            string folderName = typeof(ApplicationUtils).Assembly.GetName().Name;
            if (string.IsNullOrEmpty(file)) file = folderName;
            string fileName = Path.GetFileNameWithoutExtension(file);
            string tempFolderName = fileName;
            string tempDirectory = Path.Combine(Path.GetTempPath(), folderName, tempFolderName);
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        /// <summary>
        /// Download and unzip in <paramref name="temporaryDirectory"/>
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool Download(string address, out string temporaryDirectory)
        {
            temporaryDirectory = CreateTemporaryDirectory();
            return Download(temporaryDirectory, address);
        }

        /// <summary>
        /// Download and unzip
        /// </summary>
        /// <param name="applicationFolder">Folder of the Application</param>
        /// <param name="address"></param>
        /// <param name="downloadFileException"></param>
        /// <returns></returns>
        public static bool Download(string applicationFolder, string address, Action<Exception> downloadFileException = null)
        {
            var task = Task.Run(async () =>
            {
                return await DownloadAsync(applicationFolder, address, downloadFileException);
            });
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Download and unzip Application Async
        /// </summary>
        /// <param name="applicationFolder">Folder of the Application</param>
        /// <param name="address"></param>
        /// <param name="downloadFileException"></param>
        /// <returns></returns>
        public static async Task<bool> DownloadAsync(string applicationFolder, string address, Action<Exception> downloadFileException = null)
        {
            var fileName = Path.GetFileName(address);
            var zipPath = Path.Combine(applicationFolder, fileName);
            var result = false;

            using (var client = new WebClient())
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
                client.Headers[HttpRequestHeader.UserAgent] = nameof(ApplicationUtils);
                try
                {
                    await client.DownloadFileTaskAsync(new Uri(address), zipPath);
                    ExtractZipToDirectory(zipPath, applicationFolder);
                    result = true;
                }
                catch (Exception ex)
                {
                    downloadFileException?.Invoke(ex);
                }
                if (Path.GetExtension(zipPath) == ZIP_FILE_EXTENSION)
                    if (File.Exists(zipPath)) File.Delete(zipPath);
            }

            return result;
        }

        /// <summary>
        /// ExtractToDirectory with overwrite enable
        /// </summary>
        /// <param name="archiveFileName"></param>
        /// <param name="destinationDirectoryName"></param>
        private static void ExtractZipToDirectory(string archiveFileName, string destinationDirectoryName)
        {
            if (Path.GetExtension(archiveFileName) != ZIP_FILE_EXTENSION) return;

            using (var archive = ZipFile.OpenRead(archiveFileName))
            {
                foreach (var file in archive.Entries)
                {
                    var fileFullName = file.FullName;

                    var completeFileName = Path.Combine(destinationDirectoryName, fileFullName);
                    var directory = Path.GetDirectoryName(completeFileName);

                    Debug.WriteLine($"{fileFullName} |\t {completeFileName}");

                    AdapterLogger.Logger.Debug($"ExtractZip: {fileFullName} |\t {completeFileName}");

                    if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
                        Directory.CreateDirectory(directory);

                    if (file.Name != "")
                        file.ExtractToFile(completeFileName, true);
                }
            }

        }
    }
}