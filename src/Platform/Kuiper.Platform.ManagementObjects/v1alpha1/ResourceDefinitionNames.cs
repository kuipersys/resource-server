// <copyright file="ResourceDefinitionNames.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public sealed class ResourceDefinitionNames
    {
        [Required]
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [Required]
        [JsonPropertyName("singular")]
        public string Singular { get; set; }

        [Required]
        [JsonPropertyName("plural")]
        public string Plural { get; set; }

        [JsonPropertyName("shortNames")]
        public string[] ShortNames { get; set; }
    }
}
