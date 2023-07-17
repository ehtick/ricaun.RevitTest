// **************************************************
// TestLogger.cs based
// https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/TestLogger.cs
// **************************************************

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ricaun.RevitTest.TestAdapter
{
    /// <summary>
    /// ITestLogger
    /// </summary>
    internal interface ITestLogger
    {
        void Error(string message);
        void Error(string message, Exception ex);
        void Warning(string message);
        void Warning(string message, Exception ex);
        void Info(string message);
        void Info(string message, int verbosity);
        int Verbosity { get; set; }
        void Debug(string message);
    }

    /// <summary>
    /// TestLogger wraps an IMessageLogger and adds various
    /// utility methods for sending messages. Since the
    /// IMessageLogger is only provided when the discovery
    /// and execution objects are called, we use two-phase
    /// construction. Until Initialize is called, the logger
    /// simply swallows all messages without sending them
    /// anywhere.
    /// </summary>
    internal class TestLogger : IMessageLogger, ITestLogger
    {
        private const string EXCEPTION_FORMAT = "Exception {0}, {1}";

        private IMessageLogger MessageLogger { get; }

        public int Verbosity { get; set; }

        public TestLogger(IMessageLogger messageLogger)
        {
            MessageLogger = messageLogger;
        }

        #region Error Messages

        public void Error(string message)
        {
            SendMessage(TestMessageLevel.Error, message);
        }

        public void Error(string message, Exception ex)
        {
            SendMessage(TestMessageLevel.Error, message, ex);
        }

        #endregion

        #region Warning Messages

        public void Warning(string message)
        {
            SendMessage(TestMessageLevel.Warning, message);
        }

        public void Warning(string message, Exception ex)
        {
            SendMessage(TestMessageLevel.Warning, message, ex);
        }

        #endregion

        #region Information Messages

        public void Info(string message)
        {
            Info(message, 1);
        }

        public void Info(string message, int verbosity)
        {
            //if (adapterSettings?.Verbosity >= 0)
            if (Verbosity >= verbosity)
                SendMessage(TestMessageLevel.Informational, message);
        }

        #endregion

        #region Debug Messages

        public void Debug(string message)
        {
            if (Verbosity >= 2)
                SendMessage(TestMessageLevel.Informational, message);
        }

        #endregion

        #region SendMessage

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            MessageLogger?.SendMessage(testMessageLevel, message);
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message, Exception ex)
        {
            switch (Verbosity)
            {
                case 0:
                    var type = ex.GetType();
                    SendMessage(testMessageLevel, string.Format(EXCEPTION_FORMAT, type, message));
                    SendMessage(testMessageLevel, ex.Message);
                    SendMessage(testMessageLevel, ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        SendMessage(testMessageLevel, $"InnerException: {ex.InnerException}");
                    }
                    break;

                default:
                    SendMessage(testMessageLevel, message);
                    SendMessage(testMessageLevel, ex.ToString());
                    SendMessage(testMessageLevel, ex.StackTrace);
                    break;
            }
        }
        #endregion

        #region SpecializedMessages
        public void DebugRunfrom()
        {
#if NET35
            string fw = "Net Framework";
#else
            string fw = "Net Core";
#endif
            var assLoc = Assembly.GetExecutingAssembly().Location;
            Debug($"{fw} adapter running from {assLoc}");
        }

        public void InfoNoTests(bool discoveryResultsHasNoNUnitTests, string assemblyPath)
        {
            Info(discoveryResultsHasNoNUnitTests
                ? "   NUnit couldn't find any tests in " + assemblyPath
                : "   NUnit failed to load " + assemblyPath);
        }

        public void InfoNoTests(string assemblyPath)
        {
            Info($"   NUnit couldn't find any tests in {assemblyPath}");
        }
        #endregion
    }

    /// <summary>
    /// ITestLoggerExtension
    /// </summary>
    internal static class ITestLoggerExtension
    {
        [Conditional("DEBUG")]
        internal static void DebugOnlyLocal(this ITestLogger logger, string message)
        {
            logger.Warning($"\tDEBUG: {message}");
        }
    }
}