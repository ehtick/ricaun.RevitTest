using System.Diagnostics;

namespace ricaun.RevitTest.TestAdapter.Services
{
    internal static class FileVersionInfoUtils
    {
        /// <summary>
        /// Get ProductVersion without +build only the version-prerelease
        /// </summary>
        /// <param name="fileVersionInfo"></param>
        /// <returns></returns>
        public static string GetSafeProductVersion(this FileVersionInfo fileVersionInfo)
        {
            if (fileVersionInfo is null)
                return null;
            return fileVersionInfo.ProductVersion.Split('+')[0];
        }
    }
}