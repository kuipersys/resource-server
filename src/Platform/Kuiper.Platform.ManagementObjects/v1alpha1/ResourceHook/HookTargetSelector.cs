// <copyright file="HookTargetSelector.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.ManagementObjects.v1alpha1.ResourceHook
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public sealed class HookTargetSelector
    {
        [Required]
        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; } = default!;

        [Required]
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = default!;
    }
}
