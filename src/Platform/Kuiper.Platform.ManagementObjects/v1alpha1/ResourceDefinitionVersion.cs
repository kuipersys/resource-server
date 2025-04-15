// <copyright file="ResourceDefinitionVersion.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework;

    public sealed class ResourceDefinitionVersion
    {
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("served")]
        public bool Served { get; set; } = true;

        [JsonPropertyName("storage")]
        public bool Storage { get; set; } = true;

        [JsonPropertyName("schema")]
        public ResourceDefinitionVersionSchema Schema { get; set; }

        [JsonPropertyName("resourceHandlerType")]
        public string ResourceOperatorType { get; set; }

        [JsonPropertyOrder(int.MaxValue)]
        [JsonExtensionData]
        public PropertyBag ExtensionData { get; set; } = new PropertyBag();

        public void SetResourceHandlerType(Type type)
        {
            this.ResourceOperatorType = type.FullName;
        }

        public NJsonSchema.JsonSchema GetJsonSchema()
        {
            if (this.Schema == null || this.Schema.OpenAPIV3Schema == null)
            {
                return null;
            }

            return SystemSchema.LoadSchema(this.Schema.OpenAPIV3Schema.Value);
        }
    }
}
