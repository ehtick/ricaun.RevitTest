using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    public static class MetadataSettings
    {

        private static Dictionary<string, object> GroupByNamesAndKeys(Dictionary<string, string> input)
        {
            var result = new Dictionary<string, object>();

            foreach (var kvp in input)
            {
                var parts = kvp.Key.Split('.');

                if (!result.ContainsKey(parts[0]))
                {
                    result[parts[0]] = new Dictionary<string, object>();
                }

                var subDict = (Dictionary<string, object>)result[parts[0]];

                for (int i = 1; i < parts.Length - 1; i++)
                {
                    if (!subDict.ContainsKey(parts[i]))
                    {
                        subDict[parts[i]] = new Dictionary<string, object>();
                    }

                    subDict = (Dictionary<string, object>)subDict[parts[i]];
                }

                subDict[parts[parts.Length - 1]] = kvp.Value;
            }

            return result;
        }
        public static Dictionary<string, string> ConvertToDictionary(AssemblyMetadataAttribute[] metadataAttributes)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var metadataAttribute in metadataAttributes)
            {
                dictionary[metadataAttribute.Key] = metadataAttribute.Value;
            }

            return dictionary;
        }
        private static string CreateXml(string root, AssemblyMetadataAttribute[] assemblies)
        {
            var xml = new XDocument(new XElement(root, CreateXElements(assemblies)));
            using (StringWriter writer = new StringWriter())
            {
                xml.Save(writer);
                var result = writer.ToString();
                return result;
            }
        }

        private static XElement[] CreateXElements(AssemblyMetadataAttribute[] assemblies)
        {
            var result = new List<XElement>();
            var dictionary = GroupByNamesAndKeys(ConvertToDictionary(assemblies));

            foreach (var item in dictionary)
            {
                result.Add(CreateXElements(item));
            }

            return result.ToArray();
        }

        private static XElement CreateXElements(KeyValuePair<string, object> item)
        {
            var element = new XElement(item.Key);
            if (item.Value is Dictionary<string, object> dictionary)
            {
                foreach (var subItem in dictionary)
                {
                    element.Add(CreateXElements(subItem));
                }
            }
            else
            {
                element.Value = item.Value.ToString();
            }
            return element;
        }


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

                //var xml = new XDocument(
                //    new XElement("RunSettings",
                //        new XElement("NUnit", new XElement("Version", "2021"))
                //    )
                //);

                //using (StringWriter writer = new StringWriter())
                //{
                //    xml.Save(writer);
                //    var result = writer.ToString();
                //    AdapterLogger.Logger.Info(result);
                //}


                var metadataRunSettings = CreateXml("RunSettings", assemblyMetadataAttributes);
                AdapterLogger.Logger.Info(metadataRunSettings);

                //MetadataMapper.Map(assemblyMetadataAttributes, AdapterSettings.Settings.NUnit);

                //Mapper.Map(metadataRunSettings.DeserializeXml<RunSettingsModel>(), AdapterSettings.Settings);

                if (assemblyMetadataAttributes.Any())
                    AdapterLogger.Logger.Info($"AdapterSettings: {AdapterSettings.Settings}");
            }
            catch { }
        }
    }
}
