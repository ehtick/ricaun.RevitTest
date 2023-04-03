using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Reflection;

//[assembly: AssemblyMetadata("NUnit.RevitVersion", "2022")]
[assembly: AssemblyMetadata("NUnit.RevitOpen", "false")]
[assembly: AssemblyMetadata("NUnit.RevitClose", "false")]
[assembly: AssemblyMetadata("NUnit.Verbosity", "5")]

namespace ricaun.RevitTest.Tests
{
#if DEBUG
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