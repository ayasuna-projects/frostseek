namespace Ayasuna.Frostseek.Providers.Docsify;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for all operation providers that provide <a href="https://docsify.js.org">docsify</a> operations
/// </summary>
public interface IDocsifyOperationsProvider : IOperationsProvider
{
    /// <summary>
    /// Initializes a new docsify based documentation in the given <paramref name="directory"/>
    /// </summary>
    /// <param name="directory">The directory to initialize the documentation in</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task which will complete after the documentation has been setup</returns>
    Task Init(DirectoryInfo directory, CancellationToken cancellationToken);
}