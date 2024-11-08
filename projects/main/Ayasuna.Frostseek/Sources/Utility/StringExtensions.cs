namespace Ayasuna.Frostseek.Utility;

using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Provides extensions methods for the <see cref="string"/> type
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Searches through the given given <paramref name="self"/> string and replaces all placeholders "<c>${BINDING_NAME}</c>"
    /// for which a binding can be found in the given <paramref name="bindings"/> dictionary with the value of the found binding. <br/>
    /// </summary>
    /// <param name="self">The string to search through</param>
    /// <param name="bindings">The bindings to replace the placeholders with</param>
    /// <returns>A new string in which the placeholders, for which a binding could be found, are replaced with value of the binding</returns>
    public static string ReplacePlaceholders(this string self, IDictionary<string, string> bindings)
    {
        return Regex.Replace
        (
            self,
            "\\${(.*?)}",
            match =>
            {
                if (bindings.TryGetValue(match.Groups[1].Value, out var foundValue))
                {
                    return foundValue;
                }

                return match.Value;
            }
        );
    }

    /// <summary>
    /// Returns a <see cref="string"/> which is a copy of the given <paramref name="self"/> string but with the first letter capitalized. 
    /// </summary>
    /// <param name="self">The string</param>
    /// <returns>The capitalized string</returns>
    public static string Capitalize(this string self)
    {
        return char.ToUpperInvariant(self[0]) + self[1..];
    }
}