namespace Ayasuna.Frostseek.Executor;

using System.IO;
using Data;

/// <summary>
/// Represents the options that are available when creating a new solution
/// </summary>
/// <param name="Name">The name of the new solution to create</param>
/// <param name="Target">The directory to create the new solution in</param>
/// <param name="Git">Determines if a git repository should be initialized</param>
/// <param name="CopyrightLicense">The copyright license that should be added to the new solution</param>
/// <param name="Documentation">The documentation type that should be initialized for the new solution</param>
public sealed record NewSolutionCommandOptions
(
    string Name,
    DirectoryInfo Target,
    bool Git,
    CopyrightLicense CopyrightLicense,
    Documentation Documentation
) : ICommandOptions;