namespace Ayasuna.Frostseek.Executor;

using System.IO;
using Ayasuna.Frostseek.Data;

/// <summary>
/// Represents the options that are available when creating a new .NET project
/// </summary>
/// <param name="Name">The name of the new project to create</param>
/// <param name="Type">The type of project to create</param>
/// <param name="Target">The (relative) target directory in which the project should be created</param>
/// <param name="Template">The project template to apply when creating the new project</param>
/// <param name="Solution">The solution to create the project for</param>
public record NewProjectCommandOptions(string Name, ProjectType Type, string Target, ProjectTemplate Template, FileInfo Solution) : ICommandOptions;