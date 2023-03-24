using ricaun.NUnit;
using ricaun.Revit.Installation;
using System;
using System.IO;

namespace ricaun.RevitTest.Console
{
    public static class RevitTestUtils
    {
        /// <summary>
        /// Get Test Full Names using RevitInstallation if needed (Revit +2021)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[] GetTestFullNames(string filePath)
        {
            var tests = TestEngine.GetTestFullNames(filePath);
            if (RevitUtils.TryGetRevitVersion(filePath, out var revitVersion))
            {
                Log.WriteLine($"RevitTestUtils: {revitVersion}");
                if (tests.Length == 0)
                {
                    // Problem with AnavRes.dll / adui22res.dll (version -2020)
                    revitVersion = Math.Max(revitVersion, 2021);
                    if (RevitInstallationUtils.InstalledRevit.TryGetRevitInstallationGreater(revitVersion, out RevitInstallation revitInstallation))
                    {
                        Log.WriteLine($"RevitTestUtils: {revitInstallation.InstallLocation}");
                        tests = TestEngine.GetTestFullNames(filePath, revitInstallation.InstallLocation);
                    }
                }
            }
            return tests;
        }
    }
}