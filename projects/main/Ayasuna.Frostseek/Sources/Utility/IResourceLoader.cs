namespace Ayasuna.Frostseek.Utility;

using System.Threading.Tasks;

/// <summary>
/// A resource loader can be used to load resources
/// </summary>
public interface IResourceLoader
{
    /// <summary>
    /// Loads the template which is stored under the given <paramref name="path"/>. <br/>
    /// The given <paramref name="path"/> must not include the <c>.template</c> file extension.
    /// </summary>
    /// <param name="path">The path to the template to load</param>
    /// <returns>A task which will complete with the loaded template</returns>
    Task<string> LoadTemplate(string path);
}