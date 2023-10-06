using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace ricaun.RevitTest.TestAdapter.Services
{
    internal static class TargetFrameworkUtils
    {
        /// <summary>
        /// Get Target Framework Name (Net Framework or Net Core)
        /// </summary>
        /// <returns></returns>
        public static string GetName()
        {
#if NETFRAMEWORK
            string fw = "Net Framework";
#else
            string fw = "Net Core";
#endif
            return fw;
        }

        /// <summary>
        /// GetTargetFrameworkAttribute
        /// </summary>
        /// <returns></returns>
        public static TargetFrameworkAttribute GetTargetFrameworkAttribute()
        {
            var targetFrameworkAttribute = typeof(TargetFrameworkUtils).Assembly
               .GetCustomAttribute(typeof(TargetFrameworkAttribute)) as TargetFrameworkAttribute;

            return targetFrameworkAttribute;
        }
    }
}
