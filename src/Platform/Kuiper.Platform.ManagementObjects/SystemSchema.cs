// <copyright file="SystemSchema.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using NJsonSchema.Generation;

    public static class SystemSchema
    {
        public class FlexibleBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.True => true,
                    JsonTokenType.False => false,
                    JsonTokenType.String => bool.TryParse(reader.GetString(), out var value) && value,
                    _ => throw new JsonException("Invalid boolean value.")
                };
            }

            public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            {
                writer.WriteBooleanValue(value); // serialize as true/false, not a string
            }
        }

        private static SystemTextJsonSchemaGeneratorSettings GetSchemaSerializerSettings()
        {
            return new SystemTextJsonSchemaGeneratorSettings()
            {
                SchemaType = NJsonSchema.SchemaType.OpenApi3,
                FlattenInheritanceHierarchy = true,
                UseXmlDocumentation = true,
                GenerateEnumMappingDescription = true,
                SerializerOptions = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter(),
                        new FlexibleBooleanJsonConverter(),
                    },
                },
            };
        }

        public static NJsonSchema.JsonSchema? GetSchema<T>()
            where T : SystemObject
        {
            var schema = JsonSchemaGenerator.FromType(typeof(T), GetSchemaSerializerSettings());

            return schema;
        }

        public static NJsonSchema.JsonSchema? LoadSchema(JsonElement schema)
        {
            var conversionTask = NJsonSchema.JsonSchema.FromJsonAsync(schema.GetRawText());
            conversionTask.Wait();

            return conversionTask.Result;
        }

        public static JsonElement GetSchemaAsJsonElement<T>()
            where T : SystemObject
        {
            var schemaJson = GetSchema<T>()?.ToJson();

            return JsonSerializer.Deserialize<JsonElement>(schemaJson!);
        }
    }
}
