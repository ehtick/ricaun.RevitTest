using ricaun.RevitTest.TestAdapter.Extensions;

namespace ricaun.RevitTest.TestAdapter.Services
{
    internal static class ResourceConsoleUtils
    {
        public static string Name => GetName();

        private static string GetName()
        {
            return Properties.Resources.ricaun_RevitTest_Console_Name;
        }

        public static string CopyToFile(string file)
        {
#if NET7_0_OR_GREATER
            return Properties.NetCore.Resources.ricaun_RevitTest_Console.CopyToFile(file);
#elif NETFRAMEWORK
            return Properties.NetFramework.Resources.ricaun_RevitTest_Console.CopyToFile(file);
#endif
        }
    }
}