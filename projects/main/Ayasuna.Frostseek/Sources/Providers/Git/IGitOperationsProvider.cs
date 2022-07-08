namespace Ayasuna.Frostseek.Providers.Git;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <inheritdoc />
/// <summary>
/// Interface for all operation providers that provide <c>git</c> operations. 
/// </summary>
public interface IGitOperationsProvider : IOperationsProvider
{
    /// <summary>
    /// Initializes a new git repository in the given <paramref name="directory"/>
    /// </summary>
    /// <param name="directory">The directory to initialize the git repository in</param>
    /// <param name="cancellationToken">The cancellation token which can be used to cancel the operation</param>
    /// <returns>A task which will complete after the git repository has been initialized</returns>
    Task Init(DirectoryInfo directory, CancellationToken cancellationToken);

    /// <summary>
    /// Adds all files in the given <paramref name="directory"/> to git.  
    /// </summary>
    /// <param name="directory">The directory</param>
    /// <param name="cancellationToken">The cancellation token which can be used to cancel the operation</param>
    /// <returns>A task which will complete after all files have been added to git</returns>
    Task AddAll(DirectoryInfo directory, CancellationToken cancellationToken);
}