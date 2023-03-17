using System;
using System.IO;

namespace ricaun.RevitTest.Console.Utils
{
    public class FileWatcher : IDisposable
    {
        private string filePath;
        private FileSystemWatcher watcher;
        private Action<string, bool> fileUpdateExists;

        public FileWatcher()
        {

        }

        public FileWatcher Initialize(string filePath, Action<string, bool> fileUpdateExists)
        {
            Dispose();

            this.filePath = filePath;
            this.fileUpdateExists = fileUpdateExists;

            // Create a new FileSystemWatcher and set its properties
            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(filePath);

            // Watch for changes in LastWrite time and file creation events
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

            // Only monitor text files
            watcher.Filter = Path.GetFileName(filePath);

            // Attach event handlers to the events
            watcher.Created += OnFileChanged;
            watcher.Changed += OnFileChanged;
            watcher.Deleted += OnFileChanged;

            // Start monitoring the directory
            watcher.EnableRaisingEvents = true;

            TriggerFile();

            return this;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            TriggerFile();
        }

        private void TriggerFile()
        {
            this.fileUpdateExists?.Invoke(filePath, File.Exists(filePath));
        }

        public void Dispose()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
            }
        }
    }
}
