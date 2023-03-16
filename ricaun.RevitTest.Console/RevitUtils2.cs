using System;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ricaun.RevitTest.Console
{
    public static class RevitUtils2
    {
        private const string RevitAPIName = "RevitAPI";

        #region Revit Version File
        /// <summary>
        /// TryGetRevitVersion using the RevitAPI ReferencedAssemblies
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="revitVersion"></param>
        /// <returns></returns>
        public static bool TryGetRevitVersion(string assemblyFile, out int revitVersion)
        {
            if (File.Exists(assemblyFile) == false)
                throw new FileNotFoundException();

            revitVersion = 0;

            var assemblyReferences = ReferenceLoaderUtils.GetReferencedAssemblies(assemblyFile);

            var revit = assemblyReferences
                .FirstOrDefault(e => e.Name.StartsWith(RevitAPIName));

            if (revit == null) return false;

            var version = revit.Version.Major;
            if (version < 2000) version += 2000;

            revitVersion = version;

            return true;
        }
        #endregion
    }
}