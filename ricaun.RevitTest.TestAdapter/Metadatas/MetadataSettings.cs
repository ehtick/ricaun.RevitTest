﻿using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    internal static class MetadataSettings
    {
        public static void Create(string source)
        {
            try
            {
                if (AdapterSettings.Settings.NUnit.Metadata == false) return;

                var assembly = Assembly.Load(File.ReadAllBytes(source));
                var assemblyMetadataAttributes = assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
                        .OfType<AssemblyMetadataAttribute>().ToArray();

                foreach (var assemblyMetadataAttribute in assemblyMetadataAttributes)
                {
                    AdapterLogger.Logger.Info($"Metadata: {assemblyMetadataAttribute.Key} \t {assemblyMetadataAttribute.Value}");
                }

                if (assemblyMetadataAttributes.Any(e => e.Key.StartsWith(nameof(AdapterSettings.Settings.NUnit))))
                {
                    MetadataMapper.Map(AdapterSettings.Settings.NUnit, assemblyMetadataAttributes);
                    AdapterLogger.SetVerbosity(AdapterSettings.Settings.NUnit.Verbosity);
                    AdapterLogger.Logger.Info($"AdapterSettings: {AdapterSettings.Settings}");
                }
            }
            catch (Exception ex)
            {
                AdapterLogger.Logger.Info($"Metadata: {ex}", 0);
            }
        }
    }
}
