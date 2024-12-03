using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using ricaun.Nuke.Extensions;

public interface IBuildCommand : IBuildConsole
{
    Target BuildCommand => _ => _
        .TriggeredBy(BuildConsole)
        .Before(Release)
        .Executes(() =>
        {
            var project = Solution.GetOtherProject("ricaun.RevitTest.Command");
            var releaseDirectory = GetReleaseDirectory(MainProject);

            Solution.BuildProject(project, (project) =>
            {
                SignProject(project);
                var fileName = project.Name;
                var version = project.GetInformationalVersion();
                var exampleDirectory = GetExampleDirectory(project);

                var releaseFileName = CreateReleaseFromDirectory(exampleDirectory, fileName, version);
                Serilog.Log.Information($"Release: {releaseFileName}");

                Globbing.GlobFiles(exampleDirectory, "**/*.nupkg")
                    .ForEach(file =>
                    {
                        Serilog.Log.Information($"Copy nupkg: {file} to {releaseDirectory}");
                        AbsolutePathExtensions.CopyToDirectory(file, releaseDirectory, ExistsPolicy.FileOverwriteIfNewer);
                    });

            });

        });
}
