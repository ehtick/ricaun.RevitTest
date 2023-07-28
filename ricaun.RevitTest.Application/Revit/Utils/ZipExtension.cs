using System;
using System.IO;
using System.IO.Compression;

namespace ricaun.RevitTest.Application.Revit
{
    public static class ZipExtension
    {
        /// <summary>
        /// Extract <paramref name="zipFile"/> to Folder add temp folder if exist
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="zipDestination"></param>
        /// <returns></returns>
        public static bool ExtractToTempFolder(string zipFile, out string zipDestination)
        {
            var zipName = Path.GetFileNameWithoutExtension(zipFile);
            var zipDirectory = Path.GetDirectoryName(zipFile);
            zipDestination = Path.Combine(zipDirectory, zipName);

            if (Directory.Exists(zipDestination))
            {
                try
                {
                    Directory.Delete(zipDestination, true);
                }
                catch { }
            }

            if (Directory.Exists(zipDestination))
            {
                zipDestination += $"_{DateTime.Now.Ticks}";
            }

            Directory.CreateDirectory(zipDestination);

            try
            {
                ZipFile.ExtractToDirectory(zipFile, zipDestination);
                File.Delete(zipFile);
                return true;
            }
            catch { }

            return false;
        }

        public static void ExtractToDirectoryIfNewer(string zipFile, string zipDestination)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string filePath = Path.Combine(zipDestination, entry.FullName);

                    if (File.Exists(filePath) == false | entry.LastWriteTime > File.GetLastWriteTime(filePath))
                    {
                        var directory = Path.GetDirectoryName(filePath);

                        try
                        {
                            if (string.IsNullOrEmpty(entry.Name)) continue;

                            if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
                                Directory.CreateDirectory(directory);

                            // Log.WriteLine($"ExtractToFile: {entry.Name}");

                            entry.ExtractToFile(filePath, true);
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory.
        /// </summary>
        /// <param name="sourceDirectoryName"></param>
        /// <param name="destinationArchiveFileName"></param>
        /// <param name="includeBaseDirectory"></param>
        public static string CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, bool includeBaseDirectory = false)
        {
            destinationArchiveFileName = Path.ChangeExtension(destinationArchiveFileName, "zip");
            var folder = Path.GetDirectoryName(destinationArchiveFileName);
            if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
            if (File.Exists(destinationArchiveFileName)) return destinationArchiveFileName;
            ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, CompressionLevel.Optimal, includeBaseDirectory);
            return destinationArchiveFileName;
        }
    }
}