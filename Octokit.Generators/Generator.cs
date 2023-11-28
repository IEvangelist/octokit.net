// Provides an entry point for code generation of various types.
// The backing source for generation will either be the source files in this repo or
// the OpenAPI Descriptions from the GitHub REST API: https://github.com/github/rest-api-description

using Octokit.Generators;

var operation = args.Length > 0 ? args[0] : "AsyncPaginationExtensions";

if (operation is "AsyncPaginationExtensions")
{
    await AsyncPaginationExtensionsGenerator.GenerateAsync();
}

// Put more generation operations here, convert to case when needed.
