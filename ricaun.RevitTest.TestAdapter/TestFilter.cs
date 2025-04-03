using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ricaun.RevitTest.TestAdapter
{
    /// <summary>
    /// Factory class for creating instances of <see cref="TestFilter"/>.
    /// </summary>
    /// <remarks>
    /// Based: https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/VsTestFilter.cs
    /// </remarks>
    public static class TestFilterFactory
    {
        public static TestFilter CreateTestFilter(IRunContext context) => new TestFilter(context);
    }

    public interface ITestFilter
    {
        ITestCaseFilterExpression TestCaseFilterExpression { get; }

        bool IsEmpty { get; }

        IEnumerable<TestCase> CheckFilter(IEnumerable<TestCase> tests);
    }
    public class TestFilter : ITestFilter
    {
        private static readonly Dictionary<string, TestProperty> SupportedPropertiesCache;
        private static readonly List<string> SupportedProperties;
        private readonly IRunContext runContext;

        static TestFilter()
        {
            SupportedPropertiesCache = new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase);
            SupportedProperties = new List<string>();

            SupportedPropertiesCache = new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase)
            {
                ["FullyQualifiedName"] = TestCaseProperties.FullyQualifiedName,
                ["Name"] = TestCaseProperties.DisplayName,
            };
        }

        public TestFilter(IRunContext runContext)
        {
            this.runContext = runContext;
        }

        private ITestCaseFilterExpression testCaseFilterExpression;
        public ITestCaseFilterExpression TestCaseFilterExpression =>
        testCaseFilterExpression ??= runContext.GetTestCaseFilter(SupportedProperties, PropertyProvider);

        public bool IsEmpty => TestCaseFilterExpression == null || TestCaseFilterExpression.TestCaseFilterValue == string.Empty;

        public IEnumerable<TestCase> CheckFilter(IEnumerable<TestCase> tests)
        {
            if (IsEmpty)
            {
                return tests;
            }

            return tests.Where(CheckFilter).ToList();
        }

        protected virtual bool CheckFilter(TestCase testCase)
        {
            return TestCaseFilterExpression?.MatchTestCase(testCase, p => PropertyValueProvider(testCase, p)) != false;
        }

        /// <summary>
        /// Provides TestProperty for property name 'propertyName' as used in filter.
        /// </summary>
        public static TestProperty LocalPropertyProvider(string propertyName)
        {
            SupportedPropertiesCache.TryGetValue(propertyName, out var testProperty);
            return testProperty;
        }

        public static TestProperty PropertyProvider(string propertyName)
        {
            var testProperty = LocalPropertyProvider(propertyName);
            if (testProperty != null)
            {
                return testProperty;
            }
            return null;
        }

        /// <summary>
        /// Provides value of TestProperty corresponding to property name 'propertyName' as used in filter.
        /// Return value should be a string for single valued property or array of strings for multivalued property (e.g. TestCategory).
        /// </summary>
        public static object PropertyValueProvider(TestCase currentTest, string propertyName)
        {
            var testProperty = LocalPropertyProvider(propertyName);
            if (testProperty != null)
            {
                // Test case might not have defined this property. In that case GetPropertyValue()
                // would return default value. For filtering, if property is not defined return null.
                if (currentTest.Properties.Contains(testProperty))
                {
                    return currentTest.GetPropertyValue(testProperty);
                }
            }
            return null;
        }

    }
}