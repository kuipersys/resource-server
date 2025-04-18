// <copyright file="SystemObjectMetadata.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    using NJsonSchema.Annotations;
    /// <summary>
    /// https://kubernetes.io/blog/2024/10/02/steering-committee-results-2024/
    /// </summary>
    [JsonSchemaIgnore]
    public class SystemObjectMetadata
    {
        [Required]
        [JsonPropertyOrder(0)]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyOrder(1)]
        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets the an internal unique identifier for the object.
        /// </summary>
        [JsonPropertyOrder(2)]
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyOrder(3)]
        [JsonPropertyName("creationTimestamp")]
        public long? CreationTimestamp { get; set; }

        [JsonPropertyOrder(4)]
        [JsonPropertyName("deletionTimestamp")]
        public long? DeletionTimestamp { get; set; }

        [JsonPropertyOrder(5)]
        [JsonPropertyName("resourceVersion")]
        public string? ResourceVersion { get; set; }

        [JsonPropertyOrder(6)]
        [JsonPropertyName("selfLink")]
        public string? SelfLink { get; set; }

        [JsonPropertyOrder(10)]
        [JsonPropertyName("labels")]
        public IDictionary<string, string>? Labels { get; set; }

        [JsonPropertyOrder(11)]
        [JsonPropertyName("annotations")]
        public IDictionary<string, string>? Annotations { get; set; }

        [JsonPropertyOrder(int.MaxValue)]
        [JsonExtensionData]
        public PropertyBag ExtensionData { get; set; } = new PropertyBag();
    }
}
