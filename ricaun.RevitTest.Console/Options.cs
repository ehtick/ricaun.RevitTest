using CommandLine;

namespace ricaun.RevitTest.Console
{
    public class Options
    {
        [Option('f', "file",
        Required = true,
        HelpText = "Input file to be processed.")]
        public string File { get; set; }

        [Option('o', "output",
            HelpText = "Output file processed. (Use 'console' to output in 'System.Console')")]
        public string Output { get; set; }

        [Option('v', "version",
          HelpText = "Force to run with Revit version.")]
        public int Version { get; set; }

        [Option('l', "log",
          Default = false,
          HelpText = "Prints all messages to log in standard output.")]
        public bool Log { get; set; }

        [Option('r', "read",
          Default = false,
          HelpText = "Only read the tests name.")]
        public bool Read { get; set; }
    }
}