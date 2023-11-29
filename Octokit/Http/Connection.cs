using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Octokit.Caching;
using Octokit.Internal;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit
{
    // NOTE: Every request method must go through the `RunRequest` code path. So if you need to add a new method
    // ensure it goes through there. :)
    /// <summary>
    /// A connection for making HTTP requests against URI endpoints.
    /// </summary>
    public class Connection : IConnection
    {
        static readonly Uri _defaultGitHubApiUrl = GitHubClient.GitHubApiUrl;
        static readonly ICredentialStore _anonymousCredentials = new InMemoryCredentialStore(Credentials.Anonymous);

        readonly Authenticator _authenticator;
        readonly JsonHttpPipeline _jsonPipeline;
        internal IHttpClient _httpClient;

        /// <summary>
        /// Creates a new connection instance used to make requests of the GitHub API.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="productInformation">
        /// The name (and optionally version) of the product using this library, the name of your GitHub organization, or your GitHub username (in that order of preference). This is sent to the server as part of
        /// the user agent for analytics purposes, and used by GitHub to contact you if there are problems.
        /// </param>
        public Connection(ProductHeaderValue productInformation)
            : this(productInformation, _defaultGitHubApiUrl, _anonymousCredentials)
        {
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the GitHub API.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="productInformation">
        /// The name (and optionally version) of the product using this library, the name of your GitHub organization, or your GitHub username (in that order of preference). This is sent to the server as part of
        /// the user agent for analytics purposes, and used by GitHub to contact you if there are problems.
        /// </param>
        /// <param name="httpClient">
        /// The client to use for executing requests
        /// </param>
        public Connection(ProductHeaderValue productInformation, IHttpClient httpClient)
            : this(productInformation,
                  _defaultGitHubApiUrl,
                  _anonymousCredentials,
                  httpClient,
#if NET6_0_OR_GREATER
                  new SystemTextJsonSerializer()
#else
                  new SimpleJsonSerializer()
#endif
                  )
        {
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the GitHub API.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="productInformation">
        /// The name (and optionally version) of the product using this library, the name of your GitHub organization, or your GitHub username (in that order of preference). This is sent to the server as part of
        /// the user agent for analytics purposes, and used by GitHub to contact you if there are problems.
        /// </param>
        /// <param name="baseAddress">
        /// The address to point this client to such as https://api.github.com or the URL to a GitHub Enterprise
        /// instance</param>
        public Connection(ProductHeaderValue productInformation, Uri baseAddress)
            : this(productInformation, baseAddress, _anonymousCredentials)
        {
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the GitHub API.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="productInformation">
        /// The name (and optionally version) of the product using this library, the name of your GitHub organization, or your GitHub username (in that order of preference). This is sent to the server as part of
        /// the user agent for analytics purposes, and used by GitHub to contact you if there are problems.
        /// </param>
        /// <param name="credentialStore">Provides credentials to the client when making requests</param>
        public Connection(ProductHeaderValue productInformation, ICredentialStore credentialStore)
            : this(productInformation, _defaultGitHubApiUrl, credentialStore)
        {
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the GitHub API.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="productInformation">
        /// The name (and optionally version) of the product using this library, the name of your GitHub organization, or your GitHub username (in that order of preference). This is sent to the server as part of
        /// the user agent for analytics purposes, and used by GitHub to contact you if there are problems.
        /// </param>
        /// <param name="baseAddress">
        /// The address to point this client to such as https://api.github.com or the URL to a GitHub Enterprise
        /// instance</param>
        /// <param name="credentialStore">Provides credentials to the client when making requests</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Connection(ProductHeaderValue productInformation, Uri baseAddress, ICredentialStore credentialStore)
            : this(productInformation,
                  baseAddress,
                  credentialStore,
                  new HttpClientAdapter(HttpMessageHandlerFactory.CreateDefault),
#if NET6_0_OR_GREATER
                  new SystemTextJsonSerializer()
#else
                  new SimpleJsonSerializer()
#endif
                  )
        {
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the GitHub API.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="productInformation">
        /// The name (and optionally version) of the product using this library, the name of your GitHub organization, or your GitHub username (in that order of preference). This is sent to the server as part of
        /// the user agent for analytics purposes, and used by GitHub to contact you if there are problems.
        /// </param>
        /// <param name="baseAddress">
        /// The address to point this client to such as https://api.github.com or the URL to a GitHub Enterprise
        /// instance</param>
        /// <param name="credentialStore">Provides credentials to the client when making requests</param>
        /// <param name="httpClient">A raw <see cref="IHttpClient"/> used to make requests</param>
        /// <param name="serializer">Class used to serialize and deserialize JSON requests</param>
        public Connection(
            ProductHeaderValue productInformation,
            Uri baseAddress,
            ICredentialStore credentialStore,
            IHttpClient httpClient,
            IJsonSerializer serializer)
        {
            Ensure.ArgumentNotNull(productInformation, nameof(productInformation));
            Ensure.ArgumentNotNull(baseAddress, nameof(baseAddress));
            Ensure.ArgumentNotNull(credentialStore, nameof(credentialStore));
            Ensure.ArgumentNotNull(httpClient, nameof(httpClient));
            Ensure.ArgumentNotNull(serializer, nameof(serializer));

            if (!baseAddress.IsAbsoluteUri)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The base address '{0}' must be an absolute URI",
                        baseAddress), nameof(baseAddress));
            }

            UserAgent = FormatUserAgent(productInformation);
            BaseAddress = baseAddress;
            _authenticator = new Authenticator(credentialStore);
            _httpClient = httpClient;
            _jsonPipeline = new JsonHttpPipeline(serializer);
        }

        /// <summary>
        /// Gets the latest API Info - this will be null if no API calls have been made
        /// </summary>
        /// <returns><seealso cref="ApiInfo"/> representing the information returned as part of an Api call</returns>
        public ApiInfo GetLastApiInfo()
        {
            // We've chosen to not wrap the _lastApiInfo in a lock.  Originally the code was returning a reference - so there was a danger of
            // on thread writing to the object while another was reading.  Now we are cloning the ApiInfo on request - thus removing the need (or overhead)
            // of putting locks in place.
            // See https://github.com/octokit/octokit.net/pull/855#discussion_r36774884
            return _lastApiInfo?.Clone();
        }

        private ApiInfo _lastApiInfo;

#if NET6_0_OR_GREATER
        public Task<IApiResponse<T>> Get<T>(Uri uri, JsonTypeInfo<T> jsonTypeInfo, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(jsonTypeInfo, nameof(jsonTypeInfo));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, jsonTypeInfo, null, CancellationToken.None);
        }

        public Task<IApiResponse<T>> Get<T>(Uri uri, JsonTypeInfo<T> jsonTypeInfo, IDictionary<string, string> parameters, string accepts)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Get<T>(Uri uri, JsonTypeInfo<T> jsonTypeInfo, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Get<T>(Uri uri, JsonTypeInfo<T> jsonTypeInfo, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Get<T>(Uri uri, JsonTypeInfo<T> jsonTypeInfo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
#else
        /// <inheritdoc cref="IConnection.Get{T}(Uri, IDictionary{string, string})" />
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, null, null, CancellationToken.None);
        }

        /// <inheritdoc cref="IConnection.Get{T}(Uri, IDictionary{string, string}, string)" />
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null, CancellationToken.None);
        }

        /// <inheritdoc cref="IConnection.Get{T}(Uri, IDictionary{string, string}, string, CancellationToken)" />
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null, cancellationToken);
        }

        /// <inheritdoc cref="IConnection.Get{T}(Uri, IDictionary{string, string}, string, CancellationToken, Func{object, object})" />
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null, cancellationToken, null, null, preprocessResponseBody);
        }

        /// <inheritdoc cref="IConnection.Get{T}(Uri, TimeSpan)" />
        public Task<IApiResponse<T>> Get<T>(Uri uri, TimeSpan timeout)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri, HttpMethod.Get, null, null, null, timeout, CancellationToken.None);
        }
