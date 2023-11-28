#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization.Metadata;

namespace Octokit.Internal;

public record class Request<TBody>(TBody Body, JsonTypeInfo<TBody> JsonTypeInfo) : IRequest<TBody>
{
    public Dictionary<string, string> Headers { get; private set; } = [];
    public HttpMethod Method { get; set; }
    public Dictionary<string, string> Parameters { get; private set; } = [];
    public Uri BaseAddress { get; set; }
    public Uri Endpoint { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
    public string ContentType { get; set; }
}
#endif