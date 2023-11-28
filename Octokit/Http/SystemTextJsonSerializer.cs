#if NET6_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
#else
using System;
#endif

namespace Octokit.Internal;

public class SystemTextJsonSerializer : IJsonSerializer
{
#if NETSTANDARD2_0
    public T Deserialize<T>(string json)
    {
        throw new NotImplementedException(
                "When targeting .NET Standard 2.0, use the SimpleJsonSerializer.");
    }

    public string Serialize(object item)
    {
        throw new NotImplementedException(
            "When targeting .NET Standard 2.0, use the SimpleJsonSerializer.");
    }
#else
    public T Deserialize<T>(string json, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Deserialize<T>(json, jsonTypeInfo);
    }

    public string Serialize<T>(T item, JsonTypeInfo<T> jsonTypeInfo)
    {
        return JsonSerializer.Serialize(item, jsonTypeInfo);
    }
#endif
}
