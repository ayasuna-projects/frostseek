namespace Ayasuna.Frostseek.Setup;

using System.CommandLine;
using Executor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Providers.Docsify;
using Providers.Dotnet;
using Providers.Git;
using Utility;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the logging services to the service collection <paramref name="self"/>
    /// </summary>
    /// <param name="self">The service collection to add the logging services to</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddLoggingForFrostseek(this IServiceCollection self)
    {
        return self
            .AddLogging(builder => { builder.AddConsole(); });
    }

    /// <summary>
    /// Adds the <see cref="RootCommand"/> to the service collection <paramref name="self"/>
    /// </summary>
    /// <param name="self">The service collection to add the root command to</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddRootCommandForFrostseek(this IServiceCollection self)
    {
        self
            .AddSingleton<IResourceLoader, ResourceLoader>()
            .AddSingleton<IDocsifyOperationsProvider, DocsifyCliUsingDocsifyOperationsProvider>()
            .AddSingleton<IGitOperationsProvider, GitCliUsingGitOperationsProvider>()
            .AddSingleton<IDotnetOperationsProvider, DotnetCliUsingDotnetOperationsProvider>()
            .AddSingleton<ICommandExecutor<NewSolutionCommandOptions>, NewSolutionCommandExecutor>()
            .AddSingleton<ICommandExecutor<NewProjectCommandOptions>, NewProjectCommandExecutor>()
            .AddSingleton
            (
                sp =>
                {
                    var rootCommand = new RootCommand();

                    rootCommand
                        .AddNewSubCommand(sp);

                    return rootCommand;
                }
            );

        return self;
    }
}