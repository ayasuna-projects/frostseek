namespace Ayasuna.Frostseek.Providers.Docsify;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Utility;

/// <summary>
/// Implementation of <see cref="IDocsifyOperationsProvider"/> that uses the <c>docsify</c> CLI.
/// </summary>
public sealed class DocsifyCliUsingDocsifyOperationsProvider : IDocsifyOperationsProvider
{
    private readonly ILogger<DocsifyCliUsingDocsifyOperationsProvider> _logger;

    /// <summary>
    /// Constructs a new <see cref="DocsifyCliUsingDocsifyOperationsProvider"/> object
    /// </summary>
    /// <param name="logger">The logger to use</param>
    public DocsifyCliUsingDocsifyOperationsProvider(ILogger<DocsifyCliUsingDocsifyOperationsProvider> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Init(DirectoryInfo directory, CancellationToken cancellationToken)
    {
        if (!await FileSystemUtils.IsDirectoryEmpty(directory))
        {
            throw new ArgumentException(Messages.DocsifyCliUsingDocsifyOperationsProvider_TargetDirectoryIsNotEmpty, nameof(directory));
        }

        var parentDirectory = directory.Parent!;

        // TODO: This acts as a workaround for now as docsify expects the document directory to not exist by default. 
        directory.Delete();

        await ProcessUtils.ExecuteProcess
        (
            parentDirectory,
            "docsify",
            $"init {directory}",
            msg => _logger.LogInformation("{msg}", msg),
            cancellationToken
        );
    }
}