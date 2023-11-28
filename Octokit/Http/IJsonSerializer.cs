#if NET6_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace Octokit.Internal
{
    /// <summary>
    /// A JSON serializer used to serialize and deserialize GitHub API data.
    /// </summary>
    public interface IJsonSerializer
    {
#if NETSTANDARD2_0
        /// <summary>
        /// Serializes the given object into a JSON <see langword="string" />.
        /// </summary>
        /// <param name="item">The item to serialize as a JSON <see langword="string" />.</param>
        /// <returns>The JSON <see langword="string" /> representation of the serialized <paramref name="item"/>.</returns>
        string Serialize(object item);

        /// <summary>
        /// Deserializes the JSON <see langword="string" /> into an object of the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The generic-type to deserialize to.</typeparam>
        /// <param name="json">The given JSON <see langword="string" /> value to deserialize from.</param>
        /// <returns>An instance of type <typeparamref name="T"/>.</returns>
        T Deserialize<T>(string json);
#else
        /// <summary>
        /// Serializes the given object into a JSON <see langword="string" />.
        /// </summary>
        /// <typeparam name="T">The generic-type to deserialize from.</typeparam>
        /// <param name="item">The item to serialize as a JSON <see langword="string" />.</param>
        /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
        /// <returns>The JSON <see langword="string" /> representation of the serialized <paramref name="item"/>.</returns>
        string Serialize<T>(T item, JsonTypeInfo<T> jsonTypeInfo);

        /// <summary>
        /// Deserializes the JSON <see langword="string" /> into an object of the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The generic-type to deserialize to.</typeparam>
        /// <param name="json">The given JSON <see langword="string" /> value to deserialize from.</param>
        /// <param name="jsonTypeInfo">The JSON type information for the given <typeparamref name="T"/> type.</param>
        /// <returns>An instance of type <typeparamref name="T"/>.</returns>
        T Deserialize<T>(string json, JsonTypeInfo<T> jsonTypeInfo);
#endif
    }
}
