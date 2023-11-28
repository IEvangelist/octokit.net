using System;
using System.Net;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit;

public partial interface IConnection
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP PUT request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The body of the request</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Put<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo);
#else
    /// <summary>
    /// Performs an asynchronous HTTP PUT request.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The body of the request</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Put<T>(Uri uri, object body);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP PUT request using the provided two factor authentication code.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
    /// <param name="twoFactorAuthenticationCode">Two factory authentication code to use</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Put<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string twoFactorAuthenticationCode);
#else
    /// <summary>
    /// Performs an asynchronous HTTP PUT request using the provided two factor authentication code.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="twoFactorAuthenticationCode">Two factory authentication code to use</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP PUT request using the provided two factor authentication code.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
    /// <param name="twoFactorAuthenticationCode">Two factory authentication code to use</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Put<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string twoFactorAuthenticationCode, string accepts);
#else
    /// <summary>
    /// Performs an asynchronous HTTP PUT request using the provided two factor authentication code.
    /// Attempts to map the response body to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <param name="twoFactorAuthenticationCode">Two factory authentication code to use</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode, string accepts);
#endif
    /// <summary>
    /// Performs an asynchronous HTTP PUT request that expects an empty response.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Put(Uri uri);

#if NETSTANDARD2_0
    /// <summary>
    /// Performs an asynchronous HTTP PUT request that expects an empty response.
    /// </summary>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="body">The object to serialize as the body of the request</param>
    /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
    Task<HttpStatusCode> Put(Uri uri, object body);
#endif
}
