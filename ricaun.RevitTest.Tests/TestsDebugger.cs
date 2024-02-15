using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Reflection;

#if DEBUG
//[assembly: AssemblyMetadata("NUnit.Version", "2021")]
//[assembly: AssemblyMetadata("NUnit.Open", "true")]
//[assembly: AssemblyMetadata("NUnit.Close", "true")]
//[assembly: AssemblyMetadata("NUnit.Version", "2024")]

//[assembly: AssemblyMetadata("NUnit.Verbosity", "2")]

[assembly: AssemblyMetadata("NUnit.Application", "RICAUN_REVIT_TEST_APPLICATION_DA4R_LOCAL")]
[assembly: AssemblyMetadata("NUnit.Application", "NUNIT_APPLICATION_TEST")]
[assembly: AssemblyMetadata("NUnit.Application", "RICAUN_REVIT_TEST_APPLICATION_DA4R_ONLINE_TEST")]
[assembly: AssemblyMetadata("NUnit.Application", "D:\\Users\\ricau\\source\\repos\\ricaun.RevitTest\\ricaun.RevitTest.Console\\bin\\Debug\\ricaun.RevitTest.Console.exe")]
//[assembly: AssemblyMetadata("NUnit.Language", "ENU /hosted")]

//[assembly: AssemblyMetadata("NUnit.Application", "C:\\Users\\ricau\\Downloads\\SampleTest\\ricaun.DA4R.NUnit.Console.zip")]
//[assembly: AssemblyMetadata("NUnit.Application", "..\\..\\..\\ricaun.RevitTest.Console\\bin\\Debug\\ricaun.RevitTest.Console.exe")]
#endif

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