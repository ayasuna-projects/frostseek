namespace Ayasuna.Frostseek.Data;

/// <summary>
/// Defines the supported documentation types
/// </summary>
public enum Documentation
{
    /// <summary>
    /// Defines that no documentation should be initialized for the generated solution 
    /// </summary>
    None = 0,

    /// <summary>
    /// Defines that a <a href="https://github.com/docsifyjs/docsify">docsify</a> based documentation should initialized for the generated solution. 
    /// </summary>
    Docsify = 1,
}