// **************************************************
// TestAdapter.cs based
// https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/NUnitTestAdapter.cs
// **************************************************

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using ricaun.RevitTest.TestAdapter.Services;
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
        protected TestAdapter()
        {
            var assemblyName = typeof(TestAdapter).GetTypeInfo().Assembly.GetName();
            AdapterVersion = $"{assemblyName.Name} {assemblyName.Version.ToString(3)}";
        }

        protected void Initialize(IDiscoveryContext discoveryContext, IMessageLogger messageLogger)
        {
            AdapterSettings.Create(discoveryContext);
            AdapterLogger.Create(messageLogger, AdapterSettings.Settings.NUnit.Verbosity);

            AdapterLogger.Logger.Info($"TestAdapter: {this.AdapterVersion}");
            AdapterLogger.Logger.Info($"AdapterSettings: {AdapterSettings.Settings}");
            //AdapterLogger.Logger.Debug($"SettingsXml: {discoveryContext.RunSettings.SettingsXml}");

            if (messageLogger is IFrameworkHandle frameworkHandle)
                ProcessDebug.Initialize(frameworkHandle);
        }
    }
}