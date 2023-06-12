using System;

namespace ricaun.RevitTest.Command.Utils
{
    public static class DebuggerUtils
    {
        private static bool DebuggerAttached { get; set; }
        public static void AttachedDebugger(bool debugger)
        {
            DebuggerAttached = debugger;
        }
        public static bool IsDebuggerAttached { get { return System.Diagnostics.Debugger.IsAttached | DebuggerAttached; } }
    }
}