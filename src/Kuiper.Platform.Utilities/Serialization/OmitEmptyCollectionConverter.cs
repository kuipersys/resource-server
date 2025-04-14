// <copyright file="OmitEmptyCollectionConverter.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Utilities.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class OmitEmptyCollectionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IEnumerable).IsAssignableFrom(typeToConvert)
                && typeToConvert != typeof(string)
                && !IsDictionaryType(typeToConvert);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var elementType = typeToConvert.IsGenericType
                ? typeToConvert.GetGenericArguments()[0]
                : typeof(object);

            var converterType = typeof(OmitEmptyCollectionConverter<>).MakeGenericType(elementType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        private static bool IsDictionaryType(Type type) =>
            type.IsGenericType && (
                type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
    }

    public class OmitEmptyCollectionConverter<T> : JsonConverter<IEnumerable<T>>
    {
        public override IEnumerable<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize<List<T>>(ref reader, options);

        public override void Write(Utf8JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
        {
            if (value != null && value.Any())
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }

}
