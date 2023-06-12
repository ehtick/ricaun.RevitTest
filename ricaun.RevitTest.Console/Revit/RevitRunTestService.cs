using ricaun.RevitTest.Console.Command;
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
            int revitVersionNumber,
            Action<string> actionOutput = null,
            bool forceToOpenNewRevit = false,
            bool forceToWaitRevit = false,
            bool forceToCloseRevit = false,
            params string[] testFilters)
        {
            RevitTestUtils.CreateRevitServer(fileToTest, revitVersionNumber, actionOutput, forceToOpenNewRevit, forceToWaitRevit, forceToCloseRevit, testFilters);
            return true;
        }
    }
}