#endif

        /// <inheritdoc cref="IConnection.GetHtml(Uri, IDictionary{string, string})" />
        public Task<IApiResponse<string>> GetHtml(Uri uri, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return GetHtml(new Request
            {
                Method = HttpMethod.Get,
                BaseAddress = BaseAddress,
                Endpoint = uri.ApplyParameters(parameters)
            });
        }

        /// <inheritdoc cref="IConnection.GetRaw(Uri, IDictionary{string, string})" />
        public Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return GetRaw(new Request
            {
                Method = HttpMethod.Get,
                BaseAddress = BaseAddress,
                Endpoint = uri.ApplyParameters(parameters)
            });
        }

        /// <inheritdoc cref="IConnection.GetRaw(Uri, IDictionary{string, string}, TimeSpan)" />
        public Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters, TimeSpan timeout)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return GetRaw(new Request
            {
                Method = HttpMethod.Get,
                BaseAddress = BaseAddress,
                Endpoint = uri.ApplyParameters(parameters),
                Timeout = timeout
            });
        }

        /// <inheritdoc cref="IConnection.GetRawStream(Uri, IDictionary{string, string})" />
        public Task<IApiResponse<Stream>> GetRawStream(Uri uri, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return GetRawStream(new Request
            {
                Method = HttpMethod.Get,
                BaseAddress = BaseAddress,
                Endpoint = uri.ApplyParameters(parameters)
            });
        }

