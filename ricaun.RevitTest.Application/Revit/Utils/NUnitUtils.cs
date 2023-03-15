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
            Assembly[] assemblies = GetAssemblies();

            foreach (var assembly in assemblies)
            {
                Console.WriteLine($"AppDomain: {assembly}");
            }

            //var hasversion = assemblies.Any(e => e.GetName().Version.ToString(3) == NUNIT_VERSION);
            //if (!hasversion)
            //{
            //    Console.WriteLine($"LoadFile: {LoadFile()}");
            //}

            //TestUtils.Initialize();
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