using Nuke.Common;
using Nuke.Common.Tools.DotNet;

public interface IBuildLocalTests : IBuildConsole, IHazTest
{
    /// <summary>
    /// TestLocalProjectName (Default: "*.Tests")
    /// </summary>
    [Parameter]
    string TestLocalProjectName => TryGetValue<string>(() => TestLocalProjectName) ?? "*.Tests";

    Target BuildTests => _ => _
        .TriggeredBy(BuildConsole)
        .OnlyWhenStatic(() => IsLocalBuild)
        .Executes(() =>
        {
            TestProjects(TestLocalProjectName, customDotNetTestSettings: (setting) =>
            {
                return setting
                    .SetRunSetting("NUnit.RevitVersion", 2021)
                    //.SetVerbosity(DotNetVerbosity.Normal)
                    //.SetRunSetting("NUnit.Verbosity", 1)
                    .SetRunSetting("NUnit.RevitOpen", true)
                    .SetRunSetting("NUnit.RevitClose", true);
            });
        });
}
