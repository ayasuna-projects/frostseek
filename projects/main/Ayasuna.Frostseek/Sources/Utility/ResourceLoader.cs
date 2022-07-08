namespace Ayasuna.Frostseek.Utility;

using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Default <see cref="IResourceLoader"/> implementation that loads resources from the embedded resources.
/// </summary>
public sealed class ResourceLoader : IResourceLoader
{
    private const string Dot = ".";

    private const string Slash = "/";

    private const string Backslash = "\\";
    
    private const string TemplateExtension = ".template";

    /// <inheritdoc />
    public Task<string> LoadTemplate(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var manifestResourceName = $"Ayasuna.Frostseek.{path.Replace(Slash, Dot).Replace(Backslash, Dot)}{TemplateExtension}";

        using var resourceStream = assembly.GetManifestResourceStream(manifestResourceName);

        if (resourceStream == null)
        {
            return Task.FromException<string>(new FileNotFoundException($"Could not find resource under the given path {path}", path));
        }

        using var reader = new StreamReader(resourceStream, Encoding.UTF8, true);

        return Task.FromResult(reader.ReadToEnd());
    }
}