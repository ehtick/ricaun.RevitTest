// **************************************************
// TestAdapter.cs based
// https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/NUnitTestAdapter.cs
// **************************************************

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using ricaun.RevitTest.TestAdapter.Models;
using System;
using System.Reflection;

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
            var assemblyName = typeof(TestAdapter).GetTypeInfo().Assembly.GetName();
            AdapterVersion = $"{assemblyName.Name} {assemblyName.Version.ToString(3)}";
        }

        protected void Initialize(IDiscoveryContext discoveryContext, IMessageLogger messageLogger)
        {
            AdapterSettings.Create(discoveryContext);
            AdapterLogger.Create(messageLogger, AdapterSettings.Settings.NUnit.Verbosity);

            var targetFrameworkAttribute = this.GetType().Assembly
               .GetCustomAttribute(typeof(System.Runtime.Versioning.TargetFrameworkAttribute)) as System.Runtime.Versioning.TargetFrameworkAttribute;

            AdapterLogger.Logger.Info($"TestAdapter: {this.AdapterVersion}", 0);
            AdapterLogger.Logger.Info($"TargetFramework: {targetFrameworkAttribute?.FrameworkName}");
            AdapterLogger.Logger.Info($"AdapterSettings: {AdapterSettings.Settings}");

            if (AdapterSettings.Instance.RunSettings is null)
            {
                AdapterLogger.Logger.Error($"RunSettings is null");
            }

#if DEBUG
            AdapterLogger.Logger.DebugOnlyLocal("-DEBUG-");
            AdapterLogger.Logger.DebugOnlyLocal($"\tTestAdapter: {this.AdapterVersion}");
            AdapterLogger.Logger.DebugOnlyLocal($"\tTargetFramework: {targetFrameworkAttribute?.FrameworkName}");
            AdapterLogger.Logger.DebugOnlyLocal($"\tAdapterSettings: {AdapterSettings.Settings}");

            var collection = Metadatas.XmlUtils.ParseKeyValues(discoveryContext.RunSettings.SettingsXml);
            foreach (var item in collection)
            {
                AdapterLogger.Logger.DebugOnlyLocal($"\t{item.Key}: {item.Value}");
            }

            AdapterLogger.Logger.DebugOnlyLocal("-");

            foreach (var item in Metadatas.MapperKey.GetNames(AdapterSettings.Instance))
            {
                AdapterLogger.Logger.DebugOnlyLocal($"\t{item}");
            }
            AdapterLogger.Logger.DebugOnlyLocal("-DEBUG-");

            //var RunSettings = Metadatas.Mapper.Map(collection, new RunSettingsModel());
            //AdapterLogger.Logger.Warning($"RunSettings: {RunSettings}");
            // AdapterLogger.Logger.Warning($"SettingsXml: {discoveryContext.RunSettings.SettingsXml}");
#endif
        }
    }
}