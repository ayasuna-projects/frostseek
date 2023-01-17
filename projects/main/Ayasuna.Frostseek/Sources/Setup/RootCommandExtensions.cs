namespace Ayasuna.Frostseek.Setup;

using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using Data;
using Executor;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for the <see cref="RootCommand"/> type
/// </summary>
public static class RootCommandExtensions
{
    /// <summary>
    /// Adds the <c>new</c> sub command to the <paramref name="self"/> root command.
    /// </summary>
    /// <param name="self">The root command to add the <c>new</c> sub command to</param>
    /// <param name="serviceProvider">The service provider to use</param>
    /// <returns>The <paramref name="self"/> root command</returns>
    public static RootCommand AddNewSubCommand(this RootCommand self, IServiceProvider serviceProvider)
    {
        self.AddCommand(CreateNewSubCommand(serviceProvider));

        return self;
    }

    /// <summary>
    /// Creates the <c>new</c> sub command
    /// </summary>
    /// <param name="serviceProvider">The service provider to use</param>
    /// <returns>The created <c>new</c> sub command</returns>
    private static Command CreateNewSubCommand(IServiceProvider serviceProvider)
    {
        var newSubCommand = new Command
        (
            "new",
            "Creates a new solution or project"
        );

        newSubCommand.AddCommand(CreateNewSolutionSubCommand(serviceProvider));
        newSubCommand.AddCommand(CreateNewProjectSubCommand(serviceProvider));

        return newSubCommand;
    }

    /// <summary>
    /// Creates the <c>new project</c> sub command
    /// </summary>
    /// <param name="serviceProvider">The service provider to use</param>
    /// <returns>The created <c>new project</c> sub command</returns>
    private static Command CreateNewProjectSubCommand(IServiceProvider serviceProvider)
    {
        var nameOption = new Option<string>
        (
            name: "--name",
            description: "The name of the new project"
        );

        var solutionOption = new Option<FileInfo>
        (
            name: "--solution",
            description: "The solution to add the new project to"
        );

        var projectTypeOption = new Option<ProjectType>
        (
            name: "--type",
            description: "Determines the type of project that should be created",
            getDefaultValue: () => ProjectType.Main
        );

        var targetOption = new Option<string>
        (
            name: "--target",
            description: "The directory in which to create the project (relative to the default project directory of the selected type)",
            getDefaultValue: () => "."
        );

        var projectTemplateOption = new Option<ProjectTemplate>
        (
            name: "--template",
            description: "Determines the template that should be applied when creating the project",
            getDefaultValue: () => ProjectTemplate.Library
        );

        var newProjectSubCommand = new Command
        (
            "project",
            "Creates a new project"
        );

        newProjectSubCommand.AddOption(nameOption);
        newProjectSubCommand.AddOption(solutionOption);
        newProjectSubCommand.AddOption(projectTypeOption);
        newProjectSubCommand.AddOption(targetOption);
        newProjectSubCommand.AddOption(projectTemplateOption);

        var executor = serviceProvider.GetRequiredService<ICommandExecutor<NewProjectCommandOptions>>();

        newProjectSubCommand
            .SetHandler
            (
                (ctx) =>
                {
                    var nameOptionValue = GetValueForOption(ctx.ParseResult, nameOption);
                    var projectTypeOptionValue = GetValueForOption(ctx.ParseResult, projectTypeOption);
                    var projectTargetOptionValue = GetValueForOption(ctx.ParseResult, targetOption);
                    var projectTemplateOptionValue = GetValueForOption(ctx.ParseResult, projectTemplateOption);
                    var solutionOptionValue = GetValueForOption(ctx.ParseResult, solutionOption);

                    return executor.Execute
                    (
                        new NewProjectCommandOptions
                        (
                            nameOptionValue,
                            projectTypeOptionValue,
                            projectTargetOptionValue,
                            projectTemplateOptionValue,
                            Path.IsPathRooted(solutionOptionValue.ToString())
                                ? solutionOptionValue
                                : new FileInfo(Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), solutionOptionValue.ToString())))
                        ),
                        ctx.GetCancellationToken()
                    );
                }
            );

        return newProjectSubCommand;
    }

    /// <summary>
    /// Creates the <c>new solution</c> sub command
    /// </summary>
    /// <param name="serviceProvider">The service provider to use</param>
    /// <returns>The created <c>new solution</c> sub command</returns>
    private static Command CreateNewSolutionSubCommand(IServiceProvider serviceProvider)
    {
        var nameOption = new Option<string>
        (
            name: "--name",
            description: "The name of the new solution"
        );

        var targetOption = new Option<DirectoryInfo>
        (
            name: "--target",
            description: "The directory to create the solution in",
            getDefaultValue: () => new DirectoryInfo(Directory.GetCurrentDirectory())
        );

        var gitOption = new Option<bool>
        (
            name: "--git",
            description: "Determines if a git repository should be initialized",
            getDefaultValue: () => true
        );

        var copyrightLicenseOption = new Option<CopyrightLicense>
        (
            name: "--copyright-license",
            description: "Determines the copyright licenses that should be added to the new solution",
            getDefaultValue: () => CopyrightLicense.None
        );

        var documentationOption = new Option<Documentation>
        (
            name: "--documentation",
            description: "Determines the documentation type that should be initialized for the new solution",
            getDefaultValue: () => Documentation.Docsify
        );

        var newSolutionSubCommand = new Command
        (
            "solution",
            "Creates a new solution"
        );

        newSolutionSubCommand.AddOption(nameOption);
        newSolutionSubCommand.AddOption(targetOption);
        newSolutionSubCommand.AddOption(gitOption);
        newSolutionSubCommand.AddOption(copyrightLicenseOption);
        newSolutionSubCommand.AddOption(documentationOption);

        var executor = serviceProvider.GetRequiredService<ICommandExecutor<NewSolutionCommandOptions>>();

        newSolutionSubCommand
            .SetHandler
            (
                (ctx) =>
                {
                    var nameOptionValue = GetValueForOption(ctx.ParseResult, nameOption);
                    var targetOptionValue = GetValueForOption(ctx.ParseResult, targetOption);
                    var gitOptionValue = GetValueForOption(ctx.ParseResult, gitOption);
                    var copyrightLicenseOptionValue = GetValueForOption(ctx.ParseResult, copyrightLicenseOption);
                    var documentationOptionValue = GetValueForOption(ctx.ParseResult, documentationOption);

                    return executor.Execute
                    (
                        new NewSolutionCommandOptions
                        (
                            nameOptionValue,
                            targetOptionValue,
                            gitOptionValue,
                            copyrightLicenseOptionValue,
                            documentationOptionValue
                        ),
                        ctx.GetCancellationToken()
                    );
                }
            );


        return newSolutionSubCommand;
    }


    /// <summary>
    /// Gets the value for the given <paramref name="option"/> from the given <paramref name="parseResult"/>
    /// </summary>
    /// <param name="parseResult">The parse result</param>
    /// <param name="option">The option to return the value for</param>
    /// <typeparam name="T">The type of the option value</typeparam>
    /// <returns>The option value</returns>
    private static T GetValueForOption<T>(ParseResult parseResult, Option<T> option)
    {
        var result = parseResult.GetValueForOption(option);

        if (result == null)
        {
            throw new ArgumentException(string.Format(Messages.RootCommandExtensions_NoValueHasBeenSpecifiedForRequiredOption, option.Name), nameof(option));
        }

        return result;
    }
}