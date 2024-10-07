using System;
using System.Collections.Generic;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    internal static class EnvironmentSettings
    {
        /// <summary>
        /// Apply the environment settings.
        /// </summary>
        public static void Create()
        {
            try
            {
                var environmentDictionary = new Dictionary<string, string>();
                var environmentKeyNames = GetEnvironmentKeyNames();
                foreach (var environmentKeyName in environmentKeyNames)
                {
                    var environmentName = environmentKeyName.Value;
                    var value = Environment.GetEnvironmentVariable(environmentName);

                    AdapterLogger.Logger.DebugOnlyLocal($"\tEnvironment: {environmentName} \t {value}");

                    if (value is null) 
                        continue;

                    environmentDictionary[environmentKeyName.Key] = value;
                    AdapterLogger.Logger.Info($"Environment: {environmentName}");
                }

                MapperKey.Map(AdapterSettings.Settings, environmentDictionary);

                AdapterLogger.Logger.DebugOnlyLocal($"\tAdapterSettings: {AdapterSettings.Settings}");
            }
            catch (Exception ex)
            {
                AdapterLogger.Logger.InfoAny($"Environment: {ex}");
            }
        }

        /// <summary>
        /// Gets the environment names.
        /// </summary>
        /// <returns>The environment names.</returns>
        /// <remarks>
        /// NUnit.Version -> RICAUN_REVITTEST_TESTADAPTER_NUNIT_VERSION
        /// </remarks>
        public static IEnumerable<string> GetEnvironmentNames()
        {
            return GetEnvironmentKeyNames().Values;
        }

        /// <summary>
        /// Gets the environment key names.
        /// </summary>
        /// <returns>The environment key names.</returns>
        /// <remarks>
        /// NUnit.Version -> RICAUN_REVITTEST_TESTADAPTER_NUNIT_VERSION
        /// </remarks>
        internal static Dictionary<string, string> GetEnvironmentKeyNames()
        {
            var result = new Dictionary<string, string>();
            var assemblyName = typeof(EnvironmentSettings).Assembly.GetName().Name;
            var names = MapperKey.GetNames(AdapterSettings.Settings);
            foreach (var name in names)
            {
                var fullName = $"{assemblyName}.{name}";
                result[name] = ConvertToEnvironmentName(fullName);
            }
            return result;
        }

        /// <summary>
        /// Converts the name to environment name.
        /// </summary>
        /// <param name="name">The name to convert.</param>
        /// <returns>The converted environment name.</returns>
        private static string ConvertToEnvironmentName(string name)
        {
            return name.Replace(".", "_").ToUpper();
        }
    }
}
