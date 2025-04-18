// <copyright file="SystemTextJsonExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Serialization.Serialization
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Kuiper.Platform.Serialization.Serialization.Json;

    public static class SystemTextJsonExtensions
    {
        public static string ObjectToJson<T>(this T obj, bool writeIndented = false)
            => JsonSerializer.Serialize(obj, obj.GetType(), GetJsonSerializerWriterOptions(writeIndented));

        public static Task ObjectToJsonAsync<T>(this T obj, Stream stream, bool writeIndented = false)
            => JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), GetJsonSerializerWriterOptions(writeIndented));

        public static string ObjectToJson(this object obj, Type type, bool writeIndented = false)
            => JsonSerializer.Serialize(obj, type, GetJsonSerializerWriterOptions(writeIndented));

        public static Task ObjectToJsonAsync(this object obj, Type type, Stream stream, bool writeIndented = false)
            => JsonSerializer.SerializeAsync(stream, obj, type, GetJsonSerializerWriterOptions(writeIndented));

        public static string ObjectToJson<T>(this T obj, JsonSerializerOptions options)
            => JsonSerializer.Serialize(obj, obj.GetType(), options ?? GetJsonSerializerWriterOptions(false));

        public static string ObjectToJson(this object obj, Type type, JsonSerializerOptions options)
            => JsonSerializer.Serialize(obj, type, options ?? GetJsonSerializerWriterOptions(false));

        public static T ObjectFromJson<T>(this string json, JsonSerializerOptions options = null)
            => JsonSerializer.Deserialize<T>(json, options ?? SerializationSettings.StandardReadOptions);

        public static T ObjectFromJson<T>(this Stream stream, JsonSerializerOptions options = null)
            => JsonSerializer.Deserialize<T>(stream, options ?? SerializationSettings.StandardReadOptions);

        public static ValueTask<T> ObjectFromJsonAsync<T>(this Stream stream, JsonSerializerOptions options = null)
            => JsonSerializer.DeserializeAsync<T>(stream, options ?? SerializationSettings.StandardReadOptions);

        public static T MarshalAs<T>(this object value, JsonSerializerOptions options = null)
        {
            if (typeof(T) == value.GetType())
            {
                return (T)value;
            }

            return value
                .ObjectToJson(options)
                .ObjectFromJson<T>(options);
        }

        public static T MarshalAs<T>(this object value, Type type, JsonSerializerOptions options = null)
        {
            if (typeof(T) == value.GetType())
            {
                return (T)value;
            }
            return value
                .ObjectToJson(type, options)
                .ObjectFromJson<T>(options);
        }

        public static JsonElement JsonBytesToJsonElement(this byte[] value, JsonDocumentOptions? options = null)
        {
            if (options == null)
            {
                options = new JsonDocumentOptions()
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip,
                    MaxDepth = 100,
                };
            }

            var document = JsonDocument.Parse(value, options.Value);

            return document.RootElement.Clone();
        }

        public static async Task<JsonElement> StreamToJsonElementAsync(this Stream stream, JsonDocumentOptions? options = null)
        {
            if (options == null)
            {
                options = new JsonDocumentOptions()
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip,
                    MaxDepth = 100,
                };
            }

            var document = await JsonDocument.ParseAsync(stream, options.Value);

            return document.RootElement.Clone();
        }

        public static JsonSerializerOptions GetJsonSerializerWriterOptions(bool writeIndented)
            => writeIndented ? SerializationSettings.IndentedWriteOptions : SerializationSettings.StandardWriteOptions;
    }
}
