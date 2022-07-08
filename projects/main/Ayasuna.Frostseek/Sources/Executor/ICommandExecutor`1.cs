namespace Ayasuna.Frostseek.Executor;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents the executor, meaning the execution logic, of a command
/// </summary>
/// <typeparam name="T">The type of options this command executor accepts</typeparam>
public interface ICommandExecutor<in T> where T : ICommandOptions
{
    /// <summary>
    /// Executes this command with the given <paramref name="options"/>
    /// </summary>
    /// <param name="options">The options to use when executing this command</param>
    /// <param name="cancellationToken">The cancellation token which can be used to cancel the command execution</param>
    /// <returns>A task which will complete after the command has been executed</returns>
    Task Execute(T options, CancellationToken cancellationToken);
}