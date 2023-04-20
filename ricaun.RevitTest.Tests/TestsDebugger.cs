using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Reflection;

#if DEBUG
//[assembly: AssemblyMetadata("NUnit.Version", "2021")]
//[assembly: AssemblyMetadata("NUnit.Open", "true")]
//[assembly: AssemblyMetadata("NUnit.Close", "true")]
[assembly: AssemblyMetadata("NUnit.Version", "2024")]
[assembly: AssemblyMetadata("NUnit.Verbosity", "1")]
[assembly: AssemblyMetadata("NUnit.Application", "D:\\Users\\ricau\\source\\repos\\ricaun.RevitTest\\ricaun.RevitTest.Console\\bin\\Debug\\ricaun.RevitTest.Console.exe")]
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