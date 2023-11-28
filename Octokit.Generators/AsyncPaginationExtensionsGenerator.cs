using System.Text.RegularExpressions;
using System.Data;
using System.Text;

namespace Octokit.Generators;

/// <summary>
/// AsyncPaginationExtensionsGenerator for generating pagination extensions for Octokit.net Clients that return collections.
/// </summary>
/// <remarks>
/// This generator originally appeared in https://github.com/octokit/octokit.net/pull/2516
/// The generator solves a small part of a larger effort that is being discussed:
///  https://github.com/octokit/octokit.net/discussions/2499
///  https://github.com/octokit/octokit.net/discussions/2495
///  https://github.com/octokit/octokit.net/issues/2517
///  In the future, we should be able to unify generation for
///   * models (request and response)
///   * clients
///   * routing and related helpers
///  Rosyln source generators were considered, but they will not work for this 
///  as they no longer support reading from the file system. In other words, file
///  APIs are not available in the context of a source generator so we cannot read
///  interfaces to generate extensions for.
/// </remarks>
internal partial class AsyncPaginationExtensionsGenerator
{

    private const string HEADER = """
        /// <summary>
        /// Provides all extensions for pagination.
        /// </summary>
        /// <remarks>
        /// The <code>pageSize</code> parameter at the end of all methods allows for specifying the amount of elements to be fetched per page.
        /// Only useful to optimize the amount of API calls made.
        /// </remarks>
        public static class Extensions
        {
            private const int DEFAULT_PAGE_SIZE = 30;
      
        """;

    private const char FOOTER = '}';

    /// <summary>
    /// GenerateAsync static entry point for generating pagination extensions.
    /// </summary>
    /// <remarks>
    /// This defaults the search path to the root of the project
    /// This expects to generate the resulting code and put it in Octokit.AsyncPaginationExtension
    /// This does a wholesale overwrite on ./Octokit.AsyncPaginationExtension/Extensions.cs
    /// </remarks>
    public static async Task GenerateAsync(string root = "./")
    {
        var builder = new StringBuilder(HEADER);
        var enumOptions = new EnumerationOptions { RecurseSubdirectories = true };
        var paginatedCallRegex = PaginatedCallRegex();

        // Impose deterministic ordering on the files to ensure
        // that the generated code has consistent ordering, thus it's easier to review updates.
        var interfaces = Directory.EnumerateFiles(root, "I*.cs", enumOptions)
            .Select(static file => new FileInfo(file))
            .Where(static fi => fi.Exists)
            .OrderBy(static fi => fi.DirectoryName)
            .ThenBy(static fi => fi.Name)
            .Select(static fi => fi.FullName)
            .ToList();

        Console.WriteLine($"""
            Discovered: {interfaces.Count} interface files.
            """);

        interfaces.ForEach(file =>
        {
            var type = Path.GetFileNameWithoutExtension(file);

            foreach (var line in File.ReadAllLines(file))
            {
                var match = PaginatedCallRegex().Match(line);

                if (match.Success is false)
                {
                    continue;
                }

                Console.WriteLine($"""
                    Writing extension for {type}.{match.Groups["name"].Value}
                    """);

                builder.Append(BuildBodyFromTemplate(match, type));
            }
        });

        builder.AppendLine();
        builder.Append(FOOTER);

        await WriteToFileAsync(builder);
    }

    private static async Task WriteToFileAsync(StringBuilder builder)
    {
        var extensionsFilePath = FindFilePath(
            "Octokit.AsyncPaginationExtension", // Project name
            "Extensions.cs");                          // File name

        if (File.Exists(extensionsFilePath) is false)
        {
            Console.Error.WriteLine(
                $"Could not find {extensionsFilePath}!");

            return;
        }
        else
        {
            Console.WriteLine($"""
                Found the Extensions.cs file at {extensionsFilePath}.
                """);
        }

        await File.WriteAllTextAsync(extensionsFilePath, builder.ToString());

        // Find the correct location of the Extensions.cs file to overwrite.
        static string FindFilePath(string directoryName, string name)
        {
            // Traverse the directory structure until we find the file.
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir is not null)
            {
                var file = Path.Combine(dir.FullName, directoryName, name);
                if (File.Exists(file))
                {
                    return file;
                }

                dir = dir.Parent;
            }

            return Path.Combine(Directory.GetCurrentDirectory(), directoryName, name);
        }
    }

    /// <summary>
    /// BuildBodyFromTemplate uses the match from the regex search and parses values from the given source
    /// to use to generate the paging implementations.
    /// </summary>
    /// <remarks>
    /// TODO: This should be reworked to use source templates
    /// </remarks>
    private static string BuildBodyFromTemplate(Match match, string type)
    {
        var argSplitRegex = ArgSplitRegex();
        var returnType = match.Groups["returnType"].Value;
        var name = match.Groups["name"].Value;
        var arg = match.Groups["arg"].Value;
        var template = match.Groups["template"];
        var templateStr = template.Success ? template.Value : string.Empty;
        var splitArgs = argSplitRegex.Split(arg).ToArray();

        var lambda = arg.Length == 0
            ? $"t.{name}{templateStr}"
            : $"options => t.{name}{templateStr}({string.Join(' ', splitArgs.Where((_, i) => i % 2 == 1))}, options)";

        var docArgs = string.Join(", ", splitArgs.Where((_, i) => i % 2 == 0)).Replace('<', '{').Replace('>', '}');
        if (docArgs.Length != 0)
        {
            docArgs += ", ";
        }

        if (arg.Length != 0)
        {
            arg += ", ";
        }

        return $"""

                /// <inheritdoc cref="{type}.{name}({docArgs}ApiOptions)"/>
                public static IPaginatedList<{returnType}> {name}Async{templateStr}(this {type} t, {arg}int pageSize = DEFAULT_PAGE_SIZE)
                    => pageSize > 0 ? new PaginatedList<{returnType}>({lambda}, pageSize) : throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "The page size must be positive.");

            """;
    }

    [GeneratedRegex(@" (?![^<]*>)")]
    private static partial Regex ArgSplitRegex();

    [GeneratedRegex(@".*Task<IReadOnlyList<(?<returnType>\w+)>>\s*(?<name>\w+)(?<template><.*>)?\((?<arg>.*?)(, )?ApiOptions \w*\);")]
    private static partial Regex PaginatedCallRegex();
}
