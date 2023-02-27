using System.IO;

namespace ricaun.RevitTest.Console
{
    public static class ResourcesExtension
    {
        public static void CopyToFile(this Stream stream, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                stream.CopyTo(fs);
            }
        }

        public static string CopyToFile(this byte[] data, string path)
        {
            FileInfo fi = new FileInfo(path);
            File.WriteAllBytes(path, data);
            return fi.FullName;
        }
    }

}
