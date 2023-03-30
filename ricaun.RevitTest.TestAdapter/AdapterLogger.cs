using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;

namespace ricaun.RevitTest.TestAdapter
{
    public class AdapterLogger
    {
        private static AdapterLogger Instance { get; set; }
        public static ITestLogger Logger
        {
            get
            {
                if (Instance == null)
                    return new NoneTestLogger();

                return Instance.TestLogger;
            }
        }
        public static void Create(IMessageLogger messageLogger, int verbosity = 10)
        {
            if (Instance == null)
            {
                Instance = new AdapterLogger();
                Instance.Initialize(messageLogger);
                Instance.TestLogger.Verbosity = verbosity;
            }
        }

        public ITestLogger TestLogger { get; set; }
        public void Initialize(IMessageLogger messageLogger)
        {
            this.TestLogger = new TestLogger(messageLogger);
        }

        public class NoneTestLogger : ITestLogger
        {
            public int Verbosity { get; set; }

            public void Debug(string message)
            {
            }

            public void Error(string message)
            {
            }

            public void Error(string message, Exception ex)
            {
            }

            public void Info(string message)
            {
            }

            public void Warning(string message)
            {
            }

            public void Warning(string message, Exception ex)
            {
            }
        }
    }
}
