#addin "Cake.Incubator"
#tool "GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

DateTime now = DateTime.UtcNow;
var projectName = "TSC.Core.Projections";
var artifactsDir    = Directory("./artifacts");
var solutionFile = projectName + ".sln";
var devlanShare = @"\\devlan-share\Software\";
var buildFolder = devlanShare + @"Builds\" + projectName + @"\";
var nowKey = now.ToString("yyyyMMddHHmm");
var buildOutputDirectory = buildFolder + nowKey;
var resharperReportsDirectory = "S:/Builds/" + projectName + "/" + nowKey + "_ReSharperReports/";
var devlanNugetSource = devlanShare + "NugetOfflineCache";
var devlanNugetFeed = "http://tfs:8080/tfs/TscCollection/_packaging/TSC/nuget/v3/index.json";
var assemblyVersion = "1.0.0";
var assemblyFileVersion = "1.0.0";
var assemblyInfoVersion = "1.0.0";
var nugetVersion = "1.0.0";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("SetVersions")
	.Does(() =>
	{
		var result = GitVersion();
		
		assemblyVersion = string.Format("{0}.0.0.0", result.Major);
		assemblyFileVersion = result.MajorMinorPatch;
		assemblyInfoVersion = result.InformationalVersion;
		nugetVersion = assemblyFileVersion;
		Information(result.Dump());
	});


Task("Clean")
    .Does(() =>
	{
		var directoriesToClean = GetDirectories("./src/**/bin/*");
		directoriesToClean.Add(GetDirectories("./src/**/obj/*"));
		directoriesToClean.Add(GetDirectories("./test/**/bin/*"));
		directoriesToClean.Add(GetDirectories("./test/**/obj/*"));
		directoriesToClean.Add(artifactsDir);

		CleanDirectories(directoriesToClean);
		Information("All clean and shiny!!");
	});

Task("RestorePackages")
	.IsDependentOn("Clean")
    .Does(() =>
{
	var settings = new DotNetCoreRestoreSettings
    {
		Sources = new[] {"https://api.nuget.org/v3/index.json", devlanNugetFeed}
    }; 

    DotNetCoreRestore(settings);
});

Task("Build")
	.IsDependentOn("SetVersions")
    .IsDependentOn("RestorePackages")
    .Does(() =>
	{
		var customArgs = string.Format("/p:Version={0};FileVersion={1};InformationalVersion={2}", assemblyVersion, assemblyFileVersion, assemblyInfoVersion);

		var settings = new DotNetCoreBuildSettings
		{
			ArgumentCustomization = args => args.Append(customArgs),
			Configuration = configuration
		};

		DotNetCoreBuild(solutionFile, settings);
	});

Task("RunTests")
    .IsDependentOn("Build")
    .Does(() =>
	{
		var testSettings = new DotNetCoreRunSettings
		{
			Configuration = configuration
		};

		DotNetCoreRun("./test/" + projectName + ".Tests/" + projectName + ".Tests.csproj", "--formatterOptions:file=" + projectName + ".nspec-results.xml", testSettings);
	});

Task("NuGetPack")
	.IsDependentOn("Build")
	.Does(() =>
	{
		var dotNetCorePackSettings   = new DotNetCorePackSettings
		{
			ArgumentCustomization = args => args.Append("/p:Version=" + nugetVersion),
			OutputDirectory = artifactsDir,
			NoBuild = true,
			Configuration = configuration
		};
    
		DotNetCorePack("./src/" + projectName, dotNetCorePackSettings);
	});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("RunTests")
	.IsDependentOn("NuGetPack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);