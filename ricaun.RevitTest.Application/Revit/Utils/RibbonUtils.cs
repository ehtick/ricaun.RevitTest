using System.Reflection;

namespace ricaun.RevitTest.Application.Revit
{
    public static class RibbonUtils
    {
        public static string RevitTest { get; } = GetComponentImage("test-pass-light");
        public static string TestPass { get; } = GetComponentImage("test-pass-light");
        public static string TestFail { get; } = GetComponentImage("test-fail-light");
        public static string TestSkip { get; } = GetComponentImage("test-skip-light");
        public static string TestWait { get; } = GetComponentImage("test-wait-light");
        public static string GetComponentImage(string imageName)
        {
            return string.Format("Images/{0}.ico", imageName);
        }
    }
}