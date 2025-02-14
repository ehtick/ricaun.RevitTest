// **************************************************
// TestAdapter.cs based
// https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/NUnitTestAdapter.cs
// **************************************************

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using ricaun.RevitTest.TestAdapter.Metadatas;
using ricaun.RevitTest.TestAdapter.Models;
using ricaun.RevitTest.TestAdapter.Services;
using System;

namespace ricaun.RevitTest.TestAdapter
{
    internal abstract class TestAdapter
    {
        public const string ExecutorUriString = "executor://ricaun.RevitTest.TestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        // The adapter version
        protected string AdapterVersion { get; set; }
        // Our logger used to display messages
        protected TestAdapter()
        {
            var assembly = typeof(TestAdapter).Assembly;
            var assemblyName = assembly.GetName();
            var version = assemblyName.Version.ToString(3);
            var frameworkName = TargetFrameworkUtils.GetName();

            try
            {
                var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                version = fileVersionInfo.GetSafeProductVersion();
            }
            catch { }

            AdapterVersion = $"{assemblyName.Name} \t{version} \t[{frameworkName}]";
        }

        protected void Initialize(IDiscoveryContext discoveryContext, IMessageLogger messageLogger)
        {
            AdapterSettings.Create(discoveryContext);
            EnvironmentSettings.Create();
            AdapterLogger.Create(messageLogger, AdapterSettings.Settings.NUnit.Verbosity);

            AdapterLogger.Logger.InfoAny($"TestAdapter: {this.AdapterVersion}");
            AdapterLogger.Logger.Info($"AdapterSettings: {AdapterSettings.Settings}");

            if (AdapterSettings.Instance.RunSettings is null)
            {
                AdapterLogger.Logger.Error($"RunSettings is null");
            }

#if DEBUG
            AdapterLogger.Logger.DebugOnlyLocal("-DEBUG-");
            AdapterLogger.Logger.DebugOnlyLocal($"\tTestAdapter: {this.AdapterVersion}");
            AdapterLogger.Logger.DebugOnlyLocal($"\tAdapterSettings: {AdapterSettings.Settings}");

            var collection = XmlUtils.ParseKeyValues(discoveryContext.RunSettings.SettingsXml);
            foreach (var item in collection)
            {
                AdapterLogger.Logger.DebugOnlyLocal($"\t{item.Key}: {item.Value}");
            }

            AdapterLogger.Logger.DebugOnlyLocal("-");

            foreach (var item in MapperKey.GetNames(AdapterSettings.Instance))
            {
                AdapterLogger.Logger.DebugOnlyLocal($"\t{item}");
            }

            AdapterLogger.Logger.DebugOnlyLocal("-");

            foreach (var item in EnvironmentSettings.GetEnvironmentNames())
            {
                AdapterLogger.Logger.DebugOnlyLocal($"\tEnvironment: {item}");
            }

            //Environment.SetEnvironmentVariable("RICAUN_REVITTEST_TESTADAPTER_NUNIT_VERBOSITY", "3");

            AdapterLogger.Logger.DebugOnlyLocal("-DEBUG-");

            //var RunSettings = Metadatas.Mapper.Map(collection, new RunSettingsModel());
            //AdapterLogger.Logger.Warning($"RunSettings: {RunSettings}");
            // AdapterLogger.Logger.Warning($"SettingsXml: {discoveryContext.RunSettings.SettingsXml}");
#endif
        }
    }
}