namespace Ayasuna.Frostseek.Utility;

using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Provides utility methods for working with the file system. 
/// </summary>
public static class FileSystemUtils
{
    /// <summary>
    /// Creates a new file with the given name <paramref name="fileName"/> in the given <paramref name="directory"/>
    /// </summary>
    /// <param name="directory">The directory to create the file in</param>
    /// <param name="fileName">The name of the file to create</param>
    /// <param name="fileContent">The content of the file to create</param>
    public static async Task CreateFile(DirectoryInfo directory, string fileName, string fileContent)
    {
        await using var editorConfigStreamWriter = File.CreateText(Path.Join(directory.ToString(), fileName));

        await editorConfigStreamWriter.WriteAsync(fileContent);
    }

    /// <summary>
    /// Creates a new directory with the name <paramref name="directoryName"/> within the given <paramref name="baseDirectory"/>.
    /// If the directory already exists nothing happens.
    /// </summary>
    /// <param name="baseDirectory">The directory to create the new directory in</param>
    /// <param name="directoryName">The name of the directory to create</param>
    /// <returns>The directory info of the new directory</returns>
    public static Task<DirectoryInfo> CreateDirectory(DirectoryInfo baseDirectory, string directoryName)
    {
        return Task.FromResult(Directory.CreateDirectory(Path.Join(baseDirectory.FullName, directoryName)));
    }

    /// <summary>
    /// Checks whether the given <paramref name="directory"/> is empty
    /// </summary>
    /// <param name="directory">The directory to check</param>
    /// <returns>A task which will complete with <c>true</c> if the directory is empty or with <c>false</c> otherwise</returns>
    public static Task<bool> IsDirectoryEmpty(DirectoryInfo directory)
    {
        return Task.FromResult(directory.GetFileSystemInfos().Length == 0);
    }
}