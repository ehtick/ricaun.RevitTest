﻿using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using ricaun.Nuke.Components;
using ricaun.Nuke.Extensions;

public interface IBuildConsole : IHazExample, IRevitPackageBuilder
{
    Target BuildConsole => _ => _
        .TriggeredBy(PackageBuilder)
        .Executes(() =>
        {
            var project = Solution.GetOtherProject("ricaun.RevitTest.Console");

            var releaseDirectory = GetReleaseDirectory(MainProject);
            PathConstruction.GlobFiles(releaseDirectory, "**/*.bundle.zip")
                .ForEach(file =>
                {
                    var resourcesDirectory = project.Directory / "Resources";
                    Serilog.Log.Information($"Copy Bundle: {file} to {resourcesDirectory}");
                    FileSystemTasks.CopyFileToDirectory(file, resourcesDirectory, FileExistsPolicy.OverwriteIfNewer);
                });

            Solution.BuildProject(project, (project) =>
            {
                SignProject(project);
                var fileName = project.Name;
                var version = project.GetInformationalVersion();
                var exampleDirectory = GetExampleDirectory(project);

                var releaseFileName = CreateReleaseFromDirectory(exampleDirectory, fileName, version);
                Serilog.Log.Information($"Release: {releaseFileName}");
            });
        });
}