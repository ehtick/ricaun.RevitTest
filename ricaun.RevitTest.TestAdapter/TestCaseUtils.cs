using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Linq;

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
                Id = GuidFromString(testName),
            };

            return testCase;
        }

        /// <summary>
        /// GuidFromString using EqtHash
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        /// <remarks>
        /// <code>
        /// Reference:
        /// https://github.com/microsoft/vstest/blob/main/src/Microsoft.TestPlatform.ObjectModel/TestCase.cs#L173
        /// https://github.com/microsoft/vstest/blob/main/src/Microsoft.TestPlatform.ObjectModel/Utilities/EqtHash.cs#L20
        /// </code>
        /// </remarks>
        private static System.Guid GuidFromString(string testName)
        {
            return Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities.EqtHash.GuidFromString(testName);
        }

        #region private

        private static void SplitTestName(string testName, out string fullyQualifiedName, out string displayName)
        {
            var splitDots = testName.Split('.');

            var index = LastIndexSplitOfDisplayName(splitDots);

            fullyQualifiedName = string.Join(".", splitDots.Take(index));
            displayName = string.Join(".", splitDots.Skip(index));

            AdapterLogger.Logger.DebugOnlyLocal($"SplitTestName: {testName} - {index} {fullyQualifiedName} {displayName}");
        }

        private static int LastIndexSplitOfDisplayName(string[] splitDots)
        {
            for (int i = 0; i < splitDots.Length; i++)
            {
                var name = splitDots[i];
                if (name.Contains('(') | name.Contains('"'))
                    return i;
            }
            return splitDots.Length - 1;
        }
        #endregion
    }
}