#if NET6_0_OR_GREATER
        /// <inheritdoc cref="IConnection.Patch{T}(Uri, object)" />
        public Task<IApiResponse<T>> Patch<TBody, TResult>(Uri uri, T body)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpVerb.Patch, body, null, null, CancellationToken.None);
        }

        /// <inheritdoc cref="IConnection.Patch(Uri, object, string)" />
        public Task<IApiResponse<T>> Patch<T>(Uri uri, object body, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            return SendData<T>(uri, HttpVerb.Patch, body, accepts, null, CancellationToken.None);
        }
#else
        /// <inheritdoc cref="IConnection.Patch(Uri, object)" />
        public Task<IApiResponse<T>> Patch<T>(Uri uri, object body)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpVerb.Patch, body, null, null, CancellationToken.None);
        }

        /// <inheritdoc cref="IConnection.Patch(Uri, object, string)" />
        public Task<IApiResponse<T>> Patch<T>(Uri uri, object body, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            return SendData<T>(uri, HttpVerb.Patch, body, accepts, null, CancellationToken.None);
        }
#endif

        /// <inheritdoc cref="IConnection.Post(Uri, CancellationToken)" />
        public async Task<HttpStatusCode> Post(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Post, null, null, null, cancellationToken).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

#if NETSTANDARD2_0
        /// <inheritdoc cref="IConnection.Post(Uri, object, string, CancellationToken)" />
        public async Task<HttpStatusCode> Post(Uri uri, object body, string accepts, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Post, body, accepts, null, cancellationToken).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }
