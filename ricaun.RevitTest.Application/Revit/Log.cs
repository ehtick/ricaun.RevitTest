using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.IO;

namespace ricaun.RevitTest.Application.Revit
{
    public static class Log
    {
        private static string LogName = "ricaun.RevitTest";

        static ControlledApplication ControlledApplication;
        public static void Initilize(UIControlledApplication application)
        {
            Initilize(application.ControlledApplication);
        }
        public static void Initilize(ControlledApplication application)
        {
            ControlledApplication = application;
        }

        private static void WriteJornal(string value)
        {
            if (ControlledApplication is null) return;
            ControlledApplication?.WriteJournalComment(value, false);
        }

        public static void OpenJornal()
        {
            if (ControlledApplication is null) return;
            Process.Start(ControlledApplication.RecordingJournalFilename);
        }

        private static string FileName = "RevitTest.log";
        private static string FilePath = Path.Combine(Path.GetDirectoryName(typeof(Log).Assembly.Location), FileName);
        private static void WriteFile(string value)
        {
            value = $"{DateTime.Now}: {value}{Environment.NewLine}";
            File.AppendAllText(FilePath, value);
        }

        public static void OpenFile()
        {
            if (File.Exists(FilePath))
                Process.Start(FilePath);
        }

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLine(string value)
        {
            value = $"{LogName}: {value}";
            Debug.WriteLine(value);
            WriteJornal(value);
            WriteFile(value);
        }

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public static void WriteLine(string format, params object[] arg) => WriteLine(string.Format(format, arg));

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLine(object value) => WriteLine(value.ToString());

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="exception"></param>
        public static void WriteLine(Exception exception) => WriteLine(exception.ToString());

        /// <summary>
        /// WriteLine
        /// </summary>
        public static void WriteLine() => WriteLine("-------------------------------");
    }

}