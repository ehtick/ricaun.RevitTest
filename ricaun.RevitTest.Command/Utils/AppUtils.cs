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
            var info = $"{assemblyName.Name} {assemblyName.Version.ToString(3)} [{GetTargetFrameworkName()}]";
            return info;
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
