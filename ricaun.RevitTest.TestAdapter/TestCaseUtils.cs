using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ricaun.RevitTest.TestAdapter
{
    public static class TestCaseUtils
    {
        public static string GetFullName(TestCase testCase)
        {
            return $"{testCase.FullyQualifiedName}.{testCase.DisplayName}";
        }
        public static TestCase Create(string source, string testName)
        {
            var indexOf = LastIndexOfDisplayName(testName);
            var fullyQualifiedName = testName.Substring(0, indexOf);
            var displayName = testName.Substring(indexOf + 1);

            var testCase = new TestCase(fullyQualifiedName, TestAdapter.ExecutorUri, source)
            {
                DisplayName = displayName,
                Id = GetGuid($"{testName}"),
            };

            return testCase;
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

        private static System.Guid GetGuid(string name)
        {
            return new System.Guid(name.GetHashCode(), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }
}