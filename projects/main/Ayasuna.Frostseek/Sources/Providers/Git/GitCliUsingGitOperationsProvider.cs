namespace Ayasuna.Frostseek.Providers.Git;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Utility;

/// <summary>
/// Implementation of <see cref="IGitOperationsProvider"/> which uses the <c>git</c> CLI.
/// </summary>
public sealed class GitCliUsingGitOperationsProvider : IGitOperationsProvider
{
    private const string Git = "git";

    private readonly ILogger<GitCliUsingGitOperationsProvider> _logger;

    /// <summary>
    /// Constructs a new <see cref="GitCliUsingGitOperationsProvider"/> object
    /// </summary>
    /// <param name="logger">The logger to use</param>
    public GitCliUsingGitOperationsProvider(ILogger<GitCliUsingGitOperationsProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task Init(DirectoryInfo directory, CancellationToken cancellationToken)
    {
        return ProcessUtils.ExecuteProcess
        (
            directory,
            Git,
            "init",
            msg => _logger.LogInformation("{msg}", msg),
            cancellationToken
        );
    }

    /// <inheritdoc />
    public Task AddAll(DirectoryInfo directory, CancellationToken cancellationToken)
    {
        return ProcessUtils.ExecuteProcess
        (
            directory,
            Git,
            "add .",
            msg => _logger.LogInformation("{msg}", msg),
            cancellationToken
        );
    }
}