using System;
using System.IO;
using System.Reflection;

namespace ricaun.RevitTest.Console.Extensions
{
    public static class ResourcesExtension
    {
        /// <summary>
        /// Copy <paramref name="stream"/> to <paramref name="path"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        public static void CopyToFile(this Stream stream, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                stream.CopyTo(fs);
            }
        }

        /// <summary>
        /// Copy <paramref name="data"/> to <paramref name="path"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CopyToFile(this byte[] data, string path)
        {
            FileInfo fi = new FileInfo(path);
            File.WriteAllBytes(path, data);
            return fi.FullName;
        }
    }

}
