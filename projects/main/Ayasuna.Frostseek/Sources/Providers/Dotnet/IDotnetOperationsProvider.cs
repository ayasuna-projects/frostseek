namespace Ayasuna.Frostseek.Providers.Dotnet;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for all operation providers that provide <c>dotnet</c> operations.
/// </summary>
public interface IDotnetOperationsProvider : IOperationsProvider
{
    /// <summary>
    /// Creates a new .NET solution in the given <paramref name="targetDirectory"/> with the given <paramref name="name"/>
    /// </summary>
    /// <param name="targetDirectory">The directory to create the solution in</param>
    /// <param name="name">The name of the solution to create</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task which will complete after the solution has been created</returns>
    Task CreateSolution(DirectoryInfo targetDirectory, string name, CancellationToken cancellationToken);

    /// <summary>
    /// Adds the project, defined by within the given <paramref name="projectFile"/>, under the given <paramref name="solutionFolder"/> to the solution defined with
    /// the given <paramref name="solutionFile"/>.
    /// </summary>
    /// <param name="solutionFile">The solution file to add the project to</param>
    /// <param name="projectFile">The project file</param>
    /// <param name="solutionFolder">The solution folder</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task which will complete after the project has been added to the solution</returns>
    Task AddProjectToSolution(FileInfo solutionFile, FileInfo projectFile, string solutionFolder, CancellationToken cancellationToken);
}