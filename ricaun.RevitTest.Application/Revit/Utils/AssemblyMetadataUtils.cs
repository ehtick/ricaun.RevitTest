using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.Application.Revit
{
    public static class AssemblyMetadataUtils
    {
        public static string Get(this AssemblyMetadataAttribute[] attributes, string key)
        {
            var attribute = attributes.FirstOrDefault(e => e.Key.IndexOf(key, System.StringComparison.InvariantCultureIgnoreCase) != -1);
            return attribute?.Value;
        }

        public static double GetDouble(this AssemblyMetadataAttribute[] attributes, string key, double defaultValue = 0.0)
        {
            var value = attributes.Get(key);
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return defaultValue;
        }

        public static double GetDouble(this Assembly assembly, string key, double defaultValue = 0.0)
        {
            var value = assembly.Get(key);
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return defaultValue;
        }

        public static string Get(this Assembly assembly, string key)
        {
            return assembly.GetAssemblyMetadata(key)?.Value;
        }

        internal static AssemblyMetadataAttribute GetAssemblyMetadata(this Assembly assembly, string key)
        {
            return assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(e => e.Key.IndexOf(key, System.StringComparison.InvariantCultureIgnoreCase) != -1);
        }
    }
}