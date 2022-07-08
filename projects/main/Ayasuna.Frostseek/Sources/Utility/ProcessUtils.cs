namespace Ayasuna.Frostseek.Utility;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides utility methods for working with processes. 
/// </summary>
public static class ProcessUtils
{
    /// <summary>
    /// Runs the given executable <paramref name="executableName"/> as a (sub) process and waits for it to exit. 
    /// </summary>
    /// <param name="workingDirectory">The working directory that the sub process should use</param>
    /// <param name="executableName">The name of the executable that should be started as a sub process</param>
    /// <param name="arguments">The command line arguments to start the sub process with</param>
    /// <param name="logger">The function to call to output the logs of the (sub) process</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task which will complete after the given sub process has exited</returns>
    public static async Task ExecuteProcess(DirectoryInfo workingDirectory, string executableName, string arguments, Action<string> logger, CancellationToken cancellationToken)
    {
        var processStartInfo = new ProcessStartInfo
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            WorkingDirectory = workingDirectory.FullName,
            RedirectStandardOutput = true,
            FileName = executableName,
            Arguments = arguments
        };

        var process = Process.Start(processStartInfo)!;

        while (!process.StandardOutput.EndOfStream)
        {
            var line = await process.StandardOutput.ReadLineAsync();

            if (line is { Length: > 0 })
            {
                logger($"{executableName}: {line}");
            }
        }

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new Exception($"Sub process '{executableName} {arguments}' completed with non zero exit code, please see the logs for more information");
        }
    }
}