using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.Tests
{
    public class TestsRevitUI
    {
        static IEnumerable<Type> GetTypes()
        {
            yield return typeof(UIApplication);
            yield return typeof(Application);
            yield return typeof(UIFramework.RevitRibbonControl);
            yield return typeof(Autodesk.Windows.RibbonControl);
        }
        [TestCaseSource(nameof(GetTypes))]
        public void TestTypes(Type type)
        {
            Console.WriteLine(type);
        }

#if DEBUG
        static IEnumerable<Assembly> GetAssemblyTypes()
        {
            return GetTypes().Select(t => t.Assembly);
        }

        [TestCaseSource(nameof(GetAssemblyTypes))]
        public void TestAssemblyTypes(Assembly assembly)
        {
            Console.WriteLine(assembly);
        }
#endif
    }
}