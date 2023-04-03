using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.Application.Revit
{
    public static class NUnitUtils
    {
        private const string NUNIT_NAME = "nunit.framework";
        private const string NUNIT_VERSION = "3.13.3";

        public static void Initialize()
        {
            Log.WriteLine();
            Log.WriteLine($"TestEngine: {ricaun.NUnit.TestEngine.Initialize(out string testInitialize)} {testInitialize}");
            Log.WriteLine();

            Assembly[] assemblies = GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Log.WriteLine($"AppDomain: {assembly}");
            }
        }

        /// <summary>
        /// GetAssemblies with "nunit.framework"
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetAssemblies()
        {
            var nunits = AppDomain.CurrentDomain.GetAssemblies().Where(e => e.GetName().Name.StartsWith(NUNIT_NAME));
            return nunits.ToArray();
        }

        /// <summary>
        /// Force to Load "nunit.framework"
        /// </summary>
        /// <returns></returns>
        public static Assembly LoadFile()
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{NUNIT_NAME}.dll");
            var file = files.FirstOrDefault();
            if (file != null)
            {
                return Assembly.LoadFrom(file);
            }
            return null;
        }
    }

}