namespace Ayasuna.Frostseek.Executor;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Logging;
using Providers.Dotnet;
using Utility;

/// <inheritdoc />
public sealed class NewProjectCommandExecutor : ICommandExecutor<NewProjectCommandOptions>
{
    private static readonly string ResourcesBasePath = Path.Join("Resources", "Project");

    private readonly ILogger<NewProjectCommandExecutor> _logger;

    private readonly IDotnetOperationsProvider _dotnetOperationsProvider;

    private readonly IResourceLoader _resourceLoader;

    /// <summary>
    /// Constructs a new <see cref="NewProjectCommandExecutor"/> object
    /// </summary>
    /// <param name="logger">The logger to use</param>
    /// <param name="dotnetOperationsProvider">The .NET operations provider to use</param>
    /// <param name="resourceLoader">The resource loader to use</param>
    public NewProjectCommandExecutor(ILogger<NewProjectCommandExecutor> logger, IDotnetOperationsProvider dotnetOperationsProvider, IResourceLoader resourceLoader)
    {
        _logger = logger;
        _dotnetOperationsProvider = dotnetOperationsProvider;
        _resourceLoader = resourceLoader;
    }

    /// <inheritdoc />
    public async Task Execute(NewProjectCommandOptions options, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Beginning to create new project with name {name} for solution {solution}", options.Name, options.Solution.Name);

        var workingDirectory = options.Solution.Directory!;


        var type = options.Type switch
        {
            ProjectType.Main => "main",
            ProjectType.Test => "test",
            ProjectType.Meta => "meta",
            _ => throw new ArgumentOutOfRangeException()
        };

        var targetParts = options.Target.Split
            (
                [
                    '/',
                    '\\'
                ],
                StringSplitOptions.RemoveEmptyEntries
            )
            .Where(e => e != "." && e != "..")
            .ToList();

        var currentTargetDirectory = Path.Join(workingDirectory.FullName, "projects", type);
        foreach (var targetPart in targetParts)
        {
            currentTargetDirectory = Path.Join(currentTargetDirectory, targetPart);

            if (!Directory.Exists(currentTargetDirectory))
            {
                var directoryInfo = Directory.CreateDirectory(currentTargetDirectory);
                await FileSystemUtils.CreateFile
                    (directoryInfo, "Directory.Build.props", await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "Properties", "TargetDirectory.Build.props")));
            }
        }

        var targetDirectory = new DirectoryInfo(Path.Join(workingDirectory.FullName, "projects", targetParts.Aggregate(type, Path.Join), options.Name));


        var placeholders = new Dictionary<string, string>
        {
            { "PROJECT_NAME", options.Name },
            { "PROJECT_TYPE", type },
            { "OUTPUT_TYPE", options.Template == ProjectTemplate.Application && options.Type == ProjectType.Main ? "Exe" : "Library" },
            { "TARGET_FRAMEWORKS", "$(FrostseekDefaultTargetFrameworks)" },
        };

        var sourcesDirectory = await FileSystemUtils.CreateDirectory(targetDirectory, "Sources");
        await FileSystemUtils.CreateDirectory(targetDirectory, "Resources");

        var projectFileName = $"{options.Name}.csproj";

        await FileSystemUtils.CreateFile
        (
            targetDirectory,
            projectFileName,
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "Project.csproj"))).ReplacePlaceholders(placeholders)
        );
        await FileSystemUtils.CreateFile
        (
            targetDirectory,
            $"{projectFileName}.DotSettings",
            (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "Project.csproj.DotSettings"))).ReplacePlaceholders(placeholders)
        );

        if (options is { Template: ProjectTemplate.Application, Type: ProjectType.Main })
        {
            await FileSystemUtils.CreateFile
            (
                sourcesDirectory,
                "Program.cs",
                (await _resourceLoader.LoadTemplate(Path.Join(ResourcesBasePath, "Program"))).ReplacePlaceholders(placeholders)
            );
        }

        await _dotnetOperationsProvider.AddProjectToSolution
        (
            options.Solution,
            new FileInfo(Path.Join(targetDirectory.FullName, projectFileName)),
            targetParts.Aggregate(type.Capitalize(), (a, b) => Path.Join(a, b.Capitalize())),
            cancellationToken
        );

        _logger.LogInformation("Successfully created project with name {name} and associated it with solution {solution}", options.Name, options.Solution.Name);
    }
}