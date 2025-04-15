// <copyright file="ResourceDefinitionVersionSchema.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public sealed class ResourceDefinitionVersionSchema
    {
        [JsonPropertyName("openAPIV3Schema")]
        public JsonElement? OpenAPIV3Schema { get; set; }
    }
}
