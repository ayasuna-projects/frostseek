namespace Ayasuna.Frostseek.Executor;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ayasuna.Frostseek.Data;
using Ayasuna.Frostseek.Providers.Docsify;
using Ayasuna.Frostseek.Providers.Dotnet;
using Ayasuna.Frostseek.Providers.Git;
using Ayasuna.Frostseek.Utility;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public sealed class NewSolutionCommandExecutor : ICommandExecutor<NewSolutionCommandOptions>
{
    private static readonly string ResourcesBasePath = Path.Join("Resources", "Solution");

    private readonly ILogger<NewSolutionCommandExecutor> _logger;

    private readonly IDotnetOperationsProvider _dotnetOperationsProvider;

    private readonly IGitOperationsProvider _gitOperationsProvider;

    private readonly IDocsifyOperationsProvider _docsifyOperationsProvider;

    private readonly IResourceLoader _resourceLoader;

    /// <summary>
    /// Constructs a new <see cref="NewSolutionCommandExecutor"/> object
    /// </summary>
    /// <param name="logger">The logger to use</param>
    /// <param name="dotnetOperationsProvider">The .NET operations provider to use</param>
    /// <param name="gitOperationsProvider">The git operations provider to use</param>
    /// <param name="docsifyOperationsProvider">The docsify operations provider to use</param>
    /// <param name="resourceLoader">The resource loader to use</param>
    public NewSolutionCommandExecutor
    (
        ILogger<NewSolutionCommandExecutor> logger,
        IDotnetOperationsProvider dotnetOperationsProvider,
        IGitOperationsProvider gitOperationsProvider,
        IDocsifyOperationsProvider docsifyOperationsProvider,
        IResourceLoader resourceLoader
    )
    {
        _logger = logger;
        _dotnetOperationsProvider = dotnetOperationsProvider;
        _gitOperationsProvider = gitOperationsProvider;
        _docsifyOperationsProvider = docsifyOperationsProvider;
        _resourceLoader = resourceLoader;
    }

    /// <inheritdoc />
    public async Task Execute(NewSolutionCommandOptions options, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Beginning creation of new solution with name {name}, in directory {target}", options.Name, options.Target);

        var rootDirectory = Directory.CreateDirectory
        (
            Path.Join(options.Target.FullName, options.Name.ToLowerInvariant().Replace(".", "-"))
        );

        if (!await FileSystemUtils.IsDirectoryEmpty(rootDirectory))
        {
            throw new ArgumentException(string.Format(Messages.NewSolutionCommandExecutor_TargetDirectoryIsNotEmpty, rootDirectory));
        }

        var placeholderBindings = new Dictionary<string, string>
        {
            { "YEAR", DateTimeOffset.UtcNow.Year.ToString() },
            { "PROJECT_NAME", options.Name }
        };

        var setupResults = new List<SubDirectorySetupResult>
        {
            await SetupArtifactsDirectory(rootDirectory),
            await SetupBuildDirectory(rootDirectory),
            await SetupDocsDirectory(rootDirectory, options.Documentation, cancellationToken),
            await SetupProjectsDirectory(rootDirectory, placeholderBindings)
        };

        await SetupRootDirectory(rootDirectory, options.Name, options.CopyrightLicense, placeholderBindings, cancellationToken);

        if (options.Git)
        {
            await SetupGit(rootDirectory, setupResults.SelectMany(e => e.LeafDirectories), placeholderBindings, cancellationToken);
        }

        _logger.LogInformation("Successfully created new solution with name {name}, in directory {target}", options.Name, options.Target);
    }


    /// <summary>
    /// Sets up a new git repository in the given <paramref name="rootDirectory"/> and also adds git related files
    /// </summary>
    /// <param name="rootDirectory">The directory to create the git repository in</param>
    /// <param name="leafDirectories">The created leaf directories</param>
    /// <param name="placeholderBindings">The available placeholder bindings</param>
    /// <param name="cancellationToken">The cancellation token to use</param>
    private async Task SetupGit
        (DirectoryInfo rootDirectory, IEnumerable<DirectoryInfo> leafDirectories, IDictionary<string, string> placeholderBindings, CancellationToken cancellationToken)
    {
        foreach (var leafDirectory in leafDirectories)
        {
            await FileSystemUtils.CreateFile(leafDirectory, ".gitkeep", string.Empty);
        }

        await FileSystemUtils.CreateFile
        (
            rootDirectory,
            ".gitignore",
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "Gitignore"))).ReplacePlaceholders(placeholderBindings)
        );

        await _gitOperationsProvider.Init(rootDirectory, cancellationToken);

        await _gitOperationsProvider.AddAll(rootDirectory, cancellationToken);
    }

    /// <summary>
    /// Sets up the root directory
    /// </summary>
    /// <param name="rootDirectory">The root directory to setup</param>
    /// <param name="name">The solution name</param>
    /// <param name="copyrightLicense">The license to add to the directory</param>
    /// <param name="placeholderBindings">The available placeholder bindings</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <exception cref="ArgumentOutOfRangeException">If no mapping for the copyright license exists</exception>
    private async Task SetupRootDirectory
    (
        DirectoryInfo rootDirectory,
        string name,
        CopyrightLicense copyrightLicense,
        IDictionary<string, string> placeholderBindings,
        CancellationToken cancellationToken
    )
    {
        await FileSystemUtils.CreateFile
        (
            rootDirectory,
            ".editorconfig",
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "Editorconfig"))).ReplacePlaceholders(placeholderBindings)
        );
        await FileSystemUtils.CreateFile
        (
            rootDirectory,
            "README.md",
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "README.md"))).ReplacePlaceholders(placeholderBindings)
        );

        switch (copyrightLicense)
        {
            case CopyrightLicense.None:
                // Nothing to do here
                break;
            case CopyrightLicense.MIT:
                await FileSystemUtils.CreateFile
                    (rootDirectory, "LICENSE", (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "Licenses", "MIT"))).ReplacePlaceholders(placeholderBindings));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(copyrightLicense));
        }

        await _dotnetOperationsProvider.CreateSolution(rootDirectory, name, cancellationToken);
    }

    /// <summary>
    /// Sets up the artifacts directory
    /// </summary>
    /// <param name="rootDirectory">The directory to create the artifacts directory in</param>
    private static async Task<SubDirectorySetupResult> SetupArtifactsDirectory(DirectoryInfo rootDirectory)
    {
        var directoryInfo = await FileSystemUtils.CreateDirectory(rootDirectory, "artifacts");

        return new SubDirectorySetupResult(directoryInfo);
    }

    /// <summary>
    /// Sets up the build directory
    /// </summary>
    /// <param name="rootDirectory">The directory to create the build directory in</param>
    private static async Task<SubDirectorySetupResult> SetupBuildDirectory(DirectoryInfo rootDirectory)
    {
        var directoryInfo = await FileSystemUtils.CreateDirectory(rootDirectory, "build");

        return new SubDirectorySetupResult(directoryInfo);
    }

    /// <summary>
    /// Sets up the docs directory
    /// </summary>
    /// <param name="rootDirectory">The directory to create the docs directory in</param>
    /// <param name="documentation">The type of documentation to setup in the directory</param>
    /// <param name="cancellationToken">The cancellation token to use</param>
    private async Task<SubDirectorySetupResult> SetupDocsDirectory(DirectoryInfo rootDirectory, Documentation documentation, CancellationToken cancellationToken)
    {
        if (documentation == Documentation.None)
        {
            _logger.LogDebug("Not setting up any documentation for new solution as the selected documentation type was {type}", Documentation.None);
            return new SubDirectorySetupResult(Array.Empty<DirectoryInfo>());
        }

        var documentationRootDirectory = await FileSystemUtils.CreateDirectory(rootDirectory, "docs");

        if (documentation == Documentation.Docsify)
        {
            await _docsifyOperationsProvider.Init(documentationRootDirectory, cancellationToken);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(documentation));
        }

        return new SubDirectorySetupResult(documentationRootDirectory);
    }

    /// <summary>
    /// Sets up the projects directory
    /// </summary>
    /// <param name="rootDirectory">The directory to create the projects directory in</param>
    /// <param name="placeholderBindings">The available placeholder bindings</param>
    private async Task<SubDirectorySetupResult> SetupProjectsDirectory(DirectoryInfo rootDirectory, IDictionary<string, string> placeholderBindings)
    {
        var projectsDirectory = await FileSystemUtils.CreateDirectory(rootDirectory, "projects");

        const string directoryBuildPropsFileName = "Directory.Build.props";
        const string propertiesDirectory = "Properties";

        await FileSystemUtils.CreateFile
        (
            projectsDirectory,
            directoryBuildPropsFileName,
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, propertiesDirectory, "ProjectsDirectory.Build.props"))).ReplacePlaceholders(placeholderBindings)
        );

        var mainDirectory = await FileSystemUtils.CreateDirectory(projectsDirectory, "main");

        await FileSystemUtils.CreateFile
        (
            mainDirectory,
            directoryBuildPropsFileName,
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, propertiesDirectory, "MainDirectory.Build.props"))).ReplacePlaceholders(placeholderBindings)
        );

        var testDirectory = await FileSystemUtils.CreateDirectory(projectsDirectory, "test");

        await FileSystemUtils.CreateFile
        (
            testDirectory,
            directoryBuildPropsFileName,
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, propertiesDirectory, "TestDirectory.Build.props"))).ReplacePlaceholders(placeholderBindings)
        );

        var metaDirectory = await FileSystemUtils.CreateDirectory(projectsDirectory, "meta");

        await FileSystemUtils.CreateFile
        (
            metaDirectory,
            directoryBuildPropsFileName,
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, propertiesDirectory, "MetaDirectory.Build.props"))).ReplacePlaceholders(placeholderBindings)
        );


        return new SubDirectorySetupResult
        (
            [
                mainDirectory,
                testDirectory
            ]
        );
    }

    /// <summary>
    /// Represents the result of a sub directory setup operation
    /// </summary>
    /// <param name="LeafDirectories">The leaf directories that the sub directory setup operation created</param>
    private sealed record SubDirectorySetupResult(DirectoryInfo[] LeafDirectories)
    {
        /// <summary>
        /// Constructs a new <see cref="SubDirectorySetupResult"/> object
        /// </summary>
        /// <param name="leafDirectory">The leaf directory</param>
        internal SubDirectorySetupResult(DirectoryInfo leafDirectory)
            : this(new[] { leafDirectory })
        {
        }
    }
}