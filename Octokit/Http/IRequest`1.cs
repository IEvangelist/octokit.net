#if NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Text.Json.Serialization.Metadata;

namespace Octokit.Internal;

public interface IRequest<TBody>
{
    TBody Body { get; }
    JsonTypeInfo<TBody> JsonTypeInfo { get; }
    Dictionary<string, string> Headers { get; }
    HttpMethod Method { get; }
    Dictionary<string, string> Parameters { get; }
    Uri BaseAddress { get; }
    Uri Endpoint { get; }
    TimeSpan Timeout { get; }
    string ContentType { get; }
}
#endif
