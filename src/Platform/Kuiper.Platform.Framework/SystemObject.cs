// <copyright file="SystemObject.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Framework
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    using Kuiper.Platform.Framework.Abstractions.Objects;

    using NJsonSchema.Annotations;

    [JsonSchemaIgnore]
    [JsonDerivedType(typeof(SystemObject), nameof(SystemObject))]
    public class SystemObject : ISystemObject
    {
        [Required]
        [JsonPropertyOrder(0)]
        [JsonPropertyName("apiVersion")]
        public string? ApiVersion { get; set; }

        [Required]
        [JsonPropertyOrder(1)]
        [JsonPropertyName("kind")]
        public string? Kind { get; set; }

        [Required]
        [JsonPropertyOrder(2)]
        [JsonPropertyName("metadata")]
        public SystemObjectMetadata Metadata { get; set; }
            = new SystemObjectMetadata();

        [JsonPropertyOrder(int.MaxValue)]
        [JsonExtensionData]
        public PropertyBag ExtensionData { get; set; } = new PropertyBag();
    }
}
