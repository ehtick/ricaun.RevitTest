using System;
using System.Collections.Generic;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    internal static class EnviromentSettings
    {
        /// <summary>
        /// Apply the environment settings.
        /// </summary>
        public static void Create()
        {
            try
            {
                var enviromentDictionary = new Dictionary<string, string>();
                var enviromentKeyNames = GetEnviromentKeyNames();
                foreach (var enviromentKeyName in enviromentKeyNames)
                {
                    var enviromentName = enviromentKeyName.Value;
                    var value = Environment.GetEnvironmentVariable(enviromentName);

                    AdapterLogger.Logger.DebugOnlyLocal($"\tEnviroment: {enviromentName} \t {value}");

                    if (value is null) 
                        continue;

                    enviromentDictionary[enviromentKeyName.Key] = value;
                    AdapterLogger.Logger.Info($"Enviroment: {enviromentName}");
                }

                MapperKey.Map(AdapterSettings.Settings, enviromentDictionary);

                AdapterLogger.Logger.DebugOnlyLocal($"\tAdapterSettings: {AdapterSettings.Settings}");
            }
            catch (Exception ex)
            {
                AdapterLogger.Logger.InfoAny($"Enviroment: {ex}");
            }
        }

        /// <summary>
        /// Gets the environment names.
        /// </summary>
        /// <returns>The environment names.</returns>
        /// <remarks>
        /// NUnit.Version -> RICAUN_REVITTEST_TESTADAPTER_NUNIT_VERSION
        /// </remarks>
        public static IEnumerable<string> GetEnviromentNames()
        {
            return GetEnviromentKeyNames().Values;
        }

        /// <summary>
        /// Gets the environment key names.
        /// </summary>
        /// <returns>The environment key names.</returns>
        /// <remarks>
        /// NUnit.Version -> RICAUN_REVITTEST_TESTADAPTER_NUNIT_VERSION
        /// </remarks>
        internal static Dictionary<string, string> GetEnviromentKeyNames()
        {
            var result = new Dictionary<string, string>();
            var assemblyName = typeof(EnviromentSettings).Assembly.GetName().Name;
            var names = MapperKey.GetNames(AdapterSettings.Settings);
            foreach (var name in names)
            {
                var fullName = $"{assemblyName}.{name}";
                result[name] = ConvertToEnviromentName(fullName);
            }
            return result;
        }

        /// <summary>
        /// Converts the name to environment name.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted environment name.</returns>
        private static string ConvertToEnviromentName(string name)
        {
            return name.Replace(".", "_").ToUpper();
        }
    }
}
