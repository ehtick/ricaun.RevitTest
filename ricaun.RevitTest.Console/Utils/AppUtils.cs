namespace ricaun.RevitTest.Console.Utils
{
    public static class AppUtils
    {
        public static string GetInfo()
        {
            var assemblyName = typeof(AppUtils).Assembly.GetName();
            var info = $"{assemblyName.Name} {assemblyName.Version.ToString(3)}";
            return info;
        }
    }
}
