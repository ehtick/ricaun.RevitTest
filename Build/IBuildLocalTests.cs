using Nuke.Common;

public interface IBuildLocalTests : IHazTest, IBuildConsole
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
            TestProjects(TestLocalProjectName);
        });

}
