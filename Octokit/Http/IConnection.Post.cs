using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit;

public partial interface IConnection
{
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Post(Uri uri, CancellationToken cancellationToken = default);

#if NETSTANDARD2_0
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Post(Uri uri, object body, string accepts, CancellationToken cancellationToken = default);
#endif

    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<TResult>> Post<TResult>(Uri uri, CancellationToken cancellationToken = default);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="parameters">Extra parameters for authentication.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(
        Uri uri,
        T body,
        JsonTypeInfo<T> jsonTypeInfo,
        string accepts,
        string contentType,
        IDictionary<string, string> parameters = null,
        CancellationToken cancellationToken = default);
#else
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="parameters">Extra parameters for authentication.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(
        Uri uri,
        object body,
        string accepts,
        string contentType,
        IDictionary<string, string> parameters = null,
        CancellationToken cancellationToken = default);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="twoFactorAuthenticationCode">Two Factor Authentication Code</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts, string contentType, string twoFactorAuthenticationCode, CancellationToken cancellationToken = default);
#else
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="twoFactorAuthenticationCode">Two Factor Authentication Code</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, string twoFactorAuthenticationCode, CancellationToken cancellationToken = default);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts, string contentType, TimeSpan timeout, CancellationToken cancellationToken = default);
#else
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, TimeSpan timeout, CancellationToken cancellationToken = default);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <remarks>
    /// We have one case where we need to override the BaseAddress. This overload is for that case.
    /// https://developer.github.com/v3/oauth/#web-application-flow
    /// </remarks>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="baseAddress">Allows overriding the base address for a post.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts, string contentType, Uri baseAddress, CancellationToken cancellationToken = default);
#else
    /// <summary>
    /// Performs an asynchronous HTTP POST request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <remarks>
    /// We have one case where we need to override the BaseAddress. This overload is for that case.
    /// https://developer.github.com/v3/oauth/#web-application-flow
    /// </remarks>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="contentType">Specifies the media type of the request body</param>
    /// <param name="baseAddress">Allows overriding the base address for a post.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, Uri baseAddress, CancellationToken cancellationToken = default);
#endif
}
