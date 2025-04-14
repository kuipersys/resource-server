// <copyright file="SchemaExtensions.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Utilities.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;

    using Kuiper.Platform.Utilities.Serialization;

    using NJsonSchema;
    using NJsonSchema.Generation;

    public static class SchemaExtensions
    {
        public static JsonSchema ObjectTypeToJsonSchema<T>()
            => typeof(T).ObjectTypeToJsonSchema();

        public static JsonSchema ObjectTypeToJsonSchema(this Type type)
        {
            var schema = JsonSchemaGenerator.FromType(type, new SystemTextJsonSchemaGeneratorSettings()
            {
                SchemaType = SchemaType.OpenApi3,
                UseXmlDocumentation = true,
                GenerateAbstractSchemas = false,
                IgnoreObsoleteProperties = true,
                FlattenInheritanceHierarchy = true
            });

            schema.AllowAdditionalProperties = true;

            return schema;
        }

        private static string FixSchemaReferenceEmission(this string json)
        {
            var temp = json.ObjectFromJson<IDictionary<string, object>>();

            if (temp.ContainsKey("$schema"))
            {
                temp.Remove("$schema");
            }

            return temp.ObjectToJson();
        }

        public static JsonElement SchemaToJsonElement(this JsonSchema schema, bool removeSchemaReferences = false)
        {
            var schemaJsonBytes = Encoding.UTF8.GetBytes(
                removeSchemaReferences ?
                    schema.ToJson().FixSchemaReferenceEmission() :
                    schema.ToJson());

            return schemaJsonBytes.JsonBytesToJsonElement();
        }
    }
}
