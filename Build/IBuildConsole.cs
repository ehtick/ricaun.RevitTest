using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using ricaun.Nuke.Components;
using ricaun.Nuke.Extensions;

public interface IBuildConsole : IHazExample, IRevitPackageBuilder
{
    public AbsolutePath GetExampleDirectory(Project project) => project.Directory / "bin" / "Release";

    Target BuildConsole => _ => _
        .TriggeredBy(PackageBuilder)
        .Before(Release)
        .Executes(() =>
        {
            var project = Solution.GetOtherProject("ricaun.RevitTest.Console");

            var releaseDirectory = GetReleaseDirectory(MainProject);
            Globbing.GlobFiles(releaseDirectory, "**/*.bundle.zip")
                .ForEach(file =>
                {
                    var resourcesDirectory = project.Directory / "Resources";
                    Serilog.Log.Information($"Copy Bundle: {file} to {resourcesDirectory}");
                    AbsolutePathExtensions.CopyToDirectory(file, resourcesDirectory, ExistsPolicy.FileOverwriteIfNewer);
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

            var projectTestAdapter = Solution.GetOtherProject("ricaun.RevitTest.TestAdapter");

            var releaseDirectoryConsole = GetExampleDirectory(Solution.GetOtherProject("ricaun.RevitTest.Console"));
            Globbing.GlobFiles(releaseDirectoryConsole, "**/*.exe")
                .ForEach(file =>
                {
                    var resourcesDirectory = projectTestAdapter.Directory / "Resources";
                    Serilog.Log.Information($"Copy Exe: {file} to {resourcesDirectory}");

                    var directory = file.Parent;
                    var resourceNet = resourcesDirectory / directory.Name;
                    resourceNet.DeleteDirectory();

                    var zipPath = resourceNet / file.Name;
                    ZipExtension.CreateFromDirectory(directory, zipPath);
                });

            Solution.BuildProject(projectTestAdapter, (project) =>
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
