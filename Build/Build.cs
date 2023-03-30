using Nuke.Common;
using Nuke.Common.Execution;
using ricaun.Nuke;
using ricaun.Nuke.Components;

class Build : NukeBuild, IPublishRevit, IBuildConsole, IBuildLocalTests
{
    string IHazRevitPackageBuilder.Application => "Revit.App";
    bool IHazPackageBuilderProject.ReleasePackageBuilder => false;
    string IHazMainProject.MainName => "ricaun.RevitTest.Application";
    public static int Main() => Execute<Build>(x => x.From<IPublishRevit>().Build);
}
