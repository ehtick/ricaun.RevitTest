using Autodesk.Revit.ApplicationServices;

namespace ricaun.RevitTest.Application.Revit
{
    public static class Log
    {
        static ControlledApplication ControlledApplication;
        public static void Initilize(ControlledApplication application)
        {
            ControlledApplication = application;
        }

        private static void WriteJornal(string value)
        {
            if (ControlledApplication is null) return;
            ControlledApplication.WriteJournalComment(value, false);
        }

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLine(string value)
        {
            value = " " + value;
            System.Console.WriteLine(value);
            WriteJornal(value);
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
        public static void WriteLine() => WriteLine("-------------------------------");
    }

}