#endif

        /// <inheritdoc cref="IConnection.Post{T}(Uri, CancellationToken)" />
        public Task<IApiResponse<T>> Post<T>(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri, HttpMethod.Post, null, null, null, cancellationToken);
        }

        /// <inheritdoc cref="IConnection.Post{T}(Uri, object, string, string, string, CancellationToken)" />
        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, cancellationToken);
        }

        /// <inheritdoc cref="IConnection.Post{T}(Uri, object, string, string, IDictionary{string, string}, CancellationToken)" />
        public Task<IApiResponse<T>> Post<T>(
            Uri uri,
            object body,
            string accepts,
            string contentType,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Post, body, accepts, contentType, cancellationToken);
        }

        /// <inheritdoc cref="IConnection.Post{T}(Uri, object, string, string, string, CancellationToken)" />
        public Task<IApiResponse<T>> Post<T>(
            Uri uri,
            object body,
            string accepts,
            string contentType,
            string twoFactorAuthenticationCode,
            CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));
            Ensure.ArgumentNotNullOrEmptyString(twoFactorAuthenticationCode, nameof(twoFactorAuthenticationCode));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, cancellationToken, twoFactorAuthenticationCode);
        }

        /// <inheritdoc cref="IConnection.Post{T}(Uri, object, string, string, TimeSpan, CancellationToken)" />
        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, timeout, cancellationToken);
        }

        /// <inheritdoc cref="IConnection.Post{T}(Uri, object, string, string, Uri, CancellationToken)" />
        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, Uri baseAddress, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, cancellationToken, baseAddress: baseAddress);
        }

        /// <inheritdoc cref="IConnection.Put(Uri, object)" />
        public Task<IApiResponse<T>> Put<T>(Uri uri, object body)
        {
            return SendData<T>(uri, HttpMethod.Put, body, null, null, CancellationToken.None);
        }

        /// <inheritdoc cref="IConnection.Put(Uri, object, string)" />
        public Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode)
        {
            return SendData<T>(uri,
                HttpMethod.Put,
                body,
                null,
                null,
                CancellationToken.None,
                twoFactorAuthenticationCode);
        }

        /// <inheritdoc cref="IConnection.Put(Uri, object, string, string)" />
        public Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode, string accepts)
        {
            return SendData<T>(uri,
                HttpMethod.Put,
                body,
                accepts,
                null,
                CancellationToken.None,
                twoFactorAuthenticationCode);
        }

#if NET6_0_OR_GREATER
        Task<IApiResponse<TResult>> SendData<TBody, TResult>(
            Uri uri,
            HttpMethod method,
            TBody body,
            JsonTypeInfo<TResult> jsonTypeInfo,
            string accepts,
            string contentType,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            string twoFactorAuthenticationCode = null,
            Uri baseAddress = null,
            Func<object, object> preprocessResponseBody = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.GreaterThanZero(timeout, nameof(timeout));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri,
                Timeout = timeout
            };

            return SendDataInternal<T>(body, jsonTypeInfo, accepts, contentType, cancellationToken, twoFactorAuthenticationCode, request, preprocessResponseBody);
        }
#else
        Task<IApiResponse<T>> SendData<T>(
            Uri uri,
            HttpMethod method,
            object body,
            string accepts,
            string contentType,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            string twoFactorAuthenticationCode = null,
            Uri baseAddress = null,
            Func<object, object> preprocessResponseBody = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.GreaterThanZero(timeout, nameof(timeout));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri,
                Timeout = timeout
            };

            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, twoFactorAuthenticationCode, request, preprocessResponseBody);
        }
#endif

#if NET6_0_OR_GREATER
        Task<IApiResponse<TResult>> SendData<TBody, TResult>(
            Uri uri,
            HttpMethod method,
            T body,
            JsonTypeInfo<TResult> jsonTypeInfo,
            string accepts,
            string contentType,
            CancellationToken cancellationToken,
            string twoFactorAuthenticationCode = null,
            Uri baseAddress = null,
            Func<object, object> preprocessResponseBody = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri
            };

            return SendDataInternal<T>(body, jsonTypeInfo, accepts, contentType, cancellationToken, twoFactorAuthenticationCode, request, preprocessResponseBody);
        }
#else
        Task<IApiResponse<T>> SendData<T>(
            Uri uri,
            HttpMethod method,
            object body,
            string accepts,
            string contentType,
            CancellationToken cancellationToken,
            string twoFactorAuthenticationCode = null,
            Uri baseAddress = null,
            Func<object, object> preprocessResponseBody = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri
            };

            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, twoFactorAuthenticationCode, request, preprocessResponseBody);
        }
#endif

#if NET6_0_OR_GREATER
        Task<IApiResponse<T>> SendDataInternal<T>(
            T body,
            JsonTypeInfo<T> jsonTypeInfo,
            string accepts,
            string contentType,
            CancellationToken cancellationToken,
            string twoFactorAuthenticationCode,
            Request request,
            Func<object, object> preprocessResponseBody)
        {
            if (!string.IsNullOrEmpty(accepts))
            {
                request.Headers["Accept"] = accepts;
            }

            if (!string.IsNullOrEmpty(twoFactorAuthenticationCode))
            {
                request.Headers["X-GitHub-OTP"] = twoFactorAuthenticationCode;
            }

            if (body != null)
            {
                request.Body = body;
                // Default Content Type per: http://developer.github.com/v3/
                request.ContentType = contentType ?? "application/x-www-form-urlencoded";
            }

            return Run<T>(request, jsonTypeInfo, cancellationToken, preprocessResponseBody);
        }
