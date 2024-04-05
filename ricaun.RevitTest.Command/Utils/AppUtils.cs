using System.Reflection;

namespace ricaun.RevitTest.Command.Utils
{
    public static class AppUtils
    {
        /// <summary>
        /// Get Info (Assembly Name, Version and Target Framework)
        /// </summary>
        /// <returns></returns>
        public static string GetInfo()
        {
            var assemblyName = typeof(AppUtils).Assembly.GetName();
            var info = $"{assemblyName.Name} {GetSemanticVersion()} [{GetTargetFrameworkName()}]";
            return info;
        }

        /// <summary>
        /// Get Semantic Version
        /// </summary>
        /// <returns></returns>
        public static string GetSemanticVersion() {

            var assembly = typeof(AppUtils).Assembly;
            var version = assembly.GetName().Version.ToString(3);

            try
            {
                var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                version = informationalVersion.Split('+')[0];
            }
            catch { }

            return version;
        }

        /// <summary>
        /// Get Target Framework Name (Net Framework or Net Core)
        /// </summary>
        /// <returns></returns>
        public static string GetTargetFrameworkName()
        {
#if NETFRAMEWORK
            string fw = "Net Framework";
#else
            string fw = "Net Core";
#endif
            return fw;
        }
    }
}
