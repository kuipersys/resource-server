// <copyright file="ResourceDefinitionVersion.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework.Abstractions.Objects;
    using Kuiper.Platform.ManagementObjects;

    /// <summary>
    /// Represents a version of a resource definition. Used to define the schema and other properties of a resource.
    /// </summary>
    public sealed class ResourceDefinitionVersion : IResourceDefinitionVersion
    {
        /// <summary>
        /// Gets or sets the name of the resource definition version.
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the resource definition version is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the schema for the resource definition version.
        /// </summary>
        [JsonPropertyName("schema")]
        public ResourceDefinitionVersionSchema Schema { get; set; }

        /// <summary>
        /// Gets or sets an optional field that allows a remote service to be specified for handling.
        /// </summary>
        [JsonPropertyName("operationEndpoint")]
        public string? OperationEndpoint { get; set; }

        [JsonPropertyOrder(int.MaxValue)]
        [JsonExtensionData]
        public PropertyBag ExtensionData { get; set; } = new PropertyBag();

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