#else
        Task<IApiResponse<T>> SendDataInternal<T>(object body, string accepts, string contentType, CancellationToken cancellationToken, string twoFactorAuthenticationCode, Request request, Func<object, object> preprocessResponseBody)
        {
            if (!string.IsNullOrEmpty(accepts))
            {
                request.Headers["Accept"] = accepts;
            }

            if (!string.IsNullOrEmpty(twoFactorAuthenticationCode))
            {
                request.Headers["X-GitHub-OTP"] = twoFactorAuthenticationCode;
            }

            if (body != null)
            {
                request.Body = body;
                // Default Content Type per: http://developer.github.com/v3/
                request.ContentType = contentType ?? "application/x-www-form-urlencoded";
            }

            return Run<T>(request, cancellationToken, preprocessResponseBody);
        }
#endif

        /// <inheritdoc cref="IConnection.Patch(Uri)" />
        public async Task<HttpStatusCode> Patch(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpVerb.Patch,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <inheritdoc cref="IConnection.Patch(Uri, string)" />
        public async Task<HttpStatusCode> Patch(Uri uri, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response = await SendData<object>(uri, new HttpMethod("PATCH"), null, accepts, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <inheritdoc cref="IConnection.Put(Uri)" />
        public async Task<HttpStatusCode> Put(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpMethod.Put,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

#if NETSTANDARD2_0
        /// <inheritdoc cref="IConnection.Put(Uri, object)" />
        public async Task<HttpStatusCode> Put(Uri uri, object body)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            var response = await SendData<object>(uri, HttpMethod.Put, body, null, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }
#endif

        /// <inheritdoc cref="IConnection.Delete(Uri)" />
        public async Task<HttpStatusCode> Delete(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpMethod.Delete,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <inheritdoc cref="IConnection.Delete(Uri, string)" />
        public async Task<HttpStatusCode> Delete(Uri uri, string twoFactorAuthenticationCode)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Delete, null, null, null, CancellationToken.None, twoFactorAuthenticationCode).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

#if NETSTANDARD2_0
        /// <inheritdoc cref="IConnection.Delete(Uri, object)" />
        public async Task<HttpStatusCode> Delete(Uri uri, object data)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(data, nameof(data));

            var request = new Request
            {
                Method = HttpMethod.Delete,
                Body = data,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <inheritdoc cref="IConnection.Delete(Uri, object, string)" />
        public async Task<HttpStatusCode> Delete(Uri uri, object data, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response = await SendData<object>(uri, HttpMethod.Delete, data, accepts, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }
#endif

        /// <inheritdoc cref="IConnection.Delete{T}(Uri, object)" />
        public Task<IApiResponse<T>> Delete<T>(Uri uri, object data)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(data, nameof(data));

            return SendData<T>(uri, HttpMethod.Delete, data, null, null, CancellationToken.None);
        }

        /// <inheritdoc cref="IConnection.Delete{T}(Uri, object, string)" />
        public Task<IApiResponse<T>> Delete<T>(Uri uri, object data, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            return SendData<T>(uri, HttpMethod.Delete, data, accepts, null, CancellationToken.None);
        }

        /// <inheritdoc cref="IConnection.BaseAddress" />
        public Uri BaseAddress { get; private set; }

        /// <summary>
        /// Gets the user agent used for all requests.
        /// </summary>
        public string UserAgent { get; private set; }

        /// <inheritdoc cref="IConnection.CredentialStore" />
        public ICredentialStore CredentialStore
        {
            get { return _authenticator.CredentialStore; }
        }

        /// <inheritdoc cref="IConnection.Credentials" />
        public Credentials Credentials
        {
            get
            {
                var credentialTask = CredentialStore.GetCredentials();
                if (credentialTask == null) return Credentials.Anonymous;
                return credentialTask.Result ?? Credentials.Anonymous;
            }
            // Note this is for convenience. We probably shouldn't allow this to be mutable.
            set
            {
                Ensure.ArgumentNotNull(value, nameof(value));
                _authenticator.CredentialStore = new InMemoryCredentialStore(value);
            }
        }

        /// <inheritdoc cref="IConnection.ResponseCache" />
        public IResponseCache ResponseCache
        {
            set
            {
                Ensure.ArgumentNotNull(value, nameof(value));
                _httpClient = new CachingHttpClient(_httpClient, value);
            }
        }

        async Task<IApiResponse<string>> GetHtml(IRequest request)
        {
            request.Headers.Add("Accept", AcceptHeaders.StableVersionHtml);
            var response = await RunRequest(request, CancellationToken.None).ConfigureAwait(false);
            return new ApiResponse<string>(response, response.Body as string);
        }

        async Task<IApiResponse<byte[]>> GetRaw(IRequest request)
        {
            request.Headers.Add("Accept", AcceptHeaders.RawContentMediaType);
            var response = await RunRequest(request, CancellationToken.None).ConfigureAwait(false);

            if (response.Body is Stream stream)
            {
                return new ApiResponse<byte[]>(response, await StreamToByteArray(stream));
            }

            return new ApiResponse<byte[]>(response, response.Body as byte[]);
        }

        async Task<IApiResponse<Stream>> GetRawStream(IRequest request)
        {
            request.Headers.Add("Accept", AcceptHeaders.RawContentMediaType);
            var response = await RunRequest(request, CancellationToken.None).ConfigureAwait(false);

            return new ApiResponse<Stream>(response, response.Body as Stream);
        }

        async Task<byte[]> StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

#if NET6_0_OR_GREATER
        async Task<IApiResponse<T>> Run<T>(IRequest request, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody = null)
        {
            _jsonPipeline.SerializeRequest(request, jsonTypeInfo);

            var response = await RunRequest(request, cancellationToken, preprocessResponseBody).ConfigureAwait(false);

            return _jsonPipeline.DeserializeResponse<T>(response, jsonTypeInfo);

        }
#else
        async Task<IApiResponse<T>> Run<T>(IRequest request, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody = null)
        {
            _jsonPipeline.SerializeRequest(request);

            var response = await RunRequest(request, cancellationToken, preprocessResponseBody).ConfigureAwait(false);

            return _jsonPipeline.DeserializeResponse<T>(response);
        }
#endif

        // THIS IS THE METHOD THAT EVERY REQUEST MUST GO THROUGH!
        async Task<IResponse> RunRequest(IRequest request, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody = null)
        {
            request.Headers.Add("User-Agent", UserAgent);
            await _authenticator.Apply(request).ConfigureAwait(false);
            var response = await _httpClient.Send(request, cancellationToken, preprocessResponseBody).ConfigureAwait(false);
            if (response != null)
            {
                // Use the clone method to avoid keeping hold of the original (just in case it effect the lifetime of the whole response
                _lastApiInfo = response.ApiInfo.Clone();
            }
            HandleErrors(response);
            return response;
        }

        static readonly Dictionary<HttpStatusCode, Func<IResponse, Exception>> _httpExceptionMap =
            new()
            {
                { HttpStatusCode.Unauthorized, GetExceptionForUnauthorized },
                { HttpStatusCode.Forbidden, GetExceptionForForbidden },
                { HttpStatusCode.NotFound, response => new NotFoundException(response) },
                { (HttpStatusCode)422, response => new ApiValidationException(response) },
                { (HttpStatusCode)451, response => new LegalRestrictionException(response) }
            };

        static void HandleErrors(IResponse response)
        {
            if (_httpExceptionMap.TryGetValue(response.StatusCode, out Func<IResponse, Exception> exceptionFunc))
            {
                throw exceptionFunc(response);
            }

            if ((int)response.StatusCode >= 400)
            {
                throw new ApiException(response);
            }
        }

        static Exception GetExceptionForUnauthorized(IResponse response)
        {
            var twoFactorType = ParseTwoFactorType(response);

            return twoFactorType == TwoFactorType.None
                ? new AuthorizationException(response)
                : new TwoFactorRequiredException(response, twoFactorType);
        }

        static Exception GetExceptionForForbidden(IResponse response)
        {
            string body = response.Body as string ?? "";

            if (body.Contains("rate limit exceeded"))
            {
                return new RateLimitExceededException(response);
            }

            if (body.Contains("secondary rate limit"))
            {
                return new SecondaryRateLimitExceededException(response);
            }

            if (body.Contains("number of login attempts exceeded"))
            {
                return new LoginAttemptsExceededException(response);
            }

            if (body.Contains("abuse-rate-limits") || body.Contains("abuse detection mechanism"))
            {
                return new AbuseException(response);
            }

            return new ForbiddenException(response);
        }

        internal static TwoFactorType ParseTwoFactorType(IResponse restResponse)
        {
            if (restResponse == null || restResponse.Headers == null || !restResponse.Headers.Any()) return TwoFactorType.None;
            var otpHeader = restResponse.Headers.FirstOrDefault(header =>
                header.Key.Equals("X-GitHub-OTP", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(otpHeader.Value)) return TwoFactorType.None;
            var factorType = otpHeader.Value;
#if NET6_0_OR_GREATER
            var parts = factorType.Split(';', StringSplitOptions.RemoveEmptyEntries);
#else
            var parts = factorType.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
#endif
            if (parts.Length > 0 && parts[0] == "required")
            {
                var secondPart = parts.Length > 1 ? parts[1].Trim() : null;
                return secondPart switch
                {
                    "sms" => TwoFactorType.Sms,
                    "app" => TwoFactorType.AuthenticatorApp,
                    _ => TwoFactorType.Unknown,
                };
            }
            return TwoFactorType.None;
        }

        static string FormatUserAgent(ProductHeaderValue productInformation)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1}; {2}; Octokit.net {3})",
                productInformation,
                GetPlatformInformation(),
                GetCultureInformation(),
                GetVersionInformation());
        }

        private static string _platformInformation;

        static string GetPlatformInformation()
        {
            if (string.IsNullOrEmpty(_platformInformation))
            {
                try
                {
                    _platformInformation = string.Format(CultureInfo.InvariantCulture,
                        "{0} {1}; {2}",
                        Environment.OSVersion.Platform,
                        Environment.OSVersion.Version.ToString(3),
                        Environment.Is64BitOperatingSystem ? "amd64" : "x86"
                    );
                }
                catch
                {
                    _platformInformation = "Unknown Platform";
                }
            }

            return _platformInformation;
        }

        static string GetCultureInformation()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        private static string _versionInformation;

        static string GetVersionInformation()
        {
            if (string.IsNullOrEmpty(_versionInformation))
            {
                _versionInformation = typeof(IGitHubClient)
                    .GetTypeInfo()
                    .Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
            }

            return _versionInformation;
        }

        /// <inheritdoc cref="IConnection.SetRequestTimeout(TimeSpan)" />
        public void SetRequestTimeout(TimeSpan timeout)
        {
            _httpClient.SetRequestTimeout(timeout);
        }

        public Task<IApiResponse<T>> Put<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Put<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string twoFactorAuthenticationCode)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Put<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string twoFactorAuthenticationCode, string accepts)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts, string contentType, IDictionary<string, string> parameters = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts, string contentType, string twoFactorAuthenticationCode, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts, string contentType, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts, string contentType, Uri baseAddress, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Delete<T>(Uri uri, T data, JsonTypeInfo<T> jsonTypeInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Delete<T>(Uri uri, T data, JsonTypeInfo<T> jsonTypeInfo, string accepts)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Patch<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse<T>> Patch<T>(Uri uri, T body, JsonTypeInfo<T> jsonTypeInfo, string accepts)
        {
            throw new NotImplementedException();
        }
    }
}
