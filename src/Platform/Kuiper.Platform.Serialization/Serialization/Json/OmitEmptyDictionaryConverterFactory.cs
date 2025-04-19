// <copyright file="OmitEmptyDictionaryConverterFactory.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Serialization.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class OmitEmptyDictionaryConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType &&
                   typeToConvert.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var keyType = typeToConvert.GetGenericArguments()[0];
            var valueType = typeToConvert.GetGenericArguments()[1];

            var converterType = typeof(OmitEmptyDictionaryConverter<,>).MakeGenericType(keyType, valueType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        private sealed class OmitEmptyDictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : notnull
        {
            public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(ref reader, options);

            public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options)
            {
                if (value != null && value.Count > 0)
                {
                    JsonSerializer.Serialize(writer, value, options);
                }
            }
        }
    }
}
