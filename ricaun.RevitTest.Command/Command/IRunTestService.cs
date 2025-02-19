using System;

namespace ricaun.RevitTest.Command
{
    public interface IRunTestService
    {
        public string[] GetTests(string filePath);
        public bool RunTests(
            string fileToTest,
            string revitVersion,
            Action<string> actionOutput = null,
            string forceLanguageToRevit = null,
            bool forceToOpenNewRevit = false,
            bool forceToCloseRevit = false,
            double timeoutMinutes = 0,
            params string[] testFilters);
    }
}