using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ricaun.RevitTest.TestAdapter
{
    internal static class TestCaseUtils
    {
        public static bool IsSimilarTestName(TestCase testCase, string testName, bool onlyFullyQualifiedName = false)
        {
            SplitTestName(testName, out string fullyQualifiedName, out string displayName);

            if (onlyFullyQualifiedName)
                return testCase.FullyQualifiedName == fullyQualifiedName;

            return testCase.FullyQualifiedName == fullyQualifiedName && testCase.DisplayName == displayName;
        }

        public static string GetFullName(TestCase testCase)
        {
            return $"{testCase.FullyQualifiedName}.{testCase.DisplayName}";
        }

        public static TestCase Create(string source, string testName)
        {
            SplitTestName(testName, out string fullyQualifiedName, out string displayName);

            var testCase = new TestCase(fullyQualifiedName, TestAdapter.ExecutorUri, source)
            {
                DisplayName = displayName,
                Id = GetGuid(testName),
            };

            return testCase;
        }

        #region private

        private static void SplitTestName(string testName, out string fullyQualifiedName, out string displayName)
        {
            var indexOf = LastIndexOfDisplayName(testName);
            fullyQualifiedName = testName.Substring(0, indexOf);
            displayName = testName.Substring(indexOf + 1);
        }

        private static int LastIndexOfDisplayName(string testName)
        {
            var lastIndexOfDot = testName.LastIndexOf('.');
            var indexOf = testName.IndexOf('"');
            if (indexOf != -1)
            {
                lastIndexOfDot = testName.LastIndexOf('.', indexOf);
            }

            return lastIndexOfDot;
        }

        private static System.Guid GetGuid(string testName)
        {
            return new System.Guid(testName.GetHashCode(), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        #endregion
    }
}