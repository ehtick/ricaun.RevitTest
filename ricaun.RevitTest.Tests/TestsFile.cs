using NUnit.Framework;
using System;
using System.IO;

namespace ricaun.RevitTest.Tests
{
    public class TestsFile
    {
        const string FILE_NAME = "file.txt";

        [Test]
        public void Test1Write()
        {
            File.WriteAllText(FILE_NAME, DateTime.Now.ToString());
        }

        [Test]
        public void Test2Read()
        {
            Assert.IsTrue(File.Exists(FILE_NAME));
            var text = File.ReadAllText(FILE_NAME);
            Console.WriteLine(text);
        }

        [Test]
        public void Test3Delete()
        {
            if (File.Exists(FILE_NAME))
                File.Delete(FILE_NAME);
        }

    }

    public class TestsFolder
    {
        const string FILE_FOLDER = "folder";
        const string FILE_NAME = "file.txt";
        static string FILE = Path.Combine(FILE_FOLDER, FILE_NAME);

        [Test]
        public void Test3FolderDelete()
        {
            if (Directory.Exists(FILE_FOLDER))
                Directory.Delete(FILE_FOLDER, true);
        }

        [Test]
        public void Test2FolderFiles()
        {
            if (Directory.Exists(FILE_FOLDER))
            {
                var files = Directory.GetFiles(FILE_FOLDER);
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                }
            }
        }

        [Test]
        public void Test0FolderCreate()
        {
            Directory.CreateDirectory(FILE_FOLDER);
        }

        [Test]
        public void Test1FileWrite()
        {
            File.WriteAllText(FILE, DateTime.Now.ToString());
        }

        [Test]
        public void Test2FileRead()
        {
            Assert.IsTrue(File.Exists(FILE));
            var text = File.ReadAllText(FILE);
            Console.WriteLine(text);
        }

        [Test]
        public void Test3FileDelete()
        {
            File.Delete(FILE);
        }

    }
}