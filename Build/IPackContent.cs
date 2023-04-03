using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using ricaun.Nuke.Components;
using ricaun.Nuke.Extensions;

/// <summary>
/// IPackContent
/// </summary>
public interface IPackContent : IHazPack, IHazContent, ISign, IHazGitRepository, INukeBuild, IBuildConsole, IGitRelease
{
    /// <summary>
    /// Target Pack
    /// </summary>
    Target Pack => _ => _
        .TriggeredBy(BuildConsole)
        .After(GitRelease)
        .OnlyWhenStatic(() => NugetApiUrl.SkipEmpty())
        .OnlyWhenStatic(() => NugetApiKey.SkipEmpty())
        .OnlyWhenStatic(() => IsServerBuild)
        .OnlyWhenDynamic(() => GitRepository.IsOnMainOrMasterBranch())
        .Executes(() =>
        {
            PathConstruction.GlobFiles(ContentDirectory, "**/*.nupkg")
               .ForEach(x =>
               {
                   DotNetTasks.DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey)
                        .EnableSkipDuplicate()
                   );
               });
        });
}
