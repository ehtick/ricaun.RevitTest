using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    public static class MetadataSettings
    {
        public static void Create(string source)
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(source));
                var assemblyMetadataAttributes = assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
                        .OfType<AssemblyMetadataAttribute>().ToArray();

                foreach (var assemblyMetadataAttribute in assemblyMetadataAttributes)
                {
                    AdapterLogger.Logger.Info($"Metadata: {assemblyMetadataAttribute.Key} \t {assemblyMetadataAttribute.Value}");
                }

                //var metadataRunSettings = MetadataXmlUtils.CreateXml("RunSettings", assemblyMetadataAttributes);
                //AdapterLogger.Logger.Info(metadataRunSettings);

                MetadataMapper.Map(assemblyMetadataAttributes, AdapterSettings.Settings.NUnit);

                //Mapper.Map(metadataRunSettings.DeserializeXml<RunSettingsModel>(), AdapterSettings.Settings);

                if (assemblyMetadataAttributes.Any())
                    AdapterLogger.Logger.Info($"AdapterSettings: {AdapterSettings.Settings}");
            }
            catch { }
        }
    }
}
