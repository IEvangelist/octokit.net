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
    /// Performs an asynchronous HTTP DELETE request that expects an empty response.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Delete(Uri uri);

    /// <summary>
    /// Performs an asynchronous HTTP DELETE request that expects an empty response.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="twoFactorAuthenticationCode">Two Factor Code</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Delete(Uri uri, string twoFactorAuthenticationCode);

#if NETSTANDARD2_0
    /// <summary>
    /// Performs an asynchronous HTTP DELETE request that expects an empty response.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="data">The object to serialize as the body of the request</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Delete(Uri uri, object data);

    /// <summary>
    /// Performs an asynchronous HTTP DELETE request that expects an empty response.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="data">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accept response media type</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Delete(Uri uri, object data, string accepts);
#endif

#if NET6_0_OR_GREATER
        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
        Task<IApiResponse<T>> Delete<T>(Uri uri, T data, JsonTypeInfo<T> jsonTypeInfo);
#else
    /// <summary>
    /// Performs an asynchronous HTTP DELETE request.
    /// </summary>
    /// <typeparam name="T">The API resource's type.</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="data">The object to serialize as the body of the request</param>
    Task<IApiResponse<T>> Delete<T>(Uri uri, object data);
#endif

#if NET6_0_OR_GREATER
        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
        /// <param name="accepts">Specifies accept response media type</param>
        Task<IApiResponse<T>> Delete<T>(Uri uri, T data, JsonTypeInfo<T> jsonTypeInfo, string accepts);
#else
    /// <summary>
    /// Performs an asynchronous HTTP DELETE request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="data">The object to serialize as the body of the request</param>
    /// <param name="accepts">Specifies accept response media type</param>
    Task<IApiResponse<T>> Delete<T>(Uri uri, object data, string accepts);
#endif
}
