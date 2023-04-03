using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using ricaun.Nuke.Components;
using ricaun.Nuke.Extensions;

/// <summary>
/// IPackRelease
/// </summary>
public interface IPackRelease : IHazPack, IHazContent, ISign, IHazGitRepository, INukeBuild, IBuildConsole, IGitRelease
{
    /// <summary>
    /// Target Pack
    /// </summary>
    Target PackRelease => _ => _
        .TriggeredBy(BuildConsole)
        .After(GitRelease)
        .OnlyWhenStatic(() => NugetApiUrl.SkipEmpty())
        .OnlyWhenStatic(() => NugetApiKey.SkipEmpty())
        .OnlyWhenStatic(() => IsServerBuild)
        .OnlyWhenDynamic(() => GitRepository.IsOnMainOrMasterBranch())
        .Executes(() =>
        {
            var releaseDirectory = GetReleaseDirectory(MainProject);
            PathConstruction.GlobFiles(releaseDirectory, "**/*.nupkg")
               .ForEach(x =>
               {
                   Serilog.Log.Information($"DotNetNuGetPush: {x}");
                   DotNetTasks.DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey)
                        .EnableSkipDuplicate()
                   );
               });
        });
}
