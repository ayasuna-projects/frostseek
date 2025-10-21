namespace Ayasuna.Frostseek.Providers.Dotnet;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ayasuna.Frostseek.Utility;
using Microsoft.Extensions.Logging;

/// <summary>
/// <see cref="IDotnetOperationsProvider"/> implementation that uses the <c>dotnet</c> CLI to provide the operations.
/// </summary>
public sealed class DotnetCliUsingDotnetOperationsProvider : IDotnetOperationsProvider
{
    private const string Dotnet = "dotnet";

    private readonly ILogger<DotnetCliUsingDotnetOperationsProvider> _logger;

    /// <summary>
    /// Constructs a new <see cref="DotnetCliUsingDotnetOperationsProvider"/> object
    /// </summary>
    /// <param name="logger">The logger to use</param>
    public DotnetCliUsingDotnetOperationsProvider(ILogger<DotnetCliUsingDotnetOperationsProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task CreateSolution(DirectoryInfo targetDirectory, string name, CancellationToken cancellationToken)
    {
        return ProcessUtils.ExecuteProcess
        (
            targetDirectory,
            Dotnet,
            $"new sln --name {name} --format slnx --output {targetDirectory}",
            msg => _logger.LogInformation("{msg}", msg),
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task AddProjectToSolution(FileInfo solution, FileInfo project, string solutionFolder, CancellationToken cancellationToken)
    {
        return ProcessUtils
            .ExecuteProcess
            (
                solution.Directory!,
                Dotnet,
                $"sln {solution} add {project} --solution-folder {solutionFolder}",
                msg => _logger.LogInformation("{msg}", msg),
                cancellationToken
            );
    }
}