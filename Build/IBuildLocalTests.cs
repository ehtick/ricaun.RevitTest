using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using ricaun.Nuke.Components;

public interface IBuildLocalTests : IBuildConsole, IHazTest
{
    /// <summary>
    /// TestLocalProjectName (Default: "*.Tests")
    /// </summary>
    [Parameter]
    string TestLocalProjectName => TryGetValue<string>(() => TestLocalProjectName) ?? "*.Tests";

    Target BuildTests => _ => _
        .TriggeredBy(BuildConsole)
        .Before(Release)
        .OnlyWhenStatic(() => IsLocalBuild)
        .Executes(() =>
        {
            TestProjects(TestLocalProjectName, customDotNetTestSettings: (setting) =>
            {
                return setting
                    //.SetRunSetting("NUnit.Version", 2021)
                    //.SetVerbosity(DotNetVerbosity.Normal)
                    //.SetRunSetting("NUnit.Verbosity", 1)
                    .SetRunSetting("NUnit.Open", true)
                    .SetRunSetting("NUnit.Close", true);
            });
        });
}
