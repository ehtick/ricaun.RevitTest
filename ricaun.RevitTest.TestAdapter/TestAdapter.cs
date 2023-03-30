// **************************************************
// TestAdapter.cs based
// https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/NUnitTestAdapter.cs
// **************************************************

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Reflection;

namespace ricaun.RevitTest.TestAdapter
{
    public abstract class TestAdapter
    {
        public const string ExecutorUriString = "executor://ricaun.RevitTest.TestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        // The adapter version
        protected string AdapterVersion { get; set; }
        // Our logger used to display messages
        public TestLogger TestLog { get; private set; }
        protected TestAdapter()
        {
            AdapterVersion = typeof(TestAdapter).GetTypeInfo().Assembly.GetName().Version.ToString();
        }

        // The Adapter is constructed using the default constructor.
        // We don't have any info to initialize it until one of the
        // ITestDiscovery or ITestExecutor methods is called. Each
        // Discover or Execute method must call this method.
        protected void Initialize(IMessageLogger messageLogger)
        {
            TestLog = new TestLogger(messageLogger);
            try
            {

            }
            catch (Exception e)
            {
                TestLog.Warning("Error initializing RunSettings. Default settings will be used");
                TestLog.Warning(e.ToString());
            }
            finally
            {
                TestLog.DebugRunfrom();
            }
        }
    }
}