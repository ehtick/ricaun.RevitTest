using System;
using System.IO;
using System.Linq;

namespace ricaun.RevitTest.Console.Extensions
{
    public class RevitAddinDisable : IDisposable
    {
        public static string FormatAddin = ".addin";
        public static string FormatDisable = ".disable";
        public static string AddinPath = "Addins";
        private readonly string revitFolder;

        private string[] DisableAddins() => GetAddins(FormatAddin).Select(e => RenameExtension(e, FormatDisable)).ToArray();
        private string[] EnableAddins() => GetAddins(FormatDisable).Select(e => RenameExtension(e, FormatAddin)).ToArray();

        public RevitAddinDisable(string revitFolder)
        {
            this.revitFolder = revitFolder;
            DisableAddins();
        }

        private string RenameExtension(string filePath, string changeExtension)
        {
            var newFilePath = Path.ChangeExtension(filePath, changeExtension);
            if (File.Exists(filePath))
            {
                try
                {
                    System.Console.WriteLine(Path.GetFileName(newFilePath));
                    File.Move(filePath, newFilePath);
                }
                catch (Exception ex) { System.Console.WriteLine(ex); }
            }
            return newFilePath;
        }

        private string[] GetAddins(string extension)
        {
            var folder = Path.Combine(revitFolder, AddinPath);
            var addinFilePaths = Directory.GetFiles(folder, $"*{extension}", SearchOption.AllDirectories);
            return addinFilePaths;
        }

        public void Dispose()
        {
            EnableAddins();
        }
    }

}
