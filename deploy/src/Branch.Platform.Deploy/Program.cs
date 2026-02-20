using System.Diagnostics;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run --project deploy/src/Branch.Platform.Deploy -- <build|push|deploy|all>");
    return 1;
}

var command = args[0].ToLowerInvariant();
var registry = Environment.GetEnvironmentVariable("BRANCH_ACR_SERVER") ?? "";
var imageName = Environment.GetEnvironmentVariable("BRANCH_IMAGE_NAME") ?? "branchplatformapi";
var imageTag = Environment.GetEnvironmentVariable("BRANCH_IMAGE_TAG") ?? "latest";
var stack = Environment.GetEnvironmentVariable("BRANCH_PULUMI_STACK") ?? "dev";

var fullImage = string.IsNullOrWhiteSpace(registry)
    ? $"{imageName}:{imageTag}"
    : $"{registry}/{imageName}:{imageTag}";

try
{
    switch (command)
    {
        case "build":
            await Run("docker", $"build -f src/Branch.Platform.Api/Dockerfile -t {fullImage} .");
            break;
        case "push":
            await Run("docker", $"push {fullImage}");
            break;
        case "deploy":
            await Run("pulumi", $"up --cwd infra -s {stack} --yes --config branch-platform-infra:containerImage={imageName}:{imageTag}");
            break;
        case "all":
            await Run("docker", $"build -f src/Branch.Platform.Api/Dockerfile -t {fullImage} .");
            await Run("docker", $"push {fullImage}");
            await Run("pulumi", $"up --cwd infra -s {stack} --yes --config branch-platform-infra:containerImage={imageName}:{imageTag}");
            break;
        default:
            Console.WriteLine($"Unknown command: {command}");
            return 2;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    return 3;
}

return 0;

static async Task Run(string fileName, string arguments)
{
    Console.WriteLine($"> {fileName} {arguments}");
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        }
    };

    process.OutputDataReceived += (_, e) => { if (!string.IsNullOrWhiteSpace(e.Data)) Console.WriteLine(e.Data); };
    process.ErrorDataReceived += (_, e) => { if (!string.IsNullOrWhiteSpace(e.Data)) Console.Error.WriteLine(e.Data); };

    if (!process.Start()) throw new InvalidOperationException($"Failed to start process {fileName}");

    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    await process.WaitForExitAsync();

    if (process.ExitCode != 0)
        throw new InvalidOperationException($"Command failed ({process.ExitCode}): {fileName} {arguments}");
}
