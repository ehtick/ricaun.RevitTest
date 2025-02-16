using ricaun.RevitTest.Command;
using ricaun.RevitTest.Console.Revit.Utils;
using System;

namespace ricaun.RevitTest.Console.Revit
{
    public class RevitRunTestService : IRunTestService
    {
        public string[] GetTests(string filePath)
        {
            return RevitTestUtils.GetTestFullNames(filePath);
        }

        public bool RunTests(string fileToTest,
            string revitVersion,
            Action<string> actionOutput = null,
            string forceLanguageToRevit = null,
            bool forceToOpenNewRevit = false,
            bool forceToCloseRevit = false,
            double timeoutMinutes = 0,
            params string[] testFilters)
        {
            RevitTestUtils.CreateRevitServer(
                fileToTest, revitVersion, actionOutput, forceLanguageToRevit,
                forceToOpenNewRevit, forceToCloseRevit, timeoutMinutes, testFilters);
            return true;
        }
    }
}
