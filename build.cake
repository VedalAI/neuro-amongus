var target = Argument("target", "Default");

var workflow = BuildSystem.GitHubActions.Environment.Workflow;
var buildId = workflow.RunNumber;
var tag = workflow.RefType == GitHubActionsRefType.Tag ? workflow.RefName : null;

Task("Build-ReleaseFull")
    .Does(() =>
{
    var settings = new DotNetBuildSettings
    {
        Configuration = "ReleaseFull",
        MSBuildSettings = new DotNetMSBuildSettings()
    };

    UpdateBuildSettings(settings, tag, buildId);
    DotNetBuild(".", settings);
});

Task("Build-ReleaseDataCollection")
    .Does(() =>
{
    var settings = new DotNetBuildSettings
    {
        Configuration = "ReleaseDataCollection",
        MSBuildSettings = new DotNetMSBuildSettings()
    };

    UpdateBuildSettings(settings, tag, buildId);
    DotNetBuild(".", settings);
});

Task("Default")
    .IsDependentOn("Build-ReleaseFull")
    .IsDependentOn("Build-ReleaseDataCollection");

RunTarget(target);

void UpdateBuildSettings(DotNetBuildSettings settings, string tag, int buildId)
{
    if (tag != null) 
    {
        settings.MSBuildSettings.Version = tag;
    }
    else if (buildId != 0)
    {
        settings.MSBuildSettings.VersionSuffix = "ci." + buildId;
    }
}
