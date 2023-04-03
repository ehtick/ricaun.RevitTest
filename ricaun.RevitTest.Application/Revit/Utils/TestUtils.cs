using System;
using System.Diagnostics;

namespace ricaun.RevitTest.Application.Revit
{
    public static class TestUtils
    {
        public static string Initialize()
        {
            Log.WriteLine();
            Log.WriteLine($"TestEngine: {ricaun.NUnit.TestEngine.Initialize(out string testInitialize)} {testInitialize}");
            Log.WriteLine();
            return testInitialize;
        }

        public static string GetInitialize()
        {
            ricaun.NUnit.TestEngine.Initialize(out string testInitialize);
            return testInitialize;
        }
    }
}