using System;

namespace ricaun.RevitTest.Console
{
    public static class Log
    {
        /// <summary>
        /// Enabled
        /// </summary>
        public static bool Enabled { get; set; } = true;

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="value"></param>
        public static void Write(string value)
        {
            if (Enabled)
            {
                value = " " + value;
                System.Console.Write(value);
            }
        }

        /// <summary>
        /// WriteLine
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLine(string value)
        {
            if (Enabled)
            {
                value = " " + value;
                System.Console.WriteLine(value);
            }
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
        public static void WriteLine(object value) => WriteLine(value?.ToString());

        /// <summary>
        /// WriteLine
        /// </summary>
        public static void WriteLine() => WriteLine("-------------------------------");
    }

}
