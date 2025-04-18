// <copyright file="ResourceDefinitionSpec.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1.Resource
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public sealed class ResourceDefinitionSpec
    {
        [Required]
        [JsonPropertyName("group")]
        public string Group { get; set; }

        [Required]
        [JsonPropertyName("names")]
        public ResourceDefinitionNames Names { get; set; } = new ResourceDefinitionNames();

        [Required]
        [JsonPropertyName("scope")]
        public ResourceScope Scope { get; set; }

        [Required]
        [JsonPropertyName("versions")]
        public ResourceDefinitionVersion[] Versions { get; set; }
    }
}
