using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Reflection;

#if DEBUG
//[assembly: AssemblyMetadata("NUnit.Version", "2021")]
//[assembly: AssemblyMetadata("NUnit.Open", "true")]
//[assembly: AssemblyMetadata("NUnit.Close", "true")]
//[assembly: AssemblyMetadata("NUnit.Version", "2024")]

//[assembly: AssemblyMetadata("NUnit.Verbosity", "3")]

//[assembly: AssemblyMetadata("NUnit.Application", "RICAUN_REVIT_TEST_APPLICATION_DA4R_LOCAL")]
//[assembly: AssemblyMetadata("NUnit.Application", "NUNIT_APPLICATION_TEST")]
//[assembly: AssemblyMetadata("NUnit.Application", "RICAUN_REVIT_TEST_APPLICATION_DA4R_ONLINE_TEST")]
//#if NET
//[assembly: AssemblyMetadata("NUnit.Application", "..\\..\\..\\..\\ricaun.RevitTest.Console\\bin\\Debug\\net8.0-windows\\ricaun.RevitTest.Console.exe")]
//#else
//[assembly: AssemblyMetadata("NUnit.Application", "..\\..\\..\\..\\ricaun.RevitTest.Console\\bin\\Debug\\net48\\ricaun.RevitTest.Console.exe")]
//#endif

//[assembly: AssemblyMetadata("NUnit.Application", "..\\..\\..\\..\\ricaun.RevitTest.Console\\bin\\Debug\\ricaun.DA4R.NUnit.Console.zip")]

//[assembly: AssemblyMetadata("NUnit.Language", "ENU /hosted")]
#endif

[assembly: AssemblyMetadata("NUnit.Timeout", "5.5")]

namespace ricaun.RevitTest.Tests
{
#if DEBUG
    [Explicit]
    public class TestsDebugger
    {
        [Test(ExpectedResult = true)]
        public bool Test()
        {
            Debug.WriteLine($"Debugger.IsAttached: {Debugger.IsAttached}");
            Console.WriteLine($"Debugger.IsAttached: {Debugger.IsAttached}");
            return Debugger.IsAttached;
        }
    }
#endif
}