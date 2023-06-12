using CommandLine;
using ricaun.RevitTest.Console.Command;
using System.Collections.Generic;

namespace ricaun.RevitTest.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = RunTest.ParseArguments<Revit.RevitRunTestService>(args);

#if DEBUG
            if (!result)
                Revit.RevitDebug.ProcessServerSelect();
#endif
        }
    }
}