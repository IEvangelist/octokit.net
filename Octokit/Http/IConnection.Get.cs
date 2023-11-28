using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit;

public partial interface IConnection
{
#if NET6_0_OR_GREATER
        /// <summary>
        /// Performs an asynchronous HTTP GET request.
        /// Attempts to map the response to an object of type <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="TResult"/> type.</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<TResult>> Get<TResult>(Uri uri, JsonTypeInfo<TResult> jsonTypeInfo, IDictionary<string, string> parameters);
#else
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="parameters">Querystring parameters for the request</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="TResult"/> type.</param>
    /// <param name="parameters">Querystring parameters for the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<TResult>> Get<TResult>(Uri uri, JsonTypeInfo<TResult> jsonTypeInfo, IDictionary<string, string> parameters, string accepts);
#else
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="parameters">Querystring parameters for the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
    Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="TResult"/> type.</param>
    /// <param name="parameters">Querystring parameters for the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="cancellationToken">A token used to cancel the Get request</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<TResult>> Get<TResult>(Uri uri, JsonTypeInfo<TResult> jsonTypeInfo, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken);
#else
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="parameters">Querystring parameters for the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="cancellationToken">A token used to cancel the Get request</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
    Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="TResult"/> type.</param>
    /// <param name="parameters">Querystring parameters for the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="cancellationToken">A token used to cancel the Get request</param>
    /// <param name="preprocessResponseBody">Function to preprocess HTTP response prior to deserialization (can be null)</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<TResult>> Get<TResult>(Uri uri, JsonTypeInfo<TResult> jsonTypeInfo, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody);
#else
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="parameters">Querystring parameters for the request</param>
    /// <param name="accepts">Specifies accepted response media types.</param>
    /// <param name="cancellationToken">A token used to cancel the Get request</param>
    /// <param name="preprocessResponseBody">Function to preprocess HTTP response prior to deserialization (can be null)</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
    Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TResult">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="TResult"/> type.</param>
    /// <param name="timeout">Expiration time of the request</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<TResult>> Get<TResult>(Uri uri, JsonTypeInfo<TResult> jsonTypeInfo, TimeSpan timeout);
#else
    /// <summary>
    /// Performs an asynchronous HTTP GET request.
    /// Attempts to map the response to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type to map the response to</typeparam>
    /// <param name="uri">URI endpoint to send request to</param>
    /// <param name="timeout">Expiration time of the request</param>
    /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
    Task<IApiResponse<T>> Get<T>(Uri uri, TimeSpan timeout);
#endif
}
