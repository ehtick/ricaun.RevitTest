using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Services
{
    /// <summary>
    /// ApplicationUtils
    /// </summary>
    internal static class ApplicationUtils
    {
        private const string ZIP_FILE_EXTENSION = ".zip";

        private static void ClearTemporaryDirectory(string folderDirectory)
        {
            try
            {
                foreach (var delete in Directory.GetDirectories(folderDirectory))
                {
                    try
                    {
                        Directory.Delete(delete);
                    }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// Create Temporary Directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string CreateTemporaryDirectory(string file = null)
        {
            string folderName = typeof(ApplicationUtils).Assembly.GetName().Name;
            if (string.IsNullOrEmpty(file)) file = folderName;
            string folderDirectory = Path.Combine(Path.GetTempPath(), folderName);
            ClearTemporaryDirectory(folderDirectory);

            string fileName = Path.GetFileNameWithoutExtension(file);
            string tempFolderName = $"{fileName}_{DateTime.Now.Ticks}";
            string tempDirectory = Path.Combine(folderDirectory, tempFolderName);
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
            return Download(temporaryDirectory, address, (ex) =>
            {
                AdapterLogger.Logger.DebugOnlyLocal($"Download Exception: {ex.Message}");
            });
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

            using (var client = new HttpClient())
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
                try
                {
                    if (File.Exists(address))
                    {
                        File.Copy(address, zipPath, true);
                        AdapterLogger.Logger.DebugOnlyLocal($"Download File Exists: {address}");
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", nameof(ApplicationUtils));
                        using (var s = await client.GetStreamAsync(address))
                        {
                            using (var fs = new FileStream(zipPath, FileMode.Create))
                            {
                                await s.CopyToAsync(fs);
                            }
                        }
                        AdapterLogger.Logger.DebugOnlyLocal($"Download File: {address}");
                    }
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
        /// Download and unzip Application Async
        /// </summary>
        /// <param name="applicationFolder">Folder of the Application</param>
        /// <param name="address"></param>
        /// <param name="downloadFileException"></param>
        /// <returns></returns>
        public static async Task<bool> DownloadAsyncWeb(string applicationFolder, string address, Action<Exception> downloadFileException = null)
        {
            var fileName = Path.GetFileName(address);
            var zipPath = Path.Combine(applicationFolder, fileName);
            var result = false;

#pragma warning disable SYSLIB0014 // Type or member is obsolete
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
#pragma warning restore SYSLIB0014 // Type or member is obsolete

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

                    AdapterLogger.Logger.DebugOnlyLocal($"ExtractZip: {fileFullName} |\t {completeFileName}");

                    if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
                        Directory.CreateDirectory(directory);

                    if (file.Name != "")
                        file.ExtractToFile(completeFileName, true);
                }
            }

        }
    }
}