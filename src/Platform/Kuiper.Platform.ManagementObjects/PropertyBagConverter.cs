// <copyright file="PropertyBagConverter.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Serialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.ManagementObjects;

    internal class PropertyBagConverter : JsonConverter<PropertyBag>
    {
        public override PropertyBag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = new PropertyBag();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                reader.Read();

                if (propertyName == "$type")
                {
                    continue;
                }

                var value = JsonSerializer.Deserialize<object>(ref reader, options);

                if (value is JsonElement element)
                {
                    value = this.DeserializeKnownType(propertyName!, element, options);
                }

                result.Add(propertyName!, value);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, PropertyBag value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var item in value)
            {
                // If the item is an object, we need to include the type information
                if (item.Value is PropertyBag)
                {
                    writer.WritePropertyName(item.Key);
                    JsonSerializer.Serialize(writer, item.Value, item.Value.GetType(), options);
                    continue;
                }

                writer.WritePropertyName(item.Key);
                JsonSerializer.Serialize(writer, item.Value, options);
            }

            writer.WriteEndObject();
        }

        private object DeserializeKnownType(string propertyName, JsonElement jsonElement, JsonSerializerOptions options)
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
            {
                return jsonElement;
            }

            var typeName = jsonElement.GetProperty("$type").GetString();
            var knownType = KnownTypeCache.ResolveType(typeName!);

            if (knownType == null)
            {
                throw new JsonException($"Unknown type: {typeName ?? "<null>"}");
            }

            return jsonElement.Deserialize(knownType, options)!;
        }
    }
}
