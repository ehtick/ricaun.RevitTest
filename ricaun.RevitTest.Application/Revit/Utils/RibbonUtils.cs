using System.Reflection;

namespace ricaun.RevitTest.Application.Revit
{
    public static class RibbonUtils
    {
        public static string TestPass { get; } = GetComponentImage("test-pass");
        public static string TestFail { get; } = GetComponentImage("test-fail");
        public static string TestSkip { get; } = GetComponentImage("test-skip");
        public static string TestWait { get; } = GetComponentImage("test-wait");
        public static string GetComponentImage(string imageName)
        {
            return string.Format("Images/{0}.png", imageName);
        }
        static string GetComponentImagePack(string imageName)
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            return string.Format("pack://application:,,,/{0};component/Images/{1}.png", assemblyName, imageName);
        }
    }
}