using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

#if NET6_0_OR_GREATER
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit
{
    /// <summary>
    /// Error payload from the API response
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ApiError
    {
        public ApiError() { }

        public ApiError(string message)
        {
            Message = message;
        }

        public ApiError(string message, string documentationUrl, IReadOnlyList<ApiErrorDetail> errors)
        {
            Message = message;
            DocumentationUrl = documentationUrl;
            Errors = errors;
        }

        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// URL to the documentation for this error.
        /// </summary>
        public string DocumentationUrl { get; private set; }

        /// <summary>
        /// Additional details about the error
        /// </summary>
        public IReadOnlyList<ApiErrorDetail> Errors { get; private set; }

        internal string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Message: {0}", Message);
            }
        }
    }

    [JsonSerializable(typeof(ApiError))]
    internal partial class ApiErrorContext : JsonSerializerContext
    {
    }
}
