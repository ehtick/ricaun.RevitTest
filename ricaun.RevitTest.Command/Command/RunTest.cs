using CommandLine;

namespace ricaun.RevitTest.Command
{
    public static class RunTest
    {
        public static bool ParseArguments<T>(string[] args) where T : IRunTestService, new()
        {
            var result = true;
            Options.Parser.ParseArguments<Options>(args)
                .WithParsed((options) =>
                {
                    new RunCommand<T>(options).Run();
                })
                .WithNotParsed((errors) =>
                {
                    if (errors.IsHelp()) return;
                    if (errors.IsVersion()) return;
                    result = false;
                });
            return result;
        }
    }
}