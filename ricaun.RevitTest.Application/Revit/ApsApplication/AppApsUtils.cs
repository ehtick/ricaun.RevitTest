using System;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    public static class AppApsUtils
    {
        public static string AppId =>
            Assembly.GetExecutingAssembly().GetName().Name.Split(new[] { ".Dev." }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        public static string AppVersion =>
            Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
    }
}