using ricaun.RevitTest.TestAdapter.Extensions;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Services
{
    public class RevitTestConsole : IDisposable
    {
        private readonly string path;

        /// <summary>
        /// Create Temporary Directory
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string CreateTemporaryDirectory(string file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string folderName = Assembly.GetExecutingAssembly().GetName().Name;
            string tempFolderName = fileName;
            string tempDirectory = Path.Combine(Path.GetTempPath(), folderName, tempFolderName);
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public RevitTestConsole()
        {
            //var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var directory = CreateTemporaryDirectory(Properties.Resources.ricaun_RevitTest_Console_Name);
            var file = Path.Combine(directory, Properties.Resources.ricaun_RevitTest_Console_Name);
            path = Properties.Resources.ricaun_RevitTest_Console.CopyToFile(file);
        }

        public async Task<string> Run(params string[] arguments)
        {
            return await new ProcessStart(path).Run(string.Join(" ", arguments));
        }

        public async Task<string> RunTest(string file, int version = 0, params string[] filter)
        {
            var read = await Run($"-f \"{file}\" -v {version} -o console -t \"{string.Join(",", filter)}\"");
            return read;
        }

        public async Task RunTestAction(
            string file,
            int version = 0,
            Action<string> consoleAction = null,
            params string[] filter)
        {
            var arguments = $"-f \"{file}\" -v {version} -o console";

            if (filter.Length > 0)
                arguments += $" -t \"{string.Join(",", filter)}\"";

            await new ProcessStart(path).Run(arguments, consoleAction);
        }

        public async Task RunTestAction(
            string file,
            int version = 0,
            bool revitOpen = false,
            bool revitClose = false,
            Action<string> consoleAction = null,
            params string[] filter)
        {
            var arguments = $"-f \"{file}\" -v {version} -o console";

            if (revitOpen)
                arguments += $" --open";

            if (revitClose)
                arguments += $" --close";

            if (filter.Length > 0)
                arguments += $" -t \"{string.Join(",", filter)}\"";

            await new ProcessStart(path).Run(arguments, consoleAction);
        }

        public async Task<string[]> RunTestRead(string file)
        {
            var read = await Run($"-f \"{file}\" -r -o console");
            var testNames = read.Deserialize<string[]>();
            return testNames;
        }

        public void Dispose()
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}