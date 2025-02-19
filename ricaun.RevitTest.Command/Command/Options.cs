using CommandLine;
using System.Collections.Generic;

namespace ricaun.RevitTest.Command
{
    public class Options
    {
        #region Parser
        public static Parser Parser { get; } = CreateParser();
        private static Parser CreateParser()
        {
            var parser = new Parser(with =>
            {
                with.HelpWriter = System.Console.Error;
                with.IgnoreUnknownArguments = true;
            });
            return parser;
        }
        #endregion

        [Option('f', "file",
        Required = true,
        HelpText = "Input file to be processed.")]
        public string File { get; set; }

        [Option('o', "output",
            HelpText = "Output file processed. (Use 'console' to output in standard output).")]
        public string Output { get; set; }

        [Option('v', "version",
          HelpText = "Force to run with Revit version.")]
        public string RevitVersion { get; set; }

        [Option("language",
          HelpText = "Force to run with Revit with language. (Default: ENU)")]
        public string RevitLanguage { get; set; }

        [Option('l', "log",
          Default = false,
          HelpText = "Prints all messages to log in standard output.")]
        public bool Log { get; set; }

        [Option('r', "read",
          Default = false,
          HelpText = "Only read the tests name.")]
        public bool Read { get; set; }

        [Option('t', "test",
          HelpText = "Filter tests using wildcard pattern separated by space. \n(Example: \"*.Test1\" \"*.Test2\")")]
        public IEnumerable<string> Test { get; set; }

        [Option("open",
          Default = false,
          HelpText = "Force to open a new Revit process.")]
        public bool ForceToOpen { get; set; }

        [Option("close",
          Default = false,
          HelpText = "Force to close the Revit process.")]
        public bool ForceToClose { get; set; }

        [Option("timeout",
          Default = 0,
          HelpText = "Timeout in minutes to abort this application.")]
        public double Timeout { get; set; }

        [Option("debugger",
            Default = false,
            Hidden = true,
            HelpText = "Force to attach debugger in the Revit process.")]
        public bool DebuggerAttach { get; set; }
    }
}