using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octokit.Caching;
using Octokit.Internal;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit
{
    /// <summary>
    /// A connection for making HTTP requests against URI endpoints.
    /// </summary>
    public partial interface IConnection : IApiInfoProvider
    {
        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing HTML.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<string>> GetHtml(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        /// <remarks>The <see cref="IResponse.Body"/> property will be <c>null</c> if the <paramref name="uri"/> points to a directory instead of a file</remarks>
        Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <param name="timeout">The Timeout value</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        /// <remarks>The <see cref="IResponse.Body"/> property will be <c>null</c> if the <paramref name="uri"/> points to a directory instead of a file</remarks>
        Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters, TimeSpan timeout);

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        /// <remarks>The <see cref="IResponse.Body"/> property will be <c>null</c> if the <paramref name="uri"/> points to a directory instead of a file</remarks>
        Task<IApiResponse<Stream>> GetRawStream(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Base address for the connection.
        /// </summary>
        Uri BaseAddress { get; }

        /// <summary>
        /// Gets the <seealso cref="ICredentialStore"/> used to provide credentials for the connection.
        /// </summary>
        ICredentialStore CredentialStore { get; }

        /// <summary>
        /// Gets or sets the credentials used by the connection.
        /// </summary>
        /// <remarks>
        /// You can use this property if you only have a single hard-coded credential. Otherwise, pass in an
        /// <see cref="ICredentialStore"/> to the constructor.
        /// Setting this property will change the <see cref="ICredentialStore"/> to use
        /// the default <see cref="InMemoryCredentialStore"/> with just these credentials.
        /// </remarks>
        Credentials Credentials { get; set; }

        /// <summary>
        /// Sets response cache used by the connection.
        /// </summary>
        /// <remarks>
        /// Setting this property will wrap existing <see cref="IHttpClient"/> in <see cref="CachingHttpClient"/>.
        /// </remarks>
        IResponseCache ResponseCache { set; }

        /// <summary>
        /// Sets the timeout for the connection between the client and the server.
        /// </summary>
        /// <param name="timeout">The Timeout value</param>
        void SetRequestTimeout(TimeSpan timeout);
    }
}
