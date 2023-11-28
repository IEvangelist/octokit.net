using System;
using System.Net;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit;

public partial interface IConnection
{
    /// <summary>
    /// Performs an asynchronous HTTP PATCH request.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<HttpStatusCode> Patch(Uri uri);

    /// <summary>
    /// Performs an asynchronous HTTP PATCH request.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<HttpStatusCode> Patch(Uri uri, string accepts);

#if NET6_0_OR_GREATER
        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name = "jsonTypeInfo" > The JSON type information for the given<typeparamref name="T"/> type.</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Patch<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo);
#else
    /// <summary>
    /// Performs an asynchronous HTTP PATCH request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Patch<T>(Uri uri, object body);
#endif

#if NET6_0_OR_GREATER
        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Patch<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts);
#else
    /// <summary>
    /// Performs an asynchronous HTTP PATCH request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Patch<T>(Uri uri, object body, string accepts);
#endif
}
