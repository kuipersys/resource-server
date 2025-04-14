// <copyright file="SystemObjectRef.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Sdk
{
    using System.Text.Json.Serialization;

    [JsonDerivedType(typeof(SystemObjectRef), nameof(SystemObjectRef))]
    public class SystemObjectRef
    {
        [JsonPropertyOrder(10)]
        public string? ApiVersion { get; set; }

        [JsonPropertyOrder(11)]
        public string? Namespace { get; set; }

        [JsonPropertyOrder(12)]
        public string? Kind { get; set; }

        [JsonPropertyOrder(13)]
        public string? Name { get; set; }

        [JsonPropertyOrder(14)]
        public string? Uid { get; set; }

        [JsonPropertyOrder(15)]
        public string? Version { get; set; }

        [JsonPropertyOrder(16)]
        public string? Link { get; set; }

        public string ToResourceId()
        {
            string resourceId = $"{this.Kind}/{this.Name}";

            if (!string.IsNullOrWhiteSpace(this.Namespace))
            {
                resourceId = $"{this.Namespace}/{this.ApiVersion}/{resourceId}";
            }
            else
            {
                resourceId = $"default/{this.ApiVersion}/{resourceId}";
            }

            return resourceId;
        }
    }
}
