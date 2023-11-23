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
            Initilize();
        }

        public static void Initilize()
        {
            CreateFile();
        }

        public static void Finish()
        {
            RemoveFile();
        }

        #region Jornal
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
        #endregion

        #region File
        private static string FileName = "RevitTest_{0}.log";
        private static string FilePath = null;

        private static void CreateFile()
        {
            if (FilePath is null)
            {
                FilePath = Path.Combine(Path.GetTempPath(), string.Format(FileName, DateTime.Now.Ticks));
            }
        }

        private static void RemoveFile()
        {
            if (FilePath is null) return;
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
        private static void WriteFile(string value)
        {
            try
            {
                if (FilePath is null) return;
                value = $"{DateTime.Now}: {value}{Environment.NewLine}";
                File.AppendAllText(FilePath, value);
            }
            catch { }
        }

        public static void OpenFile()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    Process.Start(FilePath);
                }
                catch (Exception)
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {FilePath}"));
                }
            }
        }
        #endregion

        #region WriteLine
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
        #endregion
    }

}