using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    internal static class MetadataSettings
    {
        public static void Create(string source)
        {
            try
            {
                if (AdapterSettings.Settings.NUnit.Metadata == false)
                {
                    AdapterLogger.Logger.Warning($"NUnit.Metadata: Disabled");
                    return;
                }

                var assembly = Assembly.Load(File.ReadAllBytes(source));
                var assemblyMetadataAttributes = assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
                        .OfType<AssemblyMetadataAttribute>().ToArray();

                foreach (var assemblyMetadataAttribute in assemblyMetadataAttributes)
                {
                    AdapterLogger.Logger.Info($"Metadata: {assemblyMetadataAttribute.Key} \t {assemblyMetadataAttribute.Value}");
                }

#if DEBUG
                AdapterLogger.Logger.Warning($"-");
                foreach (var assemblyMetadataAttribute in assemblyMetadataAttributes)
                {
                    AdapterLogger.Logger.Warning($"Metadata: {assemblyMetadataAttribute.Key} \t {assemblyMetadataAttribute.Value}");
                }
                AdapterLogger.Logger.Warning($"-");
#endif

                if (assemblyMetadataAttributes.Any(e => e.Key.StartsWith(nameof(AdapterSettings.Settings.NUnit))))
                {
                    MetadataMapper.Map(AdapterSettings.Settings, assemblyMetadataAttributes);
                    AdapterLogger.SetVerbosity(AdapterSettings.Settings.NUnit.Verbosity);
                    AdapterLogger.Logger.Info($"AdapterSettings: {AdapterSettings.Settings}");
                }

#if DEBUG
                AdapterLogger.Logger.Warning($"AdapterSettings: {AdapterSettings.Settings}");
                AdapterLogger.Logger.Warning($"-");
#endif
            }
            catch (Exception ex)
            {
                AdapterLogger.Logger.Info($"Metadata: {ex}", 0);
            }
        }
    }
}
