#addin "Cake.Git"
#addin "Cake.FileHelpers"
#addin "nuget:http://nuget.oss-concept.ch/nuget/?package=Opten.Cake"

var target = Argument("target", "Default");

string feedUrl = "https://www.nuget.org/api/v2/package";
string version = null;

var dest = Directory("./artifacts");

// Cleanup

Task("Clean")
	.Does(() =>
{
	if (DirectoryExists(dest))
	{
		CleanDirectory(dest);
		DeleteDirectory(dest, recursive: true);
	}
});

// Versioning

Task("Version")
	.IsDependentOn("Clean") 
	.Does(() =>
{
	if (DirectoryExists(dest) == false)
	{
		CreateDirectory(dest);
	}

	version = GitDescribe("../", false, GitDescribeStrategy.Tags, 0);

	PatchAssemblyInfo("../src/Opten.Web.Http/Properties/AssemblyInfo.cs", version);
	FileWriteText(dest + File("Opten.Web.Http.variables.txt"), "version=" + version);
});

// Building

Task("Restore-NuGet-Packages")
	.IsDependentOn("Version") 
	.Does(() =>
{ 
	NuGetRestore("../Opten.Web.Http.sln", new NuGetRestoreSettings {
		NoCache = true
	});
});

// Building

Task("Build") 
	.IsDependentOn("Restore-NuGet-Packages") 
	.Does(() =>
{
	MSBuild("../src/Opten.Web.Http/Opten.Web.Http.csproj", settings =>
		settings.SetConfiguration("Debug"));

	MSBuild("../src/Opten.Web.Http/Opten.Web.Http.csproj", settings =>
		settings.SetConfiguration("Release"));
});

Task("Pack")
	.IsDependentOn("Build")
	.Does(() =>
{
	NuGetPackWithDependencies("./Opten.Web.Http.nuspec", new NuGetPackSettings {
		Version = version,
		BasePath = "../",
		OutputDirectory = dest
	}, feedUrl);
});


// Deploying

Task("Deploy")
	.Does(() =>
{
	// This is from the Bamboo's Script Environment variables
	string packageId = "Opten.Web.Http";

	// Get the Version from the .txt file
	string version = EnvironmentVariable("bamboo_inject_" + packageId.Replace(".", "_") + "_version");

	if(string.IsNullOrWhiteSpace(version))
	{
		throw new Exception("Version is missing for " + packageId + ".");
	}

	// Get the path to the package
	var package = File(packageId + "." + version + ".nupkg");
			
	// Push the package
	NuGetPush(package, new NuGetPushSettings {
		Source = feedUrl,
		ApiKey = EnvironmentVariable("NUGET_API_KEY")
	});

	// Notifications
	Slack(new SlackSettings {
		ProjectName = packageId
	});
});

Task("Default")
	.IsDependentOn("Pack");

RunTarget(target);