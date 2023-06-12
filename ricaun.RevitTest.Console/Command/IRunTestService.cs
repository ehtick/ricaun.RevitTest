using System;

namespace ricaun.RevitTest.Console.Command
{
    public interface IRunTestService
    {
        public string[] GetTests(string filePath);
        public bool RunTests(
            string fileToTest,
            int revitVersionNumber,
            Action<string> actionOutput = null,
            bool forceToOpenNewRevit = false,
            bool forceToWaitRevit = false,
            bool forceToCloseRevit = false,
            params string[] testFilters);
    }
}